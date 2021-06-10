namespace Net.Share
{
    /// <summary>
    /// 帧同步列表
    /// </summary>
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class OperationList
    {
        /// <summary>
        /// 帧索引
        /// </summary>
        public uint frame;
        /// <summary>
        /// 帧操作列表
        /// </summary>
        public Operation[] operations;

        /// <summary>
        /// 构造
        /// </summary>
        public OperationList()
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
            operations = new Operation[0];
#endif
        }

        /// <summary>
        /// 构造
        /// </summary>
        public OperationList(uint frame)
        {
            this.frame = frame;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="opts"></param>
        public OperationList(Operation[] opts)
        {
            operations = opts;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="opts"></param>
        public OperationList(uint frame, Operation[] opts)
        {
            this.frame = frame;
            operations = opts;
        }

        public override string ToString()
        {
            return $"frame:{frame} - opers:{operations}";
        }
    }
}
