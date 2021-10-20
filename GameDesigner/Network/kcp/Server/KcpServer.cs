namespace Net.Server
{
    using Kcp;
    using Net.Share;
    using global::System;
    using global::System.IO;
    using global::System.Net;
    using global::System.Runtime.InteropServices;
    using global::System.Threading;
    using static Kcp.KcpLib;
    using Net.System;

    /// <summary>
    /// kcp服务器
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    /// <typeparam name="Player"></typeparam>
    /// <typeparam name="Scene"></typeparam>
    public class KcpServer<Player, Scene> : ServerBase<Player, Scene> where Player : KcpPlayer, new() where Scene : NetScene<Player>, new()
    {
        public override void Start(ushort port = 6666)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(path + "\\kcp.dll"))
                throw new FileNotFoundException($"kcp.dll没有在程序根目录中! 请从GameDesigner文件夹下找到kcp.dll复制到{path}目录下.");
#endif
            base.Start(port);

            ikcp_Malloc = Ikcp_malloc_hook;
            ikcp_Free = Ikcp_Free;
            IntPtr mallocPtr = Marshal.GetFunctionPointerForDelegate(ikcp_Malloc);
            IntPtr freePtr = Marshal.GetFunctionPointerForDelegate(ikcp_Free);
            ikcp_allocator(mallocPtr, freePtr);
        }

        ikcp_malloc_hook ikcp_Malloc;
        ikcp_free_hook ikcp_Free;

        private unsafe IntPtr Ikcp_malloc_hook(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        private unsafe void Ikcp_Free(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }

        protected unsafe override void ReceiveProcessed(EndPoint remotePoint, Segment buffer, int count, bool tcp_udp)
        {
            if (!AllClients.TryGetValue(remotePoint, out Player client))//在线客户端  得到client对象
            {
                if (ignoranceNumber >= LineUp)//排队人数
                {
                    exceededNumber++;
                    OnExceededNumber(remotePoint);
                    return;
                }
                if (onlineNumber >= OnlineLimit)//服务器最大在线人数
                {
                    blockConnection++;
                    OnBlockConnection(remotePoint);
                    return;
                }
                exceededNumber = 0;
                blockConnection = 0;
                int uid = UserIDNumber;
                UserIDNumber++;
                client = ObjectPool<Player>.Take();
                client.UserID = uid;
                client.playerID = uid.ToString();
                client.RemotePoint = remotePoint;
                client.LastTime = DateTime.Now.AddMinutes(5);
                client.isDispose = false;
                client.CloseSend = false;
                IntPtr kcp = ikcp_create(1400, (IntPtr)1);
                IntPtr output = Marshal.GetFunctionPointerForDelegate(client.output);
                ikcp_setoutput(kcp, output);
                ikcp_wndsize(kcp, 1024, 1024);
                ikcp_nodelay(kcp, 1, 10, 2, 1);
                client.Kcp = kcp;
                OnHasConnectHandle(client);
                AllClients.TryAdd(remotePoint, client);
                Interlocked.Increment(ref ignoranceNumber);
                client.revdDataBeProcessed = RevdDataBeProcesseds[threadNum];
                client.sendDataBeProcessed = SendDataBeProcesseds[threadNum];
                if (++threadNum >= RevdDataBeProcesseds.Count)
                    threadNum = 0;
            }
            fixed (byte* p = &buffer.Buffer[0])
            {
                ikcp_input(client.Kcp, p, count);
            }
            ikcp_update(client.Kcp, (uint)Environment.TickCount);
            int len;
            while ((len = ikcp_peeksize(client.Kcp)) > 0)
            {
                var buffer1 = BufferPool.Take();//注意下面没有Push, 是因为还有处理线程在使用这个buffer, Push在Handle线程处理
                fixed (byte* p1 = &buffer1.Buffer[0])
                {
                    ikcp_recv(client.Kcp, p1, len);
                    client.revdDataBeProcessed.Enqueue(new RevdDataBuffer() { client = client, buffer = buffer1, count = len, tcp_udp = false });
                }
            }
            client.heart = 0;
            BufferPool.Push(buffer);
        }

        protected override void SendRTDataHandle(Player client, QueueSafe<RPCModel> rtRPCModels)
        {
            SendDataHandle(client, rtRPCModels, true);
        }

        protected unsafe override void SendByteData(Player client, byte[] buffer, bool reliable)
        {
            if (client.CloseSend)
                return;
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            sendAmount++;
            sendCount += buffer.Length;
            fixed (byte* p = &buffer[0])
            {
                int count = ikcp_send(client.Kcp, p, buffer.Length);
                ikcp_update(client.Kcp, (uint)Environment.TickCount);
                if (count < 0)
                    OnSendErrorHandle?.Invoke(client, buffer, reliable);
            }
        }
    }
}
