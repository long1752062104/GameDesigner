using Net.Event;
using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Client
{
    /// <summary>
    /// 双通讯(tcp, udp)客户端 当使用Send发送数据时, 使用udp协议发送. 当使用SendRT发送数据时, 使用tcp协议发送
    /// 当Send发送数据大于50000字节后内部自动转换为tcp协议发送
    /// </summary>
    [Serializable]
    public class NetworkClient : ClientBase
    {
        public Socket TcpClient { get; protected set; }
        /// <summary>
        /// 帧尾或叫数据长度(4) + CRC检验索引(1) + CRC校验码(1) + 玩家id(4) = 10
        /// </summary>
        protected new readonly int frame = 10;

        /// <summary>
        /// 构造双协议(tcp,udp)客户端
        /// </summary>
        public NetworkClient()
        {
        }

        /// <summary>
        /// 构造双协议(tcp,udp)客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public NetworkClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~NetworkClient()
        {
#if !UNITY_EDITOR
            Close();
#endif
        }

        protected override Task ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            return Task.Run(()=> 
            {
                try
                {
                    TcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建套接字
                    this.localPort = localPort;
                    if (localPort != -1)
                        TcpClient.Bind(new IPEndPoint(IPAddress.Any, localPort));
                    try
                    {
                        TcpClient.Connect(host, port);
                        OnConnected();
                        StartupThread();
                        var time = DateTime.Now.AddSeconds(5);
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
                    catch (Exception ex)
                    {
                        NDebug.Log("连接错误:" + ex.ToString());
                        TcpClient?.Close();
                        TcpClient = null;
                        InvokeContext((arg) => {
                            networkState = NetworkState.ConnectFailed;
                            result(false); 
                        });
                    }
                }
                catch (Exception ex)
                {
                    NDebug.Log("连接错误:" + ex.ToString());
                    InvokeContext((arg) => {
                        networkState = NetworkState.ConnectFailed;
                        result(false); 
                    });
                }
            });
        }

        private void OnConnected()
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Client.Connect(host, port);
        }

        protected override void BroadcastSend(Socket client, IPEndPoint ipEndPoint)
        {
            client.SendTo(new byte[] { 6, 0, 0, 0, 0, 0x2d, 0, 0, 0, 0, 74, NetCmd.Broadcast, 0, 0, 0, 0 }, ipEndPoint);
        }

        protected override void HeartHandle()
        {
            while (openClient & currFrequency < 10)
            {
                try
                {
                    Thread.Sleep(HeartInterval);//5秒发送一个心跳包
                    heart++;
                    if (!Connected)
                        Reconnection(10);
                    else
                        SendRT(NetCmd.SendHeartbeat, new byte[0]);//保活连接
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
            StartThread("TCPReceiveHandle", TCPReceiveHandle);
            StartThread("NetworkFlowHandle", NetworkFlowHandle);
            StartThread("CheckRpcHandle", CheckRpcHandle);
            StartThread("HeartHandle", HeartHandle);
            StartThread("VarSyncHandler", VarSyncHandler);
            if (!UseUnityThread)
                StartThread("UpdateHandle", UpdateHandle);
        }

        private void TCPReceiveHandle()
        {
            var buffer = BufferPool.Take(65507);
            while (Connected)
            {
                try
                {
                    Thread.Sleep(1);//安卓上如果设为0会卡死
                    int count = TcpClient.Receive(buffer);
                    receiveCount += count;
                    receiveAmount++;
                    heart = 0;
                    ResolveBuffer(buffer, 0, count, true);
                    revdLoopNum++;
                }
                catch (Exception ex)
                {
                    NetworkException(ex);
                }
            }
        }

        protected override void SendRTDataHandle()
        {
            SendDataHandle(rtRPCModels, true);
        }

        protected override void SendDataHandle(QueueSafe<RPCModel> rPCModels, bool reliable)
        {
            int count = rPCModels.Count;
            if (count <= 0)
                return;
            var stream = BufferPool.Take();
            WriteDataHead(stream);
            stream.Write(BitConverter.GetBytes(UID), 0, 4);//与其他协议不同的是, 双协议需要uid
            WriteDataBody(ref stream, rPCModels, count, reliable);
            byte[] buffer = SendData(stream);
            SendByteData(buffer, reliable);
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

        protected override void SendByteData(byte[] buffer, bool reliable)
        {
            sendCount += buffer.Length;
            sendAmount++;
            int count;
            if (reliable | buffer.Length > 50000)
                count = TcpClient.Send(buffer, 0, buffer.Length, SocketFlags.None);
            else
                count = Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
            if (count <= 0)
                OnSendErrorHandle?.Invoke(buffer, reliable);
        }

#if TEST
        internal void Test(byte[] buffer, int index, int count, bool isTcp) 
        {
            ResolveBuffer(buffer, index, count, isTcp);
        }
#endif

        protected override void ResolveBuffer(Segment buffer, int index, int count, bool isTcp)
        {
            if (!isTcp)
            {
                int size = BitConverter.ToInt32(buffer, index);
                int crcIndex = buffer[4];//CRC检验索引, 使用者自己改变CRCCode属性
                byte crcCode = buffer[5];//CRC校验码, 使用者自己改变CRCCode属性
                index += frame;
                if (index + size == count)
                {
                    if (!OnCRC(crcIndex, crcCode))
                        return;
                    DataHandle(buffer, index, size, count);
                }
            }
            else
            {
                BufferHandle(buffer, index, count);
            }
        }

        private void BufferHandle(Segment buffer, int index, int count)
        {
            if (stack > StackNumberMax)//不能一直叠包
            {
                stack = 0;
                NDebug.LogError($"请设置StackNumberMax属性, 叠包次数过高, 叠包数量达到{StackNumberMax}次以上...");
                return;
            }
            if (stack >= 1)
            {
                stack++;
                heart = 0;//如果传输大文件时, 一直不能解析状态下, 导致掉线问题
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
                count = (int)StackStream.Position;//.Length; //错误问题,不能用length, 这是文件总长度, 之前可能已经有很大一波数据
                buffer = BufferPool.Take(count);
                StackStream.Seek(0, SeekOrigin.Begin);
                StackStream.Read(buffer, 0, count);
            }
            while (index < count)
            {
                int size = BitConverter.ToInt32(buffer, index);
                int crcIndex = buffer[4];
                byte crcCode = buffer[5];
                int uid = BitConverter.ToInt32(buffer, 6);
                if (uid < 0 | size < 0 | size > StackBufferSize)//如果出现解析的数据包大小有问题，则不处理
                {
                    stack = 0;
                    NDebug.LogError($"数据错乱:uid:{uid} size:{size}");
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
            AbortedThread();
            Client?.Close();
            Client = null;
            TcpClient?.Close();
            TcpClient = null;
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
        /// udp压力测试
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="clientLen">测试客户端数量</param>
        /// <param name="dataLen">每个客户端数据大小</param>
        public static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                List<Socket> clients = new List<Socket>();
                for (int i = 0; i < clientLen; i++)
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(ip, port);
                    clients.Add(socket);
                }
                byte[] buffer = new byte[dataLen];
                using (MemoryStream stream = new MemoryStream(512))
                {
                    int crcIndex = 0;
                    byte crcCode = 0x2d;
                    stream.Write(new byte[4], 0, 4);
                    stream.WriteByte((byte)crcIndex);
                    stream.WriteByte(crcCode);
                    stream.Write(BitConverter.GetBytes(10001), 0, 4);
                    RPCModel rPCModel = new RPCModel(NetCmd.CallRpc, buffer);
                    stream.WriteByte((byte)(rPCModel.kernel ? 68 : 74));
                    stream.WriteByte(rPCModel.cmd);
                    stream.Write(BitConverter.GetBytes(rPCModel.buffer.Length), 0, 4);
                    stream.Write(rPCModel.buffer, 0, rPCModel.buffer.Length);
                    stream.Position = 0;
                    int len = (int)stream.Length - 10;
                    stream.Write(BitConverter.GetBytes(len), 0, 4);
                    stream.Position = len + 10;
                    buffer = stream.ToArray();
                }
                while (!cts.IsCancellationRequested)
                {
                    Thread.Sleep(31);
                    for (int i = 0; i < clients.Count; i++)
                    {
                        Buffer.BlockCopy(BitConverter.GetBytes(10000 + i), 0, buffer, 6, 4);
                        clients[i].Send(buffer);
                    }
                }
                for (int i = 0; i < clients.Count; i++)
                    clients[i].Close();
            }, cts.Token);
            return cts;
        }
    }
}