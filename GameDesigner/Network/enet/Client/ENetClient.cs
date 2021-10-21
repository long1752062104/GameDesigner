namespace Net.Client
{
    using ENet;
    using Net.Event;
    using Net.Share;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Net.System;

    /// <summary>
    /// ENet客户端类型 
    /// 第三版本 2020.9.14
    /// </summary>
    [Serializable]
    [Obsolete("enet不稳定!")]
    public class ENetClient : ClientBase, IHost
    {
        public Host ClientHost { get; private set; }
        public new Peer Client { get; private set; }
        public byte ChannelID { get; private set; }

        /// <summary>
        /// 构造客户端
        /// </summary>
        public ENetClient()
        {
        }

        /// <summary>
        /// 构造客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public ENetClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~ENetClient()
        {
#if !UNITY_EDITOR
            Close();
#endif
        }

        public override Task Connect(string host, int port, int localPort, Action<bool> result)
        {
            if (!Library.INIT)
            {
                Library.INIT = true;
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!File.Exists(path + "\\enet.dll"))
                    throw new FileNotFoundException($"enet.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 enet.dll复制到{path}目录下.");
                if (!File.Exists(path + "\\ENetX64.dll"))
                    throw new FileNotFoundException($"ENetX64.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 ENetX64.dll复制到{path}目录下.");
                if (!File.Exists(path + "\\ENetX86.dll"))
                    throw new FileNotFoundException($"ENetX86.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 ENetX86.dll复制到{path}目录下.");
#endif
                Library.Initialize();
            }
            Library.HOSTS.Add(this);
            return base.Connect(host, port, localPort, result);
        }

        protected override Task ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            try
            {
                ClientHost = new Host();
                Address address = new Address();
                address.SetHost(host);
                address.Port = (ushort)port;
                ClientHost.Create(65507);
                Client = ClientHost.Connect(address);
                return Task.Run(() =>
                {
                    DateTime time = DateTime.Now.AddSeconds(5);
                    while (true)
                    {
                        if (DateTime.Now > time)
                        {
                            ClientHost.Dispose();
                            ClientHost = null;
                            InvokeContext((arg) => {
                                networkState = NetworkState.ConnectFailed;
                                result(false); 
                            });
                            return;
                        }
                        if (ClientHost.CheckEvents(out Event netEvent) <= 0)
                            if (ClientHost.Service(15, out netEvent) <= 0)
                                continue;
                        if (netEvent.Type == EventType.Timeout)
                        {
                            ClientHost.Dispose();
                            ClientHost = null;
                            InvokeContext((arg) => {
                                networkState = NetworkState.ConnectFailed;
                                result(false);
                            });
                        }
                        else if (netEvent.Type == EventType.Connect)
                        {
                            Connected = true;
                            ChannelID = netEvent.ChannelID;
                            StartupThread();
                            InvokeContext((arg) => {
                                networkState = NetworkState.Connected;
                                result(true); 
                            });
                        }
                        break;
                    }
                });
            }
            catch (Exception ex)
            {
                NDebug.Log("连接错误: 如果提示视图加载错误则为平台设置不对! " + ex.ToString());
                networkState = NetworkState.ConnectFailed;
                result(false);
                return null;
            }
        }

        protected override void SendRTDataHandle()
        {
            SendDataHandle(rtRPCModels, true);
        }

        protected override void ReceiveHandle()
        {
            while (Connected)
            {
                try
                {
                    if (ClientHost == null)
                        return;
                    if (ClientHost.NativeData == IntPtr.Zero)
                        return;
                    if (Native.enet_host_check_events(ClientHost.NativeData, out ENetEvent eNetEvent) <= 0)
                        if (Native.enet_host_service(ClientHost.NativeData, out eNetEvent, 1) <= 0)
                        {
                            Thread.Sleep(1);
                            continue;
                        }
                    Event netEvent = new Event(eNetEvent);
                    switch (netEvent.Type)
                    {
                        case EventType.Disconnect:
                            Connected = false;
                            NetworkState = networkState = NetworkState.ConnectLost;
                            sendRTList.Clear();
                            revdRTList.Clear();
                            rtRPCModels = new QueueSafe<RPCModel>();
                            rPCModels = new QueueSafe<RPCModel>();
                            ClientHost.Dispose();
                            ClientHost = null;
                            NDebug.Log("断开连接！");
                            return;
                        case EventType.Receive:
                            heart = 0;
                            int count = netEvent.Packet.Length;
                            var buffer = BufferPool.Take(count);
                            netEvent.Packet.CopyTo(buffer);
                            receiveCount += count;
                            receiveAmount++;
                            netEvent.Packet.Dispose();
                            ResolveBuffer(buffer, 0, count, false);
                            BufferPool.Push(buffer);
                            break;
                        case EventType.Timeout:
                            Connected = false;
                            NetworkState = networkState = NetworkState.ConnectLost;
                            sendRTList.Clear();
                            revdRTList.Clear();
                            rtRPCModels = new QueueSafe<RPCModel>();
                            rPCModels = new QueueSafe<RPCModel>();
                            ClientHost.Dispose();
                            ClientHost = null;
                            Client.DisconnectNow(0);
                            NDebug.Log("连接中断！");
                            return;
                    }
                }
                catch (Exception ex)
                {
                    NDebug.LogWarning("处理ENet异常:" + ex);
                }
            }
        }

        protected override void SendByteData(byte[] buffer, bool reliable)
        {
            sendCount += buffer.Length;
            sendAmount++;
            Packet packet = new Packet();
            packet.Create(buffer, reliable ? PacketFlags.Reliable : PacketFlags.None);
            bool error = Client.Send(ChannelID, ref packet);
            if (error)
                OnSendErrorHandle?.Invoke(buffer, reliable);
        }

        public override void Close(bool await = true, int millisecondsTimeout = 1000)
        {
            Client.DisconnectNow(0);
            Connected = false;
            openClient = false;
            NetworkState = networkState = NetworkState.ConnectClosed;
            AbortedThread();
            sendRTList.Clear();
            revdRTList.Clear();
            StackStream?.Close();
            stack = 0;
            if (Instance == this)
                Instance = null;
            if (ClientHost != null)
            {
                ClientHost.Flush();
                ClientHost.Dispose();
                ClientHost = null;

            }
            Library.HOSTS.Remove(this);
            if (Library.HOSTS.Count == 0)
            {
                Library.Deinitialize();
                Library.INIT = false;
            }
            Config.GlobalConfig.ThreadPoolRun = false;
            NDebug.Log("客户端已关闭！");
        }

        /// <summary>
        /// udp压力测试
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="clientLen">测试客户端数量</param>
        /// <param name="dataLen">每个客户端数据大小</param>
        public static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen)
        {
            if (!Library.INIT)
            {
                Library.INIT = true;
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!File.Exists(path + "\\enet.dll"))
                    throw new FileNotFoundException($"enet.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 enet.dll复制到{path}目录下.");
                if (!File.Exists(path + "\\ENetX64.dll"))
                    throw new FileNotFoundException($"ENetX64.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 ENetX64.dll复制到{path}目录下.");
                if (!File.Exists(path + "\\ENetX86.dll"))
                    throw new FileNotFoundException($"ENetX86.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 ENetX86.dll复制到{path}目录下.");
#endif
                Library.Initialize();
            }
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                List<Peer> clients = new List<Peer>();
                List<Host> clients1 = new List<Host>();
                for (int i = 0; i < clientLen; i++)
                {
                    Host host = new Host();
                    Address address = new Address();
                    address.SetHost(ip);
                    address.Port = (ushort)port;
                    host.Create(65507);
                    Peer socket = host.Connect(address);
                    clients.Add(socket);
                    clients1.Add(host);
                }
                byte[] buffer = new byte[dataLen];
                using (MemoryStream stream = new MemoryStream(512))
                {
                    int crcIndex = 0;
                    byte crcCode = 0x2d;
                    stream.Write(new byte[4], 0, 4);
                    stream.WriteByte((byte)crcIndex);
                    stream.WriteByte(crcCode);
                    RPCModel rPCModel = new RPCModel(NetCmd.CallRpc, buffer);
                    stream.WriteByte((byte)(rPCModel.kernel ? 68 : 74));
                    stream.WriteByte(rPCModel.cmd);
                    stream.Write(BitConverter.GetBytes(rPCModel.buffer.Length), 0, 4);
                    stream.Write(rPCModel.buffer, 0, rPCModel.buffer.Length);

                    stream.Position = 0;
                    int len = (int)stream.Length - 6;
                    stream.Write(BitConverter.GetBytes(len), 0, 4);
                    stream.Position = len + 6;
                    buffer = stream.ToArray();
                }
                while (!cts.IsCancellationRequested)
                {
                    Thread.Sleep(31);
                    for (int i = 0; i < clients.Count; i++)
                    {
                        if (Native.enet_host_check_events(clients1[i].NativeData, out ENetEvent eNetEvent) <= 0)
                            if (Native.enet_host_service(clients1[i].NativeData, out eNetEvent, 1) <= 0)
                                continue;
                        Packet packet = new Packet();
                        packet.Create(buffer, PacketFlags.Reliable);
                        clients[i].Send(0, ref packet);
                    }
                }
                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].DisconnectNow(0);
                }
            }, cts.Token);
            return cts;
        }
    }
}