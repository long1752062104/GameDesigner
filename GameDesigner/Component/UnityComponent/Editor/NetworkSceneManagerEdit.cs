#if UNITY_EDITOR
using Net.UnityComponent;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NetworkSceneManager))]
[CanEditMultipleObjects]
public class NetworkSceneManagerEdit : Editor
{
    private NetworkSceneManager nt;

    private void OnEnable()
    {
        nt = target as NetworkSceneManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("…Ë÷√registerObjectIndex"))
        {
            for (int i = 0; i < nt.registerObjects.Count; i++)
            {
                nt.registerObjects[i].registerObjectIndex = i;
                EditorUtility.SetDirty(nt.registerObjects[i]);
            }
            EditorUtility.SetDirty(nt);
        }
    }
}
#endif