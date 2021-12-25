#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using global::System.Collections.Generic;
    using Net.Component;
    using Net.Share;
    using Net.System;
    using UnityEngine;

    public class NetworkSceneManager : MonoBehaviour
    {
        public List<NetworkIdentity> registerObjects = new List<NetworkIdentity>();
        [HideInInspector]
        public MyDictionary<int, NetworkIdentity> identitys = new MyDictionary<int, NetworkIdentity>();
        [HideInInspector]
        internal MyDictionary<int, NetworkIdentity> localIdentitys = new MyDictionary<int, NetworkIdentity>();

        // Start is called before the first frame update
        void Start()
        {
            ClientManager.I.client.OnRegisterNetworkIdentity += RegisterNetworkIdentity;
            ClientManager.I.client.OnOperationSync += OperationSync;
        }

        private void RegisterNetworkIdentity(int id, int newId)
        {
            if (localIdentitys.TryGetValue(id, out NetworkIdentity identity))
            {
                identity.identity = newId;
                identitys.Add(newId, identity);
                localIdentitys.Remove(id);
                foreach (var item in identity.networkBehaviours)
                {
                    item.OnNetworkIdentityInit(id, newId);
                }
            }
        }

        private void OperationSync(OperationList list)
        {
            foreach (var opt in list.operations)
            {
                OnNetworkOperSync(opt);
            }
        }

        void OnNetworkOperSync(Operation opt)
        {
            if (!identitys.TryGetValue(opt.identity, out NetworkIdentity identity))
            {
                identity = Instantiate(registerObjects[opt.identity]);
                identity.identity = opt.identity;
                foreach (var item in identity.networkBehaviours)
                {
                    item.OnNetworkIdentityCreate(opt);
                }
            }
            foreach (var item in identity.networkBehaviours)
            {
                item.OnNetworkOperationHandler(opt);
            }
        }

        void OnDestroy()
        {
            ClientManager.I.client.OnRegisterNetworkIdentity -= RegisterNetworkIdentity;
            ClientManager.I.client.OnOperationSync -= OperationSync;
        }
    }
}
#endif