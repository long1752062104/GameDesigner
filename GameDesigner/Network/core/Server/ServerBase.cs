/*版权所有（C）GDNet框架
*
*该软件按“原样”提供，不提供任何形式的明示或暗示担保，
*无论是由于软件，使用或其他方式产生的，侵权或其他形式的任何索赔，损害或其他责任，作者或版权所有者概不负责。
*
*允许任何人出于任何目的使用本框架，
*包括商业应用程序，并对其进行修改和重新发布自由
*
*受以下限制：
*
*  1. 不得歪曲本软件的来源；您不得
*声称是你写的原始软件。如果你用这个框架
*在产品中，产品文档中要确认感谢。
*  2. 更改的源版本必须清楚地标记来源于GDNet框架，并且不能
*被误传为原始软件。
*  3. 本通知不得从任何来源分发中删除或更改。
*/
namespace Net.Server
{
    using Net.Share;
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Reflection;
    using global::System.Text;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Debug = Event.NDebug;
    using Net.System;
    using Net.Serialize;

    /// <summary>
    /// 网络服务器核心基类 2019.11.22
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    public abstract class ServerBase<Player, Scene> : IServerHandle<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
        #region 属性
        /// <summary>
        /// (分布式)服务器名称
        /// </summary>
        public string Name { get; set; } = "GDNet";
        /// <summary>
        /// 分布式(集群)服务器区域名称
        /// </summary>
        public string AreaName { get; set; } = "电信1区";
        /// <summary>
        /// 服务器套接字
        /// </summary>
        public Socket Server { get; protected set; }
        /// <summary>
        /// IOCP套接字
        /// </summary>
        protected SocketAsyncEventArgs SocketAsync { get; set; }
        /// <summary>
        /// 远程过程调用委托
        /// </summary>
        protected List<RPCMethod> Rpcs { get; set; } = new List<RPCMethod>();
        /// <summary>
        /// 远程函数优化字典
        /// </summary>
        private readonly Dictionary<string, List<RPCMethod>> RpcsDic = new Dictionary<string, List<RPCMethod>>();
        /// <summary>
        /// 远程方法遮罩
        /// </summary>
        private readonly MyDictionary<ushort, string> RpcMaskDic = new MyDictionary<ushort, string>();
        /// <summary>
        /// 所有在线的客户端
        /// </summary>
        public List<Player> Clients
        {
            get
            {
                List<Player> unclients = new List<Player>();
                foreach (Player client in AllClients.Values)
                    if (client.Login) unclients.Add(client);
                return unclients;
            }
        }
        /// <summary>
        /// 所有在线的客户端 与<see cref="UIDClients"/>为互助字典 所添加的键值为<see cref="NetPlayer.playerID"/>
        /// </summary>
        public ConcurrentDictionary<string, Player> Players { get; private set; } = new ConcurrentDictionary<string, Player>();
        /// <summary>
        /// 所有在线的客户端 与<see cref="Players"/>为互助字典 所添加的键值为<see cref="NetPlayer.UserID"/>
        /// </summary>
        public ConcurrentDictionary<int, Player> UIDClients { get; private set; } = new ConcurrentDictionary<int, Player>();
        /// <summary>
        /// 未知客户端连接 或 刚连接服务器还未登录账号的IP
        /// </summary>
        public List<Player> UnClients
        {
            get
            {
                List<Player> unclients = new List<Player>();
                foreach (Player client in AllClients.Values)
                    if (!client.Login) unclients.Add(client);
                return unclients;
            }
        }
        /// <summary>
        /// 所有客户端列表
        /// </summary>
        public ConcurrentDictionary<EndPoint, Player> AllClients { get; private set; } = new ConcurrentDictionary<EndPoint, Player>();
        /// <summary>
        /// 服务器场景，key是场景名或房间名，关卡名。 value是(场景或房间，关卡等)对象
        /// </summary>
        public ConcurrentDictionary<string, Scene> Scenes { get; set; } = new ConcurrentDictionary<string, Scene>();
        /// <summary>
        /// 网络服务器单例
        /// </summary>
        public static ServerBase<Player, Scene> Instance { get; protected set; }
        /// <summary>
        /// 当前玩家在线人数
        /// </summary>
        public int OnlinePlayers { get { return onlineNumber; } }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort Port { get; protected set; }
        /// <summary>
        /// 服务器是否处于运行状态, 如果服务器套接字已经被释放则返回False, 否则返回True. 当调用Close方法后将改变状态
        /// </summary>
        public bool IsRunServer { get; set; }
        /// <summary>
        /// 网络发送频率, 正常情况是每秒60次 (FPS)
        /// </summary>
        [Obsolete("已弃用, 发送频率为1ms!", false)]
        public int SyncFrequency { get; set; } = 1;
        /// <summary>
        /// 网络场景同步时间(帧同步间隔), 默认每33毫秒同步一次, 一秒同步30次, 可自己设置
        /// </summary>
        public int SyncSceneTime { get; set; } = 33;
        /// <summary>
        /// 获取或设置最大可排队人数， 如果未知客户端人数超出LineUp值将不处理超出排队的未知客户端数据请求 ， 默认排队1000人
        /// </summary>
        public int LineUp { get; set; } = 1000;
        /// <summary>
        /// 允许玩家在线人数最大值（玩家在线上限）默认2000人同时在线
        /// </summary>
        public int OnlineLimit { get; set; } = 2000;
        /// <summary>
        /// 超出的排队人数，不处理的人数
        /// </summary>
        protected int exceededNumber;
        /// <summary>
        /// 服务器爆满, 阻止连接人数 与OnlineLimit属性有关
        /// </summary>
        protected int blockConnection;
        /// <summary>
        /// 服务器主场景名称
        /// </summary>
        public string MainSceneName { get; protected set; } = "MainScene";
        /// <summary>
        /// 网络统计发送数据长度/秒
        /// </summary>
        protected int sendCount;
        /// <summary>
        /// 网络统计发送次数/秒
        /// </summary>
        protected int sendAmount;
        /// <summary>
        /// 网络统计解析次数/秒
        /// </summary>
        protected int resolveAmount;
        /// <summary>
        /// 网络统计接收次数/秒
        /// </summary>
        protected int receiveAmount;
        /// <summary>
        /// 网络统计接收长度/秒
        /// </summary>
        protected int receiveCount;
        /// <summary>
        /// 发送线程循环次数 并发数,类似fps
        /// </summary>
        protected int sendLoopNum;
        /// <summary>
        /// 接收线程循环次数(FPS)
        /// </summary>
        protected int revdLoopNum;
        /// <summary>
        /// 帧尾或叫数据长度(4) + CRC检验索引(1) + CRC校验码(1) = 6
        /// </summary>
        protected readonly byte frame = 6;
        /// <summary>
        /// 允许叠包缓冲器最大值 默认可发送5242880(5M)的数据包
        /// </summary>
        public int StackBufferSize { get; set; } = 5242880;
        /// <summary>
        /// 允许叠包最大次数，如果数据包太大，接收数据的次数超出StackNumberMax值，则会清除叠包缓存器 默认可叠包50次
        /// </summary>
        public int StackNumberMax { get; set; } = 50;
        /// <summary>
        /// 心跳时间间隔, 默认每2秒检查一次玩家是否离线, 玩家心跳确认为5次, 如果超出5次 则移除玩家客户端. 确认玩家离线总用时10秒, 
        /// 如果设置的值越小, 确认的速度也会越快. 但发送的数据也会增加. [开发调式时尽量把心跳值设置高点]
        /// </summary>
        public int HeartInterval { get; set; } = 2000;
        /// <summary>
        /// <para>心跳检测次数, 默认为5次检测, 如果5次发送心跳给客户端或服务器, 没有收到回应的心跳包, 则进入断开连接处理</para>
        /// <para>当一直有数据往来时是不会发送心跳数据的, 只有当没有数据往来了, 才会进入发送心跳数据</para>
        /// </summary>
        public byte HeartLimit { get; set; } = 5;
        /// <summary>
        /// 由于随机数失灵导致死循环, 所以用计数来标记用户标识 (从10000开始标记)
        /// </summary>
        protected int UserIDNumber = 10000;
        /// <summary>
        /// 使用字节压缩吗? 如果使用: 当buffer数据大于1000时, 启用压缩功能
        /// </summary>
        public bool ByteCompression { get; set; }
        /// <summary>
        /// <para>（Maxium Transmission Unit）最大传输单元, 最大传输单元为1500字节, 这里默认为50000, 如果数据超过50000,则是该框架进行分片. 传输层则需要分片为50000/1472=34个数据片</para>
        /// <para>1.链路层：以太网的数据帧的长度为(64+18)~(1500+18)字节，其中18是数据帧的帧头和帧尾，所以数据帧的内容最大为1500字节（不包括帧头和帧尾），即MUT为1500字节</para>
        /// <para>2.网络层：IP包的首部要占用20字节，所以这里的MTU＝1500－20＝1480字节</para>
        /// <para>3.传输层：UDP包的首部要占有8字节，所以这里的MTU＝1480－8＝1472字节</para>
        /// <see langword="注意:服务器和客户端的MTU属性的值必须保持一致性,否则分包的数据将解析错误!"/> <see cref="Client.ClientBase.MTU"/>
        /// </summary>
        public int MTU { get; set; } = 1300;
        /// <summary>
        /// （Retransmission TimeOut）重传超时时间。 默认为50毫秒重传一次
        /// </summary>
        public int RTO { get; set; } = 50;
        /// <summary>
        /// 超时重传模式 默认为可变重传
        /// </summary>
        public RTOMode RTOMode { get; set; }
        /// <summary>
        /// (Maximum traffic per second) 每秒允许传输最大流量, 默认最大每秒可以传输1m大小
        /// </summary>
        public int MTPS { get; set; } = 1024 * 1024;
        /// <summary>
        /// 未知客户端人数, 即在线不登录账号的客户端
        /// </summary>
        public int UnClientNumber { get { return ignoranceNumber; } }
        /// <summary>
        /// 并发线程数量, 发送线程和接收处理线程数量
        /// </summary>
        public int MaxThread { get; set; } = 10;
        /// <summary>
        /// 在线客户端数量
        /// </summary>
        protected internal int onlineNumber;
        protected internal int ignoranceNumber;
        /// <summary>
        /// 是以太网? 此属性控制组包发送时,执行一次能把n个数据包组合在一起, 然后一次发送, 全由数据包大小决定. 如果此属性是以太网(true), 则根据mut来判断, 否则是局域网, 固定值50000字节
        /// </summary>
        public bool IsEthernet { get; set; }
        /// <summary>
        /// 组包数量，如果是一些小数据包，最多可以组合多少个？ 默认是组合1000个后发送
        /// </summary>
        public int PackageLength { get; set; } = 1000;
        /// <summary>
        /// 程序根路径, 网络数据缓存读写路径
        /// </summary>
        protected string rootPath;
        #endregion

