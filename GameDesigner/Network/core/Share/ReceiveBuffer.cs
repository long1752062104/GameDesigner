namespace Net.Share
{
    using Net.Server;
    using global::System.Net;

    /// <summary>
    /// 服务器接收数据缓存
    /// </summary>
    public struct ReceiveBuffer
    {
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        public byte[] buffer;
        /// <summary>
        /// 数据长度
        /// </summary>
        public int count;
        /// <summary>
        /// UDP终端
        /// </summary>
        public EndPoint remotePoint;
        /// <summary>
        /// TCP客户端
        /// </summary>
        public NetPlayer client;

        /// <summary>
        /// 构造接收数据缓存器
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        public ReceiveBuffer(byte[] buffer, int count)
        {
            this.buffer = buffer;
            this.count = count;
            remotePoint = null;
            client = null;
        }

        /// <summary>
        /// 构造接收数据缓存器
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        /// <param name="remotePoint"></param>
        public ReceiveBuffer(byte[] buffer, int count, EndPoint remotePoint)
        {
            this.buffer = buffer;
            this.count = count;
            this.remotePoint = remotePoint;
            client = null;
        }

        /// <summary>
        /// 构造接收数据缓存器
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        /// <param name="client"></param>
        public ReceiveBuffer(byte[] buffer, int count, NetPlayer client)
        {
            this.buffer = buffer;
            this.count = count;
            this.client = client;
            remotePoint = null;
        }
    }
}
