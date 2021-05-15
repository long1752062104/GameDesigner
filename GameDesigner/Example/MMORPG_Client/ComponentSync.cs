#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.Client
{
    using Net.Component.MMORPG_Client;
    using Net.Share;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using Random = UnityEngine.Random;

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    [System.Serializable]
    public class PropertySync
    {
        public string name;
        [ProtoBuf.ProtoIgnore]
        public PropertyInfo property;
        public byte[] value;

        [ProtoBuf.ProtoIgnore]
        public object Value
        {
            get
            {
                return NetConvert.Deserialize(value)[0];
            }
        }
    }

    public class ComponentSync : MonoBehaviour
    {
        public Component component;
        public List<PropertySync> propertySyncs = new List<PropertySync>();
        public string componentTypeName;

        private void Awake()
        {
            var sm = FindObjectOfType<SceneManager>();
            if (sm == null)
            {
                Debug.Log("没有找到SceneManager组件！ComponentSync组件无效！");
                Destroy(gameObject);
                return;
            }
            string name1 = name;
        JUMP: name = name1 + Random.Range(0, 99999);
            if (sm.componentSyncs.ContainsKey(name))
                goto JUMP;
            sm.componentSyncs.Add(name, this);
        }

        // Start is called before the first frame update
        void Start()
        {
            componentTypeName = component.GetType().FullName;
            foreach (var pro in propertySyncs)
            {
                pro.property = component.GetType().GetProperty(pro.name);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!ClientManager.Instance.control)
                return;

            foreach (var pro in propertySyncs)
            {
                var value = pro.property.GetValue(component);
                pro.value = NetConvert.Serialize(new RPCModel() { pars = new object[] { value } });
            }
            //ClientManager.Instance.client.AddOperation(Command.PropertySync, name, componentTypeName, propertySyncs);
        }

        public void SetPropertySync(List<PropertySync> properties)
        {
            foreach (var pro in properties)
            {
                if (pro.property == null)
                    pro.property = component.GetType().GetProperty(pro.name);
                pro.property.SetValue(component, pro.Value);
            }
        }

        void OnDestroy()
        {
            if (ClientManager.Instance == null)
                return;
            if (!ClientManager.Instance.control)
                return;
            ClientManager.AddOperation(new Operation(Command.Destroy, name));
        }
    }
}
#endif