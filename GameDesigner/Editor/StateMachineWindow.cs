#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameDesigner
{
    public class StateMachineWindow : GraphEditor
    {
        public static StateMachine stateMachine = null;//状态管理器是通过Editor编辑器脚本自动赋值给此对象的
        public static StateMachineWindow Instance
        {
            get
            {
                if (BlueprintGUILayout.Instance.GraphEditor == null)
                {
                    BlueprintGUILayout.Instance.GraphEditor = GetWindow<StateMachineWindow>(BlueprintGUILayout.Instance.LANGUAGE[84], true);
                }
                return BlueprintGUILayout.Instance.GraphEditor as StateMachineWindow;
            }
        }

        [MenuItem("GameDesigner/StateMachine/StateMachine")]
        public static void Init()
        {
            BlueprintGUILayout.Instance.GraphEditor = GetWindow<StateMachineWindow>(BlueprintGUILayout.Instance.LANGUAGE[84], true);
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Button(new GUIContent(stateMachine ? stateMachine.name : "NoStateMachineSelected !", BlueprintGUILayout.Instance.stateMachineImage), GUI.skin.GetStyle("GUIEditor.BreadcrumbLeft"), GUILayout.Width(180));
            stateMachine = (StateMachine)EditorGUILayout.ObjectField(GUIContent.none, stateMachine, typeof(StateMachine), true, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            if (GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[85], GUI.skin.GetStyle("GUIEditor.BreadcrumbLeft"), GUILayout.Width(50)))
            {
                if (stateMachine == null)
                    return;
                if (stateMachine.states.Count > 0)
                {
                    UpdateScrollPosition(stateMachine.states[0].rect.position - new Vector2(position.size.x / 2 - 75, position.size.y / 2 - 15)); //  更新滑动矩阵
                }
                else
                    UpdateScrollPosition(Center); //  归位到矩形的中心
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            ZoomableAreaBegin(new Rect(0f, 0f, scaledCanvasSize.width, scaledCanvasSize.height + 21), scale, false);
            BeginWindow();
            if (stateMachine)
                DrawStates();
            EndWindow();
            ZoomableAreaEnd();
            if (stateMachine == null)
                CreateStateMachineMenu();
            else if (openStateMenu)
                OpenStateContextMenu(stateMachine.selectState);
            else
                OpenWindowContextMenu();
            Repaint();
        }

        private void CreateStateMachineMenu()
        {
            if (currentType == EventType.MouseDown & Event.current.button == 1)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[86]), false, delegate
                {
                    if (Selection.activeGameObject == null)
                    {
                        EditorUtility.DisplayDialog(
                            BlueprintGUILayout.Instance.LANGUAGE[87],
                            BlueprintGUILayout.Instance.LANGUAGE[88],
                            BlueprintGUILayout.Instance.LANGUAGE[89],
                            BlueprintGUILayout.Instance.LANGUAGE[90]);
                    }
                    else if (Selection.activeGameObject.GetComponent<StateManager>())
                    {
                        Selection.activeGameObject.GetComponent<StateManager>().stateMachine = StateMachine.CreateStateMachineInstance();
                        Selection.activeGameObject.GetComponent<StateManager>().stateMachine.transform.SetParent(Selection.activeGameObject.GetComponent<StateManager>().transform);
                        stateMachine = Selection.activeGameObject.GetComponent<StateManager>().stateMachine;
                    }
                    else
                    {
                        Selection.activeGameObject.AddComponent<StateManager>().stateMachine = StateMachine.CreateStateMachineInstance();
                        Selection.activeGameObject.GetComponent<StateManager>().stateMachine.transform.SetParent(Selection.activeGameObject.GetComponent<StateManager>().transform);
                        stateMachine = Selection.activeGameObject.GetComponent<StateManager>().stateMachine;
                    }
                });
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        /// <summary>
        /// 绘制状态(状态的层,状态窗口举行)
        /// </summary>
        protected void DrawStates()
        {
            stateMachine.gameObject.hideFlags = BlueprintGUILayout.Instance.StateMachineHideFlags;
            foreach (var state in stateMachine.states)
            {
                state.gameObject.hideFlags = BlueprintGUILayout.Instance.StateHideFlags;
                DrawLineStatePosToMousePosTransition(state);
                foreach (var t in state.transitions)
                {
                    if (stateMachine.selectTransition == t)
                    {
                        DrawConnection(state.rect.center, t.nextState.rect.center, Color.green, 1, true);
                        if (Event.current.keyCode == KeyCode.Delete)
                        {
                            state.transitions.Remove(t);
                            Undo.DestroyObjectImmediate(t.gameObject);
                            return;
                        }
                        ClickTransition(state, t);
                    }
                    else
                    {
                        DrawConnection(state.rect.center, t.nextState.rect.center, Color.white, 1, true);
                        ClickTransition(state, t);
                    }
                }
                if (state.rect.Contains(Event.current.mousePosition) & currentType == EventType.MouseDown & Event.current.button == 0)
                {
                    if (Event.current.control)
                        stateMachine.selectState = state;
                    else if (!stateMachine.selectStates.Contains(state))
                    {
                        stateMachine.selectStates = new List<State>();
                        stateMachine.selectStates.Add(state);
                    }
                    stateMachine.selectTransition = state.transitions.Count > 0 ? state.transitions[0] : null;
                    switch (BlueprintGUILayout.Instance.selectObjMode)
                    {
                        case SelectObjMode.SelectionStateManager:
                            Selection.activeObject = stateMachine.stateManager;
                            break;
                        case SelectObjMode.SelectionStateMachine:
                            Selection.activeObject = stateMachine;
                            break;
                        case SelectObjMode.SelectionStateObject:
                            Selection.activeObject = state;
                            break;
                    }
                }
                else if (state.rect.Contains(mousePosition) & currentType == EventType.MouseDown & currentEvent.button == 1)
                {
                    openStateMenu = true;
                    stateMachine.selectState = state;
                }
                if (currentEvent.keyCode == KeyCode.Delete & currentEvent.type == EventType.KeyUp)
                {
                    DeletedState();
                    return;
                }
            }
            foreach (var state in stateMachine.states)
            {
                if (state == stateMachine.defaultState & stateMachine.selectState == stateMachine.defaultState)
                    DragStateBoxPosition(state.rect, state.name, StateMachineSetting.Instance.defaultAndSelectStyle);
                else if (state == stateMachine.defaultState & state.stateID == stateMachine.stateIndex)
                    DragStateBoxPosition(state.rect, state.name, StateMachineSetting.Instance.defaultAndRuntimeIndexStyle);
                else if (state == stateMachine.defaultState)
                    DragStateBoxPosition(state.rect, state.name, StateMachineSetting.Instance.stateInDefaultStyle);
                else if (stateMachine.stateIndex == state.stateID)
                    DragStateBoxPosition(state.rect, state.name, StateMachineSetting.Instance.indexInRuntimeStyle);
                else if (state == stateMachine.selectState)
                    DragStateBoxPosition(state.rect, state.name, StateMachineSetting.Instance.selectStateStyle);
                else
                    DragStateBoxPosition(state.rect, state.name, StateMachineSetting.Instance.defaultStyle);
            }
            DragSelectStates();
        }

        /// <summary>
        /// 绘制选择状态
        /// </summary>
        private void DragSelectStates()
        {
            for (int i = 0; i < stateMachine.selectStates.Count; i++)
            {
                if (stateMachine.selectStates[i] == null)
                {
                    stateMachine.selectStates.RemoveAt(i);
                    continue;
                }
                DragStateBoxPosition(stateMachine.selectStates[i].rect, stateMachine.selectStates[i].name, StateMachineSetting.Instance.selectStateStyle);
            }

            switch (currentType)
            {
                case EventType.MouseDown:
                    selectionStartPosition = mousePosition;
                    if (currentEvent.button == 2 | currentEvent.button == 1)
                    {
                        mode = SelectMode.none;
                        return;
                    }
                    foreach (State state in stateMachine.states)
                    {
                        if (state.rect.Contains(currentEvent.mousePosition))
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
                    if (stateMachine.selectState)
                        DragStateBoxPosition(stateMachine.selectState.rect, stateMachine.selectState.name, StateMachineSetting.Instance.selectStateStyle);
                    break;
                case SelectMode.selectState:
                    GUI.Box(FromToRect(selectionStartPosition, mousePosition), "", "SelectionRect");
                    SelectStatesInRect(FromToRect(selectionStartPosition, mousePosition));
                    break;
            }
        }

        private void SelectStatesInRect(Rect r)
        {
            for (int i = 0; i < stateMachine.states.Count; i++)
            {
                Rect rect = stateMachine.states[i].rect;
                if (rect.xMax < r.x || rect.x > r.xMax || rect.yMax < r.y || rect.y > r.yMax)
                {
                    stateMachine.selectStates.Remove(stateMachine.states[i]);
                    continue;
                }
                if (!stateMachine.selectStates.Contains(stateMachine.states[i]))
                {
                    stateMachine.selectStates.Add(stateMachine.states[i]);
                }
                DragStateBoxPosition(stateMachine.states[i].rect, stateMachine.states[i].name, StateMachineSetting.Instance.selectStateStyle);
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

        /// <summary>
        /// 点击连接线条
        /// </summary>

        protected void ClickTransition(State state, Transition t)
        {
            if (state.rect.Contains(mousePosition) | t.nextState.rect.Contains(mousePosition))
                return;

            if (currentType == EventType.MouseDown)
            {
                bool offset = state.stateID > t.nextState.stateID ? true : false;
                Vector3 start = state.rect.center;
                Vector3 end = t.nextState.rect.center;
                Vector3 cross = Vector3.Cross((start - end).normalized, Vector3.forward);
                if (offset)
                {
                    start = start + cross * 6;
                    end = end + cross * 6;
                }
                if (HandleUtility.DistanceToLine(start, end) < 8f)
                {  //返回到线的距离
                    stateMachine.selectTransition = t;
                    stateMachine.selectState = state;
                    switch (BlueprintGUILayout.Instance.selectObjMode)
                    {
                        case SelectObjMode.SelectionStateManager:
                            Selection.activeObject = stateMachine.stateManager;
                            break;
                        case SelectObjMode.SelectionStateMachine:
                            Selection.activeObject = stateMachine;
                            break;
                        case SelectObjMode.SelectionStateObject:
                            Selection.activeObject = t;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 绘制一条从状态点到鼠标位置的线条
        /// </summary>

        protected void DrawLineStatePosToMousePosTransition(State state)
        {
            if (state == null)
                return;

            if (state.makeTransition)
            {
                Vector2 startpos = new Vector2(state.rect.x + 80, state.rect.y + 15);
                Vector2 endpos = currentEvent.mousePosition;
                DrawConnection(startpos, endpos, Color.white, 1, true);
                if (currentEvent.button == 0 & currentType == EventType.MouseDown)
                {
                    foreach (var s in stateMachine.states)
                    {
                        if (state != s & s.rect.Contains(mousePosition))
                        {
                            foreach (var t in state.transitions)
                            {
                                if (t.nextState == s)
                                { // 如果拖动的线包含在自身状态盒矩形内,则不添加连接线
                                    state.makeTransition = false;
                                    return;
                                }
                            }
                            Transition tran = Transition.CreateTransitionInstance(state, s);
                            tran.transform.SetParent(state.transform);
                            break;
                        }
                    }
                    state.makeTransition = false;
                }
            }
        }

        /// <summary>
        /// 右键打开状态菜单
        /// </summary>
        protected void OpenStateContextMenu(State state)
        {
            if (state == null)
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
                menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[91]), false, delegate
                {
                    state.makeTransition = true;
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[92]), false, delegate
                {
                    stateMachine.defaultState = state;
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[93]), false, delegate
                {
                    stateMachine.selectState = state;
                });
                menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[94]), false, delegate { DeletedState(); });
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        /// <summary>
        /// 删除状态节点
        /// </summary>
        private void DeletedState()
        {
            for (int i = 0; i < stateMachine.selectStates.Count; i++)
            {
                Undo.DestroyObjectImmediate(stateMachine.selectStates[i].gameObject);
                stateMachine.states.Remove(stateMachine.selectStates[i]);
            }
            for (int i = 0; i < stateMachine.states.Count; i++)
            {
                stateMachine.states[i].stateID = i;
                for (int n = 0; n < stateMachine.states[i].transitions.Count; n++)
                {
                    if (stateMachine.states[i].transitions[n] == null)
                    {
                        stateMachine.states[i].transitions.RemoveAt(n);
                    }
                    else if (stateMachine.states[i].transitions[n].nextState == null)
                    {//如果这个链接要连接的下一个状态是即将被删除的状态索引，则删除这个链接，因为当删除状态后链接连接到的是一个空的状态
                        Undo.DestroyObjectImmediate(stateMachine.states[i].transitions[n].gameObject);
                        stateMachine.states[i].transitions.RemoveAt(n);
                    }
                }
            }
        }

        /// <summary>
        /// 右键打开窗口菜单
        /// </summary>

        protected void OpenWindowContextMenu()
        {
            if (stateMachine == null)
                return;

            if (currentType == EventType.MouseDown)
            {
                if (currentEvent.button == 1)
                {
                    foreach (State state in stateMachine.states)
                    {
                        if (state.rect.Contains(currentEvent.mousePosition))
                            return;
                    }
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[95]), false,
                        delegate
                        {
                            State s = State.CreateStateInstance(stateMachine, BlueprintGUILayout.Instance.LANGUAGE[96] + stateMachine.states.Count, mousePosition);
                            Undo.RegisterCreatedObjectUndo(s.gameObject, s.name);
                            for (int i = 0; i < stateMachine.states.Count; ++i)
                            {
                                stateMachine.states[i].stateID = i;
                            }
                        }
                    );
                    if (stateMachine.selectState != null)
                    {
                        menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[97]), false,
                            delegate
                            {
                                List<State> states = new List<State>();
                                State s = Instantiate(stateMachine.selectStates[0], stateMachine.transform);
                                s.name = stateMachine.selectStates[0].name;
                                s.rect.center = mousePosition;
                                stateMachine.states.Add(s);
                                states.Add(s);
                                Vector2 dis = stateMachine.selectStates[0].rect.center - mousePosition;
                                Undo.RegisterCreatedObjectUndo(s.gameObject, s.name);
                                for (int i = 1; i < stateMachine.selectStates.Count; ++i)
                                {
                                    State ss = Instantiate(stateMachine.selectStates[i], stateMachine.transform);
                                    ss.name = stateMachine.selectStates[i].name;
                                    ss.rect.position -= dis;
                                    stateMachine.states.Add(ss);
                                    states.Add(ss);
                                    Undo.RegisterCreatedObjectUndo(ss.gameObject, ss.name);
                                }
                                foreach (var state in states)
                                    foreach (var tran in state.transitions)
                                        foreach (var sta in states)
                                            if (tran.nextState.stateID == sta.stateID)
                                                tran.nextState = sta;
                                for (int i = 0; i < stateMachine.states.Count; ++i)
                                {
                                    stateMachine.states[i].stateID = i;
                                }
                                stateMachine.selectStates = states;
                            }
                        );
                        menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[98]), false, delegate { DeletedState(); });
                    }
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[99]), false, delegate
                {
                    if (Selection.activeGameObject == null)
                    {
                        EditorUtility.DisplayDialog(
                            BlueprintGUILayout.Instance.LANGUAGE[100],
                            BlueprintGUILayout.Instance.LANGUAGE[101],
                            BlueprintGUILayout.Instance.LANGUAGE[102],
                            BlueprintGUILayout.Instance.LANGUAGE[103]);
                        return;
                    }
                    StateManager manager = Selection.activeGameObject.GetComponent<StateManager>();
                    if (manager == null)
                        manager = Selection.activeGameObject.AddComponent<StateManager>();
                    else if (manager.stateMachine != null)
                        Undo.DestroyObjectImmediate(manager.stateMachine.gameObject);
                    StateMachine machine = StateMachine.CreateStateMachineInstance();
                    Undo.RegisterCreatedObjectUndo(machine.gameObject, machine.name);
                    manager.stateMachine = machine;
                    machine.transform.SetParent(manager.transform);
                });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[104]), false, () =>
                {
                    if (Selection.activeGameObject == null)
                    {
                        EditorUtility.DisplayDialog(
                            BlueprintGUILayout.Instance.LANGUAGE[105],
                            BlueprintGUILayout.Instance.LANGUAGE[106],
                            BlueprintGUILayout.Instance.LANGUAGE[107],
                            BlueprintGUILayout.Instance.LANGUAGE[108]);
                        return;
                    }
                    StateManager manager = Selection.activeGameObject.GetComponent<StateManager>();
                    if (manager == null)
                        manager = Selection.activeGameObject.AddComponent<StateManager>();
                    StateMachine machine = StateMachine.CreateStateMachineInstance(BlueprintGUILayout.Instance.LANGUAGE[109]);
                    Undo.RegisterCreatedObjectUndo(machine.gameObject, machine.name);
                    manager.stateMachine = machine;
                    machine.transform.SetParent(manager.transform);
                });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[110]), false, () =>
                {
                    if (stateMachine == null)
                        return;
                    Undo.DestroyObjectImmediate(stateMachine.gameObject);
                });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[111]), false, () =>
                    {
                        if (stateMachine == null)
                            return;
                        if (stateMachine.stateManager == null)
                            return;
                        Undo.DestroyObjectImmediate(stateMachine.gameObject);
                        Undo.DestroyObjectImmediate(stateMachine.stateManager);
                    });
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }
        }

        bool dragState = false;

        /// <summary>
        /// 拖动状态盒位置
        /// 参数 dragRect : 拖动矩阵 ， 并且返回拖动后的这个矩阵的值
        /// 参数 target : 可以传入三种类型对象 ， string , GUIContent , Texture
        /// 参数 style : 盒子皮肤
        /// 参数 eventButton : 事件按钮有三个主要的按钮值 ： 0 左键按钮 1 鼠标滑动按钮 2 右键按钮
        /// </summary>

        protected Rect DragStateBoxPosition(Rect dragRect, string name, GUIStyle style = null, int eventButton = 0)
        {
            GUI.Box(dragRect, name, style);

            if (Event.current.button == eventButton)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        if (dragRect.Contains(Event.current.mousePosition))
                            dragState = true;
                        break;
                    case EventType.MouseDrag:
                        if (dragState & stateMachine.selectState != null)
                        {
                            foreach (var state in stateMachine.selectStates)
                            {
                                state.rect.position += Event.current.delta;//拖到状态按钮
                            }
                        }
                        Event.current.Use();
                        break;
                    case EventType.MouseUp:
                        dragState = false;
                        break;
                }
            }
            return dragRect;
        }
    }
}
#endif