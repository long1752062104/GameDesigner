#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using global::System.Collections.Generic;
    using Net.Component;
    using Net.Helper;
    using Net.Share;
    using UnityEngine;

    /// <summary>
    /// 网络物体标识组件
    /// </summary>
    public class NetworkObject : MonoBehaviour
    {
        private static bool isInit;
        internal static int Identity;
        internal static Queue<int> IDENTITY_POOL = new Queue<int>();
        public static int Capacity { get; private set; }
        [DisplayOnly] public int m_identity = -1;
        [Tooltip("自定义唯一标识, 当值不为0后,可以不通过NetworkSceneManager的registerObjects去设置, 直接放在设计的场景里面, 不需要做成预制体")]
        public int identity;//可以设置的id
        [Tooltip("注册的网络物体索引, registerObjectIndex要对应NetworkSceneManager的registerObjects数组索引, 如果设置了自定义唯一标识, 则此字段无效!")]
        public int registerObjectIndex;
        internal bool isOtherCreate;
        internal List<NetworkBehaviour> networkBehaviours = new List<NetworkBehaviour>();
        internal List<SyncVarInfo> syncVarInfos = new List<SyncVarInfo>();
        public virtual void Start()
        {
            if (isOtherCreate)
                return;
            var sm = NetworkSceneManager.I;
            if (sm == null)
            {
                Debug.Log("没有找到NetworkSceneManager组件！NetworkIdentity组件无效！");
                Destroy(gameObject);
                return;
            }
            if (m_identity > 0)
            {
                sm.identitys.Add(m_identity, this);
                foreach (var item in networkBehaviours)
                    item.OnNetworkObjectInit(m_identity);
                return;
            }
            if (identity > 0)
            {
                m_identity = identity;
                sm.identitys.Add(m_identity, this);
                foreach (var item in networkBehaviours)
                    item.OnNetworkObjectInit(m_identity);
                return;
            }
            if (IDENTITY_POOL.Count <= 0)
            {
                Debug.Log("已经没有唯一标识可用!");
                Destroy(gameObject);
                return;
            }
            m_identity = IDENTITY_POOL.Dequeue();
            sm.identitys.Add(m_identity, this);
            foreach (var item in networkBehaviours)
                item.OnNetworkObjectInit(m_identity);
        }

        internal void InitSyncVar(object target)
        {
            ClientManager.I.client.AddRpcHandle(target, false, (info) =>
            {
                info.id = (ushort)syncVarInfos.Count;
                syncVarInfos.Add(info);
            });
        }

        internal void CheckSyncVar()
        {
            SyncVarHelper.CheckSyncVar(isOtherCreate, syncVarInfos, (buffer)=> 
            {
                ClientManager.AddOperation(new Operation(NetCmd.SyncVar, m_identity)
                {
                    uid = ClientManager.UID,
                    index = registerObjectIndex,
                    buffer = buffer
                });
            });
        }

        internal void SyncVarHandler(Operation opt)
        {
            if (opt.cmd != NetCmd.SyncVar | opt.uid == ClientManager.UID)
                return;
            SyncVarHelper.SyncVarHandler(syncVarInfos, opt.buffer);
        }

        internal void RemoveSyncVar(NetworkBehaviour target)
        {
            SyncVarHelper.RemoveSyncVar(syncVarInfos, target);
        }

        internal void PropertyAutoCheckHandler() 
        {
            foreach (var networkBehaviour in networkBehaviours)
            {
                if (!networkBehaviour.CheckEnabled())
                    continue;
                networkBehaviour.OnPropertyAutoCheck();
            }
        }

        public virtual void OnDestroy()
        {
            if (m_identity == -1)
                return;
            var nsm = NetworkSceneManager.I;
            if(nsm != null)
                nsm.identitys.Remove(m_identity);
            if (isOtherCreate | m_identity < 10000)//identity < 10000则是自定义唯一标识
                return;
            IDENTITY_POOL.Enqueue(m_identity);
            if (ClientManager.I == null)
                return;
            ClientManager.AddOperation(new Operation(Command.Destroy, m_identity));
        }

        /// <summary>
        /// 初始化网络唯一标识
        /// </summary>
        /// <param name="capacity">一个客户端可以用的唯一标识容量</param>
        public static void Init(int capacity = 5000) 
        {
            if (isInit)
                return;
            isInit = true;
            Capacity = capacity;
            Identity = 10000 + ((ClientManager.UID + 1 - 10000) * capacity);
            for (int i = Identity; i < Identity + capacity; i++)
                IDENTITY_POOL.Enqueue(i);
        }
    }
}
#endif