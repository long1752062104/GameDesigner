#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 扩展矩形类Extension关键字
    /// </summary>
    public static class RectExtension
    {
        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect rect1 = rect;
            rect1.x = rect1.x - pivotPoint.x;
            rect1.y = rect1.y - pivotPoint.y;
            rect1.xMin = rect1.xMin * scale;
            rect1.xMax = rect1.xMax * scale;
            rect1.yMin = rect1.yMin * scale;
            rect1.yMax = rect1.yMax * scale;
            rect1.x = rect1.x + pivotPoint.x;
            rect1.y = rect1.y + pivotPoint.y;
            return rect1;
        }

        public static Vector2 TopLeft(this Rect r)
        {
            return new Vector2(r.x, r.y);
        }

        public static Vector2 TopRight(this Rect r)
        {
            return new Vector2(r.xMax, r.y);
        }
    }

    public enum SelectMode
    {
        none,
        selectState,
        dragState
    }

    /// <summary>
    /// 图表编辑器-基类
    /// </summary>
    public abstract class GraphEditor : EditorWindow
    {
        public const float MaxCanvasSize = 10000f;
        private const float GridMinorSize = 12f;
        private const float GridMajorSize = 120f;
        public static Vector2 Center
        {
            get
            {
                return new Vector2(GraphEditor.MaxCanvasSize * 0.5f, GraphEditor.MaxCanvasSize * 0.5f);
            }
        }
        [SerializeField]
        protected Vector2 scrollPosition;
        [SerializeField]
        protected Rect canvasSize;
        [SerializeField]
        protected float scale = 1.0f;
        protected Rect scaledCanvasSize
        {
            get
            {
                return new Rect(canvasSize.x * (1f / scale), canvasSize.y * (1f / scale), canvasSize.width * (1f / scale), canvasSize.height * (1f / scale));
            }
        }

        [SerializeField]
        private Rect worldViewRect;
        [SerializeField]
        private Vector2 offset;
        [SerializeField]
        protected Vector2 mousePosition;
        protected EventType currentType;
        protected Event currentEvent;
        private Rect scrollView;
        private Material material;
        protected bool openStateMenu = false;
        protected Vector2 selectionStartPosition;
        protected SelectMode mode = SelectMode.none;

        private GUIStyle _canvasBackground;
        private GUIStyle canvasBackground
        {
            get
            {
                if (_canvasBackground == null)
                    _canvasBackground = "flow background";
                return _canvasBackground;
            }
        }

        private GUIStyle _selectionRect = null;
        protected GUIStyle selectionRect
        {
            get
            {
                if (_selectionRect == null)
                {
                    _selectionRect = "SelectionRect";
                }
                return _selectionRect;
            }
        }

        protected virtual void OnEnable()
        {
            this.scrollView = new Rect(0, 0, MaxCanvasSize, MaxCanvasSize);
            this.UpdateScrollPosition(GraphEditor.Center);
        }


        protected void BeginWindow()
        {
            mousePosition = Event.current.mousePosition;
            currentType = Event.current.type;
            currentEvent = Event.current;
            this.canvasSize = GetCanvasSize();
            if (currentEvent.type == EventType.ScrollWheel)
            {
                Vector2 offset = (scaledCanvasSize.size - canvasSize.size) * 0.5f;
                scale -= currentEvent.delta.y / 100f;
                scale = Mathf.Clamp(scale, 0.05f, 1.0f);

                UpdateScrollPosition(scrollPosition - (scaledCanvasSize.size - canvasSize.size) * 0.5f + offset);
                Event.current.Use();
            }
            if (currentEvent.type == EventType.Repaint)
            {
                canvasBackground.Draw(scaledCanvasSize, false, false, false, false);
            }
            DrawGrid();
            Vector2 curScroll = GUI.BeginScrollView(scaledCanvasSize, scrollPosition, scrollView, GUIStyle.none, GUIStyle.none);
            UpdateScrollPosition(curScroll);
            mousePosition = Event.current.mousePosition;
        }

        protected void EndWindow()
        {
            DragCanvas();
            GUI.EndScrollView();
        }

        private static Matrix4x4 prevGuiMatrix;
        private static float kEditorWindowTabHeight;

        protected void ZoomableAreaBegin(Rect screenCoordsArea, float zoomScale, bool docked)
        {
            GUI.EndGroup();
            kEditorWindowTabHeight = (docked ? 19f : 22f);
            Rect rect = screenCoordsArea.ScaleSizeBy(1f / zoomScale, screenCoordsArea.TopLeft());
            rect.y = rect.y + kEditorWindowTabHeight;
            GUI.BeginGroup(rect);
            prevGuiMatrix = GUI.matrix;
            Matrix4x4 matrix4x4 = Matrix4x4.TRS(rect.TopLeft(), Quaternion.identity, Vector3.one);
            Vector3 vector3 = Vector3.one;
            float single = zoomScale;
            float single1 = single;
            vector3.y = single;
            vector3.x = single1;
            Matrix4x4 matrix4x41 = Matrix4x4.Scale(vector3);
            GUI.matrix = ((matrix4x4 * matrix4x41) * matrix4x4.inverse) * GUI.matrix;
        }

        protected void ZoomableAreaEnd()
        {
            GUI.matrix = prevGuiMatrix;
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0f, kEditorWindowTabHeight, Screen.width, Screen.height));
        }

        protected virtual Rect GetCanvasSize()
        {
            return new Rect(0, 20f, position.width, position.height);
        }

        protected void UpdateScrollPosition(Vector2 position)
        {
            offset = offset + (scrollPosition - position);
            scrollPosition = position;
            worldViewRect = new Rect(scaledCanvasSize);
            worldViewRect.y += scrollPosition.y;
            worldViewRect.x += scrollPosition.x;
        }

        private void DragCanvas()
        {
            if (currentEvent.button != 2)
            {
                return;
            }

            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (currentEvent.rawType)
            {
                case EventType.MouseDown:
                    GUIUtility.hotControl = controlID;
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        UpdateScrollPosition(scrollPosition - currentEvent.delta * (1f / scale));
                    }
                    break;
            }
        }

        private void DrawGrid()
        {
            GL.PushMatrix();
            GL.Begin(1);
            this.DrawGridLines(scaledCanvasSize, GraphEditor.GridMinorSize, offset, new Color(0f, 0f, 0f, 0.1f));
            this.DrawGridLines(scaledCanvasSize, GraphEditor.GridMajorSize, offset, new Color(0f, 0f, 0f, 0.15f));
            GL.End();
            GL.PopMatrix();
        }

        private void DrawGridLines(Rect rect, float gridSize, Vector2 offset, Color gridColor)
        {
            GL.Color(gridColor);
            for (float i = rect.x + (offset.x < 0f ? gridSize : 0f) + offset.x % gridSize; i < rect.x + rect.width; i = i + gridSize)
            {
                this.DrawLine(new Vector2(i, rect.y), new Vector2(i, rect.y + rect.height));
            }
            for (float j = rect.y + (offset.y < 0f ? gridSize : 0f) + offset.y % gridSize; j < rect.y + rect.height; j = j + gridSize)
            {
                this.DrawLine(new Vector2(rect.x, j), new Vector2(rect.x + rect.width, j));
            }
        }

        private void DrawLine(Vector2 p1, Vector2 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }

        protected void DrawConnection(Vector3 start, Vector3 end, Color color, int arrows, bool offset, bool drawAAPolyLineOrBezier = true, int offsetValue = 6)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            Vector3 cross = Vector3.Cross((start - end).normalized, Vector3.forward);
            if (offset)
            {
                start = start + cross * offsetValue;
                end = end + cross * offsetValue;
            }

            if (drawAAPolyLineOrBezier)
            {
                Handles.color = color;
                Handles.DrawAAPolyLine(null, 3.0f, new Vector3[] { start, end });
                Vector3 vector3 = end - start;
                Vector3 vector31 = vector3.normalized;
                Vector3 vector32 = (vector3 * 0.5f) + start;
                vector32 = vector32 - (cross * 0.5f);
                Vector3 vector33 = vector32 + vector31;
                for (int i = 0; i < arrows; i++)
                {
                    Vector3 center = vector33 + vector31 * 10.0f * i + vector31 * 5.0f - vector31 * arrows * 5.0f;
                    DrawArrow(color, cross, vector31, center, 6.0f);
                }
            }
            else
            {
                float tangent = end.x > start.x ? end.x - start.x : start.x - end.x;
                if (tangent > 100)
                    tangent = 100;
                Handles.DrawBezier(start, end, start + new Vector3(tangent, 0), end - new Vector3(tangent, 0), color, null, 3.5f);
            }
        }

        private void DrawArrow(Color color, Vector3 cross, Vector3 direction, Vector3 center, float size)
        {
            Rect rect = new Rect(worldViewRect);
            rect.y -= canvasSize.y - size;
            if (!rect.Contains(center))
            {
                return;
            }
            Vector3[] vector3Array = new Vector3[] {
                center + (direction * size),
                (center - (direction * size)) + (cross * size),
                (center - (direction * size)) - (cross * size),
                center + (direction * size)
            };

            Color color1 = color;
            color1.r *= 0.8f;
            color1.g *= 0.8f;
            color1.b *= 0.8f;

            CreateMaterial();
            material.SetPass(0);

            GL.Begin(GL.TRIANGLES);
            GL.Color(color1);
            GL.Vertex(vector3Array[0]);
            GL.Vertex(vector3Array[1]);
            GL.Vertex(vector3Array[2]);
            GL.End();
        }

        private void CreateMaterial()
        {
            if (material != null)
                return;

            material = new Material(Resources.Load<Shader>("GridLine"))
            {
                hideFlags = HideFlags.HideAndDontSave
            };
        }
    }
}
#endif