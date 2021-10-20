#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Share;
    using Net.System;
    using UnityEngine;

    /// <summary>
    /// 场景管理组件, 这个组件负责 同步玩家操作, 玩家退出游戏移除物体对象, 怪物网络行为同步, 攻击同步等
    /// </summary>
    public class SceneManager : SingleCase<SceneManager>
    {
        [Header("TransformComponent组件的index字段值必须设置对应数组元素的索引值!")]
        public NetworkTransformBase[] prefabs;
        [HideInInspector]
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
                        DestroyTransform(opt);
                        break;
                    case Command.Animator:
                        AnimatorSync(opt);
                        break;
                    case Command.AnimatorParameter:
                        AnimatorParameterSync(opt);
                        break;
                    case Command.Animation:
                        AnimationSync(opt);
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

        public virtual void OnCrateTransform(Operation opt, NetworkTransformBase t)
        {
        }

        public virtual void DestroyTransform(Operation opt)
        {
            if (transforms.TryGetValue(opt.index, out NetworkTransformBase t))
                DestroyTransform(opt, t);
        }

        public virtual void DestroyTransform(Operation opt, NetworkTransformBase t)
        {
            transforms.Remove(opt.index);
            OnDestroyTransform(t);
            if (t.syncMode == SyncMode.Synchronized | t.syncMode == SyncMode.SynchronizedAll)
                Destroy(t.gameObject);
        }

        public virtual void OnDestroyTransform(NetworkTransformBase t)
        {
        }

        protected void TransformSync(Operation opt)
        {
            if (!transforms.TryGetValue(opt.index, out NetworkTransformBase t))
            {
                if (prefabs == null)
                    return;
                if (opt.cmd2 >= prefabs.Length)
                    return;
                var prefab = prefabs[opt.cmd2];
                t = Instantiate(prefab, opt.position, opt.rotation);
                SyncMode mode = (SyncMode)opt.cmd1;
                if(mode == SyncMode.Control)
                    t.syncMode = SyncMode.SynchronizedAll;
                else
                    t.syncMode = SyncMode.Synchronized;
                t.identity = opt.index;
                transforms.Add(opt.index, t);
                OnCrateTransform(opt, t);
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
            OnTransformSync(opt);
        }

        /// <summary>
        /// 当同步transform组件调用, opt的index,index1,index2,cmd,cmd1,cmd2,position,rotation,direction已被使用
        /// </summary>
        /// <param name="opt"></param>
        public virtual void OnTransformSync(Operation opt)
        {
        }

        protected void AnimatorSync(Operation opt) 
        {
            if (transforms.TryGetValue(opt.index, out NetworkTransformBase t)) 
                t.animators[opt.index1].Play(opt.index2);
        }

        protected void AnimatorParameterSync(Operation opt)
        {
            if (transforms.TryGetValue(opt.index, out NetworkTransformBase t))
                t.animators[opt.cmd1].SyncAnimatorParameter(opt);
        }

        protected void AnimationSync(Operation opt) 
        {
            if (transforms.TryGetValue(opt.index, out NetworkTransformBase t))
                t.animations[opt.index1].Play(opt.index2);
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