namespace Net.Server
{
    using Net.Share;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Threading;
    using Debug = Event.NDebug;
    using Net.System;
    using global::System.Security.Cryptography;
    using Net.Helper;

    /// <summary>
    /// tcp 输入输出完成端口服务器
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    public class TcpServerIOCP<Player, Scene> : ServerBase<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// tcp数据长度(4) + 2CRC协议 = 6
        /// </summary>
        protected new readonly int frame = 6;

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
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
            Server.NoDelay = true;
            Server.Bind(ip);
            Server.Listen(LineUp);
            IsRunServer = true;
            AcceptConnect();
            Thread send = new Thread(SendDataHandle) { IsBackground = true, Name = "SendDataHandle" };
            send.Start();
            Thread suh = new Thread(SceneUpdateHandle) { IsBackground = true, Name = "SceneUpdateHandle" };
            suh.Start();
            ThreadManager.Invoke("DataTrafficThread", 1f, DataTrafficHandler);
            ThreadManager.Invoke("SingleHandler", SingleHandler);
            ThreadManager.Invoke("SyncVarHandler", SyncVarHandler);
            ThreadManager.Invoke("CheckHeartHandler", HeartInterval / 1000f, CheckHeartHandler, true);
            for (int i = 0; i < MaxThread; i++)
            {
                QueueSafe<RevdDataBuffer> revdQueue = new QueueSafe<RevdDataBuffer>();
                RevdQueues.Add(revdQueue);
                Thread revd = new Thread(RevdDataHandle) { IsBackground = true, Name = "RevdDataHandle" + i };
                revd.Start(revdQueue);
                threads.Add("RevdDataHandle" + i, revd);
                QueueSafe<SendDataBuffer> sendDataBeProcessed = new QueueSafe<SendDataBuffer>();
                SendQueues.Add(sendDataBeProcessed);
                Thread proSend = new Thread(ProcessSend) { IsBackground = true, Name = "ProcessSend" + i };
                proSend.Start(sendDataBeProcessed);
                threads.Add("ProcessSend" + i, proSend);
            }
            threads.Add("SendDataHandle", send);
            threads.Add("SceneUpdateHandle", suh);
            KeyValuePair<string, Scene> scene = OnAddDefaultScene();
            MainSceneName = scene.Key;
            scene.Value.Name = scene.Key;
            Scenes.TryAdd(scene.Key, scene.Value);
            scene.Value.onSerializeOptHandle = OnSerializeOpt;
            OnStartupCompletedHandle();
#if WINDOWS
            Win32KernelAPI.timeBeginPeriod(1);
#endif
            InitUserID();
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
                Server.AcceptAsync(SocketAsync);
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
        RevdData: switch (socketOpt)
            {
                case SocketAsyncOperation.Accept:
                    try
                    {
                        client = args.AcceptSocket;
                        if (client.RemoteEndPoint == null)
                            return;
                        SocketAsyncEventArgs args1 = new SocketAsyncEventArgs();
                        args1.Completed += OnTCPIOCompleted;
                        args1.SetBuffer(new byte[65507], 0, 65507);
                        args1.UserToken = client;
                        Player unClient = new Player();
                        unClient.Client = client;
                        unClient.LastTime = DateTime.Now.AddMinutes(5);
                        unClient.RemotePoint = client.RemoteEndPoint;
                        unClient.TcpRemoteEndPoint = client.RemoteEndPoint;
                        UserIDStack.TryPop(out int uid);
                        unClient.UserID = uid;
                        unClient.MID = GetMID((IPEndPoint)client.RemoteEndPoint);
                        unClient.PlayerID = uid.ToString();
                        unClient.stackStream = BufferStreamShare.Take();
                        unClient.isDispose = false;
                        unClient.CloseSend = false;
                        unClient.SocketAsync = args1;
                        Interlocked.Increment(ref ignoranceNumber);
                        var buffer = BufferPool.Take(50);
                        buffer.WriteValue(uid);
                        buffer.WriteValue(unClient.MID);
                        buffer.WriteValue(unClient.PlayerID);
                        SendRT(unClient, NetCmd.Identify, buffer.ToArray(true));
                        unClient.revdQueue = RevdQueues[threadNum];
                        unClient.sendQueue = SendQueues[threadNum];
                        if (++threadNum >= RevdQueues.Count)
                            threadNum = 0;
                        AllClients.TryAdd(client.RemoteEndPoint, unClient);//之前放在上面, 由于接收线程并行, 还没赋值revdQueue就已经接收到数据, 导致提示内存池泄露
                        OnHasConnectHandle(unClient);
                        bool willRaiseEvent = client.ReceiveAsync(args1);
                        if (!willRaiseEvent)
                        {
                            socketOpt = SocketAsyncOperation.Receive;
                            goto RevdData;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                    finally
                    {
                        AcceptConnect();
                    }
                    break;
                case SocketAsyncOperation.Receive:
                    client = args.UserToken as Socket;
                    int count = args.BytesTransferred;
                    if (count > 0 & args.SocketError == SocketError.Success)
                    {
                        var buffer = BufferPool.Take();
                        buffer.Count = count;
                        Buffer.BlockCopy(args.Buffer, args.Offset, buffer, 0, count);
                        receiveCount += count;
                        receiveAmount++;
                        EndPoint remotePoint = client.RemoteEndPoint;
                        if (AllClients.TryGetValue(remotePoint, out Player client1))//在线客户端  得到client对象
                            client1.revdQueue.Enqueue(new RevdDataBuffer() { client = client1, buffer = buffer, tcp_udp = true });
                        if (!client.ReceiveAsync(args))
                            goto RevdData;
                    }
                    break;
                case SocketAsyncOperation.Send:
                    client = args.UserToken as Socket;
                    bool willRaiseEvent1 = client.ReceiveAsync(args);
                    if (!willRaiseEvent1)
                    {
                        socketOpt = SocketAsyncOperation.Receive;
                        goto RevdData;
                    }
                    break;
            }
        }

        protected override void RevdDataHandle(object state)
        {
            var revdQueue = state as QueueSafe<RevdDataBuffer>;
            while (IsRunServer)
            {
                try
                {
                    revdLoopNum++;
                    int count = revdQueue.Count;
                    if (count <= 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        if (revdQueue.TryDequeue(out RevdDataBuffer revdData))
                        {
                            BufferHandle(revdData.client as Player, ref revdData.buffer);
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

        protected override bool IsInternalCommand(Player client, RPCModel model)
        {
            if (model.cmd == NetCmd.Connect | model.cmd == NetCmd.Broadcast)
                return true;
            return false;
        }

        protected override void WriteDataHead(Segment stream)
        {
            if (MD5CRC)
            {
                stream.Position = 20;//留20个字节记录size+md5哈希值
            }
            else
            {
                int crcIndex = RandomHelper.Range(0, 256);
                byte crcCode = CRCCode[crcIndex];
                stream.Position = 4;//数据大小
                stream.WriteByte((byte)crcIndex);
                stream.WriteByte(crcCode);
            }
        }

        protected override void ResetDataHead(Segment stream)
        {
            if (MD5CRC)
                stream.SetPositionLength(20);//size+md5
            else
                stream.SetPositionLength(frame);
        }

        protected override byte[] PackData(Segment stream)
        {
            if (MD5CRC)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(stream, 20, stream.Count - 20);
                EncryptHelper.ToEncrypt(Password, retVal);
                int len = stream.Count - 20;
                stream.Position = 0;
                stream.Write(BitConverter.GetBytes(len), 0, 4);
                stream.Write(retVal, 0, retVal.Length);
                stream.Position = len + 20;
            }
            else
            {
                int len = stream.Count - frame;
                stream.Position = 0;
                stream.Write(BitConverter.GetBytes(len), 0, 4);
                stream.Position = len + frame;
            }
            return stream.ToArray();
        }

        protected override void SendRTDataHandle(Player client, QueueSafe<RPCModel> rtRPCModels)
        {
            SendDataHandle(client, rtRPCModels, true);
        }

        protected override void SendByteData(Player client, byte[] buffer, bool reliable)
        {
            if (!client.Client.Connected)
                return;
            if (client.sendQueue.Count >= 268435456)//最大只能处理每秒256m数据
            {
                Debug.LogError("发送缓冲列表已经超出限制!");
                return;
            }
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            client.sendQueue.Enqueue(new SendDataBuffer(client, buffer));
        }

        protected override void ProcessSend(object state)
        {
            var sendQueue = state as QueueSafe<SendDataBuffer>;
            while (IsRunServer)
            {
                try
                {
                    int count = sendQueue.Count;
                    if (count <= 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        if (sendQueue.TryDequeue(out SendDataBuffer sendData))
                        {
                            Player client = sendData.client as Player;
                            if (!client.Client.Connected)
                                continue;
                            int count1 = client.Client.Send(sendData.buffer, 0, sendData.buffer.Length, SocketFlags.None, out SocketError error);
                            if (error != SocketError.Success)
                                continue;
                            if (count1 <= 0)
                                OnSendErrorHandle?.Invoke(client, sendData.buffer, true);
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
                if (!client.Value.Login)
                {
                    if (DateTime.Now > client.Value.LastTime)
                    {
                        Debug.Log($"赖在服务器的客户端:{client.Key}被强制下线!");
                        client.Value.TcpRemoteEndPoint = client.Key;//解决key偶尔不对导致一直移除不了问题
                        RemoveClient(client.Value);
                        continue;
                    }
                }
                if (client.Value.heart <= HeartLimit)//有5次确认心跳包
                    continue;
                SendRT(client.Value, NetCmd.SendHeartbeat, new byte[0]);//保活连接状态
            }
        }

        public override void RemoveClient(Player client)
        {
            base.RemoveClient(client);
        }

        public override void Close()
        {
            if (SocketAsync != null)
            {
                SocketAsync.Completed -= OnTCPIOCompleted;
                SocketAsync.Dispose();
                SocketAsync = null;
            }
            base.Close();
        }
    }
}