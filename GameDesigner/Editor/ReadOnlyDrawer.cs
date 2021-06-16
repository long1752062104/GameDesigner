#if UNITY_EDITOR
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
#endif