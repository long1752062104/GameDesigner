#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TrueSync
{

    [CustomPropertyDrawer(typeof(FP))]
    public class TSFPDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();//解决影响下行字段的值（多选问题）
            EditorGUI.BeginProperty(position, label, property);
            var rawProp = property.FindPropertyRelative("_serializedValue");

            FP fpValue = FP.FromRaw(rawProp.longValue);
            fpValue = EditorGUI.FloatField(position, label, (float)fpValue);
            if (GUI.changed)
            {
                rawProp.longValue = fpValue.RawValue;
                EditorUtility.SetDirty(rawProp.serializedObject.targetObject);
                GUI.changed = false;
            }
            EditorGUI.EndProperty();
            EditorGUI.EndChangeCheck();
        }
    }
}
#endif