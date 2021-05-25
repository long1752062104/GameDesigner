#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.Client
{
    public enum SerializeAdapterType
    {
        Default,
        PB_JSON_FAST,
        Binary,
        Binary2
    }

    public class SerializeAdapter : SingleCase<SerializeAdapter>
    {
        public SerializeAdapterType type;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            InitSDRpc();
        }

        public void InitSDRpc()
        {
            var cm = GetComponent<ClientManager>();
            switch (type) {
                case SerializeAdapterType.Default:
                    break;
                case SerializeAdapterType.PB_JSON_FAST:
                    cm.client.AddAdapter(new Adapter.SerializeFastAdapter());
                    break;
                case SerializeAdapterType.Binary:
                    cm.client.AddAdapter(new Adapter.SerializeAdapter());
                    break;
                case SerializeAdapterType.Binary2:
                    cm.client.AddAdapter(new Adapter.SerializeAdapter2());
                    break;
            }
        }
    }
}
#endif