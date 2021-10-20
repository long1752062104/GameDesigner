namespace Net
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Reflection;
    using Net.Serialize;

    /// <summary>
    /// 克隆工具类
    /// </summary>
    public sealed class Clone
    {
        /// <summary>
        /// 克隆对象, 脱离引用对象的地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T Instance<T>(object target) where T : class
        {
            var segment = NetConvertBinary.SerializeObject(target, false, true);
            return NetConvertBinary.DeserializeObject<T>(segment, false, true);
        }

        /// <summary>
        /// 贱复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T Copy<T>(object target) where T : class
        {
            var t = typeof(T);
            var obj = (T)Activator.CreateInstance(t);
            var fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                field.SetValue(obj, field.GetValue(target));
            }
            return obj;
        }

        /// <summary>
        /// 深复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(object target) where T : class
        {
            return (T)DeepCopy(typeof(T), target);
        }

        /// <summary>
        /// 深复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static object DeepCopy(object target)
        {
            return DeepCopy(target.GetType(), target);
        }

        /// <summary>
        /// 深复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static object DeepCopy(Type type, object target)
        {
            var obj = Activator.CreateInstance(type);
            var loops = new List<object>();
            loops.Add(target);
            DeepCopy(ref target, ref obj, loops, new List<Type>() { typeof(UnityEngine.Object) });
            return obj;
        }

        private static void DeepCopy(ref object source, ref object value, List<object> loops, List<Type> types) 
        {
            var t = value.GetType();
            var fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.FieldType.IsPointer)
                    continue;
                bool copy = false;
                foreach (var item in types)
                {
                    if (field.FieldType == item | field.FieldType.IsSubclassOf(item))
                    {
                        copy = true;
                        break;
                    }
                }
                var code = Type.GetTypeCode(field.FieldType);
                if (code != TypeCode.Object | copy)
                {
                    field.SetValue(value, field.GetValue(source));
                }
                else if (field.FieldType.IsArray)
                {
                    Type itemType = field.FieldType.GetInterface(typeof(IList<>).FullName).GenericTypeArguments[0];
                    if (itemType.IsPointer)
                        continue;
                    Array list = (Array)field.GetValue(source);
                    if (list == null)
                        continue;
                    Array list1 = Array.CreateInstance(itemType, list.Length);
                    copy = false;
                    foreach (var item in types)
                    {
                        if (itemType == item | itemType.IsSubclassOf(item))
                        {
                            copy = true;
                            break;
                        }
                    }
                    if (Type.GetTypeCode(itemType) != TypeCode.Object | copy)
                    {
                        Array.Copy(list, list1, list.Length);
                    }
                    else 
                    {
                        for (int i = 0; i < list.Length; i++)
                        {
                            var value1 = list.GetValue(i);
                            var value2 = Activator.CreateInstance(itemType);
                            if (value1 == null)
                                continue;
                            DeepCopy(ref value1, ref value2, loops, types);
                            list1.SetValue(value2, i);
                        }
                    }
                    field.SetValue(value, list1);
                }
                else if (field.FieldType.IsGenericType)
                {
                    var arguments = field.FieldType.GenericTypeArguments;
                    if (arguments.Length != 1)
                        continue;
                    Type itemType = arguments[0];
                    if (itemType.IsPointer)
                        continue;
                    IList list = (IList)field.GetValue(source);
                    if (list == null)
                        continue;
                    IList list1 = (IList)Activator.CreateInstance(field.FieldType);
                    copy = false;
                    foreach (var item in types)
                    {
                        if (itemType == item | itemType.IsSubclassOf(item))
                        {
                            copy = true;
                            break;
                        }
                    }
                    if (Type.GetTypeCode(itemType) != TypeCode.Object | copy)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            list1.Add(list[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var value1 = list[i];
                            var value2 = Activator.CreateInstance(itemType);
                            if (value1 == null)
                                continue;
                            DeepCopy(ref value1, ref value2, loops, types);
                            list1.Add(value2);
                        }
                    }
                    field.SetValue(value, list1);
                }
                else if(!field.IsNotSerialized)
                {
                    var value1 = field.GetValue(source);
                    var value2 = field.GetValue(value);
                    if (field.FieldType.IsValueType)
                    {
                        DeepCopy(ref value1, ref value2, loops, types);
                        field.SetValue(value, value1);
                    }
                    else
                    {
                        if (value1 == null | value2 == null | loops.Contains(value2))
                            continue;
                        loops.Add(value2);
                        DeepCopy(ref value1, ref value2, loops, types);
                    }
                }
            }
        }
    }
}
