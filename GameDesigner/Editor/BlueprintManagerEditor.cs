#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameDesigner
{
    [CustomEditor(typeof(BlueprintManager))]
    public class BlueprintManagerEditor : Editor
    {
        BlueprintManager sdm = null;
        string path = "Assets/";
        string fileName = "MyBlueprint";

        void OnEnable()
        {
            sdm = target as BlueprintManager;
            sdm.CheckUpdate();
            BlueprintEditor.designer = sdm.blueprint;
        }

        public override void OnInspectorGUI()
        {
            sdm.blueprint = (Blueprint)EditorGUILayout.ObjectField("使用的蓝图文件", sdm.blueprint, typeof(Blueprint), true);
            if (GUILayout.Button("打开蓝图编辑器!"))
            {
                BlueprintEditor.Init();
                sdm.CheckUpdate();
            }
            foreach (var node in sdm.blueprint.selectNodes)
            {
                if (node == null)
                    continue;
                if (node.method.memberTypes == MemberTypes.Constructor)
                    continue;
                BlueprintGUILayout.BeginStyleVertical("节点变量属性", "box");
                EditorGUILayout.LabelField("节点名称", node.method.name);
                node.method.nodeName = EditorGUILayout.TextField("变量名称", node.method.nodeName);
                node.method.targetValue.Value = BlueprintGUILayout.PropertyField("变量值", node.method.targetValue.Value, node.method.targetType);
                BlueprintGUILayout.EndStyleVertical();
            }
            GUILayout.Space(5);
            path = EditorGUILayout.TextField("文件路径", path);
            fileName = EditorGUILayout.TextField("文件名称", fileName);
            if (GUILayout.Button("保存蓝图文件!"))
            {
                PrefabUtility.SaveAsPrefabAssetAndConnect(sdm.blueprint.gameObject, path + fileName + ".prefab", InteractionMode.AutomatedAction);
                PrefabUtility.UnpackPrefabInstance(sdm.blueprint.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif