using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
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
    /// dotNetty客户端
    /// </summary>
    public class DotNettyClient : ClientBase
    {
        private IChannel channel;
        private IChannelPipeline pipeline;
        private MultithreadEventLoopGroup group;
        private EchoClientHandler clientHandler;

        public class EchoClientHandler : ChannelHandlerAdapter
        {
            private readonly DotNettyClient client;
            private readonly Action<Exception> disconnect;

            public EchoClientHandler(DotNettyClient client, Action<Exception> disconnect)
            {
                this.client = client;
                this.disconnect = disconnect;
            }

            public override void ChannelRead(IChannelHandlerContext context, object message)
            {
                if (message is IByteBuffer byteBuffer)
                {
                    client.ResolveBuffer(byteBuffer);
                }
            }

            public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

            public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
            {
                context.CloseAsync();
            }

            public override void HandlerRemoved(IChannelHandlerContext context)
            {
                disconnect(new SocketException((int)SocketError.SocketError));
            }
        }

        protected override Task ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(path + "\\libuv.dll"))
                throw new FileNotFoundException($"libuv.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 libuv.dll复制到{path}目录下.");
#endif
            return Task.Run(() =>
            {
                try
                {
                    group = new MultithreadEventLoopGroup();
                    Bootstrap bootstrap = new Bootstrap();
                    bootstrap
                        .Group(group)
                        .Channel<TcpSocketChannel>()
                        .Option(ChannelOption.TcpNodelay, true)
                        .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                        {
                            pipeline = channel.Pipeline;
                            clientHandler = new EchoClientHandler(this, NetworkException);
                            pipeline.AddLast("echo", clientHandler);
                            this.channel = channel;
                        }));
                    bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(host), port)).Wait(10000);
                    StartupThread();
                    InvokeContext((arg) => { result(true); });
                }
                catch (Exception)
                {
                    channel.CloseAsync();
                    group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                    Connected = false;
                    InvokeContext((arg) => { result(false); });
                }
            });
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
            fileStreamName = Path.GetTempFileName();
#endif
        }

        internal void ResolveBuffer(IByteBuffer buffer)
        {
            receiveCount += buffer.ReadableBytes;
            receiveAmount++;
            heart = 0;
            var buffer2 = new Segment(buffer.Array, buffer.ArrayOffset, buffer.ArrayOffset + buffer.ReadableBytes, false);
            ResolveBuffer(buffer2, buffer2.Index, buffer2.Count, false);
        }

        protected override void SendRTDataHandle()
        {
            SendDataHandle(rtRPCModels, true);
        }

        protected override void SendByteData(byte[] buffer, bool reliable)
        {
            sendCount += buffer.Length;
            sendAmount++;
            IByteBuffer byteBuffer = Unpooled.Buffer(buffer.Length);
            byteBuffer.WriteBytes(buffer);
            channel.WriteAndFlushAsync(byteBuffer);
        }

        public override void Close(bool await = true, int millisecondsTimeout = 1000)
        {
            base.Close(await, millisecondsTimeout);
            channel?.CloseAsync();
            group?.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// dotnetty压力测试
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
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    {
                        NoDelay = true
                    };
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
                        clients[i].Send(buffer);
                }
                for (int i = 0; i < clients.Count; i++)
                    clients[i].Close();
            }, cts.Token);
            return cts;
        }
    }
}
