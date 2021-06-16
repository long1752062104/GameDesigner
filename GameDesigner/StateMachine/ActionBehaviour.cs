namespace GameDesigner
{
    using System;
    using UnityEngine;

    /// <summary>
    /// 动作行为--用户添加的组件 v2017/12/6
    /// </summary>
    [Serializable]
    public class ActionBehaviour : IBehaviour
    {
        /// <summary>
        /// 当进入状态
        /// </summary>
        /// <param name="action">当前动作</param>
        public virtual void OnEnter(StateAction action) { }

        /// <summary>
        /// 当更新状态
        /// </summary>
        /// <param name="action">当前动作</param>
        public virtual void OnUpdate(StateAction action) { }

        /// <summary>
        /// 当退出状态
        /// </summary>
        /// <param name="action">当前动作</param>
        public virtual void OnExit(StateAction action) { }

        /// <summary>
        /// 当停止动作 : 当动作不使用动画循环时, 动画时间到达100%后调用
        /// </summary>
        /// <param name="action"></param>
        public virtual void OnStop(StateAction action) { }

        /// <summary>
        /// 当进入动画事件
        /// </summary>
        /// <param name="action">当前动作</param>
        /// <param name="animEventTime">动画事件时间</param>
        public virtual void OnAnimationEvent(StateAction action, float animEventTime) { }

        /// <summary>
        /// 当实例化技能物体
        /// </summary>
        /// <param name="action">当前动作</param>
        /// <param name="spwan">技能物体</param>
        public virtual void OnInstantiateSpwan(StateAction action, GameObject spwan) { }

        public ActionBehaviour InitBehaviour()
        {
            var type = SystemType.GetType(name);
            var runtimeBehaviour = (ActionBehaviour)Activator.CreateInstance(type);
            runtimeBehaviour.stateMachine = stateMachine;
            runtimeBehaviour.Active = Active;
            runtimeBehaviour.ID = ID;
            runtimeBehaviour.name = name;
            runtimeBehaviour.metadatas = metadatas;
            foreach (var metadata in metadatas)
            {
                var field = type.GetField(metadata.name);
                if (field == null)
                    continue;
                metadata.field = field;
                metadata.target = runtimeBehaviour;
                field.SetValue(runtimeBehaviour, metadata.Read());
            }
            return runtimeBehaviour;
        }
    }
}