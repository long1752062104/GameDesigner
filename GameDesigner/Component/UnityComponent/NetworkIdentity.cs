#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Reflection;
    using Net.Component;
    using Net.Event;
    using Net.Share;
    using Net.System;
    using UnityEngine;

    public class NetworkIdentity : MonoBehaviour
    {
        private static bool isInit;
        internal static int Identity;
        internal static Queue<int> IDENTITY_POOL = new Queue<int>();
        public static int Capacity { get; private set; }
        [DisplayOnly] public int identity = -1;
        [Tooltip("自定义唯一标识, 可以不通过NetworkSceneManager的registerObjects去设置, 直接放在设计的场景里面, 不需要做成预制体")]
        public int m_identity;//可以设置的id
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
            if (m_identity != 0)
            {
                if (m_identity > 10000)
                {
                    Debug.Log("自定义唯一标识不能大于10000!");
                    Destroy(gameObject);
                    return;
                }
                identity = m_identity;
                sm.identitys.Add(m_identity, this);
                return;
            }
            if (IDENTITY_POOL.Count <= 0)
            {
                Debug.Log("已经没有唯一标识可用!");
                Destroy(gameObject);
                return;
            }
            identity = IDENTITY_POOL.Dequeue();
            sm.identitys.Add(identity, this);
            foreach (var item in networkBehaviours)
                item.OnNetworkIdentityInit(identity);
        }

        internal void InitSyncVar(object target)
        {
            Type type = target.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                SyncVar syncVar = field.GetCustomAttribute<SyncVar>();
                if (syncVar == null)
                    continue;
                var code = Type.GetTypeCode(field.FieldType);
                if(field.FieldType.IsClass & code == TypeCode.Object)
                {
                    NDebug.LogError($"SyncVar错误! 无法自动检测同步类的字段, 请使用结构体! 错误定义:{target.GetType().Name}类的{field.Name}字段");
                    continue;
                }
                MethodInfo method = null;
                if (!string.IsNullOrEmpty(syncVar.hook))
                    method = type.GetMethod(syncVar.hook, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                syncVarInfos.Add(new SyncVarFieldInfo()
                {
                    type = field.FieldType,
                    target = target,
                    fieldInfo = field,
                    value = field.GetValue(target),
                    OnValueChanged = method,
                    authorize = syncVar.authorize,
                    isEnum = field.FieldType.IsEnum,
                    baseType = code != TypeCode.Object
                });
            }
        }

        internal unsafe void CheckSyncVar()
        {
            Segment segment = null;
            for (int i = 0; i < syncVarInfos.Count; i++)
            {
                var syncVar = syncVarInfos[i];
                if (isOtherCreate & !syncVar.authorize)
                    continue;
                var value = syncVar.GetValue();
                if (value == null)
                    continue;
                if (!value.Equals(syncVar.value))
                {
                    syncVar.value = value;
                    if (segment == null)
                        segment = BufferPool.Take();
                    segment.WriteValue(i);
                    if (syncVar.baseType)
                        segment.WriteValue(value);
                    else
                        Serialize.NetConvertBinary.SerializeObject(segment, value, false, true);
                }
            }
            if (segment != null)
            {
                var buffer = segment.ToArray(true);
                ClientManager.AddOperation(new Operation(Command.SyncVar, identity)
                {
                    uid = ClientManager.UID,
                    index = registerObjectIndex,
                    buffer = buffer
                });
            }
        }

        internal void SyncVarHandler(Operation opt)
        {
            if (opt.cmd != Command.SyncVar | opt.uid == ClientManager.UID)
                return;
            Segment segment1 = new Segment(opt.buffer, false);
            while (segment1.Position < segment1.Index + segment1.Count)
            {
                var index = segment1.ReadValue<int>();
                var syncVar = syncVarInfos[index];
                var oldValue = syncVar.value;
                object value;
                if (syncVar.baseType)
                    value = segment1.ReadValue(syncVar.type);
                else
                    value = Serialize.NetConvertBinary.DeserializeObject(segment1, syncVar.type, false, true);
                if (syncVar.isEnum)
                    value = Enum.ToObject(syncVar.type, value);
                syncVar.SetValue(value);
                syncVar.value = value;
                syncVar.OnValueChanged?.Invoke(syncVar.target, new object[] { oldValue, value });
            }
        }

        internal void PropertyAutoCheckHandler() 
        {
            foreach (var networkBehaviour in networkBehaviours)
            {
                if (!networkBehaviour.enabled)
                    continue;
                networkBehaviour.OnPropertyAutoCheck();
            }
        }

        public virtual void OnDestroy()
        {
            if (identity == -1)
                return;
            var nsm = NetworkSceneManager.I;
            if(nsm != null)
                nsm.identitys.Remove(identity);
            if (isOtherCreate | identity < 10000)//identity < 10000则是自定义唯一标识
                return;
            IDENTITY_POOL.Enqueue(identity);
            if (ClientManager.I == null)
                return;
            ClientManager.AddOperation(new Operation(Command.Destroy, identity));
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