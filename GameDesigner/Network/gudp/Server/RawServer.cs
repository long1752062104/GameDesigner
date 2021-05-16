namespace Net.Server
{
    using Net.Share;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Debug = Event.NDebug;

    /// <summary>
    /// 原始Udp服务器, 内部处理了IP头部
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
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
            Server.Bind(new IPEndPoint(IPAddress.Any, port));
#if !UNITY_ANDROID//在安卓启动服务器时忽略此错误
            Server.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, 1);
            //byte[] IN = new byte[4] { 1, 0, 0, 0 };
            //byte[] OUT = new byte[4];
            //int SIO_R = unchecked((int)0x98000001);
            //int ret_code = Server.IOControl(SIO_R, IN, OUT);//接收所有IP数据包, bing的ip不能是127.0.0.1
            //ret_code = OUT[0] + OUT[1] + OUT[2] + OUT[3];//把4个8位字节合成一个32位整数
            //if (ret_code != 0)
            //    throw new IOException("设置低级别操作失败!");
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            Server.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);//udp远程关闭现有连接方案
#endif
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
                    byte temp_protocol = buffer[9];
                    if (temp_protocol != 17)//udp协议
                        continue;
                    ushort OriginationPort = BitConverter.ToUInt16(new byte[] { buffer[21], buffer[20] }, 0);
                    ushort DestinationPort = BitConverter.ToUInt16(new byte[] { buffer[23], buffer[22] }, 0);
                    IPAddress OriginationAddress = new IPAddress(BitConverter.ToUInt32(buffer, 12));
                    if (DestinationPort != Port)
                        continue;
                    IPEndPoint remotePoint = new IPEndPoint(OriginationAddress, OriginationPort);
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
            int len;
            var bufferPool = BufferPool.Take();
            using (MemoryStream stream1 = new MemoryStream(bufferPool))
            {
                stream1.SetLength(0);
                for (int i = 0; i < count; i++)
                {
                    if (!rtRPCModels.TryDequeue(out RPCModel rpc1))
                        continue;
                    if (rpc1.kernel & rpc1.serialize)
                        rpc1.buffer = OnSerializeRpc(rpc1);
                    stream1.WriteByte((byte)(rpc1.kernel ? 68 : 74));
                    stream1.WriteByte(rpc1.cmd);
                    stream1.Write(BitConverter.GetBytes(rpc1.buffer.Length), 0, 4);
                    stream1.Write(rpc1.buffer, 0, rpc1.buffer.Length);
                    if (stream1.Length > ushort.MaxValue)
                        break;
                }
                len = (int)stream1.Position;
            }
            int index = 0;
            ushort dataIndex = 0;
            float dataCount = (float)len / MTU;
            var rtDic = new MyDictionary<ushort, RTBuffer>();
            client.sendRTList.Add(client.sendReliableFrame, rtDic);
            var bufferPool1 = BufferPool.Take();
            using (MemoryStream stream = new MemoryStream(bufferPool1))
            {
                while (index < len)
                {
                    int count1 = MTU;
                    if (index + count1 >= len)
                        count1 = len - index;
                    stream.SetLength(0);
                    stream.Write(client.IPHeader, 0, 28);
                    int crcIndex = RandomHelper.Range(0, 256);
                    byte crcCode = CRCCode[crcIndex];
                    stream.Write(new byte[4], 0, 4);
                    stream.WriteByte((byte)crcIndex);
                    stream.WriteByte(crcCode);

                    stream.WriteByte(74);
                    stream.WriteByte(NetCmd.ReliableTransport);
                    stream.Write(BitConverter.GetBytes(count1 + 16), 0, 4);

                    stream.Write(BitConverter.GetBytes(dataIndex), 0, 2);
                    stream.Write(BitConverter.GetBytes((ushort)Math.Ceiling(dataCount)), 0, 2);
                    stream.Write(BitConverter.GetBytes(count1), 0, 4);
                    stream.Write(BitConverter.GetBytes(len), 0, 4);
                    stream.Write(BitConverter.GetBytes(client.sendReliableFrame), 0, 4);
                    stream.Write(bufferPool, index, count1);

                    byte[] buffer2 = SendData(client, stream);
                    rtDic.Add(dataIndex, new RTBuffer(buffer2));

                    index += MTU;
                    dataIndex++;
                }
            }
            BufferPool.Push(bufferPool);
            BufferPool.Push(bufferPool1);
            client.sendRTListCount = client.sendRTList[client.sendReliableFrame].Count;
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
                        continue;
                    foreach (var list in rtlist)
                    {
                        RTBuffer buffer3 = list.Value;
                        if (DateTime.Now < buffer3.time)
                            continue;
                        buffer3.time = DateTime.Now.AddMilliseconds(client.currRto);
                        bytesLen += buffer3.buffer.Length;
                        SendByteData(client, buffer3.buffer, true);
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
            var segment = BufferPool.Take();
            using (MemoryStream stream = new MemoryStream(segment))
            {
                stream.SetLength(0);
                stream.Write(client.IPHeader, 0, 28);
                int crcIndex = RandomHelper.Range(0, 256);
                byte crcCode = CRCCode[crcIndex];
                stream.Write(new byte[4], 0, 4);
                stream.WriteByte((byte)crcIndex);
                stream.WriteByte(crcCode);
                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    if (!rPCModels.TryDequeue(out RPCModel rPCModel))
                        continue;
                    if (rPCModel.kernel & rPCModel.serialize)
                        rPCModel.buffer = OnSerializeRpc(rPCModel);
                    int num = (int)stream.Length + rPCModel.buffer.Length + frame + 28;
                    if (num > BufferPool.Size)
                    {
                        BufferPool.Push(segment);
                        Debug.LogError($"内存已经超出范围({num}/{BufferPool.Size}), 如果需要发送大数据, 请设置BufferPool.Size的值!");
                        return;
                    }
                    if (num >= (IsEthernet ? MTU : 50000) | ++index >= 1000)//这里的判断是第二次for以上生效
                    {
                        byte[] buffer = SendData(client, stream);
                        SendByteData(client, buffer, reliable);
                        index = 0;
                        stream.SetLength(28 + frame);
                    }
                    stream.WriteByte((byte)(rPCModel.kernel ? 68 : 74));
                    stream.WriteByte(rPCModel.cmd);
                    stream.Write(BitConverter.GetBytes(rPCModel.buffer.Length), 0, 4);
                    stream.Write(rPCModel.buffer, 0, rPCModel.buffer.Length);
                    if (rPCModel.bigData)
                        break;
                }
                byte[] buffer1 = SendData(client, stream);
                SendByteData(client, buffer1, reliable);
            }
            BufferPool.Push(segment);
        }

        protected override byte[] SendData(Player client, MemoryStream stream)
        {
            if (ByteCompression & stream.Length > 1000)
            {
                int oldlen = (int)stream.Length;
                byte[] array = new byte[oldlen - frame];
                Buffer.BlockCopy(stream.GetBuffer(), frame, array, 0, array.Length);
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
                int len = (int)stream.Length - (28 + frame);
                stream.Write(BitConverter.GetBytes(len), 0, 4);
                stream.Position = len + 28 + frame;
                return stream.ToArray();
            }
        }
    }
}