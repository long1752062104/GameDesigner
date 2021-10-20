namespace Net.Server
{
    using ENet;
    using Net.Share;
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Debug = Event.NDebug;
    using Net.System;

    /// <summary>
    /// ENet服务器类型
    /// 第三版本 2020.9.14
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    [Obsolete("enet不稳定!")]
    public class ENetServer<Player, Scene> : ServerBase<Player, Scene>, IHost where Player : NetPeer, new() where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// enet服务器对象
        /// </summary>
        public new Host Server;
        private readonly ConcurrentDictionary<Peer, Player> peers = new ConcurrentDictionary<Peer, Player>();

        ~ENetServer()
        {
            Close();
        }

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
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(path + "\\enet.dll"))
                throw new FileNotFoundException($"enet.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 enet.dll复制到{path}目录下.");
            if (!File.Exists(path + "\\ENetX64.dll"))
                throw new FileNotFoundException($"ENetX64.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 ENetX64.dll复制到{path}目录下.");
            if (!File.Exists(path + "\\ENetX86.dll"))
                throw new FileNotFoundException($"ENetX86.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 ENetX86.dll复制到{path}目录下.");
#endif
            try
            {
                if (!Library.INIT)
                {
                    Library.INIT = true;
                    Library.Initialize();//如果加载错误, 则需要右键项目属性设置当前系统的平台(X64或X86)
                }
            }
            catch (Exception ex) { throw new IOException("请选择正确的系统平台编译! 右键项目属性设置当前系统的平台(X64或X86)" + ex); }
            Server = new Host();
            Address address = new Address { Port = port };
            Server.Create(address, LineUp, 255);
            Library.HOSTS.Add(this);
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
            scene.Value.Name = scene.Key;
            Scenes.TryAdd(scene.Key, scene.Value);
            scene.Value.onSerializeOptHandle = OnSerializeOpt;
            OnStartupCompletedHandle();
        }

        protected virtual void ProcessReceive()
        {
            while (IsRunServer)
            {
                try
                {
                    revdLoopNum++;
                    if (Server.NativeData == IntPtr.Zero)
                        return;
                    if (Native.enet_host_check_events(Server.NativeData, out ENetEvent eNetEvent) <= 0)
                        if (Native.enet_host_service(Server.NativeData, out eNetEvent, 1) <= 0)
                        {
                            Thread.Sleep(1);
                            continue;
                        }
                    Event netEvent = new Event(eNetEvent);
                J: switch (netEvent.Type)
                    {
                        case EventType.Connect:
                            int uid = UserIDNumber;
                            UserIDNumber++;
                            Player unClient = ObjectPool<Player>.Take();
                            unClient.UserID = uid;
                            unClient.playerID = uid.ToString();
                            unClient.EClient = netEvent.Peer;
                            unClient.ChannelID = netEvent.ChannelID;
                            IPEndPoint remotePoint = new IPEndPoint(IPAddress.Parse(netEvent.Peer.IP), netEvent.Peer.Port);
                            unClient.RemotePoint = remotePoint;
                            unClient.LastTime = DateTime.Now.AddMinutes(5);
                            unClient.isDispose = false;
                            unClient.CloseSend = false;
                            peers.TryAdd(netEvent.Peer, unClient);
                            Interlocked.Increment(ref ignoranceNumber);
                            AllClients.TryAdd(remotePoint, unClient);
                            OnHasConnectHandle(unClient);
                            unClient.revdDataBeProcessed = RevdDataBeProcesseds[threadNum];
                            unClient.sendDataBeProcessed = SendDataBeProcesseds[threadNum];
                            if (++threadNum >= RevdDataBeProcesseds.Count)
                                threadNum = 0;
                            break;
                        case EventType.Disconnect:
                            if (peers.TryRemove(netEvent.Peer, out Player client))
                                Disconnected(client);
                            break;
                        case EventType.Receive:
                            if (peers.TryGetValue(netEvent.Peer, out Player client1))
                            {
                                client1.heart = 0;
                                int count = netEvent.Packet.Length;
                                var buffer = BufferPool.Take();
                                netEvent.Packet.CopyTo(buffer);
                                receiveCount += count;
                                receiveAmount++;
                                client1.revdDataBeProcessed.Enqueue(new RevdDataBuffer() { client = client1, buffer = buffer, count = count, tcp_udp = false });
                            }
                            else
                            {
                                netEvent.Type = EventType.Connect;
                                goto J;
                            }
                            netEvent.Packet.Dispose();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("处理ENet异常:" + ex);
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
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            sendAmount++;
            sendCount += buffer.Length;
            try
            {
                Packet packet = new Packet();
                packet.Create(buffer, reliable ? PacketFlags.Reliable : PacketFlags.None);
                bool error = client.EClient.Send(client.ChannelID, ref packet);
                if (error)
                    OnSendErrorHandle?.Invoke(client, buffer, reliable);
            }
            catch (Exception ex)
            {
                Debug.LogError("发送错误:" + ex.Message);
            }
        }

        protected void Disconnected(Player client)
        {
            RemoveClient(client);
        }

        public override void RemoveClient(Player client)
        {
            if (client.isDispose)
                return;
            if (client.Login & onlineNumber > 0) Interlocked.Decrement(ref onlineNumber);
            else if (!client.Login & ignoranceNumber > 0) Interlocked.Decrement(ref ignoranceNumber);
            peers.TryRemove(client.EClient, out _);
            Players.TryRemove(client.playerID, out _);
            UIDClients.TryRemove(client.UserID, out _);
            AllClients.TryRemove(client.RemotePoint, out _);
            OnRemoveClientHandle(client);
            client.OnRemoveClient();
            ExitScene(client, false);
            client.Dispose();
            //ObjectPool<Player>.Push(client);
        }

        protected override void HeartHandle()
        {
            //base.HeartHandle();
        }

        public override void Close()
        {
            base.Close();
            if (Server != null)
            {
                Server.Flush();
                Server.Dispose();
            }
            Library.HOSTS.Remove(this);
            if (Library.HOSTS.Count == 0)
            {
                Library.Deinitialize();
                Library.INIT = false;
            }
        }
    }
}