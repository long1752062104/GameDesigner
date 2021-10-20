namespace Net.Share
{
    using global::System;

    /// <summary>
    /// 网络客户端接口处理 2019.7.9
    /// </summary>
    public interface INetClient
    {
        /// <summary>
        /// 当连接服务器成功事件
        /// </summary>
        event Action OnConnectedHandle;
        /// <summary>
        /// 当连接失败事件
        /// </summary>
        event Action OnConnectFailedHandle;
        /// <summary>
        /// 当尝试连接服务器事件
        /// </summary>
        event Action OnTryToConnectHandle;
        /// <summary>
        /// 当连接中断 (异常) 事件
        /// </summary>
        event Action OnConnectLostHandle;
        /// <summary>
        /// 当断开连接事件
        /// </summary>
        event Action OnDisconnectHandle;
        /// <summary>
        /// 当接收到网络数据处理事件
        /// </summary>
        event Action<RPCModel> OnRevdBufferHandle;
        /// <summary>
        /// 当断线重连成功触发事件
        /// </summary>
        event Action OnReconnectHandle;
        /// <summary>
        /// 当关闭连接事件
        /// </summary>
        event Action OnCloseConnectHandle;
        /// <summary>
        /// 当统计网络流量时触发
        /// </summary>
        event NetworkDataTraffic OnNetworkDataTraffic;
    }
}