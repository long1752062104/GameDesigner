namespace Example2
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(LineOfSight))]
    [CanEditMultipleObjects]
    public class LineOfSightEdit : Editor
    {
        private LineOfSight self;

        private void OnEnable()
        {
            self = target as LineOfSight;
        }

        void OnSceneGUI()
        {
            Handles.color = new Color(0.8f, 0.7f, 0.2f, 0.3f);
            Handles.DrawSolidArc(self.transform.position + Vector3.up, self.transform.up, self.transform.forward, self.viewAngle / 2, self.detectionRadius);
            Handles.DrawSolidArc(self.transform.position + Vector3.up, self.transform.up, self.transform.forward, -self.viewAngle / 2, self.detectionRadius);

        }
    }
}
