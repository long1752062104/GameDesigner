#if UNITY_EDITOR
using Net.Component;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneManager))]
[CanEditMultipleObjects]
public class SceneManagerEdit : Editor
{
    private SceneManager sm;

    private void OnEnable()
    {
        sm = target as SceneManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (sm.setIndex)
        {
            sm.setIndex = false;
            for (int i = 0; i < sm.prefabs.Length; i++)
            {
                sm.prefabs[i].index = (byte)i;
                EditorUtility.SetDirty(sm.prefabs[i]);
            }
            Debug.Log("设置完成!");
        }
    }
}
#endif