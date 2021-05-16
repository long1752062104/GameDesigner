namespace Net.Share
{
    /// <summary>
    /// 超时重传模式
    /// </summary>
    public enum RTOMode
    {
        /// <summary>
        /// 可变的重传时间, 以ping指令来自动设置重传时间, 每秒ping一次, 并设置rto时间, 你还可以监听服务器,客户端的OnPingCallback事件来查看网络延迟
        /// </summary>
        Variable,
        /// <summary>
        /// 固定重传时间, 以ServerBase的RTO时间重传
        /// </summary>
        Fixed
    }
}
