namespace Net.Server
{
    using Net.Share;
    using global::System;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Text;
    using global::System.Threading;
    using Debug = Event.NDebug;
    using Net.System;

    /// <summary>
    /// TCP服务器类型
    /// 第三版本 2020.9.14
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    public class TcpServer<Player, Scene> : ServerBase<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
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
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//---TCP协议
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);//IP端口设置
            Server.NoDelay = true;
            Server.Bind(ip);//绑定UDP IP端口
            Server.Listen(LineUp);
            IsRunServer = true;
            Thread proAcc = new Thread(ProcessAcceptConnect) { IsBackground = true, Name = "ProcessAcceptConnect" };
            proAcc.Start();
            Thread proRevd = new Thread(ProcessReceive) { IsBackground = true, Name = "ProcessReceive" };
            proRevd.Start();
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
            threads.Add("ProcessAcceptConnect", proAcc);
            threads.Add("ProcessReceiveFrom", proRevd);
            threads.Add("SendDataHandle", send);
            threads.Add("HeartUpdate", hupdate);
            threads.Add("DataTrafficThread", dtt);
            threads.Add("SceneUpdateHandle", suh);
            threads.Add("VarSyncHandler", vsh);
            var scene = OnAddDefaultScene();
            MainSceneName = scene.Key;
            scene.Value.Name = scene.Key;
            Scenes.TryAdd(scene.Key, scene.Value);
            scene.Value.onSerializeOptHandle = OnSerializeOpt;
            OnStartupCompletedHandle();
#if WINDOWS
            Win32KernelAPI.timeBeginPeriod(1);
#endif
        }

        private void ProcessAcceptConnect()
        {
            while (IsRunServer)
            {
                try
                {
                    Socket socket = Server.Accept();
                    Player client = ObjectPool<Player>.Take();
                    client.Client = socket;
                    client.LastTime = DateTime.Now.AddMinutes(5);
                    client.TcpRemoteEndPoint = socket.RemoteEndPoint;
                    client.RemotePoint = socket.RemoteEndPoint;
                    client.isDispose = false;
                    client.CloseSend = false;
                    int uid = UserIDNumber;
                    UserIDNumber++;
                    client.UserID = uid;
                    client.playerID = uid.ToString();
                    client.stackStreamName = rootPath + $"/reliable/{Name}-" + uid + ".stream";
                    client.stackStream = new FileStream(client.stackStreamName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    Interlocked.Increment(ref ignoranceNumber);
                    AllClients.TryAdd(socket.RemoteEndPoint, client);
                    OnHasConnectHandle(client);
                    byte[] uidbytes = BitConverter.GetBytes(uid);
                    byte[] identify = Encoding.Unicode.GetBytes(client.playerID);
                    byte[] buffer = new byte[identify.Length + 4];
                    Array.Copy(uidbytes, 0, buffer, 0, 4);
                    Array.Copy(identify, 0, buffer, 4, identify.Length);
                    SendRT(client, NetCmd.Identify, buffer);
                    client.revdDataBeProcessed = RevdDataBeProcesseds[threadNum];
                    client.sendDataBeProcessed = SendDataBeProcesseds[threadNum];
                    if (++threadNum >= RevdDataBeProcesseds.Count)
                        threadNum = 0;
                }
                catch (Exception ex)
                {
                    Debug.Log($"接受异常:{ex}");
                }
            }
        }

        private void ProcessReceive()
        {
            Player[] allClients = new Player[0];
            while (IsRunServer)
            {
                try
                {
                    Thread.Sleep(1);
                    if (allClients.Length != AllClients.Count)
                        allClients = AllClients.Values.ToArray();
                    for (int i = 0; i < allClients.Length; i++)
                    {
                        Player client = allClients[i];
                        if (!client.Client.Connected)
                            continue;
                        if (client.Client.Poll(0, SelectMode.SelectRead))
                        {
                            var segment = BufferPool.Take();
                            int count = client.Client.Receive(segment, 0, segment.Length, SocketFlags.None, out SocketError error);
                            if (error != SocketError.Success)
                                continue;
                            receiveCount += count;
                            receiveAmount++;
                            client.revdDataBeProcessed.Enqueue(new RevdDataBuffer() { client = client, buffer = segment, count = count, tcp_udp = true });
                        }
                    }
                    revdLoopNum++;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
            }
        }

        protected override void RevdDataHandle(object state)
        {
            QueueSafe<RevdDataBuffer> RevdDataBeProcessed = state as QueueSafe<RevdDataBuffer>;
            while (IsRunServer)
            {
                try
                {
                    int count = RevdDataBeProcessed.Count;
                    if (count <= 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    while (count > 0)
                    {
                        count--;//避免崩错. 先--
                        if (RevdDataBeProcessed.TryDequeue(out RevdDataBuffer revdData))
                        {
                            BufferHandle(revdData.client as Player, revdData.buffer, revdData.index, revdData.count);
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
                if (size < 0 | size > StackBufferSize)//如果出现解析的数据包大小有问题，则不处理
                {
                    client.stack = 0;
                    Debug.LogError($"数据错乱或数据量太大: size:{size}， 如果想传输大数据，请设置StackBufferSize属性");
                    return;
                }
                if (index + frame + size <= count)
                {
                    index += frame;
                    client.stack = 0;
                    if (!OnCRC(crcIndex, crcCode))
                        return;
                    DataHandle(client, buffer, index, size, index + size); //count会出错, 因为tcp的叠包
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

        protected override bool IsInternalCommand(Player client, RPCModel model)
        {
            if (model.cmd == NetCmd.Connect | model.cmd == NetCmd.Broadcast)
                return true;
            return false;
        }

        protected override void SendRTDataHandle(Player client, QueueSafe<RPCModel> rtRPCModels)
        {
            SendDataHandle(client, rtRPCModels, true);
        }

        protected override void SendByteData(Player client, byte[] buffer, bool reliable)
        {
            if (!client.Client.Connected)
                return;
            if (client.sendDataBeProcessed.Count >= 268435456)//最大只能处理每秒256m数据
            {
                Debug.LogError("发送缓冲列表已经超出限制!");
                return;
            }
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            client.sendDataBeProcessed.Enqueue(new SendDataBuffer(client, buffer));
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
                            int count1 = client.Client.Send(sendData.buffer, 0, sendData.buffer.Length, SocketFlags.None, out SocketError error);
                            if (error != SocketError.Success | count1 <= 0)
                            {
                                OnSendErrorHandle?.Invoke(client, sendData.buffer, true);
                                continue;
                            }
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
                if (RTOMode == RTOMode.Variable)
                    Ping(client.Value);
                if (!client.Value.Login)
                {
                    if (DateTime.Now > client.Value.LastTime)
                    {
                        Debug.Log($"赖在服务器的客户端:{client.Key}被强制下线!");
                        client.Value.RemotePoint = client.Key;//解决key偶尔不对导致一直移除不了问题
                        RemoveClient(client.Value);
                        break;
                    }
                }
                if (client.Value.heart <= HeartLimit)//有5次确认心跳包
                    continue;
                SendRT(client.Value, NetCmd.SendHeartbeat, new byte[0]);//保活连接状态
            }
        }
    }
}