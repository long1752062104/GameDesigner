#if UNITY_EDITOR
using FixMath;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(F64))]
public class TSFPDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var rawProp = property.FindPropertyRelative("Raw");

        F64 fpValue = F64.FromRaw(rawProp.longValue);
        fpValue = EditorGUI.FloatField(position, label, fpValue);

        if (GUI.changed)
        {
            rawProp.longValue = fpValue.Raw;
            EditorUtility.SetDirty(rawProp.serializedObject.targetObject);
            GUI.changed = false;
        }

        EditorGUI.EndProperty();
    }
}
#endif