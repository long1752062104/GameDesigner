namespace GameDesigner
{
    using UnityEngine;

    /// <summary>
    /// 连接行为--用户可以继承此类添加组件 2017年12月6日(星期三)
    /// </summary>
    public abstract class TransitionBehaviour : IBehaviour
    {
        /// <summary>
        /// 状态管理器转换组建
        /// </summary>
        public new Transform transform
        {
            get
            {
                return stateManager.transform;
            }
        }

        private Transition _transition;
        /// <summary>
        /// 连接对象
        /// </summary>
        public Transition transition
        {
            get
            {
                if (_transition == null)
                    _transition = GetComponent<Transition>();
                return _transition;
            }
        }

        /// <summary>
        /// 当连接更新
        /// </summary>
        public virtual void OnUpdate(ref bool isEnterNextState) { }
    }
}