        #region 服务器事件处理
        /// <summary>
        /// 开始运行服务器事件
        /// </summary>
        public Action OnStartingHandle { get; set; }
        /// <summary>
        /// 服务器启动成功事件
        /// </summary>
        public Action OnStartupCompletedHandle { get; set; }
        /// <summary>
        /// 当前有客户端连接触发事件
        /// </summary>
        public Action<Player> OnHasConnectHandle { get; set; }
        /// <summary>
        /// 当添加客户端到所有在线的玩家集合中触发的事件
        /// </summary>
        public Action<Player> OnAddClientHandle { get; set; }
        /// <summary>
        /// 当接收到自定义的网络指令时处理事件
        /// </summary>
        public virtual RevdBufferHandle<Player> OnRevdBufferHandle { get; set; }
        /// <summary>
        /// 当移除客户端时触发事件
        /// </summary>
        public Action<Player> OnRemoveClientHandle { get; set; }
        /// <summary>
        /// 当统计网络流量时触发
        /// </summary>
        public NetworkDataTraffic OnNetworkDataTraffic { get; set; }
        /// <summary>
        /// 当客户端在时间帧发送的操作数据， 当使用客户端的<see cref="Client.ClientBase.AddOperation(Operation)"/>方法时调用
        /// </summary>
        public Action<Player, OperationList> OnOperationSyncHandle { get; set; }
        /// <summary>
        /// 当客户端发送的大数据时, 可监听此事件显示进度值
        /// </summary>
        public Action<Player, RTProgress> OnRevdRTProgressHandle { get; set; }
        /// <summary>
        /// 当服务器发送可靠数据时, 可监听此事件显示进度值 (NetworkServer,TcpServer类无效)
        /// </summary>
        public Action<Player, RTProgress> OnSendRTProgressHandle { get; set; }
        /// <summary>
        /// 输出日志, 这里是输出全部日志(提示,警告,错误等信息). 如果想只输出指定的日志, 请使用NDebug类进行监听
        /// </summary>
        public Action<string> Log { get; set; }
        /// <summary>
        /// ping服务器回调 参数double为延迟毫秒单位 当<see cref="RTOMode"/>=<see cref="RTOMode.Variable"/>可变重传时, 内核将会每秒自动ping一次
        /// </summary>
        public Action<Player, double> OnPingCallback;
        /// <summary>
        /// 当socket发送失败调用.参数1:玩家对象, 参数2:发送的字节数组, 参数3:发送标志(可靠和不可靠)  ->可通过<see cref="SendByteData"/>方法重新发送
        /// </summary>
        public Action<Player, byte[], bool> OnSendErrorHandle;
        /// <summary>
        /// 当添加远程过程调用方法时调用， 参数1：要收集rpc特性的对象， 参数2：如果服务器的rpc中已经有了这个对象，还可以添加进去？
        /// </summary>
        public Action<object, bool> OnAddRpcHandle { get; set; }
        /// <summary>
        /// 当移除远程过程调用对象， 参数1：移除此对象的所有rpc方法
        /// </summary>
        public Action<object> OnRemoveRpc { get; set; }
        /// <summary>
        /// 当执行调用远程过程方法时触发
        /// </summary>
        public Action<Player, RPCModel> OnRPCExecute { get; set; }
        /// <summary>
        /// 当序列化远程过程调用方法
        /// </summary>
        public Func<RPCModel, byte[]> OnSerializeRPC { get;set; }
        /// <summary>
        /// 当反序列化远程过程调用方法
        /// </summary>
        public Func<byte[], int, int, FuncData> OnDeserializeRPC { get; set; }
        /// <summary>
        /// 当序列化远程过程调用操作
        /// </summary>
        public Func<OperationList, byte[]> OnSerializeOPT { get; set; }
        /// <summary>
        /// 当反序列化远程过程调用操作
        /// </summary>
        public Func<byte[], int, int, OperationList> OnDeserializeOPT { get; set; }
        /// <summary>
        /// 当开始下载文件时调用, 参数1(Player):下载哪个玩家上传的文件 参数2(string):客户端上传的文件名 返回值(string):开发者指定保存的文件路径(全路径名称)
        /// </summary>
        public Func<Player, string, string> OnDownloadFileHandle { get; set; }
        /// <summary>
        /// 当客户端发送的文件完成, 接收到文件后调用, 返回true:框架内部释放文件流和删除临时文件(默认) false:使用者处理
        /// </summary>
        public Func<Player, FileData, bool> OnReceiveFileHandle { get; set; }
        /// <summary>
        /// 当接收到发送的文件进度
        /// </summary>
        public Action<Player, RTProgress> OnRevdFileProgress { get; set; }
        /// <summary>
        /// 当发送的文件进度
        /// </summary>
        public Action<Player, RTProgress> OnSendFileProgress { get; set; }
        /// <summary>
        /// 可靠传输是排队模式? 排队模式下, 可靠包是一个一个处理. 不排队模式: 可靠传输数据组成多列并发 ---> 默认是无排队模式
        /// </summary>
        public bool Seqencing { get; set; }
        /// <summary>
        /// 服务器线程管理
        /// </summary>
        protected internal Dictionary<string, Thread> threads = new Dictionary<string, Thread>();
        #endregion

        /// <summary>
        /// 构造网络服务器函数
        /// </summary>
        public ServerBase()
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
            rootPath = UnityEngine.Application.persistentDataPath;
#else
            rootPath = AppDomain.CurrentDomain.BaseDirectory;
#endif
            Directory.CreateDirectory(rootPath + "/reliable/");
        }

