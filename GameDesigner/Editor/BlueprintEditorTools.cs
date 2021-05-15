#if UNITY_EDITOR
using GameDesigner;
using UnityEditor;
using UnityEngine;

public class BlueprintEditorTools : EditorWindow
{
    public BlueprintEditorManager designer = null;

    [MenuItem("GameDesigner/Blueprint/BlueprintEditorManager")]
    static void init()
    {
        GetWindow<BlueprintEditorTools>();
    }

    void OnGUI()
    {
        GUILayout.TextArea("蓝图编辑器类，当你将蓝图designer组件拖入designer对象后便同意你将委托此蓝图组件，这时候designer将被执行");
        designer = (BlueprintEditorManager)EditorGUILayout.ObjectField("Blueprint", designer, typeof(BlueprintEditorManager), true);
        if (designer != null)
        {
            if (designer.onGUI.runtime != null)
            {
                designer.onGUI.runtime.Invoke();
            }
        }
        Repaint();
    }
}
#endif