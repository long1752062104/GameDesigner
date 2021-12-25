#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Share;
    using global::System;
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
        public bool syncPosition = true;
        public bool syncRotation = true;
        public bool syncScale = false;
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

        internal void Check(int identity, int index)
        {
            if (transform.localPosition != position | transform.localRotation != rotation | transform.localScale != localScale)
            {
                position = transform.localPosition;
                rotation = transform.localRotation;
                localScale = transform.localScale;
                ClientManager.AddOperation(new Operation(Command.Transform, identity, syncScale ? localScale : Net.Vector3.zero, syncPosition ? position : Net.Vector3.zero, syncRotation ? rotation : Net.Quaternion.zero)
                {
                    cmd1 = (byte)mode,
                    uid = ClientManager.UID,
                    index = index,
                    index1 = this.identity
                });
            }
        }

        public void SyncTransform()
        {
            if(syncPosition)
                transform.localPosition = Vector3.Lerp(transform.localPosition, netPosition, 0.3f);
            if (syncRotation)
                if (netRotation != Net.Quaternion.identity)
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, netRotation, 0.3f);
            if (syncScale)
                transform.localScale = netLocalScale;
        }

        public void SyncControlTransform()
        {
            if (syncPosition)
                transform.localPosition = netPosition;
            if (syncRotation)
                transform.localRotation = netRotation;
            if (syncScale)
                transform.localScale = netLocalScale;
        }
    }

    /// <summary>
    /// 网络Transform同步组件, 可以同步子物体
    /// </summary>
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
                    childs[i].Check(identity, index);
                }
                sendTime = Time.time + (1f / rate);
            }
        }
    }
}
#endif