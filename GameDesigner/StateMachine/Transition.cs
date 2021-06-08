namespace GameDesigner
{
    using System.Collections.Generic;

    /// <summary>
    /// 状态连接组件 2017年12月6日
    /// 版本修改2019.8.27
    /// </summary>
    [System.Serializable]
    public sealed class Transition : IState
    {
        public int currStateID, nextStateID;
        /// <summary>
        /// 当前状态
        /// </summary>
		public State currState { 
            get {
                foreach (var item in stateMachine.states)
                    if (item.ID == currStateID)
                        return item;
                return null;
            } 
        }
        /// <summary>
        /// 下一个状态
        /// </summary>
		public State nextState {
            get
            {
                foreach (var item in stateMachine.states)
                    if (item.ID == nextStateID)
                        return item;
                return null;
            }
        }
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

        public Transition() { }

        /// <summary>
        /// 创建连接实例
        /// </summary>
        /// <param name="state">连接的开始状态</param>
        /// <param name="nextState">连接的结束状态</param>
        /// <param name="transitionName">连接名称</param>
        /// <returns></returns>
		public static Transition CreateTransitionInstance(State state, State nextState, string transitionName = "New Transition")
        {
            Transition t = new Transition();
            t.name = transitionName;
            t.currStateID = state.ID;
            t.nextStateID = nextState.ID;
            t.stateMachine = state.stateMachine;
            state.transitions.Add(t);
            for (int i = 0; i < state.transitions.Count; i++)
                state.transitions[i].ID = i;
            return t;
        }
    }
}