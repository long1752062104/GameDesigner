#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Share;
    using System;
    using UnityEngine;

    [Serializable]
    public class ChildTransform
    {
        public string name;
        public Transform transform;
        internal Net.Vector3 position;
        internal Net.Quaternion rotation;
        internal Net.Vector3 localScale;
        public SyncMode mode = SyncMode.Control;
        public SyncProperty property = SyncProperty.All;
        [DisplayOnly] public int identity = -1;//自身id
       
        internal Net.Vector3 netPosition;
        internal Net.Quaternion netRotation;
        internal Net.Vector3 netLocalScale;

        internal void Init(int identity)
        {
            this.identity = identity;
            position = transform.localPosition;
            rotation = transform.localRotation;
            localScale = transform.localScale;
        }

        internal void Check(int identity)
        {
            if (transform.localPosition != position | transform.localRotation != rotation | transform.localScale != localScale)
            {
                position = transform.localPosition;
                rotation = transform.localRotation;
                localScale = transform.localScale;
                switch (property)
                {
                    case SyncProperty.All:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity, localScale, position, rotation)
                        {
                            cmd1 = (byte)mode,
                            index1 = ClientManager.UID,
                            index2 = this.identity
                        });
                        break;
                    case SyncProperty.position:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity)
                        {
                            position = position,
                            cmd1 = (byte)mode,
                            index1 = ClientManager.UID,
                            index2 = this.identity
                        });
                        break;
                    case SyncProperty.rotation:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity)
                        {
                            rotation = rotation,
                            cmd1 = (byte)mode,
                            index1 = ClientManager.UID,
                            index2 = this.identity
                        });
                        break;
                    case SyncProperty.localScale:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity)
                        {
                            direction = localScale,
                            cmd1 = (byte)mode,
                            index1 = ClientManager.UID,
                            index2 = this.identity
                        });
                        break;
                    case SyncProperty.position_rotation:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity, position, rotation)
                        {
                            cmd1 = (byte)mode,
                            index1 = ClientManager.UID,
                            index2 = this.identity
                        });
                        break;
                    case SyncProperty.position_localScale:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity)
                        {
                            position = position,
                            direction = localScale,
                            cmd1 = (byte)mode,
                            index1 = ClientManager.UID,
                            index2 = this.identity
                        });
                        break;
                    case SyncProperty.rotation_localScale:
                        ClientManager.AddOperation(new Operation(Command.Transform, identity)
                        {
                            rotation = rotation,
                            direction = localScale,
                            cmd1 = (byte)mode,
                            index1 = ClientManager.UID,
                            index2 = this.identity
                        });
                        break;
                }
            }
        }

        public void SyncTransform()
        {
            switch (property)
            {
                case SyncProperty.All:
                    transform.localPosition = Vector3.Lerp(transform.localPosition, netPosition, 0.3f);
                    if (netRotation != Net.Quaternion.identity)
                        transform.localRotation = Quaternion.Lerp(transform.localRotation, netRotation, 0.3f);
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.position:
                    transform.localPosition = Vector3.Lerp(transform.localPosition, netPosition, 0.3f);
                    break;
                case SyncProperty.rotation:
                    if (netRotation != Net.Quaternion.identity)
                        transform.localRotation = Quaternion.Lerp(transform.localRotation, netRotation, 0.3f);
                    break;
                case SyncProperty.localScale:
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.position_rotation:
                    transform.localPosition = Vector3.Lerp(transform.localPosition, netPosition, 0.3f);
                    if (netRotation != Net.Quaternion.identity)
                        transform.localRotation = Quaternion.Lerp(transform.localRotation, netRotation, 0.3f);
                    break;
                case SyncProperty.position_localScale:
                    transform.localPosition = Vector3.Lerp(transform.localPosition, netPosition, 0.3f);
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.rotation_localScale:
                    if (netRotation != Net.Quaternion.identity)
                        transform.localRotation = Quaternion.Lerp(transform.localRotation, netRotation, 0.3f);
                    transform.localScale = netLocalScale;
                    break;
            }
        }

        public void SyncControlTransform()
        {
            switch (property)
            {
                case SyncProperty.All:
                    transform.localPosition = netPosition;
                    transform.localRotation = netRotation;
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.position:
                    transform.localPosition = netPosition;
                    break;
                case SyncProperty.rotation:
                    transform.localRotation = netRotation;
                    break;
                case SyncProperty.localScale:
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.position_rotation:
                    transform.localPosition = netPosition;
                    transform.localRotation = netRotation;
                    break;
                case SyncProperty.position_localScale:
                    transform.localPosition = netPosition;
                    transform.localScale = netLocalScale;
                    break;
                case SyncProperty.rotation_localScale:
                    transform.localRotation = netRotation;
                    transform.localScale = netLocalScale;
                    break;
            }
        }
    }

    [ExecuteInEditMode]
    public class NetworkTransform : NetworkTransformBase
    {
        public bool getChilds;
        public ChildTransform[] childs;

        public override void Start()
        {
            base.Start();
            if (identity == -1)
                return;
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].Init(i + 1);
            }
        }

        // Update is called once per frame
        public override void Update()
        {
#if UNITY_EDITOR
            if (getChilds) 
            {
                getChilds = false;
                var childs1 = transform.GetComponentsInChildren<Transform>();
                childs = new ChildTransform[childs1.Length - 1];
                for (int i = 1; i < childs1.Length; i++)
                {
                    childs[i - 1] = new ChildTransform() {
                        name = childs1[i].name,
                        transform = childs1[i]
                    };
                }
            }
#endif
            if (mode == SyncMode.Synchronized)
            {
                SyncTransform();
                for (int i = 0; i < childs.Length; i++)
                {
                    childs[i].SyncTransform();
                }
            }
            else if (Time.time > sendTime)
            {
                Check();
                for (int i = 0; i < childs.Length; i++)
                {
                    childs[i].Check(identity);
                }
                sendTime = Time.time + (1f / rate);
            }
        }
    }
}
#endif