#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Share;
    using UnityEngine;

    public enum SyncMode 
    {
        Local,
        Control,
        Synchronized,
        SynchronizedAll
    }

    public enum SyncProperty
    {
        All,
        position,
        rotation,
        localScale,
        position_rotation,
        position_localScale,
        rotation_localScale,
    }

    public class TransformComponent : MonoBehaviour
    {
        internal static int Identity = 0;
        private Net.Vector3 position;
        private Net.Quaternion rotation;
        private Net.Vector3 localScale;
        public SyncMode syncMode = SyncMode.Control;
        public SyncProperty property = SyncProperty.All;
        [DisplayOnly]
        public SyncMode mode = SyncMode.Synchronized;
        [DisplayOnly]
        public int identity = -1;//自身id
        internal float sendTime;
        public float interval = 0.5f;
        internal Net.Vector3 netPosition;
        internal Net.Quaternion netRotation;
        internal Net.Vector3 netLocalScale;

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
                switch (property)
                {
                    case SyncProperty.All:
                        transform.position = Vector3.Lerp(transform.position, netPosition, 0.3f);
                        if (netRotation != Net.Quaternion.identity)
                            transform.rotation = Quaternion.Lerp(transform.rotation, netRotation, 0.3f);
                        transform.localScale = netLocalScale;
                        break;
                    case SyncProperty.position:
                        transform.position = Vector3.Lerp(transform.position, netPosition, 0.3f);
                        break;
                    case SyncProperty.rotation:
                        if (netRotation != Net.Quaternion.identity)
                            transform.rotation = Quaternion.Lerp(transform.rotation, netRotation, 0.3f);
                        break;
                    case SyncProperty.localScale:
                        transform.localScale = netLocalScale;
                        break;
                    case SyncProperty.position_rotation:
                        transform.position = Vector3.Lerp(transform.position, netPosition, 0.3f);
                        if (netRotation != Net.Quaternion.identity)
                            transform.rotation = Quaternion.Lerp(transform.rotation, netRotation, 0.3f);
                        break;
                    case SyncProperty.position_localScale:
                        transform.position = Vector3.Lerp(transform.position, netPosition, 0.3f);
                        transform.localScale = netLocalScale;
                        break;
                    case SyncProperty.rotation_localScale:
                        if (netRotation != Net.Quaternion.identity)
                            transform.rotation = Quaternion.Lerp(transform.rotation, netRotation, 0.3f);
                        transform.localScale = netLocalScale;
                        break;
                }
            }
            else if (Time.time > sendTime)
            {
                if (transform.position != position | transform.rotation != rotation | transform.localScale != localScale) 
                {
                    position = transform.position;
                    rotation = transform.rotation;
                    localScale = transform.localScale;
                    switch (property)
                    {
                        case SyncProperty.All:
                            ClientManager.AddOperation(new Operation(Command.Transform, identity, localScale, position, rotation)
                            {
                                cmd1 = (byte)mode,
                                index1 = ClientManager.UID
                            });
                            break;
                        case SyncProperty.position:
                            ClientManager.AddOperation(new Operation(Command.Transform, identity)
                            {
                                position = position,
                                cmd1 = (byte)mode,
                                index1 = ClientManager.UID
                            });
                            break;
                        case SyncProperty.rotation:
                            ClientManager.AddOperation(new Operation(Command.Transform, identity)
                            {
                                rotation = rotation,
                                cmd1 = (byte)mode,
                                index1 = ClientManager.UID
                            });
                            break;
                        case SyncProperty.localScale:
                            ClientManager.AddOperation(new Operation(Command.Transform, identity)
                            {
                                direction = localScale,
                                cmd1 = (byte)mode,
                                index1 = ClientManager.UID
                            });
                            break;
                        case SyncProperty.position_rotation:
                            ClientManager.AddOperation(new Operation(Command.Transform, identity, position, rotation)
                            {
                                cmd1 = (byte)mode,
                                index1 = ClientManager.UID
                            });
                            break;
                        case SyncProperty.position_localScale:
                            ClientManager.AddOperation(new Operation(Command.Transform, identity)
                            {
                                position = position,
                                direction = localScale,
                                cmd1 = (byte)mode,
                                index1 = ClientManager.UID
                            });
                            break;
                        case SyncProperty.rotation_localScale:
                            ClientManager.AddOperation(new Operation(Command.Transform, identity)
                            {
                                rotation = rotation,
                                direction = localScale,
                                cmd1 = (byte)mode,
                                index1 = ClientManager.UID
                            });
                            break;
                    }
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