﻿#if UNITY_EDITOR
using GGPhys.Core;
using UnityEditor;
using UnityEngine;

namespace TrueSync
{

    [CustomPropertyDrawer(typeof(TSVector3))]
    public class TSVectorDrawer : PropertyDrawer
    {

        private const int INDENT_OFFSET = 15;
        private const int LABEL_WIDTH = 12;
        private static GUIContent xLabel = new GUIContent("X");
        private static GUIContent yLabel = new GUIContent("Y");
        private static GUIContent zLabel = new GUIContent("Z");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);

            position.width /= 3f;
            float indentOffsetLevel = INDENT_OFFSET * EditorGUI.indentLevel;
            position.width += indentOffsetLevel;

            EditorGUIUtility.labelWidth = indentOffsetLevel + LABEL_WIDTH;

            SerializedProperty xSerProperty = property.FindPropertyRelative("x");
            position.x -= indentOffsetLevel;
            EditorGUI.PropertyField(position, xSerProperty, xLabel);

            position.x += position.width;

            SerializedProperty ySerProperty = property.FindPropertyRelative("y");
            position.x -= indentOffsetLevel;
            EditorGUI.PropertyField(position, ySerProperty, yLabel);

            position.x += position.width;

            SerializedProperty zSerProperty = property.FindPropertyRelative("z");
            position.x -= indentOffsetLevel;
            EditorGUI.PropertyField(position, zSerProperty, zLabel);

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(Vector3d))]
    public class TSVectorDrawer1 : PropertyDrawer
    {

        private const int INDENT_OFFSET = 15;
        private const int LABEL_WIDTH = 12;
        private static GUIContent xLabel = new GUIContent("X");
        private static GUIContent yLabel = new GUIContent("Y");
        private static GUIContent zLabel = new GUIContent("Z");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);

            position.width /= 3f;
            float indentOffsetLevel = INDENT_OFFSET * EditorGUI.indentLevel;
            position.width += indentOffsetLevel;

            EditorGUIUtility.labelWidth = indentOffsetLevel + LABEL_WIDTH;

            SerializedProperty xSerProperty = property.FindPropertyRelative("x");
            position.x -= indentOffsetLevel;
            EditorGUI.PropertyField(position, xSerProperty, xLabel);

            position.x += position.width;

            SerializedProperty ySerProperty = property.FindPropertyRelative("y");
            position.x -= indentOffsetLevel;
            EditorGUI.PropertyField(position, ySerProperty, yLabel);

            position.x += position.width;

            SerializedProperty zSerProperty = property.FindPropertyRelative("z");
            position.x -= indentOffsetLevel;
            EditorGUI.PropertyField(position, zSerProperty, zLabel);

            EditorGUI.EndProperty();
        }
    }
}
#endif