#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using Net.Component;
    using Net.Share;
    using UnityEngine;

    [RequireComponent(typeof(NetworkIdentity))]
    public abstract class NetworkBehaviour : MonoBehaviour
    {
        internal NetworkIdentity networkIdentity;
        public virtual void Awake()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            networkIdentity.networkBehaviours.Add(this); 
            networkIdentity.InitSyncVar(this);
        }
        public virtual void Start() 
        {
            ClientManager.AddRpcHandler(this);
        }
        public virtual void OnNetworkIdentityInit(int identity) { }
        public virtual void OnNetworkIdentityCreate(Operation opt) { }
        public virtual void OnNetworkOperationHandler(Operation opt) { }
        public virtual void OnPropertyAutoCheck() { }
        public virtual void OnDestroy()
        {
            networkIdentity.RemoveSyncVar(this);
            networkIdentity.networkBehaviours.Remove(this);
        }
    }
}
#endif