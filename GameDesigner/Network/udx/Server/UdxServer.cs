﻿namespace Net.Server
{
    using Net.Share;
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net;
    using global::System.Runtime.InteropServices;
    using global::System.Text;
    using global::System.Threading;
    using Udx;
    using Debug = Event.NDebug;
    using Net.System;

    /// <summary>
    /// udx服务器类型  只能300人以下连接, 如果想要300个客户端以上, 请进入udx网址:www.goodudx.com 联系作者下载专业版FastUdxApi.dll, 然后更换下框架内的FastUdxApi.dll即可
    /// 第三版本 2020.9.14
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    public class UdxServer<Player, Scene> : ServerBase<Player, Scene>, IUDX where Player : UdxPlayer, new() where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// udx服务器对象
        /// </summary>
        public new IntPtr Server;
        private UDXPRC uDXPRC;
        private readonly ConcurrentDictionary<IntPtr, Player> peers = new ConcurrentDictionary<IntPtr, Player>();

        public override void Start(ushort port = 6666)
        {
            if (Server != IntPtr.Zero)//如果服务器套接字已创建
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
            if (!File.Exists(path + "\\FastUdxApi.dll"))
                throw new FileNotFoundException($"FastUdxApi.dll没有在程序根目录中! 请从GameDesigner文件夹下找到FastUdxApi.dll复制到{path}目录下.");
#endif
            if (!UdxLib.INIT)
            {
                UdxLib.INIT = true;
                UdxLib.UInit(8);
            }
            UdxLib.UDXS.Add(this);
            Server = UdxLib.UCreateFUObj();
            UdxLib.UBind(Server, null, port);
            uDXPRC = new UDXPRC(ProcessReceive);
            UdxLib.USetFUCB(Server, uDXPRC);
            GC.KeepAlive(uDXPRC);
            IsRunServer = true;
            Thread send = new Thread(SendDataHandle) { IsBackground = true, Name = "SendDataHandle" };
            send.Start();
            Thread suh = new Thread(SceneUpdateHandle) { IsBackground = true, Name = "SceneUpdateHandle" };
            suh.Start();
            ThreadManager.Invoke("DataTrafficThread", 1f, DataTrafficHandler);
            ThreadManager.Invoke("SingleHandler", SingleHandler);
            ThreadManager.Invoke("SyncVarHandler", SyncVarHandler);
            ThreadManager.Invoke("CheckHeartHandler", HeartInterval / 1000f, CheckHeartHandler, true);
            for (int i = 0; i < MaxThread; i++)
            {
                QueueSafe<RevdDataBuffer> revdQueue = new QueueSafe<RevdDataBuffer>();
                RevdQueues.Add(revdQueue);
                Thread revd = new Thread(RevdDataHandle) { IsBackground = true, Name = "RevdDataHandle" + i };
                revd.Start(revdQueue);
                threads.Add("RevdDataHandle" + i, revd);
                QueueSafe<SendDataBuffer> sendDataBeProcessed = new QueueSafe<SendDataBuffer>();
                SendQueues.Add(sendDataBeProcessed);
                Thread proSend = new Thread(ProcessSend) { IsBackground = true, Name = "ProcessSend" + i };
                proSend.Start(sendDataBeProcessed);
                threads.Add("ProcessSend" + i, proSend);
            }
            threads.Add("SendDataHandle", send);
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
            InitUserID();
        }

        protected void ProcessReceive(UDXEVENT_TYPE eventtype, int erro, IntPtr cli, IntPtr pData, int len)
        {
            try
            {
                switch (eventtype)
                {
                    case UDXEVENT_TYPE.E_CONNECT:
                        byte[] ipbytes = new byte[128];
                        int port = 0;
                        int ntype = 0;
                        UdxLib.USetGameMode(cli, true);
                        UdxLib.UGetRemoteAddr(cli, ipbytes, ref port, ref ntype);
                        port = UdxLib.UGetDesStreamID(cli);
                        string ip = Encoding.ASCII.GetString(ipbytes, 0, 128);
                        ip = ip.Replace("\0", "");
                        IPEndPoint remotePoint = new IPEndPoint(IPAddress.Parse(ip), port);
                        UserIDStack.TryPop(out int uid);
                        Player unClient = new Player();
                        unClient.Udx = cli;
                        unClient.UserID = uid;
                        unClient.PlayerID = uid.ToString();
                        unClient.Name = uid.ToString();
                        unClient.RemotePoint = remotePoint;
                        unClient.LastTime = DateTime.Now.AddMinutes(5);
                        unClient.isDispose = false;
                        unClient.CloseSend = false;
                        peers.TryAdd(cli, unClient);
                        Interlocked.Increment(ref ignoranceNumber);
                        unClient.revdQueue = RevdQueues[threadNum];
                        unClient.sendQueue = SendQueues[threadNum];
                        if (++threadNum >= RevdQueues.Count)
                            threadNum = 0;
                        AllClients.TryAdd(remotePoint, unClient);//之前放在上面, 由于接收线程并行, 还没赋值revdQueue就已经接收到数据, 导致提示内存池泄露
                        OnHasConnectHandle(unClient);
                        break;
                    case UDXEVENT_TYPE.E_LINKBROKEN:
                        if (peers.TryRemove(cli, out Player client2))
                            RemoveClient(client2);
                        break;
                    case UDXEVENT_TYPE.E_DATAREAD:
                        if (peers.TryGetValue(cli, out Player client1))
                        {
                            client1.heart = 0;
                            var buffer = BufferPool.Take(len);
                            buffer.Count = len;
                            Marshal.Copy(pData, buffer, 0, len);
                            receiveCount += len;
                            receiveAmount++;
                            client1.revdQueue.Enqueue(new RevdDataBuffer() { client = client1, buffer = buffer });
                        }
                        else
                        {
                            ProcessReceive(UDXEVENT_TYPE.E_CONNECT, erro, cli, pData, len);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected override bool IsInternalCommand(Player client, RPCModel model)
        {
            if (model.cmd == NetCmd.Connect | model.cmd == NetCmd.Broadcast)
                return true;
            return false;
        }

        protected override void SendRTDataHandle(Player client, QueueSafe<RPCModel> rtRPCModels)
        {
            SendDataHandle(client, rtRPCModels, true);
        }

        protected unsafe override void SendByteData(Player client, byte[] buffer, bool reliable)
        {
            if (client.Udx == IntPtr.Zero)
                return;
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            sendAmount++;
            sendCount += buffer.Length;
            fixed (byte* ptr = buffer)
            {
                int count = UdxLib.USend(client.Udx, ptr, buffer.Length);
                if (count <= 0)
                    OnSendErrorHandle?.Invoke(client, buffer, reliable);
            }
        }

        public override void RemoveClient(Player client)
        {
            base.RemoveClient(client);
            peers.TryRemove(client.Udx, out _);
        }

        public override void Close()
        {
            base.Close();
            if (Server != IntPtr.Zero)
            {
                UdxLib.UDestroyFUObj(Server);
                Server = IntPtr.Zero;
            }
            UdxLib.UDXS.Remove(this);
            if (UdxLib.UDXS.Count == 0)
            {
                UdxLib.UUnInit();
                UdxLib.INIT = false;
            }
        }

        ~UdxServer()
        {
            Close();
        }
    }
}