using System;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 状态行为脚本 v2017/12/6 - 2020.6.7
    /// </summary>
    [Serializable]
    public class StateBehaviour : IBehaviour
    {
        /// <summary>
        /// 状态管理器转换组建
        /// </summary>
        public Transform transform { get { return stateManager.transform; } }

        /// <summary>
        /// 当状态进入时
        /// </summary>
        /// <param name="currentState">当前状态</param>
        /// <param name="nextState">下一个状态</param>
        public virtual void OnEnter(State currentState, State nextState) { }

        /// <summary>
        /// 当状态每一帧
        /// </summary>
        /// <param name="currentState">当前状态</param>
        /// <param name="nextState">下一个状态</param>
        public virtual void OnUpdate(State currentState, State nextState) { }

        /// <summary>
        /// 当状态退出后
        /// </summary>
        /// <param name="currentState">当前状态</param>
        /// <param name="nextState">下一个状态</param>
        public virtual void OnExit(State currentState, State nextState) { }

        /// <summary>
        /// 当停止动作 : 当动作不使用动画循环时, 动画时间到达100%后调用
        /// </summary>
        /// <param name="state"></param>
        public virtual void OnStop(State state) { }

        /// <summary>
        /// 当动作处于循环模式时, 子动作动画每次结束都会调用一次
        /// </summary>
        /// <param name="state"></param>
        public virtual void OnActionExit(State state) { }

        private StateBehaviour runtimeBehaviour;
        public StateBehaviour RuntimeBehaviour
        {
            get
            {
                if (runtimeBehaviour == null)
                {
                    var type = SystemType.GetType(name);
                    runtimeBehaviour = (StateBehaviour)Activator.CreateInstance(type);
                    runtimeBehaviour.stateMachine = stateMachine;
                    runtimeBehaviour.Active = Active;
                    runtimeBehaviour.ID = ID;
                    runtimeBehaviour.name = name; 
                    foreach (var metadata in metadatas)
                    {
                        var field = type.GetField(metadata.name);
                        metadata.field = field;
                        metadata.target = runtimeBehaviour;
                        field.SetValue(runtimeBehaviour, metadata.Read());
                    }
                }
                return runtimeBehaviour;
            }
        }
    }
}