﻿namespace Net.Server
{
    using Net.Component;
    using Net.Event;
    using Net.Share;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// 网络玩家 - 当客户端连接服务器后都会为每个客户端生成一个网络玩家对象，(玩家对象由服务器管理) 2019.9.9
    /// <code>注意:不要试图new player出来, new出来后是没有作用的!</code>
    /// </summary>
    public class NetPlayer : IDisposable
    {
        /// <summary>
        /// Tcp套接字
        /// </summary>
        public Socket Client { get; set; }
        /// <summary>
        /// 存储UDP客户端终端
        /// </summary>
        public EndPoint RemotePoint { get; set; }
        /// <summary>
        /// 此玩家所在的场景ID
        /// </summary>
        public string sceneID = "MainScene";
        /// <summary>
        /// 客户端玩家的标识
        /// </summary>
        public string playerID = string.Empty;
        /// <summary>
        /// 玩家所在的场景实体
        /// </summary>
        public virtual object Scene { get; set; }
        /// <summary>
        /// 玩家rpc
        /// </summary>
        public Dictionary<string, RPCMethod> Rpcs { get; set; } = new Dictionary<string, RPCMethod>();
        /// <summary>
        /// 临时客户端持续时间: (内核使用):
        /// 未知客户端连接服务器, 长时间未登录账号, 未知客户端临时内存对此客户端回收, 并强行断开此客户端连接
        /// </summary>
        public DateTime LastTime { get; set; }
        /// <summary>
        /// 跳动的心
        /// </summary>
        internal byte heart = 0;
        /// <summary>
        /// 发送可靠数据缓冲
        /// </summary>
        internal MyDictionary<uint, MyDictionary<ushort, RTBuffer>> sendRTList = new MyDictionary<uint, MyDictionary<ushort, RTBuffer>>();
        /// <summary>
        /// 接收可靠数据缓冲
        /// </summary>
        internal HashSet<int> revdRTList = new HashSet<int>();
        internal QueueSafe<AckQueue> ackQueue = new QueueSafe<AckQueue>();
        internal int sendRTListCount;
        /// <summary>
        /// TCP叠包值， 0:正常 >1:叠包次数 >25:清空叠包缓存流
        /// </summary>
        internal int stack = 0;
        internal string stackStreamName;
        internal int stackIndex;
        internal int stackCount;
        /// <summary>
        /// TCP叠包临时缓存流
        /// </summary>
        internal FileStream stackStream;
        /// <summary>
        /// 用户唯一身份标识
        /// </summary>
        public int UserID { get; internal set; }
        internal QueueSafe<RPCModel> tcpRPCModels = new QueueSafe<RPCModel>();
        internal QueueSafe<RPCModel> udpRPCModels = new QueueSafe<RPCModel>();
        internal DateTime rtTime;
        /// <summary>
        /// 当前可靠帧, 比如服务器发送两帧可靠数据帧给客户端, 客户端只需要收到一个帧数据即可 (发送帧)
        /// </summary>
        internal uint sendReliableFrame;
        /// <summary>
        /// 当前可靠帧, 比如服务器发送两帧可靠数据帧给客户端, 客户端只需要收到一个帧数据即可 (接收帧)
        /// </summary>
        internal uint revdReliableFrame;
        public double currRto = 50;
        internal Dictionary<uint, FrameList> revdFrames = new Dictionary<uint, FrameList>();
        internal long fileStreamCurrPos;
        internal QueueSafe<RevdDataBuffer> revdDataBeProcessed = new QueueSafe<RevdDataBuffer>();
        internal QueueSafe<SendDataBuffer> sendDataBeProcessed = new QueueSafe<SendDataBuffer>();
        internal bool login;
        internal bool isDispose;
        /// <summary>
        /// 关闭发送数据, 当关闭发送数据后, 数据将会停止发送, (允许接收客户端数据,但不能发送!)
        /// </summary>
        public bool CloseSend { get; set; }
        /// <summary>
        /// tcp远程端口, 由于socket被关闭后无法访问RemoteEndPoint的问题
        /// </summary>
        internal EndPoint TcpRemoteEndPoint { get; set; }

        #region 创建网络客户端(玩家)
        /// <summary>
        /// 构造网络客户端
        /// </summary>
        public NetPlayer() { }

        /// <summary>
        /// 构造网络客户端，Tcp
        /// </summary>
        /// <param name="client">客户端套接字</param>
        public NetPlayer(Socket client)
        {
            Client = client;
            RemotePoint = client.RemoteEndPoint;
        }

        /// <summary>
        /// 构造网络客户端
        /// </summary>
        /// <param name="remotePoint"></param>
        public NetPlayer(EndPoint remotePoint)
        {
            RemotePoint = remotePoint;
        }
        #endregion

        #region 客户端释放内存
        /// <summary>
        /// 析构网络客户端
        /// </summary>
        ~NetPlayer()
        {
            Dispose();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Dispose()
        {
            if (isDispose)
                return;
            isDispose = true;
            Client?.Close();
            stackStream?.Close();
            stackStream = null;
            stack = 0;
            stackIndex = 0;
            stackCount = 0;
            sendReliableFrame = 0;
            revdReliableFrame = 0;
            CloseSend = true;
            fileStreamCurrPos = 0;
            heart = 0;
            revdFrames.Clear();
            Rpcs.Clear();
            sendRTList.Clear();
            revdRTList.Clear();
            tcpRPCModels = new QueueSafe<RPCModel>();//可能这个类并非真正释放, 而是在运行时数据库, 
            udpRPCModels = new QueueSafe<RPCModel>();//为了下次登录不出错,所以下线要清除在线时的发送数据
            login = false;
            if (File.Exists(stackStreamName) & !string.IsNullOrEmpty(stackStreamName))
                File.Delete(stackStreamName);
            stackStreamName = "";
            addressBuffer = null;
        }
        #endregion

        #region 客户端(玩家)Rpc(远程过程调用)处理
        /// <summary>
        /// 添加远程过程调用函数,从对象进行收集
        /// </summary>
        /// <param name="append">可以重复添加rpc?</param>
        public void AddRpc(bool append = false)
        {
            AddRpc(this, append);
        }

        /// <summary>
        /// 添加远程过程调用函数,从对象进行收集
        /// </summary>
        /// <param name="target"></param>
        /// <param name="append">可以重复添加rpc?</param>
        public void AddRpc(object target, bool append = false)
        {
            if (!append)
                foreach (RPCMethod o in Rpcs.Values)
                    if (o.target == target)
                        return;

            foreach (MethodInfo info in target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                RPCFun rpc = info.GetCustomAttribute<RPCFun>();
                if (rpc == null)
                    continue;
                if (Rpcs.ContainsKey(info.Name))
                {
                    throw new TargetException($"添加客户端私有Rpc错误！Rpc方法{info.Name}使用同一函数名，这是不允许的，字典键值无法添加相同的函数名！");
                }
                Rpcs.Add(info.Name, new RPCMethod(target, info, rpc.cmd));
            }
        }

        private byte[] addressBuffer;
        internal byte[] RemoteAddressBuffer()
        {
            if (addressBuffer != null)
                return addressBuffer;
            SocketAddress socketAddress = RemotePoint.Serialize();
            addressBuffer = (byte[])socketAddress.GetType().GetField("m_Buffer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(socketAddress);
            return addressBuffer;
        }

        /// <summary>
        /// 移除网络远程过程调用函数
        /// </summary>
        /// <param name="target">移除的rpc对象</param>
        public void RemoveRpc(object target)
        {
            RPCMethod[] rpcs = new RPCMethod[Rpcs.Count];
            Rpcs.Values.CopyTo(rpcs, 0);
            foreach (RPCMethod rpc in rpcs)
            {
                if (rpc.target == target | rpc.target.Equals(target) | rpc.target.Equals(null) | rpc.method.Equals(null))
                {
                    Rpcs.Remove(rpc.method.Name);
                }
            }
        }
        #endregion

        #region 客户端数据处理函数
        /// <summary>
        /// 当未知客户端发送数据请求，返回null，不添加到clients，返回对象，添加到clients中
        /// 客户端玩家的入口点，在这里可以控制客户端是否可以进入服务器与其他客户端进行网络交互
        /// 在这里可以用来判断客户端登录和注册等等进站许可
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool OnUnClientRequest(RPCModel model)
        {
            return true;
        }

        /// <summary>
        /// 当web服务器未知客户端发送数据请求，返回null，不添加到clients，返回对象，添加到clients中
        /// 客户端玩家的入口点，在这里可以控制客户端是否可以进入服务器与其他客户端进行网络交互
        /// 在这里可以用来判断客户端登录和注册等等进站许可
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool OnWSUnClientRequest(MessageModel model)
        {
            return true;
        }

        /// <summary>
        /// 当接收到客户端自定义数据请求,在这里可以使用你自己的网络命令，系列化方式等进行解析网络数据。（你可以在这里使用ProtoBuf或Json来解析网络数据）
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnRevdBufferHandle(RPCModel model) { }

        /// <summary>
        /// 当接收到webSocket客户端自定义数据请求,在这里可以使用你自己的网络命令，系列化方式等进行解析网络数据。（你可以在这里使用ProtoBuf或Json来解析网络数据）
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnWSRevdBuffer(MessageModel model) { }

        /// <summary>
        /// 当服务器判定客户端为断线或连接异常时，移除客户端时调用
        /// </summary>
        public virtual void OnRemoveClient() { }

        /// <summary>
        /// 当执行Rpc(远程过程调用函数)时, 提高性能可重写此方法进行指定要调用的函数
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnRpcExecute(RPCModel model)
        {
            RPCMethod rpc = Rpcs[model.func];
            rpc.Invoke(model.pars);
        }
        #endregion

        #region 提供简便的重写方法
        /// <summary>
        /// 当玩家登录成功初始化调用
        /// </summary>
        public virtual void OnStart()
        {
            NDebug.Log($"玩家[{playerID}]登录了游戏...");
        }

        /// <summary>
        /// 当玩家更新操作
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// 当玩家进入场景 ->场景对象在Scene属性
        /// </summary>
        public virtual void OnEnter() { }

        /// <summary>
        /// 当玩家退出场景 ->场景对象在Scene属性
        /// </summary>
        public virtual void OnExit() { }

        /// <summary>
        /// 当场景被移除 ->场景对象在Scene属性
        /// </summary>
        public virtual void OnRemove() { }

        /// <summary>
        /// 当接收到客户端使用Client.AddOperation方法发送的请求时调用. 如果重写此方法, 
        /// <code>返回false, 则服务器对象类会重新把操作列表加入到场景中, 你可以重写服务器的OnOperationSync方法让此次失效</code>
        /// <code>返回true, 服务器不再把数据加入到场景列表, 认为你已经在此处把数据处理了</code>
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual bool OnOperationSync(OperationList list) { return false; }
        #endregion

        /// <summary>
        /// 添加一个对象的所有Rpc方法
        /// </summary>
        /// <param name="target"></param>
        public void AddRpcHandle(object target)
        {
            List<RPCMethod> rpcs = NetBehaviour<NetPlayer, NetScene<NetPlayer>>.GetRpcs(target);
            foreach (RPCMethod rpc in rpcs)
            {
                if (!Rpcs.ContainsKey(rpc.Name))
                {
                    Rpcs.Add(rpc.Name, rpc);
                }
                else
                {
                    NDebug.LogWarning($"Rpc函数冲突(同名): {rpc.Name}, 请修改函数名称! Rpc函数名是唯一的...");
                }
            }
        }

        /// <summary>
        /// 此方法需要自己实现, 实现内容如下: Server.Instance.RemoveClient(this);
        /// </summary>
        public virtual void Close() { }

        public override string ToString()
        {
            return $"[玩家ID:{playerID}][用户ID:{UserID}][场景ID:{sceneID}][登录:{login}]";
        }
    }
}