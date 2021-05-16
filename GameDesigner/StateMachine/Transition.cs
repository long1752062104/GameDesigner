namespace GameDesigner
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 状态连接组件 2017年12月6日
    /// 版本修改2019.8.27
    /// </summary>
    public sealed class Transition : IState
    {
        /// <summary>
        /// 当前状态
        /// </summary>
		public State currState = null;
        /// <summary>
        /// 下一个状态
        /// </summary>
		public State nextState = null;
        /// <summary>
        /// 连接控制模式
        /// </summary>
		public TransitionModel model = TransitionModel.ScriptControl;
        /// <summary>
        /// 当前时间
        /// </summary>
		public float time = 0;
        /// <summary>
        /// 结束时间
        /// </summary>
		public float exitTime = 1;
        /// <summary>
        /// 连接行为
        /// </summary>
		public List<TransitionBehaviour> behaviours = new List<TransitionBehaviour>();
        /// <summary>
        /// 是否进入下一个状态?
        /// </summary>
		public bool isEnterNextState = false;

        private Transition() { }

        /// <summary>
        /// 创建连接实例
        /// </summary>
        /// <param name="state">连接的开始状态</param>
        /// <param name="nextState">连接的结束状态</param>
        /// <param name="transitionName">连接名称</param>
        /// <returns></returns>
		public static Transition CreateTransitionInstance(State state, State nextState, string transitionName = "New Transition")
        {
            Transition t = new GameObject(transitionName).AddComponent<Transition>();
            t.name = transitionName;
            t.currState = state;
            t.nextState = nextState;
            state.transitions.Add(t);
            t.transform.SetParent(state.transform);
            t.transform.hideFlags = HideFlags.None;
            return t;
        }
    }
}