#region 索引
        /// <summary>
        /// 玩家索引
        /// </summary>
        /// <param name="remotePoint"></param>
        /// <returns></returns>
        public Player this[EndPoint remotePoint] => AllClients[remotePoint];

        /// <summary>
        /// uid索引
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Player this[int uid] => UIDClients[uid];

        /// <summary>
        /// 场景索引
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        public Scene this[string sceneID] => Scenes[sceneID];

        /// <summary>
        /// 获得所有在线的客户端对象
        /// </summary>
        /// <returns></returns>
        public List<Player> GetClients()
        {
            List<Player> players = new List<Player>();
            foreach (Player p in AllClients.Values)
                if (p.Login)
                    players.Add(p);
            return players;
        }

        /// <summary>
        /// 获得所有服务器场景
        /// </summary>
        /// <returns></returns>
        public List<Scene> GetScenes()
        {
            return new List<Scene>(Scenes.Values);
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 当未知客户端发送数据请求，返回<see langword="false"/>，不允许<see langword="unClient"/>进入服务器!，如果返回的是<see langword="true"/>，则允许<see langword="unClient"/>客户端进入服务器
        /// 同时会将<see langword="unClient"/>添加到<see cref="Players"/>和<see cref="UIDClients"/>在线字典中.
        /// <code>客户端玩家的入口点，在这里可以控制客户端是否可以进入服务器与其他客户端进行网络交互</code>
        /// 在这里可以用来判断客户端登录和注册等等进站许可 (默认是允许进入服务器)
        /// </summary>
        /// <param name="unClient">尚未登录的客户端对象</param>
        /// <param name="model">数据模型</param>
        /// <returns></returns>
        protected virtual bool OnUnClientRequest(Player unClient, RPCModel model)
        {
            return true;
        }

        /// <summary>
        /// 当开始启动服务器
        /// </summary>
        protected virtual void OnStarting() { Debug.Log("服务器开始运行..."); }

        /// <summary>
        /// 当服务器启动完毕
        /// </summary>
        protected virtual void OnStartupCompleted() { Debug.Log("服务器启动成功!"); }

        /// <summary>
        /// 当添加默认网络场景，服务器初始化后会默认创建一个主场景，供所有玩家刚登陆成功分配的临时场景，默认初始化场景人数为1000人
        /// </summary>
        /// <returns>返回值string：网络玩家所在的场景名称 , 返回值NetScene：网络玩家的场景对象</returns>
        protected virtual KeyValuePair<string, Scene> OnAddDefaultScene()
        {
            return new KeyValuePair<string, Scene>(MainSceneName, new Scene { Name = MainSceneName, sceneCapacity = 1000 });
        }

        /// <summary>
        /// 当添加玩家到默认场景， 如果不想添加刚登录游戏成功的玩家进入主场景，可重写此方法让其失效
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnAddPlayerToScene(Player client)
        {
            if (Scenes.TryGetValue(MainSceneName, out Scene scene))
            {
                scene.AddPlayer(client);//将网络玩家添加到主场景集合中
            }
        }

        /// <summary>
        /// 当有客户端连接
        /// </summary>
        /// <param name="client">客户端套接字</param>
        protected virtual void OnHasConnect(Player client)
        {
            if (client.RemotePoint != null)
                Debug.Log("有客户端连接:" + client.RemotePoint.ToString());
            else if (client.Client != null)
                Debug.Log("有客户端连接:" + client.Client.RemoteEndPoint.ToString());
        }

        /// <summary>
        /// 当服务器判定客户端为断线或连接异常时，移除客户端时调用
        /// </summary>
        /// <param name="client">要移除的客户端</param>
        protected virtual void OnRemoveClient(Player client) { Debug.Log($"[{client.playerID}]断开连接...!"); }

        /// <summary>
        /// 当开始调用服务器RPC函数 或 开始调用自定义网络命令时 可设置请求客户端的client为全局字段，方便在服务器RPC函数内引用!!!
        /// 在多线程时有1%不安全，当出现client赋值到其他玩家对象时，可在网络方法加<see langword="[Rpc(NetCmd.SafeCall)]"/>特性
        /// </summary>
        /// <param name="client">发送请求数据的客户端</param>
        [Obsolete("请重写OnRpcExecute方法实现!")]
        protected virtual void OnInvokeRpc(Player client) { }

        /// <summary>
        /// 当接收到客户端自定义数据请求,在这里可以使用你自己的网络命令，系列化方式等进行解析网络数据。（你可以在这里使用ProtoBuf或Json来解析网络数据）
        /// </summary>
        /// <param name="client">当前客户端</param>
        /// <param name="model"></param>
        protected virtual void OnReceiveBuffer(Player client, RPCModel model) { }

        /// <summary>
        /// 当接收到客户端发送的文件
        /// </summary>
        /// <param name="client">当前客户端</param>
        /// <param name="fileData"></param>
        protected virtual bool OnReceiveFile(Player client, FileData fileData) { return true; }

        /// <summary>
        /// 当接收到客户端使用<see cref="Client.ClientBase.AddOperation(Operation)"/>方法发送的请求时调用
        /// </summary>
        /// <param name="client">当前客户端</param>
        /// <param name="list">操作列表</param>
        protected virtual void OnOperationSync(Player client, OperationList list)
        {
            if (client.OnOperationSync(list))
                return;
            if (Scenes.TryGetValue(client.sceneID, out Scene scene))
                scene.OnOperationSync(client, list);
        }

        /// <summary>
        /// 当客户端发送的大数据时, 可监听此事件显示进度值
        /// </summary>
        /// <param name="client"></param>
        /// <param name="progress"></param>
        protected virtual void OnRevdRTProgress(Player client, RTProgress progress) { }

        /// <summary>
        /// 当服务器发送的大数据时, 可监听此事件显示进度值
        /// </summary>
        /// <param name="client"></param>
        /// <param name="progress"></param>
        protected virtual void OnSendRTProgress(Player client, RTProgress progress) { }

        /// <summary>
        /// 当内核序列化远程函数时调用, 如果想改变内核rpc的序列化方式, 可重写定义序列化协议
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual byte[] OnSerializeRpc(RPCModel model) { return OnSerializeRPC(model); }

        protected internal byte[] OnSerializeRpcInternal(RPCModel model) { return NetConvert.Serialize(model); }

        /// <summary>
        /// 当内核解析远程过程函数时调用, 如果想改变内核rpc的序列化方式, 可重写定义解析协议
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected virtual FuncData OnDeserializeRpc(byte[] buffer, int index, int count) { return OnDeserializeRPC(buffer, index, count); }

        protected internal FuncData OnDeserializeRpcInternal(byte[] buffer, int index, int count) { return NetConvert.Deserialize(buffer, index, count); }
#endregion

        /// <summary>
        /// 运行服务器
        /// </summary>
        /// <param name="port">服务器端口号</param>
        public void Run(ushort port = 6666) => Start(port);

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="port">端口</param>
        public virtual void Start(ushort port = 6666)
        {
            if (Server != null)//如果服务器套接字已创建
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
            OnAddRpcHandle(this, true);
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
            Server.Bind(ip);
#if !UNITY_ANDROID//在安卓启动服务器时忽略此错误
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            Server.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);//udp远程关闭现有连接方案
#endif
            IsRunServer = true;
            UdpStartReceive();
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
            threads.Add("VarSyncHandler", vsh);
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

        /// <summary>
        /// 网络场景推动玩家同步更新处理线程, 如果想自己处理场景同步, 可重写此方法让同步失效
        /// </summary>
        protected virtual void SceneUpdateHandle()
        {
            while (IsRunServer)
            {
                try
                {
                    Thread.Sleep(SyncSceneTime);
                    Parallel.ForEach(Scenes.Values, scene =>
                    {
                        scene.Update(this);
                    });
                }
                catch (Exception ex)
                {
                    Debug.LogError("场景更新异常:" + ex);
                }
            }
        }

        /// <summary>
        /// 流量统计线程
        /// </summary>
        protected virtual void DataTrafficThread()
        {
            while (IsRunServer)
            {
                try
                {
                    Thread.Sleep(1000);
                    OnNetworkDataTraffic?.Invoke(sendAmount, sendCount, receiveAmount, receiveCount, resolveAmount, sendLoopNum, revdLoopNum);
                }
                finally
                {
                    sendCount = 0;
                    sendAmount = 0;
                    resolveAmount = 0;
                    receiveAmount = 0;
                    receiveCount = 0;
                    sendLoopNum = 0;
                    revdLoopNum = 0;
                }
            }
        }

        /// <summary>
        /// Udp开始接收数据
        /// </summary>
        private void UdpStartReceive()
        {
            SocketAsync = new SocketAsyncEventArgs { UserToken = Server };
            SocketAsync.Completed += OnIOCompleted;
            SocketAsync.SetBuffer(new byte[65507], 0, 65507);
            SocketAsync.RemoteEndPoint = Server.LocalEndPoint;
            Server.ReceiveFromAsync(SocketAsync);
        }

        protected virtual void OnIOCompleted(object sender, SocketAsyncEventArgs args)
        {
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    try
                    {
                        int count = args.BytesTransferred;
                        if (count > 0)
                        {
                            var buffer = BufferPool.Take();
                            Buffer.BlockCopy(args.Buffer, 0, buffer, 0, count);
                            receiveCount += count;
                            receiveAmount++;
                            EndPoint remotePoint = args.RemoteEndPoint;
                            ReceiveProcessed(remotePoint, buffer, count, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                    finally
                    {
                        if (Server != null & IsRunServer)
                            if (!Server.ReceiveFromAsync(args))
                                OnIOCompleted(null, args);
                    }
                    break;
            }
        }

        protected virtual void ReceiveProcessed(EndPoint remotePoint, Segment buffer, int count, bool tcp_udp)
        {
            if (!AllClients.TryGetValue(remotePoint, out Player client))//在线客户端  得到client对象
            {
                if (ignoranceNumber >= LineUp)//排队人数
                {
                    exceededNumber++;
                    OnExceededNumber(remotePoint);
                    BufferPool.Push(buffer);
                    return;
                }
                if (onlineNumber >= OnlineLimit)//服务器最大在线人数
                {
                    blockConnection++;
                    OnBlockConnection(remotePoint);
                    BufferPool.Push(buffer);
                    return;
                }
                exceededNumber = 0;
                blockConnection = 0;
                int uid = UserIDNumber;
                UserIDNumber++;
                //client = ObjectPool<Player>.Take();
                client = new Player();
                client.UserID = uid;
                client.playerID = uid.ToString();
                client.RemotePoint = remotePoint;
                client.LastTime = DateTime.Now.AddMinutes(5);
                client.isDispose = false;
                client.CloseSend = false;
                OnHasConnectHandle(client);
                AllClients.TryAdd(remotePoint, client);
                Interlocked.Increment(ref ignoranceNumber);
                client.revdDataBeProcessed = RevdDataBeProcesseds[threadNum];
                client.sendDataBeProcessed = SendDataBeProcesseds[threadNum];
                if (++threadNum >= RevdDataBeProcesseds.Count)
                    threadNum = 0;
            }
            client.revdDataBeProcessed.Enqueue(new RevdDataBuffer() { client = client, buffer = buffer, count = count, tcp_udp = tcp_udp });
        }

        protected volatile int threadNum;
        protected List<QueueSafe<RevdDataBuffer>> RevdDataBeProcesseds = new List<QueueSafe<RevdDataBuffer>>();
        protected List<QueueSafe<SendDataBuffer>> SendDataBeProcesseds = new List<QueueSafe<SendDataBuffer>>();

        protected virtual void RevdDataHandle(object state)//处理线程
        {
            QueueSafe<RevdDataBuffer> RevdDataBeProcessed = state as QueueSafe<RevdDataBuffer>;
            while (IsRunServer)
            {
                try
                {
                    revdLoopNum++;
                    int count = RevdDataBeProcessed.Count;
                    if (count <= 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        if (RevdDataBeProcessed.TryDequeue(out RevdDataBuffer revdData))
                        {
                            DataHandle(revdData.client as Player, revdData.buffer, revdData.index, revdData.count);
                            BufferPool.Push(revdData.buffer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("处理异常:" + ex);
                }
            }
        }

        protected virtual void DataHandle(Player client, Segment buffer, int index, int count)
        {
            int size = BitConverter.ToInt32(buffer, index + 0);
            int crcIndex = buffer[index + 4];//CRC检验索引
            byte crcCode = buffer[index + 5];//CRC校验码
            if (index + frame + size == count)//解析协议
            {
                if (!OnCRC(crcIndex, crcCode))
                    return;
                DataHandle(client, buffer, index + frame, size, count);
            }
        }

        protected virtual void DataHandle(Player client, Segment buffer, int index, int size, int count)
        {
            if (index + frame > count)
                return;
            if (buffer[index + 0] == 31 & buffer[index + 1] == 139 & buffer[index + 2] == 8)
            {
                buffer = UnZipHelper.Decompress(buffer, index, size);
                index = 0;
                count = buffer.Length;
            }
            while (index < count)
            {
                int kernelV = buffer[index];
                bool kernel = kernelV == 68;
                if (!kernel & kernelV != 74)
                {
                    Debug.LogError("[忽略]协议出错!");
                    break;
                }
                index++;
                byte cmd1 = buffer[index];
                index++;
                int dataCount = BitConverter.ToInt32(buffer, index);
                index += 4;
                if (index + dataCount > count)
                    break;
                RPCModel rpc = new RPCModel(cmd1, kernel, buffer, index, dataCount);
                if (kernel & cmd1 != NetCmd.Scene & cmd1 != NetCmd.SceneRT & cmd1 != NetCmd.Notice & cmd1 != NetCmd.NoticeRT & cmd1 != NetCmd.Local & cmd1 != NetCmd.LocalRT)
                {
                    FuncData func = OnDeserializeRpc(buffer, index, dataCount);
                    if (func.error)
                        break;
                    rpc.func = func.name;
                    rpc.pars = func.pars;
                    rpc.methodMask = func.mask;
                }
                index += dataCount;
                DataHandle(client, rpc);//解析协议完成
            }
        }

        protected virtual bool IsInternalCommand(Player client, RPCModel model)
        {
            if (model.cmd == NetCmd.Connect)
            {
                Server.SendTo(new byte[] { frame, 0, 0, 0, 0, 0x2d, 74, NetCmd.Connect, 0, 0, 0, 0 }, client.RemotePoint);
                return true;
            }
            if (model.cmd == NetCmd.Broadcast)
            {
                string hostName = Dns.GetHostName();
                IPHostEntry iPHostEntry = Dns.GetHostEntry(hostName);
                IPAddress ipAddress = IPAddress.Any;
                foreach (IPAddress ipAdd in iPHostEntry.AddressList)
                    if (ipAdd.AddressFamily.ToString() == "InterNetwork")
                        ipAddress = ipAdd;
                byte[] b = Encoding.Unicode.GetBytes(ipAddress.ToString());
                Server.SendTo(b, client.RemotePoint);
                return true;
            }
            return false;
        }

        protected virtual void DataHandle(Player client, RPCModel model)
        {
            if (IsInternalCommand(client, model))
                return;
            if (client.Login)
            {
                RpcDataHandle(client, model);
                return;
            }
            client.heart = 0;
            switch (model.cmd)
            {
                case NetCmd.ReliableTransport:
                    RpcDataHandle(client, model);
                    return;
                case NetCmd.ReliableCallback:
                    RpcDataHandle(client, model);
                    return;
                case NetCmd.SendHeartbeat:
                    Send(client, NetCmd.RevdHeartbeat, new byte[0]);
                    return;
                case NetCmd.RevdHeartbeat:
                    client.heart = 0;
                    return;
                case NetCmd.QuitGame:
                    RemoveClient(client);
                    return;
                case NetCmd.Ping:
                    return;
                case NetCmd.PingCallback:
                    return;
                case NetCmd.EntityRpc:
                    client.Login = client.OnUnClientRequest(model);
                    break;
                default:
                    client.Login = OnUnClientRequest(client, model);
                    break;
            }
            if (client.Login)//当有客户端连接时,如果允许用户添加此客户端
            {
                LoginInternal(client);
            }
        }

        /// <summary>
        /// 主动登录服务器, 类似OnUnClientRequest重写方法的返回值为true
        /// </summary>
        /// <param name="client"></param>
        protected void LoginHandle(Player client) 
        {
            if (!client.Login) 
            {
                client.Login = true;
                LoginInternal(client);
            }
        }

        private void LoginInternal(Player client) 
        {
            if (ignoranceNumber > 0)
                Interlocked.Decrement(ref ignoranceNumber);
            Interlocked.Increment(ref onlineNumber);
            Players.TryAdd(client.playerID, client);
            UIDClients.TryAdd(client.UserID, client);
            client.OnStart();
            OnAddPlayerToScene(client);
            client.AddRpc(client);
            OnAddClientHandle?.Invoke(client);
            byte[] uidbytes = BitConverter.GetBytes(client.UserID);
            byte[] identify = Encoding.Unicode.GetBytes(client.playerID);
            byte[] buffer = new byte[identify.Length + 4];
            Buffer.BlockCopy(uidbytes, 0, buffer, 0, 4);
            Buffer.BlockCopy(identify, 0, buffer, 4, identify.Length);
            SendRT(client, NetCmd.Identify, buffer);
        }

        protected virtual void RpcDataHandle(Player client, RPCModel model)
        {
            resolveAmount++;
            client.heart = 0;
            switch (model.cmd)
            {
                case NetCmd.EntityRpc:
                    client.OnRpcExecute(model);
                    break;
                case NetCmd.CallRpc:
                    OnRpcExecute(client, model);
                    break;
                case NetCmd.SafeCall:
                    OnRpcExecute(client, model);
                    break;
                case NetCmd.Local:
                    client.udpRPCModels.Enqueue(new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodMask));
                    break;
                case NetCmd.LocalRT:
                    client.tcpRPCModels.Enqueue(new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodMask));
                    break;
                case NetCmd.Scene:
                    if (!(client.Scene is Scene scene))
                    {
                        client.udpRPCModels.Enqueue(new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodMask));
                        return;
                    }
                    Multicast(scene.Clients, false, new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodMask));
                    break;
                case NetCmd.SceneRT:
                    if (!(client.Scene is Scene scene1))
                    {
                        client.tcpRPCModels.Enqueue(new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodMask));
                        return;
                    }
                    Multicast(scene1.Clients, true, new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodMask));
                    break;
                case NetCmd.Notice:
                    Multicast(Players.Values.ToList(), false, new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodMask));
                    break;
                case NetCmd.NoticeRT:
                    Multicast(Players.Values.ToList(), true, new RPCModel(model.cmd, model.Buffer, model.kernel, false, model.methodMask));
                    break;
                case NetCmd.SendHeartbeat:
                    Send(client, NetCmd.RevdHeartbeat, new byte[0]);
                    break;
                case NetCmd.RevdHeartbeat:
                    client.heart = 0;
                    break;
                case NetCmd.QuitGame:
                    RemoveClient(client);
                    break;
                case NetCmd.ReliableTransport:
                    ushort index = BitConverter.ToUInt16(model.buffer, model.index + 0);
                    ushort entry = BitConverter.ToUInt16(model.buffer, model.index + 2);
                    int count = BitConverter.ToInt32(model.buffer, model.index + 4);
                    int dataCount = BitConverter.ToInt32(model.buffer, model.index + 8);
                    uint frame = BitConverter.ToUInt32(model.buffer, model.index + 12);
                    byte[] rtbuffer = new byte[6];
                    if (client.revdReliableFrame > frame)
                    {
                        Buffer.BlockCopy(BitConverter.GetBytes(frame), 0, rtbuffer, 0, 4);
                        Buffer.BlockCopy(BitConverter.GetBytes(index), 0, rtbuffer, 4, 2);
                        Send(client, NetCmd.ReliableCallback, rtbuffer);//ack发送过程中丢失了, 需要补发
                        return;
                    }
                    if (client.stackStream == null)
                    {
                        client.stackStreamName = rootPath + $"/reliable/{Name}-" + client.UserID + ".stream";//解决当运行两个服务器以上时共用一个文件问题
                        client.stackStream = new FileStream(client.stackStreamName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    }
                    if (!client.revdFrames.ContainsKey(frame))
                    {
                        client.revdFrames.Add(frame, new FrameList(entry)
                        {
                            streamPos = client.fileStreamCurrPos,
                            frameLen = entry,
                            frame = frame,
                            dataCount = dataCount
                        });
                        client.fileStreamCurrPos += dataCount;
                        if (client.fileStreamCurrPos > (1024 * 1024 * 1024))//如果文件大于1g, 则从0开始记录
                            client.fileStreamCurrPos = 0;
                    }
                    FrameList revdFrame = client.revdFrames[frame];
                    if (revdFrame.Add(index))
                    {
                        client.stackStream.Seek(revdFrame.streamPos + (index * MTU), SeekOrigin.Begin);
                        client.stackStream.Write(model.buffer, model.index + 16, count);
                        InvokeRevdRTProgress(client, revdFrame.Count, entry);
                    }
                    Buffer.BlockCopy(BitConverter.GetBytes(frame), 0, rtbuffer, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(index), 0, rtbuffer, 4, 2);
                    Send(client, NetCmd.ReliableCallback, rtbuffer);
                    if (client.revdFrames.ContainsKey(client.revdReliableFrame))//排序执行
                        revdFrame = client.revdFrames[client.revdReliableFrame];
                    else //让客户端发送revdReliableFrame帧的所有帧数据
                    {
                        rtbuffer = new byte[4];
                        Buffer.BlockCopy(BitConverter.GetBytes(client.revdReliableFrame), 0, rtbuffer, 0, 4);
                        Send(client, NetCmd.TakeFrameList, rtbuffer);
                        return;
                    }
                    while (revdFrame.Count >= revdFrame.frameLen)
                    {
                        client.revdFrames.Remove(client.revdReliableFrame);
                        client.revdReliableFrame++;
                        var buffer = BufferPool.Take(revdFrame.dataCount);
                        client.stackStream.Seek(revdFrame.streamPos, SeekOrigin.Begin);
                        client.stackStream.Read(buffer, 0, revdFrame.dataCount);
                        ReliableTransportComplete(client, buffer, revdFrame.dataCount);
                        BufferPool.Push(buffer);
                        if (client.revdFrames.ContainsKey(client.revdReliableFrame))
                            revdFrame = client.revdFrames[client.revdReliableFrame];
                        else break;//结束死循环
                    }
                    for (ushort i = 0; i < revdFrame.frameLen; i++)
                    {
                        if (!revdFrame.ContainsKey(i))
                        {
                            if (DateTime.Now < revdFrame.time)
                                continue;
                            revdFrame.time = DateTime.Now.AddMilliseconds(client.currRto);
                            rtbuffer = new byte[6];
                            Buffer.BlockCopy(BitConverter.GetBytes(client.revdReliableFrame), 0, rtbuffer, 0, 4);
                            Buffer.BlockCopy(BitConverter.GetBytes(i), 0, rtbuffer, 4, 2);
                            Send(client, NetCmd.TakeFrame, rtbuffer);
                        }
                    }
                    break;
                case NetCmd.ReliableCallback:
                    uint frame1 = BitConverter.ToUInt32(model.buffer, model.index + 0);
                    ushort index1 = BitConverter.ToUInt16(model.buffer, model.index + 4);
                    client.ackQueue.Enqueue(new AckQueue(frame1, index1));
                    break;
                case NetCmd.TakeFrame:
                    uint frame2 = BitConverter.ToUInt32(model.buffer, model.index + 0);
                    ushort index2 = BitConverter.ToUInt16(model.buffer, model.index + 4);
                    if (client.sendRTList.TryGetValue(frame2, out MyDictionary<ushort, RTBuffer> rtbuffer1))
                    {
                        if (rtbuffer1.TryGetValue(index2, out RTBuffer buffer2))
                        {
                            buffer2.time = default;
                        }
                    }
                    break;
                case NetCmd.TakeFrameList:
                    uint frame3 = BitConverter.ToUInt32(model.buffer, model.index + 0);
                    if (client.sendRTList.TryGetValue(frame3, out MyDictionary<ushort, RTBuffer> rtbuffer2))
                    {
                        var entries = rtbuffer2.entries;
                        for (int i = 0; i < entries.Length; i++)
                        {
                            if (entries[i].hashCode >= 0)
                            {
                                var buffer2 = entries[i].value;
                                if (buffer2 == null)//出现null, 则为客户端已经被Dispose
                                    continue;
                                buffer2.time = default;
                            }
                        }
                    }
                    break;
                case NetCmd.OperationSync:
                    OperationList list = OnDeserializeOpt(model.buffer, model.index, model.count);
                    OnOperationSyncHandle(client, list);
                    break;
                case NetCmd.Ping:
                    client.udpRPCModels.Enqueue(new RPCModel(NetCmd.PingCallback, model.Buffer, model.kernel, false, model.methodMask));
                    break;
                case NetCmd.PingCallback:
                    long ticks = BitConverter.ToInt64(model.buffer, model.index);
                    DateTime time = new DateTime(ticks);
                    client.currRto = DateTime.Now.Subtract(time).TotalMilliseconds + 100d;
                    OnPingCallback?.Invoke(client, (client.currRto - 100d) / 2);
                    break;
                case NetCmd.P2P:
                    int uid = BitConverter.ToInt32(model.buffer, model.index);
                    if(UIDClients.TryGetValue(uid, out Player player)){
                        Segment segment = new Segment(new byte[10], 0, 10, false);
                        IPEndPoint iPEndPoint = player.RemotePoint as IPEndPoint;
#pragma warning disable CS0618 // 类型或成员已过时
                        segment.WriteValue(iPEndPoint.Address.Address);
#pragma warning restore CS0618 // 类型或成员已过时
                        segment.WriteValue(iPEndPoint.Port);
                        SendRT(client, NetCmd.P2P, segment.ToArray(false));
                    }
                    break;
                case NetCmd.VarSync:
                    Segment segment1 = new Segment(model.buffer, model.index, model.count, false);
                    while (segment1.Position < segment1.Index + segment1.Count)
                    {
                        var id = segment1.ReadValue<ushort>();
                        if (client.varSyncInfos.TryGetValue(id, out VarSyncInfo varSync))
                        {
                            var value = segment1.ReadValue(varSync.type);
                            varSync.SetValue(value);
                        }
                        else 
                        {
                            Debug.LogWarning($"未定义同步变量ID={id}, 请定义或收集同步变量, 使用{typeof(Player)}.AddRpc(xxx)方法收集!");
                            break;
                        }
                    }
                    break;
                case NetCmd.SendFile:
                    {
                        Segment segment = new Segment(model.Buffer, false);
                        var key = segment.ReadValue<int>();
                        var length = segment.ReadValue<long>();
                        var fileName = segment.ReadValue<string>();
                        var buffer = segment.ReadArray<byte>();
                        if (!client.ftpDic.TryGetValue(key, out FileData fileData))
                        {
                            fileData = new FileData();
                            string path;
                            if (OnDownloadFileHandle != null)
                            {
                                path = OnDownloadFileHandle(client, fileName);
                                var path1 = Path.GetDirectoryName(path);
                                if (!Directory.Exists(path1))
                                {
                                    Debug.LogError("文件不存在! 或者文件路径字符串编码错误! 提示:可以使用Notepad++查看, 编码是ANSI,不是UTF8");
                                    return;
                                }
                            }
                            else
                            {
                                path = Path.GetTempFileName();
                            }
                            fileData.fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                            fileData.fileName = fileName;
                            client.ftpDic.Add(key, fileData);
                        }
                        fileData.fileStream.Write(buffer, 0, buffer.Length);
                        fileData.Length += buffer.Length;
                        if (fileData.Length >= length)
                        {
                            client.ftpDic.Remove(key);
                            OnRevdFileProgress?.Invoke(client, new RTProgress(fileName, fileData.Length / (float)length * 100f, RTState.Complete));
                            fileData.fileStream.Position = 0;
                            if (OnReceiveFileHandle(client, fileData))
                            {
                                fileData.fileStream.Close();
                                File.Delete(fileData.fileStream.Name);
                            }
                        }
                        else 
                        {
                            segment.Position = 0;
                            segment.WriteValue(key);
                            SendRT(client, NetCmd.Download, segment.ToArray());
                            OnRevdFileProgress?.Invoke(client, new RTProgress(fileName, fileData.Length / (float)length * 100f, RTState.Download));
                        }
                    }
                    break;
                case NetCmd.Download:
                    {
                        Segment segment = new Segment(model.Buffer, false);
                        var key = segment.ReadValue<int>();
                        if (client.ftpDic.TryGetValue(key, out FileData fileData))
                            SendFile(client, key, fileData);
                    }
                    break;
                default:
                    client.OnRevdBufferHandle(model);
                    OnRevdBufferHandle(client, model);
                    break;
            }
        }

        protected virtual byte[] OnSerializeOpt(OperationList list)
        {
            return OnSerializeOPT(list);
        }

        protected internal byte[] OnSerializeOptInternal(OperationList list)
        {
            var segment = BufferPool.Take();
            using (MemoryStream stream = new MemoryStream(segment))
            {
                stream.SetLength(0);
                ProtoBuf.Serializer.Serialize(stream, list);
                var buffer = stream.ToArray();
                BufferPool.Push(segment);
                return buffer;
            }
        }

        protected virtual OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            return OnDeserializeOPT(buffer, index, count);
        }

        protected internal OperationList OnDeserializeOptInternal(byte[] buffer, int index, int count)
        {
            using (MemoryStream stream = new MemoryStream(buffer, index, count))
            {
                OperationList list = ProtoBuf.Serializer.Deserialize<OperationList>(stream);
                return list;
            }
        }

        protected virtual void ReliableTransportComplete(Player client, Segment buffer, int count)//为了与NetworkServer协议接轨增加的方法
        {
            DataHandle(client, buffer, 0, count, count);
        }

        protected void InvokeRevdRTProgress(Player client, int currValue, int dataCount)
        {
            float bfb = currValue / (float)dataCount * 100f;
            RTProgress progress = new RTProgress(bfb, RTState.Sending);
            OnRevdRTProgressHandle(client, progress);
        }

        protected void InvokeSendRTProgress(Player client, int currValue, int dataCount)
        {
            float bfb = currValue / (float)dataCount * 100f;
            RTProgress progress = new RTProgress(bfb, RTState.Sending);
            OnSendRTProgressHandle(client, progress);
        }

        /// <summary>
        /// 当服务器连接人数溢出时调用
        /// </summary>
        /// <param name="remotePoint"></param>
        protected virtual void OnExceededNumber(EndPoint remotePoint)
        {
            Debug.Log("未知客户端排队爆满,阻止连接次数: " + exceededNumber);
            Server.SendTo(new byte[] { frame, 0, 0, 0, 0, 0x2d, 74, NetCmd.ExceededNumber, 0, 0, 0, 0 }, 0, remotePoint);
        }

        /// <summary>
        /// 当服务器爆满时调用
        /// </summary>
        /// <param name="remotePoint"></param>
        protected virtual void OnBlockConnection(EndPoint remotePoint)
        {
            Debug.Log("服务器爆满,阻止连接次数: " + blockConnection);
            Server.SendTo(new byte[] { frame, 0, 0, 0, 0, 0x2d, 74, NetCmd.BlockConnection, 0, 0, 0, 0 }, 0, remotePoint);
        }

        protected virtual void SendDataHandle()//发送线程
        {
            Player[] allClients = new Player[0];
            while (IsRunServer)
            {
                try
                {
                    Thread.Sleep(1);
                    if (allClients.Length != AllClients.Count)
                        allClients = AllClients.Values.ToArray();
                    Parallel.ForEach(allClients, client =>
                    {
                        SendDirect(client);
                    });
                    sendLoopNum++;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("发送异常:" + ex);
                }
            }
        }

        /// <summary>
        /// 立刻发送, 不需要等待帧时间 (当你要强制把客户端下线时,你还希望客户端先发送完数据后,再强制客户端退出游戏用到)
        /// </summary>
        /// <param name="client"></param>
        public virtual void SendDirect(Player client)
        {
            SendDataHandle(client, client.udpRPCModels, false);//不可靠发送
            SendRTDataHandle(client, client.tcpRPCModels);//可靠发送
        }

        protected virtual void SendRTDataHandle(Player client, QueueSafe<RPCModel> rtRPCModels)
        {
            if (RTOMode == RTOMode.Fixed)
                client.currRto = RTO;//一秒最快发送50次 rto (重传超时时间) 如果网络拥堵, 别耽误事, 赶紧重传
            int rtcount = client.sendRTList.Values.Count;
            if ((rtcount > 0 & Seqencing) | (rtcount > 0 & client.sendRTListCount > 5) | rtcount > 100)// mtu(1400) * 5
                goto JUMP;
            int count = rtRPCModels.Count;
            if (count == 0 & rtcount > 0)
                goto JUMP;
            if (count == 0)
                return;
            if (count > 1000)
                count = 1000;
            var stream = BufferPool.Take();
            WriteDataBody(client, ref stream, rtRPCModels, count, true);
            int len = stream.Position;
            int index = 0;
            ushort dataIndex = 0;
            float dataCount = (float)len / MTU;
            var rtDic = new MyDictionary<ushort, RTBuffer>();
            client.sendRTList.Add(client.sendReliableFrame, rtDic);
            var stream1 = BufferPool.Take();
            int crcIndex = RandomHelper.Range(0, 256);
            byte crcCode = CRCCode[crcIndex];
            stream1.SetPositionLength(4);
            stream1.WriteByte((byte)crcIndex);
            stream1.WriteByte(crcCode);
            stream1.WriteByte(74);
            stream1.WriteByte(NetCmd.ReliableTransport);
            while (index < len)
            {
                int count1 = MTU;
                if (index + count1 >= len)
                    count1 = len - index;
                stream1.SetPositionLength(8);
                stream1.Write(BitConverter.GetBytes(count1 + 16), 0, 4);
                stream1.Write(BitConverter.GetBytes(dataIndex), 0, 2);
                stream1.Write(BitConverter.GetBytes((ushort)Math.Ceiling(dataCount)), 0, 2);
                stream1.Write(BitConverter.GetBytes(count1), 0, 4);
                stream1.Write(BitConverter.GetBytes(len), 0, 4);
                stream1.Write(BitConverter.GetBytes(client.sendReliableFrame), 0, 4);
                stream1.Write(stream, index, count1);
                byte[] buffer = SendData(stream1);
                rtDic.Add(dataIndex, new RTBuffer(buffer));
                index += MTU;
                dataIndex++;
            }
            BufferPool.Push(stream);
            BufferPool.Push(stream1);
            client.sendRTListCount = rtDic.Count;
            client.sendReliableFrame++;
        JUMP:
            count = client.ackQueue.Count;
            for (int i = 0; i < count; i++)
            {
                if (!client.ackQueue.TryDequeue(out AckQueue ack))
                    continue;
                if (!client.sendRTList.ContainsKey(ack.frame))
                    continue;
                var rtlist = client.sendRTList[ack.frame];
                if (!rtlist.ContainsKey(ack.index))
                    continue;
                rtlist.Remove(ack.index);
                if (rtlist.Count == 0)
                    client.sendRTList.Remove(ack.frame);
                InvokeSendRTProgress(client, client.sendRTListCount - rtlist.Count, client.sendRTListCount);
            }
            int bytesLen = 0;
            var entries = client.sendRTList.entries;
            for (int i = 0; i < client.sendRTList.count; i++)
            {
                if (entries[i].hashCode >= 0)
                {
                    var rtlist = entries[i].value;
                    if (rtlist == null)//出现null, 则为客户端已经被Dispose
                        return;
                    foreach (var list in rtlist)
                    {
                        RTBuffer rtb = list.Value;
                        if (DateTime.Now < rtb.time)
                            continue;
                        rtb.time = DateTime.Now.AddMilliseconds(client.currRto);
                        bytesLen += rtb.buffer.Length;
                        SendByteData(client, rtb.buffer, true);
                        if (bytesLen > MTPS / 1000)//一秒最大发送1m数据, 这里会有可能每秒执行1000次
                            return;
                    }
                }
            }
        }

        protected virtual void WriteDataHead(Segment stream)
        {
            int crcIndex = RandomHelper.Range(0, 256);
            byte crcCode = CRCCode[crcIndex];
            stream.Position += 4;//size
            stream.WriteByte((byte)crcIndex);
            stream.WriteByte(crcCode);
        }

        protected virtual void WriteDataBody(Player client, ref Segment stream, QueueSafe<RPCModel> rPCModels, int count, bool reliable)
        {
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                if (!rPCModels.TryDequeue(out RPCModel rPCModel))
                    continue;
                if (rPCModel.kernel & rPCModel.serialize)
                {
                    rPCModel.buffer = OnSerializeRpc(rPCModel);
                    if (rPCModel.buffer.Length == 0)
                        continue;
                }
                int len = stream.Count + rPCModel.buffer.Length + frame;
                if (len > BufferPool.Size)
                {
                    var stream2 = BufferPool.Take(len);
                    stream2.Write(stream, 0, stream.Count);
                    BufferPool.Push(stream);
                    stream = stream2;
                }
                if ((len >= (IsEthernet ? MTU : 50000) | ++index >= PackageLength) & !reliable)//这里的判断是第二次for以上生效
                {
                    byte[] buffer = SendData(stream);
                    SendByteData(client, buffer, reliable);
                    index = 0;
                    stream.SetPositionLength(frame);
                }
                stream.WriteByte((byte)(rPCModel.kernel ? 68 : 74));
                stream.WriteByte(rPCModel.cmd);
                stream.Write(BitConverter.GetBytes(rPCModel.buffer.Length), 0, 4);
                stream.Write(rPCModel.buffer, 0, rPCModel.buffer.Length);
                if (rPCModel.bigData)
                    break;
            }
        }

        protected virtual void SendDataHandle(Player client, QueueSafe<RPCModel> rPCModels, bool reliable)
        {
            int count = rPCModels.Count;//源码中Count执行也不少, 所以优化一下   这里已经取出要处理的长度
            if (count <= 0)
                return;
            var stream = BufferPool.Take();
            WriteDataHead(stream);
            WriteDataBody(client, ref stream, rPCModels, count, reliable);
            byte[] buffer = SendData(stream);
            SendByteData(client, buffer, reliable);
            BufferPool.Push(stream);
        }

        protected virtual byte[] SendData(Segment stream)
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

        protected virtual void SendByteData(Player client, byte[] buffer, bool reliable)
        {
            if (client.sendDataBeProcessed.Count >= 268435456)//最大只能处理每秒256m数据
            {
                Debug.LogError("发送缓冲列表已经超出限制!");
                return;
            }
            if (buffer.Length == frame)//解决长度==6的问题(没有数据)
                return;
            if (buffer.Length <= 65507)
                client.sendDataBeProcessed.Enqueue(new SendDataBuffer(client, buffer));
            else
                Debug.LogError("数据太大! 请使用SendRT");
        }

        protected unsafe virtual void ProcessSend(object state)
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
#if WINDOWS
                            fixed (byte* ptr = sendData.buffer)
                                Win32KernelAPI.sendto(Server.Handle, ptr, sendData.buffer.Length, SocketFlags.None, client.RemoteAddressBuffer(), 16);
