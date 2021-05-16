#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameDesigner
{
    [CustomEditor(typeof(StateManager))]
    public class StateManagerEditor : Editor
    {
        private StateManager stateManager = null;

        private static DirectoryInfo directoryInfo;
        public static string GetGameDesignerPath
        {
            get
            {
                if (directoryInfo == null)
                {
                    directoryInfo = new DirectoryInfo(Application.dataPath);
                    var dirs = directoryInfo.GetDirectories("GameDesigner", SearchOption.AllDirectories);
                    if (dirs.Length > 0)
                    {
                        directoryInfo = dirs[0];
                    }
                }
                return directoryInfo.FullName;
            }
        }

        void OnEnable()
        {
            stateManager = target as StateManager;
            stateManager.animation = stateManager.GetComponent<Animation>();
            if (stateManager.animation != null)
            {
                if (stateManager.clipNames.Count != AnimationUtility.GetAnimationClips(stateManager.gameObject).Length)
                {
                    stateManager.clipNames = new List<string>();
                    foreach (AnimationClip clip in AnimationUtility.GetAnimationClips(stateManager.gameObject))
                    {
                        stateManager.clipNames.Add(clip.name);
                    }
                }
            }
            stateManager.animator = stateManager.GetComponent<Animator>();
            if (stateManager.animator != null)
            {
                if (stateManager.clipNames.Count != stateManager.animator.runtimeAnimatorController.animationClips.Length)
                {
                    stateManager.clipNames = new List<string>();
                    foreach (AnimationClip clip in stateManager.animator.runtimeAnimatorController.animationClips)
                    {
                        stateManager.clipNames.Add(clip.name);
                    }
                }
            }
            StateMachineWindow.stateMachine = stateManager.stateMachine;
        }

        public override void OnInspectorGUI()
        {
            stateManager.stateMachine = (StateMachine)EditorGUILayout.ObjectField(BlueprintGUILayout.Instance.LANGUAGE[0], stateManager.stateMachine, typeof(StateMachine), true);
            if (GUILayout.Button(BlueprintSetting.Instance.LANGUAGE[1], GUI.skin.GetStyle("LargeButtonMid"), GUILayout.ExpandWidth(true)))
                StateMachineWindow.Init();
            EditorGUILayout.Space();
            if (stateManager.stateMachine == null)
                return;
            try
            {
                if (stateManager.stateMachine.selectState != null)
                {
                    DrawState(stateManager.stateMachine.selectState, stateManager);
                    EditorGUILayout.Space();
                    for (int i = 0; i < stateManager.stateMachine.selectState.transitions.Count; ++i)
                    {
                        DrawTransition(stateManager.stateMachine.selectState.transitions[i]);
                    }
                }
                else if (stateManager.stateMachine.selectTransition != null)
                {
                    DrawTransition(stateManager.stateMachine.selectTransition);
                }
            }
            catch { }
            EditorGUILayout.Space();
            Repaint();
        }

        /// <summary>
        /// 绘制状态监视面板属性
        /// </summary>
        public static void DrawState(State s, StateManager man = null)
        {
            SerializedObject serializedObject = new SerializedObject(s);
            serializedObject.Update();
            GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[2], GUI.skin.GetStyle("dragtabdropwindow"));
            EditorGUILayout.BeginVertical("ProgressBarBack");
            s.name = EditorGUILayout.TextField(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[3], "name"), s.name);
            EditorGUILayout.IntField(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[4], "stateID"), s.stateID);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actionSystem"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[5], "actionSystem  专为玩家角色AI其怪物AI所设计的一套AI系统！"), true);
            if (s.actionSystem)
            {
                man.stateMachine.animMode = (AnimationMode)EditorGUILayout.EnumPopup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[6], "animMode"), man.stateMachine.animMode);
                if (man.stateMachine.animMode == AnimationMode.Animation)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("anim"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[7], "anim"), true);
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("animator"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[8], "animator"), true);
                }
                s.animPlayMode = (AnimPlayMode)EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[9], "animPlayMode"), (int)s.animPlayMode, new GUIContent[]{
                    new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[10],"Random"),
                    new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[11],"Sequence") }
                );
                EditorGUILayout.PropertyField(serializedObject.FindProperty("animSpeed"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[12], "animSpeed"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("animLoop"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[13], "animLoop"), true);
                s.isExitState = EditorGUILayout.Toggle(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[14], "isExitState"), s.isExitState);
                if (s.isExitState)
                {
                    s.DstStateID = EditorGUILayout.Popup(BlueprintGUILayout.Instance.LANGUAGE[15], s.DstStateID, Array.ConvertAll(s.transitions.ToArray(), new Converter<Transition, string>(delegate (Transition t) { return t.currState.name + " -> " + t.nextState.name + "   ID:" + t.nextState.stateID; })));
                }
                BlueprintGUILayout.BeginStyleVertical(BlueprintGUILayout.Instance.LANGUAGE[16], "ProgressBarBack");
                EditorGUI.indentLevel = 1;
                Rect actRect = EditorGUILayout.GetControlRect();
                s.foldout = EditorGUI.Foldout(new Rect(actRect.position, new Vector2(actRect.size.x - 120f, 15)), s.foldout, BlueprintGUILayout.Instance.LANGUAGE[17], true);

                if (GUI.Button(new Rect(new Vector2(actRect.size.x - 40f, actRect.position.y), new Vector2(60, 16)), BlueprintGUILayout.Instance.LANGUAGE[18]))
                {
                    s.actions.Add(new StateAction());
                }
                if (GUI.Button(new Rect(new Vector2(actRect.size.x - 100, actRect.position.y), new Vector2(60, 16)), BlueprintGUILayout.Instance.LANGUAGE[19]))
                {
                    if (s.actions.Count > 1)
                    {
                        foreach (ActionBehaviour behaviour in s.actions[s.actions.Count - 1].behaviours)
                        {
                            DestroyImmediate(behaviour.gameObject, true);
                        }
                        s.actions.RemoveAt(s.actions.Count - 1);
                    }
                }

                if (s.foldout)
                {
                    EditorGUI.indentLevel = 2;
                    for (int a = 0; a < s.actions.Count; ++a)
                    {
                        StateAction act = s.actions[a];
                        Rect foldoutRect = EditorGUILayout.GetControlRect();
                        act.foldout = EditorGUI.Foldout(foldoutRect, act.foldout, new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[20] + a, "actions[" + a + "]"), true);
                        if (foldoutRect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                        {
                            s.actionMenuIndex = a;
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[21]), false, delegate ()
                            {
                                foreach (ActionBehaviour behaviour in s.actions[s.actionMenuIndex].behaviours)
                                {
                                    DestroyImmediate(behaviour, true);
                                }
                                s.actions.RemoveAt(s.actionMenuIndex);
                                return;
                            });
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[22]), false, delegate ()
                            {
                                StateSystem.Component = s.actions[s.actionMenuIndex];
                            });
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[23]), StateSystem.CopyComponent ? true : false, delegate ()
                            {
                                if (StateSystem.Component != null)
                                {
                                    if (StateSystem.Component.GetType() == typeof(StateAction))
                                    {
                                        StateAction stateAction = StateSystem.Component as StateAction;
                                        List<ActionBehaviour> actionBehaviours = new List<ActionBehaviour>();
                                        foreach (var behaviour in stateAction.behaviours)
                                        {
                                            ActionBehaviour actionBehaviour = (ActionBehaviour)s.gameObject.AddComponent(behaviour.GetType());
                                            actionBehaviours.Add(actionBehaviour);
                                            SystemType.SetFieldValue(actionBehaviour, behaviour);
                                        }
                                        StateAction stateAction1 = new StateAction();
                                        SystemType.SetFieldValue(stateAction1, stateAction);
                                        stateAction1.behaviours.AddRange(actionBehaviours);
                                        s.actions.Add(stateAction1);
                                    }
                                }
                            });
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[24]), StateSystem.CopyComponent ? true : false, delegate ()
                            {
                                if (StateSystem.Component != null)
                                {
                                    if (StateSystem.Component == s.actions[s.actionMenuIndex])
                                    {//如果要黏贴的动作是复制的动作则返回
                                        return;
                                    }
                                    if (StateSystem.Component.GetType().FullName == s.actions[s.actionMenuIndex].GetType().FullName)
                                    {
                                        foreach (var behaviour in s.actions[s.actionMenuIndex].behaviours)//删除原先动作脚本,然后即将创建黏贴的脚本
                                        {
                                            DestroyImmediate(behaviour, true);
                                        }
                                        s.actions[s.actionMenuIndex].behaviours = new List<ActionBehaviour>();
                                        StateAction stateAction = StateSystem.Component as StateAction;
                                        List<ActionBehaviour> actionBehaviours = new List<ActionBehaviour>();
                                        foreach (var behaviour in stateAction.behaviours)
                                        {
                                            ActionBehaviour actionBehaviour = (ActionBehaviour)s.gameObject.AddComponent(behaviour.GetType());
                                            actionBehaviours.Add(actionBehaviour);
                                            SystemType.SetFieldValue(actionBehaviour, behaviour);
                                        }
                                        SystemType.SetFieldValue(s.actions[s.actionMenuIndex], stateAction);
                                        s.actions[s.actionMenuIndex].behaviours.AddRange(actionBehaviours);
                                        return;
                                    }
                                }
                            });
                            menu.ShowAsContext();
                        }
                        if (act.foldout)
                        {
                            EditorGUI.indentLevel = 3;
                            try
                            {
                                act.clipIndex = EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[25], "clipIndex"), act.clipIndex, Array.ConvertAll(s.stateMachine.stateManager.clipNames.ToArray(), new Converter<string, GUIContent>(delegate (string input)
                                {
                                    return new GUIContent(input);
                                })));
                                act.clipName = s.stateManager.clipNames[act.clipIndex];
                            }
                            catch { }

                            s.actions[a].isPlayAudio = EditorGUILayout.Toggle(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[26], "isPlayAudio"), s.actions[a].isPlayAudio);
                            if (s.actions[a].isPlayAudio)
                            {
                                act.audioModel = (AudioMode)EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[27], "audioModel"), (int)act.audioModel,
                                    new GUIContent[]{ new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[28],"EnterPlayAudio") ,
                                        new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[29],"AnimEventPlayAudio") ,
                                        new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[30],"ExitPlayAudio") }
                                    );
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("actions").GetArrayElementAtIndex(a).FindPropertyRelative("audioClips"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[31], "actions"), true);
                            }
                            act.animTime = EditorGUILayout.FloatField(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[32], "animTime"), act.animTime);
                            act.animTimeMax = EditorGUILayout.FloatField(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[33], "animTimeMax"), act.animTimeMax);
                            act.animEventTime = EditorGUILayout.FloatField(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[34], "animEventTime"), act.animEventTime);

                            act.effectSpwan = EditorGUILayout.ObjectField(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[35], "effectSpwan"), act.effectSpwan, typeof(Object), true);
                            act.activeMode = (ActiveMode)EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[36], "activeModel"), (int)act.activeMode, new GUIContent[]{
                                new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[37],"Instantiate") ,
                                new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[38],"SetActive") }
                                );
                            if (act.activeMode == ActiveMode.SetActive)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("actions").GetArrayElementAtIndex(a).FindPropertyRelative("activeObjs"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[39], "activeObjs"), true);
                            }
                            act.spwanmode = (SpwanMode)EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[40], "spwanmode"), (int)act.spwanmode, new GUIContent[]{
                                new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[41],"TransformPoint") ,
                                new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[42],"SetParent") ,
                                new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[43],"localPosition") }
                            );
                            if (act.spwanmode != SpwanMode.localPosition)
                            {
                                act.parent = EditorGUILayout.ObjectField(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[44], "parent"), act.parent, typeof(Transform), true) as Transform;
                            }
                            act.effectPostion = EditorGUILayout.Vector3Field(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[45], "effectPostion"), act.effectPostion);
                            act.spwanTime = EditorGUILayout.FloatField(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[46], "spwanTime"), act.spwanTime);

                            for (int i = 0; i < act.behaviours.Count; ++i)
                            {
                                if (act.behaviours[i] == null)
                                {
                                    act.behaviours.RemoveAt(i);
                                    continue;
                                }
                                EditorGUILayout.BeginHorizontal();
                                Rect rect = EditorGUILayout.GetControlRect();
                                act.behaviours[i].show = EditorGUI.Foldout(new Rect(rect.x, rect.y, 50, rect.height), act.behaviours[i].show, GUIContent.none);
                                act.behaviours[i].Active = EditorGUI.ToggleLeft(new Rect(rect.x + 5, rect.y, 70, rect.height), GUIContent.none, act.behaviours[i].Active);
                                EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 15, rect.height), act.behaviours[i].GetType().Name, GUI.skin.GetStyle("BoldLabel"));
                                if (GUI.Button(new Rect(rect.x + rect.width - 15, rect.y, rect.width, rect.height), GUIContent.none, GUI.skin.GetStyle("ToggleMixed")))
                                {
                                    act.behaviours[act.behaviourMenuIndex].OnDestroyComponent();
                                    DestroyImmediate(act.behaviours[i], true);
                                    act.behaviours.RemoveAt(i);
                                    continue;
                                }
                                if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                                {
                                    GenericMenu menu = new GenericMenu();
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[47]), false, delegate ()
                                    {
                                        act.behaviours[act.behaviourMenuIndex].OnDestroyComponent();
                                        DestroyImmediate(act.behaviours[act.behaviourMenuIndex], true);
                                        act.behaviours.RemoveAt(act.behaviourMenuIndex);
                                        return;
                                    });
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[48]), false, delegate ()
                                    {
                                        StateSystem.CopyComponent = act.behaviours[act.behaviourMenuIndex];
                                    });
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[49]), StateSystem.CopyComponent ? true : false, delegate ()
                                    {
                                        if (StateSystem.CopyComponent)
                                        {
                                            if (StateSystem.CopyComponent.GetType().BaseType == typeof(ActionBehaviour))
                                            {
                                                ActionBehaviour ab = (ActionBehaviour)s.gameObject.AddComponent(StateSystem.CopyComponent.GetType());
                                                act.behaviours.Add(ab);
                                                SystemType.SetFieldValue(ab, StateSystem.CopyComponent);
                                            }
                                        }
                                    });
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[50]), StateSystem.CopyComponent ? true : false, delegate ()
                                    {
                                        if (StateSystem.CopyComponent)
                                        {
                                            if (StateSystem.CopyComponent.GetType().FullName == act.behaviours[act.behaviourMenuIndex].GetType().FullName)
                                            {
                                                SystemType.SetFieldValue(act.behaviours[act.behaviourMenuIndex], StateSystem.CopyComponent);
                                            }
                                        }
                                    });
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[78]), false, delegate ()
                                    {
                                        string scriptName = act.behaviours[act.behaviourMenuIndex].GetType().Name;
                                        string[] filePath = Directory.GetFiles(Application.dataPath, scriptName + ".cs", SearchOption.AllDirectories);
                                        if (filePath.Length > 0)
                                        {
                                            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(filePath[0]);
                                            System.Diagnostics.Process p = new System.Diagnostics.Process
                                            {
                                                StartInfo = psi
                                            };
                                            p.Start();
                                            return;
                                        }
                                        Debug.Log("找不到脚本文件，类名必须是脚本文件名才能打开成功");
                                    });
                                    menu.ShowAsContext();
                                    act.behaviourMenuIndex = i;
                                }
                                EditorGUILayout.EndHorizontal();
                                if (act.behaviours[i].show)
                                {
                                    EditorGUI.indentLevel = 4;
                                    EditorGUILayout.ObjectField("Script", act.behaviours[i], typeof(StateAction), true);
                                    SerializedObject actSerializedObject = new SerializedObject(act.behaviours[i]);
                                    actSerializedObject.Update();
                                    if (!act.behaviours[i].OnInspectorGUI(s))
                                    {
                                        foreach (FieldInfo f in act.behaviours[i].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                                        {
                                            if (act.behaviours[i].GetType() == f.DeclaringType & !f.IsStatic & f.FieldType.FullName != "System.Object")
                                                try { EditorGUILayout.PropertyField(actSerializedObject.FindProperty(f.Name), true); } catch { Debug.Log(f.Name + "  " + f.FieldType.FullName); }
                                        }
                                    }
                                    actSerializedObject.ApplyModifiedProperties();
                                    GUILayout.Space(4);
                                    GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
                                    GUILayout.Space(4);
                                    EditorGUI.indentLevel = 3;
                                }
                            }

                            Rect r = EditorGUILayout.GetControlRect();
                            Rect rr = new Rect(new Vector2(r.x + (r.size.x / 4f), r.y), new Vector2(r.size.x / 2f, 20));
                            if (GUI.Button(rr, BlueprintGUILayout.Instance.LANGUAGE[51]))
                            {
                                act.findBehaviours = true;
                            }

                            if (act.findBehaviours)
                            {
                                EditorGUILayout.Space();
                                try
                                {
                                    Type[] types = Assembly.Load("Assembly-CSharp").GetTypes().Where(t => t.IsSubclassOf(typeof(ActionBehaviour))).ToArray();
                                    foreach (Type type in types)
                                    {
                                        if (GUILayout.Button(type.Name))
                                        {
                                            ActionBehaviour stb = (ActionBehaviour)s.gameObject.AddComponent(type);
                                            act.behaviours.Add(stb);
                                            act.findBehaviours = false;
                                        }
                                        if (s.compiling & type.Name == act.createScriptName)
                                        {
                                            ActionBehaviour stb = (ActionBehaviour)s.gameObject.AddComponent(type);
                                            act.behaviours.Add(stb);
                                            act.findBehaviours = false;
                                            s.compiling = false;
                                        }
                                    }
                                }
                                catch { }
                                EditorGUILayout.Space();
                                EditorGUI.indentLevel = 0;
                                EditorGUILayout.LabelField(BlueprintGUILayout.Instance.LANGUAGE[52]);
                                IState.StateActionScriptPath = EditorGUILayout.TextField(IState.StateActionScriptPath);
                                Rect addRect = EditorGUILayout.GetControlRect();
                                act.createScriptName = EditorGUI.TextField(new Rect(addRect.position, new Vector2(addRect.size.x - 125f, 18)), act.createScriptName);
                                if (GUI.Button(new Rect(new Vector2(addRect.size.x - 100f, addRect.position.y), new Vector2(120, 18)), BlueprintGUILayout.Instance.LANGUAGE[53]))
                                {
                                    string[] scriptCode = File.ReadAllLines(GetGameDesignerPath + "/Editor/Resources/ActionBehaviourScript.script");
                                    scriptCode[7] = scriptCode[7].Replace("ActionBehaviourScript", act.createScriptName);
                                    ScriptTools.CreateScript(Application.dataPath + IState.StateActionScriptPath, act.createScriptName, scriptCode);
                                    s.compiling = true;
                                }
                                if (GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[54]))
                                {
                                    act.findBehaviours = false;
                                }
                            }
                            EditorGUILayout.Space();
                        }
                        EditorGUI.indentLevel = 2;
                    }
                }

                BlueprintGUILayout.EndStyleVertical();
            }

            EditorGUILayout.Space();
            DrawBehaviours(s);
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制状态行为
        /// </summary>
        public static void DrawBehaviours(State s)
        {
            GUILayout.Space(10);
            GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
            GUILayout.Space(5);

            for (int i = 0; i < s.behaviours.Count; ++i)
            {
                if (s.behaviours[i] == null)
                {
                    s.behaviours.RemoveAt(i);
                    continue;
                }
                EditorGUI.indentLevel = 1;
                EditorGUILayout.BeginHorizontal();
                Rect rect = EditorGUILayout.GetControlRect();
                s.behaviours[i].show = EditorGUI.Foldout(new Rect(rect.x, rect.y, 20, rect.height), s.behaviours[i].show, GUIContent.none);
                s.behaviours[i].Active = EditorGUI.ToggleLeft(new Rect(rect.x + 5, rect.y, 30, rect.height), GUIContent.none, s.behaviours[i].Active);
                EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 15, rect.height), s.behaviours[i].GetType().Name, GUI.skin.GetStyle("BoldLabel"));
                if (GUI.Button(new Rect(rect.x + rect.width - 15, rect.y, rect.width, rect.height), GUIContent.none, GUI.skin.GetStyle("ToggleMixed")))
                {
                    s.behaviours[s.behaviourMenuIndex].OnDestroyComponent();
                    DestroyImmediate(s.behaviours[i], true);
                    s.behaviours.RemoveAt(i);
                    continue;
                }
                if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[55]), false, (GenericMenu.MenuFunction)delegate ()
                    {
                        s.behaviours[s.behaviourMenuIndex].OnDestroyComponent();
                        DestroyImmediate(s.behaviours[s.behaviourMenuIndex], true);
                        s.behaviours.RemoveAt(s.behaviourMenuIndex);
                        return;
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[56]), false, (GenericMenu.MenuFunction)delegate ()
                    {
                        StateSystem.CopyComponent = s.behaviours[s.behaviourMenuIndex];
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[57]), StateSystem.CopyComponent ? true : false, (GenericMenu.MenuFunction)delegate ()
                    {
                        if (StateSystem.CopyComponent)
                        {
                            if (StateSystem.CopyComponent.GetType().BaseType == typeof(StateBehaviour))
                            {
                                StateBehaviour ab = (StateBehaviour)s.gameObject.AddComponent(StateSystem.CopyComponent.GetType());
                                ab.transform.SetParent(s.transform);
                                s.behaviours.Add(ab);
                                SystemType.SetFieldValue(ab, StateSystem.CopyComponent);
                            }
                        }
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[58]), StateSystem.CopyComponent ? true : false, delegate ()
                    {
                        if (StateSystem.CopyComponent)
                        {
                            if (StateSystem.CopyComponent.GetType().FullName == s.behaviours[s.behaviourMenuIndex].GetType().FullName)
                            {
                                SystemType.SetFieldValue(s.behaviours[s.behaviourMenuIndex], StateSystem.CopyComponent);
                            }
                        }
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[79]), false, delegate ()
                    {
                        string scriptName = s.behaviours[s.behaviourMenuIndex].GetType().Name;
                        string[] filePath = Directory.GetFiles(Application.dataPath, scriptName + ".cs", SearchOption.AllDirectories);
                        if (filePath.Length > 0)
                        {
                            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(filePath[0]);
                            System.Diagnostics.Process p = new System.Diagnostics.Process
                            {
                                StartInfo = psi
                            };
                            p.Start();
                            return;
                        }
                        Debug.Log("找不到脚本文件，类名必须是脚本文件名才能打开成功");
                    });
                    menu.ShowAsContext();
                    s.behaviourMenuIndex = i;
                }
                EditorGUILayout.EndHorizontal();
                if (s.behaviours[i].show)
                {
                    EditorGUILayout.ObjectField("Script", s.behaviours[i], typeof(StateBehaviour), true);
                    SerializedObject stateSerializedObject = new SerializedObject(s.behaviours[i]);
                    stateSerializedObject.Update();
                    if (!s.behaviours[i].OnInspectorGUI(s))
                    {
                        foreach (FieldInfo f in s.behaviours[i].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (s.behaviours[i].GetType() == f.DeclaringType & !f.IsStatic & f.FieldType.FullName != "System.Object")
                                try { EditorGUILayout.PropertyField(stateSerializedObject.FindProperty(f.Name), true); } catch { Debug.Log(f.Name + "  " + f.FieldType.FullName); }
                        }
                        /*var fields = s.behaviours[i].behaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                        Net.Loop.For(fields.Length, ii =>
                        {
                            var f = fields[ii];
                            if (s.behaviours[i].behaviour.GetType() == f.DeclaringType & !f.IsStatic & f.FieldType.FullName != "System.Object")
                            {
                                try
                                {
                                    if (f.FieldType.IsArray)
                                    {
                                        var itemType = SystemType.GetType(f.FieldType.FullName.Replace("[]", ""));
                                        var drawList = typeof(StateManagerEditor).GetMethod("DrawArray", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                                        MethodInfo curMethod = drawList.MakeGenericMethod(itemType);
                                        var obj = curMethod.Invoke(null, new object[] { "", f.Name, rect, f.GetValue(s.behaviours[i].behaviour) });
                                        f.SetValue(s.behaviours[i].behaviour, obj);
                                    }
                                    else if (f.FieldType.IsGenericType)
                                    {
                                        var itemType = f.FieldType.GetGenericArguments()[0];
                                        var drawList = typeof(StateManagerEditor).GetMethod("DrawList", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                                        MethodInfo curMethod = drawList.MakeGenericMethod(itemType);
                                        var obj = curMethod.Invoke(null, new object[] { "", f.Name, rect, f.GetValue(s.behaviours[i].behaviour) });
                                        f.SetValue(s.behaviours[i].behaviour, obj);
                                    }
                                    else if (f.FieldType.GetCustomAttribute<SerializableAttribute>() != null)
                                    {
                                        var itemType = f.FieldType;
                                        var drawList = typeof(StateManagerEditor).GetMethod("DrawEntity", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                                        MethodInfo curMethod = drawList.MakeGenericMethod(itemType);
                                        var obj = curMethod.Invoke(null, new object[] { "", f.Name, rect, f.GetValue(s.behaviours[i].behaviour) });
                                        f.SetValue(s.behaviours[i].behaviour, obj);
                                    }
                                    else if (f.FieldType.IsValueType | f.FieldType.IsEnum | f.FieldType == typeof(string) | f.FieldType.IsSubclassOf(typeof(Object)))
                                    {
                                        var obj = BlueprintGUILayout.PropertyField(f.Name, f.GetValue(s.behaviours[i].behaviour), f.FieldType);
                                        f.SetValue(s.behaviours[i].behaviour, obj);
                                    }
                                }
                                catch
                                {
                                    Debug.Log(f.Name + "  " + f.FieldType.FullName);
                                }
                            }
                        });*/
                    }
                    stateSerializedObject.ApplyModifiedProperties();
                    GUILayout.Space(4);
                    GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
                }
            }

            Rect r = EditorGUILayout.GetControlRect();
            Rect rr = new Rect(new Vector2(r.x + (r.size.x / 4f), r.y), new Vector2(r.size.x / 2f, 20));
            if (GUI.Button(rr, BlueprintGUILayout.Instance.LANGUAGE[59]))
            {
                s.findBehaviours = true;
            }

            if (s.findBehaviours)
            {
                try
                {
                    Type[] types = Assembly.Load("Assembly-CSharp").GetTypes().Where(t => t.IsSubclassOf(typeof(StateBehaviour))).ToArray();
                    EditorGUILayout.Space();
                    foreach (Type type in types)
                    {
                        if (GUILayout.Button(type.Name))
                        {
                            StateBehaviour stb = (StateBehaviour)s.gameObject.AddComponent(type);
                            s.behaviours.Add(stb);
                            s.findBehaviours = false;
                        }
                        if (s.compiling & type.Name == s.createScriptName)
                        {
                            StateBehaviour stb = (StateBehaviour)s.gameObject.AddComponent(type);
                            s.behaviours.Add(stb);
                            s.findBehaviours = false;
                            s.compiling = false;
                        }
                    }
                }
                catch { }
                EditorGUILayout.Space();
                EditorGUI.indentLevel = 0;
                EditorGUILayout.LabelField(BlueprintGUILayout.Instance.LANGUAGE[60]);
                IState.StateBehaviourScriptPath = EditorGUILayout.TextField(IState.StateBehaviourScriptPath);
                Rect addRect = EditorGUILayout.GetControlRect();
                s.createScriptName = EditorGUI.TextField(new Rect(addRect.position, new Vector2(addRect.size.x - 125f, 18)), s.createScriptName);
                if (GUI.Button(new Rect(new Vector2(addRect.size.x - 105f, addRect.position.y), new Vector2(120, 18)), BlueprintGUILayout.Instance.LANGUAGE[61]))
                {
                    string[] scriptCode = File.ReadAllLines(GetGameDesignerPath + "/Editor/Resources/StateBehaviourScript.script");
                    scriptCode[7] = scriptCode[7].Replace("StateBehaviourScript", s.createScriptName);
                    ScriptTools.CreateScript(Application.dataPath + IState.StateBehaviourScriptPath, s.createScriptName, scriptCode);
                    s.compiling = true;
                }
                if (GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[62]))
                {
                    s.findBehaviours = false;
                }
            }
        }

        public static T[] DrawArray<T>(string space, string name, Rect rect, T[] array)
        {
            float x = rect.x;
            rect = EditorGUILayout.GetControlRect();
            EditorGUI.Foldout(new Rect(x, rect.y, 20, rect.height), true, name);
            if (array == null)
                array = new T[0];
            var len = EditorGUILayout.IntField(space + "   Size", array.Length);
            if (len != array.Length)
            {
                List<T> list = new List<T>();
                Loop.For(len, i =>
                {
                    if (i < array.Length)
                        list.Add(array[i]);
                    else if (array.Length > 0)
                        list.Add(array[array.Length - 1]);
                    else
                    {
                        T t = default;
                        try
                        {
                            t = (T)Activator.CreateInstance(typeof(T));
                        }
                        catch (Exception) { }
                        list.Add(t);
                    }
                });
                array = list.ToArray();
            }
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (T)BlueprintGUILayout.PropertyField(space + "   " + i, array[i], typeof(T));
            }
            return array;
        }

        public static List<T> DrawList<T>(string space, string name, Rect rect, List<T> list)
        {
            float x = rect.x;
            rect = EditorGUILayout.GetControlRect();
            EditorGUI.Foldout(new Rect(x, rect.y, 20, rect.height), true, name);
            if (list == null)
                list = new List<T>();
            var len = EditorGUILayout.IntField(space + "   Size", list.Count);
            if (len != list.Count)
            {
                List<T> list1 = new List<T>();
                Loop.For(len, i =>
                {
                    if (i < list.Count)
                        list1.Add(list[i]);
                    else if (list.Count > 0)
                        list1.Add(list[list.Count - 1]);
                    else
                    {
                        T t = default;
                        try
                        {
                            t = (T)Activator.CreateInstance(typeof(T));
                        }
                        catch (Exception) { }
                        list1.Add(t);
                    }
                });
                list = list1;
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = (T)BlueprintGUILayout.PropertyField(space + "   " + i, list[i], typeof(T));
            }
            return list;
        }

        public static T DrawEntity<T>(string space, string name, Rect rect, T entity)
        {
            float x = rect.x;
            rect = EditorGUILayout.GetControlRect();
            rect.position = new Vector2(x, rect.y);
            EditorGUI.Foldout(rect, true, name);
            if (entity == null)
                return default;
            foreach (FieldInfo f in entity.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (entity.GetType() == f.DeclaringType & !f.IsStatic & f.FieldType.FullName != "System.Object")
                {
                    try
                    {
                        if (f.FieldType.IsArray)
                        {
                            var itemType = SystemType.GetType(f.FieldType.FullName.Replace("[]", ""));
                            var drawList = typeof(StateManagerEditor).GetMethod("DrawArray", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                            MethodInfo curMethod = drawList.MakeGenericMethod(itemType);
                            var obj = curMethod.Invoke(null, new object[] { space + "   ", f.Name, new Rect(rect.x + 20, rect.y + 20, rect.width, rect.height), f.GetValue(entity) });
                            f.SetValue(entity, obj);
                        }
                        else if (f.FieldType.IsGenericType)
                        {
                            var itemType = f.FieldType.GetGenericArguments()[0];
                            var drawList = typeof(StateManagerEditor).GetMethod("DrawList", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                            MethodInfo curMethod = drawList.MakeGenericMethod(itemType);
                            var obj = curMethod.Invoke(null, new object[] { space + "   ", f.Name, new Rect(rect.x + 20, rect.y + 20, rect.width, rect.height), f.GetValue(entity) });
                            f.SetValue(entity, obj);
                        }
                        else if (f.FieldType.GetCustomAttribute<SerializableAttribute>() != null)
                        {
                            var itemType = f.FieldType;
                            var drawList = typeof(StateManagerEditor).GetMethod("DrawEntity", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
                            MethodInfo curMethod = drawList.MakeGenericMethod(itemType);
                            var obj = curMethod.Invoke(null, new object[] { space + "   ", f.Name, new Rect(rect.x + 20, rect.y + 20, rect.width, rect.height), f.GetValue(entity) });
                            f.SetValue(entity, obj);
                        }
                        else if (f.FieldType.IsValueType | f.FieldType.IsEnum | f.FieldType == typeof(string) | f.FieldType.IsSubclassOf(typeof(Object)))
                        {
                            f.SetValue(entity, BlueprintGUILayout.PropertyField(space + "   " + f.Name, f.GetValue(entity), f.FieldType));
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(f.Name + "  " + ex);
                    }
                }
            }
            return entity;
        }

        /// <summary>
        /// 绘制状态连接行为
        /// </summary>
        public static void DrawTransition(Transition tr)
        {
            EditorGUI.indentLevel = 0;
            GUIStyle style = GUI.skin.GetStyle("dragtabdropwindow");
            style.fontStyle = FontStyle.Bold;
            style.font = Resources.Load<Font>("Arial");
            style.normal.textColor = Color.red;
            GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[63] + tr.currState.name + " -> " + tr.nextState.name, style);
            tr.name = tr.currState.name + " -> " + tr.nextState.name;
            EditorGUILayout.BeginVertical("ProgressBarBack");

            EditorGUILayout.Space();

            tr.model = (TransitionModel)EditorGUILayout.Popup(BlueprintGUILayout.Instance.LANGUAGE[64], (int)tr.model, Enum.GetNames(typeof(TransitionModel)), GUI.skin.GetStyle("PreDropDown"));
            switch (tr.model)
            {
                case TransitionModel.ExitTime:
                    tr.time = EditorGUILayout.FloatField(BlueprintGUILayout.Instance.LANGUAGE[65], tr.time, GUI.skin.GetStyle("PreDropDown"));
                    tr.exitTime = EditorGUILayout.FloatField(BlueprintGUILayout.Instance.LANGUAGE[66], tr.exitTime, GUI.skin.GetStyle("PreDropDown"));
                    EditorGUILayout.HelpBox(BlueprintGUILayout.Instance.LANGUAGE[67], MessageType.Info);
                    break;
            }

            GUILayout.Space(10);
            GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
            GUILayout.Space(10);

            tr.isEnterNextState = EditorGUILayout.Toggle(BlueprintGUILayout.Instance.LANGUAGE[68], tr.isEnterNextState);

            GUILayout.Space(10);
            GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));

            for (int i = 0; i < tr.behaviours.Count; ++i)
            {
                if (tr.behaviours[i] == null)
                {
                    tr.behaviours.RemoveAt(i);
                    continue;
                }
                EditorGUI.indentLevel = 1;
                EditorGUILayout.BeginHorizontal();
                Rect rect = EditorGUILayout.GetControlRect();
                EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 15, 20), tr.behaviours[i].GetType().Name, GUI.skin.GetStyle("BoldLabel"));
                tr.behaviours[i].show = EditorGUI.Foldout(new Rect(rect.x, rect.y, 20, 20), tr.behaviours[i].show, GUIContent.none, true);
                tr.behaviours[i].Active = EditorGUI.ToggleLeft(new Rect(rect.x + 5, rect.y, 30, 20), GUIContent.none, tr.behaviours[i].Active);
                if (GUI.Button(new Rect(rect.x + rect.width - 15, rect.y, rect.width, rect.height), GUIContent.none, GUI.skin.GetStyle("ToggleMixed")))
                {
                    tr.behaviours[tr.behaviourMenuIndex].OnDestroyComponent();
                    DestroyImmediate(tr.behaviours[i], true);
                    tr.behaviours.RemoveAt(i);
                    continue;
                }
                if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[69]), false, delegate ()
                    {
                        tr.behaviours[tr.behaviourMenuIndex].OnDestroyComponent();
                        DestroyImmediate(tr.behaviours[tr.behaviourMenuIndex], true);
                        tr.behaviours.RemoveAt(tr.behaviourMenuIndex);
                        return;
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[70]), false, delegate ()
                    {
                        StateSystem.CopyComponent = tr.behaviours[tr.behaviourMenuIndex];
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[71]), StateSystem.CopyComponent ? true : false, delegate ()
                    {
                        if (StateSystem.CopyComponent)
                        {
                            if (StateSystem.CopyComponent.GetType().BaseType == typeof(TransitionBehaviour))
                            {
                                TransitionBehaviour ab = (TransitionBehaviour)tr.gameObject.AddComponent(StateSystem.CopyComponent.GetType());
                                tr.behaviours.Add(ab);
                                SystemType.SetFieldValue(ab, StateSystem.CopyComponent);
                            }
                        }
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[72]), StateSystem.CopyComponent ? true : false, delegate ()
                    {
                        if (StateSystem.CopyComponent)
                        {
                            if (StateSystem.CopyComponent.GetType().FullName == tr.behaviours[tr.behaviourMenuIndex].GetType().FullName)
                            {
                                SystemType.SetFieldValue(tr.behaviours[tr.behaviourMenuIndex], StateSystem.CopyComponent);
                            }
                        }
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[80]), false, delegate ()
                    {
                        string scriptName = tr.behaviours[tr.behaviourMenuIndex].GetType().Name;
                        string[] filePath = Directory.GetFiles(Application.dataPath, scriptName + ".cs", SearchOption.AllDirectories);
                        if (filePath.Length > 0)
                        {
                            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(filePath[0]);
                            System.Diagnostics.Process p = new System.Diagnostics.Process
                            {
                                StartInfo = psi
                            };
                            p.Start();
                            return;
                        }
                        Debug.Log("找不到脚本文件，类名必须是脚本文件名才能打开成功");
                    });
                    menu.ShowAsContext();
                    tr.behaviourMenuIndex = i;
                }
                EditorGUILayout.EndHorizontal();
                if (tr.behaviours[i].show)
                {
                    EditorGUILayout.ObjectField("Script", tr.behaviours[i], typeof(TransitionBehaviour), true);
                    SerializedObject trSerialozedObject = new SerializedObject(tr.behaviours[i]);
                    trSerialozedObject.Update();
                    if (!tr.behaviours[i].OnInspectorGUI(tr.currState))
                    {
                        foreach (FieldInfo f in tr.behaviours[i].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (tr.behaviours[i].GetType() == f.DeclaringType & !f.IsStatic & f.FieldType.FullName != "System.Object")
                                try { EditorGUILayout.PropertyField(trSerialozedObject.FindProperty(f.Name), true); } catch { Debug.Log(f.Name + "  " + f.FieldType.FullName); }
                        }
                    }
                    trSerialozedObject.ApplyModifiedProperties();
                    GUILayout.Space(10);
                    GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));

                }
            }

            GUILayout.Space(5);

            Rect r = EditorGUILayout.GetControlRect();
            Rect rr = new Rect(new Vector2(r.x + (r.size.x / 4f), r.y), new Vector2(r.size.x / 2f, 20));
            if (GUI.Button(rr, BlueprintGUILayout.Instance.LANGUAGE[73]))
            {
                tr.findBehaviours = true;
            }

            if (tr.findBehaviours)
            {
                Type[] types = Assembly.Load("Assembly-CSharp").GetTypes().Where(t => t.IsSubclassOf(typeof(TransitionBehaviour))).ToArray();
                EditorGUILayout.Space();
                foreach (Type type in types)
                {
                    if (GUILayout.Button(type.Name))
                    {
                        TransitionBehaviour stb = (TransitionBehaviour)tr.gameObject.AddComponent(type);
                        tr.behaviours.Add(stb);
                        tr.findBehaviours = false;
                    }
                    if (tr.compiling & type.Name == tr.createScriptName)
                    {
                        TransitionBehaviour stb = (TransitionBehaviour)tr.gameObject.AddComponent(type);
                        tr.behaviours.Add(stb);
                        tr.findBehaviours = false;
                        tr.compiling = false;
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(BlueprintGUILayout.Instance.LANGUAGE[74]);
                IState.TransitionScriptPath = EditorGUILayout.TextField(IState.TransitionScriptPath);
                Rect addRect = EditorGUILayout.GetControlRect();
                tr.createScriptName = EditorGUI.TextField(new Rect(addRect.position, new Vector2(addRect.size.x - 125f, 18)), tr.createScriptName);
                if (GUI.Button(new Rect(new Vector2(addRect.size.x - 105f, addRect.position.y), new Vector2(120, 18)), BlueprintGUILayout.Instance.LANGUAGE[75]))
                {
                    string[] scriptCode = File.ReadAllLines(GetGameDesignerPath + "/Editor/Resources/TransitionBehaviorScript.script");
                    scriptCode[7] = scriptCode[7].Replace("TransitionBehaviorScript", tr.createScriptName);
                    ScriptTools.CreateScript(Application.dataPath + IState.TransitionScriptPath, tr.createScriptName, scriptCode);
                    tr.compiling = true;
                }
                if (GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[76]))
                {
                    tr.findBehaviours = false;
                }
            }
            GUILayout.Space(10);
            EditorGUILayout.HelpBox(BlueprintGUILayout.Instance.LANGUAGE[77], MessageType.Info);
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
        }

        public static string ToArrtsString(Parameter[] ps)
        {
            string str = "(";
            foreach (Parameter p in ps)
            {
                str += "  " + p.parameterTypeName + "  " + p.name + "  ";
            }
            return str + ")";
        }
    }

}
#endif