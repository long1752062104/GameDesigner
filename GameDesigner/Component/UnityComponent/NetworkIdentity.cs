#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using global::System.Collections.Generic;
    using Net.Component;
    using UnityEngine;

    public class NetworkIdentity : MonoBehaviour
    {
        public int registerObjectIndex;
        internal List<NetworkBehaviour> networkBehaviours = new List<NetworkBehaviour>();
        [DisplayOnly] public int identity = -1;
        public int m_identity;//可以设置的id
        internal static int Identity;

        protected virtual void Start()
        {
            var sm = FindObjectOfType<NetworkSceneManager>();
            if (sm == null)
            {
                Debug.Log("没有找到NetworkSceneManager组件！NetworkIdentity组件无效！");
                Destroy(gameObject);
                return;
            }
            if (m_identity != 0)
            {
                this.identity = m_identity;
                sm.identitys.Add(m_identity, this);
                return;
            }
            var identity = Identity++;
            sm.localIdentitys.Add(identity, this);
            ClientManager.I.client.RegisterNetworkIdentity(identity);
        }
    }
}
#endif