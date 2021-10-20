namespace Net.Share
{
    using Net.Server;
    using global::System;

    /// <summary>
    /// 网络服务器事件处理
    /// </summary>
    public interface IServerEventHandle<Player> where Player : NetPlayer
    {
        /// <summary>
        /// 开始运行服务器事件
        /// </summary>
        Action OnStartingHandle { get; set; }
        /// <summary>
        /// 服务器启动成功事件
        /// </summary>
        Action OnStartupCompletedHandle { get; set; }
        /// <summary>
        /// 当前有客户端连接触发事件
        /// </summary>
        Action<Player> OnHasConnectHandle { get; set; }
        /// <summary>
        /// 当添加客户端到所有在线的玩家集合中触发的事件
        /// </summary>
        Action<Player> OnAddClientHandle { get; set; }
        /// <summary>
        /// 当接收到网络数据处理事件
        /// </summary>
        RevdBufferHandle<Player> OnRevdBufferHandle { get; set; }
        /// <summary>
        /// 当移除客户端时触发事件
        /// </summary>
        Action<Player> OnRemoveClientHandle { get; set; }
        /// <summary>
        /// 当统计网络流量时触发
        /// </summary>
        NetworkDataTraffic OnNetworkDataTraffic { get; set; }
        /// <summary>
        /// 输出日志
        /// </summary>
        Action<string> Log { get; set; }
    }
}