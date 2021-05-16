#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameDesigner
{
    [CustomEditor(typeof(State), true)]
    public class StateInspector : Editor
    {
        static public State state = null;

        void OnEnable()
        {
            state = target as State;
            state.transform.hideFlags = HideFlags.HideInInspector;
            state.transform.localPosition = Vector3.zero;
        }

        public override void OnInspectorGUI()
        {
            StateManagerEditor.DrawState(state, state.stateMachine.stateManager);

            EditorGUILayout.Space();

            for (int i = 0; i < state.transitions.Count; ++i)
            {
                StateManagerEditor.DrawTransition(state.transitions[i]);
            }

            Repaint();
        }
    }

    [CustomEditor(typeof(StateMachine), true)]
    public class StateMachineEditor : Editor
    {
        static public StateMachine stateMachine = null;

        void OnEnable()
        {
            stateMachine = target as StateMachine;
            stateMachine.transform.localPosition = Vector3.zero;
            StateMachineWindow.stateMachine = stateMachine;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("打开游戏设计师编辑器", GUI.skin.GetStyle("LargeButtonMid"), GUILayout.ExpandWidth(true)))
                StateMachineWindow.Init();

            EditorGUILayout.Space();

            if (stateMachine.selectState != null)
            {
                StateManagerEditor.DrawState(stateMachine.selectState, stateMachine.stateManager);

                EditorGUILayout.Space();

                for (int i = 0; i < stateMachine.selectState.transitions.Count; ++i)
                {
                    StateManagerEditor.DrawTransition(stateMachine.selectState.transitions[i]);
                }
            }
            else if (stateMachine.selectTransition != null)
            {
                StateManagerEditor.DrawTransition(stateMachine.selectTransition);
            }

            Repaint();
        }

    }
}
#endif