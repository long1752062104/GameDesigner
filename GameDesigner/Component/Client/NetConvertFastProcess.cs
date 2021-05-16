#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.Client
{
    using Net.Share;

    public class NetConvertFastProcess : SingleCase<NetConvertFastProcess>
    {
        private void Awake()
        {
            var cm = GetComponent<ClientManager>();
            cm.client.OnSerializeRPC = SerializeRpc;
            cm.client.OnDeserializeRPC = DeserializeRpc;
            NetConvertFast.Init();
        }

        private byte[] SerializeRpc(RPCModel arg)
        {
            return NetConvertFast.Serialize(arg);
        }

        private FuncData DeserializeRpc(byte[] buffer, int index, int count)
        {
            return NetConvertFast.Deserialize(buffer, index, count);
        }
    }
}
#endif