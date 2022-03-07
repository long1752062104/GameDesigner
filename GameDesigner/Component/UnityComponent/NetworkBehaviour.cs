#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using Net.Share;
    using UnityEngine;

    /// <summary>
    /// 网络行为基础组件
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public abstract class NetworkBehaviour : MonoBehaviour
    {
        internal NetworkObject netObj;
        /// <summary>
        /// 这个物体是本机生成的?
        /// true:这个物体是从你本机实例化后, 同步给其他客户端的, 其他客户端的IsLocal为false
        /// false:这个物体是其他客户端实例化后,同步到本机客户端上, IsLocal为false
        /// </summary>
        public bool IsLocal => !netObj.isOtherCreate;
        public virtual void Awake()
        {
            netObj = GetComponent<NetworkObject>();
            netObj.networkBehaviours.Add(this); 
            netObj.InitSyncVar(this);
        }
        /// <summary>
        /// 当网络物体被初始化, 只有本机实例化的物体才会被调用
        /// </summary>
        /// <param name="identity"></param>
        public virtual void OnNetworkObjectInit(int identity) { }
        /// <summary>
        /// 当网络物体被创建后调用, 只有其他客户端发送创建信息给本机后才会被调用
        /// </summary>
        /// <param name="opt"></param>
        public virtual void OnNetworkObjectCreate(Operation opt) { }
        /// <summary>
        /// 当网络操作到达后应当开发者进行处理
        /// </summary>
        /// <param name="opt"></param>
        public virtual void OnNetworkOperationHandler(Operation opt) { }
        /// <summary>
        /// 当属性自动同步检查
        /// </summary>
        public virtual void OnPropertyAutoCheck() { }
        /// <summary>
        /// 检查组件是否启用
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckEnabled() { return enabled; }
        public virtual void OnDestroy()
        {
            netObj.RemoveSyncVar(this);
            netObj.networkBehaviours.Remove(this);
        }
    }
}
#endif