using System.Collections.Generic;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 状态机 v2017/12/6
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
        /// <summary>
        /// 默认状态ID
        /// </summary>
		public int defaulID = 0;
        /// <summary>
        /// 当前运行的状态索引
        /// </summary>
		public int stateID = 0;
        /// <summary>
        /// 所有状态
        /// </summary>
		public List<State> states = new List<State>();
        /// <summary>
        /// 选中的状态,可以多选
        /// </summary>
		public List<int> selectStates = new List<int>();
        /// <summary>
        /// 动画选择模式
        /// </summary>
        public AnimationMode animMode = AnimationMode.Animation;
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

        /// <summary>
        /// 以状态ID取出状态对象
        /// </summary>
        /// <param name="stateID"></param>
        /// <returns></returns>
        public State this[int stateID]
        {
            get
            {
                return states[stateID];
            }
        }

        /// <summary>
        /// 获取 或 设置 默认状态
        /// </summary>
        public State defaultState
        {
            get
            {
                if (defaulID < states.Count)
                    return states[defaulID];
                return null;
            }
            set { defaulID = value.ID; }
        }

        /// <summary>
        /// 当前状态
        /// </summary>
		public State currState => states[stateID];

        /// <summary>
        /// 选择的状态
        /// </summary>
		public State selectState
        {
            get
            {
                if (selectStates.Count > 0)
                    return states[selectStates[0]];
                return null;
            }
            set
            {
                if (!selectStates.Contains(value.ID))
                    selectStates.Add(value.ID);
            }
        }

        [SerializeField]
        private StateManager _stateManager = null;
        /// <summary>
        /// 状态管理
        /// </summary>
		public StateManager stateManager
        {
            get
            {
                if (_stateManager == null)
                    _stateManager = transform.GetComponentInParent<StateManager>();
                return _stateManager;
            }
            set { _stateManager = value; }
        }

        /// <summary>
        /// 创建状态机实例
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns></returns>
        public static StateMachine CreateStateMachineInstance(string name = "machine")
        {
            StateMachine stateMachine = new GameObject(name).AddComponent<StateMachine>();
            stateMachine.name = name;
            return stateMachine;
        }

        public void UpdateStates()
        {
            for (int i = 0; i < states.Count; i++)
            {
                int id = states[i].ID;
                foreach (var state1 in states)
                {
                    foreach (var transition in state1.transitions)
                    {
                        if (transition.currStateID == id)
                            transition.currStateID = i;
                        if (transition.nextStateID == id)
                            transition.nextStateID = i;
                    }
                    foreach (var behaviour in state1.behaviours)
                    {
                        if (behaviour.ID == id)
                            behaviour.ID = i;
                    }
                    foreach (var action in state1.actions)
                    {
                        foreach (var behaviour in action.behaviours)
                        {
                            if (behaviour.ID == id)
                                behaviour.ID = i;
                        }
                    }
                }
                states[i].ID = i;
            }
        }
    }
}