using Net.Share;

namespace Net.Adapter
{
    /// <summary>
    /// 快速序列化适配器
    /// </summary>
    public class SerializeAdapter : ISerializeAdapter
    {
        public OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            return NetConvertBinary.DeserializeObject<OperationList>(buffer, index, count);
        }

        public FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            return NetConvertBinary.DeserializeModel(buffer, index, count);
        }

        public byte[] OnSerializeOpt(OperationList list)
        {
            return NetConvertBinary.SerializeObject(list).ToArray(true);
        }

        public byte[] OnSerializeRpc(RPCModel model)
        {
            return NetConvertBinary.SerializeModel(model);
        }
    }

    /// <summary>
    /// 快速序列化适配器
    /// </summary>
    public class SerializeAdapter2 : ISerializeAdapter
    {
        public OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            Segment segment = new Segment(buffer, index, count, false);
            return NetConvertFast2.DeserializeObject<OperationList>(segment);
        }

        public FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            return NetConvertBinary.DeserializeModel(buffer, index, count);
        }

        public byte[] OnSerializeOpt(OperationList list)
        {
            return NetConvertFast2.SerializeObject(list).ToArray(true);
        }

        public byte[] OnSerializeRpc(RPCModel model)
        {
            return NetConvertBinary.SerializeModel(model);
        }
    }
}
