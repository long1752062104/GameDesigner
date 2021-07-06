#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding : EditorWindow
{
    private string path;

    [MenuItem("ILRuntime/通过自动分析热更DLL生成CLR绑定")]
    static void GenerateCLRBindingByAnalysis()
    {
        GetWindow<ILRuntimeCLRBinding>().Show();
    }

    private void OnEnable()
    {
        LoadData();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("dll路径:", path);
        if (GUILayout.Button("选择路径", GUILayout.Width(100)))
        {
            path = EditorUtility.OpenFilePanel("保存路径", "", "dll");
            SaveData();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("生成CLR绑定", GUILayout.Height(30))) 
        {
            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                domain.LoadAssembly(fs);
                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/Scripts/Generated");
            }
            AssetDatabase.Refresh();
        }
    }

    void LoadData()
    {
        if (File.Exists(Application.dataPath.Replace("Assets", "") + "ilrdata.txt"))
        {
            var fcdata = File.ReadAllText(Application.dataPath.Replace("Assets", "") + "ilrdata.txt");
            path = Newtonsoft_X.Json.JsonConvert.DeserializeObject<string>(fcdata);
        }
    }

    void SaveData()
    {
        var path = Application.dataPath.Replace("Assets", "") + "ilrdata.txt";
        var jsonstr = Newtonsoft_X.Json.JsonConvert.SerializeObject(this.path);
        File.WriteAllText(path, jsonstr);
    }
}
#endif
