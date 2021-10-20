namespace Net.Share
{
    /// <summary>
    /// 可靠传输状态
    /// </summary>
    public enum RTState
    {
        /// <summary>
        /// 发送中
        /// </summary>
        Sending,
        /// <summary>
        /// 下载中
        /// </summary>
        Download,
        /// <summary>
        /// 发送失败
        /// </summary>
        FailSend,
        /// <summary>
        /// 发送完成
        /// </summary>
        Complete,
        /// <summary>
        /// 尝试重传
        /// </summary>
        Retransmission
    }

    /// <summary>
    /// 可靠文件发送进度委托
    /// </summary>
    /// <param name="progress">当前进度</param>
    /// <param name="state">当前状态</param>
    public delegate void SendRTProgress(int progress, RTState state);

    /// <summary>
    /// 可靠传输进度值
    /// </summary>
    public struct RTProgress
    {
        /// <summary>
        /// 进度名称
        /// </summary>
        public string name;
        /// <summary>
        /// 进度值
        /// </summary>
        public float progress;
        /// <summary>
        /// 当前状态
        /// </summary>
        public RTState state;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="state"></param>
        public RTProgress(float progress, RTState state) : this()
        {
            this.progress = progress;
            this.state = state;
        }

        public RTProgress(string name, float progress, RTState state) : this(progress, state)
        {
            this.name = name;
        }
    }
}