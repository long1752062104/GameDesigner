#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component.Client
{
    using Net.Example;
    using Net.Share;
    using UnityEngine;

    public enum SyncMode 
    {
        Local,
        Control,
        Synchronized,
        SynchronizedAll
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
        internal float sendTime;
        public float interval = 0.5f;
        internal Net.Vector3 netPosition;
        internal Net.Quaternion netRotation;

        void Start()
        {
            mode = syncMode;
            switch (mode)
            {
                case SyncMode.Synchronized:
                case SyncMode.SynchronizedAll:
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
            if (mode == SyncMode.Synchronized)
            {
                transform.position = Vector3.Lerp(transform.position, netPosition, 0.3f);
                if(netRotation != Net.Quaternion.identity)
                    transform.rotation = Quaternion.Lerp(transform.rotation, netRotation, 0.3f);
            }
            else if (Time.time > sendTime)
            {
                if (transform.position != position | transform.rotation != rotation | transform.localScale != localScale) 
                {
                    position = transform.position;
                    rotation = transform.rotation;
                    localScale = transform.localScale;
                    ClientManager.AddOperation(new Operation(Command.Transform, identity, localScale, position, rotation) {
                        cmd1 = (byte)mode, index1 = ClientManager.UID
                    });
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
                case SyncMode.Synchronized:
                case SyncMode.SynchronizedAll:
                    return;
            }
            ClientManager.AddOperation(new Operation(Command.Destroy) { index = identity });
        }
    }
}
#endif