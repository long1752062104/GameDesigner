#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.Client
{
    using Net.Component.MMORPG_Client;
    using Net.Share;
    using UnityEngine;

    public enum SyncMode 
    {
        Control,
        Local,
        Synchronized
    }

    public class TransformComponent : MonoBehaviour
    {
        internal static int Identity = 0;
        private Net.Vector3 position;
        private Net.Quaternion rotation;
        private Net.Vector3 localScale;
        public SyncMode syncMode = SyncMode.Control;
        [DisplayOnly]
        public SyncMode mode = SyncMode.Synchronized;
        [DisplayOnly]
        public int identity = -1;//自身id
        private float sendTime;

        void Start()
        {
            mode = syncMode;
            switch (mode)
            {
                case SyncMode.Control:
                    if (!ClientManager.Instance.control)
                        return;
                    break;
                case SyncMode.Synchronized:
                    return;
            }
            var sm = FindObjectOfType<SceneManager>();
            if (sm == null)
            {
                Debug.Log("没有找到SceneManager组件！TransformComponent组件无效！");
                Destroy(gameObject);
                return;
            }
            JUMP: identity = Identity++;
            if (sm.transforms.ContainsKey(identity))
                goto JUMP;
            sm.transforms.Add(identity, this);
        }

        // Update is called once per frame
        void Update()
        {
            switch (mode) 
            {
                case SyncMode.Control:
                    if (!ClientManager.Instance.control)
                        return;
                    break;
                case SyncMode.Synchronized:
                    return;
            }
            if (Time.time > sendTime)
            {
                if (transform.position != position | transform.rotation != rotation | transform.localScale != localScale) 
                {
                    position = transform.position;
                    rotation = transform.rotation;
                    localScale = transform.localScale;
                    ClientManager.AddOperation(new Operation(Command.Transform, identity, localScale, position, rotation));
                }
                sendTime = Time.time + (1f / 30f);
            }
        }

        void OnDestroy()
        {
            if (ClientManager.Instance == null)
                return;
            switch (mode)
            {
                case SyncMode.Control:
                    if (!ClientManager.Instance.control)
                        return;
                    break;
                case SyncMode.Synchronized:
                    return;
            }
            ClientManager.AddOperation(new Operation(Command.Destroy) { index = identity });
        }
    }
}
#endif