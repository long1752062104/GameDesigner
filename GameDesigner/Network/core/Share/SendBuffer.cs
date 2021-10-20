namespace Net.Share
{
    using global::System;

    /// <summary>
    /// 发送数据缓冲区
    /// </summary>
    [Serializable]
    public class SendBuffer
    {
        /// <summary>
        /// 网络命令0-5已经被插件系统占用，可用6-255为自定义命令
        /// </summary>
        public byte cmd;
        /// <summary>
        /// RPC函数名称
        /// </summary>
        public string func;
        /// <summary>
        /// RPC参数
        /// </summary>
        public object[] pars;
        /// <summary>
        /// 字节数组缓冲区
        /// </summary>
        public byte[] buffer;
        /// <summary>
        /// 使用插件内核数据 还是 自定义数据
        /// </summary>
        public bool kernel = true;

        /// <summary>
        /// 构造发送缓冲区 使用插件内核发送数据
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="fun">网络RPC函数名称</param>
        /// <param name="pars">网络RPC参数</param>
        public SendBuffer(byte cmd, string fun, object[] pars)
        {
            kernel = true;
            this.cmd = cmd;
            func = fun;
            this.pars = pars;
        }

        /// <summary>
        /// 构造发送缓冲区 使用自定义数据发送
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">缓冲区</param>
        public SendBuffer(byte cmd, byte[] buffer)
        {
            kernel = false;
            if (cmd < 5)//核心Rpc命令只有5个
                cmd = NetCmd.OtherCmd;
            this.cmd = cmd;
            this.buffer = buffer;
        }
    }
}