using Net.Serialize;
using Net.Share;
using Net.System;

namespace Net.Adapter
{
    /// <summary>
    /// 通用升级版适配器
    /// </summary>
    public class SerializeFastAdapter : ISerializeAdapter
    {
        public bool isEncrypt = false;
        public byte[] OnSerializeRpc(RPCModel model)
        {
            var buffer = NetConvertFast.Serialize(model);
            if (isEncrypt)
                Helper.EncryptHelper.ToEncrypt(12345678, buffer);
            return buffer;
        }

        public FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            if (isEncrypt)
                Helper.EncryptHelper.ToDecrypt(12345678, buffer, index, count);
            return NetConvertFast.Deserialize(buffer, index, count);
        }

        public byte[] OnSerializeOpt(OperationList list)
        {
            return NetConvertFast2.SerializeObject(list).ToArray(true);
        }

        public OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            Segment segment = new Segment(buffer, index, count, false);
            return NetConvertFast2.DeserializeObject<OperationList>(segment);
        }
    }

    /// <summary>
    /// 快速序列化适配器
    /// </summary>
    public class SerializeAdapter : ISerializeAdapter
    {
        public bool isEncrypt = false;
        public byte[] OnSerializeRpc(RPCModel model)
        {
            var buffer = NetConvertBinary.SerializeModel(model);
            if (isEncrypt)
                Helper.EncryptHelper.ToEncrypt(12345678, buffer);
            return buffer;
        }

        public FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            if (isEncrypt)
                Helper.EncryptHelper.ToDecrypt(12345678, buffer, index, count);
            return NetConvertBinary.DeserializeModel(buffer, index, count);
        }

        public byte[] OnSerializeOpt(OperationList list)
        {
            return NetConvertBinary.SerializeObject(list).ToArray(true);
        }

        public OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            return NetConvertBinary.DeserializeObject<OperationList>(buffer, index, count);
        }
    }

    /// <summary>
    /// 快速序列化2适配器
    /// </summary>
    public class SerializeAdapter2 : ISerializeAdapter
    {
        public bool isEncrypt = false;
        public byte[] OnSerializeRpc(RPCModel model)
        {
            var buffer = NetConvertBinary.SerializeModel(model);
            if (isEncrypt)
                Helper.EncryptHelper.ToEncrypt(12345678, buffer);
            return buffer;
        }

        public FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            if (isEncrypt)
                Helper.EncryptHelper.ToDecrypt(12345678, buffer, index, count);
            return NetConvertBinary.DeserializeModel(buffer, index, count);
        }

        public byte[] OnSerializeOpt(OperationList list)
        {
            return NetConvertFast2.SerializeObject(list).ToArray(true);
        }

        public OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            Segment segment = new Segment(buffer, index, count, false);
            return NetConvertFast2.DeserializeObject<OperationList>(segment);
        }
    }

    /// <summary>
    /// 极速序列化3适配器
    /// </summary>
    public class SerializeAdapter3 : ISerializeAdapter
    {
        public bool isEncrypt = false;
        public byte[] OnSerializeRpc(RPCModel model)
        {
            var buffer = NetConvertFast2.SerializeModel(model);
            if (isEncrypt)
                Helper.EncryptHelper.ToEncrypt(12345678, buffer);
            return buffer;
        }

        public FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            if (isEncrypt)
                Helper.EncryptHelper.ToDecrypt(12345678, buffer, index, count);
            Segment segment = new Segment(buffer, index, count, false);
            return NetConvertFast2.DeserializeModel(segment);
        }

        public byte[] OnSerializeOpt(OperationList list)
        {
            return NetConvertFast2.SerializeObject(list).ToArray(true);
        }

        public OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            Segment segment = new Segment(buffer, index, count, false);
            return NetConvertFast2.DeserializeObject<OperationList>(segment);
        }
    }
}