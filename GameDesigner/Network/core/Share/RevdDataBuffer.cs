using Net.System;

namespace Net.Share
{
    /// <summary>
    /// 一级数据接收缓存区
    /// </summary>
    public struct RevdDataBuffer
    {
        /// <summary>
        /// 远程端口
        /// </summary>
        public object client;
        /// <summary>
        /// 缓存区
        /// </summary>
        public Segment buffer;
        /// <summary>
        /// 数据索引
        /// </summary>
        public int index;
        /// <summary>
        /// 数据长度
        /// </summary>
        public int count;
        /// <summary>
        /// 数据协议
        /// </summary>
        public bool tcp_udp;

        public RevdDataBuffer(object client, Segment buffer, int index, int count, bool tcp_udp)
        {
            this.client = client;
            this.buffer = buffer;
            this.index = index;
            this.count = count;
            this.tcp_udp = tcp_udp;
        }

        public override string ToString()
        {
            return $"{client} - buffer:{(buffer == null ? 0 : buffer.Length)} - reliable:{tcp_udp}";
        }
    }

    /// <summary>
    /// 一级数据发送缓存区
    /// </summary>
    public struct SendDataBuffer
    {
        /// <summary>
        /// 客户端对象
        /// </summary>
        public object client;
        /// <summary>
        /// 缓存区
        /// </summary>
        public byte[] buffer;

        public bool reliable;

        public SendDataBuffer(object client, byte[] buffer)
        {
            this.client = client;
            this.buffer = buffer;
            reliable = false;
        }

        public SendDataBuffer(object client, byte[] buffer, bool reliable)
        {
            this.client = client;
            this.buffer = buffer;
            this.reliable = reliable;
        }

        public override string ToString()
        {
            global::System.Reflection.FieldInfo[] fields = typeof(NetCmd).GetFields(global::System.Reflection.BindingFlags.Static | global::System.Reflection.BindingFlags.Public);
            return $"{client} - buffer:{buffer.Length} - reliable:{reliable} - cmd:{fields[buffer[7]].Name}";
        }
    }
}
