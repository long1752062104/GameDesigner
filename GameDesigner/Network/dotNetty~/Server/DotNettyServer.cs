using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Debug = Net.Event.NDebug;

namespace Net.Server
{
    /// <summary>
    /// dotNetty服务器类
    /// </summary>
    /// <typeparam name="Player"></typeparam>
    /// <typeparam name="Scene"></typeparam>
    public class DotNettyServer<Player, Scene> : TcpServer<Player, Scene> where Player : DNPlayer, new() where Scene : NetScene<Player>, new()
    {
        private IEventLoopGroup bossGroup;
        private IEventLoopGroup workerGroup;

        public class EchoServerHandler : ChannelHandlerAdapter
        {
            private readonly Player client;

            public EchoServerHandler(Player client)
            {
                this.client = client;
            }

            public override void ChannelRead(IChannelHandlerContext context, object message)
            {
                if (message is IByteBuffer buffer)
                {
                    var segment = new Segment(buffer.Array, buffer.ArrayOffset, buffer.ArrayOffset + buffer.ReadableBytes, false);
                    Instance.SetRAC(segment.Count - segment.Index);
                    client.revdDataBeProcessed.Enqueue(new RevdDataBuffer() { client = client, buffer = segment, index = segment.Index, count = segment.Count, tcp_udp = true });
                }
            }

            public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

            public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
            {
                context.CloseAsync();
            }

            public override void HandlerRemoved(IChannelHandlerContext context)
            {
                client.CloseSend = true;
            }
        }

        public override void Start(ushort port = 6666)
        {
            if (IsRunServer)//如果服务器套接字已创建
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
            if (!File.Exists(path + "\\libuv.dll"))
                throw new FileNotFoundException($"libuv.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 libuv.dll复制到{path}目录下.");
#endif
            try
            {
                DispatcherEventLoopGroup dispatcher = new DispatcherEventLoopGroup();
                bossGroup = dispatcher;
                workerGroup = new WorkerEventLoopGroup(dispatcher, MaxThread / 2);
                ServerBootstrap bootstrap = new ServerBootstrap();
                bootstrap.Group(bossGroup, workerGroup);
                bootstrap.Channel<TcpServerChannel>();
                bootstrap.Option(ChannelOption.SoBacklog, 100)
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        lock (bootstrap)
                        {
                            IChannelPipeline pipeline = channel.Pipeline;
                            TcpChannel client = channel as TcpChannel;
                            Player unClient = ObjectPool<Player>.Take();
                            unClient.channel = channel;
                            unClient.LastTime = DateTime.Now.AddMinutes(5);
                            unClient.TcpRemoteEndPoint = client.RemoteAddress;
                            unClient.RemotePoint = client.RemoteAddress;
                            pipeline.AddLast("echo", new EchoServerHandler(unClient));
                            int uid = UserIDNumber;
                            UserIDNumber++;
                            unClient.UserID = uid;
                            unClient.playerID = uid.ToString();
                            unClient.stackStreamName = rootPath + $"/reliable/{Name}-" + uid + ".stream";
                            unClient.stackStream = new FileStream(unClient.stackStreamName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                            unClient.isDispose = false;
                            unClient.CloseSend = false;
                            OnHasConnectHandle(unClient);
                            AllClients.TryAdd(client.RemoteAddress, unClient);
                            Interlocked.Increment(ref ignoranceNumber);
                            byte[] uidbytes = BitConverter.GetBytes(uid);
                            byte[] identify = Encoding.Unicode.GetBytes(unClient.playerID);
                            byte[] buffer = new byte[identify.Length + 4];
                            Array.Copy(uidbytes, 0, buffer, 0, 4);
                            Array.Copy(identify, 0, buffer, 4, identify.Length);
                            SendRT(unClient, NetCmd.Identify, buffer);
                            unClient.revdDataBeProcessed = RevdDataBeProcesseds[threadNum];
                            unClient.sendDataBeProcessed = SendDataBeProcesseds[threadNum];
                            if (++threadNum >= RevdDataBeProcesseds.Count)
                                threadNum = 0;
                        }
                    }));
                bootstrap.BindAsync(port);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                Task.WhenAll(bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)), workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
                return;
            }
            IsRunServer = true;
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
            threads.Add("SendDataHandle", send);
            threads.Add("HeartUpdate", hupdate);
            threads.Add("DataTrafficThread", dtt);
            threads.Add("SceneUpdateHandle", suh);
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
                            IByteBuffer byteBuffer = Unpooled.Buffer(sendData.buffer.Length);
                            byteBuffer.WriteBytes(sendData.buffer);
                            client.channel.WriteAndFlushAsync(byteBuffer);
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

        protected override void HeartHandle()
        {
            foreach (var client in AllClients)
            {
                if (client.Value == null)
                    continue;
                if (client.Value.CloseSend)
                {
                    RemoveClient(client.Value);
                    continue;
                }
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
                SendRT(client.Value, NetCmd.SendHeartbeat, new byte[0]);//保活连接状态
            }
        }

        public override void Close()
        {
            base.Close();
            if (bossGroup != null & workerGroup != null)
                Task.WhenAll(bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)), workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
        }
    }
}
