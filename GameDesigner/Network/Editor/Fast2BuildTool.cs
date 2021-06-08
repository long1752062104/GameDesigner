using System.IO;
using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class Fast2BuildTools : EditorWindow
{
    private string bindTypeName = "BindingEntry";
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
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Data>(jsonStr);
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
        var jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(data);
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

public static class Fast2BuildToolMethod 
{
    private class Member
    {
        internal string Name;
        internal bool IsPrimitive;
        internal bool IsEnum;
        internal bool IsArray;
        internal bool IsGenericType;
        internal Type Type;
        internal TypeCode TypeCode;
        internal Type ItemType;
    }

    public static void Build(Type type, string savePath)
    {
        StringBuilder str = new StringBuilder();
        bool hasns = !string.IsNullOrEmpty(type.Namespace);
        str.AppendLine("using System;");
        str.AppendLine("using System.Collections.Generic;");
        str.AppendLine("using Net.Share;");
        str.AppendLine("");
        str.AppendLine(hasns ? $"namespace Binding\n" + "{" : "");
        var className = type.FullName.Replace(".", "");
        str.AppendLine($"{(hasns ? "\t" : "")}public struct {className}Bind : ISerialize<{type.FullName}>");
        str.AppendLine($"{(hasns ? "\t{" : "{")}");
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        List<Member> members = new List<Member>();
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<NonSerializedAttribute>() != null)
                continue;
            var member = new Member()
            {
                IsArray = field.FieldType.IsArray,
                IsEnum = field.FieldType.IsEnum,
                IsGenericType = field.FieldType.IsGenericType,
                IsPrimitive = field.FieldType.IsPrimitive,
                Name = field.Name,
                Type = field.FieldType,
                TypeCode = Type.GetTypeCode(field.FieldType)
            };
            if (field.FieldType.IsArray)
            {
                var serType = field.FieldType.GetInterface("IList`1");
                var itemType = serType.GetGenericArguments()[0];
                member.ItemType = itemType;
            }
            else if (field.FieldType.GenericTypeArguments.Length == 1)
            {
                Type itemType = field.FieldType.GenericTypeArguments[0];
                member.ItemType = itemType;
            }
            else if (field.FieldType.GenericTypeArguments.Length == 2)
            {
                throw new Exception("尚未支持字典类型!");
            }
            members.Add(member);
        }
        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<NonSerializedAttribute>() != null)
                continue;
            if (!property.CanRead | !property.CanWrite)
                continue;
            if (property.GetIndexParameters().Length > 0)
                continue;
            var member = new Member()
            {
                IsArray = property.PropertyType.IsArray,
                IsEnum = property.PropertyType.IsEnum,
                IsGenericType = property.PropertyType.IsGenericType,
                IsPrimitive = property.PropertyType.IsPrimitive,
                Name = property.Name,
                Type = property.PropertyType,
                TypeCode = Type.GetTypeCode(property.PropertyType)
            };
            if (property.PropertyType.IsArray)
            {
                var serType = property.PropertyType.GetInterface("IList`1");
                var itemType = serType.GetGenericArguments()[0];
                member.ItemType = itemType;
            }
            else if (property.PropertyType.GenericTypeArguments.Length == 1)
            {
                Type itemType = property.PropertyType.GenericTypeArguments[0];
                member.ItemType = itemType;
            }
            else if (property.PropertyType.GenericTypeArguments.Length == 2)
            {
                throw new Exception("尚未支持字典类型!");
            }
            members.Add(member);
        }
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public void Write({type.FullName} value, Segment strem)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}int pos = strem.Position;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}strem.Position += {((members.Count - 1) / 8) + 1};");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}byte[] bits = new byte[{((members.Count - 1) / 8) + 1}];");
        for (int i = 0; i < members.Count; i++)
        {
            int bitInx1 = i % 8;
            int bitPos = i / 8;
            var typecode = Type.GetTypeCode(members[i].Type);
            if (typecode != TypeCode.Object)
            {
                if (typecode == TypeCode.String)
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if (!string.IsNullOrEmpty(value.{members[i].Name}))");
                else if (typecode == TypeCode.Boolean)
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != false)");
                else if (typecode == TypeCode.DateTime)
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != default)");
                else
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != 0)");
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}NetConvertBase.SetBit(ref bits[{bitPos}], {++bitInx1}, true);");
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}strem.WriteValue(value.{members[i].Name});");
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
            }
            else if (members[i].Type.IsArray)
            {
                typecode = Type.GetTypeCode(members[i].ItemType);
                if (typecode != TypeCode.Object)
                {
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != null)");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}NetConvertBase.SetBit(ref bits[{bitPos}], {++bitInx1}, true);");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}strem.WriteArray(value.{members[i].Name});");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                }
                else
                {
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != null)");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}int count = value.{members[i].Name}.Length;");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}strem.WriteValue(count);");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}if (count == 0) goto JMP;");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}NetConvertBase.SetBit(ref bits[{bitPos}], {++bitInx1}, true);");
                    var local = members[i].ItemType.FullName.Replace(".", "") + "Bind";
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}{local} bind = new {local}();");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}foreach (var value1 in value.{members[i].Name})");
                    str.AppendLine($"{(hasns ? "\t\t\t\t\t" : "\t\t\t\t")}bind.Write(value1, strem);");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}JMP:;");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                }
            }
            else
            {
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != null)");
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}NetConvertBase.SetBit(ref bits[{bitPos}], {++bitInx1}, true);");
                var local = members[i].Type.FullName.Replace(".", "") + "Bind";
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}{local} bind = new {local}();");
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}bind.Write(value.{members[i].Name}, strem);");
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
            }
        }
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}int pos1 = strem.Position;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}strem.Position = pos;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}strem.Write(bits, 0, {((members.Count - 1) / 8) + 1});");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}strem.Position = pos1;");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");
        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public {type.FullName} Read(Segment strem)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");

        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}byte[] bits = strem.Read({((members.Count - 1) / 8) + 1});");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var value = new {type.FullName}();");
        for (int i = 0; i < members.Count; i++)
        {
            int bitInx1 = i % 8;
            int bitPos = i / 8;
            var typecode = Type.GetTypeCode(members[i].Type);
            if (typecode != TypeCode.Object)
            {
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                if (members[i].IsEnum)
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = strem.ReadValue<{members[i].Type.FullName}>();");
                else
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = strem.ReadValue<{members[i].Type.Name}>();");
            }
            else if (members[i].Type.IsArray)
            {
                typecode = Type.GetTypeCode(members[i].ItemType);
                if (typecode != TypeCode.Object)
                {
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = strem.ReadArray<{members[i].ItemType.FullName}>();");
                }
                else
                {
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}var count = strem.ReadValue<int>();");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = new {members[i].ItemType.FullName}[count];");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}if (count == 0) goto JMP;");
                    var local = members[i].ItemType.FullName.Replace(".", "") + "Bind";
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}{local} bind = new {local}();");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}for (int i = 0; i < count; i++)");
                    str.AppendLine($"{(hasns ? "\t\t\t\t\t" : "\t\t\t\t")}value.{members[i].Name}[i] = bind.Read(strem);");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}JMP:;");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                }
            }
            else
            {
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                var local = members[i].Type.FullName.Replace(".", "") + "Bind";
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}{local} bind = new {local}();");
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = bind.Read(strem);");
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
            }
        }
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}return value;");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");
        str.AppendLine($"{(hasns ? "\t}" : "}")}");
        if (hasns) str.AppendLine("}");
        File.WriteAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static void BuildArray(Type type, string savePath)
    {
        StringBuilder str = new StringBuilder();
        bool hasns = !string.IsNullOrEmpty(type.Namespace);
        str.AppendLine("");
        str.AppendLine(hasns ? $"namespace Binding\n" + "{" : "");
        var className = type.FullName.Replace(".", "");
        str.AppendLine($"{(hasns ? "\t" : "")}public struct {className}ArrayBind : ISerialize<{type.FullName}[]>");
        str.AppendLine($"{(hasns ? "\t{" : "{")}");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public void Write({type.FullName}[] value, Segment strem)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}int count = value.Length;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}strem.WriteValue(count);");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if (count == 0) return;");
        var local = type.FullName.Replace(".", "") + "Bind";
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}{local} bind = new {local}();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}foreach (var value1 in value)");
        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}bind.Write(value1, strem);");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");
        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public {type.FullName}[] Read(Segment strem)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");

        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var count = strem.ReadValue<int>();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var value = new {type.FullName}[count];");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if (count == 0) return value;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}{local} bind = new {local}();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}for (int i = 0; i < count; i++)");
        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value[i] = bind.Read(strem);");

        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}return value;");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");
        str.AppendLine($"{(hasns ? "\t}" : "}")}");
        if (hasns) str.AppendLine("}");
        File.AppendAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static void BuildGeneric(Type type, string savePath)
    {
        StringBuilder str = new StringBuilder();
        bool hasns = !string.IsNullOrEmpty(type.Namespace);
        str.AppendLine("");
        str.AppendLine(hasns ? $"namespace Binding\n" + "{" : "");
        var className = type.FullName.Replace(".", "");
        str.AppendLine($"{(hasns ? "\t" : "")}public struct {className}GenericBind : ISerialize<List<{type.FullName}>>");
        str.AppendLine($"{(hasns ? "\t{" : "{")}");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public void Write(List<{type.FullName}> value, Segment strem)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}int count = value.Count;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}strem.WriteValue(count);");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if (count == 0) return;");
        var local = type.FullName.Replace(".", "") + "Bind";
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}{local} bind = new {local}();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}foreach (var value1 in value)");
        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}bind.Write(value1, strem);");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");
        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public List<{type.FullName}> Read(Segment strem)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");

        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var count = strem.ReadValue<int>();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var value = new List<{type.FullName}>();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if (count == 0) return value;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}{local} bind = new {local}();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}for (int i = 0; i < count; i++)");
        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.Add(bind.Read(strem));");

        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}return value;");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");
        str.AppendLine($"{(hasns ? "\t}" : "}")}");
        if (hasns) str.AppendLine("}");
        File.AppendAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }
}