#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using Net.Share;
    using UnityEngine;

    [RequireComponent(typeof(NetworkIdentity))]
    public abstract class NetworkBehaviour : MonoBehaviour
    {
        public NetworkIdentity networkIdentity;
        public virtual void Awake()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            networkIdentity.networkBehaviours.Add(this);
        }
        public abstract void OnNetworkIdentityInit(int id, int newId);
        public abstract void OnNetworkIdentityCreate(Operation opt);
        public abstract void OnNetworkOperationHandler(Operation opt);
        public virtual void OnDestroy()
        {
            networkIdentity.networkBehaviours.Remove(this);
        }
    }
}
#endif