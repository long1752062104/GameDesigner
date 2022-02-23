#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using Net.Component;
    using Net.Share;
    using global::System;
    using UnityEngine;

    /// <summary>
    /// 网络Transform同步组件基类
    /// </summary>
    public class NetworkTransformMulti : NetworkTransformBase
    {
        public ChildTransform[] childs;

        public void Start()
        {
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].Init(i + 1);
            }
        }

        public override void Update()
        {
            if (netObj.m_identity == -1)
                return;
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
                    childs[i].Check(netObj.m_identity, netObj.registerObjectIndex);
                }
                sendTime = Time.time + (1f / rate);
            }
        }

        public override void StartSyncTransformState()
        {
            ClientManager.AddOperation(new Operation(Command.Transform, netObj.m_identity, syncScale ? localScale : Net.Vector3.zero, syncPosition ? position : Net.Vector3.zero, syncRotation ? rotation : Net.Quaternion.zero)
            {
                cmd1 = (byte)mode,
                index = netObj.registerObjectIndex,
                uid = ClientManager.UID
            });
        }

        public override void OnNetworkOperationHandler(Operation opt)
        {
            if (ClientManager.UID == opt.uid | opt.cmd != Command.Transform)
                return;
            sendTime = Time.time + interval;
            if (opt.index1 == 0)
            {
                netPosition = opt.position;
                netRotation = opt.rotation;
                netLocalScale = opt.direction;
                if (mode == SyncMode.SynchronizedAll | mode == SyncMode.Control)
                    SyncControlTransform();
            }
            else
            {
                var child = childs[opt.index1 - 1];
                child.netPosition = opt.position;
                child.netRotation = opt.rotation;
                child.netLocalScale = opt.direction;
                if (child.mode == SyncMode.SynchronizedAll | child.mode == SyncMode.Control)
                    child.SyncControlTransform();
            }
        }
    }

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
            if (syncPosition)
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
}
#endif