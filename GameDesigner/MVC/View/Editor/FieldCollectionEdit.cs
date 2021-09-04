#if UNITY_EDITOR
namespace MVC.View
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class FieldCollectionWindow : EditorWindow
    {
        private FieldCollectionEdit field;
        private bool setValue;
        private bool setName;

        internal static void Init(FieldCollectionEdit field)
        {
            var win = GetWindow<FieldCollectionWindow>("字段收集器", true);
            win.field = field;
        }

        void OnGUI() 
        {
            GUILayout.Label("将组件拖到此窗口上! 如果是赋值模式, 拖入的对象将不会显示选择组件!");
            setValue = GUILayout.Toggle(setValue, "赋值模式/创建模式");
            setName = GUILayout.Toggle(setName, "设置字段名和赋值对象");
            if ((Event.current.type == EventType.DragUpdated | Event.current.type == EventType.DragPerform) & !setValue)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;//拖动时显示辅助图标
                if (Event.current.type == EventType.DragPerform)
                {
                    if (setName)
                    {
                        foreach (var obj in DragAndDrop.objectReferences) 
                        {
                            field.fieldName = obj.name;
                            field.selectObject = obj;
                            field.AddField(obj.GetType().FullName);
                        }
                        return;
                    }
                    else 
                    {
                        field.search1 = "";
                        field.search = DragAndDrop.objectReferences[0].GetType().Name.ToLower();
                    }
                }
            }
            try { field.OnInspectorGUI(); } catch { }
        }
    }

    [CustomEditor(typeof(FieldCollection))]
    public class FieldCollectionEdit : Editor
    {
        FieldCollection field;
        bool selectType;
        internal string search = "", search1 = "", fieldName = "";
        string[] types = new string[0];
        DateTime searchTime;
        int deleteArrayIndex = -1;
        private string nameSpace; 
        private string nameSpace1;
        bool doubleClick;
        int index;
        private string savePath;
        private string selectTypeName;
        internal UnityEngine.Object selectObject;
        private string csprojFile;
        private bool fullpath;

        public class JsonSave 
        {
            public string nameSpace;
            public string savePath;
            public string csprojFile;
            public bool fullPath;
        }

        private void OnEnable()
        {
            field = target as FieldCollection;
            var objects = Resources.FindObjectsOfTypeAll<UnityEngine.Object>();
            HashSet<string> types1 = new HashSet<string>();
            foreach (var obj in objects)
            {
                var str = obj.GetType().FullName;
                if (!types1.Contains(str))
                    types1.Add(str);
            }
            var types2 = typeof(Vector2).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(UnityEngine.Object))).ToArray();
            foreach (var obj in types2)
            {
                var str = obj.FullName;
                if (!types1.Contains(str))
                    types1.Add(str);
            }
            types = types1.ToArray();
            LoadData();
            if (string.IsNullOrEmpty(savePath))
                savePath = Application.dataPath;
        }

        void LoadData() 
        {
            if (File.Exists(Application.dataPath.Replace("Assets", "") + "fcdata.txt"))
            {
                var fcdata = File.ReadAllText(Application.dataPath.Replace("Assets", "") + "fcdata.txt");
                var jsonsave = Newtonsoft_X.Json.JsonConvert.DeserializeObject<JsonSave>(fcdata);
                nameSpace = jsonsave.nameSpace;
                savePath = jsonsave.savePath;
                csprojFile = jsonsave.csprojFile;
                fullpath = jsonsave.fullPath;
            }
        }

        void SaveData() 
        {
            var path = Application.dataPath.Replace("Assets", "") + "fcdata.txt";
            var jsonSave = new JsonSave() { 
                csprojFile = csprojFile,
                nameSpace = nameSpace,
                savePath = savePath,
                fullPath = fullpath
            };
            var jsonstr = Newtonsoft_X.Json.JsonConvert.SerializeObject(jsonSave);
            File.WriteAllText(path, jsonstr);
        }

        internal void AddField(string typeName) 
        {
            selectType = true;
            var name = fieldName;
            if (name == "")
                name = "name" + field.nameIndex++;
            foreach (var f in field.fields)
            {
                if (f.name == fieldName)
                {
                    name += field.nameIndex++;
                    break;
                }
            }
            var field1 = new FieldCollection.Field() { name = name, typeName = typeName };
            field.fields.Add(field1);
            if (selectObject != null)
                field1.target = selectObject;
            selectTypeName = typeName;
            EditorUtility.SetDirty(field);
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("打开收集器界面")) 
                FieldCollectionWindow.Init(this);
            var so = serializedObject;
            so.Update();
            field.fieldName = EditorGUILayout.TextField("收集器名称", field.fieldName);
            var rect2 = EditorGUILayout.GetControlRect();
            fieldName = EditorGUI.TextField(rect2, "字段名称", fieldName);
            if (GUI.Button(new Rect(rect2.x + 100, rect2.y, 20, rect2.height), "+"))
            {
                if (string.IsNullOrEmpty(selectTypeName)) 
                {
                    Debug.Log("请先选择一次字段类型!");
                    return;
                }
                var name = fieldName;
                if (name == "")
                    name = "name" + field.nameIndex++;
                foreach (var f in field.fields)
                {
                    if (f.name == fieldName)
                    {
                        name += field.nameIndex++;
                        break;
                    }
                }
                field.fields.Add(new FieldCollection.Field() { name = name, typeName = selectTypeName });
                EditorUtility.SetDirty(field);
                return;
            }
            search = EditorGUILayout.TextField("字段类型", search);
            if (search != search1)
            {
                selectType = false;
                search1 = search;
                searchTime = DateTime.Now.AddMilliseconds(20);
            }
            if (DateTime.Now > searchTime & !selectType & search.Length > 0)
            {
                foreach (var type1 in types)
                {
                    if (!type1.ToLower().Contains(search))
                        continue;
                    if (GUILayout.Button(type1))
                    {
                        AddField(type1);
                        return;
                    }
                }
            }
            for (int i = 0; i < field.fields.Count; i++)
            {
                try
                {
                    if (deleteArrayIndex != -1)
                    {
                        field.fields.RemoveAt(deleteArrayIndex);
                        deleteArrayIndex = -1;
                        EditorUtility.SetDirty(field);
                        break;
                    }
                    var rect = EditorGUILayout.GetControlRect();
                    so.FindProperty("fields").GetArrayElementAtIndex(i).FindPropertyRelative("target").objectReferenceValue = EditorGUI.ObjectField(rect, field.fields[i].name, field.fields[i].target, field.fields[i].Type, true);
                    if (Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))//判断鼠标右键事件
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("删除字段"), false, (index) =>
                        {
                            deleteArrayIndex = (int)index;
                        }, i);
                        menu.ShowAsContext();
                        Event.current.Use();//设置该事件被使用
                    }
                    if (Event.current.clickCount == 2 && rect.Contains(Event.current.mousePosition))//判断鼠标左键事件
                    {
                        index = i;
                        doubleClick = true;
                    }
                    if (doubleClick & index == i) 
                    {
                        field.fields[i].name = EditorGUI.TextField(rect, field.fields[i].name);
                        if (Event.current.type == EventType.MouseDown | Event.current.keyCode == KeyCode.Return) 
                        {
                            doubleClick = false;
                            index = -1;
                            EditorUtility.SetDirty(field);
                            break;
                        }
                    }
                }
                catch
                {
                }
            }
            if (Event.current.type == EventType.DragUpdated | Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;//拖动时显示辅助图标
                if (Event.current.type == EventType.DragPerform)
                {
                    search1 = "";
                    search = DragAndDrop.objectReferences[0].GetType().Name.ToLower();
                }
            }
            nameSpace = EditorGUILayout.TextField("namespace", nameSpace);
            if (nameSpace != nameSpace1) 
            {
                nameSpace1 = nameSpace;
                SaveData();
            }
            fullpath = EditorGUILayout.Toggle("(绝/相)对路径", fullpath);
            var rect1 = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(rect1, "文件路径:", savePath);
            if (GUI.Button(new Rect(rect1.x + rect1.width - 60, rect1.y, 60, rect1.height), "选择"))
            {
                if (fullpath)
                {
                    savePath = EditorUtility.OpenFolderPanel("选择保存路径", "", "");
                    SaveData();
                }
                else 
                {
                    savePath = EditorUtility.OpenFolderPanel("选择保存路径", "", "");
                    var strs = savePath.ToCharArray();
                    var strs1 = Application.dataPath.Replace("Assets", "").ToCharArray();
                    int index = 0;
                    for (int i = 0; i < strs.Length; i++)
                    {
                        if (i >= strs1.Length)
                        {
                            index = i;
                            break;
                        }
                        if (strs[i] != strs1[i])
                        {
                            index = i;
                            break;
                        }
                    }
                    savePath = savePath.Remove(0, index);
                    SaveData();
                }
            }
            var rect3 = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(rect3, "csproj文件:", csprojFile);
            if (GUI.Button(new Rect(rect3.x + rect3.width - 60, rect3.y, 60, rect3.height), "选择"))
            {
                if (fullpath) 
                {
                    csprojFile = EditorUtility.OpenFilePanel("选择文件", "", "csproj");
                    SaveData();
                }
                else
                {
                    csprojFile = EditorUtility.OpenFilePanel("选择文件", "", "csproj");
                    var strs = csprojFile.ToCharArray();
                    var strs1 = Application.dataPath.Replace("Assets", "").ToCharArray();
                    int index = 0;
                    for (int i = 0; i < strs.Length; i++)
                    {
                        if (i >= strs1.Length)
                        {
                            index = i;
                            break;
                        }
                        if (strs[i] != strs1[i])
                        {
                            index = i;
                            break;
                        }
                    }
                    csprojFile = csprojFile.Remove(0, index);
                    SaveData();
                }
            }
            if (GUILayout.Button("生成脚本(hotfix)"))
            {
                bool hasns = nameSpace != "";
                Func<string> action = new Func<string>(()=> {
                    string str = "";
                    for (int i = 0; i < field.fields.Count; i++) 
                    {
                        str += $"{(hasns ? "\t\t" : "\t")}" + $"public {field.fields[i].Type.Name} {field.fields[i].name};\n";
                    }
                    return str + "\n";
                });
                Func<string> action1 = new Func<string>(() => {
                    string str = "";
                    for (int i = 0; i < field.fields.Count; i++)
                    {
                        str += $"{(hasns ? "\t\t\t" : "\t\t")}" + $"{field.fields[i].name} = fc[\"{field.fields[i].name}\"].target as {field.fields[i].Type.Name};\n";
                    }
                    return str;
                });
                Func<string> action2 = new Func<string>(() => {
                    string str = "";
                    for (int i = 0; i < field.fields.Count; i++)
                    {
                        if (field.fields[i].Type == typeof(Button))
                        {
                            str += $"{(hasns ? "\t\t\t" : "\t\t")}" + $"{field.fields[i].name}.onClick.AddListener(() => " + "{" + "});\n";
                        }
                        else if (field.fields[i].Type == typeof(Toggle))
                        {
                            str += $"{(hasns ? "\t\t\t" : "\t\t")}" + $"{field.fields[i].name}.onValueChanged.AddListener((value) => " + "{" + "});\n";
                        }
                    }
                    return str;
                });
                var scriptStr = "using MVC.View;\n" +
                "using UnityEngine;\n" +
                "using UnityEngine.UI;\n\n" +
                (hasns ? "namespace " + nameSpace + "\n{\n" : "") +
                $"{(hasns ? "\t" : "")}" + $"//热更新生成的脚本, 请看gitee的mvc模块使用介绍图示\n" +
                $"{(hasns ? "\t" : "")}" + $"public class {field.fieldName}\n" +
                $"{(hasns ? "\t" : "")}" + "{\n" +
                $"{(hasns ? "\t\t" : "\t")}" + $"public static {field.fieldName} Instance = new {field.fieldName}();\n" +
                $"{(hasns ? "\t\t" : "\t")}" + "public GameObject panel;\n" +
                action() +
                $"{(hasns ? "\t\t" : "\t")}" + "public void Init(FieldCollection fc)\n" +
                $"{(hasns ? "\t\t" : "\t")}" + "{\n" +
                $"{(hasns ? "\t\t\t" : "\t\t")}" + "panel = fc.gameObject;\n" +
                action1() +
                action2() +
                $"{(hasns ? "\t\t" : "\t")}" + "}\n" +
                $"{(hasns ? "\t" : "")}" + "}" +
                (hasns ? "\n}" : "");
                string path = "";
                string path1 = "";
                if (fullpath)
                {
                    path = savePath + $"/{field.fieldName}.cs";
                    path1 = csprojFile;
                }
                else
                {
                    path = Application.dataPath.Replace("Assets", "") + savePath + $"/{field.fieldName}.cs";
                    path1 = Application.dataPath.Replace("Assets", "") + csprojFile;
                }
                if (File.Exists(path)) 
                {
                    if(!EditorUtility.DisplayDialog("写入脚本文件", "脚本已存在, 是否替换? 或 尾部添加?", "替换", "尾部添加"))
                        File.AppendAllText(path, scriptStr);
                    else File.WriteAllText(path, scriptStr);
                } else File.WriteAllText(path, scriptStr);
                if (File.Exists(path1)) 
                {
                    var rows = File.ReadAllLines(path1);
                    foreach (var row in rows)
                    {
                        if (row.Contains("<Compile Include=\"")) 
                        {
                            var row1 = row.Replace("<Compile Include=\"", "");
                            row1 = row1.Replace("\" />", "");
                            var csName = Path.GetFileName(row1);
                            var csName1 = Path.GetFileName(path);
                            if (csName == csName1)
                                goto J;
                        }
                    }
                    var cspath = Path.GetDirectoryName(path1).Replace("\\", "/");
                    var path2 = path.Replace(cspath, "").TrimStart('/').Replace("/", "\\");
                    List<string> rows1 = new List<string>(rows);
                    rows1.Insert(rows.Length - 3, $"    <Compile Include=\"{path2}\" />");
                    File.WriteAllLines(path1, rows1);
                }
                J: AssetDatabase.Refresh();
                Debug.Log($"生成成功:{path}");
            }
            if (GUILayout.Button("生成脚本(主工程)"))
            {
                bool hasns = nameSpace != "";
                Func<string> action = new Func<string>(() => {
                    string str = "";
                    for (int i = 0; i < field.fields.Count; i++)
                    {
                        str += $"{(hasns ? "\t\t" : "\t")}" + $"private {field.fields[i].Type.Name} {field.fields[i].name};\n";
                    }
                    return str;
                });
                Func<string> action1 = new Func<string>(() => {
                    string str = "";
                    int index = 0;
                    for (int i = 0; i < field.fields.Count; i++)
                    {
                        var comps = field.transform.GetComponentsInChildren(field.fields[i].Type);
                        for (int ii = 0; ii < comps.Length; ii++) 
                        {
                            var comp = field.fields[i].target as Component;
                            if (comp == comps[ii]) {
                                index = ii;
                                break;
                            }
                        }
                        str += $"{(hasns ? "\t\t\t" : "\t\t")}" + $"{field.fields[i].name} = transform.GetComponentsInChildren<{field.fields[i].Type.Name}>()[{index}];\n";
                    }
                    return str;
                });
                Func<string> action2 = new Func<string>(() => {
                    string str = "";
                    for (int i = 0; i < field.fields.Count; i++)
                    {
                        if (field.fields[i].Type == typeof(Button))
                        {
                            str += $"{(hasns ? "\t\t\t" : "\t\t")}" + $"{field.fields[i].name}.onClick.AddListener(() => " + "{" + "});\n";
                        }
                    }
                    return str;
                });
                var scriptStr = "using Net.Component;\n" +
                "using UnityEngine;\n" +
                "using UnityEngine.UI;\n\n" +
                (hasns ? "namespace " + nameSpace + "\n{\n" : "") +
                $"{(hasns ? "\t" : "")}public class {field.fieldName} : SingleCase<{field.fieldName}>\n" +
                $"{(hasns ? "\t" : "")}" + "{\n" +
                action() +
                $"\n{(hasns ? "\t\t" : "\t")}void Start()\n" +
                $"{(hasns ? "\t\t" : "\t")}" + "{\n" +
                action1() +
                action2() +
                $"{(hasns ? "\t\t" : "\t")}" + "}\n" +
                $"{(hasns ? "\t" : "")}" + "}" +
                (hasns ? "\n}" : "");
                string path = "";
                if (fullpath)
                    path = savePath + $"/{field.fieldName}.cs";
                else
                    path = Application.dataPath.Replace("Assets", "") + savePath + $"/{field.fieldName}.cs";
                if (File.Exists(path)) 
                {
                    if(!EditorUtility.DisplayDialog("写入脚本文件", "脚本已存在, 是否替换? 或 尾部添加?", "替换", "尾部添加"))
                        File.AppendAllText(path, scriptStr);
                    else File.WriteAllText(path, scriptStr);
                } else File.WriteAllText(path, scriptStr);
                //csproj对主工程无效
                AssetDatabase.Refresh();
                Debug.Log($"生成成功:{path}");
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif