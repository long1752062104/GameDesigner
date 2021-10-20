using Net.Serialize;
using Net.Server;

namespace Net.Share
{
    public enum AdapterType 
    {
        Serialize,
        RPC,
        NetworkEvt,
    }

    /// <summary>
    /// 基础适配器接口
    /// </summary>
    public interface IAdapter
    {
    }

    /// <summary>
    /// 序列化适配器
    /// </summary>
    public interface ISerializeAdapter : IAdapter
    {
        byte[] OnSerializeRpc(RPCModel model);

        FuncData OnDeserializeRpc(byte[] buffer, int index, int count);

        byte[] OnSerializeOpt(OperationList list);

        OperationList OnDeserializeOpt(byte[] buffer, int index, int count);
    }

    /// <summary>
    /// 客户端RPC适配器
    /// </summary>
    public interface IRPCAdapter : IAdapter
    {
        void AddRpcHandle(object target, bool append);

        void OnRpcExecute(RPCModel model);

        void RemoveRpc(object target);
        /// <summary>
        /// 每50毫秒调用检查rpc是否被释放
        /// </summary>
        void CheckRpcUpdate();
    }

    /// <summary>
    /// 服务器RPC适配器
    /// </summary>
    /// <typeparam name="Player"></typeparam>
    public interface IRPCAdapter<Player> : IAdapter where Player : NetPlayer
    {
        void AddRpcHandle(object target, bool append);

        void OnRpcExecute(Player client, RPCModel model);

        void RemoveRpc(object target);
    }

    /// <summary>
    /// 网络事件适配器
    /// </summary>
    public interface INetworkEvtAdapter : INetworkHandle, IAdapter 
    {
    }
}
