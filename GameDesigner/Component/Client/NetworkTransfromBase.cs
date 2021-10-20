#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Share;
    using global::System.Collections.Generic;
    using UnityEngine;

    public enum SyncMode
    {
        /// <summary>
        /// 自身同步, 只有自身才能控制, 同步给其他客户端, 其他客户端无法控制这个物体的移动
        /// </summary>
        Local,
        /// <summary>
        /// 完全控制, 所有客户端都可以移动这个物体, 并且其他客户端都会被同步
        /// 同步条件是哪个先移动这个物体会有<see cref="NetworkTransformBase.interval"/>秒完全控制,
        /// 其他客户端无法控制,如果先移动的客户端一直移动这个物体,则其他客户端无法移动,只有先移动的客户端停止操作,下个客户端才能同步这个物体
        /// </summary>
        Control,
        /// <summary>
        /// 无效
        /// </summary>
        Authorize,
        /// <summary>
        /// 自身同步在其他客户端显示的状态
        /// </summary>
        Synchronized,
        /// <summary>
        /// 完全控制在其他客户端显示的状态
        /// </summary>
        SynchronizedAll
    }

    /// <summary>
    /// 网络Transform同步组件基类
    /// </summary>
    public abstract class NetworkTransformBase : MonoBehaviour
    {
        /// <summary>
        /// 实例化的物体带有<see cref="TransformComponent"/>组件的第一个唯一标识是从此字段开始计数++ (默认是0开始)
        /// </summary>
        public static int Identity = 0;
        protected Net.Vector3 position;
        protected Net.Quaternion rotation;
        protected Net.Vector3 localScale;
        public SyncMode syncMode = SyncMode.Control;
        public bool syncPosition = true;
        public bool syncRotation = true;
        public bool syncScale = false;
        [DisplayOnly]
        public SyncMode mode = SyncMode.Synchronized;
        [DisplayOnly]
        public int identity = -1;//自身id
        internal float sendTime;
        public float interval = 0.5f;
        internal Net.Vector3 netPosition;
        internal Net.Quaternion netRotation;
        internal Net.Vector3 netLocalScale;
        [Tooltip("对应SceneManager组件的prefabs数组元素索引")]
        public byte index;//在SceneManager的prefabs数组的索引
        [Tooltip("设置自定义的唯一ID,注意:不要让唯一ID冲突,否则会出现问题!")]
        public int m_identity;
        public float rate = 30f;//网络帧率, 一秒30次
        public float lerpSpeed = 0.3f;
        public bool fixedSync;
        public float fixedSendTime = 1f;//固定发送时间
        private float fixedTime;
        internal List<NetworkAnimation> animations = new List<NetworkAnimation>();
        internal List<NetworkAnimator> animators = new List<NetworkAnimator>();

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
            if (m_identity != 0) 
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
            if (transform.position != position | transform.rotation != rotation | transform.localScale != localScale | (Time.time > fixedTime & fixedSync))
            {
                position = transform.position;
                rotation = transform.rotation;
                localScale = transform.localScale;
                fixedTime = Time.time + fixedSendTime;
                ClientManager.AddOperation(new Operation(Command.Transform, identity, syncScale ? localScale : Net.Vector3.zero, syncPosition ? position : Net.Vector3.zero, syncRotation ? rotation : Net.Quaternion.zero)
                {
                    cmd1 = (byte)mode,
                    cmd2 = index,
                    index1 = ClientManager.UID
                });
            }
        }

        public virtual void SyncTransform()
        {
            if (syncPosition)
                transform.position = Vector3.Lerp(transform.position, netPosition, lerpSpeed);
            if (syncRotation)
                if (netRotation != Net.Quaternion.identity)
                    transform.rotation = Quaternion.Lerp(transform.rotation, netRotation, lerpSpeed);
            if(syncScale)
                transform.localScale = netLocalScale;
        }

        public virtual void SyncControlTransform()
        {
            if (syncPosition)
                transform.position = netPosition;
            if (syncRotation) 
                transform.rotation = netRotation;
            if (syncScale) 
                transform.localScale = netLocalScale;
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