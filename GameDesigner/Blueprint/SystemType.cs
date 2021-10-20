using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace GameDesigner
{
    public enum SetValueModel
    {
        Add, Cut, Null
    }

    /// <summary>
    /// 插件系统类管理
    /// </summary>
    public static class SystemType
    {
        private static ConcurrentDictionary<string, Type> Types = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// 解释 : 获得应用程序当前已加载的所有程序集中查找typeName的类型
        /// </summary>
        public static Type GetType(string typeName)
        {
            //代码优化
            if (Types.ContainsKey(typeName))
                return Types[typeName];
            Type type1 = Type.GetType(typeName);
            if (type1 != null)
            {
                Types.TryAdd(typeName, type1);
                return type1;
            }
            string typeName1 = typeName;
            typeName1 = typeName1.Replace("&", ""); // 反射参数的 out 标示
            typeName1 = typeName1.Replace("*", ""); // 反射参数的 int*(指针) 标示
            typeName1 = typeName1.Replace("[]", ""); // 反射参数的 object[](数组) 标示
            if (typeName1.Contains("["))
            {
                string[] typeNames = typeName1.Split('['); //泛型类型
                if (typeNames.Length > 0)
                    typeName1 = typeNames[1];
            }
            typeName1 = typeName1.Replace("]", ""); // 反射参数的 object[](数组) 标示
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeName1);
                if (type != null)
                {
                    if (typeName.Contains("[]"))
                    {
                        var arrayType = Array.CreateInstance(type, 0).GetType();
                        Types.TryAdd(typeName, arrayType);
                        return arrayType;
                    }
                    else if (typeName.Contains("List`1"))
                    {
                        var arrayType = Activator.CreateInstance(type).GetType();
                        Types.TryAdd(typeName, arrayType);
                        return arrayType;
                    }
                    else 
                    {
                        Types.TryAdd(typeName, type);
                        return type;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取所有在unity所引用的dll文件的类型，并通过types委托调用，types不是一次获取完所有dll文件的类型，而是一个dll文件获取完（所有类型）就执行一次委托
        /// </summary>
        /// <param name="types">单个dll文件的所有类型委托到types的参数中</param>
        public static void GetTypes(Action<Type[]> types)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                types(assembly.GetTypes());
            }
        }

        /// <summary>
        /// 解释 : 判断type的基类是否是Typeof类型,是返回真,不是返回假
        /// </summary>
        static public bool IsSubclassOf(Type type, Type Typeof)
        {
            if (type == null | Typeof == null)
                return false;
            if (type.IsSubclassOf(Typeof) | type == Typeof)
                return true;
            return false;
        }

        /// <summary>
        /// 解释 : 判断type所继承的类(基类)是否是Typeof类 和 Typeof所继承的类型(基类)是否是type类 , 是返回真,不是返回假
        /// </summary>

        static public bool IsSubclassOfs(Type type, Type Typeof)
        {
            if (type == null | Typeof == null)
                return false;
            if (type.IsSubclassOf(Typeof) | type == Typeof)
                return true;
            if (Typeof.IsSubclassOf(type) | type == Typeof)
                return true;
            return false;
        }

        /// <summary>
        /// 解释 : 判断type的基类是否是Typeof类型,是返回真,不是返回假
        /// </summary>

        static public bool IsSubclassOf<T>(Type type) where T : class
        {
            if (type == null)
                return false;
            if (type.IsSubclassOf(typeof(T)) | type == typeof(T))
                return true;
            return false;
        }

        /// <summary>
        /// 在监视面板显示类的值并且可视化修改 ( type 给定类型名称 , value 转换这个字符串为type类型的值 )
        /// </summary>

        static public object StringToValue(string type = "System.Int32", string value = "0")
        {
            switch (type)
            {
                case "System.Int32":
                    return Convert.ToInt32(value);

                case "System.Single":
                    return Convert.ToSingle(value);

                case "System.String":
                    return Convert.ToString(value);

                case "System.Boolean":
                    return Convert.ToBoolean(value);

                case "System.Char":
                    return Convert.ToChar(value);

                case "System.Int16":
                    return Convert.ToInt16(value);

                case "System.Int64":
                    return Convert.ToInt64(value);

                case "System.UInt16":
                    return Convert.ToUInt16(value);

                case "System.UInt32":
                    return Convert.ToUInt32(value);

                case "System.UInt64":
                    return Convert.ToUInt64(value);

                case "System.Double":
                    return Convert.ToDouble(value);

                case "System.Byte":
                    return Convert.ToByte(value);

                case "System.SByte":
                    return Convert.ToSByte(value);

                case "UnityEngine.Vector2":
                    return ConvertUtility.ToVector2_3_4(type, value);

                case "UnityEngine.Vector3":
                    return ConvertUtility.ToVector2_3_4(type, value);

                case "UnityEngine.Vector4":
                    return ConvertUtility.ToVector2_3_4(type, value);

                case "UnityEngine.Rect":
                    return ConvertUtility.ToRect(type, value);

                case "UnityEngine.Color":
                    return ConvertUtility.ToColor(type, value);

                case "UnityEngine.Quaternion":
                    return ConvertUtility.ToQuaternion(type, value);

                case "System.Void":
                    return null; ;
            }

            //Object判断
            return GetType(type);
        }

        /// <summary>
        /// 克隆对象, 脱离引用对象的地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        static public T Instance<T>(object target) where T : class
        {
            T t = Activator.CreateInstance<T>();
            string str = Net.Serialize.NetConvertOld.Serialize(target);
            object obj = Net.Serialize.NetConvertOld.Deserialize(str).pars[0];
            foreach (FieldInfo field in target.GetType().GetFields())
            {
                if (field.IsStatic | field.IsPrivate | field.FieldType.IsAbstract)
                    continue;
                if (IsSubclassOf(field.FieldType, typeof(UnityEngine.Object)) | field.FieldType == typeof(string) | field.FieldType.IsValueType | field.FieldType.IsEnum | field.FieldType.IsArray)
                {
                    field.SetValue(t, field.GetValue(obj));
                }
                else
                {
                    try { field.SetValue(t, field.GetValue(obj)); } catch { }
                }
            }
            return t;
        }

        /// <summary>
        /// 设置类的变量值,解决派生类的值控制父类的变量值 ( 被赋值变量对象 , 赋值变量对象 ) [尽可能的使用此方法,此方法产生GC]
        /// </summary>
        static public void SetFieldValue(object target, object setValue)
        {
            foreach (FieldInfo field in target.GetType().GetFields())
            {
                if (field.IsStatic | field.IsPrivate | field.FieldType.IsAbstract)
                    continue;
                if (IsSubclassOf(field.FieldType, typeof(UnityEngine.Object)) | field.FieldType == typeof(string) | field.FieldType.IsValueType | field.FieldType.IsEnum | field.FieldType.IsArray)
                {
                    field.SetValue(target, field.GetValue(setValue));
                }
                else if (!field.IsNotSerialized)
                {
                    try { For(field.GetValue(target), field.GetValue(setValue)); } catch { }
                }
            }
        }

        static void For(object target, object setValue)
        {
            if (target == null)
                return;
            foreach (FieldInfo field in target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.IsStatic | field.IsPrivate | field.FieldType.IsAbstract)
                    continue;
                if (IsSubclassOf(field.FieldType, typeof(UnityEngine.Object)) | field.FieldType == typeof(string) | field.FieldType.IsValueType | field.FieldType.IsEnum | field.FieldType.IsArray)
                {
                    field.SetValue(target, field.GetValue(setValue));
                }
                else if (!field.IsNotSerialized)
                {
                    try { For(field.GetValue(target), field.GetValue(setValue)); } catch { }
                }
            }
        }

        /// <summary>
        /// 设置类的变量值,解决派生类的值控制父类的变量值 ( 被赋值变量对象 , 赋值变量对象 ) [尽可能的使用此方法,此方法产生GC]
        /// </summary>

        static public void SetPropertyValue(object target, object setValue)
        {
            foreach (PropertyInfo property in target.GetType().GetProperties())
            {
                if (!property.CanWrite)
                    continue;
                if (IsSubclassOf(property.PropertyType, typeof(UnityEngine.Object)) | property.PropertyType == typeof(string) | property.PropertyType == typeof(object) | property.PropertyType.IsValueType | property.PropertyType.IsEnum)
                {
                    property.SetValue(target, property.GetValue(setValue, null), null);
                }
                else
                {
                    PropertyFor(property.GetValue(target, null), property.GetValue(setValue, null));
                }
            }
        }

        static void PropertyFor(object target, object setValue)
        {
            if (target == null)
                return;

            foreach (PropertyInfo property in target.GetType().GetProperties())
            {
                if (!property.CanWrite)
                    continue;
                if (IsSubclassOf(property.PropertyType, typeof(UnityEngine.Object)) | property.PropertyType == typeof(string) | property.PropertyType == typeof(object) | property.PropertyType.IsValueType | property.PropertyType.IsEnum)
                {
                    property.SetValue(target, property.GetValue(setValue, null), null);
                }
                else
                {
                    PropertyFor(property.GetValue(target, null), property.GetValue(setValue, null));
                }
            }
        }

        /// <summary>
        /// 设置类的变量值,解决派生类的值控制父类的变量值 ( 被赋值变量对象 , 赋值变量对象 , 不赋值的变量名数组 ) [尽可能的使用此方法,此方法产生GC]
        /// </summary>

        static public void SetFieldValue(object target, object setValue, string[] notSetValueNames, SetValueModel model = SetValueModel.Null)
        {
            foreach (FieldInfo field in target.GetType().GetFields())
            {
                if (field.IsStatic)
                    continue;

                bool isSetValue = true;
                foreach (string name in notSetValueNames)
                {
                    if (field.Name == name)
                    {
                        isSetValue = false;
                        break;
                    }
                }
                if (isSetValue)
                {
                    switch (model)
                    {
                        case SetValueModel.Add:
                            {
                                if (SystemType.IsSubclassOf(field.FieldType, typeof(UnityEngine.Object)) | field.FieldType == typeof(string) | field.FieldType.IsValueType | field.FieldType.IsEnum)
                                {
                                    switch (field.FieldType.FullName)
                                    {
                                        case "System.Int32":
                                            {
                                                field.SetValue(target, (int)field.GetValue(target) + (int)field.GetValue(setValue));
                                                break;
                                            }
                                        case "System.Single":
                                            {
                                                field.SetValue(target, (float)field.GetValue(target) + (float)field.GetValue(setValue));
                                                break;
                                            }
                                        default:
                                            {
                                                field.SetValue(target, field.GetValue(setValue));
                                                break;
                                            }
                                    }
                                }
                                else if (!field.IsNotSerialized)
                                {
                                    For(field.GetValue(target), field.GetValue(setValue), true);
                                }
                                break;
                            }
                        case SetValueModel.Cut:
                            {
                                if (SystemType.IsSubclassOf(field.FieldType, typeof(UnityEngine.Object)) | field.FieldType == typeof(string) | field.FieldType.IsValueType | field.FieldType.IsEnum)
                                {
                                    switch (field.FieldType.FullName)
                                    {
                                        case "System.Int32":
                                            {
                                                field.SetValue(target, (int)field.GetValue(target) - (int)field.GetValue(setValue));
                                                break;
                                            }
                                        case "System.Single":
                                            {
                                                field.SetValue(target, (float)field.GetValue(target) - (float)field.GetValue(setValue));
                                                break;
                                            }
                                        default:
                                            {
                                                field.SetValue(target, field.GetValue(setValue));
                                                break;
                                            }
                                    }
                                }
                                else if (!field.IsNotSerialized)
                                {
                                    For(field.GetValue(target), field.GetValue(setValue), false);
                                }
                                break;
                            }
                        case SetValueModel.Null:
                            {
                                if (SystemType.IsSubclassOf(field.FieldType, typeof(UnityEngine.Object)) | field.FieldType == typeof(string) | field.FieldType.IsValueType | field.FieldType.IsEnum)
                                {
                                    field.SetValue(target, field.GetValue(setValue));
                                }
                                else if (!field.IsNotSerialized)
                                {
                                    For(field.GetValue(target), field.GetValue(setValue));
                                }
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// 设置类的变量值,解决派生类的值控制父类的变量值 ( 被赋值变量对象 , 赋值变量对象 ) [尽可能的使用此方法,此方法产生GC]
        /// </summary>

        static public void SetFieldValue(object target, object setValue, bool AddOrCut)
        {
            foreach (FieldInfo field in target.GetType().GetFields())
            {
                if (field.IsStatic)
                    continue;
                if (SystemType.IsSubclassOf(field.FieldType, typeof(UnityEngine.Object)) | field.FieldType == typeof(string) | field.FieldType.IsValueType | field.FieldType.IsEnum)
                {
                    switch (field.FieldType.FullName)
                    {
                        case "System.Int32":
                            {
                                if (AddOrCut)
                                {
                                    field.SetValue(target, (int)field.GetValue(target) + (int)field.GetValue(setValue));
                                }
                                else
                                {
                                    field.SetValue(target, (int)field.GetValue(target) - (int)field.GetValue(setValue));
                                }
                                break;
                            }
                        case "System.Single":
                            {
                                if (AddOrCut)
                                {
                                    field.SetValue(target, (float)field.GetValue(target) + (float)field.GetValue(setValue));
                                }
                                else
                                {
                                    field.SetValue(target, (float)field.GetValue(target) - (float)field.GetValue(setValue));
                                }
                                break;
                            }
                        default:
                            {
                                field.SetValue(target, field.GetValue(setValue));
                                break;
                            }
                    }
                }
                else if (!field.IsNotSerialized)
                {
                    For(field.GetValue(target), field.GetValue(setValue), AddOrCut);
                }
            }
        }

        static void For(object target, object setValue, bool AddOrCut)
        {
            if (target == null)
                return;

            foreach (FieldInfo field in target.GetType().GetFields())
            {
                if (field.IsStatic)
                    continue;
                if (SystemType.IsSubclassOf(field.FieldType, typeof(UnityEngine.Object)) | field.FieldType == typeof(string) | field.FieldType.IsValueType | field.FieldType.IsEnum)
                {
                    switch (field.FieldType.FullName)
                    {
                        case "System.Int32":
                            {
                                if (AddOrCut)
                                {
                                    field.SetValue(target, (int)field.GetValue(target) + (int)field.GetValue(setValue));
                                }
                                else
                                {
                                    field.SetValue(target, (int)field.GetValue(target) - (int)field.GetValue(setValue));
                                }
                                break;
                            }
                        case "System.Single":
                            {
                                if (AddOrCut)
                                {
                                    field.SetValue(target, (float)field.GetValue(target) + (float)field.GetValue(setValue));
                                }
                                else
                                {
                                    field.SetValue(target, (float)field.GetValue(target) - (float)field.GetValue(setValue));
                                }
                                break;
                            }
                        default:
                            {
                                field.SetValue(target, field.GetValue(setValue));
                                break;
                            }
                    }
                }
                else if (!field.IsNotSerialized)
                {
                    For(field.GetValue(target), field.GetValue(setValue), AddOrCut);
                }
            }
        }
    }
}