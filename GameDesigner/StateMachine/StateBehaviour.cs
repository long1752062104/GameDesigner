using System.Collections.Generic;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 状态行为脚本 v2017/12/6 - 2020.6.7
    /// </summary>
    public abstract class StateBehaviour : IBehaviour
    {
        /// <summary>
        /// 状态管理器转换组建
        /// </summary>
        public new Transform transform { get { return stateManager.transform; } }

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
    }
}