#if !CLOSE_ILR
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Reflection;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Net.Share
{
    public static class ObjectExtensions
    {
        public static Dictionary<string, Type> Types = new Dictionary<string, Type>();

        public static Type GetType(this object self)
        {
            if (self is ILTypeInstance obj)
                return obj.Type.ReflectionType;
            return self.GetType();
        }

        public static Type GetType(this Type self)
        {
            string typeName = self.FullName;
            if (Types.ContainsKey(typeName))
                return Types[typeName];
            Type type1 = Type.GetType(typeName);
            if (type1 != null)
            {
                Types.Add(typeName, type1);
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
                    if (typeName.EndsWith("[]"))
                    {
                        type = Array.CreateInstance(type, 0).GetType();
                        Types.Add(typeName, type);
                        return type;
                    }
                    if (typeName.Contains("System.Collections.Generic.List`1"))
                    {
                        Type type2 = Type.GetType("System.Collections.Generic.List`1"); //得到此类类型 注：（`1） 为占位符 不明确类型
                        type = type2.MakeGenericType(type); //指定泛型类
                        Types.Add(typeName, type);
                        return type;
                    }
                    Types.Add(typeName, type);
                    return type;
                }
            }
            ILRuntimeType type3 = self as ILRuntimeType;
            if (type3 != null)
            {
                Types.Add(typeName, type3);
                return type3;
            }
            Event.NDebug.LogError($"找不到类型:{typeName}, 类型太复杂时需要使用 NetConvertOld.AddSerializeType(type) 添加类型后再进行系列化!");
            return null;
        }

        public static Type GetType(string typeName)
        {
            if (ILRuntime.Runtime.Enviorment.AppDomain.Instance == null)
                return null;
            IType type = ILRuntime.Runtime.Enviorment.AppDomain.Instance.GetType(typeName);
            if (type != null)
                return type.ReflectionType;
            return null;
        }

        public static object CreateInstance(Type constructType)
        {
            ILRuntimeType type = constructType as ILRuntimeType;
            if (type != null)
                return type.appdomain.Instantiate(constructType.FullName);
            return Activator.CreateInstance(constructType);
        }
    }
}
#endif