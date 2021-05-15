namespace GameDesigner
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 状态执行管理类
    /// V2017.12.6
    /// 版本修改V2019.8.27
    /// </summary>
    public sealed class StateManager : MonoBehaviour
    {
        /// <summary>
        /// 状态机
        /// </summary>
		public StateMachine stateMachine = null;
        /// <summary>
        /// 旧版动画组件
        /// </summary>
		public new Animation animation = null;
        /// <summary>
        /// 新版动画组件
        /// </summary>
        public Animator animator = null;
        /// <summary>
        /// 动画剪辑
        /// </summary>
        public List<string> clipNames = new List<string>();

        void Awake()
        {
            animation = GetComponent<Animation>();
            animator = GetComponent<Animator>();
            if (stateMachine == null)
            {
                enabled = false;
                return;
            }
            if (stateMachine.GetComponentInParent<StateManager>() == null)
            {//当使用本地公用状态机时
                StateMachine sm = Instantiate(stateMachine);
                sm.name = stateMachine.name;
                sm.transform.SetParent(transform);
                sm.transform.localPosition = Vector3.zero;
                stateMachine = sm;
            }
        }

        void Start()
        {
            if (stateMachine != null)
            {
                if (stateMachine.defaultState.actionSystem)
                    stateMachine.defaultState.OnEnterState();
            }
        }

        private void Update()
        {
            if (stateMachine.currState == null)
                return;
            OnState(stateMachine.currState);
        }

        /// <summary>
        /// 处理状态各种行为与事件方法
        /// </summary>
        /// <param name="state">要执行的状态</param>
        public void OnState(State state)
        {
            if (state.actionSystem)
            {
                try
                {
                    state.OnUpdateState(this, state);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("系统动作出现异常,为了不影响性能,系统即将自动关闭系统动作 == > " + e);
                    state.actionSystem = false;
                }
            }
            for (int i = 0; i < state.behaviours.Count; ++i) //用户自定义脚本行为
                if (state.behaviours[i].Active)
                    state.behaviours[i].OnUpdate(state, null);
            for (int i = 0; i < state.transitions.Count; ++i)
                OnTransition(state.transitions[i]);
        }

        /// <summary>
        /// 处理连接行为线条方法
        /// </summary>
        /// <param name="transition">要执行的连接线条</param>
        public void OnTransition(Transition transition)
        {
            for (int i = 0; i < transition.behaviours.Count; ++i)
                if (transition.behaviours[i].Active)
                    transition.behaviours[i].OnUpdate(ref transition.isEnterNextState);
            if (transition.model == TransitionModel.ExitTime)
            {
                transition.time += Time.deltaTime;
                if (transition.time > transition.exitTime)
                    transition.isEnterNextState = true;
            }
            if (transition.isEnterNextState)
            {
                OnEnterNextState(stateMachine.currState, transition.nextState);
                transition.time = 0;
                transition.isEnterNextState = false;
            }
        }

        /// <summary>
        /// 当退出状态时处理连接事件
        /// </summary>
        /// <param name="state">要退出的状态</param>
        public void OnStateTransitionExit(State state)
        {
            foreach (Transition transition in state.transitions)
                foreach (TransitionBehaviour behaviour in transition.behaviours)
                    if (transition.model == TransitionModel.ExitTime)
                        transition.time = 0;
        }

        /// <summary>
        /// 当进入下一个状态
        /// </summary>
        /// <param name="currState">当前状态</param>
        /// <param name="enterState">要进入的状态</param>
        public void OnEnterNextState(State currState, State enterState)
        {
            foreach (StateBehaviour behaviour in currState.behaviours)//先退出当前的所有行为状态OnExitState的方法
                if (behaviour.Active)
                    behaviour.OnExit(currState, enterState);
            OnStateTransitionExit(currState);
            foreach (StateBehaviour behaviour in enterState.behaviours)//最后进入新的状态前调用这个新状态的所有行为类的OnEnterState方法
                if (behaviour.Active)
                    behaviour.OnEnter(enterState, null);
            if (currState.actionSystem)
                currState.OnExitState();
            if (enterState.actionSystem)
                enterState.OnEnterState();
            stateMachine.stateIndex = enterState.stateID;
        }

        /// <summary>
        /// 当进入下一个状态, 你也可以立即进入当前播放的状态, 如果不想进入当前播放的状态, 使用StatusEntry方法
        /// </summary>
        /// <param name="nextStateIndex">下一个状态的ID</param>
		public void OnEnterNextState(int nextStateIndex)
        {
            foreach (StateBehaviour behaviour in stateMachine.currState.behaviours)//先退出当前的所有行为状态OnExitState的方法
                if (behaviour.Active)
                    behaviour.OnExit(stateMachine.currState, stateMachine.states[nextStateIndex]);
            OnStateTransitionExit(stateMachine.states[nextStateIndex]);
            foreach (StateBehaviour behaviour in stateMachine.states[nextStateIndex].behaviours)//最后进入新的状态前调用这个新状态的所有行为类的OnEnterState方法
                if (behaviour.Active)
                    behaviour.OnEnter(stateMachine.states[nextStateIndex], null);
            if (stateMachine.currState.actionSystem)
                stateMachine.currState.OnExitState();
            if (stateMachine.states[nextStateIndex].actionSystem)
                stateMachine.states[nextStateIndex].OnEnterState();
            stateMachine.stateIndex = nextStateIndex;
        }

        /// <summary>
        /// 进入下一个状态, 如果状态正在播放就不做任何处理, 如果想让动作立即播放可以使用 OnEnterNextState 方法
        /// </summary>
        /// <param name="stateID"></param>
        public void StatusEntry(int stateID)
        {
            if (stateMachine.stateIndex == stateID)
                return;
            OnEnterNextState(stateID);
        }
    }
}