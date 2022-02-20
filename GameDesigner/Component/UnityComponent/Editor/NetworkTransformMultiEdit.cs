#if UNITY_EDITOR
using Net.UnityComponent;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NetworkTransformMulti))]
[CanEditMultipleObjects]
public class NetworkTransformMultiEdit : Editor
{
    private NetworkTransformMulti nt;

    private void OnEnable()
    {
        nt = target as NetworkTransformMulti;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("更新子物体"))
        {
            var childs1 = nt.transform.GetComponentsInChildren<Transform>();
            var list = new List<ChildTransform>();
            foreach (var child in childs1)
            {
                if (child == nt.transform)
                    continue;
                if (!child.gameObject.activeInHierarchy)
                    continue;
                list.Add(new ChildTransform()
                {
                    name = child.name,
                    transform = child,
                    mode = nt.syncMode,
                    syncPosition = nt.syncPosition,
                    syncRotation = nt.syncRotation,
                    syncScale = nt.syncScale,
                });
            }
            nt.childs = list.ToArray();
            EditorUtility.SetDirty(nt);
        }
    }
}
#endif