#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildComponentTools : EditorWindow
{
    private string savePath, savePath1;
    private Component component;

    [MenuItem("GameDesigner/Network/BuildComponentTools")]
    public static void Init()
    {
        BlueprintGUILayout.Instance.GraphEditor = GetWindow<BuildComponentTools>("BuildComponentTools", true);
    }
    private void OnEnable()
    {
        LoadData();
    }
    void OnGUI()
    {
        component = (Component)EditorGUILayout.ObjectField("组件", component, typeof(Component), true);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:", savePath);
        if (GUILayout.Button("选择路径", GUILayout.Width(100)))
        {
            savePath = EditorUtility.OpenFolderPanel("保存路径", "", "");
            SaveData();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("生成同步组件脚本", GUILayout.Height(40)))
        {
            if (string.IsNullOrEmpty(savePath))
            {
                EditorUtility.DisplayDialog("提示", "请选择生成脚本路径!", "确定");
                return;
            }
            if (component == null)
            {
                EditorUtility.DisplayDialog("提示", "请选择unity组件!", "确定");
                return;
            }
            var type = component.GetType();
            var str = Build(type);
            File.WriteAllText(savePath + $"//Network{type.Name}.cs", str.ToString());
            Debug.Log("生成脚本成功!"); 
            AssetDatabase.Refresh();
        }
    }

    static StringBuilder Build(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
        StringBuilder str = new StringBuilder();
        str.AppendLine("#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA");
        str.AppendLine("using Net.Component;");
        str.AppendLine("using Net.Share;");
        str.AppendLine("using Net.UnityComponent;");
        str.AppendLine("using UnityEngine;");
        str.AppendLine("");
        str.AppendLine("namespace BuildComponent");
        str.AppendLine("{");
        str.AppendLine("    /// <summary>");
        str.AppendLine($"    /// {type.Name}同步组件, 此代码由BuildComponentTools工具生成");
        str.AppendLine("    /// </summary>");
        str.AppendLine($"    [RequireComponent(typeof({type.FullName}))]");
        str.AppendLine($"    public class Network{type.Name} : NetworkBehaviour");
        str.AppendLine("    {");
        str.AppendLine("");
        str.AppendLine($"       private {type.FullName} self;");
        str.AppendLine($"       public bool autoCheck;");
        int parNum = 0;
        List<string> propertyCodes = new List<string>();
        for (int i = 0; i < properties.Length; i++)
        {
            var item = properties[i];
            if (!item.CanRead | !item.CanWrite | item.GetCustomAttribute<ObsoleteAttribute>() != null)
                continue;
            var ptype = item.PropertyType;
            var code = Type.GetTypeCode(ptype);
            if (code == TypeCode.Object & ptype != typeof(Vector2) & ptype != typeof(Vector3) & ptype != typeof(Vector4) &
                ptype != typeof(Rect) & ptype != typeof(Quaternion) & ptype != typeof(Color)
                & ptype != typeof(Color32) & ptype != typeof(Net.Vector2) & ptype != typeof(Net.Vector3)
                & ptype != typeof(Net.Vector4) & ptype != typeof(Net.Rect) & ptype != typeof(Net.Quaternion)
                & ptype != typeof(Net.Color) & ptype != typeof(Net.Color32)
                )
                continue;
            parNum++;
            var fieldName = item.Name + parNum;
            str.AppendLine($"       private {item.PropertyType.FullName} {fieldName};");
            propertyCodes.Add($"                {item.Name} = {item.Name};");
        }
        parNum = 0;
        for (int i = 0; i < methods.Length; i++)
        {
            var met = methods[i];
            if (met.MethodImplementationFlags == MethodImplAttributes.InternalCall | met.Name.Contains("get_") | met.Name.Contains("set_") |
                met.GetCustomAttribute<ObsoleteAttribute>() != null | met.IsGenericMethod)
                continue;
            var pars = met.GetParameters();
            bool not = false;
            foreach (var item in pars)
            {
                var ptype = item.ParameterType;
                var code = Type.GetTypeCode(ptype);
                if (code == TypeCode.Object & ptype != typeof(Vector2) & ptype != typeof(Vector3) & ptype != typeof(Vector4) &
                    ptype != typeof(Rect) & ptype != typeof(Quaternion) & ptype != typeof(Color)
                    & ptype != typeof(Color32) & ptype != typeof(Net.Vector2) & ptype != typeof(Net.Vector3)
                    & ptype != typeof(Net.Vector4) & ptype != typeof(Net.Rect) & ptype != typeof(Net.Quaternion)
                    & ptype != typeof(Net.Color) & ptype != typeof(Net.Color32)
                    )
                {
                    not = true;
                    break;
                }
            }
            if (not)
                continue;
            foreach (var item in pars)
            {
                parNum++;
                var fieldName = item.Name + parNum;
                str.AppendLine($"       private {item.ParameterType.FullName} {fieldName};");
            }
        }
        str.AppendLine("");
        str.AppendLine("       public override void Awake()");
        str.AppendLine("      {");
        str.AppendLine("          base.Awake();");
        str.AppendLine($"          self = GetComponent<{type.FullName}>();");
        str.AppendLine("      }");
        str.AppendLine("");
        parNum = 0;
        for (int i = 0; i < properties.Length; i++)
        {
            var item = properties[i];
            if (!item.CanRead | !item.CanWrite | item.GetCustomAttribute<ObsoleteAttribute>() != null)
                continue;
            var ptype = item.PropertyType;
            var code = Type.GetTypeCode(ptype);
            if (code == TypeCode.Object & ptype != typeof(Vector2) & ptype != typeof(Vector3) & ptype != typeof(Vector4) &
                ptype != typeof(Rect) & ptype != typeof(Quaternion) & ptype != typeof(Color)
                & ptype != typeof(Color32) & ptype != typeof(Net.Vector2) & ptype != typeof(Net.Vector3)
                & ptype != typeof(Net.Vector4) & ptype != typeof(Net.Rect) & ptype != typeof(Net.Quaternion)
                & ptype != typeof(Net.Color) & ptype != typeof(Net.Color32)
                )
                continue;
            str.AppendLine($"      public {ptype.FullName} {item.Name}");
            str.AppendLine("      {");
            str.AppendLine("          get");
            str.AppendLine("          {");
            str.AppendLine($"              return self.{item.Name};");
            str.AppendLine("          }");
            str.AppendLine("          set");
            str.AppendLine("          {");
            parNum++;
            var fieldName = item.Name + parNum;
            str.AppendLine($"               if ({fieldName} == value)");
            str.AppendLine("                    return;");
            str.AppendLine($"               {fieldName} = value;");
            str.AppendLine("                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)");
            str.AppendLine("                {");
            str.AppendLine($"                    index = networkIdentity.registerObjectIndex,");
            str.AppendLine($"                    index1 = {i},");
            str.AppendLine("                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)");
            str.AppendLine("                });");
            str.AppendLine("          }");
            str.AppendLine("     }");
            str.AppendLine("");
        }
        parNum = 0;
        for (int i = 0; i < methods.Length; i++)
        {
            var met = methods[i];
            if (met.MethodImplementationFlags == MethodImplAttributes.InternalCall | met.Name.Contains("get_") | met.Name.Contains("set_") |
                met.GetCustomAttribute<ObsoleteAttribute>() != null | met.IsGenericMethod)
                continue;
            var pars = met.GetParameters();
            bool not = false;
            foreach (var item in pars)
            {
                var ptype = item.ParameterType;
                var code = Type.GetTypeCode(ptype);
                if (code == TypeCode.Object & ptype != typeof(Vector2) & ptype != typeof(Vector3) & ptype != typeof(Vector4) &
                    ptype != typeof(Rect) & ptype != typeof(Quaternion) & ptype != typeof(Color)
                    & ptype != typeof(Color32) & ptype != typeof(Net.Vector2) & ptype != typeof(Net.Vector3)
                    & ptype != typeof(Net.Vector4) & ptype != typeof(Net.Rect) & ptype != typeof(Net.Quaternion)
                    & ptype != typeof(Net.Color) & ptype != typeof(Net.Color32)
                    )
                {
                    not = true;
                    break;
                }
            }
            if (not)
                continue;
            var value = "";
            foreach (var item in pars)
                value += item.ParameterType.FullName + " " + item.Name + ",";
            str.AppendLine($"public void {met.Name}({value} bool always = false)");
            str.AppendLine("{");
            if (pars.Length > 0)
            {
                value = "     if(";
                var list = new List<string>();
                foreach (var item in pars)
                {
                    parNum++;
                    var fieldName = item.Name + parNum;
                    value += $"{item.Name} == {fieldName} & ";
                    list.Add($"     {fieldName} = {item.Name};");
                }
                value += "!always) return;";
                str.AppendLine(value);
                foreach (var item in list)
                {
                    str.AppendLine(item);
                }
            }
            value = "";
            foreach (var item in pars)
                value += item.Name + ",";
            value = value.TrimEnd(',');
            str.AppendLine($"   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() {{ pars = new object[] {{ {value} }} }});");
            str.AppendLine("    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)");
            str.AppendLine("    {");
            str.AppendLine($"       index = networkIdentity.registerObjectIndex,");
            str.AppendLine($"       index1 = {properties.Length + i},");
            str.AppendLine("        buffer = buffer");
            str.AppendLine("    });");
            str.AppendLine("}");
        }
        str.AppendLine("     public override void OnPropertyAutoCheck()");
        str.AppendLine("     {");
        str.AppendLine("     if (!autoCheck)");
        str.AppendLine("         return;");
        foreach (var item in propertyCodes)
        {
            str.AppendLine(item);
        }
        str.AppendLine("     }");
        str.AppendLine("");
        str.AppendLine("     public override void OnNetworkOperationHandler(Operation opt)");
        str.AppendLine("     {");
        str.AppendLine("         if (opt.cmd != Command.BuildComponent)");
        str.AppendLine("             return;");
        str.AppendLine("         switch (opt.index1)");
        str.AppendLine("         {");
        parNum = 0;
        for (int i = 0; i < properties.Length; i++)
        {
            var item = properties[i];
            if (!item.CanRead | !item.CanWrite | item.GetCustomAttribute<ObsoleteAttribute>() != null)
                continue;
            var ptype = item.PropertyType;
            var code = Type.GetTypeCode(ptype);
            if (code == TypeCode.Object & ptype != typeof(Vector2) & ptype != typeof(Vector3) & ptype != typeof(Vector4) &
                ptype != typeof(Rect) & ptype != typeof(Quaternion) & ptype != typeof(Color)
                & ptype != typeof(Color32) & ptype != typeof(Net.Vector2) & ptype != typeof(Net.Vector3)
                & ptype != typeof(Net.Vector4) & ptype != typeof(Net.Rect) & ptype != typeof(Net.Quaternion)
                & ptype != typeof(Net.Color) & ptype != typeof(Net.Color32)
                )
                continue;
            parNum++;
            var fieldName = item.Name + parNum;
            str.AppendLine($"             case {i}:");
            str.AppendLine($"                 {fieldName} = Net.Serialize.NetConvertFast2.DeserializeObject<{ptype.FullName}>(new Net.System.Segment(opt.buffer, false));");
            str.AppendLine($"                 self.{item.Name} = {fieldName};");
            str.AppendLine("                break;");
        }
        for (int i = 0; i < methods.Length; i++)
        {
            var met = methods[i];
            if (met.MethodImplementationFlags == MethodImplAttributes.InternalCall | met.Name.Contains("get_") | met.Name.Contains("set_") |
                met.GetCustomAttribute<ObsoleteAttribute>() != null | met.IsGenericMethod)
                continue;
            var pars = met.GetParameters();
            bool not = false;
            foreach (var item in pars)
            {
                var ptype = item.ParameterType;
                var code = Type.GetTypeCode(ptype);
                if (code == TypeCode.Object & ptype != typeof(Vector2) & ptype != typeof(Vector3) & ptype != typeof(Vector4) &
                    ptype != typeof(Rect) & ptype != typeof(Quaternion) & ptype != typeof(Color)
                    & ptype != typeof(Color32) & ptype != typeof(Net.Vector2) & ptype != typeof(Net.Vector3)
                    & ptype != typeof(Net.Vector4) & ptype != typeof(Net.Rect) & ptype != typeof(Net.Quaternion)
                    & ptype != typeof(Net.Color) & ptype != typeof(Net.Color32)
                    )
                {
                    not = true;
                    break;
                }
            }
            if (not)
                continue;
            str.AppendLine($"             case {properties.Length + i}:");
            str.AppendLine("              {");
            str.AppendLine("                    var segment = new Net.System.Segment(opt.buffer, false);");
            str.AppendLine($"                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);");
            string value = "";
            for (int n = 0; n < pars.Length; n++)
            {
                if (pars[n].ParameterType.IsValueType)
                    str.AppendLine($"           var {pars[n].Name} = ({pars[n].ParameterType.FullName})data.pars[{n}];");
                else
                    str.AppendLine($"           var {pars[n].Name} = data.pars[{n}] as {pars[n].ParameterType.FullName};");
                value += pars[n].Name + ",";
            }
            value = value.TrimEnd(',');
            str.AppendLine($"                   self.{met.Name}({value});");
            str.AppendLine("              }");
            str.AppendLine("                break;");
        }
        str.AppendLine("");
        str.AppendLine("        }");
        str.AppendLine("     }");
        str.AppendLine("  }");
        str.AppendLine("}");
        str.AppendLine("#endif");
        return str;
    }

    void LoadData()
    {
        var path = Application.dataPath.Replace("Assets", "") + "data3.txt";
        if (File.Exists(path))
        {
            var jsonStr = File.ReadAllText(path);
            var data = Newtonsoft_X.Json.JsonConvert.DeserializeObject<Data>(jsonStr);
            savePath = data.savepath;
            savePath1 = data.savepath1;
        }
    }
    void SaveData()
    {
        Data data = new Data()
        {
            savepath = savePath,
            savepath1 = savePath1,
        };
        var jsonstr = Newtonsoft_X.Json.JsonConvert.SerializeObject(data);
        var path = Application.dataPath.Replace("Assets", "") + "data3.txt";
        File.WriteAllText(path, jsonstr);
    }
    internal class Data
    {
        public string savepath, savepath1;
    }
}
#endif