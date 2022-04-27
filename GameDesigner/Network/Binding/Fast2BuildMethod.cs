﻿using Microsoft.CSharp;
using Net.Event;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

public static class Fast2BuildMethod
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
        internal Type ItemType1;
    }

    /// <summary>
    /// 动态编译, 在unity开发过程中不需要生成绑定cs文件, 直接运行时编译使用, 当编译apk. app时才进行生成绑定cs文件
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public static bool DynamicBuild(params Type[] types)
    {
        return DynamicBuild(0, types);
    }

    /// <summary>
    /// 动态编译, 在unity开发过程中不需要生成绑定cs文件, 直接运行时编译使用, 当编译apk. app时才进行生成绑定cs文件
    /// </summary>
    /// <param name="compilerOptionsIndex">编译参数, 如果编译失败, 可以选择0-7测试哪个编译成功</param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static bool DynamicBuild(int compilerOptionsIndex, params Type[] types)
    {
        List<string> codes = new List<string>();
        foreach (var type in types)
        {
            var str = Build(type, true, true, true, new List<string>());
            str.Append(BuildArray(type, true));
            str.Append(BuildGeneric(type, true));
            codes.Add(str.ToString());
        }
        CSharpCodeProvider provider = new CSharpCodeProvider();
        CompilerParameters param = new CompilerParameters();
        HashSet<string> dllpaths = new HashSet<string>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assemblie in assemblies)
        {
            var path = assemblie.Location;
            var name = Path.GetFileName(path);
            if (name.Contains("Editor"))
                continue;
            dllpaths.Add(path);
        }
        param.ReferencedAssemblies.AddRange(dllpaths.ToArray());
        param.GenerateExecutable = false;
        param.GenerateInMemory = true;
        var options = new string[] { "/langversion:experimental", "/langversion:default", "/langversion:ISO-1", "/langversion:ISO-2", "/langversion:3", "/langversion:4", "/langversion:5", "/langversion:6", "/langversion:7" };
        param.CompilerOptions = options[compilerOptionsIndex];
        CompilerResults cr = provider.CompileAssemblyFromSource(param, codes.ToArray());
        if (cr.Errors.HasErrors)
        {
            NDebug.LogError("编译错误：");
            foreach (CompilerError err in cr.Errors)
                NDebug.LogError(err.ErrorText);
            return false;
        }
        Net.Serialize.NetConvertFast2.Init();
        foreach (var type in types)
            Net.Serialize.NetConvertFast2.AddSerializeType3(type);
        NDebug.Log("编译成功");
        return true;
    }

    public static void Build(Type type, string savePath)
    {
        var str = Build(type, false, true, true, new List<string>());
        var className = type.FullName.Replace(".", "").Replace("+", "");
        File.WriteAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static void Build(Type type, bool addNs, string savePath, bool serField, bool serProperty, List<string> ignores)
    {
        var str = Build(type, addNs, serField, serProperty, ignores, savePath);
        var className = type.FullName.Replace(".", "").Replace("+", "");
        File.WriteAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static StringBuilder Build(Type type, bool addNs, bool serField, bool serProperty, List<string> ignores, string savePath = null)
    {
        StringBuilder str = new StringBuilder();
        bool hasns = !string.IsNullOrEmpty(type.Namespace) | addNs;
        str.AppendLine("using System;");
        str.AppendLine("using System.Collections.Generic;");
        str.AppendLine("using Net.Serialize;");
        str.AppendLine("using Net.System;");
        str.AppendLine("");
        str.AppendLine(hasns ? $"namespace Binding\r\n" + "{" : "");
        var className = type.FullName.Replace(".", "").Replace("+", "");
        str.AppendLine($"{(hasns ? "\t" : "")}public struct {className}Bind : ISerialize<{type.FullName.Replace("+", ".")}>, ISerialize");
        str.AppendLine($"{(hasns ? "\t{" : "{")}");
        FieldInfo[] fields;
        if (serField)
            fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        else
            fields = new FieldInfo[0];
        PropertyInfo[] properties;
        if (serProperty)
            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        else
            properties = new PropertyInfo[0];
        List<Member> members = new List<Member>();
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            if (ignores.Contains(field.Name))
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
                var serType = field.FieldType.GetInterface(typeof(IList<>).FullName);
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
                Type itemType = field.FieldType.GenericTypeArguments[0]; 
                Type itemType1 = field.FieldType.GenericTypeArguments[1];
                member.ItemType = itemType;
                member.ItemType1 = itemType1;
                //throw new Exception("尚未支持字典类型!");
            }
            members.Add(member);
        }
        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            if (!property.CanRead | !property.CanWrite)
                continue;
            if (property.GetIndexParameters().Length > 0)
                continue;
            if (ignores.Contains(property.Name))
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
                var serType = property.PropertyType.GetInterface(typeof(IList<>).FullName);
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
                Type itemType = property.PropertyType.GenericTypeArguments[0]; 
                Type itemType1 = property.PropertyType.GenericTypeArguments[1];
                member.ItemType = itemType;
                member.ItemType1 = itemType1;
                //throw new Exception("尚未支持字典类型!");
            }
            members.Add(member);
        }
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public void Write({type.FullName.Replace("+", ".")} value, Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}int pos = stream.Position;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}stream.Position += {((members.Count - 1) / 8) + 1};");
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
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}stream.Write(value.{members[i].Name});");
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
            }
            else if (members[i].IsArray)
            {
                typecode = Type.GetTypeCode(members[i].ItemType);
                if (typecode != TypeCode.Object)
                {
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != null)");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}NetConvertBase.SetBit(ref bits[{bitPos}], {++bitInx1}, true);");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}stream.Write(value.{members[i].Name});");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                }
                else
                {
                    if (members[i].Type.IsValueType)
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != default)");
                    else
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != null)");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}NetConvertBase.SetBit(ref bits[{bitPos}], {++bitInx1}, true);");
                    var local = members[i].ItemType.FullName.Replace(".", "").Replace("+", "") + "ArrayBind";
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}{local} bind = new {local}();");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}bind.Write(value.{members[i].Name}, stream);");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                }
            }
            else if (members[i].IsGenericType)
            {
                if (members[i].ItemType1 == null)//List<T>
                {
                    typecode = Type.GetTypeCode(members[i].ItemType);
                    if (typecode != TypeCode.Object)
                    {
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != null)");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}NetConvertBase.SetBit(ref bits[{bitPos}], {++bitInx1}, true);");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}stream.Write(value.{members[i].Name});");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                    }
                    else
                    {
                        if (members[i].Type.IsValueType)
                            str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != default)");
                        else
                            str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != null)");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                        var local = members[i].ItemType.FullName.Replace(".", "").Replace("+", "") + "GenericBind";
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}NetConvertBase.SetBit(ref bits[{bitPos}], {++bitInx1}, true);");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}var bind = new {local}();");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}bind.Write(value.{members[i].Name}, stream);");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                    }
                }
                else //Dic
                {
                    typecode = Type.GetTypeCode(members[i].ItemType);
                    if (typecode != TypeCode.Object)
                    {
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != null)");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                        string bind;
                        if(members[i].ItemType1.IsArray)
                            bind = members[i].ItemType1.FullName.Replace(".", "").Replace("+", "") + "ArrayBind";
                        else if(members[i].ItemType1.IsGenericType)
                            bind = members[i].ItemType1.FullName.Replace(".", "").Replace("+", "") + "GenericBind";
                        else
                            bind = members[i].ItemType1.FullName.Replace(".", "").Replace("+", "") + "Bind";
                        var local = $"DictionaryBind<{members[i].ItemType.FullName}, {members[i].ItemType1.FullName}>";
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}NetConvertBase.SetBit(ref bits[{bitPos}], {++bitInx1}, true);");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}var bind = new {local}();");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}bind.Write(value.{members[i].Name}, stream, new {bind}());");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");

                        var text = BuildDictionary(members[i].Type);
                        var className1 = $"Dictionary_{members[i].ItemType.FullName.Replace(".","").Replace("+","")}_{members[i].ItemType1.FullName.Replace(".", "").Replace("+", "")}_Bind";
                        File.WriteAllText(savePath + $"//{className1}.cs", text);
                    }
                    else 
                    {
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != null)");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                        var local = $"Dictionary_{members[i].ItemType.Name}_{members[i].ItemType1.Name.Replace("`", "")}__Bind";
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}NetConvertBase.SetBit(ref bits[{bitPos}], {++bitInx1}, true);");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}var bind = new {local}();//请定义这个字典结构类来实现字典序列化");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}bind.Write(value.{members[i].Name}, stream);");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                    }
                }
            }
            else
            {
                if (members[i].Type.IsValueType)
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != default)");
                else
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(value.{members[i].Name} != null)");
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}NetConvertBase.SetBit(ref bits[{bitPos}], {++bitInx1}, true);");
                var local = members[i].Type.FullName.Replace(".", "").Replace("+", "") + "Bind";
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}var bind = new {local}();");
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}bind.Write(value.{members[i].Name}, stream);");
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
            }
        }
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}int pos1 = stream.Position;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}stream.Position = pos;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}stream.Write(bits, 0, {((members.Count - 1) / 8) + 1});");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}stream.Position = pos1;");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");
        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public {type.FullName.Replace("+", ".")} Read(Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");

        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}byte[] bits = stream.Read({((members.Count - 1) / 8) + 1});");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var value = new {type.FullName.Replace("+", ".")}();");
        for (int i = 0; i < members.Count; i++)
        {
            int bitInx1 = i % 8;
            int bitPos = i / 8;
            var typecode = Type.GetTypeCode(members[i].Type);
            if (typecode != TypeCode.Object)
            {
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                if (members[i].IsEnum)
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = stream.ReadValue<{members[i].Type.FullName}>();");
                else
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = stream.ReadValue<{members[i].Type.Name}>();");
            }
            else if (members[i].IsArray)
            {
                typecode = Type.GetTypeCode(members[i].ItemType);
                if (typecode != TypeCode.Object)
                {
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = stream.ReadArray<{members[i].ItemType.FullName}>();");
                }
                else
                {
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                    var local = members[i].ItemType.FullName.Replace(".", "").Replace("+", "") + "ArrayBind";
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}var bind = new {local}();");
                    str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = bind.Read(stream);");
                    str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                }
            }
            else if (members[i].IsGenericType)
            {
                if (members[i].ItemType1 == null) //List<T>
                {
                    typecode = Type.GetTypeCode(members[i].ItemType);
                    if (typecode != TypeCode.Object)
                    {
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = stream.ReadList<{members[i].ItemType.FullName}>();");
                    }
                    else
                    {
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                        var local = members[i].ItemType.FullName.Replace(".", "").Replace("+", "") + "GenericBind";
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}var bind = new {local}();");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = bind.Read(stream);");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                    }
                }
                else // Dic
                {
                    typecode = Type.GetTypeCode(members[i].ItemType);
                    if (typecode != TypeCode.Object)
                    {
                        string bind;
                        if (members[i].ItemType1.IsArray)
                            bind = members[i].ItemType1.FullName.Replace(".", "").Replace("+", "") + "ArrayBind";
                        else if (members[i].ItemType1.IsGenericType)
                            bind = members[i].ItemType1.FullName.Replace(".", "").Replace("+", "") + "GenericBind";
                        else
                            bind = members[i].ItemType1.FullName.Replace(".", "").Replace("+", "") + "Bind";
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                        var local = $"DictionaryBind<{members[i].ItemType.FullName}, {members[i].ItemType1.FullName}>";
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}var bind = new {local}();");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = bind.Read(stream, new {bind}());");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                    }
                    else
                    {
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                        var local = $"Dictionary_{members[i].ItemType.Name}_{members[i].ItemType1.Name.Replace("`", "")}__Bind";
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}var bind = new {local}();//请定义这个字典结构类来实现字典反序列化");
                        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = bind.Read(stream);");
                        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
                    }
                }
            }
            else
            {
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if(NetConvertBase.GetBit(bits[{bitPos}], {++bitInx1}))");
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "{");
                var local = members[i].Type.FullName.Replace(".", "").Replace("+", "") + "Bind";
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}var bind = new {local}();");
                str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.{members[i].Name} = bind.Read(stream);");
                str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}" + "}");
            }
        }
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}return value;");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");

        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public void WriteValue(object value, Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}Write(({type.FullName.Replace("+", ".")})value, stream);");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");

        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public object ReadValue(Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}return Read(stream);");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");

        str.AppendLine($"{(hasns ? "\t}" : "}")}");
        if (hasns) str.AppendLine("}");
        return str;
    }

    public static void BuildBindingType(List<Type> types, string savePath)
    {
        StringBuilder str = new StringBuilder();
        str.AppendLine("using System;");
        str.AppendLine("using System.Collections.Generic;");
        str.AppendLine("namespace Binding");
        str.AppendLine("{");
        str.AppendLine("    public static class BindingType");
        str.AppendLine("    {");
        str.AppendLine("        public static readonly Type[] TYPES = new Type[]");
        str.AppendLine("        {");
        foreach (var item in types)
        {
            if(item.IsGenericType & item.GenericTypeArguments.Length == 2)
                str.AppendLine($"\t\t\ttypeof(Dictionary<{item.GenericTypeArguments[0].FullName},{item.GenericTypeArguments[1].FullName}>),");
            else
                str.AppendLine($"\t\t\ttypeof({item.FullName}),");
        }
        str.AppendLine("        };");
        str.AppendLine("    }");
        str.AppendLine("}");
        File.WriteAllText(savePath + $"//BindingType.cs", str.ToString());
    }

    public static void BuildArray(Type type, bool addNs, string savePath)
    {
        var str = BuildArray(type, addNs);
        var className = type.FullName.Replace(".", "").Replace("+", "");
        File.AppendAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static StringBuilder BuildArray(Type type, bool addNs)
    {
        StringBuilder str = new StringBuilder();
        bool hasns = !string.IsNullOrEmpty(type.Namespace) | addNs;
        str.AppendLine("");
        str.AppendLine(hasns ? $"namespace Binding\r\n" + "{" : "");
        var className = type.FullName.Replace(".", "").Replace("+", "");
        str.AppendLine($"{(hasns ? "\t" : "")}public struct {className}ArrayBind : ISerialize<{type.FullName.Replace("+", ".")}[]>, ISerialize");
        str.AppendLine($"{(hasns ? "\t{" : "{")}");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public void Write({type.FullName.Replace("+", ".")}[] value, Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}int count = value.Length;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}stream.Write(count);");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if (count == 0) return;");
        var local = type.FullName.Replace(".", "").Replace("+", "") + "Bind";
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var bind = new {local}();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}foreach (var value1 in value)");
        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}bind.Write(value1, stream);");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");
        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public {type.FullName.Replace("+", ".")}[] Read(Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");

        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var count = stream.ReadInt32();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var value = new {type.FullName.Replace("+", ".")}[count];");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if (count == 0) return value;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var bind = new {local}();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}for (int i = 0; i < count; i++)");
        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value[i] = bind.Read(stream);");

        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}return value;");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");

        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public void WriteValue(object value, Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}Write(({type.FullName.Replace("+", ".")}[])value, stream);");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");

        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public object ReadValue(Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}return Read(stream);");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");

        str.AppendLine($"{(hasns ? "\t}" : "}")}");
        if (hasns) str.AppendLine("}");
        return str;
    }

    public static void BuildGeneric(Type type, bool addNs, string savePath)
    {
        var str = BuildGeneric(type, addNs);
        var className = type.FullName.Replace(".", "").Replace("+", "");
        File.AppendAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static StringBuilder BuildGeneric(Type type, bool addNs)
    {
        StringBuilder str = new StringBuilder();
        bool hasns = !string.IsNullOrEmpty(type.Namespace) | addNs;
        str.AppendLine("");
        str.AppendLine(hasns ? $"namespace Binding\r\n" + "{" : "");
        var className = type.FullName.Replace(".", "").Replace("+", "");
        str.AppendLine($"{(hasns ? "\t" : "")}public struct {className}GenericBind : ISerialize<List<{type.FullName.Replace("+", ".")}>>, ISerialize");
        str.AppendLine($"{(hasns ? "\t{" : "{")}");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public void Write(List<{type.FullName.Replace("+", ".")}> value, Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}int count = value.Count;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}stream.Write(count);");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if (count == 0) return;");
        var local = type.FullName.Replace(".", "").Replace("+", "") + "Bind";
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var bind = new {local}();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}foreach (var value1 in value)");
        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}bind.Write(value1, stream);");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");
        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public List<{type.FullName.Replace("+", ".")}> Read(Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");

        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var count = stream.ReadInt32();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var value = new List<{type.FullName.Replace("+", ".")}>();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}if (count == 0) return value;");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}var bind = new {local}();");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}for (int i = 0; i < count; i++)");
        str.AppendLine($"{(hasns ? "\t\t\t\t" : "\t\t\t")}value.Add(bind.Read(stream));");

        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}return value;");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");

        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public void WriteValue(object value, Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}Write((List<{type.FullName.Replace("+", ".")}>)value, stream);");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");

        str.AppendLine($"");
        str.AppendLine($"{(hasns ? "\t\t" : "\t")}public object ReadValue(Segment stream)");
        str.AppendLine($"{(hasns ? "\t\t{" : "\t{")}");
        str.AppendLine($"{(hasns ? "\t\t\t" : "\t\t")}return Read(stream);");
        str.AppendLine($"{(hasns ? "\t\t}" : "\t}")}");

        str.AppendLine($"{(hasns ? "\t}" : "}")}");
        if (hasns) str.AppendLine("}");
        return str;
    }

    public static string BuildDictionary(Type type)
    {
        var text =
@"using Binding;
using Net.Serialize;
using Net.System;
using System.Collections.Generic;
public struct Dictionary_{TKeyName}_{TValueName}_Bind : ISerialize<Dictionary<{TKey}, {TValue}>>, ISerialize
{
    public void Write(Dictionary<{TKey}, {TValue}> value, Segment stream)
    {
        int count = value.Count;
        stream.Write(count);
        if (count == 0) return;
        foreach (var value1 in value)
        {
            stream.Write(value1.Key);
            var bind = new {BindTypeName}();
            bind.Write(value1.Value, stream);
        }
    }

    public Dictionary<{TKey}, {TValue}> Read(Segment stream)
    {
        var count = stream.ReadInt32();
        var value = new Dictionary<{TKey}, {TValue}>();
        if (count == 0) return value;
        for (int i = 0; i < count; i++)
        {
            var key = stream.ReadValue<{TKey}>();
            var bind = new {BindTypeName}();
            var value1 = bind.Read(stream);
            value.Add(key, value1);
        }
        return value;
    }

    public void WriteValue(object value, Segment stream)
    {
        Write((Dictionary<{TKey}, {TValue}>)value, stream);
    }

    public object ReadValue(Segment stream)
    {
        return Read(stream);
    }
}";
        var args = type.GenericTypeArguments;
        text = text.Replace("{TKeyName}", $"{args[0].FullName.Replace(".", "").Replace("+", "")}");
        text = text.Replace("{TValueName}", $"{args[1].FullName.Replace(".", "").Replace("+", "")}");
        text = text.Replace("{TKey}", $"{args[0].FullName}");
        text = text.Replace("{TValue}", $"{args[1].FullName}");
        text = text.Replace("{BindTypeName}", $"{args[1].FullName.Replace(".", "").Replace("+", "")}Bind");
        return text;
    }
}