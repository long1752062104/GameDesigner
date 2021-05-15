using ILRuntime.Runtime.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILRuntime.Runtime
{
    public static class Extensions
    {
        public static void GetClassName(this Type type, out string clsName, out string realClsName, out bool isByRef, bool simpleClassName = false)
        {
            isByRef = type.IsByRef;
            int arrayRank = 1;

            if (isByRef)
            {
                type = type.GetElementType();
            }

            bool isArray = type.IsArray;

            if (isArray)
            {
                arrayRank = type.GetArrayRank();
                type = type.GetElementType();
                if (type.IsArray)
                {
                    type.GetClassName(out clsName, out realClsName, out isByRef, simpleClassName);

                    clsName += "_Array";
                    if (!simpleClassName)
                        clsName += "_Binding";
                    if (arrayRank > 1)
                        clsName += arrayRank;
                    if (arrayRank <= 1)
                        realClsName += "[]";
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(realClsName);
                        sb.Append('[');
                        for (int i = 0; i < arrayRank - 1; i++)
                        {
                            sb.Append(',');
                        }
                        sb.Append(']');
                        realClsName = sb.ToString();
                    }

                    return;
                }
            }
            string realNamespace = null;
            bool isNestedGeneric = false;
            if (type.IsNested)
            {
                Type rt = type.ReflectedType;
                if (rt.IsGenericType && rt.IsGenericTypeDefinition)
                {
                    if (type.IsGenericType)
                    {
                        rt = rt.MakeGenericType(type.GetGenericArguments());
                        isNestedGeneric = true;
                    }
                }
                GetClassName(rt, out string bClsName, out string bRealClsName, out bool tmp);
                clsName = bClsName + "_";
                realNamespace = bRealClsName + ".";
            }
            else
            {
                clsName = simpleClassName ? "" : (!string.IsNullOrEmpty(type.Namespace) ? type.Namespace.Replace(".", "_") + "_" : "");

                if (string.IsNullOrEmpty(type.Namespace))
                {
                    if (type.IsArray)
                    {
                        Type elementType = type.GetElementType();
                        if (elementType.IsNested && elementType.DeclaringType != null)
                        {
                            realNamespace = elementType.Namespace + "." + elementType.DeclaringType.Name + ".";
                        }
                        else
                        {
                            realNamespace = elementType.Namespace + ".";
                        }
                    }
                    else
                    {
                        realNamespace = "global::";
                    }
                }
                else
                {
                    realNamespace = type.Namespace + ".";
                }
            }
            clsName = clsName + type.Name.Replace(".", "_").Replace("`", "_").Replace("<", "_").Replace(">", "_");
            bool isGeneric = false;
            string ga = null;
            if (type.IsGenericType && !isNestedGeneric)
            {
                isGeneric = true;
                clsName += "_";
                ga = "<";
                Type[] args = type.GetGenericArguments();
                bool first = true;
                foreach (Type j in args)
                {
                    if (first)
                        first = false;
                    else
                    {
                        clsName += "_";
                        ga += ", ";
                    }
                    GetClassName(j, out string a, out string b, out bool tmp, true);
                    clsName += a;
                    ga += b;
                }
                ga += ">";
            }
            if (isArray)
            {
                clsName += "_Array";
                if (arrayRank > 1)
                    clsName += arrayRank;
            }
            if (!simpleClassName)
                clsName += "_Binding";

            realClsName = realNamespace;
            if (isGeneric)
            {
                int idx = type.Name.IndexOf("`");
                if (idx > 0)
                {
                    realClsName += type.Name.Substring(0, idx);
                    realClsName += ga;
                }
                else
                    realClsName += type.Name;
            }
            else
                realClsName += type.Name;

            if (isArray)
            {
                if (arrayRank <= 1)
                    realClsName += "[]";
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(realClsName);
                    sb.Append('[');
                    for (int i = 0; i < arrayRank - 1; i++)
                    {
                        sb.Append(',');
                    }
                    sb.Append(']');
                    realClsName = sb.ToString();
                }
            }

        }
        public static int ToInt32(this object obj)
        {
            if (obj is int)
                return (int)obj;
            if (obj is float)
                return (int)(float)obj;
            if (obj is long)
                return (int)(long)obj;
            if (obj is short)
                return (short)obj;
            if (obj is double)
                return (int)(double)obj;
            if (obj is byte)
                return (byte)obj;
            if (obj is Intepreter.ILEnumTypeInstance)
                return (int)((Intepreter.ILEnumTypeInstance)obj)[0];
            if (obj is uint)
                return (int)(uint)obj;
            if (obj is ushort)
                return (ushort)obj;
            if (obj is sbyte)
                return (sbyte)obj;
            return Convert.ToInt32(obj);
        }
        public static long ToInt64(this object obj)
        {
            if (obj is long)
                return (long)obj;
            if (obj is int)
                return (int)obj;
            if (obj is float)
                return (long)(float)obj;
            if (obj is short)
                return (short)obj;
            if (obj is double)
                return (long)(double)obj;
            if (obj is byte)
                return (byte)obj;
            if (obj is uint)
                return (uint)obj;
            if (obj is ushort)
                return (ushort)obj;
            if (obj is sbyte)
                return (sbyte)obj;
            throw new InvalidCastException();
        }
        public static short ToInt16(this object obj)
        {
            if (obj is short)
                return (short)obj;
            if (obj is long)
                return (short)(long)obj;
            if (obj is int)
                return (short)(int)obj;
            if (obj is float)
                return (short)(float)obj;
            if (obj is double)
                return (short)(double)obj;
            if (obj is byte)
                return (byte)obj;
            if (obj is uint)
                return (short)(uint)obj;
            if (obj is ushort)
                return (short)(ushort)obj;
            if (obj is sbyte)
                return (sbyte)obj;
            throw new InvalidCastException();
        }
        public static float ToFloat(this object obj)
        {
            if (obj is float)
                return (float)obj;
            if (obj is int)
                return (int)obj;
            if (obj is long)
                return (long)obj;
            if (obj is short)
                return (short)obj;
            if (obj is double)
                return (float)(double)obj;
            if (obj is byte)
                return (byte)obj;
            if (obj is uint)
                return (uint)obj;
            if (obj is ushort)
                return (ushort)obj;
            if (obj is sbyte)
                return (sbyte)obj;
            throw new InvalidCastException();
        }

        public static double ToDouble(this object obj)
        {
            if (obj is double)
                return (double)obj;
            if (obj is float)
                return (float)obj;
            if (obj is int)
                return (int)obj;
            if (obj is long)
                return (long)obj;
            if (obj is short)
                return (short)obj;
            if (obj is byte)
                return (byte)obj;
            if (obj is uint)
                return (uint)obj;
            if (obj is ushort)
                return (ushort)obj;
            if (obj is sbyte)
                return (sbyte)obj;
            throw new InvalidCastException();
        }

        public static Type GetActualType(this object value)
        {
            if (value is ILRuntime.Runtime.Enviorment.CrossBindingAdaptorType)
                return ((ILRuntime.Runtime.Enviorment.CrossBindingAdaptorType)value).ILInstance.Type.ReflectionType;
            if (value is ILRuntime.Runtime.Intepreter.ILTypeInstance)
                return ((ILRuntime.Runtime.Intepreter.ILTypeInstance)value).Type.ReflectionType;
            else
                return value.GetType();
        }

        public static bool MatchGenericParameters(this System.Reflection.MethodInfo m, Type[] genericArguments, Type returnType, params Type[] parameters)
        {
            System.Reflection.ParameterInfo[] param = m.GetParameters();
            if (param.Length == parameters.Length)
            {
                Type[] args = m.GetGenericArguments();
                if (args.Length != genericArguments.Length)
                {
                    return false;
                }
                if (args.MatchGenericParameters(m.ReturnType, returnType, genericArguments))
                {
                    for (int i = 0; i < param.Length; i++)
                    {
                        if (!args.MatchGenericParameters(param[i].ParameterType, parameters[i], genericArguments))
                            return false;
                    }

                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public static bool MatchGenericParameters(this Type[] args, Type type, Type q, Type[] genericArguments)
        {
            if (type.IsGenericParameter)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == type)
                    {
                        return q == genericArguments[i];
                    }
                }
                throw new NotSupportedException();
            }
            else
            {
                if (type.IsArray)
                {
                    if (q.IsArray)
                    {
                        return MatchGenericParameters(args, type.GetElementType(), q.GetElementType(), genericArguments);
                    }
                    else
                        return false;
                }
                else if (type.IsByRef)
                {
                    if (q.IsByRef)
                    {
                        return MatchGenericParameters(args, type.GetElementType(), q.GetElementType(), genericArguments);
                    }
                    else
                        return false;
                }
                else if (type.IsGenericType)
                {
                    if (q.IsGenericType)
                    {
                        Type t1 = type.GetGenericTypeDefinition();
                        Type t2 = type.GetGenericTypeDefinition();
                        if (t1 == t2)
                        {
                            Type[] argA = type.GetGenericArguments();
                            Type[] argB = q.GetGenericArguments();
                            if (argA.Length == argB.Length)
                            {
                                for (int i = 0; i < argA.Length; i++)
                                {
                                    if (!MatchGenericParameters(args, argA[i], argB[i], genericArguments))
                                        return false;
                                }
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return type == q;
            }
        }
    }
}
