using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class Fast2BuildTools : EditorWindow
{
    private string bindTypeName = "Net.Binding.BindingEntry";
    private string methodName = "GetBindTypes";
    private string savePath;
    private string bindTypeName1;
    private string methodName1;

    [MenuItem("GameDesigner/Network/Fast2BuildTool")]
    static void ShowWindow()
    {
        var window = GetWindow<Fast2BuildTools>("快速序列化2生成工具");
        window.position = new Rect(window.position.position, new Vector2(400,200));
        window.Show();
    }

    private void OnEnable()
    {
        var path = Application.dataPath.Replace("Assets", "") + "data.txt";
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
                Fast2BuildToolMethod.Build(type, savePath);
                Fast2BuildToolMethod.BuildArray(type, savePath);
                Fast2BuildToolMethod.BuildGeneric(type, savePath);
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
        var path = Application.dataPath.Replace("Assets", "") + "data.txt";
        File.WriteAllText(path, jsonstr);
    }

    internal class Data
    {
        public string typeName;
        public string methodName;
        public string savepath;
    }
}
#endif