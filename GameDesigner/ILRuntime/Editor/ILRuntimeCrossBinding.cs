#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCrossBinding : EditorWindow
{
    [MenuItem("ILRuntime/生成跨域继承适配器")]
    static void GenerateCrossbindAdapter()
    {
        //由于跨域继承特殊性太多，自动生成无法实现完全无副作用生成，所以这里提供的代码自动生成主要是给大家生成个初始模版，简化大家的工作
        //大多数情况直接使用自动生成的模版即可，如果遇到问题可以手动去修改生成后的文件，因此这里需要大家自行处理是否覆盖的问题
        GetWindow<ILRuntimeCrossBinding>().Show();
    }

    private List<string> typeNames = new List<string>();
    private bool selectType;
    private string search = "", search1 = "";
    private DateTime searchTime;
    private string[] types;
    private Vector2 scrollPosition;
    private Vector2 scrollPosition1;

    private void OnEnable()
    {
        HashSet<string> types1 = new HashSet<string>();
        var types2 = typeof(MVC.Control.GameInit).Assembly.GetTypes().Where(t => !t.IsAbstract & !t.IsInterface & !t.IsGenericType ).ToArray();
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
        search = EditorGUILayout.TextField("跨域适配器类型", search);
        EditorGUILayout.LabelField("生成列表:");
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
        if (GUILayout.Button("生成跨域继承适配器", GUILayout.Height(30)))
        {
            foreach (var type1 in typeNames)
            {
                var fn = type1.Replace(".", "_");
                if (!Directory.Exists("Assets/Scripts/Generated/"))
                    Directory.CreateDirectory("Assets/Scripts/Generated/");
                using (StreamWriter sw = new StreamWriter($"Assets/Scripts/Generated/{fn}_Adapter.cs"))
                {
                    Type type = Net.Serialize.NetConvertOld.GetType(type1);
                    sw.WriteLine(ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(type, "ILRuntime.Runtime.Generated"));
                }
            }
            AssetDatabase.Refresh();
        }
    }

    void LoadData()
    {
        if (File.Exists(Application.dataPath.Replace("Assets", "") + "ilrdata1.txt"))
        {
            var fcdata = File.ReadAllText(Application.dataPath.Replace("Assets", "") + "ilrdata1.txt");
            typeNames = Newtonsoft_X.Json.JsonConvert.DeserializeObject<List<string>>(fcdata);
        }
    }

    void SaveData()
    {
        var path = Application.dataPath.Replace("Assets", "") + "ilrdata1.txt";
        var jsonstr = Newtonsoft_X.Json.JsonConvert.SerializeObject(typeNames);
        File.WriteAllText(path, jsonstr);
    }
}
#endif
