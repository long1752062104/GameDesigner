#if UNITY_EDITOR
using UnityEditor;

namespace GameDesigner
{
    [CustomEditor(typeof(StateBehaviour), true)]
    public class StateBehaviourInspector : Editor
    {

        public static StateBehaviour behaviour;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (behaviour == null)
                return;

            serializedObject.Update();

            foreach (System.Reflection.FieldInfo f in behaviour.GetType().GetFields())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(f.Name), true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif