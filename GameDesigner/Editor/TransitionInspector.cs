#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameDesigner
{
    [CustomEditor(typeof(Transition), true)]
    public class TransitionInspector : Editor
    {
        static public Transition transition = null;

        void OnEnable()
        {
            transition = target as Transition;
            transition.transform.hideFlags = HideFlags.HideInInspector;
            transition.transform.localPosition = Vector3.zero;
        }

        public override void OnInspectorGUI()
        {
            StateManagerEditor.DrawTransition(transition);
        }
    }
}
#endif