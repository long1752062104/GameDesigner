#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Client;
    using Net.Share;
    using UnityEngine;

    /// <summary>
    /// 场景管理组件, 这个组件负责 同步玩家操作, 玩家退出游戏移除物体对象, 怪物网络行为同步, 攻击同步等
    /// </summary>
    public class SceneManager : NetBehaviour
    {
        public NetworkTransformBase demo;
        public MyDictionary<int, NetworkTransformBase> transforms = new MyDictionary<int, NetworkTransformBase>();

        public virtual void Start()
        {
            ClientManager.Instance.client.OnOperationSync += OnOperationSync;
        }

        /// <summary>
        /// 当网络操作同步时调用
        /// </summary>
        /// <param name="list"></param>
        public virtual void OnOperationSync(OperationList list)
        {
            foreach (var opt in list.operations)
            {
                switch (opt.cmd)
                {
                    case Command.Transform:
                        TransformSync(opt);
                        break;
                    case Command.Destroy:
                        if (transforms.TryGetValue(opt.index, out NetworkTransformBase t))
                        {
                            Destroy(t.gameObject);
                            transforms.Remove(opt.index);
                        }
                        break;
                    default:
                        OnOperationOther(opt);
                        break;
                }
            }
        }

        public virtual void OnOperationOther(Operation opt) 
        {
        }

        protected void TransformSync(Operation opt)
        {
            if (!transforms.TryGetValue(opt.index, out NetworkTransformBase t))
            {
                t = Instantiate(demo, opt.position, opt.rotation);
                SyncMode mode = (SyncMode)opt.cmd1;
                if(mode == SyncMode.Control)
                    t.syncMode = SyncMode.SynchronizedAll;
                else
                    t.syncMode = SyncMode.Synchronized;
                t.identity = opt.index;
                transforms.Add(opt.index, t);
                NetworkTransformBase.Identity++;
            }
            if (ClientManager.UID == opt.index1)
                return;
            t.sendTime = Time.time + t.interval;
            if (opt.index2 == 0)
            {
                t.netPosition = opt.position;
                t.netRotation = opt.rotation;
                t.netLocalScale = opt.direction;
                if (t.mode == SyncMode.SynchronizedAll | t.mode == SyncMode.Control)
                    t.SyncControlTransform();
            }
            else 
            {
                var nt = t as NetworkTransform;
                var child = nt.childs[opt.index2 - 1];
                child.netPosition = opt.position;
                child.netRotation = opt.rotation;
                child.netLocalScale = opt.direction;
                if (child.mode == SyncMode.SynchronizedAll | child.mode == SyncMode.Control)
                    child.SyncControlTransform();
            }
        }

        void OnDestroy()
        {
            if (ClientManager.Instance == null)
                return;
            ClientManager.Instance.client.OnOperationSync -= OnOperationSync;
        }
    }
}
#endif