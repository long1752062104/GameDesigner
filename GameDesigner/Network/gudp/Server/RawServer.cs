namespace Net.Server
{
    using Net.Share;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Runtime.InteropServices;
    using global::System.Text;
    using global::System.Threading;
    using Debug = Event.NDebug;
    using Net.System;

    [StructLayout(LayoutKind.Explicit)]
    public struct IPHeader
    {
        [FieldOffset(0)] public byte ip_verlen; //I4位首部长度+4位IP版本号 
        [FieldOffset(1)] public byte ip_tos; //8位服务类型TOS 
        [FieldOffset(2)] public ushort ip_totallength; //16位数据包总长度（字节） 
        [FieldOffset(4)] public ushort ip_id; //16位标识 
        [FieldOffset(6)] public ushort ip_offset; //3位标志位 
        [FieldOffset(8)] public byte ip_ttl; //8位生存时间 TTL 
        [FieldOffset(9)] public byte ip_protocol; //8位协议(TCP, UDP, ICMP, Etc.) 
        [FieldOffset(10)] public ushort ip_checksum; //16位IP首部校验和 
        [FieldOffset(12)] public uint ip_srcaddr; //32位源IP地址 
        [FieldOffset(16)] public uint ip_destaddr; //32位目的IP地址 
        [FieldOffset(20)] public ushort srcPort; //源端口
        [FieldOffset(22)] public ushort dstPort; //目的地端口

        public override string ToString()
        {
            return $"源地址:{new IPAddress(ip_srcaddr)}:{srcPort} 目的地:{new IPAddress(ip_destaddr)}:{dstPort}";
        }
    }

    /// <summary>
    /// 原始Udp服务器, 内部处理了IP头部
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    [Obsolete("弃用")]
    public class RawServer<Player, Scene> : ServerBase<Player, Scene> where Player : RawPlayer, new() where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="port">端口</param>
        public override void Start(ushort port = 6666)
        {
            if (Server != null)//如果服务器套接字已创建
                throw new Exception("服务器已经运行，不可重新启动，请先关闭后在重启服务器");
            Port = port;
            OnStartingHandle += OnStarting;
            OnStartupCompletedHandle += OnStartupCompleted;
            OnHasConnectHandle += OnHasConnect;
            OnRemoveClientHandle += OnRemoveClient;
            OnOperationSyncHandle += OnOperationSync;
            OnRevdBufferHandle += OnReceiveBuffer;
            OnReceiveFileHandle += OnReceiveFile;
            OnRevdRTProgressHandle += OnRevdRTProgress;
            OnSendRTProgressHandle += OnSendRTProgress;
            if (OnAddRpcHandle == null) OnAddRpcHandle = AddRpcInternal;//在start之前就要添加你的委托
            if (OnRemoveRpc == null) OnRemoveRpc = RemoveRpcInternal;
            if (OnRPCExecute == null) OnRPCExecute = OnRpcExecuteInternal;
            if (OnSerializeRPC == null) OnSerializeRPC = OnSerializeRpcInternal;
            if (OnDeserializeRPC == null) OnDeserializeRPC = OnDeserializeRpcInternal;
            if (OnSerializeOPT == null) OnSerializeOPT = OnSerializeOptInternal;
            if (OnDeserializeOPT == null) OnDeserializeOPT = OnDeserializeOptInternal;
            Debug.LogHandle += Log;
            Debug.LogWarningHandle += Log;
            Debug.LogErrorHandle += Log;
            OnStartingHandle();
            if (Instance == null)
                Instance = this;
            AddRpcHandle(this, true);
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP)
            {
                Blocking = false
            };
            Server.Bind(new IPEndPoint(IPAddress.Parse(NetPort.GetIP()), port));
            Server.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, 1);
            byte[] IN = new byte[4] { 1, 0, 0, 0 };
            byte[] OUT = new byte[4];
            int SIO_R = unchecked((int)0x98000001);//监听所有的数据包
            int ret_code = Server.IOControl(SIO_R, IN, OUT);//接收所有IP数据包, bing的ip不能是127.0.0.1
            ret_code = OUT[0] + OUT[1] + OUT[2] + OUT[3];//把4个8位字节合成一个32位整数
            if (ret_code != 0)
                throw new Exception("设置低级别操作失败!");
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            Server.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);//udp远程关闭现有连接方案
            IsRunServer = true;
            Thread proRevd = new Thread(ProcessReceive) { IsBackground = true, Name = "ProcessReceive" };
            proRevd.Start();
            Thread send = new Thread(SendDataHandle) { IsBackground = true, Name = "SendDataHandle" };
            send.Start();
            Thread hupdate = new Thread(CheckHeartHandle) { IsBackground = true, Name = "HeartUpdate" };
            hupdate.Start();
            Thread dtt = new Thread(DataTrafficThread) { IsBackground = true, Name = "DataTrafficThread" };
            dtt.Start();
            Thread suh = new Thread(SceneUpdateHandle) { IsBackground = true, Name = "SceneUpdateHandle" };
            suh.Start();
            Thread vsh = new Thread(VarSyncHandler) { IsBackground = true, Name = "VarSyncHandler" };
            vsh.Start();
            for (int i = 0; i < MaxThread; i++)
            {
                QueueSafe<RevdDataBuffer> revdDataBeProcessed = new QueueSafe<RevdDataBuffer>();
                RevdDataBeProcesseds.Add(revdDataBeProcessed);
                Thread revd = new Thread(RevdDataHandle) { IsBackground = true, Name = "RevdDataHandle" + i };
                revd.Start(revdDataBeProcessed);
                threads.Add("RevdDataHandle" + i, revd);
                QueueSafe<SendDataBuffer> sendDataBeProcessed = new QueueSafe<SendDataBuffer>();
                SendDataBeProcesseds.Add(sendDataBeProcessed);
                Thread proSend = new Thread(ProcessSend) { IsBackground = true, Name = "ProcessSend" + i };
                proSend.Start(sendDataBeProcessed);
                threads.Add("ProcessSend" + i, proSend);
            }
            threads.Add("ProcessReceive", proRevd);
            threads.Add("SendDataHandle", send);
            threads.Add("HeartUpdate", hupdate);
            threads.Add("DataTrafficThread", dtt);
            threads.Add("SceneUpdateHandle", suh);
            threads.Add("VarSyncHandler", vsh);
            KeyValuePair<string, Scene> scene = OnAddDefaultScene();
            MainSceneName = scene.Key;
            scene.Value.Name = MainSceneName;
            Scenes.TryAdd(scene.Key, scene.Value);
            scene.Value.onSerializeOptHandle = OnSerializeOpt;
            OnStartupCompletedHandle();
