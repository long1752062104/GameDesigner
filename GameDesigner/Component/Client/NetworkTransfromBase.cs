﻿#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Share;
    using UnityEngine;

    public enum SyncMode
    {
        Local,
        Control,
        Authorize,
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

    public abstract class NetworkTransformBase : MonoBehaviour
    {
        internal static int Identity = 0;
        protected Net.Vector3 position;
        protected Net.Quaternion rotation;
        protected Net.Vector3 localScale;
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
        public byte index;//在SceneManager的prefabs数组的索引
        public bool setID;
        public int m_identity;
        public float rate = 30f;//网络帧率, 一秒30次
        public float lerpSpeed = 0.3f;

        public virtual void Start()
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
            if (setID) 
            {
                identity = m_identity;
                if (!sm.transforms.ContainsKey(identity))
                    sm.transforms.Add(identity, this);
                return;
            }
        JUMP: identity = Identity++;
            if (sm.transforms.ContainsKey(identity))
                goto JUMP;
            sm.transforms.Add(identity, this);
        }

        // Update is called once per frame
        public virtual void Update()
        {
            if (mode == SyncMode.Synchronized)
            {
                SyncTransform();
            }
            else if (Time.time > sendTime)
            {
                Check();
                sendTime = Time.time + (1f / rate);
            }
        }

        public virtual void Check()
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
                            cmd2 = index,
                            index1 = ClientManager.UID
                        });
                        break;
                    case SyncProperty.position:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity)
                        {
                            position = position,
                            cmd1 = (byte)mode,
                            cmd2 = index,
                            index1 = ClientManager.UID
                        });
                        break;
                    case SyncProperty.rotation:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity)
                        {
                            rotation = rotation,
                            cmd1 = (byte)mode,
                            cmd2 = index,
                            index1 = ClientManager.UID
                        });
                        break;
                    case SyncProperty.localScale:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity)
                        {
                            direction = localScale,
                            cmd1 = (byte)mode,
                            cmd2 = index,
                            index1 = ClientManager.UID
                        });
                        break;
                    case SyncProperty.position_rotation:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity, position, rotation)
                        {
                            cmd1 = (byte)mode,
                            cmd2 = index,
                            index1 = ClientManager.UID
                        });
                        break;
                    case SyncProperty.position_localScale:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity)
                        {
                            position = position,
                            direction = localScale,
                            cmd1 = (byte)mode,
                            cmd2 = index,
                            index1 = ClientManager.UID
                        });
                        break;
                    case SyncProperty.rotation_localScale:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity)
                        {
                            rotation = rotation,
                            direction = localScale,
                            cmd1 = (byte)mode,
                            cmd2 = index,
                            index1 = ClientManager.UID
                        });
                        break;
                }
            }
        }

        public virtual void SyncTransform()
        {
            switch (property)
            {
                case SyncProperty.All:
                    transform.position = Vector3.Lerp(transform.position, netPosition, lerpSpeed);
                    if (netRotation != Net.Quaternion.identity)
                        transform.rotation = Quaternion.Lerp(transform.rotation, netRotation, lerpSpeed);
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.position:
                    transform.position = Vector3.Lerp(transform.position, netPosition, lerpSpeed);
                    break;
                case SyncProperty.rotation:
                    if (netRotation != Net.Quaternion.identity)
                        transform.rotation = Quaternion.Lerp(transform.rotation, netRotation, lerpSpeed);
                    break;
                case SyncProperty.localScale:
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.position_rotation:
                    transform.position = Vector3.Lerp(transform.position, netPosition, lerpSpeed);
                    if (netRotation != Net.Quaternion.identity)
                        transform.rotation = Quaternion.Lerp(transform.rotation, netRotation, lerpSpeed);
                    break;
                case SyncProperty.position_localScale:
                    transform.position = Vector3.Lerp(transform.position, netPosition, lerpSpeed);
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.rotation_localScale:
                    if (netRotation != Net.Quaternion.identity)
                        transform.rotation = Quaternion.Lerp(transform.rotation, netRotation, lerpSpeed);
                    transform.localScale = netLocalScale;
                    break;
            }
        }

        public virtual void SyncControlTransform()
        {
            switch (property)
            {
                case SyncProperty.All:
                    transform.position = netPosition;
                    transform.rotation = netRotation;
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.position:
                    transform.position = netPosition;
                    break;
                case SyncProperty.rotation:
                    transform.rotation = netRotation;
                    break;
                case SyncProperty.localScale:
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.position_rotation:
                    transform.position = netPosition;
                    transform.rotation = netRotation;
                    break;
                case SyncProperty.position_localScale:
                    transform.position = netPosition;
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.rotation_localScale:
                    transform.rotation = netRotation;
                    transform.localScale = netLocalScale;
                    break;
            }
        }

        public virtual void OnDestroy()
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