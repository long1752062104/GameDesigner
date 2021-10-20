namespace Net.Server
{
    using Fleck;
    using Net.Serialize;
    using Net.Share;
    using Newtonsoft_X.Json;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Text;
    using global::System.Threading;
    using Debug = Event.NDebug;
    using Net.System;

    /// <summary>
    /// web网络服务器 2020.8.25 七夕
    /// 通过JavaScript脚本, httml网页进行连接. 和 WebClient连接
    /// 客户端发送的数据请求请看Net.Share.MessageModel类定义
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    [Obsolete("尚未实测!遇到问题群里说下，给你解决!", false)]
    public class WebServer<Player, Scene> : ServerBase<Player, Scene> where Player : WebPlayer, new() where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// webSocket服务器套接字
        /// </summary>
        public new WebSocketServer Server { get; protected set; }
        /// <summary>
        /// 当web服务器接收到网络数据处理事件
        /// </summary>
        public WSRevdBufferHandle<Player> OnWSRevdBufferHandle { get; set; }

        /// <summary>
        /// 当web服务器接收到客户端其他指令的数据请求
        /// </summary>
        /// <param name="client">当前客户端</param>
        /// <param name="model"></param>
        protected void OnWSReceiveBuffer(Player client, MessageModel model) { }

        /// <summary>
        /// 当webScoket未知客户端发送数据请求，返回null，不允许unClient进入服务器!，返回对象，允许unClient客户端进入服务器
        /// 客户端玩家的入口点，在这里可以控制客户端是否可以进入服务器与其他客户端进行网络交互
        /// 在这里可以用来判断客户端登录和注册等等进站许可
        /// </summary>
        /// <param name="unClient">客户端终端</param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual bool OnWSUnClientRequest(Player unClient, MessageModel model)
        {
            return true;
        }

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
            OnWSRevdBufferHandle += OnWSReceiveBuffer;
            OnOperationSyncHandle += OnOperationSync;
            OnRevdBufferHandle += OnReceiveBuffer;
            OnReceiveFileHandle += OnReceiveFile;
            OnRevdRTProgressHandle += OnRevdRTProgress;
            OnSendRTProgressHandle += OnSendRTProgress;
            if (OnAddRpcHandle == null) OnAddRpcHandle += AddRpcInternal;//在start之前就要添加你的委托
            if (OnRemoveRpc == null) OnRemoveRpc += RemoveRpcInternal;
            if (OnRPCExecute == null) OnRPCExecute += OnRpcExecuteInternal;
            if (OnSerializeRPC == null) OnSerializeRPC = OnSerializeRpc;
            if (OnDeserializeRPC == null) OnDeserializeRPC = OnDeserializeRpc;
            if (OnSerializeOPT == null) OnSerializeOPT = OnSerializeOptInternal;
            if (OnDeserializeOPT == null) OnDeserializeOPT = OnDeserializeOptInternal;
            Debug.LogHandle += Log;
            Debug.LogWarningHandle += Log;
            Debug.LogErrorHandle += Log;
            OnStartingHandle();
            if (Instance == null)
                Instance = this;
            AddRpcHandle(this, true);
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint ip = new IPEndPoint(ipAddress, port);//IP端口设置
            Server = new WebSocketServer($"ws://{ip}");
            Server.ListenerSocket.NoDelay = true;
            Server.Start(AcceptConnect);
            IsRunServer = true;
            Thread send = new Thread(SendDataHandle) { IsBackground = true, Name = "SendDataHandle" };
            send.Start();
            Thread hupdate = new Thread(CheckHeartHandle) { IsBackground = true, Name = "HeartUpdate" };
            hupdate.Start();//创建心跳包线程
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

        //开始接受客户端连接
        private void AcceptConnect(IWebSocketConnection wsClient)
        {
            WebSocketConnection wsClient1 = wsClient as WebSocketConnection;
            Socket client = ((SocketWrapper)wsClient1.Socket)._socket;
            EndPoint remotePoint = client.RemoteEndPoint;
            wsClient1.OnOpen = () =>
            {
                if (ignoranceNumber >= LineUp)//排队人数
                {
                    exceededNumber++;
                    OnExceededNumber(wsClient1);
                    return;
                }
                if (onlineNumber >= OnlineLimit)//服务器最大在线人数
                {
                    blockConnection++;
                    OnBlockConnection(wsClient1);
                    return;
                }
                exceededNumber = 0;
                blockConnection = 0;
                int uid = UserIDNumber;
                Interlocked.Increment(ref UserIDNumber);//解决onopen是多线程问题
                Player unClient = ObjectPool<Player>.Take();
                unClient.Client = client;
                unClient.TcpRemoteEndPoint = client.RemoteEndPoint;
                unClient.WSClient = wsClient1;
                unClient.RemotePoint = client.RemoteEndPoint;
                unClient.LastTime = DateTime.Now.AddMinutes(5);
                unClient.UserID = uid;
                unClient.playerID = uid.ToString();
                unClient.isDispose = false;
                unClient.CloseSend = false;
                AllClients.TryAdd(client.RemoteEndPoint, unClient);
                Interlocked.Increment(ref ignoranceNumber);
                OnHasConnectHandle(unClient);
                unClient.revdDataBeProcessed = RevdDataBeProcesseds[threadNum];
                unClient.sendDataBeProcessed = SendDataBeProcesseds[threadNum];
                if (++threadNum >= RevdDataBeProcesseds.Count)
                    threadNum = 0;
            };
            wsClient1.OnMessage = (buffer, message) => //utf-8解析
            {
                receiveCount += buffer.Length;
                receiveAmount++;
                if (AllClients.TryGetValue(remotePoint, out Player client1))//在线客户端  得到client对象
                    WSRevdHandle(client1, buffer, message);
            };
            wsClient1.OnBinary = (buffer) =>
            {
                var segment = BufferPool.Take(buffer.Length);
                Buffer.BlockCopy(buffer, 0, segment, 0, buffer.Length);
                receiveCount += buffer.Length;
                receiveAmount++;
                if (AllClients.TryGetValue(remotePoint, out Player client1))//在线客户端  得到client对象
                    client1.revdDataBeProcessed.Enqueue(new RevdDataBuffer() { client = client1, buffer = segment, count = buffer.Length, tcp_udp = true });
            };
            wsClient1.OnClose = () =>
            {
                if (AllClients.TryGetValue(remotePoint, out Player player))
                    RemoveClient(player);
            };
        }

        protected override bool IsInternalCommand(Player client, RPCModel model)
        {
            if (model.cmd == NetCmd.Connect)
                return true;
            if (model.cmd == NetCmd.Broadcast)
                return true;
            return false;
        }

        private void WSRevdHandle(Player client, byte[] buffer, string message)
        {
            try
            {
                MessageModel model = JsonConvert.DeserializeObject<MessageModel>(message);
                RPCModel model1 = new RPCModel(model.cmd, model.func, model.GetPars()) {
                    buffer = buffer, count = buffer.Length
                };
                DataHandle(client, model1);
            }
            catch (Exception ex)
            {
                Debug.LogError("json解析出错:" + ex);
                MessageModel model = new MessageModel(0, "error", new object[] { ex.Message });
                string jsonStr = JsonConvert.SerializeObject(model);
                client.WSClient.Send(jsonStr);
            }
        }

        /// <summary>
        /// 当服务器连接人数溢出时调用
        /// </summary>
        /// <param name="client"></param>
        private void OnExceededNumber(WebSocketConnection client)
        {
            Debug.Log("未知客户端排队爆满,阻止连接次数: " + exceededNumber);
            client.Send(new byte[] { frame, 0, 0, 0, 0, 0x2d, 74, NetCmd.ExceededNumber, 0, 0, 0, 0 });
        }

        /// <summary>
        /// 当服务器爆满时调用
        /// </summary>
        /// <param name="client"></param>
        private void OnBlockConnection(WebSocketConnection client)
        {
            Debug.Log("服务器爆满,阻止连接次数: " + blockConnection);
            client.Send(new byte[] { frame, 0, 0, 0, 0, 0x2d, 74, NetCmd.BlockConnection, 0, 0, 0, 0 });
        }

        protected override void HeartHandle()
        {
            foreach (var client in AllClients)
            {
                if (client.Value == null)
                    continue;
                client.Value.heart++;
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
                Send(client.Value, NetCmd.SendHeartbeat, new byte[0]);
            }
        }

        protected override void SendRTDataHandle(Player client, QueueSafe<RPCModel> rtRPCModels)
        {
            SendDataHandle(client, rtRPCModels, true);
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
                            client.WSClient.Send(sendData.buffer);
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

        protected override void SendByteData(Player client, byte[] buffer, bool reliable)
        {
            if (client.sendDataBeProcessed.Count >= 268435456)//最大只能处理每秒256m数据
            {
                Debug.LogError("发送缓冲列表已经超出限制!");
                return;
            }
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            client.sendDataBeProcessed.Enqueue(new SendDataBuffer(client, buffer));
        }

        protected override void SceneUpdateHandle()
        {
        }

        protected override byte[] OnSerializeRpc(RPCModel model)
        {
            if (!string.IsNullOrEmpty(model.func) | model.methodMask != 0)
            {
                MessageModel model1 = new MessageModel(model.cmd, model.func, model.pars);
                string jsonStr = JsonConvert.SerializeObject(model1);
                byte[] jsonStrBytes = Encoding.UTF8.GetBytes(jsonStr);
                byte[] bytes = new byte[jsonStrBytes.Length + 1];
                bytes[0] = 32; //32=utf8的" "空字符
                Buffer.BlockCopy(jsonStrBytes, 0, bytes, 1, jsonStrBytes.Length);
                return bytes;
            }
            return NetConvert.Serialize(model, new byte[] { 10 });//10=utf8的\n字符
        }

        protected override FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            if (buffer[index++] == 32)
            {
                var message = Encoding.UTF8.GetString(buffer, index, count - 1);
                MessageModel model = JsonConvert.DeserializeObject<MessageModel>(message);
                return new FuncData(model.func, model.GetPars());
            }
            return NetConvert.Deserialize(buffer, index, count - 1);
        }
    }
}