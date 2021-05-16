#if UNITY_EDITOR
namespace GameDesigner
{
    using Net.Component.Client;
    using UnityEditor;
    using UnityEngine;
    using EditorGUILayout = UnityEditor.EditorGUILayout;

    [CustomEditor(typeof(ComponentSync))]
    public class ComponentSyncEdit : Editor
    {
        ComponentSync component;

        private void OnEnable()
        {
            component = target as ComponentSync;
        }

        public override void OnInspectorGUI()
        {
            component.component = EditorGUILayout.ObjectField("同步组件", component.component, typeof(Component), true) as Component;
            if (component.component != null)
            {
                var type = component.component.GetType();
                var pros = type.GetProperties();
                foreach (var pro in pros)
                {
                    if (!pro.CanRead | !pro.CanWrite)
                        continue;
                    if (GUILayout.Button(pro.Name))
                    {
                        component.propertySyncs.Add(new PropertySync() { name = pro.Name, property = pro });
                    }
                }
                EditorGUILayout.LabelField("Sync:");
                foreach (var pro in component.propertySyncs)
                {
                    if (GUILayout.Button(pro.name))
                    {
                        component.propertySyncs.Remove(pro);
                        break;
                    }
                }
            }
        }
    }
}
#endif