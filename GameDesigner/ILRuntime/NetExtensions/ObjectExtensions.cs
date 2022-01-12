#if !CLOSE_ILR
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Reflection;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Net.Share
{
    public static class ObjectExtensions
    {
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