#if WINDOWS
            Win32KernelAPI.timeBeginPeriod(1);
#endif
        }

        protected unsafe virtual void ProcessReceive()
        {
            while (IsRunServer)
            {
                try
                {
                    if (!Server.Poll(0, SelectMode.SelectRead))
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    var buffer = BufferPool.Take();
                    int count = Server.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    IPHeader* head;
                    fixed (byte* fixed_buf = &buffer.Buffer[0])
                        head = (IPHeader*)fixed_buf;
                    if (head->ip_protocol != 17)//udp协议
                        continue;
                    ushort srcPort = BitConverter.ToUInt16(new byte[] { buffer[21], buffer[20] }, 0);
                    ushort desPort = BitConverter.ToUInt16(new byte[] { buffer[23], buffer[22] }, 0);
                    IPAddress srcAddress = new IPAddress(head->ip_srcaddr);
                    if (desPort != Port)
                        continue;
                    IPEndPoint remotePoint = new IPEndPoint(srcAddress, srcPort);
                    receiveCount += count;
                    receiveAmount++;
                    ReceiveProcessed(remotePoint, buffer, count, false);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
            }
        }

        protected override void ReceiveProcessed(EndPoint remotePoint, Segment buffer, int count, bool tcp_udp)
        {
            if (!AllClients.TryGetValue(remotePoint, out Player client))//在线客户端  得到client对象
            {
                if (ignoranceNumber >= LineUp)//排队人数
                {
                    exceededNumber++;
                    OnExceededNumber(remotePoint);
                    return;
                }
                if (onlineNumber >= OnlineLimit)//服务器最大在线人数
                {
                    blockConnection++;
                    OnBlockConnection(remotePoint);
                    return;
                }
                exceededNumber = 0;
                blockConnection = 0;
                int uid = UserIDNumber;
                UserIDNumber++;
                client = ObjectPool<Player>.Take();
                client.UserID = uid;
                client.playerID = uid.ToString();
                client.RemotePoint = remotePoint;
                client.LastTime = DateTime.Now.AddMinutes(5);
                client.isDispose = false;
                client.CloseSend = false;
                client.IPHeader = new byte[28];
                Buffer.BlockCopy(buffer, 0, client.IPHeader, 0, 28);
                byte b1 = client.IPHeader[20];
                byte b2 = client.IPHeader[21];
                byte b3 = client.IPHeader[22];
                byte b4 = client.IPHeader[23];
                client.IPHeader[20] = b3;
                client.IPHeader[21] = b4;
                client.IPHeader[22] = b1;
                client.IPHeader[23] = b2;
                b1 = client.IPHeader[12];
                b2 = client.IPHeader[13];
                b3 = client.IPHeader[14];
                b4 = client.IPHeader[15];
                client.IPHeader[12] = client.IPHeader[16];
                client.IPHeader[13] = client.IPHeader[17];
                client.IPHeader[14] = client.IPHeader[18];
                client.IPHeader[15] = client.IPHeader[19];
                client.IPHeader[16] = b1;
                client.IPHeader[17] = b2;
                client.IPHeader[18] = b3;
                client.IPHeader[19] = b4;
                OnHasConnectHandle(client);
                AllClients.TryAdd(remotePoint, client);
                Interlocked.Increment(ref ignoranceNumber);
                client.revdDataBeProcessed = RevdDataBeProcesseds[threadNum];
                client.sendDataBeProcessed = SendDataBeProcesseds[threadNum];
                if (++threadNum >= RevdDataBeProcesseds.Count)
                    threadNum = 0;
            }
            client.revdDataBeProcessed.Enqueue(new RevdDataBuffer() { client = client, buffer = buffer, index = 28, count = count, tcp_udp = tcp_udp });
        }

        protected override bool IsInternalCommand(Player client, RPCModel model)
        {
            if (model.cmd == NetCmd.Connect)
            {
                byte[] buffer = new byte[28 + frame + 6];
                Buffer.BlockCopy(client.IPHeader, 0, buffer, 0, 28);
                buffer[28 + 7] = NetCmd.Connect;
                Server.SendTo(buffer, client.RemotePoint);
                return true;
            }
            if (model.cmd == NetCmd.Broadcast)
            {
                string hostName = Dns.GetHostName();
                IPHostEntry iPHostEntry = Dns.GetHostEntry(hostName);
                IPAddress ipAddress = IPAddress.Any;
                foreach (IPAddress ipAdd in iPHostEntry.AddressList)
                    if (ipAdd.AddressFamily.ToString() == "InterNetwork")
                        ipAddress = ipAdd;
                byte[] address = Encoding.Unicode.GetBytes(ipAddress.ToString());
                byte[] buffer = new byte[28 + address.Length];
                Buffer.BlockCopy(client.IPHeader, 0, buffer, 0, 28);
                Buffer.BlockCopy(address, 0, buffer, 28, address.Length);
                Server.SendTo(buffer, client.RemotePoint);
                return true;
            }
            return false;
        }

        protected override void SendRTDataHandle(Player client, QueueSafe<RPCModel> rtRPCModels)
        {
            if (RTOMode == RTOMode.Fixed)
                client.currRto = RTO;//一秒最快发送50次 rto (重传超时时间) 如果网络拥堵, 别耽误事, 赶紧重传
            int rtcount = client.sendRTList.Values.Count;
            if ((rtcount > 0 & Seqencing) | (rtcount > 0 & client.sendRTListCount > 5) | rtcount > 100)// mtu(1400) * 5
                goto JUMP;
            int count = rtRPCModels.Count;
            if (count == 0 & rtcount > 0)
                goto JUMP;
            if (count == 0)
                return;
            if (count > 1000)
                count = 1000;
            var stream = BufferPool.Take();
            WriteDataBody(client, ref stream, rtRPCModels, count, true);
            int len = stream.Position;
            int index = 0;
            ushort dataIndex = 0;
            float dataCount = (float)len / MTU;
            var rtDic = new MyDictionary<ushort, RTBuffer>();
            client.sendRTList.Add(client.sendReliableFrame, rtDic);
            var stream1 = BufferPool.Take();
            int crcIndex = RandomHelper.Range(0, 256);
            byte crcCode = CRCCode[crcIndex];
            stream1.Write(client.IPHeader, 0, 28);
            stream1.Position += 4;
            stream1.WriteByte((byte)crcIndex);
            stream1.WriteByte(crcCode);
            stream1.WriteByte(74);
            stream1.WriteByte(NetCmd.ReliableTransport);
            while (index < len)
            {
                int count1 = MTU;
                if (index + count1 >= len)
                    count1 = len - index;
                stream1.SetPositionLength(36);
                stream1.Write(BitConverter.GetBytes(count1 + 16), 0, 4);
                stream1.Write(BitConverter.GetBytes(dataIndex), 0, 2);
                stream1.Write(BitConverter.GetBytes((ushort)Math.Ceiling(dataCount)), 0, 2);
                stream1.Write(BitConverter.GetBytes(count1), 0, 4);
                stream1.Write(BitConverter.GetBytes(len), 0, 4);
                stream1.Write(BitConverter.GetBytes(client.sendReliableFrame), 0, 4);
                stream1.Write(stream, index, count1);
                byte[] buffer = SendData(stream1);
                rtDic.Add(dataIndex, new RTBuffer(buffer));
                index += MTU;
                dataIndex++;
            }
            BufferPool.Push(stream);
            BufferPool.Push(stream1);
            client.sendRTListCount = rtDic.Count;
            client.sendReliableFrame++;
        JUMP:
            count = client.ackQueue.Count;
            for (int i = 0; i < count; i++)
            {
                if (!client.ackQueue.TryDequeue(out AckQueue ack))
                    continue;
                if (!client.sendRTList.ContainsKey(ack.frame))
                    continue;
                var rtlist = client.sendRTList[ack.frame];
                if (!rtlist.ContainsKey(ack.index))
                    continue;
                rtlist.Remove(ack.index);
                if (rtlist.Count == 0)
                    client.sendRTList.Remove(ack.frame);
                InvokeSendRTProgress(client, client.sendRTListCount - rtlist.Count, client.sendRTListCount);
            }
            int bytesLen = 0;
            var entries = client.sendRTList.entries;
            for (int i = 0; i < client.sendRTList.count; i++)
            {
                if (entries[i].hashCode >= 0)
                {
                    var rtlist = entries[i].value;
                    if (rtlist == null)//出现null, 则为客户端已经被Dispose
                        return;
                    foreach (var list in rtlist)
                    {
                        RTBuffer rtb = list.Value;
                        if (DateTime.Now < rtb.time)
                            continue;
                        rtb.time = DateTime.Now.AddMilliseconds(client.currRto);
                        bytesLen += rtb.buffer.Length;
                        SendByteData(client, rtb.buffer, true);
                        if (bytesLen > MTPS / 1000)//一秒最大发送1m数据, 这里会有可能每秒执行1000次
                            return;
                    }
                }
            }
        }

        protected override void SendDataHandle(Player client, QueueSafe<RPCModel> rPCModels, bool reliable)
        {
            int count = rPCModels.Count;//源码中Count执行也不少, 所以优化一下   这里已经取出要处理的长度
            if (count <= 0)
                return;
            var stream = BufferPool.Take();
            stream.Write(client.IPHeader, 0, 28);
            WriteDataHead(stream);
            WriteDataBody(client, ref stream, rPCModels, count, reliable);
            byte[] buffer = SendData(stream);
            SendByteData(client, buffer, reliable);
            BufferPool.Push(stream);
        }

        protected override byte[] SendData(Segment stream)
        {
            if (ByteCompression & stream.Count > 1000)
            {
                int oldlen = stream.Count;
                byte[] array = new byte[oldlen - frame];
                Buffer.BlockCopy(stream.Buffer, frame, array, 0, array.Length);
                byte[] buffer = UnZipHelper.Compress(array);
                stream.Position = 0;
                int len = buffer.Length;
                stream.Write(BitConverter.GetBytes(len), 0, 4);
                stream.SetLength(frame);
                stream.Position = frame;
                stream.Write(buffer, 0, len);
                buffer = stream.ToArray();
                return buffer;
            }
            else
            {
                stream.Position = 28;
                int len = stream.Count - (28 + frame);
                stream.Write(BitConverter.GetBytes(len), 0, 4);
                stream.Position = len + 28 + frame;
                return stream.ToArray();
            }
        }
    }
}