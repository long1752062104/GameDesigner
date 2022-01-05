#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Reflection;
    using Net.Component;
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
                MethodInfo method = null;
                if (!string.IsNullOrEmpty(syncVar.hook))
                    method = type.GetMethod(syncVar.hook, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var code = Type.GetTypeCode(field.FieldType);
                var isClass = false;
                if (code == TypeCode.Object & field.FieldType.IsValueType)
                {
                    var fields1 = field.FieldType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var field1 in fields1)
                    {
                        var code1 = Type.GetTypeCode(field1.FieldType);
                        if (code1 == TypeCode.Object)
                        {
                            if (field1.FieldType.IsClass)
                            {
                                isClass = true;
                                break;
                            }
                            int layer = 0;
                            isClass = CheckIsClass(field1.FieldType, ref layer);
                            if (isClass)
                                break;
                        }
                    }
                }
                else if (code == TypeCode.Object & field.FieldType.IsClass)//解决string, string也是类
                    isClass = true;
                syncVarInfos.Add(new SyncVarFieldInfo()
                {
                    type = field.FieldType,
                    target = target,
                    fieldInfo = field,
                    value = isClass ? Clone.Instance(field.GetValue(target)) : field.GetValue(target),
                    OnValueChanged = method,
                    authorize = syncVar.authorize,
                    isEnum = field.FieldType.IsEnum,
                    baseType = code != TypeCode.Object,
                    isClass = isClass,
                    isList = field.FieldType.IsGenericType | field.FieldType.IsArray,
                    isUnityObject = field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) | field.FieldType == typeof(UnityEngine.Object),
                });
            }
        }

        private bool CheckIsClass(Type type, ref int layer, bool root = true)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var code = Type.GetTypeCode(field.FieldType);
                if (code == TypeCode.Object)
                {
                    if (field.FieldType.IsClass)
                        return true;
                    if (root)
                        layer = 0;
                    if (layer++ < 5)
                    {
                        var isClass = CheckIsClass(field.FieldType, ref layer, false);
                        if (isClass)
                            return true;
                    }
                }
            }
            return false;
        }

        private bool SyncListEquals(IList a, IList b)
        {
            if (a == null | b == null)
                return false;
            if (a.Count != b.Count)
                return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (!a[i].Equals(b[i]))
                    return false;
            }
            return true;
        }

        internal void CheckSyncVar()
        {
            Segment segment = null;
            for (int i = 0; i < syncVarInfos.Count; i++)
            {
                var syncVar = syncVarInfos[i];
                if ((isOtherCreate & !syncVar.authorize) | syncVar.isDispose)
                    continue;
                var value = syncVar.GetValue();
                if (value == null)
                    continue;
                if (syncVar.isList)
                {
                    var a = value as IList;
                    var b = syncVar.value as IList;
                    if (SyncListEquals(a, b))
                        continue;
                }
                else if (value.Equals(syncVar.value))
                    continue;
                if (syncVar.isUnityObject)
                {
                    syncVar.value = value;
#if UNITY_EDITOR
                    string path = UnityEditor.AssetDatabase.GetAssetPath((UnityEngine.Object)value);
                    if (segment == null)
                        segment = BufferPool.Take();
                    segment.WriteValue(i);
                    segment.WriteValue(path);
                    continue;
#else
                    return;
#endif
                }
                if (syncVar.isClass)
                    syncVar.value = Clone.Instance(value);
                else
                    syncVar.value = value;
                if (segment == null)
                    segment = BufferPool.Take();
                segment.WriteValue(i);
                if (syncVar.baseType)
                    segment.WriteValue(value);
                else
                    Serialize.NetConvertBinary.SerializeObject(segment, value, false, true);
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
                else if (syncVar.isUnityObject)
                {
                    var path = segment1.ReadValue<string>();
#if UNITY_EDITOR
                    value = UnityEditor.AssetDatabase.LoadAssetAtPath(path, syncVar.type);
                    syncVar.SetValue(value);
                    syncVar.value = value;
#endif
                    continue;
                }
                else
                    value = Serialize.NetConvertBinary.DeserializeObject(segment1, syncVar.type, false, false, true);
                if (syncVar.isEnum)
                    value = Enum.ToObject(syncVar.type, value);
                if (syncVar.isDispose)
                    continue;
                if (syncVar.isClass)
                {
                    syncVar.SetValue(value);
                    syncVar.value = Clone.Instance(value);
                }
                else
                {
                    syncVar.SetValue(value);
                    syncVar.value = value;
                }
                syncVar.OnValueChanged?.Invoke(syncVar.target, new object[] { oldValue, value });
            }
        }

        internal void RemoveSyncVar(NetworkBehaviour target)
        {
            for (int i = 0; i < syncVarInfos.Count; i++)
            {
                var syncVar = syncVarInfos[i];
                if (target.Equals(syncVar.target))
                    syncVar.isDispose = true;
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