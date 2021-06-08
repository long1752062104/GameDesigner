#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameDesigner
{
    public class BlueprintEditor : GraphEditor
    {
        public bool showWindow = false;
        public static Blueprint designer = null;
        static public BlueprintEditor Instance
        {
            get
            {
                if (BlueprintGUILayout.Instance.BlueprintEditor == null)
                {
                    BlueprintGUILayout.Instance.BlueprintEditor = GetWindow<BlueprintEditor>("蓝图设计师编辑器窗口", true);
                }
                return BlueprintGUILayout.Instance.BlueprintEditor as BlueprintEditor;
            }
        }

        [MenuItem("GameDesigner/Blueprint/BlueprintEditor")]
        static public void Init()
        {
            BlueprintGUILayout.Instance.BlueprintEditor = GetWindow<BlueprintEditor>("蓝图设计师编辑器窗口", true);
        }

        private Vector2 MousePosition;

        void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Button(new GUIContent(designer ? designer.name : "Not Selection Object !", BlueprintGUILayout.Instance.stateMachineImage), GUI.skin.GetStyle("GUIEditor.BreadcrumbLeft"), GUILayout.Width(120));
            GUILayout.Space(10);
            designer = (Blueprint)EditorGUILayout.ObjectField(GUIContent.none, designer, typeof(Blueprint), true, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            if (GUILayout.Button("复位", GUI.skin.GetStyle("GUIEditor.BreadcrumbLeft"), GUILayout.Width(50)))
            {
                if (designer == null)
                    return;
                if (designer.nodes.Count > 0)
                {
                    UpdateScrollPosition(designer.nodes[0].rect.position - new Vector2(position.size.x / 2 - 75, position.size.y / 2 - 15)); //  更新滑动矩阵
                }
                else
                    UpdateScrollPosition(Center); //  归位到矩形的中心
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            MousePosition = Event.current.mousePosition;
            ZoomableAreaBegin(new Rect(0f, 0f, scaledCanvasSize.width, scaledCanvasSize.height + 21), scale, false);
            BeginWindow();
            if (designer)
            {
                DrawBlueprint(designer);
            }
            EndWindow();
            ZoomableAreaEnd();
            if (designer == null)
                CreateStateMachineMenu();
            else if (openStateMenu)
                OpenStateContextMenu(designer.selectMethod);
            else
                OpenWindowContextMenu();
            Repaint();
        }

        private void CreateStateMachineMenu()
        {
            if (currentType == EventType.MouseDown & Event.current.button == 1)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("添加蓝图管理器组件"), false, delegate
                {
                    if (Selection.activeGameObject == null)
                    {
                        EditorUtility.DisplayDialog("提示!", "请选择游戏物体后再添加蓝图管理器组件！", "是", "否");
                    }
                    else if (Selection.activeGameObject.GetComponent<BlueprintManager>())
                    {
                        var man = Selection.activeGameObject.GetComponent<BlueprintManager>();
                        man.CheckUpdate();
                        designer = man.blueprint;
                    }
                    else
                    {
                        var man = Selection.activeGameObject.AddComponent<BlueprintManager>();
                        man.CheckUpdate();
                        designer = man.blueprint;
                    }
                });
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        public void DrawBlueprint(Blueprint _designer)
        {
            BlueprintOutputCode.designer = _designer;
            try { DrawBlueprintNodes(); } catch { }
        }

        /// <summary>
        /// 绘制状态(状态的层,状态窗口举行)
        /// </summary>
        public void DrawBlueprintNodes()
        {
            DragFunctionalBlock();
            for (int i = 0; i < designer.nodes.Count; ++i)
            {
                DrawTransition(designer.nodes[i], i);
            }
            for (int i = 0; i < designer.nodes.Count; ++i)
            {
                if (designer.selectNodes.Contains(designer.nodes[i]))
                    DoNode(designer.nodes[i], StateMachineSetting.Instance.selectNodesStyle);
                else
                    DoNode(designer.nodes[i], StateMachineSetting.Instance.designerStyle);
            }
            DragSelectNodes();
            if (Event.current.type == EventType.MouseDown & Event.current.button == 0 & AddBlueprintNode.WindowIsNull & !showWindow)
            {
                AddBlueprintNode.Instance.Close();
            }
            if (showWindow)
                showWindow = false;
        }

        private void DoNode(Node node, GUIStyle style)
        {
            if (node.method.memberTypes == System.Reflection.MemberTypes.All)
            {
                DrawFunctionTransition(node);
                DrawFunctionNode(node, style);
            }
            else
                DrawNode(node, style);
        }

        public void DrawFunctionTransition(Node method)
        {
            Rect rect = method.rect;
            if (method.runtime)
            {
                DrawConnection(method.rect.position + StateMachineSetting.Instance.runRect.center - StateMachineSetting.Instance.offset, method.runtime.rect.position + StateMachineSetting.Instance.mainRect.center - StateMachineSetting.Instance.offset, Color.green, 1, false, false);
            }
            rect.y += StateMachineSetting.Instance.topHeight;
            if (method.makeRuntimeTransition)
            {
                Vector2 startpos = method.rect.position + StateMachineSetting.Instance.runRect.center - StateMachineSetting.Instance.offset;
                Vector2 endpos = Event.current.mousePosition;
                DrawConnection(startpos, endpos, Color.green, 1, true, false, 0);
                for (int a = 0; a < designer.nodes.Count; ++a)
                {
                    if (designer.nodes[a].rect.Contains(Event.current.mousePosition) & Event.current.type == EventType.MouseDown)
                    {
                        method.runtime = designer.nodes[a];
                        FOR(method.runtime, method, method.runtime);
                        method.makeRuntimeTransition = false;
                        method.Invoke();//更新判断是否符合条件
                        return;
                    }
                }
                if (Event.current.button == 0 & Event.current.type == EventType.MouseDown)
                {
                    method.runtime = null;
                    method.makeRuntimeTransition = false;
                }
            }
        }

        public void DrawFunctionNode(Node method, GUIStyle style = null)
        {
            if (style == null)
                DragStateBoxPosition(method.rect, StateMachineSetting.Instance.designerStyle);
            else
                DragStateBoxPosition(method.rect, style);
            Rect rect = method.rect;
            rect.x += 10;
            rect.width -= 20;
            GUI.Label(new Rect(rect.x, rect.y + 10, rect.width, 20), method.method.name, BlueprintGUILayout.Instance.methodStyle);
            rect.y += StateMachineSetting.Instance.topHeight;
            if (BlueprintGUILayout.Button(new Rect(rect.position + StateMachineSetting.Instance.runRect.position, StateMachineSetting.Instance.runRect.size), BlueprintGUILayout.Instance.setRuntimeStyle, 0))
            {
                method.makeRuntimeTransition = true;
            }
            method.rect.height = rect.y - method.rect.y;
            if (method.rect.Contains(mousePosition) & currentType == EventType.MouseDown & currentEvent.button == 0)
            {
                if (!designer.selectNodes.Contains(method))
                {
                    designer.selectNodes = new List<Node> {
                        method
                    };
                }
            }
            else if (method.rect.Contains(mousePosition) & currentType == EventType.MouseDown & currentEvent.button == 1)
            {
                openStateMenu = true;
                designer.selectMethod = method;
            }
            if (currentEvent.keyCode == KeyCode.Delete & currentEvent.type == EventType.KeyUp)
                DeletedNode();
        }

        private void DragFunctionalBlock()
        {
            foreach (var fb in designer.functionalBlocks)
            {
                DragFunctionalBlockPosition(fb);
            }
        }

        bool dragBlock = false;
        bool dragBlockSize = false;
        /// <summary>
        /// 拖动状态盒位置
        /// </summary>
        protected void DragFunctionalBlockPosition(FunBlockNode node)
        {
            GUI.Box(node.rect, "", StateMachineSetting.Instance.functionalBlockNodesStyle);
            node.tooltip = GUI.TextArea(new Rect(node.rect.x, node.rect.y + 20, node.rect.width, 30), node.tooltip);

            if (dragState)
                return;

            if (Event.current.button == 0)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        if (new Rect(node.rect.x, node.rect.y, node.rect.width, 20).Contains(mousePosition))
                        {
                            dragBlock = true;
                            designer.selectBlock = node;
                        }
                        if (new Rect(node.rect.x + node.rect.width - 20, node.rect.y + node.rect.height - 20, 20, 20).Contains(mousePosition))
                        {
                            dragBlockSize = true;
                            selectionStartPosition = node.rect.position;
                            mode = SelectMode.selectState;
                            designer.selectBlock = node;
                        }
                        break;
                    case EventType.MouseDrag:
                        if (dragBlock & designer.selectBlock != null)
                        {
                            designer.selectBlock.rect.position += Event.current.delta;//拖到状态按钮
                            for (int n = 0; n < designer.selectBlock.nodes.Count; ++n)
                            {
                                designer.nodes[designer.selectBlock.nodes[n]].rect.position += Event.current.delta;//拖到状态按钮
                            }
                        }
                        if (dragBlockSize & designer.selectBlock != null)
                        {
                            designer.selectBlock.rect.size = FromToRect(designer.selectBlock.rect.position, mousePosition).size;
                            SelectNodesInRect(FromToRect(selectionStartPosition, mousePosition));
                        }
                        Event.current.Use();
                        break;
                    case EventType.MouseUp:
                        dragBlock = false;
                        if (dragBlockSize)
                        {
                            designer.selectBlock.nodes = new List<int>();
                            foreach (var node1 in designer.selectNodes)
                            {
                                designer.selectBlock.nodes.Add(node1.ID);
                            }
                            dragBlockSize = false;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 绘制选择状态
        /// </summary>
        private void DragSelectNodes()
        {
            if (dragBlock | dragBlockSize)
                return;

            switch (currentType)
            {
                case EventType.MouseDown:
                    selectionStartPosition = mousePosition;
                    if (currentEvent.button == 2 | currentEvent.button == 1)
                    {
                        mode = SelectMode.none;
                        return;
                    }
                    foreach (Node state in designer.nodes)
                    {
                        if (state.rect.Contains(mousePosition))
                        {
                            mode = SelectMode.dragState;
                            return;
                        }
                    }
                    mode = SelectMode.selectState;
                    break;
                case EventType.MouseUp:
                    mode = SelectMode.none;
                    break;
            }

            switch (mode)
            {
                case SelectMode.dragState:
                    if (designer.selectMethod)
                    {
                        if (designer.selectMethod.method.memberTypes == System.Reflection.MemberTypes.All)
                        {
                            DrawFunctionTransition(designer.selectMethod);
                            DrawFunctionNode(designer.selectMethod, StateMachineSetting.Instance.selectNodesStyle);
                        }
                        else
                            DrawNode(designer.selectMethod, StateMachineSetting.Instance.selectNodesStyle);
                    }
                    break;
                case SelectMode.selectState:
                    GUI.Box(FromToRect(selectionStartPosition, mousePosition), "", "SelectionRect");
                    SelectNodesInRect(FromToRect(selectionStartPosition, mousePosition));
                    break;
            }
        }

        private void SelectNodesInRect(Rect r)
        {
            for (int i = 0; i < designer.nodes.Count; i++)
            {
                Rect rect = designer.nodes[i].rect;
                if (rect.xMax < r.x || rect.x > r.xMax || rect.yMax < r.y || rect.y > r.yMax)
                {
                    designer.selectNodes.Remove(designer.nodes[i]);
                    continue;
                }
                if (!designer.selectNodes.Contains(designer.nodes[i]))
                {
                    designer.selectNodes.Add(designer.nodes[i]);
                }
            }
            if (Event.current.keyCode == KeyCode.LeftAlt & Event.current.type == EventType.KeyUp)
            {
                designer.functionalBlocks.Add(FunBlockNode.CreateFunctionalBlockNodeInstance(designer, r));
            }
        }

        private Rect FromToRect(Vector2 start, Vector2 end)
        {
            Rect rect = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
            if (rect.width < 0f)
            {
                rect.x = rect.x + rect.width;
                rect.width = -rect.width;
            }
            if (rect.height < 0f)
            {
                rect.y = rect.y + rect.height;
                rect.height = -rect.height;
            }
            return rect;
        }

        public Texture Image(object _image, System.Type type)
        {
            if (_image == null)
            {
                _image = EditorGUIUtility.ObjectContent(null, type).image;
                if (_image == null & type.FullName.Contains("Unity"))
                    _image = EditorGUIUtility.ObjectContent(null, typeof(MonoScript)).image;
                else if (_image == null)
                    _image = BlueprintGUILayout.Instance.cshImage;
            }
            return (Texture)_image;
        }

        /// <summary>
        /// 绘制状态(状态的层,状态窗口举行)
        /// </summary>
        public void DrawNode(Node method, GUIStyle style = null)
        {
            GUI.skin.label.normal.textColor = Color.white;
            if (style == null)
                DragStateBoxPosition(method.rect, StateMachineSetting.Instance.designerStyle);
            else
                DragStateBoxPosition(method.rect, style);
            Rect rect = method.rect;
            rect.x += 10;
            rect.width -= 20;
            GUI.Label(new Rect(rect.x, rect.y + 10, rect.width, 20), new GUIContent(method.method.name, method.method.xmlTexts), BlueprintGUILayout.Instance.methodStyle);
            rect.y += StateMachineSetting.Instance.topHeight;
            if (method.method.typeModel == ObjectModel.Object)
            {
                GUI.DrawTexture(new Rect(rect.x, rect.y, 16, 17), Image(method.method.image, method.method.targetType));
                method.method.targetValue._Object = EditorGUI.ObjectField(new Rect(rect.x + 25, rect.y, rect.width - 25, 17), method.method.targetValue._Object, method.method.targetType, true);
            }
            else
            {
                method.method.target = BlueprintGUILayout.PropertyField(rect, "", method.method.target, method.method.targetType, method.method.typeName, Image(method.method.image, method.method.targetType), method.method.targetTypeName + " " + method.method.name, true);
                if (EditorApplication.isCompiling | !EditorApplication.isPlaying)
                    method.method.targetValue.ValueToString();
            }
            if (BlueprintGUILayout.Button(new Rect(rect.position + StateMachineSetting.Instance.getRect.position, StateMachineSetting.Instance.getRect.size), BlueprintGUILayout.Instance.getValueStyle, "return value -> " + method.method.returnTypeName, 0))
                method.makeGetValueTransition = true;
            if (BlueprintGUILayout.Button(new Rect(rect.position + StateMachineSetting.Instance.setRect.position, StateMachineSetting.Instance.setRect.size), BlueprintGUILayout.Instance.setValueStyle, "set target -> " + method.method.targetTypeName, 0))
                method.makeTransition = true;
            if (BlueprintGUILayout.Button(new Rect(rect.position + StateMachineSetting.Instance.mainRect.position, StateMachineSetting.Instance.mainRect.size), BlueprintGUILayout.Instance.getRuntimeStyle, "Execution path of the last node -> ", 0))
                method.makeOutRuntimeTransition = true;
            if (BlueprintGUILayout.Button(new Rect(rect.position + StateMachineSetting.Instance.runRect.position, StateMachineSetting.Instance.runRect.size), BlueprintGUILayout.Instance.setRuntimeStyle, "Next node execution path -> ", 0))
                method.makeRuntimeTransition = true;
            rect.y += 20;
            foreach (Parameter p in method.method.Parameters)
            {
                try { p.EditorValue = BlueprintGUILayout.PropertyField(rect, p.name, p.EditorValue, p.parameterType, p.value.typeName, Image(p.image, p.parameterType), p.parameterTypeName + " " + p.name); } catch { }
                if (BlueprintGUILayout.Button(new Rect(rect.position + StateMachineSetting.Instance.setRect.position, StateMachineSetting.Instance.setRect.size), BlueprintGUILayout.Instance.setParamsStyle, "set parameter value -> " + p.parameterTypeName, 0))
                {
                    p.makeTransition = true;
                }
                if (EditorApplication.isCompiling | !EditorApplication.isPlaying)
                    p.value.ValueToString();
                rect.y += 18;
            }
            foreach (Parameter p in method.method.genericArguments)
            {
                try { p.EditorValue = BlueprintGUILayout.PropertyField(rect, "<" + p.name + ">", p.EditorValue, p.parameterType, p.value.typeName, Image(p.image, p.parameterType), p.parameterTypeName + " " + p.name); } catch { }
                if (EditorApplication.isCompiling | !EditorApplication.isPlaying)
                    p.value.ValueToString();
                rect.y += 18;
            }
            if (method.method.memberTypes == System.Reflection.MemberTypes.Property | method.method.memberTypes == System.Reflection.MemberTypes.Field)
            {
                float w = GUI.skin.label.CalcSize(new GUIContent(method.method.memberTypes.ToString())).x;
                GUI.Label(new Rect(rect.x, rect.y, w, 18), new GUIContent(method.method.memberTypes.ToString()));
                method.method.valueModel = (Method.ValueModel)EditorGUI.EnumPopup(new Rect(rect.x + w, rect.y, rect.width - w, 18), method.method.valueModel);
                rect.y += 15;
            }
            rect.y += 5;
            method.rect.height = rect.y - method.rect.y;
            if (method.rect.Contains(mousePosition) & currentType == EventType.MouseDown & currentEvent.button == 0)
            {
                if (!designer.selectNodes.Contains(method))
                {
                    designer.selectNodes = new List<Node> {
                        method
                    };
                }
            }
            else if (method.rect.Contains(mousePosition) & currentType == EventType.MouseDown & currentEvent.button == 1)
            {
                openStateMenu = true;
                designer.selectMethod = method;
            }
            if (currentEvent.keyCode == KeyCode.Delete & currentEvent.type == EventType.KeyUp)
            {
                DeletedNode();
            }
        }

        public void DrawTransition(Node method, int index)
        {
            Rect rect = method.rect;
            if (method.setValue)
            {
                DrawConnection(method.rect.position + StateMachineSetting.Instance.setRect.center - StateMachineSetting.Instance.offset, method.setValue.rect.position + StateMachineSetting.Instance.getRect.center - StateMachineSetting.Instance.offset, Color.white, 1, false, false);
            }
            if (method.runtime)
            {
                DrawConnection(method.rect.position + StateMachineSetting.Instance.runRect.center - StateMachineSetting.Instance.offset, method.runtime.rect.position + StateMachineSetting.Instance.mainRect.center - StateMachineSetting.Instance.offset, Color.green, 1, false, false);
            }
            rect.y += StateMachineSetting.Instance.topHeight;
            if (method.makeTransition)
            {
                Vector2 startpos = method.rect.position + StateMachineSetting.Instance.setRect.center - StateMachineSetting.Instance.offset;
                Vector2 endpos = Event.current.mousePosition;
                DrawConnection(startpos, endpos, Color.white, 1, true, false, 0);
                for (int a = 0; a < designer.nodes.Count; ++a)
                {
                    if (designer.nodes[a].rect.Contains(Event.current.mousePosition) & Event.current.type == EventType.MouseDown)
                    {
                        method.setValue = designer.nodes[a];
                        FOR(method.setValue, method, method.setValue);
                        method.makeTransition = false;
                        method.Invoke();//更新判断是否符合条件
                        return;
                    }
                }
                if (Event.current.button == 0 & Event.current.type == EventType.MouseDown)
                {
                    method.makeTransition = false;
                    designer.mousePosition = endpos;
                    AddBlueprintNode.designer = designer;
                    if (method.method.targetTypeName.Contains("[]"))
                    {
                        TypeInfo.instance.findtype = "Array";
                        TypeInfo.instance.typeName = "System.Array";
                    }
                    else
                    {
                        TypeInfo.instance.findtype = method.method.targetType.Name;
                        TypeInfo.instance.typeName = method.method.targetTypeName;
                    }
                    TypeInfo.InitTypeInfo(TypeInfo.instance);
                    AddBlueprintNode.typeInfo = TypeInfo.instance;
                    AddBlueprintNode.SelectionType(TypeInfo.instance.type);
                    AddBlueprintNode.Init(MousePosition);
                    showWindow = true;
                }
            }
            if (method.makeGetValueTransition)
            {
                Vector2 startpos = method.rect.position + StateMachineSetting.Instance.getRect.center - StateMachineSetting.Instance.offset;
                Vector2 endpos = Event.current.mousePosition;
                DrawConnection(endpos, startpos, Color.white, 1, true, false, 0);
                for (int a = 0; a < designer.nodes.Count; ++a)
                {
                    if (designer.nodes[a].rect.Contains(Event.current.mousePosition) & Event.current.type == EventType.MouseDown)
                    {
                        designer.nodes[a].setValue = method;
                        FOR(designer.nodes[a].setValue, method, designer.nodes[a].setValue);
                        method.makeGetValueTransition = false;
                        designer.nodes[a].Invoke();//更新判断是否符合条件
                        return;
                    }
                }
                if (Event.current.button == 0 & Event.current.type == EventType.MouseDown)
                {
                    method.makeGetValueTransition = false;
                    designer.mousePosition = endpos;
                    AddBlueprintNode.designer = designer;
                    if (method.method.returnTypeName.Contains("[]"))
                    {
                        TypeInfo.instance.findtype = "System.Array";
                        TypeInfo.instance.typeName = "System.Array";
                    }
                    else if (method.method.memberTypes == System.Reflection.MemberTypes.Custom)
                    { //有时候属于对象时使用,对象没有返回值
                        TypeInfo.instance.findtype = method.method.targetType.Name;
                        TypeInfo.instance.typeName = method.method.targetTypeName;
                    }
                    else
                    {
                        TypeInfo.instance.findtype = method.method.returnType.Name;
                        TypeInfo.instance.typeName = method.method.returnTypeName;
                    }
                    TypeInfo.InitTypeInfo(TypeInfo.instance);
                    AddBlueprintNode.typeInfo = TypeInfo.instance;
                    AddBlueprintNode.SelectionType(TypeInfo.instance.type);
                    AddBlueprintNode.Init(MousePosition);
                    showWindow = true;
                }
            }
            if (method.makeRuntimeTransition)
            {
                Vector2 startpos = method.rect.position + StateMachineSetting.Instance.runRect.center - StateMachineSetting.Instance.offset;
                Vector2 endpos = Event.current.mousePosition;
                DrawConnection(startpos, endpos, Color.green, 1, true, false, 0);
                for (int a = 0; a < designer.nodes.Count; ++a)
                {
                    if (designer.nodes[a].rect.Contains(Event.current.mousePosition) & Event.current.type == EventType.MouseDown)
                    {
                        method.runtime = designer.nodes[a];
                        FOR(method.runtime, method,  method.runtime);
                        method.makeRuntimeTransition = false;
                        method.Invoke();//更新判断是否符合条件
                        return;
                    }
                }
                if (Event.current.button == 0 & Event.current.type == EventType.MouseDown)
                {
                    method.runtime = null;
                    method.makeRuntimeTransition = false;
                }
            }
            rect.x += 10;
            rect.y += 20;
            foreach (Parameter p in method.method.Parameters)
            {
                if (p.makeTransition)
                {
                    Vector2 startpos = rect.position + StateMachineSetting.Instance.setRect.center;
                    Vector2 endpos = Event.current.mousePosition;
                    DrawConnection(startpos, endpos, Color.white, 1, true, false, 0);
                    for (int a = 0; a < designer.nodes.Count; ++a)
                    {
                        if (designer.nodes[a].rect.Contains(Event.current.mousePosition) & Event.current.type == EventType.MouseDown)
                        {
                            p.blueprint = designer;
                            p.setValue = designer.nodes[a];
                            FOR(p.setValue, method,  p.setValue);
                            if (p.setValue) { p.setValue.Invoke(); }
                            p.makeTransition = false;
                            return;
                        }
                    }
                    if (Event.current.button == 0 & Event.current.type == EventType.MouseDown)
                    {
                        p.makeTransition = false;
                        designer.mousePosition = Event.current.mousePosition;
                        AddBlueprintNode.designer = designer;
                        if (p.parameterTypeName.Contains("[]"))
                        {
                            TypeInfo.instance.findtype = "Array";
                            TypeInfo.instance.typeName = "System.Array";
                        }
                        else
                        {
                            TypeInfo.instance.findtype = p.parameterType.Name;
                            TypeInfo.instance.typeName = p.parameterTypeName;
                        }
                        TypeInfo.InitTypeInfo(TypeInfo.instance);
                        AddBlueprintNode.typeInfo = TypeInfo.instance;
                        AddBlueprintNode.SelectionType(TypeInfo.instance.type);
                        AddBlueprintNode.Init(MousePosition);
                        showWindow = true;
                    }
                }
                if (p.setValue & p.parameterTypeName == "GameDesigner.BlueprintNode")
                {
                    DrawConnection(rect.position + StateMachineSetting.Instance.setRect.center, p.setValue.rect.position + StateMachineSetting.Instance.mainRect.center - StateMachineSetting.Instance.offset, Color.cyan, 1, false, false);
                }
                else if (p.setValue)
                {
                    DrawConnection(rect.position + StateMachineSetting.Instance.setRect.center, p.setValue.rect.position + StateMachineSetting.Instance.getRect.center - StateMachineSetting.Instance.offset, Color.white, 1, false, false);
                }
                rect.y += 18;
            }
        }

        static void FOR(Node a, Node b, Node set)
        {
            if (a.setValue)
            {
                if (a.setValue == b)
                {
                    set.setValueIndex = -1;
                    Debug.Log("死循环对象值! -- 系统自动取消设置值...");
                }
                else
                {
                    For(a.setValue, b,  set);
                }
            }
            if (a.runtime)
            {
                if (a.runtime == b)
                {
                    set.runtimeIndex = -1;
                    Debug.Log("死循环运行路径! -- 系统自动取消设置值...");
                }
                else
                {
                    For(a.runtime, b, set);
                }
            }

            foreach (var pp in a.method.Parameters)
            {
                if (pp.setValue == null)
                    continue;
                if (pp.setValue == b)
                {
                    set.setValueIndex = -1;
                    Debug.Log("死循环参数! -- 系统自动取消设置值...");
                    return;
                }
                For(pp.setValue, b, set);
            }
        }

        static void For(Node a, Node b, Node set)
        {
            if (a.setValue)
            {
                if (a.setValue == b)
                {
                    set.setValueIndex = -1;
                    Debug.Log("死循环对象值! -- 系统自动取消设置值...");
                }
                else
                {
                    For(a.setValue, b, set);
                }
            }
            if (a.runtime)
            {
                if (a.runtime == b)
                {
                    set.runtimeIndex = -1;
                    Debug.Log("死循环运行路径! -- 系统自动取消设置值...");
                }
                else
                {
                    For(a.runtime, b, set);
                }
            }

            foreach (var pp in a.method.Parameters)
            {
                if (pp.setValue == null)
                    continue;
                if (pp.setValue == b)
                {
                    set.setValueIndex = -1;
                    Debug.Log("死循环参数! -- 系统自动取消设置值...");
                    return;
                }
                For(pp.setValue, b, set);
            }
        }

        /// <summary>
        /// 右键打开状态菜单
        /// </summary>
        protected void OpenStateContextMenu(Node body)
        {
            if (body == null)
            {
                openStateMenu = false;
                return;
            }

            if (currentType == EventType.MouseDown & currentEvent.button == 0)
            {
                openStateMenu = false;
            }
            else if (currentType == EventType.MouseDown & currentEvent.button == 1)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("复制节点"), false, delegate
                {
                    designer.selectMethod = body;
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("删除节点"), false, delegate
                {
                    DeletedNode();
                });
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        /// <summary>
        /// 删除蓝图节点
        /// </summary>
        private void DeletedNode()
        {
            for (int i = 0; i < designer.selectNodes.Count; i++)
            {
                var node = designer.selectNodes[i];
                designer.nodes.Remove(node);
                foreach (var node1 in designer.nodes)
                {
                    if (node1.setValueIndex >= node.ID)
                        node1.setValueIndex = -1;
                    if (node1.runtimeIndex >= node.ID)
                        node1.runtimeIndex = -1;
                    foreach (var par in node1.method.Parameters)
                    {
                        if (par.setValueIndex >= node.ID)
                            par.setValueIndex = -1;
                    }
                }
            }
            for (int i = 0; i < designer.nodes.Count; i++)
            {
                designer.nodes[i].ID = i;
            }
        }

        public bool dragState = false;
        /// <summary>
        /// 拖动状态盒位置
        /// 参数 dragRect : 拖动矩阵 ， 并且返回拖动后的这个矩阵的值
        /// 参数 style : 盒子皮肤
        /// 参数 eventButton : 事件按钮有三个主要的按钮值 ： 0 左键按钮 1 鼠标滑动按钮 2 右键按钮
        /// </summary>
        protected void DragStateBoxPosition(Rect dragRect, GUIStyle style = null)
        {
            GUI.Box(dragRect, "", style);

            if (Event.current.button == 0)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        if (dragRect.Contains(Event.current.mousePosition))
                            dragState = true;
                        break;
                    case EventType.MouseDrag:
                        if (dragState & designer.selectMethod != null)
                        {
                            foreach (var node in designer.selectNodes)
                            {
                                node.rect.position += Event.current.delta;//拖到状态按钮
                            }
                        }
                        Event.current.Use();
                        break;
                    case EventType.MouseUp:
                        dragState = false;
                        foreach (var node in designer.selectNodes)
                        {
                            foreach (var n in designer.functionalBlocks)
                            {
                                if (!n.rect.Contains(node.rect.position))
                                {
                                    n.nodes.Remove(node.ID);
                                }
                                else if(!n.nodes.Contains(node.ID))
                                {
                                    n.nodes.Add(node.ID);
                                }
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 右键打开窗口菜单
        /// </summary>

        public void OpenWindowContextMenu()
        {
            if (currentEvent.type == EventType.MouseDown & currentEvent.button == 1)
            {
                foreach (var node in designer.functionalBlocks)
                {
                    if (new Rect(node.rect.x, node.rect.y, node.rect.width, 20).Contains(mousePosition))
                    {
                        designer.selectBlock = node;
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("删除功能块面板"), false, delegate
                        {
                            //Undo.DestroyObjectImmediate(designer.selectBlock.gameObject);
                            designer.functionalBlocks.Remove(designer.selectBlock);
                        });
                        menu.ShowAsContext();
                        return;
                    }
                }
                if (designer.selectMethod != null)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("打开节点查看窗口"), false, delegate
                    {
                        designer.mousePosition = mousePosition;
                        AddBlueprintNode.designer = designer;
                        AddBlueprintNode.Init(MousePosition);
                    });
                    menu.AddItem(new GUIContent("粘贴选择状态"), false, delegate
                    {
                        List<Node> nodes = new List<Node>();
                        Node n = Net.Clone.Instance<Node>(designer.selectNodes[0]);
                        n.name = designer.selectNodes[0].name;
                        n.rect.center = mousePosition;
                        n.blueprint = designer;
                        n.preID = n.ID;
                        n.ID = designer.nodes.Count;
                        designer.nodes.Add(n);
                        nodes.Add(n);
                        Vector2 dis = designer.selectNodes[0].rect.center - mousePosition;
                        for (int i = 1; i < designer.selectNodes.Count; ++i)
                        {
                            Node nn = Net.Clone.Instance<Node>(designer.selectNodes[i]);
                            nn.name = designer.selectNodes[i].name;
                            nn.rect.position -= dis;
                            nn.blueprint = designer;
                            nn.preID = nn.ID;
                            nn.ID = designer.nodes.Count;
                            designer.nodes.Add(nn);
                            nodes.Add(nn);
                        }
                        foreach (var node in nodes)
                        {
                            if (node.runtime != null)
                            {
                                foreach (var sta in nodes)
                                {
                                    if (node.runtimeIndex == sta.preID)
                                        node.runtime = sta;
                                }
                            }
                            if (node.setValue != null)
                            {
                                foreach (var sta in nodes)
                                {
                                    if (node.setValueIndex == sta.preID)
                                        node.setValue = sta;
                                }
                            }
                            foreach (var par in node.method.Parameters)
                            {
                                if (par.setValue != null)
                                {
                                    foreach (var sta in nodes)
                                    {
                                        if (par.setValueIndex == sta.preID)
                                        {
                                            par.blueprint = sta.blueprint;
                                            par.setValue = sta;
                                        }
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < designer.nodes.Count; ++i)
                        {
                            designer.nodes[i].ID = i;
                        }
                        designer.selectNodes = nodes;
                    }
                    );
                    menu.AddItem(new GUIContent("删除选择状态"), false, delegate { DeletedNode(); });
                    menu.ShowAsContext();
                }
                else
                {
                    foreach (Node state in designer.nodes)
                    {
                        if (state.rect.Contains(mousePosition))
                            return;
                    }
                    designer.mousePosition = mousePosition;
                    AddBlueprintNode.designer = designer;
                    AddBlueprintNode.Init(MousePosition);
                }
            }
            if (currentEvent.type == EventType.DragUpdated | Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;//拖动时显示辅助图标
                if (currentEvent.type == EventType.DragPerform)
                {
                    designer.mousePosition = mousePosition;
                    TypeInfo.instance.typeName = DragAndDrop.objectReferences[0].GetType().FullName;
                    TypeInfo.InitTypeInfo(TypeInfo.instance);
                    AddBlueprintNode.typeInfo = TypeInfo.instance;
                    AddBlueprintNode.SelectionType(TypeInfo.instance.type);
                    AddBlueprintNode.Init(MousePosition);
                }
            }
        }
    }
}
#endif