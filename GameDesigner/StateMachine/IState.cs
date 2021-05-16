using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 状态基类
    /// </summary>
    public abstract class IState : MonoBehaviour
    {
#if UNITY_EDITOR || DEBUG || DEBUG
        /// <summary>
        /// 查找行为组件
        /// </summary>
        [HideInInspector]
        public bool findBehaviours = false;
        /// <summary>
        /// 绘制状态连接线条
        /// </summary>
		[HideInInspector]
        public bool makeTransition = false;
        /// <summary>
        /// 绘制蓝图获得值线条
        /// </summary>
		[HideInInspector]
        public bool makeGetValueTransition = false;
        /// <summary>
        /// 绘制蓝图运行下一个节点线条
        /// </summary>
		[HideInInspector]
        public bool makeRuntimeTransition = false;
        /// <summary>
        /// 绘制蓝图运行线条
        /// </summary>
		[HideInInspector]
        public bool makeOutRuntimeTransition = false;
        /// <summary>
        /// 创建脚本名称
        /// </summary>
		[HideInInspector]
        public string createScriptName = "NewStateBehaviour";
        /// <summary>
        /// 创建脚本路径
        /// </summary>
		public static string StateActionScriptPath = "/Actions/StateActions";
        /// <summary>
        /// 创建脚本路径
        /// </summary>
		public static string StateBehaviourScriptPath = "/Actions/StateBehaviours";
        /// <summary>
        /// 创建脚本路径
        /// </summary>
		public static string TransitionScriptPath = "/Actions/Transitions";
        /// <summary>
        /// 展开编辑器 和 添加脚本编译状态
        /// </summary>
		[HideInInspector]
        public bool foldout = true, compiling = false;
        /// <summary>
        /// 右键行为脚本存储行为的索引 和 动作菜单索引
        /// </summary>
		[HideInInspector]
        public int behaviourMenuIndex = 0, actionMenuIndex = 0;
#endif
        /// <summary>
        /// 蓝图 和 状态 的编辑器矩形
        /// </summary>
        public Rect rect = new Rect(10, 10, 150, 30);
        [SerializeField]
        [HideInInspector]
        private StateMachine _stateMachine = null;
        /// <summary>
        /// 状态机
        /// </summary>
		public StateMachine stateMachine
        {
            get
            {
                if (_stateMachine == null)
                {
                    _stateMachine = transform.GetComponentInParent<StateMachine>();
                }
                return _stateMachine;
            }
            set { _stateMachine = value; }
        }

        [SerializeField]
        [HideInInspector]
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
    }
}