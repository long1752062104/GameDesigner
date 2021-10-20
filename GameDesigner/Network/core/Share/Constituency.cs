namespace Net.Share
{
    /// <summary>
    /// 选区网络传输对象
    /// </summary>
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    [global::System.Serializable]
    public class Constituency
    {
        /// <summary>
        /// 服务器区名
        /// </summary>
        public string name;
        /// <summary>
        /// 当前在线人数
        /// </summary>
        public int online;
        /// <summary>
        /// 当前服务器区域状态, 顺畅，拥挤，爆满
        /// </summary>
        public string status;
        /// <summary>
        /// 服务器区ip
        /// </summary>
        public string ip;
        /// <summary>
        /// 服务器区端口
        /// </summary>
        public ushort port;

        public Constituency()
        {
        }

        public Constituency(string name, int online, string status)
        {
            this.name = name;
            this.online = online;
            this.status = status;
        }

        public Constituency(string name, string ip, ushort port)
        {
            this.name = name;
            this.ip = ip;
            this.port = port;
        }
    }
}
