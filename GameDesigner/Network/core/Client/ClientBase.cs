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
namespace Net.Client
{
    using Net.Event;
    using Net.Share;
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net;
    using global::System.Net.NetworkInformation;
    using global::System.Net.Sockets;
    using global::System.Reflection;
    using global::System.Text;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Net.System;
    using Net.Serialize;

    /// <summary>
    /// 网络客户端核心基类 2019.3.3
    /// </summary>
    public abstract class ClientBase : INetClient, ISendHandle
    {
        /// <summary>
        /// UDP客户端套接字
        /// </summary>
        public Socket Client { get; protected set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string host = "127.0.0.1";
        /// <summary>
        /// 端口号
        /// </summary>
        public int port = 6666;
        /// <summary>
        /// 发送缓存器
        /// </summary>
        protected QueueSafe<RPCModel> rPCModels = new QueueSafe<RPCModel>();
        /// <summary>
        /// 可靠传输缓冲器
        /// </summary>
        protected QueueSafe<RPCModel> rtRPCModels = new QueueSafe<RPCModel>();
        /// <summary>
        /// 接收缓存器
        /// </summary>
        protected List<RevdBuffer> revdBuffers = new List<RevdBuffer>();

        /// <summary>
        /// 网络委托函数 主要用于 unity 面板查看
        /// </summary>
        /// <returns></returns>
        private List<RPCMethod> Rpcs = new List<RPCMethod>();
        /// <summary>
        /// 远程方法优化字典
        /// </summary>
        protected MyDictionary<string, List<RPCMethod>> RpcsDic = new MyDictionary<string, List<RPCMethod>>();
        /// <summary>
        /// 远程方法遮罩
        /// </summary>
        private readonly MyDictionary<ushort, string> RpcMaskDic = new MyDictionary<ushort, string>();
        /// <summary>
        /// 线程字典
        /// </summary>
        protected ConcurrentDictionary<string, Thread> threadDic = new ConcurrentDictionary<string, Thread>();
        /// <summary>
        /// 网络连接状态
        /// </summary>
        public NetworkState NetworkState { get; protected set; } = NetworkState.None;
        /// <summary>
        /// 服务器与客户端是否是连接状态
        /// </summary>
        public bool Connected { get; protected set; }
        /// <summary>
        /// 网络客户端实例
        /// </summary>
        public static ClientBase Instance { get; set; }
        /// <summary>
        /// 输出调用RPC错误级别,红色警告 (仅限编辑器调式使用!)
        /// </summary>
        public bool ThrowException;
        /// <summary>
        /// 是否使用unity主线程进行每一帧更新？  
        /// True：使用unity的Update等方法进行更新，unity的组建可以在Rpc函数内进行调用。
        /// False：使用多线程进行网络更新，使用多线程更新后unity的组件将不能在rpc函数内进行赋值设置等操作，否则会出现错误问题!
        /// </summary>
        public bool UseUnityThread { get; set; }
        /// <summary>
        /// 当前客户端网络状态
        /// </summary>
        protected NetworkState networkState = NetworkState.None;
        /// <summary>
        /// 当前尝试重连次数
        /// </summary>
        protected int currFrequency;
        /// <summary>
        /// 每秒发送数据长度
        /// </summary>
        protected int sendCount;
        /// <summary>
        /// 每秒发送数据次数
        /// </summary>
        protected int sendAmount;
        /// <summary>
        /// 每秒解析rpc函数次数
        /// </summary>
        protected int resolveAmount;
        /// <summary>
        /// 每秒接收网络数据次数
        /// </summary>
        protected int receiveAmount;
        /// <summary>
        /// 每秒接收网络数据大小
        /// </summary>
        protected int receiveCount;
        /// <summary>
        /// 发送线程循环次数
        /// </summary>
        protected int sendLoopNum;
        /// <summary>
        /// 接收线程循环次数 只有ENetServer
        /// </summary>
        protected int revdLoopNum;
        /// <summary>
        /// 心跳次数
        /// </summary>
        protected int heart = 0;
        /// <summary>
        /// 当前客户端是否打开(运行)
        /// </summary>
        protected bool openClient;
        /// <summary>
        /// 客户端是否处于打开状态
        /// </summary>
        public bool IsOpenClient => openClient;

        /// <summary>
        /// 输出调用网络函数
        /// </summary>
        public bool LogRpc { get; set; }
        /// <summary>
        /// 输出日志, 这里是输出全部日志(提示,警告,错误等信息). 如果想只输出指定的日志, 请使用NDebug类进行输出
        /// </summary>
        public event Action<string> Log { add { NDebug.BindLogAll(value); } remove { NDebug.RemoveLogAll(value); } }
        /// <summary>
        /// 当连接服务器成功事件
        /// </summary>
        public event Action OnConnectedHandle;
        /// <summary>
        /// 当连接失败事件
        /// </summary>
        public event Action OnConnectFailedHandle;
        /// <summary>
        /// 当尝试连接服务器事件
        /// </summary>
        public event Action OnTryToConnectHandle;
        /// <summary>
        /// 当连接中断 (异常) 事件
        /// </summary>
        public event Action OnConnectLostHandle;
        /// <summary>
        /// 当断开连接事件
        /// </summary>
        public event Action OnDisconnectHandle;
        /// <summary>
        /// 当接收到自定义的cmd指令时调用事件
        /// </summary>
        public event Action<RPCModel> OnRevdBufferHandle;
        /// <summary>
        /// 当断线重连成功触发事件
        /// </summary>
        public event Action OnReconnectHandle;
        /// <summary>
        /// 当关闭连接事件
        /// </summary>
        public event Action OnCloseConnectHandle;
        /// <summary>
        /// 当统计网络流量时触发
        /// </summary>
        public event NetworkDataTraffic OnNetworkDataTraffic;
        /// <summary>
        /// 当服务器连接人数溢出(未知客户端连接总数), 服务器忽略当前客户端的所有Rpc请求时调用
        /// </summary>
        public event Action OnExceededNumberHandle;
        /// <summary>
        /// 当服务器在线人数爆满, 服务器忽略当前客户端的所有Rpc请求时调用
        /// </summary>
        public event Action OnBlockConnectionHandle;
        /// <summary>
        /// 当使用服务器的NetScene.AddOperation方法时调用， 场景内的所有演员行为同步
        /// </summary>
        public event Action<OperationList> OnOperationSync;
        /// <summary>
        /// 当服务器发送的大数据时, 可监听此事件显示进度值
        /// </summary>
        public event Action<RTProgress> OnRevdRTProgress;
        /// <summary>
        /// 当客户端发送可靠数据时, 可监听此事件显示进度值 (NetworkClient,TcpClient类无效)
        /// </summary>
        public event Action<RTProgress> OnSendRTProgress;
        /// <summary>
        /// 当添加远程过程调用方法时调用， 参数1：要收集rpc特性的对象， 参数2：如果客户端的rpc列表中已经有了这个对象，还可以添加进去？
        /// </summary>
        public Action<object, bool> OnAddRpcHandle { get; set; }
        /// <summary>
        /// 当移除远程过程调用对象， 参数1：移除此对象的所有rpc方法
        /// </summary>
        public Action<object> OnRemoveRpc { get; set; }
        /// <summary>
        /// 当执行调用远程过程方法时触发
        /// </summary>
        public Action<RPCModel> OnRPCExecute { get; set; }
        /// <summary>
        /// 检查rpc对象，如果对象被释放则自动移除
        /// </summary>
        private Action OnCheckRpcUpdate;
        /// <summary>
        /// 当内核序列化远程函数时调用, 如果想改变内核rpc的序列化方式, 可重写定义序列化协议 (只允许一个委托, 例子:OnSerializeRpcHandle = (model)=>{return new byte[0];};)
        /// </summary>
        public Func<RPCModel, byte[]> OnSerializeRPC;
        /// <summary>
        /// 当内核解析远程过程函数时调用, 如果想改变内核rpc的序列化方式, 可重写定义解析协议 (只允许一个委托, 例子:OnDeserializeRpcHandle = (buffer)=>{return new FuncData();};)
        /// </summary>
        public Func<byte[], int, int, FuncData> OnDeserializeRPC;
        /// <summary>
        /// 当内部序列化帧操作列表时调用, 即将发送数据  !!!!!!!只允许一个委托
        /// </summary>
        public Func<OperationList, byte[]> OnSerializeOPT;
        /// <summary>
        /// 当内部解析帧操作列表时调用  !!!!!只允许一个委托
        /// </summary>
        public Func<byte[], int, int, OperationList> OnDeserializeOPT;
        /// <summary>
        /// ping服务器回调 参数double为延迟毫秒单位 当RTOMode属性为可变重传时, 内核将会每秒自动ping一次
        /// </summary>
        public event Action<double> OnPingCallback;
        /// <summary>
        /// 当socket发送失败调用. 参数1:发送的字节数组, 参数2:发送标志(可靠和不可靠)  ->可通过SendByteData方法重新发送
        /// </summary>
        public Action<byte[], bool> OnSendErrorHandle;
        /// <summary>
        /// 当从服务器获取的客户端地址点对点
        /// </summary>
        public Action<IPEndPoint> OnP2PCallback;
        /// <summary>
        /// 当网关服务器指定这个客户端连接到一个游戏服务器时调用,回调有游戏服务器的ip和端口
        /// </summary>
        public Action<string, ushort> OnSwitchPortHandle;
        /// <summary>
        /// 当开始下载文件时调用, 参数1(string):服务器发送的文件名 返回值(string):开发者指定保存的文件路径(全路径名称)
        /// </summary>
        public Func<string, string> OnDownloadFileHandle;
        /// <summary>
        /// 当服务器发送的文件完成, 接收到文件后调用, 返回true:框架内部释放文件流和删除临时文件(默认) false:使用者处理
        /// </summary>
        public Func<FileData, bool> OnReceiveFileHandle;
        /// <summary>
        /// 当接收到发送的文件进度
        /// </summary>
        public Action<RTProgress> OnRevdFileProgress;
        /// <summary>
        /// 当发送的文件进度
        /// </summary>
        public Action<RTProgress> OnSendFileProgress;
        /// <summary>
        /// 发送可靠传输缓冲
        /// </summary>
        protected ConcurrentDictionary<uint, MyDictionary<ushort, RTBuffer>> sendRTList = new ConcurrentDictionary<uint, MyDictionary<ushort, RTBuffer>>();
        /// <summary>
        /// 接收可靠传输缓冲
        /// </summary>
        protected HashSet<int> revdRTList = new HashSet<int>();
        protected internal QueueSafe<AckQueue> ackQueue = new QueueSafe<AckQueue>();
        protected internal MyDictionary<int, RevdAck> revdAck = new MyDictionary<int, RevdAck>();
        protected internal int revdRTStreamLen;
        /// <summary>
        /// 可靠传输最大值
        /// </summary>
        protected int sendRTListCount;
        /// <summary>
        /// 可靠传输流文件名称
        /// </summary>
        protected string fileStreamName;
        /// <summary>
        /// 可靠传输文件流对象
        /// </summary>
        protected FileStream revdRTStream;
        /// <summary>
        /// 帧尾或叫数据长度(4) + 2CRC协议 = 6
        /// </summary>
        protected readonly int frame = 6;
        /// <summary>
        /// 心跳时间间隔, 默认每1秒检查一次玩家是否离线, 玩家心跳确认为5次, 如果超出5次 则移除玩家客户端. 确认玩家离线总用时5秒, 
        /// 如果设置的值越小, 确认的速度也会越快. 值太小有可能出现直接中断问题, 设置的最小值在100以上
        /// </summary>
        public int HeartInterval { get; set; } = 1000;
        /// <summary>
        /// <para>心跳检测次数, 默认为5次检测, 如果5次发送心跳给客户端或服务器, 没有收到回应的心跳包, 则进入断开连接处理</para>
        /// <para>当一直有数据往来时是不会发送心跳数据的, 只有当没有数据往来了, 才会进入发送心跳数据</para>
        /// </summary>
        public byte HeartLimit { get; set; } = 5;
        /// <summary>
        /// 客户端唯一标识, 当登录游戏后, 服务器下发下来的唯一标识, 这个标识就是你的玩家名称, 是NetPlayer.playerID值
        /// </summary>
        public string Identify { get; protected set; }
        /// <summary>
        /// 用户唯一标识, 对应服务器的NetPlayer.UID
        /// </summary>
        public int UID { get; protected set; }
        /// <summary>
        /// 网络数据发送频率, 大概每秒发送1000个数据
        /// </summary>
        [Obsolete("已弃用, 发送频率固定为1ms!", false)]
        public int SyncFrequency { get; set; } = 1;
        /// <summary>
        /// 在多线程调用unity主线程的上下文对象
        /// </summary>
        public SynchronizationContext Context { get; protected set; }
        /// <summary>
        /// 允许叠包缓冲器最大值 默认可发送5242880(5M)的数据包
        /// </summary>
        public int StackBufferSize { get; set; } = 5242880;
        /// <summary>
        /// 允许叠包最大次数，如果数据包太大，接收数据的次数超出StackNumberMax值，则会清除叠包缓存器 默认可叠包50次
        /// </summary>
        public int StackNumberMax { get; set; } = 50;
        /// <summary>
        /// TCP叠包值， 0:正常 >1:叠包次数 > StackNumberMax :清空叠包缓存流
        /// </summary>
        protected int stack;
        internal string stackStreamName;
        internal int stackIndex;
        internal int stackCount;
        /// <summary>
        /// TCP叠包临时缓存流
        /// </summary>
        protected FileStream StackStream { get; set; }
        /// <summary>
        /// 在多线程中调用OperationSync事件?
        /// </summary>
        public bool OperationSyncInThread { get; set; }
        /// <summary>
        /// 玩家操作是以可靠传输进行发送的?     
        /// 服务器的对应属性SendOperationReliable在 NetScene类里面
        /// </summary>
        public bool SendOperationReliable { get; set; }
        /// <summary>
        /// 待发送的操作列表
        /// </summary>
        private readonly ListPool<Operation> operations = new ListPool<Operation>();
        /// <summary>
        /// <para>（Maxium Transmission Unit）最大传输单元, 最大传输单元为1500字节, 这里默认为50000, 如果数据超过50000,则是该框架进行分片. 传输层则需要分片为50000/1472=34个数据片</para>
        /// <para>------ 局域网可以设置为50000, 公网需要设置为1300 或 1400, 如果设置为1400还是发送失败, 则需要设置为1300或以下进行测试 ------</para>
        /// <para>1.链路层：以太网的数据帧的长度为(64+18)~(1500+18)字节，其中18是数据帧的帧头和帧尾，所以数据帧的内容最大为1500字节（不包括帧头和帧尾），即MUT为1500字节</para>
        /// <para>2.网络层：IP包的首部要占用20字节，所以这里的MTU＝1500－20＝1480字节</para>
        /// <para>3.传输层：UDP包的首部要占有8字节，所以这里的MTU＝1480－8＝1472字节</para>
        /// <see langword="注意:服务器和客户端的MTU属性的值必须保持一致性,否则分包的数据将解析错误!"/> <see cref="Server.ServerBase{Player, Scene}.MTU"/>
        /// </summary>
        public int MTU { get; set; } = 1300;
        /// <summary>
        /// （Retransmission TimeOut）重传超时时间。 默认为20毫秒重传一次
        /// </summary>
        public int RTO { get; set; } = 20;
        /// <summary>
        /// 超时重传模式 默认为可变重传
        /// </summary>
        public RTOMode RTOMode { get; set; }
        public double currRto = 50;
        /// <summary>
        /// (Maximum traffic per second) 每秒允许传输最大流量, 默认最大每秒可以传输1m大小
        /// </summary>
        public int MTPS { get; set; } = 1024 * 1024;

        /// <summary>
        /// 客户端端口
        /// </summary>
        protected int localPort;
        /// <summary>
        /// 当前可靠帧, 比如服务器发送两帧可靠数据帧给客户端, 客户端只需要收到一个帧数据即可 (发送帧)
        /// </summary>
        internal uint sendReliableFrame;
        /// <summary>
        /// 当前可靠帧, 比如服务器发送两帧可靠数据帧给客户端, 客户端只需要收到一个帧数据即可 (接收帧)
        /// </summary>
        internal uint revdReliableFrame;
        /// <summary>
        /// 使用字节压缩吗? 如果使用, 每次发送字节大于1000个后进行压缩处理
        /// </summary>
        public bool ByteCompression { get; set; }
        /// <summary>
        /// 可靠传输是排队模式? 排队模式下, 可靠包是一个一个处理. 不排队模式: 可靠传输数据组成多列并发 ---> 默认是无排队模式
        /// </summary>
        public bool Seqencing { get; set; }
        /// <summary>
        /// 是以太网? 此属性控制组包发送时,执行一次能把n个数据包组合在一起, 然后一次发送, 全由数据包大小决定. 如果此属性是以太网(true), 则根据mut来判断, 否则是局域网, 固定值50000字节
        /// </summary>
        public bool IsEthernet { get; set; }
        /// <summary>
        /// 组包数量，如果是一些小数据包，最多可以组合多少个？ 默认是组合1000个后发送
        /// </summary>
        public int PackageLength { get; set; } = 1000;

        internal string persistentDataPath;
        private readonly MyDictionary<uint, FrameList> revdFrames = new MyDictionary<uint, FrameList>();
        private long fileStreamCurrPos;
        private readonly MyDictionary<ushort, VarSyncInfo> varSyncInfos = new MyDictionary<ushort, VarSyncInfo>();
        private readonly MyDictionary<int, FileData> ftpDic = new MyDictionary<int, FileData>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public ClientBase()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="useUnityThread">
        /// 是否使用unity主线程进行每一帧更新？  
        /// True：使用unity的Update等方法进行更新，unity的组建可以在Rpc函数内进行调用。
        /// False：使用多线程进行网络更新，使用多线程更新后unity的组件将不能在rpc函数内进行赋值设置等操作，否则会出错!
        /// </param>
        public ClientBase(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~ClientBase()
        {
#if !UNITY_EDITOR || BUILT_UNITY
            Close();
#elif UNITY_EDITOR
            Close(false, 1);
#endif
        }

        public List<RPCMethod> RPCs { get { return Rpcs; } set { Rpcs = value; } }
        public MyDictionary<string, List<RPCMethod>> RPCsDic { get { return RpcsDic; } set { RpcsDic = value; } }

        /// <summary>
        /// 添加网络Rpc
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        public void AddRpcHandle(object target)
        {
            AddRpcHandle(target, false);
        }

        /// <summary>
        /// 添加网络Rpc
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        /// <param name="append">一个Rpc方法是否可以多次添加到Rpcs里面？</param>
        public void AddRpcHandle(object target, bool append)
        {
            if (OnAddRpcHandle == null)
                OnAddRpcHandle = AddRpcInternal;
            OnAddRpcHandle(target, append);
        }

        private void AddRpcInternal(object target, bool append)
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
            Type type = target.GetType();
            var members = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var info in members)
            {
                RPCFun rpc = info.GetCustomAttribute<RPCFun>();
                if (rpc != null)
                {
                    RPCMethod item = new RPCMethod(target, info as MethodInfo, rpc.cmd);
                    if (rpc.mask != 0)
                    {
                        if (!RpcMaskDic.TryGetValue(rpc.mask, out string func))
                            RpcMaskDic.Add(rpc.mask, info.Name);
                        else if (func != info.Name)
                            NDebug.LogError($"错误! 请修改Rpc方法{info.Name}或{func}的mask值, mask值必须是唯一的!");
                    }
                    AddRpc(RpcsDic, Rpcs, item);
                }
                else
                {
                    VarSync varSync = info.GetCustomAttribute<VarSync>();
                    if (varSync == null)
                        continue;
                    if(info is FieldInfo field)
                    {
                        var code = Type.GetTypeCode(field.FieldType);
                        if (code == TypeCode.Object & !field.FieldType.IsEnum)
                        {
                            NDebug.LogError($"错误! 尚未支持同步字段,属性的{field.FieldType}类型! 错误定义:{target.GetType().Name}类的{field.Name}字段");
                            continue;
                        }
                        if (!varSyncInfos.TryGetValue(varSync.id, out VarSyncInfo varSyncInfo))
                        {
                            varSyncInfos.Add(varSync.id, new VarSyncFieldInfo()
                            {
                                id = varSync.id,
                                type = field.FieldType,
                                target = target,
                                fieldInfo = field,
                                value = field.GetValue(target),
                                passive = varSync.passive
                            });
                        }
                        else if (varSyncInfo is VarSyncFieldInfo field1)
                        {
                            NDebug.LogError($"错误! 变量同步唯一id冲突, {field1.target.GetType().Name}类的{field1.fieldInfo.Name}字段和{target.GetType().Name}类的{field.Name}字段id冲突!");
                        }
                        else if (varSyncInfo is VarSyncPropertyInfo property)
                        {
                            NDebug.LogError($"错误! 变量同步唯一id冲突, {property.target.GetType().Name}类的{property.propertyInfo.Name}字段和{target.GetType().Name}类的{field.Name}字段id冲突!");
                        }
                    }
                    else if (info is PropertyInfo property)
                    {
                        if (!property.CanRead | !property.CanWrite)
                        {
                            NDebug.LogError($"错误! {target.GetType().Name}类的{property.Name}属性不能完全读写!");
                            continue;
                        }
                        var code = Type.GetTypeCode(property.PropertyType);
                        if (code == TypeCode.Object & !property.PropertyType.IsEnum)
                        {
                            NDebug.LogError($"错误! 尚未支持同步字段,属性的{property.PropertyType}类型! 错误定义:{target.GetType().Name}类的{property.Name}属性字段");
                            continue;
                        }
                        if (!varSyncInfos.TryGetValue(varSync.id, out VarSyncInfo varSyncInfo))
                        {
                            varSyncInfos.Add(varSync.id, new VarSyncPropertyInfo()
                            {
                                id = varSync.id,
                                type = property.PropertyType,
                                target = target,
                                propertyInfo = property,
                                value = property.GetValue(target),
                                passive = varSync.passive
                            });
                        }
                        else if (varSyncInfo is VarSyncFieldInfo field1)
                        {
                            NDebug.LogError($"错误! 变量同步唯一id冲突, {field1.target.GetType().Name}类的{field1.fieldInfo.Name}字段和{target.GetType().Name}类的{property.Name}字段id冲突!");
                        }
                        else if (varSyncInfo is VarSyncPropertyInfo property1)
                        {
                            NDebug.LogError($"错误! 变量同步唯一id冲突, {property1.target.GetType().Name}类的{property1.propertyInfo.Name}字段和{target.GetType().Name}类的{property.Name}字段id冲突!");
                        }
                    }
                }
            }
        }

