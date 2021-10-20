namespace Net.Share// 网络共用(通用)命名空间
{
    /// <summary>
    /// 网络命令基类 - 可继承此类定义自己的网络命令 19.7.16
    /// </summary>
    public abstract class NetCmd
    {
        /// <summary>
        /// 面向实体类型调用远程函数
        /// 使用此命令即可在NetPlayer的派生类中定义rpc函数进行调用
        /// </summary>
        public const byte EntityRpc = 0;
        /// <summary>
        /// 如果是客户端调用则在服务器执行 如果是服务器调用则在客户端执行.
        /// 在服务器端,如果出现多线程抢夺资源调用Client错误时，可使用SafeCall命令来执行
        /// </summary>
        public const byte CallRpc = 1;
        /// <summary>
        /// 安全调用服务器函数,当多线程并行时会有1%的几率发生线程抢夺资源，
        /// 如果在RPC函数内部调用client的时候是其他客户端的client对象。出现这种情况时建议使用此命令，
        /// 否则可以使用CallRpc命令，
        /// 使用此命令时,函数第一个参数将会嵌入NetPlayer参数
        /// </summary>
        public const byte SafeCall = 2;
        /// <summary>
        /// (自身转发)服务器只转发给发送方客户端
        /// </summary>
        public const byte Local = 3;
        /// <summary>
        /// (场景转发)服务器负责转发给在同一房间或场景内的玩家
        /// </summary>
        public const byte Scene = 4;
        /// <summary>
        /// (场景转发,可靠指令)服务器负责转发给在同一房间或场景内的玩家
        /// </summary>
        public const byte SceneRT = 5;
        /// <summary>
        /// (公告指令)服务器负责转发给所有在线的玩家
        /// </summary>
        public const byte Notice = 6;
        /// <summary>
        /// (公告指令,可靠传输)服务器负责转发给所有在线的玩家
        /// </summary>
        public const byte NoticeRT = 7;
        /// <summary>
        /// 发送心跳包命令, 内部命令
        /// </summary>
        public const byte SendHeartbeat = 8;
        /// <summary>
        /// 回调心跳包命令, 内部命令
        /// </summary>
        public const byte RevdHeartbeat = 9;
        /// <summary>
        /// 多线程远程过程调用函数 (RPC)
        /// </summary>
        public const byte ThreadRpc = 10;
        /// <summary>
        /// 请求服务器移除此客户端
        /// </summary>
        public const byte QuitGame = 11;
        /// <summary>
        /// 其他命令或用户自定义命令
        /// </summary>
        public const byte OtherCmd = 12;
        /// <summary>
        /// 服务器连接人数溢出, 新的客户端将不允许连接服务器, 可设置服务器的LineUp值调整
        /// </summary>
        public const byte ExceededNumber = 13;
        /// <summary>
        /// 服务器爆满, 阻止客户端连接命令, 仅限服务器回调给客户端使用的命令, 客户端可监听OnBlockConnection事件处理， 内部命令
        /// </summary>
        public const byte BlockConnection = 14;
        /// <summary>
        /// 可靠传输接收指令. 内部命令
        /// </summary>
        public const byte ReliableTransport = 15;
        /// <summary>
        /// 可靠传输数据确认回调, 内部命令
        /// </summary>
        public const byte ReliableCallback = 16;
        /// <summary>
        /// 当客户端连接主服务器(网关服)时, 主服务器检测分区服务器在线人数如果处于爆满状态, 
        /// 服务器发送切换端口让客户端连接新的服务器IP和端口. 内部命令
        /// </summary>
        public const byte SwitchPort = 17;
        /// <summary>
        /// 标记客户端唯一标识, 内部命令
        /// </summary>
        public const byte Identify = 18;
        /// <summary>
        /// 操作同步，服务器使用NetScene.AddOperation方法，客户端UdpClient.AddOperation方法。 内部指令
        /// </summary>
        public const byte OperationSync = 19;
        /// <summary>
        /// 局域网寻找主机命令, 内部使用
        /// </summary>
        public const byte Broadcast = 20;
        /// <summary>
        /// 连接指令 (内部)
        /// </summary>
        public const byte Connect = 21;
        /// <summary>
        /// 可靠传输被清洗, 原因是接收缓存流设置过小 内部指令
        /// </summary>
        public const byte ReliableCallbackClear = 22;
        /// <summary>
        /// 自身转发, 可靠传输
        /// </summary>
        public const byte LocalRT = 23;
        /// <summary>
        /// ping测试网络延迟量
        /// </summary>
        public const byte Ping = 24;
        /// <summary>
        /// ping回调 内部指令
        /// </summary>
        public const byte PingCallback = 25;
        /// <summary>
        /// 可靠传输取单帧 内部指令
        /// </summary>
        public const byte TakeFrame = 26;
        /// <summary>
        /// 可靠传输取帧列表 内部指令
        /// </summary>
        public const byte TakeFrameList = 27;

        public const byte P2P = 28;
        /// <summary>
        /// 字段或属性同步指令 内部指令
        /// </summary>
        public const byte VarSync = 29;
        /// <summary>
        /// 发送文件
        /// </summary>
        public const byte SendFile = 30;
        /// <summary>
        /// 下载文件
        /// </summary>
        public const byte Download = 31;
    }
}