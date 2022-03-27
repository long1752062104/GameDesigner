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
    using global::System.Security.Cryptography;
    using Net.Helper;

    /// <summary>
    /// TCP客户端类型 
    /// 第三版本 2020.9.14
    /// </summary>
    [Serializable]
    public class TcpClient : ClientBase
    {
        /// <summary>
        /// tcp数据长度(4) + 1CRC协议 = 5
        /// </summary>
        protected override int frame { get; set; } = 5;
        public override bool MD5CRC
        {
            get => md5crc;
            set
            {
                md5crc = value;
                if (value)
                    frame = 5 + 16;
                else
                    frame = 5;
            }
        }

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

        protected override Task<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
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
                    StackStream = BufferStreamShare.Take();
                    InvokeContext((arg) => {
                        networkState = NetworkState.Connected;
                        result(true);
                    });
                    return true;
                }
                catch(Exception ex)
                {
                    NDebug.LogError("连接错误:" + ex);
                    AbortedThread();
                    Client?.Close();
                    Client = null;
                    InvokeContext((arg) => {
                        networkState = NetworkState.ConnectFailed;
                        result(false);
                    });
                    return false;
                }
            });
        }

        protected override bool HeartHandler()
        {
            try
            {
                if (RTOMode == RTOMode.Variable)
                    Ping();
                heart++;
                if (heart <= HeartLimit)
                    return true;
                if (!Connected)
                    Reconnection(10);
                else
                    Send(NetCmd.SendHeartbeat, new byte[0]);
            }
            catch
            {
            }
            return openClient & currFrequency < 10;
        }

        protected override void StartupThread()
        {
            AbortedThread();//断线重连处理
            Connected = true;
            StartThread("SendDataHandle", SendDataHandle);
            StartThread("ReceiveHandle", ReceiveHandle);
            checkRpcHandleID = ThreadManager.Invoke("CheckRpcHandle", CheckRpcHandle);
            networkFlowHandlerID = ThreadManager.Invoke("NetworkFlowHandler", 1f, NetworkFlowHandler);
            heartHandlerID = ThreadManager.Invoke("HeartHandler", HeartInterval * 0.001f, HeartHandler);
            syncVarHandlerID = ThreadManager.Invoke("SyncVarHandler", SyncVarHandler);
            if (!UseUnityThread)
                updateHandlerID = ThreadManager.Invoke("UpdateHandle", UpdateHandler);
            ThreadManager.PingRun();
        }

        protected override void SendRTDataHandle()
        {
            SendDataHandle(rtRPCModels, true);
        }

        protected override byte[] PackData(Segment stream)
        {
            stream.Flush();
            if (MD5CRC)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                var head = frame;
                byte[] retVal = md5.ComputeHash(stream, head, stream.Count - head);
                EncryptHelper.ToEncrypt(Password, retVal);
                int len = stream.Count - head;
                var lenBytes = BitConverter.GetBytes(len);
                byte crc = CRCHelper.CRC8(lenBytes, 0, lenBytes.Length);
                stream.Position = 0;
                stream.Write(lenBytes, 0, 4);
                stream.WriteByte(crc);
                stream.Write(retVal, 0, retVal.Length);
                stream.Position = len + head;
            }
            else 
            {
                int len = stream.Count - frame;
                var lenBytes = BitConverter.GetBytes(len);
                byte retVal = CRCHelper.CRC8(lenBytes, 0, lenBytes.Length);
                stream.Position = 0;
                stream.Write(lenBytes, 0, 4);
                stream.WriteByte(retVal);
                stream.Position = len + frame;
            }
            return stream.ToArray();
        }

        protected override void SendByteData(byte[] buffer, bool reliable)
        {
            sendCount += buffer.Length;
            sendAmount++;
            int count = Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
            if (count <= 0)
                OnSendErrorHandle?.Invoke(buffer, reliable);
        }

        protected override void ResolveBuffer(Segment buffer, bool isTcp)
        {
            heart = 0;
            if (stack > StackNumberMax)//不能一直叠包
            {
                stack = 0;
                NDebug.LogError($"请设置StackNumberMax属性, 叠包次数过高, 叠包数量达到{StackNumberMax}次以上...");
                SendRT(NetCmd.ReliableCallbackClear, new byte[0]);
                return;
            }
            if (stack > 0)
            {
                stack++;
                StackStream.Seek(stackIndex, SeekOrigin.Begin);
                int size = buffer.Count - buffer.Position;
                stackIndex += size;
                StackStream.Write(buffer, buffer.Position, size);
                if (stackIndex < stackCount)
                {
                    InvokeRevdRTProgress(stackIndex, stackCount);
                    return;
                }
                var count = (int)StackStream.Position;//.Length; //错误问题,不能用length, 这是文件总长度, 之前可能已经有很大一波数据
                BufferPool.Push(buffer);//要回收掉, 否则会提示内存泄露
                buffer = BufferPool.Take(count);//ref 才不会导致提示内存泄露
                StackStream.Seek(0, SeekOrigin.Begin);
                StackStream.Read(buffer, 0, count);
                buffer.Count = count;
            }
            while (buffer.Position < buffer.Count)
            {
                if (buffer.Position + 5 > buffer.Count)//流数据偶尔小于frame头部字节
                {
                    var position = buffer.Position;
                    var count = buffer.Count - position;
                    stackIndex = count;
                    stackCount = 0;
                    StackStream.Seek(0, SeekOrigin.Begin);
                    StackStream.Write(buffer, position, count);
                    stack++;
                    break;
                }
                var lenBytes = buffer.Read(4);
                byte crcCode = buffer.ReadByte();//CRC检验索引
                byte retVal = CRCHelper.CRC8(lenBytes, 0, lenBytes.Length);
                if (crcCode != retVal)
                {
                    stack = 0;
                    NDebug.LogError($"CRC校验失败:");
                    return;
                }
                int size = BitConverter.ToInt32(lenBytes, 0);
                if (size < 0 | size > StackBufferSize)//如果出现解析的数据包大小有问题，则不处理
                {
                    stack = 0;
                    NDebug.LogError($"数据错乱或数据量太大: size:{size}， 如果想传输大数据，请设置StackBufferSize属性");
                    return;
                }
                int value = MD5CRC ? 16 : 0;
                if (buffer.Position + size + value <= buffer.Count)
                {
                    stack = 0;
                    var count = buffer.Count;//此长度可能会有连续的数据(粘包)
                    buffer.Count = buffer.Position + value + size;//需要指定一个完整的数据长度给内部解析
                    base.ResolveBuffer(buffer, true);
                    buffer.Count = count;//解析完成后再赋值原来的总长
                }
                else
                {
                    var position = buffer.Position - 5;
                    var count = buffer.Count - position;
                    stackIndex = count;
                    stackCount = size;
                    StackStream.Seek(0, SeekOrigin.Begin);
                    StackStream.Write(buffer, position, count);
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
            if (await) Thread.Sleep(millisecondsTimeout);//给update线程一秒的时间处理关闭事件
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
            revdRTStream?.Close();
            revdRTStream = null;
            UID = 0;
            if (Instance == this) Instance = null;
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
        public static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen, Action<TcpClientTest> onInit = null, Action<List<TcpClientTest>> fpsAct = null, IAdapter adapter = null)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                List<TcpClientTest> clients = new List<TcpClientTest>();
                for (int i = 0; i < clientLen; i++)
                {
                    TcpClientTest client = new TcpClientTest();
                    onInit?.Invoke(client);
                    if(adapter != null)
                        client.AddAdapter(adapter);
                    try { 
                        client.Connect(ip, port);
                    } catch (Exception ex) {
                        NDebug.LogError(ex);
                        return;
                    }
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
                            clients[i].NetworkFlowHandler();
                            clients[i].fps = 0;
                        }
                    }
                });
                int threadNum = (clientLen / 1000) + 1;
                for (int i = 0; i < threadNum; i++)
                {
                    int index = i * 1000;
                    int end = index + 1000;
                    Task.Run(() =>
                    {
                        if (end > clientLen)
                            end = clientLen;
                        while (!cts.IsCancellationRequested)
                        {
                            Thread.Sleep(1000);
                            for (int ii = index; ii < end; ii++)
                            {
                                try
                                {
                                    var client = clients[ii];
                                    if (client.Client == null)
                                        continue;
                                    if (client.Client.Poll(0, SelectMode.SelectRead))
                                    {
                                        var buffer1 = BufferPool.Take(65536);
                                        buffer1.Count = client.Client.Receive(buffer1);
                                        client.ResolveBuffer(buffer1, false);
                                        BufferPool.Push(buffer1);
                                    }
                                    //client.Send(NetCmd.Local, buffer);
                                    client.SendRT("Register", RandomHelper.Range(10000,9999999).ToString(), "123");
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
            protected override Task<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
            {
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.localPort = localPort;
                Client.Connect(host, port);
                Client.Blocking = false;
                Client.NoDelay = true;
                rPCModels.Enqueue(new RPCModel(NetCmd.Connect, new byte[0]));
                SendDirect();
                Connected = true;
                StackStream = BufferStreamShare.Take();
                return Task.FromResult(Connected);
            }
            protected override void StartupThread() { }

            protected override void OnConnected(bool result) { }

            protected override bool OnCRC(int index, byte crcCode)
            {
                if (index < 0 | index > CRCHelper.CRCCode.Length)
                    return false;
                if (CRCHelper.CRCCode[index] == crcCode)
                    return true;
                return false;
            }
            protected override void ResolveBuffer(Segment buffer, bool isTcp)
            {
                receiveCount += buffer.Count;
                receiveAmount++;
                base.ResolveBuffer(buffer, isTcp);
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