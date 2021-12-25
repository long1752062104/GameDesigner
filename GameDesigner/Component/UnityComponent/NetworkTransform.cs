#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.UnityComponent
{
    using Net.Component;
    using Net.Share;
    using UnityEngine;

    /// <summary>
    /// 网络Transform同步组件基类
    /// </summary>
    public class NetworkTransform : NetworkBehaviour
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
        internal float checkStatusTime;//检测其他客户端是否断线, 如果断线则把断线的客户端所同步的transform组件删除

        // Update is called once per frame
        public virtual void Update()
        {
            if (networkIdentity.identity == -1)
                return;
            if (mode == SyncMode.Synchronized)
            {
                SyncTransform();
                if (fixedSync)
                {
                    if (Time.time > checkStatusTime)
                    {
                        SceneManager.I.transforms.Remove(networkIdentity.identity);
                        Destroy(gameObject);
                    }
                }
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
                cmd2 = (byte)networkIdentity.registerObjectIndex,
                index1 = ClientManager.UID
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

        public override void OnDestroy()
        {
            if (ClientManager.Instance == null)
                return;
            switch (mode)
            {
                case SyncMode.Synchronized:
                case SyncMode.SynchronizedAll:
                    return;
            }
            ClientManager.AddOperation(new Operation(Command.Destroy) { identity = networkIdentity.identity });
        }

        public override void OnNetworkIdentityInit(int id, int newId)
        {
            mode = syncMode;
            switch (mode)
            {
                case SyncMode.Synchronized:
                case SyncMode.SynchronizedAll:
                    return;
            }
        }

        public override void OnNetworkIdentityCreate(Operation opt)
        {
            SyncMode mode = (SyncMode)opt.cmd1;
            if (mode == SyncMode.Control)
                syncMode = SyncMode.SynchronizedAll;
            else
                syncMode = SyncMode.Synchronized;
        }

        public override void OnNetworkOperationHandler(Operation opt)
        {
            if (ClientManager.UID == opt.uid | opt.cmd != Command.Transform)
                return;
            if (mode == SyncMode.SynchronizedAll & Time.time < fixedTime)
                return;
            sendTime = Time.time + interval;
            checkStatusTime = Time.time + fixedSendTime * 3f;
            netPosition = opt.position;
            netRotation = opt.rotation;
            netLocalScale = opt.direction;
            if (mode == SyncMode.SynchronizedAll | mode == SyncMode.Control)
                SyncControlTransform();
        }
    }
}
#endif