#if UNITY_EDITOR
using Net.UnityComponent;
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
        if (nt.getChilds)
        {
            nt.getChilds = false;
            var childs1 = nt.transform.GetComponentsInChildren<Transform>();
            nt.childs = new ChildTransform[childs1.Length - 1];
            for (int i = 1; i < childs1.Length; i++)
            {
                nt.childs[i - 1] = new ChildTransform()
                {
                    name = childs1[i].name,
                    transform = childs1[i]
                };
                nt.childs[i - 1].mode = nt.syncMode;
            }
            EditorUtility.SetDirty(nt);
        }
    }
}
#endif