#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace GameDesigner
{
    public class StateMachineSetting : ScriptableObject
    {
#if UNITY_EDITOR || DEBUG
        static private StateMachineSetting _instance = null;
        static public StateMachineSetting Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<StateMachineSetting>("StateMachineSetting");
                    if (_instance == null)
                    {
                        _instance = CreateInstance<StateMachineSetting>();
                        var path = "Assets/" + BlueprintSetting.GetGameDesignerPath.Split(new string[] { @"Assets\" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        AssetDatabase.CreateAsset(_instance, path + "/Editor/Resources/StateMachineSetting.asset");
                    }
                }
                return _instance;
            }
        }

        [SerializeField]
        private GUIStyle _defaultAndSelectStyle = new GUIStyle();
        /// 默认状态和被选中状态的皮肤
        public GUIStyle defaultAndSelectStyle
        {
            get
            {
                if (_defaultAndSelectStyle.normal.background == null)
                {
                    _defaultAndSelectStyle = new GUIStyle(GUI.skin.GetStyle("flow node 5 on"));
                    SetImage(_defaultAndSelectStyle);
                }
                return _defaultAndSelectStyle;
            }
        }

        [SerializeField]
        private GUIStyle _defaultAndRuntimeIndexStyle = new GUIStyle();
        /// 默认状态和当前执行状态经过的皮肤
        public GUIStyle defaultAndRuntimeIndexStyle
        {
            get
            {
                if (_defaultAndRuntimeIndexStyle.normal.background == null)
                {
                    _defaultAndRuntimeIndexStyle = new GUIStyle(GUI.skin.GetStyle("flow node 2 on"));
                    SetImage(_defaultAndRuntimeIndexStyle);
                }
                return _defaultAndRuntimeIndexStyle;
            }
        }

        [SerializeField]
        private GUIStyle _stateInDefaultStyle = new GUIStyle();
        /// 默认状态的皮肤
        public GUIStyle stateInDefaultStyle
        {
            get
            {
                if (_stateInDefaultStyle.normal.background == null)
                {
                    _stateInDefaultStyle = new GUIStyle(GUI.skin.GetStyle("flow node 5"));
                    SetImage(_stateInDefaultStyle);
                }
                return _stateInDefaultStyle;
            }
        }

        [SerializeField]
        private GUIStyle _indexInRuntimeStyle = new GUIStyle();
        /// 状态执行经过的每个状态所显示的皮肤
        public GUIStyle indexInRuntimeStyle
        {
            get
            {
                if (_indexInRuntimeStyle.normal.background == null)
                {
                    _indexInRuntimeStyle = new GUIStyle(GUI.skin.GetStyle("flow node 2 on"));
                    SetImage(_indexInRuntimeStyle);
                }
                return _indexInRuntimeStyle;
            }
        }

        [SerializeField]
        private GUIStyle _selectStateStyle = new GUIStyle();
        /// 当点击选择状态的皮肤
        public GUIStyle selectStateStyle
        {
            get
            {
                if (_selectStateStyle.normal.background == null)
                {
                    _selectStateStyle = new GUIStyle(GUI.skin.GetStyle("flow node 1 on"));
                    SetImage(_selectStateStyle);
                }
                return _selectStateStyle;
            }
        }

        [SerializeField]
        private GUIStyle _defaultStyle = new GUIStyle();
        /// 空闲状态的皮肤
        public GUIStyle defaultStyle
        {
            get
            {
                if (_defaultStyle.normal.background == null)
                {
                    _defaultStyle = new GUIStyle(GUI.skin.GetStyle("flow node 0"));
                    SetImage(_defaultStyle);
                }
                return _defaultStyle;
            }
        }

        [SerializeField]
        private string _designerName = "flow node 6";
        [SerializeField]
        private GUIStyle _designerStyle = new GUIStyle();
        /// 空闲状态的皮肤
        public GUIStyle designerStyle
        {
            get
            {
                if (_designerStyle.normal.background == null)
                {
                    _designerStyle = new GUIStyle(GUI.skin.GetStyle(_designerName));
                    SetImage(_designerStyle);
                    _designerStyle.normal.background = Resources.Load<Texture2D>("Foldout_BG");
                }
                return _designerStyle;
            }
        }

        [SerializeField]
        private string _selectNodesName = "flow node 6 On";
        [SerializeField]
        private GUIStyle _selectNodesStyle = new GUIStyle();
        /// 选择蓝图节点的皮肤
        public GUIStyle selectNodesStyle
        {
            get
            {
                if (_selectNodesStyle.normal.background == null)
                {
                    _selectNodesStyle = new GUIStyle(GUI.skin.GetStyle(_selectNodesName));
                    SetImage(_selectNodesStyle);
                    _selectNodesStyle.normal.background = Resources.Load<Texture2D>("SeqNode");
                }
                return _selectNodesStyle;
            }
        }

        [SerializeField]
        private GUIStyle _functionalBlockNodesStyle = new GUIStyle();
        /// 选择蓝图节点的皮肤
        public GUIStyle functionalBlockNodesStyle
        {
            get
            {
                if (_functionalBlockNodesStyle.normal.background == null)
                {
                    _functionalBlockNodesStyle = new GUIStyle(GUI.skin.GetStyle("flow node 0 On"));
                    SetImage(_functionalBlockNodesStyle);
                }
                return _functionalBlockNodesStyle;
            }
        }

        public Color parameterNameColor = Color.white;

        public Rect getRect = new Rect(-19, 0, 20, 20);
        public Rect setRect = new Rect(168, 0, 20, 20);
        public Vector2 offset = new Vector2(-10, -30);
        public Rect mainRect = new Rect(-19, -20, 12, 15);
        public Rect runRect = new Rect(168, -20, 12, 20);
        public float topHeight = 30;

        static public void SetImage(GUIStyle style)
        {
            style.normal.textColor = Color.white;
        }
#endif
    }
}
#endif