#if UNITY_EDITOR
using FixMath;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 只显示不能修改属性绘制
/// </summary>
[CanEditMultipleObjects, CustomPropertyDrawer(typeof(DisplayOnly))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

///<summary>
///绘制多选属性
///</summary>
[CanEditMultipleObjects, CustomPropertyDrawer(typeof(EnumFlags))]
public class EnumFlagsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
    }
}

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