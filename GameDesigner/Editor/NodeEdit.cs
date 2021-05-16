#if UNITY_EDITOR
using GameDesigner;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlueprintNode))]
public class NodeEdit : Editor
{
    private BlueprintNode node;

    private void OnEnable()
    {
        node = target as BlueprintNode;
    }

    public override void OnInspectorGUI()
    {
        BlueprintGUILayout.BeginStyleVertical("方法属性", "box");
        EditorGUILayout.LabelField("方法名称", node.method.name);
        node.method.targetValue.Value = BlueprintGUILayout.PropertyField("对象", node.method.targetValue.Value, node.method.targetType);
        if (EditorApplication.isCompiling | !EditorApplication.isPlaying)
            node.method.targetValue.ValueToString();
        foreach (var par in node.method.Parameters)
        {
            par.EditorValue = BlueprintGUILayout.PropertyField(par.name, par.Value, par.parameterType);
            if (EditorApplication.isCompiling | !EditorApplication.isPlaying)
                par.value.ValueToString();
        }
        if (GUILayout.Button("添加运行脚本"))
        {
            var run = node.gameObject.AddComponent<NodeRun>();
            run.node = node;
        }
        BlueprintGUILayout.EndStyleVertical();
    }
}
#endif
