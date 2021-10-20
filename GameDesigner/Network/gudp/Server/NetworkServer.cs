namespace Net.Server
{
    using Net.Share;
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Text;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Debug = Event.NDebug;
    using Net.System;

    /// <summary>
    /// 双通讯(tcp, udp)服务器 当使用Send发送数据时, 使用udp协议发送. 当使用SendRT发送数据时, 使用tcp协议发送
    /// 当Send发送数据大于50000字节后内部自动转换为tcp协议发送
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    /// <typeparam name="Player"></typeparam>
    /// <typeparam name="Scene"></typeparam>
    public class NetworkServer<Player, Scene> : ServerBase<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// UDP的IOCP套接字
        /// </summary>
        protected SocketAsyncEventArgs UdpSocketAsync { get; set; }
        /// <summary>
        /// 帧尾或叫数据长度(4) + CRC检验索引(1) + CRC校验码(1) + 玩家id(4) = 10
        /// </summary>
        protected new readonly byte frame = 10;
        /// <summary>
        /// tcp服务器套接字
        /// </summary>
        public Socket TcpServer { get; private set; }

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
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//---UDP协议
            TcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//---TCP协议
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);//IP端口设置
            TcpServer.NoDelay = true;
            Server.Bind(ip);//绑定UDP IP端口
            TcpServer.Bind(ip);
            TcpServer.Listen(LineUp);
#if !UNITY_ANDROID//在安卓启动服务器时忽略此错误
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            Server.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);//udp远程关闭现有连接方案
#endif
            IsRunServer = true;
            AcceptConnect();
            UdpStartReceive();
            Thread send = new Thread(SendDataHandle) { IsBackground = true, Name = "SendDataHandle" };
            send.Start();
            Thread hupdate = new Thread(CheckHeartHandle) { IsBackground = true, Name = "HeartUpdate" };//tcp,udp保活连接状态
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
            threads.Add("SendDataHandle", send);
            threads.Add("HeartUpdate", hupdate);
            threads.Add("DataTrafficThread", dtt);
            threads.Add("SceneUpdateHandle", suh);
            threads.Add("VarSyncHandler", vsh);
            KeyValuePair<string, Scene> scene = OnAddDefaultScene();
            MainSceneName = scene.Key;
            scene.Value.Name = scene.Key;
            Scenes.TryAdd(scene.Key, scene.Value);
            scene.Value.onSerializeOptHandle = OnSerializeOpt;
            OnStartupCompletedHandle();
#if WINDOWS
            Win32KernelAPI.timeBeginPeriod(1);
