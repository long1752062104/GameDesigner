#if UNITY_EDITOR
using System;
using System.Collections;
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
    [CanEditMultipleObjects]
    public class StateManagerEditor : Editor
    {
        private static StateManager stateManager = null;

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
            var stateMachine = stateManager.stateMachine;
            if (stateMachine != null) 
            {
                if (stateMachine.animation == null)
                    stateMachine.animation = stateManager.GetComponentInChildren<Animation>();
                if (stateMachine.animation != null)
                {
                    if (stateMachine.clipNames.Count != AnimationUtility.GetAnimationClips(stateMachine.animation.gameObject).Length)
                    {
                        stateMachine.clipNames = new List<string>();
                        foreach (AnimationClip clip in AnimationUtility.GetAnimationClips(stateMachine.animation.gameObject))
                        {
                            stateMachine.clipNames.Add(clip.name);
                        }
                    }
                }
                stateMachine.animator = stateManager.GetComponent<Animator>();
                if (stateMachine.animator == null)
                    stateMachine.animator = stateManager.GetComponentInChildren<Animator>();
                if (stateMachine.animator != null)
                {
                    if (stateMachine.clipNames.Count != stateMachine.animator.runtimeAnimatorController.animationClips.Length)
                    {
                        stateMachine.clipNames = new List<string>();
                        foreach (AnimationClip clip in stateMachine.animator.runtimeAnimatorController.animationClips)
                        {
                            stateMachine.clipNames.Add(clip.name);
                        }
                    }
                }
            }
            StateMachineWindow.stateMachine = stateManager.stateMachine;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stateMachine"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[0]));
            if (GUILayout.Button(BlueprintSetting.Instance.LANGUAGE[1], GUI.skin.GetStyle("LargeButtonMid"), GUILayout.ExpandWidth(true)))
                StateMachineWindow.Init(stateManager.stateMachine);
            if (stateManager.stateMachine == null)
                goto J;
            if (stateManager.stateMachine.selectState != null)
            {
                DrawState(stateManager.stateMachine.selectState, stateManager);
                EditorGUILayout.Space();
                for (int i = 0; i < stateManager.stateMachine.selectState.transitions.Count; ++i)
                    DrawTransition(stateManager.stateMachine.selectState.transitions[i]);
            }
            else if (StateMachineWindow.selectTransition != null)
            {
                DrawTransition(StateMachineWindow.selectTransition);
            }
            EditorGUILayout.Space();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(stateManager.stateMachine);
            }
            Repaint();
            J: serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制状态监视面板属性
        /// </summary>
        public static void DrawState(State s, StateManager sm)
        {
            SerializedObject serializedObject = new SerializedObject(sm.stateMachine);
            var serializedProperty = serializedObject.FindProperty("states").GetArrayElementAtIndex(s.ID);
            serializedObject.Update();
            GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[2], GUI.skin.GetStyle("dragtabdropwindow"));
            EditorGUILayout.BeginVertical("ProgressBarBack");
            EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("name"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[3], "name"));
            EditorGUILayout.IntField(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[4], "stateID"), s.ID);
            EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("actionSystem"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[5], "actionSystem  专为玩家角色AI其怪物AI所设计的一套AI系统！"));
            if (s.actionSystem)
            {
                sm.stateMachine.animMode = (AnimationMode)EditorGUILayout.EnumPopup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[6], "animMode"), sm.stateMachine.animMode);
                if (sm.stateMachine.animMode == AnimationMode.Animation)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("animation"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[7], "animation"));
                else
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("animator"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[8], "animator"));
                s.animPlayMode = (AnimPlayMode)EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[9], "animPlayMode"), (int)s.animPlayMode, new GUIContent[]{
                    new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[10],"Random"),
                    new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[11],"Sequence") }
                );
                EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("animSpeed"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[12], "animSpeed"), true);
                EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("animLoop"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[13], "animLoop"), true);
                s.isExitState = EditorGUILayout.Toggle(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[14], "isExitState"), s.isExitState);
                if (s.isExitState)
                    s.DstStateID = EditorGUILayout.Popup(BlueprintGUILayout.Instance.LANGUAGE[15], s.DstStateID, Array.ConvertAll(s.transitions.ToArray(), new Converter<Transition, string>(delegate (Transition t) { return t.currState.name + " -> " + t.nextState.name + "   ID:" + t.nextState.ID; })));
                BlueprintGUILayout.BeginStyleVertical(BlueprintGUILayout.Instance.LANGUAGE[16], "ProgressBarBack");
                EditorGUI.indentLevel = 1;
                Rect actRect = EditorGUILayout.GetControlRect();
                s.foldout = EditorGUI.Foldout(new Rect(actRect.position, new Vector2(actRect.size.x - 120f, 15)), s.foldout, BlueprintGUILayout.Instance.LANGUAGE[17], true);

                if (GUI.Button(new Rect(new Vector2(actRect.size.x - 40f, actRect.position.y), new Vector2(60, 16)), BlueprintGUILayout.Instance.LANGUAGE[18]))
                {
                    s.actions.Add(new StateAction() { stateMachine = s.stateMachine });
                }
                if (GUI.Button(new Rect(new Vector2(actRect.size.x - 100, actRect.position.y), new Vector2(60, 16)), BlueprintGUILayout.Instance.LANGUAGE[19]))
                {
                    if (s.actions.Count > 1)
                    {
                        s.actions.RemoveAt(s.actions.Count - 1);
                    }
                }

                if (s.foldout)
                {
                    var actionsProperty = serializedProperty.FindPropertyRelative("actions");
                    EditorGUI.indentLevel = 2;
                    for (int a = 0; a < s.actions.Count; ++a)
                    {
                        var actionProperty = actionsProperty.GetArrayElementAtIndex(a);
                        StateAction act = s.actions[a];
                        Rect foldoutRect = EditorGUILayout.GetControlRect();
                        act.foldout = EditorGUI.Foldout(foldoutRect, act.foldout, new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[20] + a, "actions[" + a + "]"), true);
                        if (foldoutRect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                        {
                            s.actionMenuIndex = a;
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[21]), false, delegate ()
                            {
                                s.actions.RemoveAt(s.actionMenuIndex);
                                return;
                            });
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[22]), false, delegate ()
                            {
                                StateSystem.Component = s.actions[s.actionMenuIndex];
                            });
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[23]), StateSystem.CopyComponent!=null ? true : false, ()=>
                            {
                                if (StateSystem.Component is StateAction stateAction)
                                    s.actions.Add(Net.Clone.DeepCopy<StateAction>(stateAction));
                            });
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[24]), StateSystem.CopyComponent!=null ? true : false, () =>
                            {
                                if (StateSystem.Component is StateAction stateAction)
                                {
                                    if (stateAction == s.actions[s.actionMenuIndex])//如果要黏贴的动作是复制的动作则返回
                                        return;
                                    s.actions[s.actionMenuIndex] = Net.Clone.DeepCopy<StateAction>(stateAction);
                                }
                            });
                            menu.ShowAsContext();
                        }
                        if (act.foldout)
                        {
                            EditorGUI.indentLevel = 3;
                            try
                            {
                                act.clipIndex = EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[25], "clipIndex"), act.clipIndex, Array.ConvertAll(s.stateMachine.clipNames.ToArray(), new Converter<string, GUIContent>(delegate (string input)
                                { return new GUIContent(input); })));
                                act.clipName = s.stateMachine.clipNames[act.clipIndex];
                            } catch { }
                            s.actions[a].isPlayAudio = EditorGUILayout.Toggle(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[26], "isPlayAudio"), s.actions[a].isPlayAudio);
                            if (s.actions[a].isPlayAudio)
                            {
                                act.audioModel = (AudioMode)EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[27], "audioModel"), (int)act.audioModel,
                                new GUIContent[]{ new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[28],"EnterPlayAudio") ,
                                    new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[29],"AnimEventPlayAudio") ,
                                    new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[30],"ExitPlayAudio") }
                                );
                                EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("audioClips"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[31], "audioClips"), true);
                            }
                            EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("animTime"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[32], "animTime"));
                            EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("animTimeMax"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[33], "animTimeMax"));
                            EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("animEventTime"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[34], "animEventTime"));
                            EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("effectSpwan"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[35], "effectSpwan"));
                            act.activeMode = (ActiveMode)EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[36], "activeModel"), (int)act.activeMode, new GUIContent[]{
                                new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[37],"Instantiate") ,
                                new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[38],"SetActive") }
                            );
                            if (act.activeMode == ActiveMode.ObjectPool)
                                EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("activeObjs"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[39], "activeObjs"), true);
                            act.spwanmode = (SpwanMode)EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[40], "spwanmode"), (int)act.spwanmode, new GUIContent[]{
                                new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[41],"TransformPoint") ,
                                new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[42],"SetParent") ,
                                new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[43],"localPosition") }
                            );
                            if (act.spwanmode != SpwanMode.localPosition)
                                act.parent = EditorGUILayout.ObjectField(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[44], "parent"), act.parent, typeof(Transform), true) as Transform;
                            EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("effectPostion"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[45], "effectPostion"));
                            EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("effectEulerAngles"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[113], "effectEulerAngles"));
                            EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("spwanTime"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[46], "spwanTime"));
                            for (int i = 0; i < act.behaviours.Count; ++i)
                            {
                                EditorGUILayout.BeginHorizontal();
                                Rect rect = EditorGUILayout.GetControlRect();
                                act.behaviours[i].show = EditorGUI.Foldout(new Rect(rect.x, rect.y, 50, rect.height), act.behaviours[i].show, GUIContent.none);
                                act.behaviours[i].Active = EditorGUI.ToggleLeft(new Rect(rect.x + 5, rect.y, 70, rect.height), GUIContent.none, act.behaviours[i].Active);
                                EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 15, rect.height), act.behaviours[i].GetType().Name, GUI.skin.GetStyle("BoldLabel"));
                                if (GUI.Button(new Rect(rect.x + rect.width - 15, rect.y, rect.width, rect.height), GUIContent.none, GUI.skin.GetStyle("ToggleMixed")))
                                {
                                    act.behaviours[act.behaviourMenuIndex].OnDestroyComponent();
                                    act.behaviours.RemoveAt(i);
                                    continue;
                                }
                                if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                                {
                                    GenericMenu menu = new GenericMenu();
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[47]), false, delegate ()
                                    {
                                        act.behaviours[act.behaviourMenuIndex].OnDestroyComponent();
                                        act.behaviours.RemoveAt(act.behaviourMenuIndex);
                                        return;
                                    });
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[48]), false, delegate ()
                                    {
                                        StateSystem.CopyComponent = act.behaviours[act.behaviourMenuIndex];
                                    });
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[49]), StateSystem.CopyComponent != null ? true : false, delegate ()
                                    {
                                        if (StateSystem.CopyComponent is ActionBehaviour behaviour)
                                        {
                                            ActionBehaviour ab = (ActionBehaviour)Net.Clone.DeepCopy(behaviour);
                                            act.behaviours.Add(ab);
                                        }
                                    });
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[50]), StateSystem.CopyComponent!=null ? true : false, delegate ()
                                    {
                                        if (StateSystem.CopyComponent is ActionBehaviour behaviour)
                                        {
                                            if (behaviour.name == act.behaviours[act.behaviourMenuIndex].name)
                                                act.behaviours[act.behaviourMenuIndex] = (ActionBehaviour)Net.Clone.DeepCopy(StateSystem.CopyComponent);
                                        }
                                    });
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[78]), false, delegate ()
                                    {
                                        string scriptName = act.behaviours[act.behaviourMenuIndex].name;
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
                                    if (!act.behaviours[i].OnInspectorGUI(s))
                                        foreach (var metadata in act.behaviours[i].metadatas)
                                            PropertyField(metadata);
                                    GUILayout.Space(4);
                                    GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
                                    GUILayout.Space(4);
                                    EditorGUI.indentLevel = 3;
                                }
                            }
                            Rect r = EditorGUILayout.GetControlRect();
                            Rect rr = new Rect(new Vector2(r.x + (r.size.x / 4f), r.y), new Vector2(r.size.x / 2f, 20));
                            if (GUI.Button(rr, BlueprintGUILayout.Instance.LANGUAGE[51]))
                                act.findBehaviours = true;
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
                                            ActionBehaviour stb = (ActionBehaviour)Activator.CreateInstance(type);
                                            stb.InitMetadatas(act.stateMachine);
                                            stb.ID = s.ID;
                                            act.behaviours.Add(stb);
                                            act.findBehaviours = false;
                                        }
                                        if (s.compiling & type.Name == act.createScriptName)
                                        {
                                            ActionBehaviour stb = (ActionBehaviour)Activator.CreateInstance(type);
                                            stb.InitMetadatas(sm.stateMachine);
                                            stb.ID = s.ID;
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
                                    act.findBehaviours = false;
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
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
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
                EditorGUI.indentLevel = 1;
                EditorGUILayout.BeginHorizontal();
                Rect rect = EditorGUILayout.GetControlRect();
                s.behaviours[i].show = EditorGUI.Foldout(new Rect(rect.x, rect.y, 20, rect.height), s.behaviours[i].show, GUIContent.none);
                s.behaviours[i].Active = EditorGUI.ToggleLeft(new Rect(rect.x + 5, rect.y, 30, rect.height), GUIContent.none, s.behaviours[i].Active);
                EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 15, rect.height), s.behaviours[i].name, GUI.skin.GetStyle("BoldLabel"));
                if (GUI.Button(new Rect(rect.x + rect.width - 15, rect.y, rect.width, rect.height), GUIContent.none, GUI.skin.GetStyle("ToggleMixed")))
                {
                    s.behaviours[s.behaviourMenuIndex].OnDestroyComponent();
                    s.behaviours.RemoveAt(i);
                    continue;
                }
                if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[55]), false, delegate ()
                    {
                        s.behaviours[s.behaviourMenuIndex].OnDestroyComponent();
                        s.behaviours.RemoveAt(s.behaviourMenuIndex);
                        return;
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[56]), false, delegate ()
                    {
                        StateSystem.CopyComponent = s.behaviours[s.behaviourMenuIndex];
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[57]), StateSystem.CopyComponent!=null ? true : false, delegate ()
                    {
                        if (StateSystem.CopyComponent is StateBehaviour behaviour)
                        {
                            StateBehaviour ab = (StateBehaviour)Net.Clone.DeepCopy(behaviour);
                            s.behaviours.Add(ab);
                        }
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[58]), StateSystem.CopyComponent!=null ? true : false, delegate ()
                    {
                        if (StateSystem.CopyComponent is StateBehaviour behaviour)
                        {
                            if (behaviour.name == s.behaviours[s.behaviourMenuIndex].name)
                                s.behaviours[s.behaviourMenuIndex] = (StateBehaviour)Net.Clone.DeepCopy(behaviour);
                        }
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[79]), false, delegate ()
                    {
                        string scriptName = s.behaviours[s.behaviourMenuIndex].name;
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
                    EditorGUI.indentLevel = 2;
                    if (!s.behaviours[i].OnInspectorGUI(s))
                    {
                        foreach (var metadata in s.behaviours[i].metadatas)
                        {
                            PropertyField(metadata);
                        }
                    }
                    EditorGUI.indentLevel = 1;
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
                            StateBehaviour stb = (StateBehaviour)Activator.CreateInstance(type);
                            stb.InitMetadatas(s.stateMachine);
                            stb.ID = s.ID;
                            s.behaviours.Add(stb);
                            s.findBehaviours = false;
                        }
                        if (s.compiling & type.Name == s.createScriptName)
                        {
                            StateBehaviour stb = (StateBehaviour)Activator.CreateInstance(type);
                            stb.InitMetadatas(s.stateMachine);
                            stb.ID = s.ID;
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

        private static void PropertyField(Metadata metadata)
        {
            if (metadata.type == TypeCode.Byte)
                metadata.value = (byte)EditorGUILayout.IntField(metadata.name, (byte)metadata.value);
            else if (metadata.type == TypeCode.SByte)
                metadata.value = (sbyte)EditorGUILayout.IntField(metadata.name, (sbyte)metadata.value);
            else if (metadata.type == TypeCode.Boolean)
                metadata.value = EditorGUILayout.Toggle(metadata.name, (bool)metadata.value);
            else if (metadata.type == TypeCode.Int16)
                metadata.value = (short)EditorGUILayout.IntField(metadata.name, (short)metadata.value);
            else if (metadata.type == TypeCode.UInt16)
                metadata.value = (ushort)EditorGUILayout.IntField(metadata.name, (ushort)metadata.value);
            else if (metadata.type == TypeCode.Char)
                metadata.value = EditorGUILayout.TextField(metadata.name, metadata.value.ToString()).ToCharArray();
            else if (metadata.type == TypeCode.Int32)
                metadata.value = EditorGUILayout.IntField(metadata.name, (int)metadata.value);
            else if (metadata.type == TypeCode.UInt32)
                metadata.value = (uint)EditorGUILayout.IntField(metadata.name, (int)metadata.value);
            else if (metadata.type == TypeCode.Single)
                metadata.value = EditorGUILayout.FloatField(metadata.name, (float)metadata.value);
            else if (metadata.type == TypeCode.Int64)
                metadata.value = EditorGUILayout.LongField(metadata.name, (long)metadata.value);
            else if (metadata.type == TypeCode.UInt64)
                metadata.value = (ulong)EditorGUILayout.LongField(metadata.name, (long)metadata.value);
            else if (metadata.type == TypeCode.Double)
                metadata.value = EditorGUILayout.DoubleField(metadata.name, (double)metadata.value);
            else if (metadata.type == TypeCode.String)
                metadata.value = EditorGUILayout.TextField(metadata.name, metadata.value.ToString());
            else if (metadata.type == TypeCode.Vector2)
                metadata.value = EditorGUILayout.Vector2Field(metadata.name, (Vector2)metadata.value);
            else if (metadata.type == TypeCode.Vector3)
                metadata.value = EditorGUILayout.Vector3Field(metadata.name, (Vector3)metadata.value);
            else if (metadata.type == TypeCode.Vector4)
                metadata.value = EditorGUILayout.Vector4Field(metadata.name, (Vector4)metadata.value);
            else if (metadata.type == TypeCode.Quaternion)
            {
                Quaternion q = (Quaternion)metadata.value;
                var value = EditorGUILayout.Vector4Field(metadata.name, new Vector4(q.x, q.y, q.z, q.w));
                Quaternion q1 = new Quaternion(value.x, value.y, value.z, value.w);
                metadata.value = q1;
            }
            else if (metadata.type == TypeCode.Rect)
                metadata.value = EditorGUILayout.RectField(metadata.name, (Rect)metadata.value);
            else if (metadata.type == TypeCode.Color)
                metadata.value = EditorGUILayout.ColorField(metadata.name, (Color)metadata.value);
            else if (metadata.type == TypeCode.Color32)
                metadata.value = (Color32)EditorGUILayout.ColorField(metadata.name, (Color32)metadata.value);
            else if (metadata.type == TypeCode.AnimationCurve)
                metadata.value = EditorGUILayout.CurveField(metadata.name, (AnimationCurve)metadata.value);
            else if (metadata.type == TypeCode.Object)
                metadata.value = EditorGUILayout.ObjectField(metadata.name, (Object)metadata.value, metadata.Type, true);
            else if (metadata.type == TypeCode.GenericType | metadata.type == TypeCode.Array) 
            {
                var rect = EditorGUILayout.GetControlRect();
                rect.x += 40f;
                metadata.foldout = EditorGUI.BeginFoldoutHeaderGroup(rect, metadata.foldout, metadata.name);
                if (metadata.foldout) 
                {
                    EditorGUI.indentLevel = 3;
                    EditorGUI.BeginChangeCheck();
                    var arraySize = EditorGUILayout.DelayedIntField("Size", metadata.arraySize);
                    bool flag8 = EditorGUI.EndChangeCheck();
                    IList list = (IList)metadata.value;
                    if (flag8 | list.Count != metadata.arraySize)
                    {
                        metadata.arraySize = arraySize;
                        IList list1 = Array.CreateInstance(metadata.itemType, arraySize);
                        for (int i = 0; i < list1.Count; i++)
                            if (i < list.Count)
                                list1[i] = list[i];
                        if (metadata.type == TypeCode.GenericType)
                        {
                            IList list2 = (IList)Activator.CreateInstance(metadata.Type);
                            for (int i = 0; i < list1.Count; i++)
                                list2.Add(list1[i]);
                            list = list2; 
                        }
                        else list = list1;
                    }
                    for (int i = 0; i < list.Count; i++)
                        list[i] = PropertyField("Element " + i, list[i], metadata.itemType);
                    metadata.value = list;
                    EditorGUI.indentLevel = 2;
                }
                EditorGUI.EndFoldoutHeaderGroup();
            }
        }

        private static object PropertyField(string name, object obj, Type type)
        {
            var typeCode = (TypeCode)Type.GetTypeCode(type);
            if (typeCode == TypeCode.Byte)
                obj = (byte)EditorGUILayout.IntField(name, (byte)obj);
            else if (typeCode == TypeCode.SByte)
                obj = (sbyte)EditorGUILayout.IntField(name, (sbyte)obj);
            else if (typeCode == TypeCode.Boolean)
                obj = EditorGUILayout.Toggle(name, (bool)obj);
            else if (typeCode == TypeCode.Int16)
                obj = (short)EditorGUILayout.IntField(name, (short)obj);
            else if (typeCode == TypeCode.UInt16)
                obj = (ushort)EditorGUILayout.IntField(name, (ushort)obj);
            else if (typeCode == TypeCode.Char)
                obj = EditorGUILayout.TextField(name, (string)obj).ToCharArray();
            else if (typeCode == TypeCode.Int32)
                obj = EditorGUILayout.IntField(name, (int)obj);
            else if (typeCode == TypeCode.UInt32)
                obj = (uint)EditorGUILayout.IntField(name, (int)obj);
            else if (typeCode == TypeCode.Single)
                obj = EditorGUILayout.FloatField(name, (float)obj);
            else if (typeCode == TypeCode.Int64)
                obj = EditorGUILayout.LongField(name, (long)obj);
            else if (typeCode == TypeCode.UInt64)
                obj = (ulong)EditorGUILayout.LongField(name, (long)obj);
            else if (typeCode == TypeCode.Double)
                obj = EditorGUILayout.DoubleField(name, (double)obj);
            else if (typeCode == TypeCode.String)
                obj = EditorGUILayout.TextField(name, (string)obj);
            else if (type == typeof(Vector2))
                obj = EditorGUILayout.Vector2Field(name, (Vector2)obj);
            else if (type == typeof(Vector3))
                obj = EditorGUILayout.Vector3Field(name, (Vector3)obj);
            else if (type == typeof(Vector4))
                obj = EditorGUILayout.Vector4Field(name, (Vector4)obj);
            else if (type == typeof(Quaternion))
            {
                var value = EditorGUILayout.Vector4Field(name, (Vector4)obj);
                Quaternion quaternion = new Quaternion(value.x, value.y, value.z, value.w);
                obj = quaternion;
            }
            else if (type == typeof(Rect))
                obj = EditorGUILayout.RectField(name, (Rect)obj);
            else if (type == typeof(Color))
                obj = EditorGUILayout.ColorField(name, (Color)obj);
            else if (type == typeof(Color32))
                obj = EditorGUILayout.ColorField(name, (Color32)obj);
            else if (type == typeof(AnimationCurve))
                obj = EditorGUILayout.CurveField(name, (AnimationCurve)obj);
            else if (type.IsSubclassOf(typeof(Object)) | type == typeof(Object))
                obj = EditorGUILayout.ObjectField(name, (Object)obj, type, true);
            return obj;
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
                    tr.behaviours.RemoveAt(i);
                    continue;
                }
                if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[69]), false, delegate ()
                    {
                        tr.behaviours[tr.behaviourMenuIndex].OnDestroyComponent();
                        tr.behaviours.RemoveAt(tr.behaviourMenuIndex);
                        return;
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[70]), false, delegate ()
                    {
                        StateSystem.CopyComponent = tr.behaviours[tr.behaviourMenuIndex];
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[71]), StateSystem.CopyComponent!=null ? true : false, () =>
                    {
                        if (StateSystem.CopyComponent is TransitionBehaviour behaviour)
                        {
                            TransitionBehaviour ab = (TransitionBehaviour)Net.Clone.DeepCopy(behaviour);
                            tr.behaviours.Add(ab);
                        }
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[72]), StateSystem.CopyComponent!=null ? true : false, ()=>
                    {
                        if (StateSystem.CopyComponent is TransitionBehaviour behaviour)
                            if (behaviour.name == tr.behaviours[tr.behaviourMenuIndex].name)
                                tr.behaviours[tr.behaviourMenuIndex] = (TransitionBehaviour)Net.Clone.DeepCopy(behaviour);
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[80]), false, () =>
                    {
                        string scriptName = tr.behaviours[tr.behaviourMenuIndex].name;
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
                    EditorGUI.indentLevel = 2;
                    if (!tr.behaviours[i].OnInspectorGUI(tr.currState))
                    {
                        foreach (var metadata in tr.behaviours[i].metadatas)
                        {
                            PropertyField(metadata);
                        }
                    }
                    EditorGUI.indentLevel = 1;
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
                        TransitionBehaviour stb = (TransitionBehaviour)Activator.CreateInstance(type);
                        stb.InitMetadatas(tr.stateMachine);
                        tr.behaviours.Add(stb);
                        tr.findBehaviours = false;
                    }
                    if (tr.compiling & type.Name == tr.createScriptName)
                    {
                        TransitionBehaviour stb = (TransitionBehaviour)Activator.CreateInstance(type);
                        stb.InitMetadatas(tr.stateMachine);
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

        [UnityEditor.Callbacks.DidReloadScripts(0)]
        internal static void OnScriptReload()
        {
            if (stateManager == null)
                return;
            foreach (var s in stateManager.stateMachine.states) 
            {
                for (int i = 0; i < s.behaviours.Count; i++)
                {
                    var type = SystemType.GetType(s.behaviours[i].name);
                    var metadatas = new List<Metadata>(s.behaviours[i].metadatas);
                    s.behaviours[i] = (StateBehaviour)Activator.CreateInstance(type);
                    s.behaviours[i].Reload(type, stateManager.stateMachine, metadatas);
                }
                foreach (var t in s.transitions)
                {
                    for (int i = 0; i < t.behaviours.Count; i++)
                    {
                        var type = SystemType.GetType(t.behaviours[i].name);
                        var metadatas = new List<Metadata>(t.behaviours[i].metadatas);
                        t.behaviours[i] = (TransitionBehaviour)Activator.CreateInstance(type);
                        t.behaviours[i].Reload(type, stateManager.stateMachine, metadatas);
                    }
                }
                foreach (var a in s.actions)
                {
                    for (int i = 0; i < a.behaviours.Count; i++)
                    {
                        var type = SystemType.GetType(a.behaviours[i].name);
                        var metadatas = new List<Metadata>(a.behaviours[i].metadatas);
                        a.behaviours[i] = (ActionBehaviour)Activator.CreateInstance(type);
                        a.behaviours[i].Reload(type, stateManager.stateMachine, metadatas);
                    }
                }
            }
        }
    }

}
#endif