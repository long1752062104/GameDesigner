namespace Net.Share
{
    /// <summary>
    /// 帧同步操作
    /// </summary>
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public struct Operation
    {
        /// <summary>
        /// 操作指令
        /// </summary>
        public byte cmd;
        /// <summary>
        /// 其他指令
        /// </summary>
        public byte cmd1, cmd2;
        /// <summary>
        /// 玩家名称
        /// </summary>
        public string name;
        /// <summary>
        /// 玩家位置信息
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// 玩家旋转信息
        /// </summary>
        public Quaternion rotation;
        /// <summary>
        /// 玩家输入方向
        /// </summary>
        public Vector3 direction;
        /// <summary>
        /// 索引
        /// </summary>
        public int index;
        /// <summary>
        /// 其他索引
        /// </summary>
        public int index1, index2;
        /// <summary>
        /// 数据数组, 备用
        /// </summary>
        public byte[] buffer;
        /// <summary>
        /// 备用字符串
        /// </summary>
        public string name1, name2;

        /// <summary>
        /// 玩家操作指令
        /// </summary>
        /// <param name="cmd"></param>
        public Operation(byte cmd) : this()
        {
            this.cmd = cmd;
        }

        /// <summary>
        /// 玩家操作指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="buffer"></param>
        public Operation(byte cmd, byte[] buffer) : this()
        {
            this.cmd = cmd;
            this.buffer = buffer;
        }

        public Operation(byte cmd, int uid) : this()
        {
            this.cmd = cmd;
            index = uid;
        }

        /// <summary>
        /// 玩家操作指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        public Operation(byte cmd, string name) : this()
        {
            this.cmd = cmd;
            this.name = name;
        }

        /// <summary>
        /// 玩家操作指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        /// <param name="name1"></param>
        public Operation(byte cmd, string name, string name1) : this()
        {
            this.cmd = cmd;
            this.name = name;
            this.name1 = name1;
        }

        /// <summary>
        /// 玩家输入方向指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        /// <param name="direction"></param>
        public Operation(byte cmd, string name, Vector3 direction) : this(cmd)
        {
            this.name = name;
            this.direction = direction;
        }

        /// <summary>
        /// 玩家输入方向指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        /// <param name="direction"></param>
        public Operation(byte cmd, int uid, Vector3 direction) : this(cmd)
        {
            index = uid;
            this.direction = direction;
        }

        /// <summary>
        /// 玩家其他操作指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        /// <param name="index"></param>
        public Operation(byte cmd, string name, int index) : this(cmd)
        {
            this.name = name;
            this.index = index;
        }

        /// <summary>
        /// 玩家位置同步操作指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Operation(byte cmd, string name, Vector3 position, Quaternion rotation) : this(cmd)
        {
            this.name = name;
            this.position = position;
            this.rotation = rotation;
        }

        /// <summary>
        /// 玩家位置同步操作指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="identity"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Operation(byte cmd, int identity, Vector3 position, Quaternion rotation) : this(cmd)
        {
            this.index = identity;
            this.position = position;
            this.rotation = rotation;
        }

        /// <summary>
        /// 玩家位置同步操作指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        /// <param name="direction"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Operation(byte cmd, string name, Vector3 direction, Vector3 position, Quaternion rotation) : this(cmd)
        {
            this.name = name;
            this.direction = direction;
            this.position = position;
            this.rotation = rotation;
        }

        /// <summary>
        /// 玩家位置同步操作指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="identity"></param>
        /// <param name="direction"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Operation(byte cmd, int identity, Vector3 direction, Vector3 position, Quaternion rotation) : this(cmd)
        {
            index = identity;
            this.direction = direction;
            this.position = position;
            this.rotation = rotation;
        }

        /// <summary>
        /// 玩家位置同步操作指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="name"></param>
        /// <param name="direction_is_localScale"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Operation(byte cmd, string name, Vector3 position, Quaternion rotation, Vector3 direction_is_localScale) : this(cmd)
        {
            this.name = name;
            direction = direction_is_localScale;
            this.position = position;
            this.rotation = rotation;
        }

        /// <summary>
        /// 玩家位置同步操作指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="identity"></param>
        /// <param name="direction_is_localScale"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Operation(byte cmd, int identity, Vector3 position, Quaternion rotation, Vector3 direction_is_localScale) : this(cmd)
        {
            index = identity;
            direction = direction_is_localScale;
            this.position = position;
            this.rotation = rotation;
        }

        /// <summary>
        /// 重写字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{cmd}:{name}:{position}:{rotation}:{direction}";
        }
    }
}