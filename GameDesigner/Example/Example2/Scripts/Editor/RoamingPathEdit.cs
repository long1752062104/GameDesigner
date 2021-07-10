namespace Example2
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(RoamingPath))]
    [CanEditMultipleObjects]
    public class RoamingPathEdit : Editor
    {
        private RoamingPath self;
        SerializedProperty WaypointsFoldout;

        private void OnEnable()
        {
            self = target as RoamingPath;
            WaypointsFoldout = serializedObject.FindProperty("waypointsFoldout");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (GUILayout.Button("Add Waypoint"))
            {
                Vector3 newPoint = new Vector3(0, 0, 0);
                if (self.waypointsList.Count == 0)
                {
                    newPoint = new Vector3(0, 0, 5);
                }
                else if (self.waypointsList.Count > 0)
                {
                    newPoint = new Vector3(self.localWaypoints[self.localWaypoints.Count - 1].x, self.localWaypoints[self.localWaypoints.Count - 1].y, self.localWaypoints[self.localWaypoints.Count - 1].z + 5);
                }
                self.waypointsList.Add(newPoint);
                self.localWaypoints.Add(newPoint);
                EditorUtility.SetDirty(self);
            }
            if (GUILayout.Button("Clear All Waypoints", GUI.skin.button) && EditorUtility.DisplayDialog("Clear Waypoints?", "Are you sure you want to clear all of this AI's waypoints? This process cannot be undone.", "Yes", "Cancel"))
            {
                self.waypointsList.Clear();
                self.localWaypoints.Clear();
                EditorUtility.SetDirty(self);
            }
            WaypointsFoldout.boolValue = Foldout(WaypointsFoldout.boolValue, "Waypoints", true, EditorStyles.foldout);
            if (WaypointsFoldout.boolValue)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Waypoints", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                if (self.waypointsList.Count > 0)
                {
                    for (int j = 0; j < self.waypointsList.Count; ++j)
                    {
                        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
                        EditorGUILayout.LabelField("Waypoint " + (j + 1), EditorStyles.toolbarButton);
                        GUI.backgroundColor = Color.white;

                        if (GUILayout.Button("Remove Point", EditorStyles.miniButton, GUILayout.Height(18)))
                        {
                            self.waypointsList.RemoveAt(j);
                            self.localWaypoints.RemoveAt(j);
                            --j;
                            EditorUtility.SetDirty(self);
                        }
                        GUILayout.Space(10);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            serializedObject.ApplyModifiedProperties();
        }

        public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style)
        {
            Rect position = GUILayoutUtility.GetRect(40f, 40f, 16f, 16f, style);
            return EditorGUI.Foldout(position, foldout, content, toggleOnLabelClick, style);
        }
        public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick, GUIStyle style)
        {
            return Foldout(foldout, new GUIContent(content), toggleOnLabelClick, style);
        }

        void OnSceneGUI()
        {
            if (self.waypointsList.Count == 0)
                return;
            Handles.color = Color.blue;
            Handles.DrawLine(self.transform.position, self.waypointsList[0]);
            Handles.color = Color.white;

            Handles.color = Color.green;
            for (int i = 0; i < self.waypointsList.Count - 1; i++)
            {
                Handles.DrawLine(self.waypointsList[i], self.waypointsList[i + 1]);
            }
            if (self.waypointsList.Count > 1)
                Handles.DrawLine(self.waypointsList[0], self.waypointsList[self.waypointsList.Count - 1]);
            Handles.color = Color.white;

            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            for (int i = 0; i < self.waypointsList.Count; i++)
            {
                Handles.SphereHandleCap(0, self.waypointsList[i], Quaternion.identity, 0.5f, EventType.Repaint);
                drawString("Waypoint " + (i + 1), self.waypointsList[i] + Vector3.up, Color.white);
            }

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            for (int i = 0; i < self.waypointsList.Count; i++)
            {
                self.waypointsList[i] = Handles.PositionHandle(self.transform.position + self.localWaypoints[i], Quaternion.identity);
                self.localWaypoints[i] = self.waypointsList[i] - self.transform.position;
            }
        }

        static public void drawString(string text, Vector3 worldPos, Color? colour = null)
        {

            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            UnityEditor.Handles.BeginGUI();

            var restoreColor = GUI.color;

            if (colour.HasValue) GUI.color = colour.Value;
            var view = UnityEditor.SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

            if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
            {
                GUI.color = restoreColor;
                UnityEditor.Handles.EndGUI();
                return;
            }

            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text, style);
            GUI.color = restoreColor;
            UnityEditor.Handles.EndGUI();
        }
    }
}