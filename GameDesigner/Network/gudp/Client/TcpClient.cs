namespace Net.Client
{
    using Net.Event;
    using Net.Share;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Net.System;

    /// <summary>
    /// TCP客户端类型 
    /// 第三版本 2020.9.14
    /// </summary>
    [Serializable]
    public class TcpClient : ClientBase
    {
        /// <summary>
        /// 构造不可靠传输客户端
        /// </summary>
        public TcpClient()
        {
        }

        /// <summary>
        /// 构造不可靠传输客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public TcpClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~TcpClient()
        {
#if !UNITY_EDITOR
            Close();
#endif
        }

        protected override Task ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            return Task.Run(() =>
            {
                try
                {
                    Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.localPort = localPort;
                    Client.NoDelay = true;
                    if (localPort != -1)
                        Client.Bind(new IPEndPoint(IPAddress.Any, localPort));
                    Client.Connect(host, port);
                    StartupThread();
                    DateTime time = DateTime.Now.AddSeconds(5);
                    while (UID == 0)
                        if (DateTime.Now >= time)
                            throw new Exception("uid赋值失败!");
                    stackStreamName = persistentDataPath + "/c" + UID + ".stream";
                    StackStream = new FileStream(stackStreamName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    InvokeContext((arg) => {
                        networkState = NetworkState.Connected;
                        result(true);
                    });
                }
                catch(Exception ex)
                {
                    NDebug.LogError("连接错误:" + ex);
                    Client?.Close();
                    Client = null;
                    InvokeContext((arg) => {
                        networkState = NetworkState.ConnectFailed;
                        result(false);
                    });
                }
            });
        }

        protected override void HeartHandle()
        {
            while (openClient & currFrequency < 10)
            {
                Thread.Sleep(HeartInterval);//5秒发送一个心跳包
                try
                {
                    if (RTOMode == RTOMode.Variable)
                        Ping();
                    heart++;
                    if (heart <= HeartLimit)
                        continue;
                    if (!Connected)
                        Reconnection(10);
                    else
                        Send(NetCmd.SendHeartbeat, new byte[0]);
                }
                catch
                {
                }
            }
        }

        protected override void StartupThread()
        {
            Connected = true;
            StartThread("SendDataHandle", SendDataHandle);
            StartThread("ReceiveHandle", ReceiveHandle);
            StartThread("NetworkFlowHandle", NetworkFlowHandle);
            StartThread("CheckRpcHandle", CheckRpcHandle);
            StartThread("HeartHandle", HeartHandle);
            StartThread("VarSyncHandler", VarSyncHandler);
            if (!UseUnityThread)
                StartThread("UpdateHandle", UpdateHandle);
        }

        protected override void SendRTDataHandle()
        {
            SendDataHandle(rtRPCModels, true);
        }

        protected override void SendByteData(byte[] buffer, bool reliable)
        {
            sendCount += buffer.Length;
            sendAmount++;
            int count = Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
            if (count <= 0)
                OnSendErrorHandle?.Invoke(buffer, reliable);
        }

        protected override void ResolveBuffer(Segment buffer, int index, int count, bool isTcp)
        {
            heart = 0;//如果传输大文件时, 一直不能解析状态下, 导致掉线问题
            if (stack > StackNumberMax)//不能一直叠包
            {
                stack = 0;
                NDebug.LogError($"请设置StackNumberMax属性, 叠包次数过高, 叠包数量达到{StackNumberMax}次以上...");
                return;
            }
            if (stack > 0)
            {
                stack++;
                StackStream.Seek(stackIndex, SeekOrigin.Begin);
                int size = count - index;
                stackIndex += size;
                StackStream.Write(buffer, index, size);
                if (stackIndex < stackCount)
                {
                    InvokeRevdRTProgress(stackIndex, stackCount);
                    return;
                }
                index = 0;
                count = (int)StackStream.Position;//Length; //错误问题,不能用length, 这是文件总长度, 之前可能已经有很大一波数据
                buffer = BufferPool.Take(count);
                StackStream.Seek(0, SeekOrigin.Begin);
                StackStream.Read(buffer, 0, count);
            }
            while (index < count)
            {
                int size = BitConverter.ToInt32(buffer, index);
                int crcIndex = buffer[4];//CRC检验索引, 使用者自己改变CRCCode属性
                byte crcCode = buffer[5];//CRC校验码, 使用者自己改变CRCCode属性
                if (size < 0 | size > StackBufferSize)//如果出现解析的数据包大小有问题，则不处理
                {
                    stack = 0;
                    NDebug.LogError($"数据错乱或数据量太大: size:{size}， 如果想传输大数据，请设置StackBufferSize属性");
                    return;
                }
                if (index + frame + size <= count)
                {
                    index += frame;
                    stack = 0;
                    if (!OnCRC(crcIndex, crcCode))
                        return;
                    DataHandle(buffer, index, size, index + size); //叠包问题 count);
                    index += size;
                }
                else
                {
                    stackIndex = count - index;
                    stackCount = size;
                    StackStream.Seek(0, SeekOrigin.Begin);
                    StackStream.Write(buffer, index, count - index);
                    stack++;
                    break;
                }
            }
        }

        public override void Close(bool await = true, int millisecondsTimeout = 1000)
        {
            Connected = false;
            openClient = false;
            NetworkState = networkState = NetworkState.ConnectClosed;
            if (await) Thread.Sleep(1000);//给update线程一秒的时间处理关闭事件
            AbortedThread();
            Client?.Close();
            Client = null;
            sendRTList.Clear();
            revdRTList.Clear();
            StackStream?.Close();
            StackStream = null;
            stack = 0;
            stackIndex = 0;
            stackCount = 0;
            if (File.Exists(stackStreamName) & !string.IsNullOrEmpty(stackStreamName))
                File.Delete(stackStreamName);
            stackStreamName = "";
            if (Instance == this)
                Instance = null;
            Config.GlobalConfig.ThreadPoolRun = false;
            NDebug.Log("客户端已关闭！");
        }

        /// <summary>
        /// tcp压力测试
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="clientLen">测试客户端数量</param>
        /// <param name="dataLen">每个客户端数据大小</param>
        public static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen, Action<TcpClientTest> onInit = null, Action<List<TcpClientTest>> fpsAct = null)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                List<TcpClientTest> clients = new List<TcpClientTest>();
                for (int i = 0; i < clientLen; i++)
                {
                    TcpClientTest client = new TcpClientTest();
                    onInit?.Invoke(client);
                    client.Connect(ip, port);
                    clients.Add(client);
                }
                byte[] buffer = new byte[dataLen];
                Task.Run(() =>
                {
                    while (!cts.IsCancellationRequested)
                    {
                        Thread.Sleep(1000);
                        fpsAct?.Invoke(clients);
                        for (int i = 0; i < clients.Count; i++)
                        {
                            clients[i].OnNetworkFlowHandle();
                            clients[i].fps = 0;
                        }
                    }
                });
                int threadNum = (clientLen / 50) + 1;
                for (int i = 0; i < threadNum; i++)
                {
                    int index = i * 50;
                    int end = index + 50;
                    Task.Run(() =>
                    {
                        if (end > clientLen)
                            end = clientLen;
                        while (!cts.IsCancellationRequested)
                        {
                            Thread.Sleep(30);
                            for (int ii = index; ii < end; ii++)
                            {
                                try
                                {
                                    var client = clients[ii];
                                    if (client.Client.Poll(0, SelectMode.SelectRead))
                                    {
                                        var buffer1 = BufferPool.Take(65536);
                                        int count = client.Client.Receive(buffer1);
                                        client.ResolveBuffer(buffer1, 0, count, false);
                                        BufferPool.Push(buffer1);
                                    }
                                    client.Send(NetCmd.Local, buffer);
                                    client.SendDirect();
                                }
                                catch (Exception ex)
                                {
                                    NDebug.LogError(ex);
                                }
                            }
                        }
                    });
                }
                while (!cts.IsCancellationRequested)
                    Thread.Sleep(30);
                Thread.Sleep(100);
                for (int i = 0; i < clients.Count; i++)
                    clients[i].Close(false);
            }, cts.Token);
            return cts;
        }

        public class TcpClientTest : TcpClient
        {
            public int fps;
            public int revdSize { get { return receiveCount; } }
            public int sendSize { get { return sendCount; } }
            public int sendNum { get { return sendAmount; } }
            public int revdNum { get { return receiveAmount; } }
            public int resolveNum { get { return receiveAmount; } }
            
            public TcpClientTest()
            {
                OnRevdBufferHandle += (model) => { fps++; };
            }
            protected override Task ConnectResult(string host, int port, int localPort, Action<bool> result)
            {
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.localPort = localPort;
                Client.Connect(host, port);
                Client.Blocking = false;
                Client.NoDelay = true;
                SendByteData(new byte[] { 6, 0, 0, 0, 0, 0x2d, 74, NetCmd.Connect, 0, 0, 0, 0 }, false);
                Connected = true;
                stackStreamName = Path.GetTempFileName();
                StackStream = new FileStream(stackStreamName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                return null;
            }
            protected override void StartupThread() { }

            protected override void OnConnected(bool result) { }

            protected override bool OnCRC(int index, byte crcCode)
            {
                if (index < 0 | index > CRCCode.Length)
                    return false;
                if (CRCCode[index] == crcCode)
                    return true;
                return false;
            }
            protected override void ResolveBuffer(Segment buffer, int index, int count, bool isTcp)
            {
                receiveCount += count;
                receiveAmount++;
                base.ResolveBuffer(buffer, index, count, isTcp);
            }
            protected unsafe override void SendByteData(byte[] buffer, bool reliable)
            {
                sendCount += buffer.Length;
                sendAmount++;
#if WINDOWS
                fixed (byte* ptr = buffer)
                    Win32KernelAPI.send(Client.Handle, ptr, buffer.Length, SocketFlags.None);
#else
                Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
#endif
            }
            protected internal override byte[] OnSerializeOptInternal(OperationList list)
            {
                return new byte[0];
            }
            protected internal override OperationList OnDeserializeOptInternal(byte[] buffer, int index, int count)
            {
                return default;
            }
            public override string ToString()
            {
                return $"uid:{Identify} conv:{Connected}";
            }
        }
    }
}