#if UNITY_STANDALONE_WIN || UNITY_WSA
namespace Net.Client
{
    using Net.Event;
    using Net.Share;
    using Newtonsoft_X.Json;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Text;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using WebSocket4Net;
    using Net.Serialize;
    using Net.System;

    /// <summary>
    /// web客户端类型
    /// 第三版本 2020.9.14
    /// </summary>
    [Serializable]
    public class WebClient : ClientBase
    {
        public WebSocket WSClient { get; private set; }

        /// <summary>
        /// 构造不可靠传输客户端
        /// </summary>
        public WebClient()
        {
        }

        /// <summary>
        /// 构造不可靠传输客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public WebClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~WebClient()
        {
#if !UNITY_EDITOR
            Close();
#endif
        }

        protected override Task ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            try
            {
                WSClient = new WebSocket($"ws://{host}:{port}/");
                WSClient.Opened += (o, e) =>
                {
                    Connected = true;
                };
                WSClient.Error += (o, e) =>
                {
                    NDebug.LogError(e);
                };
                WSClient.Closed += (o, e) =>
                {
                    Connected = false;
                    NetworkState = networkState = NetworkState.ConnectLost;
                    sendRTList.Clear();
                    revdRTList.Clear();
                    rtRPCModels = new QueueSafe<RPCModel>();
                    rPCModels = new QueueSafe<RPCModel>();
                    NDebug.Log("断开连接！");
                };
                WSClient.MessageReceived += (o, e) =>
                {
                    receiveCount += e.Message.Length * 2;
                    receiveAmount++;
                    MessageModel model = JsonConvert.DeserializeObject<MessageModel>(e.Message);
                    RPCModel model1 = new RPCModel(model.cmd, model.func, model.GetPars());
                    RPCDataHandle(model1);
                };
                WSClient.DataReceived += (o, e) =>
                {
                    receiveCount += e.Data.Length;
                    receiveAmount++;
                    var buffer = BufferPool.Take(e.Data.Length);
                    Buffer.BlockCopy(e.Data, 0, buffer, 0, e.Data.Length);
                    ResolveBuffer(buffer, 0, e.Data.Length, false);
                    BufferPool.Push(buffer);
                };
                WSClient.Open();
                return Task.Run(() =>
                {
                    DateTime timeout = DateTime.Now.AddSeconds(5);
                    while (!Connected & DateTime.Now < timeout) { Thread.Sleep(1); }
                    if (Connected)
                        StartupThread();
                    InvokeContext((arg) => { result(Connected); });
                });
            }
            catch (Exception ex)
            {
                NDebug.Log("连接错误: " + ex.ToString());
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
            fileStreamName = Path.GetTempFileName();
#endif
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
                    if (Connected)
                    {
                        Send(NetCmd.SendHeartbeat, new byte[0]);
                    }
                    else//尝试连接执行
                    {
                        Reconnection(10);
                    }
                }
                catch { }
            }
        }

        protected override void SendRTDataHandle()
        {
            SendDataHandle(rtRPCModels, true);
        }

        protected override void SendByteData(byte[] buffer, bool reliable)
        {
            sendCount += buffer.Length;
            sendAmount++;
            WSClient.Send(buffer, 0, buffer.Length);
        }

        protected internal override byte[] OnSerializeRpcInternal(RPCModel model)
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

        protected internal override FuncData OnDeserializeRpcInternal(byte[] buffer, int index, int count)
        {
            if (buffer[index++] == 32)
            {
                var message = Encoding.UTF8.GetString(buffer, index, count - 1);
                MessageModel model = JsonConvert.DeserializeObject<MessageModel>(message);
                return new FuncData(model.func, model.GetPars());
            }
            return NetConvert.Deserialize(buffer, index, count - 1);
        }

        public override void Close(bool await = true, int millisecondsTimeout = 1000)
        {
            Connected = false;
            openClient = false;
            NetworkState = networkState = NetworkState.ConnectClosed;
            AbortedThread();
            if (WSClient != null)
            {
                WSClient.Close();
                WSClient = null;
            }
            sendRTList.Clear();
            revdRTList.Clear();
            StackStream?.Close();
            stack = 0;
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
                List<WebSocket> clients = new List<WebSocket>();
                for (int i = 0; i < clientLen; i++)
                {
                    WebSocket socket = new WebSocket($"ws://{ip}:{port}/");
                    socket.Open();
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
                        clients[i].Send(buffer, 0, buffer.Length);
                }
                for (int i = 0; i < clients.Count; i++)
                    clients[i].Close();
            }, cts.Token);
            return cts;
        }
    }
}
#endif