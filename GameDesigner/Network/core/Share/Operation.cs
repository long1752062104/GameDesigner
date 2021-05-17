namespace Net.Share
{
    /// <summary>
    /// 帧同步操作
    /// </summary>
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class Operation //: ISerialize<Operation>
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
        /// 玩家生命值
        /// </summary>
        public float health;
        /// <summary>
        /// 数据数组, 备用
        /// </summary>
        public byte[] buffer;
        /// <summary>
        /// 备用字符串
        /// </summary>
        public string name1, name2;

        /// <summary>
        /// 默认
        /// </summary>
        public Operation()
        {
        }

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
        /// 获取Rpc远程函数
        /// </summary>
        /// <returns></returns>
        public FuncData GetData()
        {
            return NetConvert.Deserialize(buffer);
        }

        public unsafe void Write(Segment strem)
        {
            int pos = strem.Position;
            strem.Position += 2;
            byte[] bits = new byte[2];
            if (cmd != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 1, true);
                strem.WriteByte(cmd);
            }
            if (cmd1 != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 2, true);
                strem.WriteByte(cmd1);
            }
            if (cmd2 != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                strem.WriteByte(cmd2);
            }
            if (!string.IsNullOrEmpty(name))
            {
                NetConvertBase.SetBit(ref bits[0], 4, true);
                strem.WriteValue(name);
            }
            if (!string.IsNullOrEmpty(name1))
            {
                NetConvertBase.SetBit(ref bits[0], 5, true);
                strem.WriteValue(name1);
            }
            if (!string.IsNullOrEmpty(name2))
            {
                NetConvertBase.SetBit(ref bits[0], 6, true);
                strem.WriteValue(name2);
            }
            if (position != Vector3.zero)
            {
                NetConvertBase.SetBit(ref bits[0], 7, true);
                fixed (void* ptr = &position.x)
                    strem.Write(ptr, 12);
            }
            if (direction != Vector3.zero)
            {
                NetConvertBase.SetBit(ref bits[0], 8, true);
                fixed (void* ptr = &direction.x)
                    strem.Write(ptr, 12);
            }
            if (rotation != Quaternion.zero)
            {
                NetConvertBase.SetBit(ref bits[1], 1, true);
                fixed (void* ptr = &rotation.x)
                    strem.Write(ptr, 16);
            }
            if (index != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 2, true);
                strem.WriteValue(index);
            }
            if (index1 != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 3, true);
                strem.WriteValue(index1);
            }
            if (index2 != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 4, true);
                strem.WriteValue(index2);
            }
            if (health != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 5, true);
                strem.WriteValue(health);
            }
            if (buffer == null)
                buffer = new byte[0];
            strem.WriteArray(buffer);
            int pos1 = strem.Position;
            strem.Position = pos;
            strem.Write(bits, 0, 2);
            strem.Position = pos1;
        }

        public void Read(Segment strem)
        {
            byte[] bits = strem.Read(2);
            if(NetConvertBase.GetBit(bits[0], 1))
                cmd = strem.ReadValue<byte>();
            if (NetConvertBase.GetBit(bits[0], 2))
                cmd1 = strem.ReadValue<byte>();
            if (NetConvertBase.GetBit(bits[0], 3))
                cmd2 = strem.ReadValue<byte>();
            if (NetConvertBase.GetBit(bits[0], 4))
                name = strem.ReadValue<string>();
            if (NetConvertBase.GetBit(bits[0], 5))
                name1 = strem.ReadValue<string>();
            if (NetConvertBase.GetBit(bits[0], 6))
                name2 = strem.ReadValue<string>();
            if (NetConvertBase.GetBit(bits[0], 7))
            {
                position.x = strem.ReadValue<float>();
                position.y = strem.ReadValue<float>();
                position.z = strem.ReadValue<float>();
            }
            if (NetConvertBase.GetBit(bits[0], 8))
            {
                direction.x = strem.ReadValue<float>();
                direction.y = strem.ReadValue<float>();
                direction.z = strem.ReadValue<float>();
            }
            if (NetConvertBase.GetBit(bits[1], 1))
            {
                rotation.x = strem.ReadValue<float>();
                rotation.y = strem.ReadValue<float>();
                rotation.z = strem.ReadValue<float>();
                rotation.w = strem.ReadValue<float>();
            }
            if (NetConvertBase.GetBit(bits[1], 2))
                index = strem.ReadValue<int>();
            if (NetConvertBase.GetBit(bits[1], 3))
                index1 = strem.ReadValue<int>();
            if (NetConvertBase.GetBit(bits[1], 4))
                index2 = strem.ReadValue<int>();
            if (NetConvertBase.GetBit(bits[1], 5))
                health = strem.ReadValue<float>();
            buffer = strem.ReadArray<byte>();
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