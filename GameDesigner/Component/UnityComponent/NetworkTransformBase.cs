#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using Net.Component;
    using Net.Share;
    using UnityEngine;

    /// <summary>
    /// 网络Transform同步组件基类
    /// </summary>
    public abstract class NetworkTransformBase : NetworkBehaviour
    {
        protected Net.Vector3 position;
        protected Net.Quaternion rotation;
        protected Net.Vector3 localScale;
        public SyncMode syncMode = SyncMode.Control;
        public bool syncPosition = true;
        public bool syncRotation = true;
        public bool syncScale = false;
        [DisplayOnly]
        public SyncMode mode = SyncMode.Synchronized;
        internal float sendTime;
        public float interval = 0.5f;
        internal Net.Vector3 netPosition;
        internal Net.Quaternion netRotation;
        internal Net.Vector3 netLocalScale;
        public float rate = 30f;//网络帧率, 一秒30次
        public float lerpSpeed = 0.3f;
        public bool fixedSync = true;
        public float fixedSendTime = 1f;//固定发送时间
        internal float fixedTime;

        // Update is called once per frame
        public virtual void Update()
        {
            if (networkIdentity.identity == -1)
                return;
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
            if (transform.position != position | transform.rotation != rotation | transform.localScale != localScale | (Time.time > fixedTime & fixedSync))
            {
                position = transform.position;
                rotation = transform.rotation;
                localScale = transform.localScale;
                fixedTime = Time.time + fixedSendTime;
                StartSyncTransformState();
            }
        }

        public virtual void StartSyncTransformState()
        {
            ClientManager.AddOperation(new Operation(Command.Transform, networkIdentity.identity, syncScale ? localScale : Net.Vector3.zero, syncPosition ? position : Net.Vector3.zero, syncRotation ? rotation : Net.Quaternion.zero)
            {
                cmd1 = (byte)mode,
                index = networkIdentity.registerObjectIndex,
                uid = ClientManager.UID
            });
        }

        public virtual void SyncTransform()
        {
            if (syncPosition)
                transform.position = Vector3.Lerp(transform.position, netPosition, lerpSpeed);
            if (syncRotation)
                if (netRotation != Net.Quaternion.identity)
                    transform.rotation = Quaternion.Lerp(transform.rotation, netRotation, lerpSpeed);
            if (syncScale)
                transform.localScale = netLocalScale;
        }

        public virtual void SyncControlTransform()
        {
            if (syncPosition)
            {
                position = netPosition;//位置要归位,要不然就会发送数据
                transform.position = netPosition;
            }
            if (syncRotation)
            {
                rotation = netRotation;
                transform.rotation = netRotation;
            }
            if (syncScale)
            {
                localScale = netLocalScale;
                transform.localScale = netLocalScale;
            }
        }

        public override void OnNetworkObjectInit(int identity)
        {
            mode = syncMode;
        }

        public override void OnNetworkObjectCreate(Operation opt)
        {
            SyncMode mode1 = (SyncMode)opt.cmd1;
            if (mode1 == SyncMode.Control | mode1 == SyncMode.SynchronizedAll)
                mode = SyncMode.SynchronizedAll;
            else
                mode = SyncMode.Synchronized;
        }

        public override void OnNetworkOperationHandler(Operation opt)
        {
            if (ClientManager.UID == opt.uid | opt.cmd != Command.Transform)
                return;
            sendTime = Time.time + interval;
            netPosition = opt.position;
            netRotation = opt.rotation;
            netLocalScale = opt.direction;
            if (mode == SyncMode.SynchronizedAll | mode == SyncMode.Control)
                SyncControlTransform();
        }
    }
}
#endif