#else
                            Server.SendTo(sendData.buffer, 0, sendData.buffer.Length, SocketFlags.None, client.RemotePoint);
#endif
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

        /// <summary>
        /// CRC校验代码表, 使用者可自行改变CRC校验码, 直接改源代码，重要提示：客户端的CRC表必须和这里的CRC表保持一致
        /// </summary>
        protected readonly byte[] CRCCode = new byte[]
        {
            0x2d, 0x9e, 0x2e, 0xbe, 0x29, 0x5e, 0x0e, 0x64, 0x30, 0xcb, 0xe5, 0xce, 0x0c, 0x4e,
            0xe8, 0x4d, 0x87, 0xf0, 0x14, 0xcd, 0x24, 0x3a, 0x4a, 0xe7, 0x73, 0x75, 0x3d, 0x85,
            0xa7, 0xde, 0x95, 0x23, 0x25, 0x07, 0x11, 0x1d, 0x82, 0x28, 0x33, 0x2c, 0xeb, 0xa5,
            0x31, 0xf3, 0x91, 0xf6, 0x5c, 0x69, 0xf5, 0xa3, 0x32, 0x26, 0xd7, 0x84, 0x3e, 0x49,
            0x77, 0xbb, 0x3b, 0xfc, 0x9b, 0xfd, 0xc0, 0xb0, 0x08, 0xb4, 0x62, 0xe4, 0x8e, 0xa6,
            0xb9, 0x18, 0xef, 0xc6, 0x46, 0xe0, 0x90, 0x20, 0x27, 0x1b, 0x72, 0xc7, 0xf2, 0xdb,
            0x71, 0x03, 0x7e, 0x00, 0x35, 0x53, 0x4c, 0xe2, 0x63, 0x55, 0x61, 0x4b, 0x9a, 0x93,
            0x02, 0xab, 0xd9, 0x3c, 0xbd, 0xf9, 0x47, 0x42, 0x09, 0xad, 0x70, 0x1a, 0xc5, 0x2a,
            0xb8, 0x34, 0xd0, 0x81, 0xe9, 0xae, 0x60, 0x10, 0x4f, 0x74, 0xb7, 0x76, 0xe3, 0xfb,
            0xe6, 0xc9, 0x6b, 0xdf, 0x3f, 0x12, 0xa8, 0xec, 0xcf, 0x05, 0x1c, 0xc8, 0x98, 0x51,
            0x21, 0x5d, 0x41, 0x45, 0x94, 0xd1, 0xe1, 0x52, 0x67, 0xea, 0x8b, 0xd5, 0x0d, 0x01,
            0x97, 0x83, 0xbf, 0x17, 0xbc, 0x40, 0xb1, 0x89, 0x79, 0x7a, 0x16, 0xfe, 0xff, 0x54,
            0x80, 0x5b, 0x43, 0x13, 0xf1, 0xfa, 0x5f, 0x57, 0x50, 0xee, 0x44, 0x92, 0xca, 0x15,
            0x9f, 0xf7, 0x56, 0x65, 0x9c, 0xdd, 0x5a, 0xc2, 0x86, 0xd3, 0xf8, 0x06, 0xa0, 0x58,
            0xa1, 0x6a, 0x39, 0x59, 0xd2, 0xf4, 0x0f, 0x6c, 0x6f, 0x1f, 0xd8, 0x68, 0x19, 0xb2,
            0x0a, 0x48, 0x6d, 0xa4, 0x8d, 0xa2, 0x37, 0x66, 0x04, 0x22, 0x0b, 0x9d, 0xb6, 0x78,
            0x36, 0x7d, 0xb3, 0xdc, 0x96, 0x8a, 0xda, 0x7c, 0xba, 0x8c, 0x8f, 0xac, 0x2f, 0x6e,
            0x7f, 0xcc, 0x38, 0x2b, 0x99, 0xaf, 0xc3, 0xd6, 0xc1, 0xd4, 0xc4, 0xaa, 0x7b, 0x88,
            0xed, 0x1e, 0xb5, 0xa9,
        };

        /// <summary>
        /// 当处理CRC校验
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnCRC(int index, byte crcCode)
        {
            if (index < 0 | index > CRCCode.Length)
                goto JUMP;
            if (CRCCode[index] == crcCode)
                return true;
            JUMP: Debug.LogError("CRC校验失败:");
            return false;
        }



        /// <summary>
        /// 当执行Rpc(远程过程调用函数)时, 如果想提供服务器效率, 可以重写此方法, 指定要调用的方法, 可以提高服务器性能 (默认反射调用)
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="model">数据模型</param>
        protected virtual void OnRpcExecute(Player client, RPCModel model)
        {
            OnRPCExecute(client, model);
        }
        
        protected internal void OnRpcExecuteInternal(Player client, RPCModel model)
        {
            if (model.methodMask != 0)
                RpcMaskDic.TryGetValue(model.methodMask, out model.func);
            if (!RpcsDic.TryGetValue(model.func, out List < RPCMethod > rpcs))
            {
                Debug.LogWarning($"没有找到:{model.func}的Rpc方法,请使用server(你的服务器类).AddRpcHandle方法注册!");
                return;
            }
            foreach (RPCMethod rpc in rpcs)
            {
                try
                {
                    if (rpc.cmd == NetCmd.SafeCall)
                    {
                        object[] pars = new object[model.pars.Length + 1];
                        pars[0] = client;
                        Array.Copy(model.pars, 0, pars, 1, model.pars.Length);
                        rpc.Invoke(pars);
                    }
                    else
                    {
                        rpc.Invoke(model.pars);
                    }
                }
                catch (Exception e)
                {
                    string str = "方法:" + rpc.method + " 参数:";
                    if (model.pars == null)
                        str += "null";
                    else
                    {
                        foreach (object p in model.pars)
                        {
                            if (p == null)
                                str += "Null , ";
                            else
                                str += p + " , ";
                        }
                    }
                    Debug.LogError(str + " -> " + e);
                }
            }
        }

        /// <summary>
        /// 心跳检测处理线程
        /// </summary>
        protected void CheckHeartHandle()
        {
            while (IsRunServer)
            {
                Thread.Sleep(HeartInterval);
                try
                {
                    HeartHandle();
                }
                catch (Exception ex)
                {
                    Debug.Log("心跳异常: " + ex);
                }
            }
        }

        /// <summary>
        /// 心跳处理
        /// </summary>
        protected virtual void HeartHandle()
        {
            foreach (var client in AllClients)
            {
                if (client.Value == null)
                    continue;
                client.Value.heart++;
                if (RTOMode == RTOMode.Variable)
                    Ping(client.Value);
                if (!client.Value.Login)
                {
                    if (DateTime.Now > client.Value.LastTime)
                    {
                        Debug.Log($"赖在服务器的客户端:{client.Key}被强制下线!");
                        client.Value.RemotePoint = client.Key;//解决key偶尔不对导致一直移除不了问题
                        goto J;
                    }
                }
                if (client.Value.heart <= HeartLimit)//有5次确认心跳包
                    continue;
                if (client.Value.heart < HeartLimit + 5)
                {
                    Send(client.Value, NetCmd.SendHeartbeat, new byte[0]);
                    continue;
                }
            J: RemoveClient(client.Value);
            }
        }

        /// <summary>
        /// 创建网络场景, 退出当前场景,进入所创建的场景 - 创建场景成功返回场景对象， 创建失败返回null
        /// </summary>
        /// <param name="player">创建网络场景的玩家实体</param>
        /// <param name="sceneID">要创建的场景号或场景名称</param>
        /// <returns></returns>
        public Scene CreateScene(Player player, string sceneID)
        {
            return CreateScene(player, sceneID, new Scene() { Name = sceneID });
        }

        /// <summary>
        /// 创建网络场景, 退出当前场景并加入所创建的场景 - 创建场景成功返回场景对象， 创建失败返回null
        /// </summary>
        /// <param name="player">创建网络场景的玩家实体</param>
        /// <param name="scene">创建场景的实体</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public Scene CreateScene(Player player, Scene scene, Action<Scene> exitCurrentSceneCall = null)
        {
            return CreateScene(player, scene.Name, scene, exitCurrentSceneCall);
        }

        /// <summary>
        /// 创建网络场景, 退出当前场景并加入所创建的场景 - 创建场景成功返回场景对象， 创建失败返回null
        /// </summary>
        /// <param name="player">创建网络场景的玩家实体</param>
        /// <param name="sceneID">要创建的场景号或场景名称</param>
        /// <param name="scene">创建场景的实体</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public Scene CreateScene(Player player, string sceneID, Scene scene, Action<Scene> exitCurrentSceneCall = null)
        {
            if (string.IsNullOrEmpty(sceneID))
                return null;
            if (Scenes.TryAdd(sceneID, scene))
            {
                if (Scenes.TryGetValue(player.sceneID, out Scene exitScene))
                {
                    exitScene.Remove(player);
                    exitCurrentSceneCall?.Invoke(exitScene);
                }
                scene.Name = sceneID;
                scene.AddPlayer(player);
                scene.onSerializeOptHandle = OnSerializeOpt;
                return scene;
            }
            return null;
        }

        /// <summary>
        /// 创建一个场景, 成功则返回场景对象, 创建失败则返回null
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        public Scene CreateScene(string sceneID)
        {
            return CreateScene(sceneID, new Scene());
        }

        /// <summary>
        /// 创建一个场景, 成功则返回场景对象, 创建失败则返回null
        /// </summary>
        /// <param name="sceneID"></param>
        /// <param name="scene"></param>
        /// <returns></returns>
        public Scene CreateScene(string sceneID, Scene scene)
        {
            if (string.IsNullOrEmpty(sceneID))
                return null;
            if (Scenes.TryAdd(sceneID, scene))
            {
                scene.Name = sceneID;
                scene.onSerializeOptHandle = OnSerializeOpt;
                return scene;
            }
            return null;
        }

        /// <summary>
        /// 退出当前场景,加入指定的场景 - 成功进入返回场景对象，进入失败返回null
        /// </summary>
        /// <param name="player">要进入sceneID场景的玩家实体</param>
        /// <param name="sceneID">场景ID，要切换到的场景号或场景名称</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public Scene JoinScene(Player player, string sceneID, Action<Scene> exitCurrentSceneCall = null) => SwitchScene(player, sceneID, exitCurrentSceneCall);

        /// <summary>
        /// 进入场景 - 成功进入返回true，进入失败返回false
        /// </summary>
        /// <param name="player">要进入sceneID场景的玩家实体</param>
        /// <param name="sceneID">场景ID，要切换到的场景号或场景名称</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public Scene EnterScene(Player player, string sceneID, Action<Scene> exitCurrentSceneCall = null) => SwitchScene(player, sceneID, exitCurrentSceneCall);

        /// <summary>
        /// 退出当前场景,切换到指定的场景 - 成功进入返回true，进入失败返回false
        /// </summary>
        /// <param name="player">要进入sceneID场景的玩家实体</param>
        /// <param name="sceneID">场景ID，要切换到的场景号或场景名称</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public Scene SwitchScene(Player player, string sceneID, Action<Scene> exitCurrentSceneCall = null)
        {
            if (string.IsNullOrEmpty(sceneID))
                return null;
            if (Scenes.TryGetValue(sceneID, out Scene scene1))
            {
                if (Scenes.TryGetValue(player.sceneID, out Scene scene2))
                {
                    scene2.Remove(player);
                    exitCurrentSceneCall?.Invoke(scene2);
                }
                scene1.AddPlayer(player);
                return scene1;
            }
            return null;
        }

        /// <summary>
        /// 退出场景 exitCurrentSceneCall回调时已经不包含player对象
        /// </summary>
        /// <param name="player"></param>
        /// <param name="isEntMain">退出当前场景是否进入主场景: 默认进入主场景</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        public void ExitScene(Player player, bool isEntMain = true, Action<Scene> exitCurrentSceneCall = null)
        {
            RemoveScenePlayer(player, isEntMain, exitCurrentSceneCall);
        }

        /// <summary>
        /// 移除服务器场景. 从服务器总场景字典中移除指定的场景: 当你移除指定场景后,如果场景内有其他玩家在内, 则把其他玩家添加到主场景内
        /// </summary>
        /// <param name="sceneID">要移除的场景id</param>
        /// <param name="addToMainScene">允许即将移除的场景内的玩家添加到主场景?</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public bool RemoveScene(string sceneID, bool addToMainScene = true, Action<Scene> exitCurrentSceneCall = null)
        {
            if (Scenes.TryRemove(sceneID, out Scene scene))
            {
                exitCurrentSceneCall?.Invoke(scene);
                if (addToMainScene)
                {
                    Scene mainScene = Scenes[MainSceneName];
                    foreach (Player p in scene.Players)
                    {
                        mainScene.AddPlayer(p);
                    }
                }
                else
                {
                    foreach (Player p in scene.Players)
                    {
                        scene.OnRemove(p);
                        p.OnRemove();
                        p.Scene = null;
                        p.sceneID = "";
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将玩家从当前所在的场景移除掉， 移除之后此客户端将会进入默认主场景 call回调时已经不包含player对象
        /// </summary>
        /// <param name="player">要执行的玩家实体</param>
        /// <param name="isEntMain">退出当前场景是否进入主场景: 默认进入主场景</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public bool RemoveScenePlayer(Player player, bool isEntMain = true, Action<Scene> exitCurrentSceneCall = null)
        {
            if (string.IsNullOrEmpty(player.sceneID))
                return false;
            if (Scenes.TryGetValue(player.sceneID, out Scene scene))
            {
                scene.Remove(player);
                exitCurrentSceneCall?.Invoke(scene);
                if (isEntMain)
                {
                    Scene mainScene = Scenes[MainSceneName];
                    mainScene.AddPlayer(player);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从所有在线玩家字典中删除(移除)玩家实体
        /// </summary>
        /// <param name="player"></param>
        public void DeletePlayer(Player player) => RemoveClient(player);

        /// <summary>
        /// 从所有在线玩家字典中移除玩家实体
        /// </summary>
        /// <param name="player"></param>
        public void RemovePlayer(Player player) => RemoveClient(player);

        /// <summary>
        /// 从客户端字典中移除客户端
        /// </summary>
        /// <param name="client"></param>
        public virtual void RemoveClient(Player client)
        {
            if (client.isDispose)
                return;
            if (client.Login & onlineNumber > 0) Interlocked.Decrement(ref onlineNumber);
            else if (!client.Login & ignoranceNumber > 0) Interlocked.Decrement(ref ignoranceNumber);
            Players.TryRemove(client.playerID, out _);
            UIDClients.TryRemove(client.UserID, out _);
            AllClients.TryRemove(client.RemotePoint, out _);
            OnRemoveClientHandle(client);
            client.OnRemoveClient();
            ExitScene(client, false);
            client.Dispose();
            //ObjectPool<Player>.Push(client);
        }

        /// <summary>
        /// 场景是否存在?
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        public bool IsHasScene(string sceneID)
        {
            return Scenes.ContainsKey(sceneID);
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public virtual void Close()
        {
            IsRunServer = false;
            foreach (Player client in AllClients.Values)
                client.Dispose();
            AllClients.Clear();
            Players.Clear();
            UIDClients.Clear();
            Thread.Sleep(50);//等待线程退出后再关闭套接字, 解决在revd方法出错
            if (Server != null)
            {
                Server.Dispose();
                Server.Close();
                Server = null;
            }
            if (SocketAsync != null)
            {
                SocketAsync.Completed -= OnIOCompleted;
                SocketAsync.Dispose();
                SocketAsync = null;
            }
            if (this == Instance)//有多个服务器实例, 需要
                Instance = null;
            threads.Clear();
            SendDataBeProcesseds.Clear();
            RevdDataBeProcesseds.Clear();
            OnStartingHandle -= OnStarting;
            OnStartupCompletedHandle -= OnStartupCompleted;
            OnHasConnectHandle -= OnHasConnect;
            OnRemoveClientHandle -= OnRemoveClient;
            OnOperationSyncHandle -= OnOperationSync;
            OnRevdBufferHandle -= OnReceiveBuffer;
            OnReceiveFileHandle -= OnReceiveFile;
            OnRevdRTProgressHandle -= OnRevdRTProgress;
            OnSendRTProgressHandle -= OnSendRTProgress;
            Debug.Log("服务器已关闭！");//先打印在移除事件
            Thread.Sleep(100);
            Debug.LogHandle -= Log;
            Debug.LogWarningHandle -= Log;
            Debug.LogErrorHandle -= Log;
        }

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送数据到的客户端</param>
        /// <param name="buffer">数据缓冲区</param>
        public virtual void Send(Player client, byte[] buffer)
        {
            Send(client, NetCmd.OtherCmd, buffer);
        }

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">数据缓冲区</param>
        public virtual void Send(Player client, byte cmd, byte[] buffer)
        {
            if (client.CloseSend)
                return;
            if (client.udpRPCModels.Count >= ushort.MaxValue)
            {
                Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                return;
            }
            if (buffer.Length > 65507)
            {
                Debug.LogError("数据太大，请分块发送!");
                return;
            }
            client.udpRPCModels.Enqueue(new RPCModel(cmd, buffer) { bigData = buffer.Length > short.MaxValue });
        }

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="buffer">数据缓冲区</param>
        public virtual void Send(Player client, bool reliable, byte[] buffer)
        {
            if (reliable)
                SendRT(client, buffer);
            else
                Send(client, buffer);
        }

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">数据缓冲区</param>
        public virtual void Send(Player client, byte cmd, bool reliable, byte[] buffer)
        {
            if (reliable)
                SendRT(client, cmd, buffer);
            else
                Send(client, cmd, buffer);
        }

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送数据到的客户端</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public virtual void Send(Player client, string func, params object[] pars)
        {
            Send(client, NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送到的客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public virtual void Send(Player client, byte cmd, string func, params object[] pars)
        {
            if (client.CloseSend)
                return;
            if (client.udpRPCModels.Count >= ushort.MaxValue)
            {
                Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                return;
            }
            client.udpRPCModels.Enqueue(new RPCModel(cmd, func, pars));
        }

        public virtual void Send(Player client, ushort methodMask, params object[] pars)
        {
            Send(client, NetCmd.CallRpc, methodMask, pars);
        }

        public virtual void Send(Player client, byte cmd, ushort methodMask, params object[] pars)
        {
            if (client.CloseSend)
                return;
            if (client.udpRPCModels.Count >= ushort.MaxValue)
            {
                Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                return;
            }
            client.udpRPCModels.Enqueue(new RPCModel(cmd, methodMask, pars));
        }

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送到的客户端</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public virtual void Send(Player client, bool reliable, string func, params object[] pars)
        {
            if (reliable)
                SendRT(client, func, pars);
            else
                Send(client, func, pars);
        }

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送到的客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public virtual void Send(Player client, byte cmd, bool reliable, string func, params object[] pars)
        {
            if (reliable)
                SendRT(client, cmd, func, pars);
            else
                Send(client, cmd, func, pars);
        }

        public virtual void Send(Player client, byte cmd, object obj)
        {
            var buffer = BufferPool.Take();
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                stream.SetLength(0);
                ProtoBuf.Serializer.Serialize(stream, obj);
                Send(client, cmd, stream.ToArray());
            }
            BufferPool.Push(buffer);
        }

        /// <summary>
        /// 发送灵活数据包
        /// </summary>
        /// <param name="client">客户端集合</param>
        /// <param name="cmd"></param>
        /// <param name="buffer">要包装的数据,你自己来定</param>
        /// <param name="kernel">内核? 你包装的数据在客户端是否被内核NetConvert序列化?</param>
        /// <param name="serialize">序列化? 你包装的数据是否在服务器即将发送时NetConvert序列化?</param>
        public void Send(Player client, byte cmd, byte[] buffer, bool kernel, bool serialize)
        {
            if (client.CloseSend)
                return;
            if (client.udpRPCModels.Count >= ushort.MaxValue)
            {
                Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                return;
            }
            if (buffer.Length > 65507)
            {
                Debug.LogError("数据太大，请分块发送!");
                return;
            }
            client.udpRPCModels.Enqueue(new RPCModel(cmd, buffer, kernel, serialize) { bigData = buffer.Length > short.MaxValue });
        }

        public void Send(Player client, RPCModel model)
        {
            if (client.CloseSend)
                return;
            if (client.udpRPCModels.Count >= ushort.MaxValue)
            {
                Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                return;
            }
            client.udpRPCModels.Enqueue(model);
        }

        /// <summary>
        /// 发送网络可靠传输数据, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        public virtual void SendRT(Player client, string func, params object[] pars)
        {
            SendRT(client, NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        public virtual void SendRT(Player client, byte cmd, string func, params object[] pars)
        {
            if (client.CloseSend)
                return;
            if (client.tcpRPCModels.Count >= ushort.MaxValue)
            {
                Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                return;
            }
            client.tcpRPCModels.Enqueue(new RPCModel(cmd, func, pars, true, true));
        }

        public virtual void SendRT(Player client, ushort methodMask, params object[] pars)
        {
            SendRT(client, NetCmd.CallRpc, methodMask, pars);
        }

        public virtual void SendRT(Player client, byte cmd, ushort methodMask, params object[] pars)
        {
            if (client.CloseSend)
                return;
            if (client.tcpRPCModels.Count >= ushort.MaxValue)
            {
                Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                return;
            }
            client.tcpRPCModels.Enqueue(new RPCModel(cmd, string.Empty, pars, true, true, methodMask));
        }

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        public virtual void SendRT(Player client, byte[] buffer)
        {
            SendRT(client, NetCmd.OtherCmd, buffer);
        }

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer"></param>
        public virtual void SendRT(Player client, byte cmd, byte[] buffer)
        {
            if (client.CloseSend)
                return;
            if (client.tcpRPCModels.Count >= ushort.MaxValue)
            {
                Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                return;
            }
            if (buffer.Length / MTU > ushort.MaxValue)
            {
                Debug.LogError("数据太大，请分块发送!");
                return;
            }
            client.tcpRPCModels.Enqueue(new RPCModel(cmd, buffer, false, false) { bigData = buffer.Length > short.MaxValue });
        }

        public virtual void SendRT(Player client, byte cmd, object obj)
        {
            if (cmd < 30)
                throw new Exception("自定义协议(命令)不能使用内核协议(命令)进行发送!");
            using (MemoryStream stream = new MemoryStream(1024))
            {
                ProtoBuf.Serializer.Serialize(stream, obj);
                SendRT(client, cmd, stream.ToArray());
            }
        }

        /// <summary>
        /// 发送灵活数据包
        /// </summary>
        /// <param name="client">客户端集合</param>
        /// <param name="cmd"></param>
        /// <param name="buffer">要包装的数据,你自己来定</param>
        /// <param name="kernel">内核? 你包装的数据在客户端是否被内核NetConvert序列化?</param>
        /// <param name="serialize">序列化? 你包装的数据是否在服务器即将发送时NetConvert序列化?</param>
        public void SendRT(Player client, byte cmd, byte[] buffer, bool kernel, bool serialize)
        {
            if (client.CloseSend)
                return;
            if (client.tcpRPCModels.Count >= ushort.MaxValue)
            {
                Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                return;
            }
            if (buffer.Length / MTU > ushort.MaxValue)
            {
                Debug.LogError("数据太大，请分块发送!");
                return;
            }
            client.tcpRPCModels.Enqueue(new RPCModel(cmd, buffer, kernel, serialize) { bigData = buffer.Length > short.MaxValue });
        }

        public void SendRT(Player client, RPCModel model)
        {
            if (client.CloseSend)
                return;
            if (client.tcpRPCModels.Count >= ushort.MaxValue)
            {
                Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                return;
            }
            client.tcpRPCModels.Enqueue(model);
        }

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="buffer">自定义字节数组</param>
        public virtual void Multicast(IList<Player> clients, byte[] buffer)
        {
            Multicast(clients, false, NetCmd.CallRpc, buffer);
        }

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="cmd"></param>
        /// <param name="buffer">自定义字节数组</param>
        public virtual void Multicast(IList<Player> clients, byte cmd, byte[] buffer)
        {
            Multicast(clients, false, cmd, buffer);
        }

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="reliable"></param>
        /// <param name="buffer">自定义字节数组</param>
        public virtual void Multicast(IList<Player> clients, bool reliable, byte[] buffer)
        {
            Multicast(clients, reliable, NetCmd.OtherCmd, buffer);
        }

        public virtual void Multicast(IList<Player> clients, bool reliable, byte cmd, object obj)
        {
            if (cmd < 30)
                throw new Exception("自定义协议(命令)不能使用内核协议(命令)进行发送!");
            using (MemoryStream stream = new MemoryStream(512))
            {
                ProtoBuf.Serializer.Serialize(stream, obj);
                Multicast(clients, reliable, cmd, stream.ToArray());
            }
        }

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="reliable">使用可靠传输?</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">自定义字节数组</param>
        public virtual void Multicast(IList<Player> clients, bool reliable, byte cmd, byte[] buffer)
        {
            if (buffer.Length / MTU > ushort.MaxValue)
            {
                Debug.LogError("数据太大，请分块发送!");
                return;
            }
            for (int i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                if (client == null)
                    continue;
                if (client.CloseSend)
                    continue;
                if (!reliable)
                {
                    if (client.udpRPCModels.Count >= ushort.MaxValue)
                    {
                        Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                        return;
                    }
                    client.udpRPCModels.Enqueue(new RPCModel(cmd, buffer, false, false) { bigData = buffer.Length > short.MaxValue });
                }
                else
                {
                    if (client.tcpRPCModels.Count >= ushort.MaxValue)
                    {
                        Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                        return;
                    }
                    client.tcpRPCModels.Enqueue(new RPCModel(cmd, buffer, false, false) { bigData = buffer.Length > short.MaxValue });
                }
            }
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端 (灵活数据包)
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="reliable"></param>
        /// <param name="cmd"></param>
        /// <param name="buffer">要包装的数据,你自己来定</param>
        /// <param name="kernel">内核? 你包装的数据在客户端是否被内核NetConvert序列化?</param>
        /// <param name="serialize">序列化? 你包装的数据是否在服务器即将发送时NetConvert序列化?</param>
        public virtual void Multicast(IList<Player> clients, bool reliable, byte cmd, byte[] buffer, bool kernel, bool serialize)
        {
            if (buffer.Length / MTU > ushort.MaxValue)
            {
                Debug.LogError("数据太大，请分块发送!");
                return;
            }
            for (int i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                if (client == null)
                    continue;
                if (client.CloseSend)
                    continue;
                if (!reliable)
                {
                    if (client.udpRPCModels.Count >= ushort.MaxValue)
                    {
                        Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                        return;
                    }
                    client.udpRPCModels.Enqueue(new RPCModel(cmd, buffer, kernel, serialize) { bigData = buffer.Length > short.MaxValue });
                }
                else
                {
                    if (client.tcpRPCModels.Count >= ushort.MaxValue)
                    {
                        Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                        return;
                    }
                    client.tcpRPCModels.Enqueue(new RPCModel(cmd, buffer, kernel, serialize) { bigData = buffer.Length > short.MaxValue });
                }
            }
        }

        public virtual void Multicast(IList<Player> clients, bool reliable, RPCModel model)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                if (client == null)
                    continue;
                if (client.CloseSend)
                    continue;
                if (!reliable)
                {
                    if (client.udpRPCModels.Count >= ushort.MaxValue)
                    {
                        Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                        return;
                    }
                    client.udpRPCModels.Enqueue(model);
                }
                else
                {
                    if (client.tcpRPCModels.Count >= ushort.MaxValue)
                    {
                        Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                        return;
                    }
                    client.tcpRPCModels.Enqueue(model);
                }
            }
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        public virtual void Multicast(IList<Player> clients, string func, params object[] pars)
        {
            Multicast(clients, false, NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="reliable">使用可靠传输?</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        public virtual void Multicast(IList<Player> clients, bool reliable, string func, params object[] pars)
        {
            Multicast(clients, reliable, NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        public virtual void Multicast(IList<Player> clients, byte cmd, string func, params object[] pars)
        {
            Multicast(clients, false, cmd, func, pars);
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="reliable">使用可靠传输?</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        public virtual void Multicast(IList<Player> clients, bool reliable, byte cmd, string func, params object[] pars)
        {
            byte[] buffer = OnSerializeRpc(new RPCModel(1, func, pars));
            if (buffer.Length / MTU > ushort.MaxValue)
            {
                Debug.LogError("数据太大，请分块发送!");
                return;
            }
            for (int i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                if (client == null)
                    continue;
                if (client.CloseSend)
                    continue;
                if (!reliable)
                {
                    if (client.udpRPCModels.Count >= ushort.MaxValue)
                    {
                        Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                        return;
                    }
                    client.udpRPCModels.Enqueue(new RPCModel(cmd, buffer, true, false));
                }
                else
                {
                    if (client.tcpRPCModels.Count >= ushort.MaxValue)
                    {
                        Debug.LogError($"[{client.UserID}]数据缓存列表超出限制!");
                        return;
                    }
                    client.tcpRPCModels.Enqueue(new RPCModel(cmd, buffer, true, false));
                }
            }
        }

        public virtual void Multicast(IList<Player> clients, bool reliable, ushort methodMask, params object[] pars)
        {
            Multicast(clients, reliable, new RPCModel(NetCmd.CallRpc, methodMask, pars));
        }

        public virtual void Multicast(IList<Player> clients, bool reliable, byte cmd, ushort methodMask, params object[] pars)
        {
            Multicast(clients, reliable, new RPCModel(cmd, methodMask, pars));
        }

        /// <summary>
        /// 添加网络Rpc(注册远程方法)
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        public void AddRpcHandle(object target)
        {
            AddRpcHandle(target, false);
        }

        /// <summary>
        /// 添加网络Rpc(注册远程方法)
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        /// <param name="append">一个Rpc方法是否可以多次添加到Rpcs里面？</param>
        public void AddRpcHandle(object target, bool append)
        {
            if (OnAddRpcHandle == null)
                OnAddRpcHandle = AddRpcInternal;
            OnAddRpcHandle(target, append);
        }
        
        protected void AddRpcInternal(object target, bool append)
        {
            if (!append)
            {
                foreach (List<RPCMethod> rpcs in RpcsDic.Values)
                {
                    foreach (RPCMethod o in rpcs)
                        if (o.target == target)
                            return;
                }
            }
            foreach (MethodInfo info in target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                RPCFun rpc = info.GetCustomAttribute<RPCFun>();
                if (rpc != null)
                {
                    RPCMethod item = new RPCMethod(target, info, rpc.cmd);
                    if (rpc.mask != 0)
                    {
                        if (!RpcMaskDic.TryGetValue(rpc.mask, out string func))
                            RpcMaskDic.Add(rpc.mask, info.Name);
                        else if (func != info.Name)
                            Debug.LogError($"错误! 请修改Rpc方法{info.Name}或{func}的mask值, mask值必须是唯一的!");
                    }
                    if (!RpcsDic.ContainsKey(item.method.Name))
                        RpcsDic.Add(item.method.Name, new List<RPCMethod>());
                    RpcsDic[item.method.Name].Add(item);
                    Rpcs.Add(item);
                }
            }
        }

        /// <summary>
        /// 移除对象的Rpc注册
        /// </summary>
        /// <param name="target">将此对象的所有带有RPCFun特性的函数移除</param>
        public void RemoveRpc(object target)
        {
            if (OnRemoveRpc == null)
                OnRemoveRpc = RemoveRpcInternal;
            OnRemoveRpc(target);
        }
        
        protected void RemoveRpcInternal(object target)
        {
            List<RPCMethod> rpcsList = new List<RPCMethod>();
            Dictionary<string, List<RPCMethod>> dic = new Dictionary<string, List<RPCMethod>>(RpcsDic);
            foreach (KeyValuePair<string, List<RPCMethod>> rpcs in dic)
            {
                for (int i = 0; i < rpcs.Value.Count; i++)
                {
                    if (rpcs.Value[i].target.Equals(target) | rpcs.Value[i].method.Equals(null))
                    {
                        rpcs.Value.RemoveAt(i);
                        i--;
                    }
                }
                if (rpcs.Value.Count <= 0)
                    RpcsDic.Remove(rpcs.Key);
                else
                    rpcsList.AddRange(rpcs.Value);
            }
            Rpcs = rpcsList;
        }

        /// <summary>
        /// playerID玩家是否在线?
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public virtual bool IsOnline(string playerID)
        {
            return Players.ContainsKey(playerID);
        }

        /// <summary>
        /// playerID玩家是否在线? 并且如果在线则out 在线玩家的对象
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public virtual bool IsOnline(string playerID, out Player client)
        {
            return Players.TryGetValue(playerID, out client);
        }

        /// <summary>
		/// 强制下线处理, 将client客户端从在线字段<see cref="Players"/>和<see cref="UIDClients"/>和<see cref="AllClients"/>字段中移除
		/// </summary>
		/// <param name="client"></param>
		public virtual void OfflineHandle(Player client)
        {
            SendDirect(client);
            RemoveClient(client);
            Debug.Log("[" + client.playerID + "]被强制下线...!");
        }

        /// <summary>
        /// 退出登录, 将client客户端从在线字段<see cref="Players"/>和<see cref="UIDClients"/>字段中移除
        /// </summary>
        /// <param name="client"></param>
        public virtual void SignOut(Player client)
        {
            if (!client.Login)
                return;
            SendDirect(client);
            if (onlineNumber > 0) Interlocked.Decrement(ref onlineNumber);
            Players.TryRemove(client.playerID, out _);
            UIDClients.TryRemove(client.UserID, out _);
            ExitScene(client, false);
            client.playerID = client.UserID.ToString();
            client.Login = false;
            Debug.Log("[" + client.playerID + "]退出登录...!");
        }

        /// <summary>
        /// 设置心跳时间
        /// </summary>
        /// <param name="timeoutLimit">心跳检测次数, 默认检测5次</param>
        /// <param name="interval">心跳时间间隔, 每interval毫秒会检测一次</param>
        public void SetHeartTime(byte timeoutLimit, int interval)
        {
            HeartLimit = timeoutLimit;
            HeartInterval = interval;
        }

        /// <summary>
        /// ping测试网络延迟, 通过<see cref="OnPingCallback"/>事件回调
        /// </summary>
        /// <param name="client"></param>
        public void Ping(Player client)
        {
            long timelong = DateTime.Now.Ticks;
            Send(client, NetCmd.Ping, BitConverter.GetBytes(timelong));
        }

        /// <summary>
        /// ping测试网络延迟, 此方法帮你监听<see cref="OnPingCallback"/>事件, 如果不使用的时候必须保证能移除委托, 建议不要用框名函数, 那样会无法移除委托
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        public void Ping(Player client, Action<Player, double> callback)
        {
            long timelong = DateTime.Now.Ticks;
            Send(client, NetCmd.Ping, BitConverter.GetBytes(timelong));
            OnPingCallback += callback;
        }

        internal void SetRAC(int length)
        {
            receiveCount += length;
            receiveAmount++;
        }

        /// <summary>
        /// 添加适配器
        /// </summary>
        /// <param name="adapter"></param>
        public void AddAdapter(IAdapter adapter)
        {
            if (adapter is ISerializeAdapter ser)
                AddAdapter(AdapterType.Serialize, ser);
            else if (adapter is IRPCAdapter<Player> rpc)
                AddAdapter(AdapterType.RPC, rpc);
            else throw new Exception("无法识别的适配器!， 注意: IRPCAdapter<Player>是服务器的RPC适配器，IRPCAdapter是客户端适配器！");
        }
        
        /// <summary>
        /// 添加适配器
        /// </summary>
        /// <param name="type"></param>
        /// <param name="adapter"></param>
        public void AddAdapter(AdapterType type, IAdapter adapter)
        {
            switch (type) 
            {
                case AdapterType.Serialize:
                    var ser = (ISerializeAdapter)adapter;
                    OnSerializeRPC = ser.OnSerializeRpc;
                    OnDeserializeRPC = ser.OnDeserializeRpc;
                    OnSerializeOPT = ser.OnSerializeOpt;
                    OnDeserializeOPT = ser.OnDeserializeOpt;
                    break;
                case AdapterType.RPC:
                    var rpc = (IRPCAdapter<Player>)adapter;
                    OnAddRpcHandle = rpc.AddRpcHandle;
                    OnRPCExecute = rpc.OnRpcExecute;
                    OnRemoveRpc = rpc.RemoveRpc;
                    break;
            }
        }

        /// <summary>
        /// 字段,属性同步线程
        /// </summary>
        protected virtual void VarSyncHandler()
        {
            while (IsRunServer)
            {
                try
                {
                    Thread.Sleep(1);
                    foreach (var client in AllClients)
                    {
                        if (client.Value == null)
                            continue;
                        Segment segment = null;
                        var entries = client.Value.varSyncInfos.entries;
                        for (int i = 0; i < client.Value.varSyncInfos.count; i++)
                        {
                            if (entries[i].hashCode >= 0)
                            {
                                var varSync = entries[i].value;
                                if (varSync == null)
                                    continue;
                                var value = varSync.GetValue();
                                if (value == null)
                                    continue;
                                if (varSync.passive)
                                    continue;
                                if (!value.Equals(varSync.value))
                                {
                                    if(segment == null)
                                        segment = BufferPool.Take();
                                    segment.WriteValue(varSync.id);
                                    segment.WriteValue(value);
                                    varSync.value = value;
                                }
                            }
                        }
                        if (segment != null)
                        {
                            var buffer = segment.ToArray(true);
                            SendRT(client.Value, NetCmd.VarSync, buffer);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        /// <summary>
        /// 发送文件, 客户端可以使用事件<see cref="Client.ClientBase.OnReceiveFileHandle"/>来监听并处理
        /// </summary>
        /// <param name="client"></param>
        /// <param name="filePath"></param>
        /// <param name="bufferSize">每次发送数据大小</param>
        /// <returns></returns>
        public bool SendFile(Player client, string filePath, int bufferSize = 50000)
        {
            var path1 = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(path1))
            {
                Debug.LogError("文件不存在! 或者文件路径字符串编码错误! 提示:可以使用Notepad++查看, 编码是ANSI,不是UTF8");
                return false;
            }
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize);
            var fileData = new FileData
            {
                ID = fileStream.GetHashCode(),
                fileStream = fileStream,
                fileName = Path.GetFileName(filePath),
                bufferSize = bufferSize
            };
            client.ftpDic.Add(fileData.ID, fileData);
            SendFile(client, fileData.ID, fileData);
            return true;
        }

        private void SendFile(Player client, int key, FileData fileData)
        {
            var fileStream = fileData.fileStream;
            bool complete = false;
            long bufferSize = fileData.bufferSize;
            if (fileStream.Position + fileData.bufferSize > fileStream.Length)
            {
                bufferSize = fileStream.Length - fileStream.Position;
                complete = true;
            }
            byte[] buffer = new byte[bufferSize];
            fileStream.Read(buffer, 0, buffer.Length);
            Segment segment1 = BufferPool.Take((int)bufferSize + 50);
            segment1.WriteValue(fileData.ID);
            segment1.WriteValue(fileData.fileStream.Length);
            segment1.WriteValue(fileData.fileName);
            segment1.WriteArray(buffer);
            SendRT(client, NetCmd.SendFile, segment1.ToArray(true));
            if (complete)
            {
                OnSendFileProgress?.Invoke(client, new RTProgress(fileData.fileName, fileStream.Position / (float)fileStream.Length * 100f, RTState.Complete));
                client.ftpDic.Remove(key);
                fileData.fileStream.Close();
            }
            else 
            {
                OnSendFileProgress?.Invoke(client, new RTProgress(fileData.fileName, fileStream.Position / (float)fileStream.Length * 100f, RTState.Sending));
            }
        }
    }
}