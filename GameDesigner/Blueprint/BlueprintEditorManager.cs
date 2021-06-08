using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 蓝图编辑器运行类，使用此类可快速设置编辑器的功能
    /// </summary>
    public class BlueprintEditorManager : GameDesigner.Blueprint
    {
        [HideInInspector]
        public Node _onGUI = null;
        public Node onGUI
        {
            get
            {
                if (_onGUI == null)
                {
                    _onGUI = Node.CreateFunctionBody(this, GetType(), "OnGUI", "EditorWindow", "当编辑器GUI");
                }
                return _onGUI;
            }
        }

        public void OnGUI()
        {
            if (onGUI.runtime != null)
            {
                onGUI.runtime.Invoke();
            }
        }
    }
}