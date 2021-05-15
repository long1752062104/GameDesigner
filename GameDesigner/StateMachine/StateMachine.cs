using System.Collections.Generic;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 编辑器选择状态物体模式
    /// </summary>
	public enum SelectObjMode
    {
        /// <summary>
        /// 不进行如何物体选择
        /// </summary>
		Null,
        /// <summary>
        /// 选择为状态管理物体
        /// </summary>
		SelectionStateManager,
        /// <summary>
        /// 选择为状态机物体
        /// </summary>
		SelectionStateMachine,
        /// <summary>
        /// 选择为状态物体
        /// </summary>
		SelectionStateObject,
    }

    /// <summary>
    /// 状态机 v2017/12/6
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
        /// <summary>
        /// 默认状态ID
        /// </summary>
		public int defaulStateID = 0;
        /// <summary>
        /// 当前运行的状态索引
        /// </summary>
		public int stateIndex = 0;
        /// <summary>
        /// 所有状态
        /// </summary>
		public List<State> states = new List<State>();
        /// <summary>
        /// 选中的状态,可以多选
        /// </summary>
		public List<State> selectStates = new List<State>();
        /// <summary>
        /// 选中的连接线,可以多选
        /// </summary>
		public List<Transition> selectTransitions = new List<Transition>();
        /// <summary>
        /// 动画选择模式
        /// </summary>
        public AnimationMode animMode = AnimationMode.Animation;

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
                if (defaulStateID < states.Count)
                    return states[defaulStateID];
                return null;
            }
            set { defaulStateID = value.stateID; }
        }

        /// <summary>
        /// 当前状态
        /// </summary>
		public State currState
        {
            get
            {
                return states[stateIndex];
            }
        }

        /// <summary>
        /// 选择的状态
        /// </summary>
		public State selectState
        {
            get
            {
                if (selectStates.Count > 0)
                    return selectStates[0];
                return null;
            }
            set
            {
                if (!selectStates.Contains(value) & value != null)
                {
                    selectStates.Add(value);
                }
            }
        }

        /// <summary>
        /// 选择的连接线
        /// </summary>
		public Transition selectTransition
        {
            get
            {
                if (selectTransitions.Count > 0)
                    return selectTransitions[0];
                return null;
            }
            set
            {
                if (selectTransitions.Count > 0)
                    selectTransitions[0] = value;
                else
                    selectTransitions.Add(value);
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
                {
                    _stateManager = transform.GetComponentInParent<StateManager>();
                }
                return _stateManager;
            }
            set { _stateManager = value; }
        }

        /// <summary>
        /// 创建状态机实例
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns></returns>
        public static StateMachine CreateStateMachineInstance(string name = "MyStateMachine")
        {
            StateMachine stateMachine = new GameObject(name).AddComponent<StateMachine>();
            stateMachine.name = name;
            return stateMachine;
        }
    }
}