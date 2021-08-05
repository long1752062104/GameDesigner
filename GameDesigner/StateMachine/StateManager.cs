namespace GameDesigner
{
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
        
        void Awake()
        {
            if (stateMachine == null)
            {
                enabled = false;
                return;
            }
            if (stateMachine.GetComponentInParent<StateManager>() == null)//当使用本地公用状态机时
            {
                StateMachine sm = Instantiate(stateMachine, transform);
                sm.name = stateMachine.name;
                sm.transform.localPosition = Vector3.zero;
                if(sm.animation==null)
                    sm.animation = GetComponentInChildren<Animation>();
                else if (!sm.animation.gameObject.scene.isLoaded)
                    sm.animation = GetComponentInChildren<Animation>();
                if (sm.animator == null)
                    sm.animator = GetComponentInChildren<Animator>();
                else if(!sm.animator.gameObject.scene.isLoaded)
                    sm.animator = GetComponentInChildren<Animator>();
                stateMachine = sm;
            }
            foreach (var state in stateMachine.states)
            {
                for (int i = 0; i < state.behaviours.Count; i++)
                {
                    state.behaviours[i] = state.behaviours[i].InitBehaviour();
                    state.behaviours[i].OnInit();
                }
                foreach (var t in state.transitions)
                {
                    for (int i = 0; i < t.behaviours.Count; i++)
                    {
                        t.behaviours[i] = t.behaviours[i].InitBehaviour();
                        t.behaviours[i].OnInit();
                    }
                }
                if (state.actionSystem)
                {
                    foreach (var action in state.actions)
                    {
                        for (int i = 0; i < action.behaviours.Count; i++)
                        {
                            action.behaviours[i] = action.behaviours[i].InitBehaviour();
                            action.behaviours[i].OnInit();
                        }
                    }
                }
            }
            if (stateMachine.defaultState.actionSystem)
                stateMachine.defaultState.OnEnterState();
        }

        private void Update()
        {
            OnState(stateMachine.currState);
        }

        /// <summary>
        /// 处理状态各种行为与事件方法
        /// </summary>
        /// <param name="state">要执行的状态</param>
        public void OnState(State state)
        {
            if (state.actionSystem)
                state.OnUpdateState();
            for (int i = 0; i < state.behaviours.Count; ++i) //用户自定义脚本行为
                if (state.behaviours[i].Active)
                    state.behaviours[i].OnUpdate();
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
                EnterNextState(stateMachine.currState, transition.nextState);
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
                if (transition.model == TransitionModel.ExitTime)
                    transition.time = 0;
        }

        /// <summary>
        /// 当进入下一个状态
        /// </summary>
        /// <param name="currState">当前状态</param>
        /// <param name="enterState">要进入的状态</param>
        public void EnterNextState(State currState, State enterState)
        {
            foreach (StateBehaviour behaviour in currState.behaviours)//先退出当前的所有行为状态OnExitState的方法
                if (behaviour.Active)
                    behaviour.OnExit();
            OnStateTransitionExit(currState);
            foreach (StateBehaviour behaviour in enterState.behaviours)//最后进入新的状态前调用这个新状态的所有行为类的OnEnterState方法
                if (behaviour.Active)
                    behaviour.OnEnter();
            if (currState.actionSystem)
                currState.OnExitState();
            if (enterState.actionSystem)
                enterState.OnEnterState();
            stateMachine.stateID = enterState.ID;
        }

        /// <summary>
        /// 当进入下一个状态, 你也可以立即进入当前播放的状态, 如果不想进入当前播放的状态, 使用StatusEntry方法
        /// </summary>
        /// <param name="nextStateIndex">下一个状态的ID</param>
		public void EnterNextState(int nextStateIndex)
        {
            foreach (StateBehaviour behaviour in stateMachine.currState.behaviours)//先退出当前的所有行为状态OnExitState的方法
                if (behaviour.Active)
                    behaviour.OnExit();
            OnStateTransitionExit(stateMachine.states[nextStateIndex]);
            foreach (StateBehaviour behaviour in stateMachine.states[nextStateIndex].behaviours)//最后进入新的状态前调用这个新状态的所有行为类的OnEnterState方法
                if (behaviour.Active)
                    behaviour.OnEnter();
            if (stateMachine.currState.actionSystem)
                stateMachine.currState.OnExitState();
            if (stateMachine.states[nextStateIndex].actionSystem)
                stateMachine.states[nextStateIndex].OnEnterState();
            stateMachine.stateID = nextStateIndex;
        }

        /// <summary>
        /// 进入下一个状态, 如果状态正在播放就不做任何处理, 如果想让动作立即播放可以使用 OnEnterNextState 方法
        /// </summary>
        /// <param name="stateID"></param>
        public void StatusEntry(int stateID)
        {
            if (stateMachine.stateID == stateID)
                return;
            EnterNextState(stateID);
        }
    }
}