#endif
        }

        private void AcceptConnect()
        {
            try
            {
                if (!IsRunServer)
                    return;
                if (SocketAsync == null)
                {
                    SocketAsync = new SocketAsyncEventArgs();
                    SocketAsync.Completed += OnTCPIOCompleted;
                }
                SocketAsync.AcceptSocket = null;// 重用前进行对象清理
                TcpServer.AcceptAsync(SocketAsync);
            }
            catch (Exception ex)
            {
                Debug.Log($"接受异常:{ex}");
            }
        }

        private void OnTCPIOCompleted(object sender, SocketAsyncEventArgs args)
        {
            Socket client = null;
            SocketAsyncOperation socketOpt = args.LastOperation;
            bool willRaiseEvent = true;
        RevdData: switch (socketOpt)
            {
                case SocketAsyncOperation.Accept:
                    try
                    {
                        client = args.AcceptSocket;
                        EndPoint remoteEndPoint = client.RemoteEndPoint;
                        if (remoteEndPoint == null)
                            return;
                        SocketAsyncEventArgs args1 = new SocketAsyncEventArgs();
                        args1.Completed += OnTCPIOCompleted;
                        args1.SetBuffer(new byte[65507], 0, 65507);
                        args1.UserToken = client;
                        Player unClient = ObjectPool<Player>.Take();
                        unClient.Client = client;
                        unClient.LastTime = DateTime.Now.AddMinutes(5);
                        unClient.TcpRemoteEndPoint = client.RemoteEndPoint;
                        int uid = UserIDNumber;
                        UserIDNumber++;
                        unClient.UserID = uid;
                        unClient.playerID = uid.ToString();
                        unClient.stackStreamName = rootPath + $"/reliable/{Name}-" + uid + ".stream";
                        unClient.stackStream = new FileStream(unClient.stackStreamName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                        unClient.isDispose = false;
                        unClient.CloseSend = false;
                        Interlocked.Increment(ref ignoranceNumber);
                        AllClients.TryAdd(remoteEndPoint, unClient);
                        UIDClients.TryAdd(uid, unClient);
                        OnHasConnectHandle(unClient);
                        byte[] uidbytes = BitConverter.GetBytes(uid);
                        byte[] identify = Encoding.Unicode.GetBytes(unClient.playerID);
                        byte[] buffer = new byte[identify.Length + 4];
                        Array.Copy(uidbytes, 0, buffer, 0, 4);
                        Array.Copy(identify, 0, buffer, 4, identify.Length);
                        SendRT(unClient, NetCmd.Identify, buffer);
                        unClient.revdDataBeProcessed = RevdDataBeProcesseds[threadNum];
                        unClient.sendDataBeProcessed = SendDataBeProcesseds[threadNum];
                        if (++threadNum >= RevdDataBeProcesseds.Count)
                            threadNum = 0;
                        willRaiseEvent = client.ReceiveAsync(args1);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                    finally
                    {
                        AcceptConnect();
                    }
                    if (!willRaiseEvent)
                    {
                        socketOpt = SocketAsyncOperation.Receive;
                        goto RevdData;
                    }
                    break;
                case SocketAsyncOperation.Receive:
                    client = args.UserToken as Socket;
                    int count = args.BytesTransferred;
                    if (count > 0 & args.SocketError == SocketError.Success)
                    {
                        var buffer = BufferPool.Take();
                        Buffer.BlockCopy(args.Buffer, args.Offset, buffer, 0, count);
                        receiveCount += count;
                        receiveAmount++;
                        EndPoint remotePoint = client.RemoteEndPoint;
                        if (AllClients.TryGetValue(remotePoint, out Player client1))//在线客户端  得到client对象
                            client1.revdDataBeProcessed.Enqueue(new RevdDataBuffer(client1, buffer, 0, count, true));
                        if (!client.ReceiveAsync(args))
                            goto RevdData;
                    }
                    break;
            }
        }

        protected override void RevdDataHandle(object state)
        {
            QueueSafe<RevdDataBuffer> RevdDataBeProcessed = state as QueueSafe<RevdDataBuffer>;
            while (IsRunServer)
            {
                try
                {
                    revdLoopNum++;
                    int count = RevdDataBeProcessed.Count;
                    if (count <= 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        if (RevdDataBeProcessed.TryDequeue(out RevdDataBuffer revdData))
                        {
                            DataHandle(revdData.client as Player, revdData.buffer, revdData.index, revdData.count, revdData.tcp_udp);
                            BufferPool.Push(revdData.buffer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("处理异常:" + ex);
                }
            }
        }

        private void DataHandle(Player client, Segment buffer, int index, int count, bool tcp_udp)
        {
            if (!tcp_udp)
            {
                int size = BitConverter.ToInt32(buffer, index);
                int crcIndex = buffer[4];
                byte crcCode = buffer[5];
                index += frame;
                if (index + size == count)
                {
                    if (!OnCRC(crcIndex, crcCode))
                        return;
                    DataHandle(client, buffer, frame, size, count);
                }
            }
            else
            {
                BufferHandle(client, buffer, index, count);
            }
        }

        private void BufferHandle(Player client, Segment buffer, int index, int count)
        {
            client.heart = 0;
            if (client.stack > StackNumberMax)//不能一直叠包
            {
                client.stack = 0;
                Debug.LogError($"请设置StackNumberMax属性, 叠包次数过高, 叠包数量达到{StackNumberMax}次以上...");
                SendRT(client, NetCmd.ReliableCallbackClear, new byte[0]);
                return;
            }
            if (client.stack > 0)
            {
                client.stack++;
                client.stackStream.Seek(client.stackIndex, SeekOrigin.Begin);
                int size = count - index;
                client.stackIndex += size;
                client.stackStream.Write(buffer, index, size);
                if (client.stackIndex < client.stackCount)
                {
                    InvokeRevdRTProgress(client, client.stackIndex, client.stackCount);
                    return;
                }
                index = 0;
                count = (int)client.stackStream.Position;//.Length; //错误问题,不能用length, 这是文件总长度, 之前可能已经有很大一波数据
                buffer = BufferPool.Take(count);
                client.stackStream.Seek(0, SeekOrigin.Begin);
                client.stackStream.Read(buffer, 0, count);
            }
            while (index < count)
            {
                int size = BitConverter.ToInt32(buffer, index);
                int crcIndex = buffer[index + 4];//CRC检验索引, 使用者自己改变CRCCode属性
                byte crcCode = buffer[index + 5];//CRC校验码, 使用者自己改变CRCCode属性
                int uid = BitConverter.ToInt32(buffer, index + 6);
                if (uid < 0 | size < 0 | size > StackBufferSize)//如果出现解析的数据包大小有问题，则不处理
                {
                    client.stack = 0;
                    Debug.LogError($"数据错乱:uid:{uid} size:{size}");
                    return;
                }
                if (index + frame + size <= count)
                {
                    index += frame;
                    client.stack = 0;
                    if (!OnCRC(crcIndex, crcCode))
                        return;
                    DataHandle(client, buffer, index, size, index + size); //叠包问题
                    index += size;
                }
                else
                {
                    client.stackIndex = count - index;
                    client.stackCount = size;
                    client.stackStream.Seek(0, SeekOrigin.Begin);
                    client.stackStream.Write(buffer, index, count - index);
                    client.stack++;
                    break;
                }
            }
        }

        public override void RemoveClient(Player client)
        {
            if (client.isDispose)
                return;
            if (client.Login & onlineNumber > 0) Interlocked.Decrement(ref onlineNumber);
            else if (!client.Login & ignoranceNumber > 0) Interlocked.Decrement(ref ignoranceNumber);
            Players.TryRemove(client.playerID, out _);
            UIDClients.TryRemove(client.UserID, out _);
            AllClients.TryRemove(client.TcpRemoteEndPoint, out _);
            OnRemoveClientHandle(client);
            client.OnRemoveClient();
            ExitScene(client, false);
            client.Dispose();
            //ObjectPool<Player>.Push(client);
        }

        private void UdpStartReceive()
        {
            UdpSocketAsync = new SocketAsyncEventArgs { UserToken = Server };
            UdpSocketAsync.Completed += OnUdpIOCompleted;
            UdpSocketAsync.SetBuffer(new byte[65507], 0, 65507);
            UdpSocketAsync.RemoteEndPoint = Server.LocalEndPoint;
            Server.ReceiveFromAsync(UdpSocketAsync);
        }

        private void OnUdpIOCompleted(object sender, SocketAsyncEventArgs args)
        {
            receiveAmount++;
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    try
                    {
                        int count = args.BytesTransferred;
                        if (count > 0 & count > frame)
                        {
                            var buffer = BufferPool.Take();
                            Buffer.BlockCopy(args.Buffer, 0, buffer, 0, count);
                            receiveCount += count;
                            receiveAmount++;
                            EndPoint remotePoint = args.RemoteEndPoint;
                            int uid = BitConverter.ToInt32(buffer, 6);
                            if (UIDClients.TryGetValue(uid, out Player client))//在线客户端  得到client对象
                                client.revdDataBeProcessed.Enqueue(new RevdDataBuffer() { client = client, buffer = buffer, count = count, tcp_udp = false });
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                    finally
                    {
                        if (Server != null & IsRunServer)
                            Server.ReceiveFromAsync(args);
                    }
                    break;
            }
        }

        //当进入到这里已经确定是完整的数据了, 也是解决了进入基类的DataHandle方法, 导致没有uid值出现的问题
        protected override void ReliableTransportComplete(Player client, Segment buffer, int count)
        {
            DataHandle(client, buffer, 0, count, count);//数据已经完整, 直接处理
        }

        protected override void SendRTDataHandle(Player client, QueueSafe<RPCModel> rtRPCModels)
        {
            SendDataHandle(client, rtRPCModels, true);
        }

        protected override void SendDataHandle(Player client, QueueSafe<RPCModel> rPCModels, bool reliable)
        {
            int count = rPCModels.Count;
            if (count <= 0)
                return;
            var stream = BufferPool.Take();
            WriteDataHead(stream);
            stream.Write(BitConverter.GetBytes(client.UserID), 0, 4);//与其他协议不同的是, 双协议需要uid
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
                stream.SetPositionLength(frame);
                stream.Position = frame;
                stream.Write(buffer, 0, len);
                buffer = stream.ToArray();
                return buffer;
            }
            else
            {
                stream.Position = 0;
                int len = stream.Count - frame;
                stream.Write(BitConverter.GetBytes(len), 0, 4);
                stream.Position = len + frame;
                return stream.ToArray();
            }
        }

        protected override void SendByteData(Player client, byte[] buffer, bool reliable)
        {
            if (client.sendDataBeProcessed.Count >= 268435456)//最大只能处理每秒256m数据
            {
                Debug.LogError("发送缓冲列表已经超出限制!");
                return;
            }
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            client.sendDataBeProcessed.Enqueue(new SendDataBuffer(client, buffer, reliable));
        }

        protected override void ProcessSend(object state)
        {
            QueueSafe<SendDataBuffer> SendDataBeProcessed = state as QueueSafe<SendDataBuffer>;
            while (IsRunServer)
            {
                try
                {
                    int count = SendDataBeProcessed.Count;
                    if (count <= 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        if (SendDataBeProcessed.TryDequeue(out SendDataBuffer sendData))
                        {
                            Player client = sendData.client as Player;
                            if (!client.Client.Connected)
                                continue;
                            int count1;
                            SocketError error = SocketError.Success;
                            if (sendData.reliable | client.RemotePoint == null | sendData.buffer.Length > 50000)
                                count1 = client.Client.Send(sendData.buffer, 0, sendData.buffer.Length, SocketFlags.None, out error);
                            else
                                count1 = Server.SendTo(sendData.buffer, 0, sendData.buffer.Length, SocketFlags.None, client.RemotePoint);
                            if (error != SocketError.Success)
                                continue;
                            if (count1 <= 0)
                                OnSendErrorHandle?.Invoke(client, sendData.buffer, sendData.reliable);
                            sendAmount++;
                            sendCount += sendData.buffer.Length;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("发送异常:" + ex);
                }
            }
        }

        protected override void HeartHandle()
        {
            foreach (var client in AllClients)
            {
                if (client.Value == null)
                    continue;
                if (!client.Value.Client.Connected)
                {
                    RemoveClient(client.Value);
                    continue;
                }
                client.Value.heart++;
                SendRT(client.Value, NetCmd.SendHeartbeat, new byte[0]);//保活连接状态
                if (client.Value.Login)
                    continue;
                if (DateTime.Now > client.Value.LastTime)
                {
                    Debug.Log($"赖在服务器的客户端:{client.Key}被强制下线!");
                    client.Value.TcpRemoteEndPoint = client.Key;//解决key偶尔不对导致一直移除不了问题
                    RemoveClient(client.Value);
                    break;
                }
            }
        }

        public override void Close()
        {
            base.Close();
            if (TcpServer != null)
            {
                TcpServer.Dispose();
                TcpServer.Close();
                TcpServer = null;
            }
            if (UdpSocketAsync != null)
            {
                UdpSocketAsync.Completed -= OnUdpIOCompleted;
                UdpSocketAsync.Dispose();
                UdpSocketAsync = null;
            }
        }
    }
}
