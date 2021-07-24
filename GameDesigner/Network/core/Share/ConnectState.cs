namespace Net.Share
{
    /// <summary>
    /// 网络状态
    /// </summary>
	public enum NetworkState : byte
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None,
        /// <summary>
        /// 连接成功
        /// </summary>
        Connected,
        /// <summary>
        /// 连接失败
        /// </summary>
        ConnectFailed,
        /// <summary>
        /// 尝试连接
        /// </summary>
        TryToConnect,
        /// <summary>
        /// 断开连接
        /// </summary>
        Disconnect,
        /// <summary>
        /// 连接中断 (连接异常)
        /// </summary>
        ConnectLost,
        /// <summary>
        /// 连接已被关闭
        /// </summary>
        ConnectClosed,
        /// <summary>
        /// 正在连接服务器中...
        /// </summary>
        Connection,
        /// <summary>
        /// 断线重连成功
        /// </summary>
        Reconnect,
        /// <summary>
        /// 服务器连接人数溢出, 服务器忽略当前客户端的所有Rpc请求
        /// </summary>
        ExceededNumber,
        /// <summary>
        /// 服务器爆满, 服务器忽略当前客户端的所有Rpc请求
        /// </summary>
        BlockConnection,
    }
}