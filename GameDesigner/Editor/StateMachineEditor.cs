#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameDesigner
{
    //[CustomEditor(typeof(StateMachine), true)]
    public class StateMachineEditor : Editor
    {
        static public StateMachine stateMachine = null;

        void OnEnable()
        {
            stateMachine = target as StateMachine;
            stateMachine.transform.localPosition = Vector3.zero;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("打开游戏设计师编辑器", GUI.skin.GetStyle("LargeButtonMid"), GUILayout.ExpandWidth(true)))
                StateMachineWindow.Init(stateMachine);
            EditorGUILayout.Space();
            if (stateMachine.selectState != null)
            {
                StateManagerEditor.DrawState(stateMachine.selectState, stateMachine.stateManager);
                EditorGUILayout.Space();
                for (int i = 0; i < stateMachine.selectState.transitions.Count; ++i)
                    StateManagerEditor.DrawTransition(stateMachine.selectState.transitions[i]);
            }
            else if(StateMachineWindow.selectTransition != null)
            {
                StateManagerEditor.DrawTransition(StateMachineWindow.selectTransition);
            }
            Repaint();
        }

    }
}
#endif