        private void AddRpc(MyDictionary<string, List<RPCMethod>> rpcs, List<RPCMethod> rpcsList, RPCMethod item)
        {
            if (!rpcs.ContainsKey(item.method.Name))
                rpcs.Add(item.method.Name, new List<RPCMethod>());
            rpcs[item.method.Name].Add(item);
            rpcsList.Add(item);
        }

        /// <summary>
        /// 移除子客户端的RPCFun函数
        /// </summary>
        /// <param name="target">将此对象的所有带有RPCFun特性的函数移除</param>
        public void RemoveRpc(object target)
        {
            if (OnRemoveRpc == null)
                OnRemoveRpc = RemoveRpcInternal;
            OnRemoveRpc(target);
        }

        private void RemoveRpcInternal(object target)
        {
            if (target is string key)
            {
                if (RpcsDic.ContainsKey(key))
                    RpcsDic.Remove(key);
                return;
            }
            var rpcsList = new List<RPCMethod>();
            var dic = new Dictionary<string, List<RPCMethod>>(RpcsDic);
            foreach (var rpcs in dic)
            {
                for (int i = 0; i < rpcs.Value.Count; i++)
                {
                    if (target is Delegate @delegate)
                    {
                        if (rpcs.Value[i].method.Equals(@delegate.Method))
                        {
                            rpcs.Value.RemoveAt(i);
                            if (i > 0) i--;
                        }
                        continue;
                    }
                    if (rpcs.Value[i].target.Equals(target) | rpcs.Value[i].method.Equals(null))
                    {
                        rpcs.Value.RemoveAt(i);
                        if (i > 0) i--;
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
        /// 绑定Rpc函数
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        public void BindRpc(object target) => AddRpcHandle(target);

        /// <summary>
        /// 绑定网络调式信息处理接口
        /// </summary>
        /// <param name="debug"></param>
        [Obsolete("请使用NDebug类输出调式信息!")]
        public void BindLogHandle(IDebugHandle debug)
        {
        }

        /// <summary>
        /// 绑定网络状态处理接口
        /// </summary>
        /// <param name="network"></param>
        public void BindNetworkHandle(INetworkHandle network)
        {
            OnConnectedHandle += network.OnConnected;
            OnConnectFailedHandle += network.OnConnectFailed;
            OnConnectLostHandle += network.OnConnectLost;
            OnDisconnectHandle += network.OnDisconnect;
            OnReconnectHandle += network.OnReconnect;
            OnTryToConnectHandle += network.OnTryToConnect;
            OnCloseConnectHandle += network.OnCloseConnect;
            OnBlockConnectionHandle += network.OnBlockConnection;
        }

        /// <summary>
        /// 移除网络状态处理接口
        /// </summary>
        /// <param name="network"></param>
        public void RemoveNetworkHandle(INetworkHandle network)
        {
            OnConnectedHandle -= network.OnConnected;
            OnConnectFailedHandle -= network.OnConnectFailed;
            OnConnectLostHandle -= network.OnConnectLost;
            OnDisconnectHandle -= network.OnDisconnect;
            OnReconnectHandle -= network.OnReconnect;
            OnTryToConnectHandle -= network.OnTryToConnect;
            OnCloseConnectHandle -= network.OnCloseConnect;
            OnBlockConnectionHandle -= network.OnBlockConnection;
        }

        /// <summary>
        /// 添加网络函数数据,添加后自动在rpc中调用
        /// </summary>
        /// <param name="model"></param>
        public void AddRpcBuffer(RPCModel model)
        {
            if (model.methodMask != 0)
                if (!RpcMaskDic.TryGetValue(model.methodMask, out model.func)) model.func = $"[mask:{model.methodMask}]";
            if (string.IsNullOrEmpty(model.func))
                return;
            if (rpcTasks.TryGetValue(model.func, out RPCModelTask model1))
            {
                model1.model = model;
                model1.IsCompleted = true;
                rpcTasks.Remove(model.func);
                return;
            }
            if (!RpcsDic.TryGetValue(model.func, out List<RPCMethod> rpcs))
            {
                NDebug.LogWarning($"{model.func}的远程方法未被收集!请定义[Rpc]void {model.func}方法和参数, 并使用client.AddRpcHandle方法收集rpc方法!");
                return;
            }
            if (rpcs.Count <= 0)
            {
                NDebug.LogWarning($"{model.func}的远程方法未被收集!请定义[Rpc]void {model.func}方法和参数, 并使用client.AddRpcHandle方法收集rpc方法!");
                return;
            }
            foreach (RPCMethod rpc in rpcs)
            {
                if (rpc.cmd == NetCmd.ThreadRpc)
                {
                    rpc.Invoke(model.pars);
                    continue;
                }
                RevdBuffer buffer = new RevdBuffer(rpc.target, rpc.method, model.pars);
                revdBuffers.Add(buffer);
            }
        }

        /// <summary>
        /// 开启心跳线程
        /// </summary>
        public void StartHeartHandle()
        {
            StartThread("HeartHandle", HeartHandle);
        }

        /// <summary>
        /// 开启线程
        /// </summary>
        /// <param name="threadKey">线程名称</param>
        /// <param name="start">线程函数</param>
        public void StartThread(string threadKey, ThreadStart start)
        {
            if (!threadDic.TryGetValue(threadKey, out Thread thread))
            {
                thread = new Thread(start)
                {
                    IsBackground = true,
                    Name = threadKey
                };
                thread.Start();
                threadDic.TryAdd(threadKey, thread);
            }
            string str = thread.ThreadState.ToString();
            if (str.Contains("Abort") | str.Contains("Stop") | str.Contains("WaitSleepJoin"))
            {
                thread.Abort();
                threadDic.TryRemove(threadKey, out _);
                StartThread(threadKey, start);
            }
        }

        /// <summary>
        /// 结束指定的线程
        /// </summary>
        /// <param name="threadKey">线程名称键值</param>
        public void AbortedThread(string threadKey)
        {
            if (threadDic.ContainsKey(threadKey))
                threadDic[threadKey].Abort();
        }

        /// <summary>
        /// 结束所有线程
        /// </summary>
        public void AbortedThread()
        {
            foreach (Thread thread in threadDic.Values)
            {
                thread?.Abort();
            }
            threadDic.Clear();
        }

        /// <summary>
        /// 每一帧执行线程
        /// </summary>
        protected void UpdateHandle()
        {
            while (openClient)
            {
                Thread.Sleep(1);
                try { FixedUpdate(); } catch { }
            }
        }

        /// <summary>
        /// 网络数据更新
        /// </summary>
        public void FixedUpdate()
        {
            int count = revdBuffers.Count;
            while (count > 0)
            {
#if UNITY_EDITOR
                if (UseUnityThread & ThrowException)
                {
                    if (revdBuffers[0].method == null | revdBuffers[0].target == null)
                        goto REMOVEAT;
                    if (LogRpc)
                        NDebug.Log($"RPC:{revdBuffers[0].method}");
                    OnInvokeRpc(revdBuffers[0]);
                REMOVEAT: count--;
                    revdBuffers.RemoveAt(0);
                    continue;
                }
#endif
                try
                {
                    if (LogRpc)
                        NDebug.Log($"RPC:{revdBuffers[0].method}");
                    OnInvokeRpc(revdBuffers[0]);
                }
                catch (Exception e)
                {
                    string bug = $"BUG:{revdBuffers[0].method} -> Target:" + revdBuffers[0].target + " -> Pars:";
                    if (revdBuffers[0].pars == null)
                    {
                        bug += "null";
                        goto LOG;
                    }
                    foreach (object par in revdBuffers[0].pars)
                        bug += par + " , ";
                    LOG: NDebug.LogError(bug + " -> " + e);
                }
                finally
                {
                    count--;
                    revdBuffers.RemoveAt(0);
                }
            }
            StateHandle();
        }

        /// <summary>
        /// 当调用Rpc函数时调用, 如果想提高性能, 可重写此方法自行判断需要调用哪个方法
        /// </summary>
        /// <param name="rpc">远程过程函数对象</param>
        public virtual void OnInvokeRpc(RevdBuffer rpc)
        {
            rpc.Invoke();
        }

        //状态处理
        protected void StateHandle()
        {
            if (networkState == NetworkState.None)
                return;
            switch (networkState)
            {
                case NetworkState.Connected:
                    OnConnectedHandle?.Invoke();
                    break;
                case NetworkState.ConnectFailed:
                    OnConnectFailedHandle?.Invoke();
                    break;
                case NetworkState.TryToConnect:
                    OnTryToConnectHandle?.Invoke();
                    break;
                case NetworkState.ConnectLost:
                    OnConnectLostHandle?.Invoke();
                    break;
                case NetworkState.Disconnect:
                    OnDisconnectHandle?.Invoke();
                    break;
                case NetworkState.ConnectClosed:
                    OnCloseConnectHandle?.Invoke();
                    break;
                case NetworkState.Reconnect:
                    OnReconnectHandle?.Invoke();
                    break;
                case NetworkState.ExceededNumber:
                    OnExceededNumberHandle?.Invoke();
                    break;
                case NetworkState.BlockConnection:
                    OnBlockConnectionHandle?.Invoke();
                    break;
            }
            networkState = NetworkState.None;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public Task Connect()
        {
            return Connect(connected => { });
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="result">连接结果</param>
        /// <returns></returns>
        public Task Connect(Action<bool> result)
        {
            return Connect(host, port, result);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host">IP地址</param>
        /// <param name="port">端口号</param>
        public virtual Task Connect(string host, int port)
        {
            return Connect(host, port, result => { });
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="result">连接结果</param>
        public virtual Task Connect(string host, int port, Action<bool> result)
        {
            return Connect(host, port, -1, result);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="localPort">设置自身端口号,如果不设置自身端口则值为-1</param>
        /// <param name="result">连接结果</param>
        public virtual Task Connect(string host, int port, int localPort, Action<bool> result)
        {
            if (networkState == NetworkState.Connection)
            {
                NDebug.Log("连接服务器中,请稍等...");
                return null;
            }
            if (openClient)
            {
                Close();
                NDebug.Log("连接服务器中,请稍等...");
            }
            openClient = true;
            networkState = NetworkState.Connection;
            if (Instance == null)
                Instance = this;
            if (Context == null)
                Context = SynchronizationContext.Current;
            if (OnAddRpcHandle == null) OnAddRpcHandle = AddRpcInternal;
            if (OnRPCExecute == null) OnRPCExecute = AddRpcBuffer;
            if (OnRemoveRpc == null) OnRemoveRpc = RemoveRpcInternal;
            if (OnSerializeRPC == null) OnSerializeRPC = OnSerializeRpcInternal;
            if (OnDeserializeRPC == null) OnDeserializeRPC = OnDeserializeRpcInternal;
            if (OnSerializeOPT == null) OnSerializeOPT = OnSerializeOptInternal;
            if (OnDeserializeOPT == null) OnDeserializeOPT = OnDeserializeOptInternal;
            AddRpcHandle(this, false);
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
            persistentDataPath = UnityEngine.Application.persistentDataPath;
#else
            persistentDataPath = AppDomain.CurrentDomain.BaseDirectory;
#endif
            if (Client == null) //如果套接字为空则说明没有连接上服务器
            {
                this.host = host;
                this.port = port;
                return ConnectResult(host, port, localPort, result1 =>
                {
                    OnConnected(result1);
                    result(result1);
                });
            }
            else if (!Connected)
            {
                Client.Close();
                Client = null;
                NetworkState = networkState = NetworkState.ConnectLost;
                NDebug.LogError("服务器连接中断!");
                AbortedThread();
                result(false);
            }
            else
            {
                result(true);
            }
            return null;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host">连接的服务器主机IP地址</param>
        /// <param name="port">连接的服务器主机端口号</param>
        /// <param name="localPort">设置自身端口号,如果不设置自身端口则值为-1</param>
        /// <param name="result">连接结果</param>
        protected virtual Task ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            try
            {
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//创建套接字
                this.localPort = localPort;
                if (localPort != -1)
                    Client.Bind(new IPEndPoint(IPAddress.Any, localPort));
                Client.Connect(host, port);
                bool isDone = false;
                Task.Run(() =>
                {
                    while (!isDone)
                    {
                        if (Client != null)
                            SendByteData(new byte[] { 6, 0, 0, 0, 0, 0x2d, 74, NetCmd.Connect, 0, 0, 0, 0 }, false);
                        Thread.Sleep(200);
                    }
                });
                return Task.Run(() =>
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        Client.ReceiveTimeout = 5000;
                        int count = Client.Receive(buffer);
                        Client.ReceiveTimeout = 0;
                        isDone = true;
                        if (buffer[7] == NetCmd.BlockConnection)
                        {
                            InvokeContext((arg) => {
                                networkState = NetworkState.BlockConnection;
                                StateHandle();
                            });
                            throw new Exception();
                        }
                        if (buffer[7] == NetCmd.ExceededNumber)
                        {
                            InvokeContext((arg) => {
                                networkState = NetworkState.ExceededNumber;
                                StateHandle();
                            });
                            throw new Exception();
                        }
                        Connected = true;
                        StartupThread();
                        InvokeContext((arg) => {
                            networkState = NetworkState.Connected;
                            result(true);
                        });
                    }
                    catch (Exception)
                    {
                        isDone = true;
                        Connected = false;
                        Client?.Close();
                        Client = null;
                        InvokeContext((arg) => {
                            networkState = NetworkState.ConnectFailed;
                            result(false);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                NDebug.LogError("连接错误:" + ex.ToString());
                networkState = NetworkState.ConnectFailed;
                result(false);
                return null;
            }
        }

        protected void InvokeContext(SendOrPostCallback action, object arg = null)
        {
            if (Context != null)
                Context.Post(action, arg);
            else action(arg);
        }

        /// <summary>
        /// 局域网广播寻找服务器主机, 如果找到则通过 result 参数调用, 如果成功获取到主机, 那么result的第一个参数为true, 并且result的第二个参数为服务器IP
        /// </summary>
        /// <param name="result">连接结果</param>
        public Task Broadcast(Action<bool, string> result = null)
        {
            return Broadcast(port, result);
        }

        /// <summary>
        /// 局域网广播寻找服务器主机, 如果找到则通过 result 参数调用, 如果成功获取到主机, 那么result的第一个参数为true, 并且result的第二个参数为服务器IP
        /// </summary>
        /// <param name="port">广播到服务器的端口号</param>
        /// <param name="result">连接结果</param>
        public Task Broadcast(int port = 6666, Action<bool, string> result = null)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Broadcast, port);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
            bool isDone = false;
            if (Context == null)
                Context = SynchronizationContext.Current;
            Task.Run(() =>
            {
                while (!isDone)
                {
                    BroadcastSend(client, ipEndPoint);
                    Thread.Sleep(1000);
                }
            });
            return Task.Run(() =>
            {
                try
                {
                    Thread.Sleep(10);//先让上面Task跑起来执行SendTo后再执行下面的Receive,如果还没SendTo就Receive就会出现错误!
                    byte[] buffer = new byte[1024];
                    int count = client.Receive(buffer);
                    string ip = Encoding.Unicode.GetString(buffer, 0, count);
                    isDone = true;
                    client?.Close();
                    client = null;
                    InvokeContext((arg) => { result(true, ip); });
                }
                catch (Exception ex)
                {
                    isDone = true;
                    client?.Close();
                    client = null;
                    InvokeContext((arg) => { result(false, ex.ToString()); });
                }
            });
        }

        protected virtual void BroadcastSend(Socket client, IPEndPoint ipEndPoint)
        {
            client.SendTo(new byte[] { 6, 0, 0, 0, 0, 0x2d, 74, NetCmd.Broadcast, 0, 0, 0, 0 }, ipEndPoint);
        }

        /// <summary>
        /// 连接成功处理
        /// </summary>
        protected virtual void StartupThread()
        {
            Connected = true;
            StartThread("SendHandle", SendDataHandle);
            StartThread("ReceiveHandle", ReceiveHandle);
            StartThread("NetworkFlowHandle", NetworkFlowHandle);
            StartThread("CheckRpcHandle", CheckRpcHandle);
            StartThread("HeartHandle", HeartHandle);
            StartThread("VarSyncHandler", VarSyncHandler);
            if (!UseUnityThread)
                StartThread("UpdateHandle", UpdateHandle);
#if UNITY_ANDROID
            if (Context == null)
                return;
            Context.Post(new SendOrPostCallback((o) => {
                var randomName = RandomHelper.Range(0, int.MaxValue);
                fileStreamName = UnityEngine.Application.persistentDataPath + "/rtTemp" + randomName + ".tmp";
            }), null);
#else
            fileStreamName = Path.GetTempFileName();
#endif
        }

        /// <summary>
        /// 连接结果处理
        /// </summary>
        /// <param name="result">结果</param>
        protected virtual void OnConnected(bool result)
        {
            if (result)
            {
                NetworkState = networkState = NetworkState.Connected;
                NDebug.Log("成功连接服务器...");
            }
            else
            {
                NetworkState = networkState = NetworkState.ConnectFailed;
                NDebug.LogError("服务器尚未开启或连接IP端口错误!");
                if (!UseUnityThread)
                    StartThread("UpdateHandle", UpdateHandle);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="reuseSocket">断开连接后还能重新使用？</param>
        public void Disconnect(bool reuseSocket)
        {
            NetworkState = networkState = NetworkState.Disconnect;
            Client.Disconnect(reuseSocket);
        }

        /// <summary>
        /// 调式输出网络流量信息
        /// </summary>
        protected void NetworkFlowHandle()
        {
            while (Connected)
            {
                Thread.Sleep(1000);
                OnNetworkFlowHandle();
            }
        }

        protected virtual void OnNetworkFlowHandle()
        {
            try
            {
                OnNetworkDataTraffic?.Invoke(sendAmount, sendCount, receiveAmount, receiveCount, resolveAmount, sendLoopNum, revdLoopNum);
            }
            catch (Exception ex)
            {
                NDebug.LogError(ex.ToString());
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

        /// <summary>
        /// rpc检查处理线程
        /// </summary>
        protected void CheckRpcHandle()
        {
            while (Connected)
            {
                try
                {
                    Thread.Sleep(1);
                    CheckEventsUpdate();
                    if (OnCheckRpcUpdate == null)
                        OnCheckRpcUpdate = CheckRpcUpdate;
                    OnCheckRpcUpdate();
                }
                catch (Exception e)
                {
                    NDebug.LogError(e);
                }
            }
        }

        //检查rpc函数
        private void CheckRpcUpdate()
        {
            var entries = RpcsDic.entries;
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].hashCode == 0)
                    continue;
                var rpcs1 = entries[i].value;
                if (rpcs1 == null)
                    continue;
                for (int n = 0; n < rpcs1.Count; n++)
                {
                    if (rpcs1[n].method == null)
                    {
                        rpcs1.RemoveAt(n);
                        UpdateRpcs();
                        continue;
                    }
                    if (rpcs1[n].method.IsStatic)
                        continue;
                    if (rpcs1[n].target == null)
                    {
                        rpcs1.RemoveAt(n);
                        UpdateRpcs();
                        continue;
                    }
                    if (rpcs1[n].target.Equals(null) | rpcs1[n].method.Equals(null))
                    {
                        rpcs1.RemoveAt(n);
                        UpdateRpcs();
                    }
                }
            }
        }

        private void UpdateRpcs()
        {
            var rpcsList = new List<RPCMethod>();
            var dic = new Dictionary<string, List<RPCMethod>>(RpcsDic);
            foreach (var rpcs in dic)
                rpcsList.AddRange(rpcs.Value);
            Rpcs = rpcsList;
        }

        //检测事件委托函数
        private void CheckEventsUpdate()
        {
            Type type = typeof(ClientBase);
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {
                object value = fields[i].GetValue(this);
                if (value == null)
                    continue;
                Delegate dele = value as Delegate;
                if (dele == null)
                    continue;
                var ds = new List<Delegate>(dele.GetInvocationList());
                int oldCount = ds.Count;
                for (int a = 0; a < ds.Count; a++)
                {
                    if (ds[a].Method == null)
                    {
                        ds.RemoveAt(a);
                        if (a > 0) a--;
                        continue;
                    }
                    if (ds[a].Method.IsStatic)//静态方法不需要判断对象是否为空
                        continue;
                    if (ds[a].Target == null)
                    {
                        ds.RemoveAt(a);
                        if (a > 0) a--;
                        continue;
                    }
                    if (ds[a].Target.Equals(null) | ds[a].Method.Equals(null))
                    {
                        ds.RemoveAt(a);
                        if (a > 0) a--;
                    }
                }
                if (oldCount != ds.Count)
                    fields[i].SetValue(this, Delegate.Combine(ds.ToArray()));
            }
        }

        /// <summary>
        /// 发包线程
        /// </summary>
        protected void SendDataHandle()
        {
            while (Connected)
            {
                try
                {
                    Thread.Sleep(1);
                    SendDirect();
                }
                catch (Exception ex)
                {
                    NetworkException(ex);
                }
            }
        }

        /// <summary>
        /// 当游戏操作行为封包数据时调用
        /// </summary>
        /// <param name="count"></param>
        protected virtual void OnOptPacket(int count)
        {
            var operations1 = operations.GetRemoveRange(0, count);
            OperationList list = new OperationList(operations1);
            var buffer = OnSerializeOPT(list);
            if (SendOperationReliable)
                rtRPCModels.Enqueue(new RPCModel(NetCmd.OperationSync, buffer, false, false));
            else
                rPCModels.Enqueue(new RPCModel(NetCmd.OperationSync, buffer, false, false));
        }

        protected internal virtual byte[] OnSerializeOptInternal(OperationList list)
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

        protected internal virtual OperationList OnDeserializeOptInternal(byte[] buffer, int index, int count)
        {
            using (MemoryStream stream = new MemoryStream(buffer, index, count))
            {
                OperationList list = ProtoBuf.Serializer.Deserialize<OperationList>(stream);
                return list;
            }
        }

        /// <summary>
        /// 立刻发送, 不需要等待帧时间 (当你要强制把客户端下线时,你还希望客户端先发送完数据后,再强制客户端退出游戏用到)
        /// </summary>
        public virtual void SendDirect()
        {
            SendOperations();
            SendDataHandle(rPCModels, false);
            SendRTDataHandle();
        }

        /// <summary>
        /// 打包操作同步马上要发送了
        /// </summary>
        protected virtual void SendOperations()
        {
            int count = operations.Count;
            if (count > 0)
            {
                while (count > 500)
                {
                    OnOptPacket(500);
                    count -= 500;
                }
                if (count > 0)
                {
                    OnOptPacket(count);
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

        protected virtual void WriteDataBody(ref Segment stream, QueueSafe<RPCModel> rPCModels, int count, bool reliable)
        {
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                if (!rPCModels.TryDequeue(out RPCModel rPCModel))
                    continue;
                if (rPCModel.kernel & rPCModel.serialize)
                {
                    rPCModel.buffer = OnSerializeRPC(rPCModel);
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
                    SendByteData(buffer, reliable);
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

        /// <summary>
        /// 发送处理
        /// </summary>
        protected virtual void SendDataHandle(QueueSafe<RPCModel> rPCModels, bool reliable)
        {
            int count = rPCModels.Count;
            if (count <= 0)
                return;
            var stream = BufferPool.Take();
            WriteDataHead(stream);
            WriteDataBody(ref stream, rPCModels, count, reliable);
            byte[] buffer = SendData(stream);
            SendByteData(buffer, reliable);
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

        protected virtual void SendRTDataHandle()
        {
            if (RTOMode == RTOMode.Fixed)
                currRto = RTO;
            int rtcount = sendRTList.Values.Count;
            if ((rtcount > 0 & Seqencing) | (rtcount > 0 & sendRTListCount > 5) | rtcount > 100)
                goto JUMP;
            int count = rtRPCModels.Count;
            if (count == 0 & rtcount > 0)
                goto JUMP;
            if (count == 0)
                return;
            if (count > 1000)
                count = 1000;
            var stream = BufferPool.Take();
            WriteDataBody(ref stream, rtRPCModels, count, true);
            int len = stream.Position;
            int index = 0;
            ushort dataIndex = 0;
            float dataCount = (float)len / MTU;
            var rtDic = new MyDictionary<ushort, RTBuffer>();
            sendRTList.TryAdd(sendReliableFrame, rtDic);
            var stream1 = BufferPool.Take();
            WriteDataHead(stream1);
            stream1.WriteByte(74);
            stream1.WriteByte(NetCmd.ReliableTransport);
            while (index < len)
            {
                int count1 = MTU;
                if (index + count1 >= len)
                    count1 = len - index;
                stream1.Position = 8;
                stream1.Write(BitConverter.GetBytes(count1 + 16), 0, 4);
                stream1.Write(BitConverter.GetBytes(dataIndex), 0, 2);
                stream1.Write(BitConverter.GetBytes((ushort)Math.Ceiling(dataCount)), 0, 2);
                stream1.Write(BitConverter.GetBytes(count1), 0, 4);
                stream1.Write(BitConverter.GetBytes(len), 0, 4);
                stream1.Write(BitConverter.GetBytes(sendReliableFrame), 0, 4);
                stream1.Write(stream, index, count1);
                byte[] buffer = SendData(stream1);
                rtDic.Add(dataIndex, new RTBuffer(buffer));
                index += MTU;
                dataIndex++;
            }
            BufferPool.Push(stream);
            BufferPool.Push(stream1);
            sendRTListCount = rtDic.Count;
            sendReliableFrame++;
        JUMP:
            count = ackQueue.Count;
            for (int i = 0; i < count; i++)
            {
                if (!ackQueue.TryDequeue(out AckQueue ack))
                    continue;
                if (!sendRTList.ContainsKey(ack.frame))
                    continue;
                var rtlist = sendRTList[ack.frame];
                if (!rtlist.ContainsKey(ack.index))
                    continue;
                rtlist.Remove(ack.index);
                if (rtlist.Count == 0)
                    sendRTList.TryRemove(ack.frame, out _);
                InvokeSendRTProgress(sendRTListCount - rtlist.Count, sendRTListCount);
            }
            int bytesLen = 0;
            foreach (var rtlist in sendRTList.Values)
            {
                foreach (var list in rtlist)
                {
                    RTBuffer rtb = list.Value;
                    if (DateTime.Now < rtb.time)
                        continue;
                    rtb.time = DateTime.Now.AddMilliseconds(currRto);
                    bytesLen += rtb.buffer.Length;
                    SendByteData(rtb.buffer, true);
                    if (bytesLen > MTPS / 1000)//一秒最大发送1m数据, 这里会有可能每秒执行1000次
                        return;
                }
            }
        }

        protected virtual void SendByteData(byte[] buffer, bool reliable)
        {
            sendCount += buffer.Length;
            sendAmount++;
            if (buffer.Length <= 65507)
                Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
            else
                NDebug.LogError("数据过大, 请使用SendRT发送...");
        }

        /// <summary>
        /// 当内核序列化远程函数时调用, 如果想改变内核rpc的序列化方式, 可重写定义序列化协议
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected internal virtual byte[] OnSerializeRpcInternal(RPCModel model) { return NetConvert.Serialize(model); }
        /// <summary>
        /// 当内核解析远程过程函数时调用, 如果想改变内核rpc的序列化方式, 可重写定义解析协议
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected internal virtual FuncData OnDeserializeRpcInternal(byte[] buffer, int index, int count) { return NetConvert.Deserialize(buffer, index, count); }

        /// <summary>
        /// CRC校验代码表, 用户可自行改变CRC校验码, 直接改源代码, 客户端和服务器检验码必须一致, 否则识别失败
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
            JUMP: NDebug.LogError("CRC校验失败:");
            return false;
        }

        /// <summary>
        /// 后台线程接收数据
        /// </summary>
        protected virtual void ReceiveHandle()
        {
            var segment = BufferPool.Take(65507);
            while (Connected)
            {
                try
                {
                    ReceiveUpdate(segment);
                }
                catch (Exception ex)
                {
                    NetworkException(ex);
                }
            }
            BufferPool.Push(segment);
        }

        /// <summary>
        /// 接收数据和更新处理
        /// </summary>
        /// <param name="segment"></param>
        public void ReceiveUpdate(Segment segment)
        {
            if (Client.Poll(1, SelectMode.SelectRead))
            {
                int count = Client.Receive(segment);
                receiveCount += count;
                receiveAmount++;
                heart = 0;
                ResolveBuffer(segment, 0, count, false);
                revdLoopNum++;
            }
        }

        /// <summary>
        /// 网络异常处理
        /// </summary>
        /// <param name="ex"></param>
        protected void NetworkException(Exception ex)
        {
            if (ex is SocketException)
            {
                Connected = false;
                NetworkState = networkState = NetworkState.ConnectLost;
                sendRTList.Clear();
                revdRTList.Clear();
                rtRPCModels = new QueueSafe<RPCModel>();
                rPCModels = new QueueSafe<RPCModel>();
                NDebug.LogError("连接中断!" + ex);
            }
            else if (ex is ObjectDisposedException)
            {
                Close();
                NDebug.LogError("客户端已被释放!" + ex);
            }
            else if (Connected)
            {
                NDebug.LogError("发送或接收异常:" + ex);
            }
        }

#if TEST
        public void TestResolveBuffer(Segment buffer, int index, int count) => ResolveBuffer(buffer, index, count, false);
#endif
        /// <summary>
        /// 解析网络数据包
        /// </summary>
        protected virtual void ResolveBuffer(Segment buffer, int index, int count, bool isTcp)
        {
            int size = BitConverter.ToInt32(buffer, index);
            int crcIndex = buffer[index + 4];//CRC检验索引, 使用者自己改变CRCCode属性
            byte crcCode = buffer[index + 5];//CRC校验码, 使用者自己改变CRCCode属性
            index += frame;
            if (index + size == count)
            {
                if (!OnCRC(crcIndex, crcCode))
                    return;
                DataHandle(buffer, index, size, count);
            }
        }

        protected void DataHandle(Segment buffer, int index, int size, int count)
        {
            if (index + 3 > count)
                return;
            if (buffer[index + 0] == 31 & buffer[index + 1] == 139 & buffer[index + 2] == 8)
            {
                buffer = UnZipHelper.Decompress(buffer, index, size);
                index = 0;
                count = buffer.Length;
            }
            while (index < count)
            {
                bool kernel = buffer[index] == 68;
                if (!kernel & buffer[index] != 74)
                {
                    NDebug.LogError("[忽略]协议出错!");
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
                if (kernel)
                {
                    FuncData func = OnDeserializeRPC(buffer, index, dataCount);
                    if (func.error)
                        break;
                    rpc.func = func.name;
                    rpc.pars = func.pars;
                    rpc.methodMask = func.mask;
                }
                index += dataCount;
                RPCDataHandle(rpc);
            }
        }

        protected virtual void RPCDataHandle(RPCModel model)
        {
            resolveAmount++;
            switch (model.cmd)
            {
                case NetCmd.RevdHeartbeat:
                    heart = 0;
                    break;
                case NetCmd.SendHeartbeat:
                    Send(NetCmd.RevdHeartbeat, new byte[0]);
                    break;
                case NetCmd.CallRpc:
                    if (model.kernel)
                        OnRPCExecute(model);
                    else
                        InvokeOnRevdBufferHandle(model);
                    break;
                case NetCmd.Local:
                    if (model.kernel)
                        OnRPCExecute(model);
                    else
                        InvokeOnRevdBufferHandle(model);
                    break;
                case NetCmd.LocalRT:
                    if (model.kernel)
                        OnRPCExecute(model);
                    else
                        InvokeOnRevdBufferHandle(model);
                    break;
                case NetCmd.Scene:
                    if (model.kernel)
                        OnRPCExecute(model);
                    else
                        InvokeOnRevdBufferHandle(model);
                    break;
                case NetCmd.SceneRT:
                    if (model.kernel)
                        OnRPCExecute(model);
                    else
                        InvokeOnRevdBufferHandle(model);
                    break;
                case NetCmd.Notice:
                    if (model.kernel)
                        OnRPCExecute(model);
                    else
                        InvokeOnRevdBufferHandle(model);
                    break;
                case NetCmd.NoticeRT:
                    if (model.kernel)
                        OnRPCExecute(model);
                    else
                        InvokeOnRevdBufferHandle(model);
                    break;
                case NetCmd.ThreadRpc:
                    if (model.kernel)
                        OnRPCExecute(model);
                    else
                        InvokeOnRevdBufferHandle(model);
                    break;
                case NetCmd.ExceededNumber:
                    networkState = NetworkState.ExceededNumber;
                    break;
                case NetCmd.BlockConnection:
                    networkState = NetworkState.BlockConnection;
                    break;
                case NetCmd.ReliableTransport:
                    ushort index = BitConverter.ToUInt16(model.buffer, model.index + 0);
                    ushort entry = BitConverter.ToUInt16(model.buffer, model.index + 2);
                    int count = BitConverter.ToInt32(model.buffer, model.index + 4);
                    int dataCount = BitConverter.ToInt32(model.buffer, model.index + 8);
                    uint frame = BitConverter.ToUInt32(model.buffer, model.index + 12);
                    byte[] rtbuffer = new byte[6];
                    if (revdReliableFrame > frame)
                    {
                        Buffer.BlockCopy(BitConverter.GetBytes(frame), 0, rtbuffer, 0, 4);
                        Buffer.BlockCopy(BitConverter.GetBytes(index), 0, rtbuffer, 4, 2);
                        Send(NetCmd.ReliableCallback, rtbuffer);//ack发送过程中丢失了, 需要补发
                        return;
                    }
                    if (revdRTStream == null)
                        revdRTStream = new FileStream(fileStreamName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    if (!revdFrames.ContainsKey(frame))
                    {
                        revdFrames.Add(frame, new FrameList(entry)
                        {
                            streamPos = fileStreamCurrPos,
                            frameLen = entry,
                            frame = frame,
                            dataCount = dataCount
                        });
                        fileStreamCurrPos += dataCount;
                        if (fileStreamCurrPos > (1024 * 1024 * 1024))//如果文件大于1g, 则从0开始记录
                            fileStreamCurrPos = 0;
                    }
                    FrameList revdFrame = revdFrames[frame];
                    if (revdFrame.Add(index))
                    {
                        revdRTStream.Seek(revdFrame.streamPos + (index * MTU), SeekOrigin.Begin);
                        revdRTStream.Write(model.buffer, model.index + 16, count);
                        InvokeRevdRTProgress(revdFrame.Count, entry);
                    }
                    Buffer.BlockCopy(BitConverter.GetBytes(frame), 0, rtbuffer, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(index), 0, rtbuffer, 4, 2);
                    Send(NetCmd.ReliableCallback, rtbuffer);
                    if (revdFrames.ContainsKey(revdReliableFrame))//排序执行
                        revdFrame = revdFrames[revdReliableFrame];
                    else //让客户端发送revdReliableFrame帧的所有帧数据
                    {
                        rtbuffer = new byte[4];
                        Buffer.BlockCopy(BitConverter.GetBytes(revdReliableFrame), 0, rtbuffer, 0, 4);
                        Send(NetCmd.TakeFrameList, rtbuffer);
                        return;
                    }
                    while (revdFrame.Count >= revdFrame.frameLen)
                    {
                        revdFrames.Remove(revdReliableFrame);
                        revdReliableFrame++;
                        var buffer = BufferPool.Take(revdFrame.dataCount);
                        revdRTStream.Seek(revdFrame.streamPos, SeekOrigin.Begin);
                        revdRTStream.Read(buffer, 0, revdFrame.dataCount);
                        DataHandle(buffer, 0, revdFrame.dataCount, revdFrame.dataCount);
                        BufferPool.Push(buffer);
                        if (revdFrames.ContainsKey(revdReliableFrame))
                            revdFrame = revdFrames[revdReliableFrame];
                        else break;//结束死循环
                    }
                    for (ushort i = 0; i < revdFrame.frameLen; i++)
                    {
                        if (!revdFrame.ContainsKey(i))
                        {
                            if (DateTime.Now < revdFrame.time)
                                continue;
                            revdFrame.time = DateTime.Now.AddMilliseconds(currRto);
                            rtbuffer = new byte[6];
                            Buffer.BlockCopy(BitConverter.GetBytes(revdReliableFrame), 0, rtbuffer, 0, 4);
                            Buffer.BlockCopy(BitConverter.GetBytes(i), 0, rtbuffer, 4, 2);
                            Send(NetCmd.TakeFrame, rtbuffer);
                        }
                    }
                    break;
                case NetCmd.ReliableCallback:
                    uint frame1 = BitConverter.ToUInt32(model.buffer, model.index + 0);
                    ushort index1 = BitConverter.ToUInt16(model.buffer, model.index + 4);
                    ackQueue.Enqueue(new AckQueue(frame1, index1));
                    break;
                case NetCmd.TakeFrame:
                    uint frame2 = BitConverter.ToUInt32(model.buffer, model.index + 0);
                    ushort index2 = BitConverter.ToUInt16(model.buffer, model.index + 4);
                    if (sendRTList.TryGetValue(frame2, out MyDictionary<ushort, RTBuffer> rtbuffer1))
                    {
                        if (rtbuffer1.TryGetValue(index2, out RTBuffer buffer2))
                        {
                            buffer2.time = default;
                        }
                    }
                    break;
                case NetCmd.TakeFrameList:
                    uint frame3 = BitConverter.ToUInt32(model.buffer, model.index + 0);
                    if (sendRTList.TryGetValue(frame3, out MyDictionary<ushort, RTBuffer> rtbuffer2))
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
                case NetCmd.ReliableCallbackClear:
                    NDebug.LogError("可靠传输被清洗, 有可能是你的StackBufferSize和StackNumberMax属性设置的太小, 而数据过大导致!");
                    break;
                case NetCmd.SwitchPort:
                    Task.Run(() => {
                        InvokeContext((arg) => {
                            if (OnSwitchPortHandle != null)
                                OnSwitchPortHandle(model.pars[0].ToString(), (ushort)model.pars[1]);
                            else
                                OnSwitchPortInternal(model.pars[0].ToString(), (ushort)model.pars[1]);
                        });
                    });
                    break;
                case NetCmd.Identify:
                    UID = BitConverter.ToInt32(model.buffer, model.index);
                    Identify = Encoding.Unicode.GetString(model.buffer, model.index + 4, model.count - 4);
                    break;
                case NetCmd.OperationSync:
                    OperationList list = OnDeserializeOPT(model.buffer, model.index, model.count);
                    InvokeOnOperationSync(list);
                    break;
                case NetCmd.Ping:
                    rPCModels.Enqueue(new RPCModel(NetCmd.PingCallback, model.Buffer, model.kernel, false));
                    break;
                case NetCmd.PingCallback:
                    long ticks = BitConverter.ToInt64(model.buffer, model.index);
                    DateTime time = new DateTime(ticks);
                    currRto = DateTime.Now.Subtract(time).TotalMilliseconds + 100d;
                    if (OnPingCallback == null)
                        return;
                    InvokeContext((arg) => { OnPingCallback((currRto - 100d) / 2); });
                    break;
                case NetCmd.P2P:
                    {
                        Segment segment = new Segment(model.buffer, model.index, 10, false);
                        var address = segment.ReadValue<long>();
                        var port = segment.ReadValue<int>();
                        IPEndPoint iPEndPoint = new IPEndPoint(address, port);
                        if (OnP2PCallback == null)
                            return;
                        InvokeContext((arg) => { OnP2PCallback(iPEndPoint); });
                    }
                    break;
                case NetCmd.VarSync:
                    Segment segment1 = new Segment(model.buffer, model.index, model.count, false);
                    while (segment1.Position < segment1.Index + segment1.Count)
                    {
                        var id = segment1.ReadValue<ushort>();
                        if (varSyncInfos.TryGetValue(id, out VarSyncInfo varSync))
                        {
                            var value = segment1.ReadValue(varSync.type);
                            varSync.SetValue(value);
                        }
                        else
                        {
                            NDebug.LogWarning($"未定义同步变量ID={id}, 请定义或收集同步变量, 使用ClientManager.I.client.AddRpcHandle(xxx)方法收集!");
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
                        if (!ftpDic.TryGetValue(key, out FileData fileData))
                        {
                            fileData = new FileData();
                            string path;
                            if (OnDownloadFileHandle != null)
                            {
                                path = OnDownloadFileHandle(fileName);
                                var path1 = Path.GetDirectoryName(path);
                                if (!Directory.Exists(path1))
                                {
                                    NDebug.LogError("文件不存在! 或者文件路径字符串编码错误! 提示:可以使用Notepad++查看, 编码是ANSI,不是UTF8");
                                    return;
                                }
                            }
                            else 
                            {
                                path = Path.GetTempFileName();
                            }
                            fileData.ID = key;
                            fileData.fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                            fileData.fileName = fileName;
                            ftpDic.Add(key, fileData);
                        }
                        fileData.fileStream.Write(buffer, 0, buffer.Length);
                        fileData.Length += buffer.Length;
                        if (fileData.Length >= length)
                        {
                            ftpDic.Remove(key);
                            fileData.fileStream.Position = 0;
                            if (OnReceiveFileHandle != null)
                                InvokeContext((arg) => {
                                    if (OnReceiveFileHandle(fileData))
                                    {
                                        fileData.fileStream.Close();
                                        File.Delete(fileData.fileStream.Name);
                                    }
                                });
                            if (OnRevdFileProgress != null)
                                InvokeContext((arg) => { OnRevdFileProgress(new RTProgress(fileName, fileData.Length / (float)length * 100f, RTState.Complete)); });
                        }
                        else
                        {
                            segment.Position = 0;
                            segment.WriteValue(key);
                            SendRT(NetCmd.Download, segment.ToArray());
                            if (OnRevdFileProgress != null)
                                InvokeContext((arg) => { OnRevdFileProgress(new RTProgress(fileName, fileData.Length / (float)length * 100f, RTState.Download)); });
                        }
                    }
                    break;
                case NetCmd.Download:
                    {
                        Segment segment = new Segment(model.Buffer, false);
                        var key = segment.ReadValue<int>();
                        if (ftpDic.TryGetValue(key, out FileData fileData))
                            SendFile(key, fileData);
                    }
                    break;
                default:
                    InvokeOnRevdBufferHandle(model);
                    break;
            }
        }

        protected virtual void OnSwitchPortInternal(string host, ushort port)
        {
            Close();
            Connect(host, port);
        }

        protected void InvokeRevdRTProgress(int currValue, int dataCount)
        {
            if (OnRevdRTProgress != null)
            {
                float bfb = currValue / (float)dataCount * 100f;
                RTProgress progress = new RTProgress(bfb, RTState.Sending);
                InvokeContext((arg) => { OnRevdRTProgress(progress); });
            }
        }

        protected void InvokeSendRTProgress(int currValue, int dataCount)
        {
            if (OnSendRTProgress != null)
            {
                float bfb = currValue / (float)dataCount * 100f;
                RTProgress progress = new RTProgress(bfb, RTState.Sending);
                InvokeContext((arg) => { OnSendRTProgress(progress); });
            }
        }

        /// <summary>
        /// 添加操作, 跟Send方法类似，区别在于AddOperation方法是将所有要发送的数据收集成一堆数据后，等待时间间隔进行发送。
        /// 而Send则是直接发送
        /// </summary>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        [Obsolete("此方法不再支持，请使用Send方法代替!", true)]
        public void AddOperation(string func, params object[] pars)
        {
            AddOperation(NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 添加操作, 跟Send方法类似，区别在于AddOperation方法是将所有要发送的数据收集成一堆数据后，等待时间间隔进行发送。
        /// 而Send则是直接发送
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        [Obsolete("此方法不再支持，请使用Send方法代替!", true)]
        public void AddOperation(byte cmd, string func, params object[] pars)
        {
            Operation opt = new Operation(cmd, OnSerializeRPC(new RPCModel(cmd, func, pars)));
            AddOperation(opt);
        }

        /// <summary>
        /// 添加操作, 跟Send方法类似，区别在于AddOperation方法是将所有要发送的数据收集成一堆数据后，等待时间间隔进行发送。
        /// 而Send则是直接发送
        /// </summary>
        /// <param name="opt"></param>
        public void AddOperation(Operation opt)
        {
            operations.Add(opt);
        }

        /// <summary>
        /// 添加操作, 跟Send方法类似，区别在于AddOperation方法是将所有要发送的数据收集成一堆数据后，等待时间间隔进行发送。
        /// 而Send则是直接发送
        /// </summary>
        /// <param name="opts"></param>
        public void AddOperations(List<Operation> opts)
        {
            foreach (Operation opt in opts)
                AddOperation(opt);
        }

        /// <summary>
        /// 后台线程发送心跳包
        /// </summary>
        protected virtual void HeartHandle()
        {
            while (openClient & currFrequency < 10)
            {
                try
                {
                    Thread.Sleep(HeartInterval);
                    if (RTOMode == RTOMode.Variable)
                        Ping();
                    heart++;
                    if (heart <= HeartLimit)
                        continue;
                    if (!Connected)
                    {
                        Reconnection(10);//尝试连接执行
                        continue;
                    }
                    if (heart < HeartLimit + 5)
                    {
                        Send(NetCmd.SendHeartbeat, new byte[0]);
                    }
                    else//连接中断事件执行
                    {
                        NetworkState = networkState = NetworkState.ConnectLost;
                        sendRTList.Clear();
                        revdRTList.Clear();
                        rtRPCModels = new QueueSafe<RPCModel>();
                        rPCModels = new QueueSafe<RPCModel>();
                        Connected = false;
                        NDebug.LogError("连接中断！");
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 测试服务器网络情况
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool PingServer(string ip)
        {
            Ping ping = new Ping();
            PingOptions options = new PingOptions { DontFragment = true };
            string data = "Test";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000;
            PingReply reply = ping.Send(ip, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
                return true;
            return false;
        }

        /// <summary>
        /// 断线重新连接
        /// </summary>
        /// <param name="maxFrequency">重连最大次数</param>
        protected virtual void Reconnection(int maxFrequency)
        {
            if (Client != null)
                Client.Close();
            bool done = false;
            ConnectResult(host, port, localPort, result =>
            {
                done = true;
                currFrequency++;
                if (result)
                {
                    currFrequency = 0;
                    heart = 0;
                    NetworkState = networkState = NetworkState.Reconnect;
                    sendRTList.Clear();
                    revdRTList.Clear();
                    rtRPCModels = new QueueSafe<RPCModel>();
                    rPCModels = new QueueSafe<RPCModel>();
                    sendReliableFrame = 0;
                    revdReliableFrame = 0;
                    NDebug.Log("重连成功...");
                }
                else if (currFrequency >= maxFrequency)//尝试maxFrequency次重连，如果失败则退出线程
                {
                    Close();
                    NDebug.LogError($"连接失败!请检查网络是否异常");
                }
                else
                {
                    NetworkState = networkState = NetworkState.TryToConnect;
                    NDebug.Log($"尝试重连:{currFrequency}...");
                }
            });
            while (!done) { }
        }

        /// <summary>
        /// 关闭连接,释放线程以及所占资源
        /// </summary>
        /// <param name="await">true:等待内部1秒结束所有线程再关闭? false:直接关闭</param>
        /// <param name="millisecondsTimeout">等待毫秒数</param>
        public virtual void Close(bool await = true, int millisecondsTimeout = 1000)
        {
            if (NetworkState != NetworkState.ConnectClosed)
                Client?.Send(new byte[] { 6, 0, 0, 0, 0, 0x2d, 74, NetCmd.QuitGame, 0, 0, 0, 0 });
            Connected = false;
            openClient = false;
            NetworkState = networkState = NetworkState.ConnectClosed;
            if (await) Thread.Sleep(millisecondsTimeout);//给update线程一秒的时间处理关闭事件
            AbortedThread();
            Client?.Close();
            Client = null;
            sendRTList.Clear();
            revdRTList.Clear();
            rtRPCModels = new QueueSafe<RPCModel>();
            rPCModels = new QueueSafe<RPCModel>();
            if (File.Exists(stackStreamName) & !string.IsNullOrEmpty(stackStreamName))
                File.Delete(stackStreamName);
            stackStreamName = "";
            if (Instance == this)
                Instance = null;
            sendReliableFrame = 0;
            revdReliableFrame = 0;
            Config.GlobalConfig.ThreadPoolRun = false;
            NDebug.Log("客户端关闭成功!");
        }

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        public virtual void Send(byte[] buffer)
        {
            Send(NetCmd.OtherCmd, buffer);
        }

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">发送字节数组缓冲区</param>
        public virtual void Send(byte cmd, byte[] buffer)
        {
            if (!Connected)
                return;
            if (rPCModels.Count >= ushort.MaxValue)
            {
                NDebug.LogError("数据缓存列表超出限制!");
                return;
            }
            if (buffer.Length > 65507)
            {
                NDebug.LogError("数据太大，请分段发送!");
                return;
            }
            rPCModels.Enqueue(new RPCModel(cmd, buffer) { bigData = buffer.Length > short.MaxValue });
        }

        public virtual void Send(byte cmd, object obj)
        {
            var buffer = BufferPool.Take();
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                stream.SetLength(0);
                ProtoBuf.Serializer.Serialize(stream, obj);
                Send(cmd, stream.ToArray());
            }
            BufferPool.Push(buffer);
        }

        /// <summary>
        /// 远程调用函数, 调用服务器的方法名为func的函数
        /// </summary>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public virtual void Send(string func, params object[] pars)
        {
            Send(NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 远程调用函数, 调用服务器的方法名为func的函数
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public virtual void Send(byte cmd, string func, params object[] pars)
        {
            if (!Connected)
                return;
            if (rPCModels.Count >= ushort.MaxValue)
            {
                NDebug.LogError("数据缓存列表超出限制!");
                return;
            }
            rPCModels.Enqueue(new RPCModel(cmd, func, pars));
        }

        public virtual void Send(ushort methodMask, params object[] pars)
        {
            Send(NetCmd.CallRpc, methodMask, pars);
        }

        public virtual void Send(byte cmd, ushort methodMask, params object[] pars)
        {
            if (!Connected)
                return;
            if (rPCModels.Count >= ushort.MaxValue)
            {
                NDebug.LogError("数据缓存列表超出限制!");
                return;
            }
            rPCModels.Enqueue(new RPCModel(cmd, methodMask, pars));
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="pars">远程参数</param>
        public virtual void Send(string func, string funcCB, Delegate callback, params object[] pars)
        {
            Send(NetCmd.CallRpc, func, funcCB, callback, 10000, null, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="pars">远程参数</param>
        public virtual void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars)
        {
            Send(NetCmd.CallRpc, func, funcCB, callback, millisecondsDelay, null, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="pars">远程参数</param>
        public virtual void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            Send(NetCmd.CallRpc, func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="pars">远程参数</param>
        public virtual void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            Send(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, Context, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="context">调用上下文线程对象</param>
        /// <param name="pars">远程参数</param>
        public virtual void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            if (!Connected)
                return;
            if (rtRPCModels.Count >= ushort.MaxValue)
            {
                NDebug.LogError("数据缓存列表超出限制!");
                return;
            }
            rPCModels.Enqueue(new RPCModel(cmd, func, pars, true, true));
            if (!rpcTasks.TryGetValue(funcCB, out RPCModelTask model))
                rpcTasks.Add(funcCB, model = new RPCModelTask());
            Task.Run(() =>
            {
                DateTime time = DateTime.Now.AddMilliseconds(millisecondsDelay);
                while (DateTime.Now < time)
                {
                    Thread.Sleep(1);
                    if (model.IsCompleted)
                    {
                        if (context != null)
                            context.Post((state) => { callback.DynamicInvoke(model.model.pars); }, null);
                        else
                            callback.DynamicInvoke(model.model.pars);
                        return;
                    }
                }
                if (context != null)
                    context.Post((state) => { outTimeAct?.Invoke(); }, null);
                else
                    outTimeAct?.Invoke();
            });
        }

        private readonly MyDictionary<string, RPCModelTask> rpcTasks = new MyDictionary<string, RPCModelTask>();

        /// <summary>
        /// 远程同步调用
        /// </summary>
        /// <param name="func"></param>
        /// <param name="callbackFunc">服务器返回后调用的函数名</param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public async Task<RPCModel> Call(string func, string callbackFunc, params object[] pars)
        {
            return await Call(NetCmd.CallRpc, func, callbackFunc, 60000, pars);
        }

        /// <summary>
        /// 远程同步调用
        /// </summary>
        /// <param name="func"></param>
        /// <param name="callbackFunc">服务器返回后调用的函数名</param>
        /// <param name="millisecondsDelay">需要等待的时间,毫秒单位</param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public async Task<RPCModel> Call(string func, string callbackFunc, int millisecondsDelay, params object[] pars)
        {
            return await Call(NetCmd.CallRpc, func, callbackFunc, millisecondsDelay, pars);
        }

        /// <summary>
        /// 远程同步调用
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="func"></param>
        /// <param name="callbackFunc">服务器返回后调用的函数名</param>
        /// <param name="millisecondsDelay">需要等待的时间,毫秒单位</param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public async Task<RPCModel> Call(byte cmd, string func, string callbackFunc, int millisecondsDelay, params object[] pars)
        {
            SendRT(cmd, func, pars);
            if (!rpcTasks.TryGetValue(callbackFunc, out RPCModelTask model))
                rpcTasks.Add(callbackFunc, model = new RPCModelTask());
            CancellationTokenSource cts = new CancellationTokenSource();
            Task<RPCModel> task = Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    Thread.Sleep(1);
                    if (model.IsCompleted)
                    {
                        return model.model;
                    }
                }
                return default;
            }, cts.Token);
            task.Wait(millisecondsDelay, cts.Token);
            return await task;
        }

        /// <summary>
        /// 发送网络可靠传输数据, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        public virtual void SendRT(string func, params object[] pars)
        {
            SendRT(NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        public virtual void SendRT(byte cmd, string func, params object[] pars)
        {
            if (!Connected)
                return;
            if (rtRPCModels.Count >= ushort.MaxValue)
            {
                NDebug.LogError("数据缓存列表超出限制!");
                return;
            }
            rtRPCModels.Enqueue(new RPCModel(cmd, func, pars, true, true));
        }

        public virtual void SendRT(ushort methodMask, params object[] pars)
        {
            SendRT(NetCmd.CallRpc, methodMask, pars);
        }

        public virtual void SendRT(byte cmd, ushort methodMask, params object[] pars)
        {
            if (!Connected)
                return;
            if (rtRPCModels.Count >= ushort.MaxValue)
            {
                NDebug.LogError("数据缓存列表超出限制!");
                return;
            }
            rtRPCModels.Enqueue(new RPCModel(cmd, methodMask, pars));
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="pars">远程参数</param>
        public virtual void SendRT(string func, string funcCB, Delegate callback, params object[] pars)
        {
            SendRT(NetCmd.CallRpc, func, funcCB, callback, 10000, null, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="pars">远程参数</param>
        public virtual void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars)
        {
            SendRT(NetCmd.CallRpc, func, funcCB, callback, millisecondsDelay, null, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="pars">远程参数</param>
        public virtual void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            SendRT(NetCmd.CallRpc, func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="pars">远程参数</param>
        public virtual void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            SendRT(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, Context, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="context">调用上下文线程</param>
        /// <param name="pars">远程参数</param>
        public virtual void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            SendRT(new RPCModel(cmd, func, pars, true, true), funcCB, callback, millisecondsDelay, outTimeAct, context, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="pars">远程参数</param>
        public virtual void SendRT(ushort func, ushort funcCB, Delegate callback, params object[] pars)
        {
            SendRT(NetCmd.CallRpc, func, funcCB, callback, 10000, null, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="pars">远程参数</param>
        public virtual void SendRT(ushort func, ushort funcCB, Delegate callback, int millisecondsDelay, params object[] pars)
        {
            SendRT(NetCmd.CallRpc, func, funcCB, callback, millisecondsDelay, null, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="pars">远程参数</param>
        public virtual void SendRT(ushort func, ushort funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            SendRT(NetCmd.CallRpc, func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        /// <summary>
        /// 发送请求, 并且监听服务端的回调请求, 服务器回调请求要对应上发送时的回调匿名, 异步回调, 并且在millisecondsDelay时间内要响应, 否则调用outTimeAct
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <param name="func">服务器函数名</param>
        /// <param name="funcCB">服务器回调函数名</param>
        /// <param name="callback">回调接收委托</param>
        /// <param name="millisecondsDelay">异步时间</param>
        /// <param name="outTimeAct">异步超时调用</param>
        /// <param name="pars">远程参数</param>
        public virtual void SendRT(byte cmd, ushort func, ushort funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            SendRT(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, Context, pars);
        }

        public virtual void SendRT(byte cmd, ushort func, ushort funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            if (!RpcMaskDic.TryGetValue(funcCB, out string funcCB1))
            {
                NDebug.LogError($"回调方法没有定义! 请在回调方法添加[Rpc(mask = {funcCB})]");
                return;
            }
            SendRT(new RPCModel(cmd, string.Empty, pars, true, true, func), funcCB1, callback, millisecondsDelay, outTimeAct, context, pars);
        }

        private void SendRT(RPCModel model, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            if (!Connected)
                return;
            if (rtRPCModels.Count >= ushort.MaxValue)
            {
                NDebug.LogError("数据缓存列表超出限制!");
                return;
            }
            rtRPCModels.Enqueue(model);
            if (!rpcTasks.TryGetValue(funcCB, out RPCModelTask model1))
                rpcTasks.Add(funcCB, model1 = new RPCModelTask());
            Task.Run(() =>
            {
                DateTime time = DateTime.Now.AddMilliseconds(millisecondsDelay);
                while (DateTime.Now < time)
                {
                    Thread.Sleep(1);
                    if (model1.IsCompleted)
                    {
                        if (context != null)
                            context.Post((state) => { callback.DynamicInvoke(model1.model.pars); }, null);
                        else
                            callback.DynamicInvoke(model1.model.pars);
                        return;
                    }
                }
                if (context != null)
                    context.Post((state) => { outTimeAct?.Invoke(); }, null);
                else
                    outTimeAct?.Invoke();
            });
        }

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void SendRT(byte[] buffer)
        {
            SendRT(NetCmd.OtherCmd, buffer);
        }

        public virtual void SendRT(byte cmd, object obj)
        {
            if (cmd < 30)
                throw new Exception("自定义协议(命令)不能使用内核协议(命令)进行发送!");
            using (MemoryStream stream = new MemoryStream(1024))
            {
                ProtoBuf.Serializer.Serialize(stream, obj);
                SendRT(cmd, stream.ToArray());
            }
        }

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 调用此方法通常情况下是一定把数据发送成功为止, 
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer"></param>
        public virtual void SendRT(byte cmd, byte[] buffer)
        {
            if (!Connected)
                return;
            if (rtRPCModels.Count >= ushort.MaxValue)
            {
                NDebug.LogError("数据缓存列表超出限制!");
                return;
            }
            if (buffer.Length / MTU > ushort.MaxValue)
            {
                NDebug.LogError("数据太大，请分段发送!");
                return;
            }
            rtRPCModels.Enqueue(new RPCModel(cmd, buffer, false, false) { bigData = buffer.Length > short.MaxValue });
        }

        /// <summary>
        /// 远程过程调用 同Send方法
        /// </summary>
        /// <param name="func">Call名</param>
        /// <param name="pars">Call函数</param>
        public virtual void CallRpc(string func, params object[] pars) => Send(func, pars);

        /// <summary>
        /// 远程过程调用 同Send方法
        /// </summary>
        /// <param name="cmd">网络命令，请看NetCmd类定义</param>
        /// <param name="func">Call名</param>
        /// <param name="pars">Call函数</param>
        public virtual void CallRpc(byte cmd, string func, params object[] pars) => Send(cmd, func, pars);

        /// <summary>
        /// 网络请求 同Send方法
        /// </summary>
        /// <param name="func">Call名</param>
        /// <param name="pars">Call函数</param>
        public virtual void Request(string func, params object[] pars) => Send(func, pars);

        /// <summary>
        /// 网络请求 同Send方法
        /// </summary>
        /// <param name="cmd">网络命令，请看NetCmd类定义</param>
        /// <param name="func">Call名</param>
        /// <param name="pars">Call函数</param>
        public virtual void Request(byte cmd, string func, params object[] pars) => Send(cmd, func, pars);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        protected void InvokeOnOperationSync(OperationList list)
        {
            if (Context != null & !OperationSyncInThread)
                Context.Post(OperationSyncCallback, list);
            else
                OnOperationSync?.Invoke(list);
        }

        private void OperationSyncCallback(object state)
        {
            OnOperationSync?.Invoke((OperationList)state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        protected void InvokeOnRevdBufferHandle(RPCModel model)
        {
            if (Context != null)
                Context.Post(OnRevdBufferCallback, model);
            else
                OnRevdBufferHandle?.Invoke(model);
        }

        private void OnRevdBufferCallback(object state)
        {
            OnRevdBufferHandle?.Invoke((RPCModel)state);
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
        public void Ping()
        {
            long timelong = DateTime.Now.Ticks;
            Send(NetCmd.Ping, BitConverter.GetBytes(timelong));
        }

        /// <summary>
        /// ping测试网络延迟, 此方法帮你监听<see cref="OnPingCallback"/>事件, 如果不使用的时候必须保证能移除委托, 建议不要用框名函数, 那样会无法移除委托
        /// </summary>
        /// <param name="callback"></param>
        public void Ping(Action<double> callback)
        {
            long timelong = DateTime.Now.Ticks;
            Send(NetCmd.Ping, BitConverter.GetBytes(timelong));
            OnPingCallback += callback;
        }

        /// <summary>
        /// 添加适配器
        /// </summary>
        /// <param name="adapter"></param>
        public void AddAdapter(IAdapter adapter)
        {
            if (adapter is ISerializeAdapter ser)
                AddAdapter(AdapterType.Serialize, ser);
            else if (adapter is IRPCAdapter rpc)
                AddAdapter(AdapterType.RPC, rpc);
            else if (adapter is INetworkEvtAdapter evt)
                AddAdapter(AdapterType.NetworkEvt, evt);
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
                    var rpc = (IRPCAdapter)adapter;
                    OnAddRpcHandle = rpc.AddRpcHandle;
                    OnRPCExecute = rpc.OnRpcExecute;
                    OnRemoveRpc = rpc.RemoveRpc;
                    OnCheckRpcUpdate = rpc.CheckRpcUpdate;
                    break;
                case AdapterType.NetworkEvt:
                    BindNetworkHandle((INetworkHandle)adapter);
                    break;
            }
        }

        /// <summary>
        /// 添加网络状态事件处理
        /// </summary>
        /// <param name="listen">要监听的网络状态</param>
        /// <param name="action">监听网络状态的回调方法</param>
        public void AddStateHandler(NetworkState listen, Action action)
        {
            switch (listen)
            {
                case NetworkState.Connected:
                    OnConnectedHandle += action;
                    break;
                case NetworkState.ConnectFailed:
                    OnConnectFailedHandle += action;
                    break;
                case NetworkState.ConnectLost:
                    OnConnectLostHandle += action;
                    break;
                case NetworkState.Reconnect:
                    OnReconnectHandle += action;
                    break;
                case NetworkState.BlockConnection:
                    OnBlockConnectionHandle += action;
                    break;
                case NetworkState.ExceededNumber:
                    OnExceededNumberHandle += action;
                    break;
                case NetworkState.ConnectClosed:
                    OnCloseConnectHandle += action;
                    break;
                case NetworkState.Disconnect:
                    OnDisconnectHandle += action;
                    break;
                case NetworkState.TryToConnect:
                    OnTryToConnectHandle += action;
                    break;
            }
        }

        /// <summary>
        /// 字段,属性同步处理线程
        /// </summary>
        protected virtual void VarSyncHandler()
        {
            while (Connected)
            {
                try
                {
                    Thread.Sleep(1);
                    Segment segment = null;
                    var entries = varSyncInfos.entries;
                    for (int i = 0; i < varSyncInfos.count; i++)
                    {
                        if (entries[i].hashCode >= 0)
                        {
                            var varSync = entries[i].value;
                            if (varSync == null)
                                continue;
                            if (varSync.passive)
                                continue;
                            var value = varSync.GetValue();
                            if (value == null)
                                continue;
                            if (!value.Equals(varSync.value))
                            {
                                if (segment == null)
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
                        SendRT(NetCmd.VarSync, buffer);
                    }
                }
                catch (Exception e)
                {
                    NDebug.LogError(e);
                }
            }
        }

        /// <summary>
        /// 发送文件, 服务器可以通过重写<see cref="Server.ServerBase{Player, Scene}.OnReceiveFile"/>方法来接收 或 使用事件<see cref="Server.ServerBase{Player, Scene}.OnReceiveFileHandle"/>来监听并处理
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bufferSize">每次发送数据大小</param>
        /// <returns></returns>
        public bool SendFile(string filePath, int bufferSize = 50000)
        {
            var path1 = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(path1))
            {
                NDebug.LogError("文件不存在! 或者文件路径字符串编码错误! 提示:可以使用Notepad++查看, 编码是ANSI,不是UTF8");
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
            ftpDic.Add(fileData.ID, fileData);
            SendFile(fileData.ID, fileData);
            return true;
        }

        private void SendFile(int key, FileData fileData)
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
            SendRT(NetCmd.SendFile, segment1.ToArray(true));
            if (complete)
            {
                if (OnSendFileProgress != null)
                    InvokeContext((arg) => { OnSendFileProgress(new RTProgress(fileData.fileName, fileStream.Position / (float)fileStream.Length * 100f, RTState.Complete)); });
                ftpDic.Remove(key);
                fileData.fileStream.Close();
            }
            else 
            {
                if (OnSendFileProgress != null)
                    InvokeContext((arg) => { OnSendFileProgress(new RTProgress(fileData.fileName, fileStream.Position / (float)fileStream.Length * 100f, RTState.Sending)); });
            }
        }
    }
}