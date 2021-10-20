#if UNITY_EDITOR
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Fast2BuildTools1 : EditorWindow
{
    private string bindTypeName = "Net.Binding.BindingEntry";
    private string methodName = "GetBindTypes";
    private string savePath;
    private string bindTypeName1;
    private string methodName1;

    [MenuItem("GameDesigner/Network/Fast2BuildTool-1")]
    static void ShowWindow()
    {
        var window = GetWindow<Fast2BuildTools1>("快速序列化2生成工具");
        window.position = new Rect(window.position.position, new Vector2(400,200));
        window.Show();
    }

    private void OnEnable()
    {
        var path = Application.dataPath.Replace("Assets", "") + "data1.txt";
        if (File.Exists(path))
        {
            var jsonStr = File.ReadAllText(path);
            var data = Newtonsoft_X.Json.JsonConvert.DeserializeObject<Data>(jsonStr);
            bindTypeName = data.typeName;
            methodName = data.methodName;
            savePath = data.savepath;
        }
    }

    private void OnGUI()
    {
        bindTypeName = EditorGUILayout.TextField("入口类型:", bindTypeName);
        methodName = EditorGUILayout.TextField("入口方法:", methodName);
        if (bindTypeName != bindTypeName1 | methodName != methodName1)
        {
            bindTypeName1 = bindTypeName;
            methodName1 = methodName;
            Save();
        }
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:", savePath);
        if (GUILayout.Button("选择路径", GUILayout.Width(100)))
        {
            savePath = EditorUtility.OpenFolderPanel("保存路径", "", "");
            Save();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("生成序列化代码", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(savePath)) 
            {
                EditorUtility.DisplayDialog("提示", "请选择生成脚本路径!", "确定");
                return;
            }
            var assembly = Assembly.GetAssembly(typeof(Net.Binding.BindingEntry));
            Debug.Log(assembly);
            var bindType = assembly.GetType(bindTypeName);
            if (bindType == null)
                throw new Exception("获取类型失败!");
            var method = bindType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            IList<Type> list = (IList<Type>)method.Invoke(null, null);
            foreach (var type in list)
            {
                Fast2BuildMethod.Build(type, true, savePath);
                Fast2BuildMethod.BuildArray(type, true, savePath);
                Fast2BuildMethod.BuildGeneric(type, true, savePath);
            }
            Debug.Log("生成完成.");
            AssetDatabase.Refresh();
        }
        EditorGUILayout.HelpBox("指定主入口类型和调用入口方法，然后选择生成代码文件夹路径，最后点击生成。绑定入口案例:请看Net.Binding.BindingEntry类的GetBindTypes方法", MessageType.Info);
    }

    void Save()
    {
        Data data = new Data() { typeName = bindTypeName, methodName = methodName, savepath = savePath };
        var jsonstr = Newtonsoft_X.Json.JsonConvert.SerializeObject(data);
        var path = Application.dataPath.Replace("Assets", "") + "data1.txt";
        File.WriteAllText(path, jsonstr);
    }

    internal class Data
    {
        public string typeName;
        public string methodName;
        public string savepath;
    }
}

public class Fast2BuildTools2 : EditorWindow
{
    private List<string> typeNames = new List<string>();
    private bool selectType;
    private string search = "", search1 = "";
    private DateTime searchTime;
    private string[] types;
    private Vector2 scrollPosition;
    private Vector2 scrollPosition1;
    private string savePath, savePath1;

    [MenuItem("GameDesigner/Network/Fast2BuildTool-2")]
    static void ShowWindow()
    {
        var window = GetWindow<Fast2BuildTools2>("快速序列化2生成工具");
        window.position = new Rect(window.position.position, new Vector2(400, 200));
        window.Show();
    }

    private void OnEnable()
    {
        HashSet<string> types1 = new HashSet<string>();
        var types2 = typeof(MVC.Control.GameInit).Assembly.GetTypes().Where(t => !t.IsAbstract & !t.IsInterface & !t.IsGenericType).ToArray();
        foreach (var obj in types2)
        {
            var str = obj.FullName;
            if (!types1.Contains(str))
                types1.Add(str);
        }
        types = types1.ToArray();
        LoadData();
    }

    private void OnGUI()
    {
        search = EditorGUILayout.TextField("搜索绑定类型", search);
        EditorGUILayout.LabelField("绑定类型列表:");
        if (typeNames.Count != 0)
        {
            scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, true, GUILayout.MaxHeight(300));
            foreach (var type1 in typeNames)
            {
                if (GUILayout.Button(type1))
                {
                    typeNames.Remove(type1);
                    SaveData();
                    return;
                }
            }
            GUILayout.EndScrollView();
        }
        if (search != search1)
        {
            selectType = false;
            search1 = search;
            searchTime = DateTime.Now.AddMilliseconds(20);
        }
        if (DateTime.Now > searchTime & !selectType & search.Length > 0)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.MaxHeight(400));
            foreach (var type1 in types)
            {
                if (!type1.ToLower().Contains(search.ToLower()))
                    continue;
                if (GUILayout.Button(type1))
                {
                    if (!typeNames.Contains(type1))
                        typeNames.Add(type1);
                    SaveData();
                    return;
                }
            }
            GUILayout.EndScrollView();
        }
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:", savePath);
        if (GUILayout.Button("选择路径", GUILayout.Width(100)))
        {
            savePath = EditorUtility.OpenFolderPanel("保存路径", "", "");
            SaveData();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径1:", savePath1);
        if (GUILayout.Button("选择路径1", GUILayout.Width(100)))
        {
            savePath1 = EditorUtility.OpenFolderPanel("保存路径", "", "");
            SaveData();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("生成绑定代码", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(savePath))
            {
                EditorUtility.DisplayDialog("提示", "请选择生成脚本路径!", "确定");
                return;
            }
            foreach (var type1 in typeNames)
            {
                Type type = Net.Serialize.NetConvertOld.GetType(type1);
                Fast2BuildMethod.Build(type, true, savePath);
                Fast2BuildMethod.BuildArray(type, true, savePath);
                Fast2BuildMethod.BuildGeneric(type, true, savePath);
                if (!string.IsNullOrEmpty(savePath1)) 
                {
                    Fast2BuildMethod.Build(type, true, savePath1);
                    Fast2BuildMethod.BuildArray(type, true, savePath1);
                    Fast2BuildMethod.BuildGeneric(type, true, savePath1);
                }
            }
            Debug.Log("生成完成.");
            AssetDatabase.Refresh();
        }
        EditorGUILayout.HelpBox("指定主入口类型和调用入口方法，然后选择生成代码文件夹路径，最后点击生成。绑定入口案例:请看Net.Binding.BindingEntry类的GetBindTypes方法", MessageType.Info);
    }

    void LoadData() 
    {
        var path = Application.dataPath.Replace("Assets", "") + "data2.txt";
        if (File.Exists(path))
        {
            var jsonStr = File.ReadAllText(path);
            var data = Newtonsoft_X.Json.JsonConvert.DeserializeObject<Data>(jsonStr);
            typeNames = data.typeNames;
            savePath = data.savepath; 
            savePath1 = data.savepath1;
        }
    }

    void SaveData()
    {
        Data data = new Data() { 
            typeNames = typeNames,
            savepath = savePath,
            savepath1 = savePath1,
        };
        var jsonstr = Newtonsoft_X.Json.JsonConvert.SerializeObject(data);
        var path = Application.dataPath.Replace("Assets", "") + "data2.txt";
        File.WriteAllText(path, jsonstr);
    }

    internal class Data
    {
        public string savepath, savepath1;
        public List<string> typeNames;
    }
}
#endif