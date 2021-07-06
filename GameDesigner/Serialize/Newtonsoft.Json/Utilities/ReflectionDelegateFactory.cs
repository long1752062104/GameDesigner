using Newtonsoft_X.Json.Serialization;
using System;
using System.Globalization;
using System.Reflection;

namespace Newtonsoft_X.Json.Utilities
{
    internal abstract class ReflectionDelegateFactory
    {
        public Func<T, object> CreateGet<T>(MemberInfo memberInfo)
        {
            PropertyInfo propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return CreateGet<T>(propertyInfo);
            }
            FieldInfo fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return CreateGet<T>(fieldInfo);
            }
            throw new Exception("Could not create getter for {0}.".FormatWith(CultureInfo.InvariantCulture, memberInfo));
        }

        public Action<T, object> CreateSet<T>(MemberInfo memberInfo)
        {
            PropertyInfo propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return CreateSet<T>(propertyInfo);
            }
            FieldInfo fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return CreateSet<T>(fieldInfo);
            }
            throw new Exception("Could not create setter for {0}.".FormatWith(CultureInfo.InvariantCulture, memberInfo));
        }

        public abstract MethodCall<T, object> CreateMethodCall<T>(MethodBase method);

        public abstract ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method);

        public abstract Func<T> CreateDefaultConstructor<T>(Type type);

        public abstract Func<T, object> CreateGet<T>(PropertyInfo propertyInfo);

        public abstract Func<T, object> CreateGet<T>(FieldInfo fieldInfo);

        public abstract Action<T, object> CreateSet<T>(FieldInfo fieldInfo);

        public abstract Action<T, object> CreateSet<T>(PropertyInfo propertyInfo);
    }
}
