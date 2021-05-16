namespace Net.Share
{
    /// <summary>
    /// 网络调式日志信息处理接口
    /// </summary>
    public interface IDebugHandle
    {
        /// <summary>
        /// 当输出信息
        /// </summary>
        /// <param name="msg"></param>
        void Log(string msg);
        /// <summary>
        /// 当远程过程调用函数
        /// </summary>
        /// <param name="msg"></param>
        void LogRpc(string msg);
    }
}
