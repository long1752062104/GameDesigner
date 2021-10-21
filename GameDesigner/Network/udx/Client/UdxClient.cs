namespace Net.Client
{
    using Net.Event;
    using Net.Share;
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net.Sockets;
    using global::System.Runtime.InteropServices;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Udx;
    using Net.System;

    /// <summary>
    /// udx客户端类型 -> 只能300人以下连接, 如果想要300个客户端以上, 请进入udx网址:www.goodudx.com 联系作者下载专业版FastUdxApi.dll, 然后更换下框架内的FastUdxApi.dll即可
    /// 第三版本 2020.9.14
    /// </summary>
    [Serializable]
    public class UdxClient : ClientBase, IUDX
    {
        private IntPtr udxObj;
        public IntPtr ClientPtr { get; private set; }
        private UDXPRC uDXPRC;

        /// <summary>
        /// 构造可靠传输客户端
        /// </summary>
        public UdxClient()
        {
        }

        /// <summary>
        /// 构造可靠传输客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public UdxClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~UdxClient()
        {
#if !UNITY_EDITOR
            Close();
#endif
        }

        public override Task Connect(string host, int port, int localPort, Action<bool> result)
        {
            if (!UdxLib.INIT)
            {
                UdxLib.INIT = true;
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!File.Exists(path + "\\FastUdxApi.dll"))
                    throw new FileNotFoundException($"FastUdxApi.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 FastUdxApi.dll复制到{path}目录下.");
#endif
                UdxLib.UInit(1);
            }
            UdxLib.UDXS.Add(this);
            return base.Connect(host, port, localPort, result);
        }

        protected override Task ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            try
            {
                udxObj = UdxLib.UCreateFUObj();
                UdxLib.UBind(udxObj, null, 0);
                uDXPRC = new UDXPRC(ProcessReceive);
                UdxLib.USetFUCB(udxObj, uDXPRC);
                GC.KeepAlive(uDXPRC);
                string host1 = host;
                if (host == "127.0.0.1")
                    host1 = Server.NetPort.GetIP();
                UdxLib.UConnect(udxObj, host1, port, 0, false, 0);
                return Task.Run(() =>
                {
                    DateTime timeout = DateTime.Now.AddSeconds(5);
                    while (!Connected & DateTime.Now < timeout) { Thread.Sleep(1); }
                    if (Connected)
                        StartupThread();
                    InvokeContext((arg) => {
                        networkState = Connected ? NetworkState.Connected : NetworkState.ConnectFailed;
                        result(Connected); 
                    });
                });
            }
            catch (Exception ex)
            {
                NDebug.Log("连接错误: " + ex.ToString());
                networkState = NetworkState.ConnectFailed;
                result(false);
                return null;
            }
        }

        protected override void StartupThread()
        {
            Connected = true;
            StartThread("SendHandle", SendDataHandle);
            StartThread("NetworkFlowHandle", NetworkFlowHandle);
            StartThread("CheckRpcHandle", CheckRpcHandle);
            StartThread("HeartHandle", HeartHandle);
            StartThread("VarSyncHandler", VarSyncHandler);
            if (!UseUnityThread)
                StartThread("UpdateHandle", UpdateHandle);
#if UNITY_ANDROID
            if (Context == null)
                return;
            Context.Post(new SendOrPostCallback((o)=> {
                var randomName = RandomHelper.Range(0, int.MaxValue);
                fileStreamName = UnityEngine.Application.persistentDataPath + "/rtTemp" + randomName + ".tmp";
            }),null);
#else
            fileStreamName = global::System.IO.Path.GetTempFileName();
#endif
        }

        protected void ProcessReceive(UDXEVENT_TYPE type, int erro, IntPtr cli, IntPtr pData, int len)//cb回调
        {
            try
            {
                heart = 0;
                switch (type)
                {
                    case UDXEVENT_TYPE.E_CONNECT:
                        if (erro != 0)
                            return;
                        ClientPtr = cli;
                        UdxLib.UDump(cli);
                        Connected = true;
                        UdxLib.USetGameMode(cli, true);
                        break;
                    case UDXEVENT_TYPE.E_LINKBROKEN:
                        Connected = false;
                        NetworkState = networkState = NetworkState.ConnectLost;
                        sendRTList.Clear();
                        revdRTList.Clear();
                        rtRPCModels = new QueueSafe<RPCModel>();
                        rPCModels = new QueueSafe<RPCModel>();
                        NDebug.Log("断开连接！");
                        break;
                    case UDXEVENT_TYPE.E_DATAREAD:
                        var buffer = BufferPool.Take(len);
                        Marshal.Copy(pData, buffer, 0, len);
                        receiveCount += len;
                        receiveAmount++;
                        ResolveBuffer(buffer, 0, len, false);
                        BufferPool.Push(buffer);
                        break;
                }
            }
            catch (Exception ex)
            {
                NDebug.LogError("处理异常:" + ex);
            }
        }

        protected override void HeartHandle()
        {
            while (openClient & currFrequency < 10)
            {
                try
                {
                    Thread.Sleep(HeartInterval);//5秒发送一个心跳包
                    heart++;
                    if (heart <= HeartLimit)
                        continue;
                    if (Connected & heart < HeartLimit + 5)
                        Send(NetCmd.SendHeartbeat, new byte[0]);
                    else if (!Connected)//尝试连接执行
                        Reconnection(10);
                } 
                catch { }
            }
        }

        protected override void SendRTDataHandle()
        {
            SendDataHandle(rtRPCModels, true);
        }

        protected unsafe override void SendByteData(byte[] buffer, bool reliable)
        {
            if (ClientPtr == IntPtr.Zero)
                return;
            sendCount += buffer.Length;
            sendAmount++;
            fixed (byte* ptr = buffer) 
            {
                int count = UdxLib.USend(ClientPtr, ptr, buffer.Length);
                if (count <= 0)
                    OnSendErrorHandle?.Invoke(buffer, reliable);
            }
        }

        public override void Close(bool await = true, int millisecondsTimeout = 1000)
        {
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
            if (ClientPtr != IntPtr.Zero)
            {
                UdxLib.UClose(ClientPtr);
                UdxLib.UUndump(ClientPtr);
                ClientPtr = IntPtr.Zero;
            }
            if (udxObj != IntPtr.Zero)
            {
                UdxLib.UDestroyFUObj(udxObj);
                udxObj = IntPtr.Zero;
            }
            UdxLib.UDXS.Remove(this);
            if (UdxLib.UDXS.Count == 0)
            {
                UdxLib.UUnInit();
                UdxLib.INIT = false;
            }
            Config.GlobalConfig.ThreadPoolRun = false;
            NDebug.Log("客户端已关闭！");
        }

        /// <summary>
        /// udx压力测试
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="clientLen">测试客户端数量</param>
        /// <param name="dataLen">每个客户端数据大小</param>
        public unsafe static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen)
        {
            if (!UdxLib.INIT)
            {
                UdxLib.INIT = true;
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!File.Exists(path + "\\FastUdxApi.dll"))
                    throw new FileNotFoundException($"FastUdxApi.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 FastUdxApi.dll复制到{path}目录下.");
#endif
                UdxLib.UInit(8);
            }
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                List<IntPtr> clients = new List<IntPtr>();
                string host1 = ip;
                if (host1 == "127.0.0.1")
                    host1 = Server.NetPort.GetIP();
                IntPtr udxObj = UdxLib.UCreateFUObj();
                UdxLib.UBind(udxObj, null, 0);
                UdxLib.USetFUCB(udxObj, (type, erro, cli, pData, len) =>
                {
                    switch (type)
                    {
                        case UDXEVENT_TYPE.E_CONNECT:
                            if (erro != 0)
                                return;
                            clients.Add(cli);
                            UdxLib.UDump(cli);
                            UdxLib.USetGameMode(cli, true);
                            break;
                        case UDXEVENT_TYPE.E_LINKBROKEN:
                            NDebug.Log("断开连接！");
                            break;
                        case UDXEVENT_TYPE.E_DATAREAD:

                            break;
                    }
                });
                for (int i = 0; i < clientLen; i++)
                {
                    UdxLib.UConnect(udxObj, host1, port, 0, false, 0);
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
                fixed (byte* ptr = buffer) 
                {
                    while (!cts.IsCancellationRequested)
                    {
                        Thread.Sleep(31);
                        for (int i = 0; i < clients.Count; i++)
                        {
                            int count = UdxLib.USend(clients[i], ptr, buffer.Length);
                        }
                    }
                }
                for (int i = 0; i < clients.Count; i++)
                {
                    UdxLib.UClose(clients[i]);
                    UdxLib.UUndump(clients[i]);
                }
                UdxLib.UDestroyFUObj(udxObj);
            }, cts.Token);
            return cts;
        }
    }
}