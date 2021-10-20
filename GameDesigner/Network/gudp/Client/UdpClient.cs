namespace Net.Client
{
    using Net.Share;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net.Sockets;
    using global::System.Reflection;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Net.System;

    /// <summary>
    /// Udp网络客户端
    /// 在安卓端必须设置可以后台运行, 如果不设置,当你按下home键后,app的所有线程将会被暂停,这会影响网络心跳检测线程,导致网络中断
    /// 解决方法 : 在android项目AndroidManifest.xml文件中的activity中添加如下内容：
    /// android:configChanges="fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen" 
    /// 详情请看此博文:https://www.cnblogs.com/nanwei/p/9125316.html
    /// 或这个博文: http://www.voidcn.com/article/p-yakpcmce-bpk.html
    /// </summary>
    [Serializable]
    public class UdpClient : ClientBase
    {
        /// <summary>
        /// 构造udp可靠客户端
        /// </summary>
        public UdpClient() { }

        /// <summary>
        /// 构造udp可靠客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public UdpClient(bool useUnityThread)
        {
            UseUnityThread = useUnityThread;
        }

        /// <summary>
        /// 获取p2p IP和端口, 通过client.OnP2PCallback事件回调
        /// </summary>
        /// <param name="uid"></param>
        public void GetP2P(int uid)
        {
            SendRT(NetCmd.P2P, BitConverter.GetBytes(uid));
        }

        /// <summary>
        /// udp压力测试
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="clientLen">测试客户端数量</param>
        /// <param name="dataLen">每个客户端数据大小</param>
        public unsafe static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen, Action<UdpClientTest> onInit = null, Action<List<UdpClientTest>> fpsAct = null, IAdapter adapter = null)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                List<UdpClientTest> clients = new List<UdpClientTest>();
                for (int i = 0; i < clientLen; i++) 
                {
                    UdpClientTest client = new UdpClientTest();
                    onInit?.Invoke(client);
                    if(adapter!=null)
                        client.AddAdapter(adapter);
                    client.Connect(ip,port);
                    clients.Add(client);
                }
                byte[] buffer = new byte[dataLen];
                Task.Run(()=> 
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
                    Task.Run(()=> 
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
                                    //client.Send(NetCmd.Local, new byte[dataLen]);
                                    client.AddOperation(new Operation(NetCmd.Local) { buffer = new byte[dataLen] });
                                    client.Update();
                                }
                                catch (Exception ex)
                                {
                                    Event.NDebug.LogError(ex);
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
    }

    public class UdpClientTest : UdpClient
    {
        public int fps;
        public int revdSize { get { return receiveCount; } }
        public int sendSize { get { return sendCount; } }
        public int sendNum { get { return sendAmount; } }
        public int revdNum { get { return receiveAmount; } }
        public int resolveNum { get { return receiveAmount; } }
        private byte[] addressBuffer;
        public UdpClientTest()
        {
            OnRevdBufferHandle += (model) => { fps++; };
        }
        protected override Task ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.localPort = localPort;
            Client.Connect(host, port);
            Client.Blocking = false;
            var socketAddress = Client.RemoteEndPoint.Serialize();
            addressBuffer = (byte[])socketAddress.GetType().GetField("m_Buffer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(socketAddress);
            SendByteData(new byte[] { 6, 0, 0, 0, 0, 0x2d, 74, NetCmd.Connect, 0, 0, 0, 0 }, false);
            Connected = true;
            fileStreamName = Path.GetTempFileName();
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
                Win32KernelAPI.sendto(Client.Handle, ptr, buffer.Length, SocketFlags.None, addressBuffer, 16);
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
        /// <summary>
        /// 单线程更新，需要开发者自动调用更新
        /// </summary>
        public void Update() 
        {
            if (!Connected)
                return;
            if (Client.Poll(0, SelectMode.SelectRead))
            {
                var buffer1 = BufferPool.Take(65536);
                int count = Client.Receive(buffer1);
                ResolveBuffer(buffer1, 0, count, false);
                BufferPool.Push(buffer1);
            }
            SendDirect();
        }
        public override string ToString()
        {
            return $"uid:{Identify} conv:{Connected}";
        }
    }
}
