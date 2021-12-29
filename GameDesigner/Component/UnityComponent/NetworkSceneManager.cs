#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using global::System.Collections.Generic;
    using Net.Component;
    using Net.Share;
    using Net.System;
    using UnityEngine;

    [RequireComponent(typeof(NetworkTime))]
    public class NetworkSceneManager : SingleCase<NetworkSceneManager>
    {
        public List<NetworkIdentity> registerObjects = new List<NetworkIdentity>();
        [HideInInspector]
        public MyDictionary<int, NetworkIdentity> identitys = new MyDictionary<int, NetworkIdentity>();

        private void OnConnectedHandle()
        {
            NetworkIdentity.Init(5000);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (ClientManager.I.client.Connected)
                OnConnectedHandle();
            else
                ClientManager.I.client.OnConnectedHandle += OnConnectedHandle;
            ClientManager.I.client.OnOperationSync += OperationSync;
        }

        public virtual void Update() 
        {
            if (NetworkTime.CanSent) 
            {
                foreach (var identity in identitys.Values)
                {
                    if (!identity.enabled)
                        continue;
                    identity.CheckSyncVar();
                    identity.PropertyAutoCheckHandler();
                }
            }
        }

        private void OperationSync(OperationList list)
        {
            foreach (var opt in list.operations)
                OnNetworkOperSync(opt);
        }

        void OnNetworkOperSync(Operation opt)
        {
            switch (opt.cmd) 
            {
                case Command.Transform:
                    {
                        if (!identitys.TryGetValue(opt.identity, out NetworkIdentity identity))
                        {
                            identity = Instantiate(registerObjects[opt.index]);
                            identity.identity = opt.identity;
                            identity.isOtherCreate = true;
                            identitys.Add(opt.identity, identity);
                            foreach (var item in identity.networkBehaviours)
                                item.OnNetworkIdentityCreate(opt);
                        }
                        foreach (var item in identity.networkBehaviours)
                            item.OnNetworkOperationHandler(opt);
                    }
                    break;
                case Command.BuildComponent:
                    {
                        if (!identitys.TryGetValue(opt.identity, out NetworkIdentity identity))
                        {
                            identity = Instantiate(registerObjects[opt.index]);
                            identity.identity = opt.identity;
                            identity.isOtherCreate = true;
                            identitys.Add(opt.identity, identity);
                            foreach (var item in identity.networkBehaviours)
                                item.OnNetworkIdentityCreate(opt);
                        }
                        foreach (var item in identity.networkBehaviours)
                            item.OnNetworkOperationHandler(opt);
                    }
                    break;
                case Command.Destroy:
                    {
                        if (identitys.TryGetValue(opt.identity, out NetworkIdentity identity))
                        {
                            Destroy(identity.gameObject);
                        }
                    }
                    break;
                case Command.OnPlayerExit:
                    {
                        var uid = 10000 + ((opt.uid + 1 - 10000) * NetworkIdentity.Capacity);
                        var count = uid + NetworkIdentity.Capacity;
                        foreach (var item in identitys)
                        {
                            if (item.Key >= uid & item.Key < count) 
                            {
                                Destroy(item.Value.gameObject);
                            }
                        }
                    }
                    break;
                case Command.SyncVar:
                    {
                        if (!identitys.TryGetValue(opt.identity, out NetworkIdentity identity))
                        {
                            identity = Instantiate(registerObjects[opt.index]);
                            identity.identity = opt.identity;
                            identity.isOtherCreate = true;
                            identitys.Add(opt.identity, identity);
                            foreach (var item in identity.networkBehaviours)
                                item.OnNetworkIdentityCreate(opt);
                        }
                        identity.SyncVarHandler(opt);
                    }
                    break;
                default:
                    OnOtherOperator(opt);
                    break;
            }
        }

        public virtual void OnOtherOperator(Operation opt)
        {
        }

        void OnDestroy()
        {
            ClientManager.I.client.OnConnectedHandle -= OnConnectedHandle;
            ClientManager.I.client.OnOperationSync -= OperationSync;
        }
    }
}
#endif