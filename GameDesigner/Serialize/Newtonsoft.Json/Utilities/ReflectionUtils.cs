using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace Newtonsoft_X.Json.Utilities
{
    internal static class ReflectionUtils
    {
        public static bool IsVirtual(this PropertyInfo propertyInfo)
        {
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
            MethodInfo methodInfo = propertyInfo.GetGetMethod();
            if (methodInfo != null && methodInfo.IsVirtual)
            {
                return true;
            }
            methodInfo = propertyInfo.GetSetMethod();
            return methodInfo != null && methodInfo.IsVirtual;
        }

        public static MethodInfo GetBaseDefinition(this PropertyInfo propertyInfo)
        {
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");
            MethodInfo methodInfo = propertyInfo.GetGetMethod();
            if (methodInfo != null)
            {
                return methodInfo.GetBaseDefinition();
            }
            methodInfo = propertyInfo.GetSetMethod();
            if (methodInfo != null)
            {
                return methodInfo.GetBaseDefinition();
            }
            return null;
        }

        public static bool IsPublic(PropertyInfo property)
        {
            return (property.GetGetMethod() != null && property.GetGetMethod().IsPublic) || (property.GetSetMethod() != null && property.GetSetMethod().IsPublic);
        }

        public static Type GetObjectType(object v)
        {
            if (v == null)
            {
                return null;
            }
            return v.GetType();
        }

        public static string GetTypeName(Type t, FormatterAssemblyStyle assemblyFormat, SerializationBinder binder)
        {
            string assemblyQualifiedName = t.AssemblyQualifiedName;
            if (assemblyFormat == FormatterAssemblyStyle.Simple)
            {
                return ReflectionUtils.RemoveAssemblyDetails(assemblyQualifiedName);
            }
            if (assemblyFormat != FormatterAssemblyStyle.Full)
            {
                throw new ArgumentOutOfRangeException();
            }
            return assemblyQualifiedName;
        }

        private static string RemoveAssemblyDetails(string fullyQualifiedTypeName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = false;
            bool flag2 = false;
            foreach (char c in fullyQualifiedTypeName)
            {
                if (c != ',')
                {
                    if (c != '[')
                    {
                        if (c != ']')
                        {
                            if (!flag2)
                            {
                                stringBuilder.Append(c);
                            }
                        }
                        else
                        {
                            flag = false;
                            flag2 = false;
                            stringBuilder.Append(c);
                        }
                    }
                    else
                    {
                        flag = false;
                        flag2 = false;
                        stringBuilder.Append(c);
                    }
                }
                else if (!flag)
                {
                    flag = true;
                    stringBuilder.Append(c);
                }
                else
                {
                    flag2 = true;
                }
            }
            return stringBuilder.ToString();
        }

        public static bool HasDefaultConstructor(Type t, bool nonPublic)
        {
            ValidationUtils.ArgumentNotNull(t, "t");
            return t.IsValueType() || ReflectionUtils.GetDefaultConstructor(t, nonPublic) != null;
        }

        public static ConstructorInfo GetDefaultConstructor(Type t)
        {
            return ReflectionUtils.GetDefaultConstructor(t, false);
        }

        public static ConstructorInfo GetDefaultConstructor(Type t, bool nonPublic)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            if (nonPublic)
            {
                bindingFlags |= BindingFlags.NonPublic;
            }
            return t.GetConstructors(bindingFlags).SingleOrDefault((ConstructorInfo c) => !c.GetParameters().Any<ParameterInfo>());
        }

        public static bool IsNullable(Type t)
        {
            ValidationUtils.ArgumentNotNull(t, "t");
            return !t.IsValueType() || ReflectionUtils.IsNullableType(t);
        }

        public static bool IsNullableType(Type t)
        {
            ValidationUtils.ArgumentNotNull(t, "t");
            return t.IsGenericType() && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type EnsureNotNullableType(Type t)
        {
            if (!ReflectionUtils.IsNullableType(t))
            {
                return t;
            }
            return Nullable.GetUnderlyingType(t);
        }

        public static bool IsGenericDefinition(Type type, Type genericInterfaceDefinition)
        {
            return type.IsGenericType() && type.GetGenericTypeDefinition() == genericInterfaceDefinition;
        }

        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition)
        {
            return ReflectionUtils.ImplementsGenericDefinition(type, genericInterfaceDefinition, out Type type2);
        }

        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition, out Type implementingType)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            ValidationUtils.ArgumentNotNull(genericInterfaceDefinition, "genericInterfaceDefinition");
            if (!genericInterfaceDefinition.IsInterface() || !genericInterfaceDefinition.IsGenericTypeDefinition())
            {
                throw new ArgumentNullException("'{0}' is not a generic interface definition.".FormatWith(CultureInfo.InvariantCulture, genericInterfaceDefinition));
            }
            if (type.IsInterface() && type.IsGenericType())
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericInterfaceDefinition == genericTypeDefinition)
                {
                    implementingType = type;
                    return true;
                }
            }
            foreach (Type type2 in type.GetInterfaces())
            {
                if (type2.IsGenericType())
                {
                    Type genericTypeDefinition2 = type2.GetGenericTypeDefinition();
                    if (genericInterfaceDefinition == genericTypeDefinition2)
                    {
                        implementingType = type2;
                        return true;
                    }
                }
            }
            implementingType = null;
            return false;
        }

        public static bool InheritsGenericDefinition(Type type, Type genericClassDefinition)
        {
            return ReflectionUtils.InheritsGenericDefinition(type, genericClassDefinition, out Type type2);
        }

        public static bool InheritsGenericDefinition(Type type, Type genericClassDefinition, out Type implementingType)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            ValidationUtils.ArgumentNotNull(genericClassDefinition, "genericClassDefinition");
            if (!genericClassDefinition.IsClass() || !genericClassDefinition.IsGenericTypeDefinition())
            {
                throw new ArgumentNullException("'{0}' is not a generic class definition.".FormatWith(CultureInfo.InvariantCulture, genericClassDefinition));
            }
            return ReflectionUtils.InheritsGenericDefinitionInternal(type, genericClassDefinition, out implementingType);
        }

        private static bool InheritsGenericDefinitionInternal(Type currentType, Type genericClassDefinition, out Type implementingType)
        {
            if (currentType.IsGenericType())
            {
                Type genericTypeDefinition = currentType.GetGenericTypeDefinition();
                if (genericClassDefinition == genericTypeDefinition)
                {
                    implementingType = currentType;
                    return true;
                }
            }
            if (currentType.BaseType() == null)
            {
                implementingType = null;
                return false;
            }
            return ReflectionUtils.InheritsGenericDefinitionInternal(currentType.BaseType(), genericClassDefinition, out implementingType);
        }

        /// <summary>
        /// Gets the type of the typed collection's items.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type of the typed collection's items.</returns>
        public static Type GetCollectionItemType(Type type)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            if (ReflectionUtils.ImplementsGenericDefinition(type, typeof(IEnumerable<>), out Type type2))
            {
                if (type2.IsGenericTypeDefinition())
                {
                    throw new Exception("Type {0} is not a collection.".FormatWith(CultureInfo.InvariantCulture, type));
                }
                return type2.GetGenericArguments()[0];
            }
            else
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return null;
                }
                throw new Exception("Type {0} is not a collection.".FormatWith(CultureInfo.InvariantCulture, type));
            }
        }

        public static void GetDictionaryKeyValueTypes(Type dictionaryType, out Type keyType, out Type valueType)
        {
            ValidationUtils.ArgumentNotNull(dictionaryType, "dictionaryType");
            if (ReflectionUtils.ImplementsGenericDefinition(dictionaryType, typeof(IDictionary<,>), out Type type))
            {
                if (type.IsGenericTypeDefinition())
                {
                    throw new Exception("Type {0} is not a dictionary.".FormatWith(CultureInfo.InvariantCulture, dictionaryType));
                }
                Type[] genericArguments = type.GetGenericArguments();
                keyType = genericArguments[0];
                valueType = genericArguments[1];
                return;
            }
            else
            {
                if (typeof(IDictionary).IsAssignableFrom(dictionaryType))
                {
                    keyType = null;
                    valueType = null;
                    return;
                }
                throw new Exception("Type {0} is not a dictionary.".FormatWith(CultureInfo.InvariantCulture, dictionaryType));
            }
        }

        /// <summary>
        /// Gets the member's underlying type.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>The underlying type of the member.</returns>
        public static Type GetMemberUnderlyingType(MemberInfo member)
        {
            ValidationUtils.ArgumentNotNull(member, "member");
            MemberTypes memberTypes = member.MemberType();
            if (memberTypes <= MemberTypes.Field)
            {
                if (memberTypes == MemberTypes.Event)
                {
                    return ((EventInfo)member).EventHandlerType;
                }
                if (memberTypes == MemberTypes.Field)
                {
                    return ((FieldInfo)member).FieldType;
                }
            }
            else
            {
                if (memberTypes == MemberTypes.Method)
                {
                    return ((MethodInfo)member).ReturnType;
                }
                if (memberTypes == MemberTypes.Property)
                {
                    return ((PropertyInfo)member).PropertyType;
                }
            }
            throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo, EventInfo or MethodInfo", "member");
        }

        public static bool IsIndexedProperty(MemberInfo member)
        {
            ValidationUtils.ArgumentNotNull(member, "member");
            PropertyInfo propertyInfo = member as PropertyInfo;
            return propertyInfo != null && ReflectionUtils.IsIndexedProperty(propertyInfo);
        }

        /// <summary>
        /// Determines whether the property is an indexed property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// 	<c>true</c> if the property is an indexed property; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIndexedProperty(PropertyInfo property)
        {
            ValidationUtils.ArgumentNotNull(property, "property");
            return property.GetIndexParameters().Length != 0;
        }

        /// <summary>
        /// Gets the member's value on the object.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="target">The target object.</param>
        /// <returns>The member's value on the object.</returns>
        public static object GetMemberValue(MemberInfo member, object target)
        {
            ValidationUtils.ArgumentNotNull(member, "member");
            ValidationUtils.ArgumentNotNull(target, "target");
            MemberTypes memberTypes = member.MemberType();
            if (memberTypes != MemberTypes.Field)
            {
                if (memberTypes == MemberTypes.Property)
                {
                    try
                    {
                        return ((PropertyInfo)member).GetValue(target, null);
                    }
                    catch (TargetParameterCountException innerException)
                    {
                        throw new ArgumentException("MemberInfo '{0}' has index parameters".FormatWith(CultureInfo.InvariantCulture, member.Name), innerException);
                    }
                }
                throw new ArgumentException("MemberInfo '{0}' is not of type FieldInfo or PropertyInfo".FormatWith(CultureInfo.InvariantCulture, CultureInfo.InvariantCulture, member.Name), "member");
            }
            return ((FieldInfo)member).GetValue(target);
        }

        /// <summary>
        /// Sets the member's value on the target object.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetMemberValue(MemberInfo member, object target, object value)
        {
            ValidationUtils.ArgumentNotNull(member, "member");
            ValidationUtils.ArgumentNotNull(target, "target");
            MemberTypes memberTypes = member.MemberType();
            if (memberTypes == MemberTypes.Field)
            {
                ((FieldInfo)member).SetValue(target, value);
                return;
            }
            if (memberTypes != MemberTypes.Property)
            {
                throw new ArgumentException("MemberInfo '{0}' must be of type FieldInfo or PropertyInfo".FormatWith(CultureInfo.InvariantCulture, member.Name), "member");
            }
            ((PropertyInfo)member).SetValue(target, value, null);
        }

        /// <summary>
        /// Determines whether the specified MemberInfo can be read.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be read.</param>
        /// /// <param name="nonPublic">if set to <c>true</c> then allow the member to be gotten non-publicly.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be read; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanReadMemberValue(MemberInfo member, bool nonPublic)
        {
            MemberTypes memberTypes = member.MemberType();
            if (memberTypes == MemberTypes.Field)
            {
                FieldInfo fieldInfo = (FieldInfo)member;
                return nonPublic || fieldInfo.IsPublic;
            }
            if (memberTypes != MemberTypes.Property)
            {
                return false;
            }
            PropertyInfo propertyInfo = (PropertyInfo)member;
            return propertyInfo.CanRead && (nonPublic || propertyInfo.GetGetMethod(nonPublic) != null);
        }

        /// <summary>
        /// Determines whether the specified MemberInfo can be set.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be set.</param>
        /// <param name="nonPublic">if set to <c>true</c> then allow the member to be set non-publicly.</param>
        /// <param name="canSetReadOnly">if set to <c>true</c> then allow the member to be set if read-only.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be set; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanSetMemberValue(MemberInfo member, bool nonPublic, bool canSetReadOnly)
        {
            MemberTypes memberTypes = member.MemberType();
            if (memberTypes == MemberTypes.Field)
            {
                FieldInfo fieldInfo = (FieldInfo)member;
                return !fieldInfo.IsLiteral && (!fieldInfo.IsInitOnly || canSetReadOnly) && (nonPublic || fieldInfo.IsPublic);
            }
            if (memberTypes != MemberTypes.Property)
            {
                return false;
            }
            PropertyInfo propertyInfo = (PropertyInfo)member;
            return propertyInfo.CanWrite && (nonPublic || propertyInfo.GetSetMethod(nonPublic) != null);
        }

        public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
        {
            List<MemberInfo> list = new List<MemberInfo>();
            list.AddRange(ReflectionUtils.GetFields(type, bindingAttr));
            list.AddRange(ReflectionUtils.GetProperties(type, bindingAttr));
            List<MemberInfo> list2 = new List<MemberInfo>(list.Count);
            foreach (IGrouping<string, MemberInfo> source in from m in list
                                                             group m by m.Name)
            {
                int num = source.Count<MemberInfo>();
                IList<MemberInfo> list3 = source.ToList<MemberInfo>();
                if (num == 1)
                {
                    list2.Add(list3.First<MemberInfo>());
                }
                else
                {
                    IList<MemberInfo> list4 = new List<MemberInfo>();
                    foreach (MemberInfo memberInfo in list3)
                    {
                        if (list4.Count == 0)
                        {
                            list4.Add(memberInfo);
                        }
                        else if (!ReflectionUtils.IsOverridenGenericMember(memberInfo, bindingAttr) || memberInfo.Name == "Item")
                        {
                            list4.Add(memberInfo);
                        }
                    }
                    list2.AddRange(list4);
                }
            }
            return list2;
        }

        private static bool IsOverridenGenericMember(MemberInfo memberInfo, BindingFlags bindingAttr)
        {
            if (memberInfo.MemberType() != MemberTypes.Property)
            {
                return false;
            }
            PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
            if (!propertyInfo.IsVirtual())
            {
                return false;
            }
            Type declaringType = propertyInfo.DeclaringType;
            if (!declaringType.IsGenericType())
            {
                return false;
            }
            Type genericTypeDefinition = declaringType.GetGenericTypeDefinition();
            if (genericTypeDefinition == null)
            {
                return false;
            }
            MemberInfo[] member = genericTypeDefinition.GetMember(propertyInfo.Name, bindingAttr);
            return member.Length != 0 && ReflectionUtils.GetMemberUnderlyingType(member[0]).IsGenericParameter;
        }

        public static T GetAttribute<T>(object attributeProvider) where T : Attribute
        {
            return ReflectionUtils.GetAttribute<T>(attributeProvider, true);
        }

        public static T GetAttribute<T>(object attributeProvider, bool inherit) where T : Attribute
        {
            T[] attributes = ReflectionUtils.GetAttributes<T>(attributeProvider, inherit);
            if (attributes == null)
            {
                return default(T);
            }
            return attributes.FirstOrDefault<T>();
        }

        public static T[] GetAttributes<T>(object attributeProvider, bool inherit) where T : Attribute
        {
            Attribute[] attributes = ReflectionUtils.GetAttributes(attributeProvider, typeof(T), inherit);
            T[] array = attributes as T[];
            if (array != null)
            {
                return array;
            }
            if (attributes == null)
                return null;
            return attributes.Cast<T>().ToArray<T>();
        }

        public static Attribute[] GetAttributes(object attributeProvider, Type attributeType, bool inherit)
        {
            ValidationUtils.ArgumentNotNull(attributeProvider, "attributeProvider");
            if (attributeProvider is Type)
            {
                Type type = (Type)attributeProvider;
                Attribute[] array = ((attributeType != null) ? type.GetCustomAttributes(attributeType, inherit) : type.GetCustomAttributes(inherit)).Cast<Attribute>().ToArray<Attribute>();
                if (inherit && type.BaseType != null)
                {
                    array = array.Union(ReflectionUtils.GetAttributes(type.BaseType, attributeType, inherit)).ToArray<Attribute>();
                }
                return array;
            }
            if (attributeProvider is Assembly)
            {
                Assembly element = (Assembly)attributeProvider;
                if (attributeType == null)
                {
                    return Attribute.GetCustomAttributes(element);
                }
                return Attribute.GetCustomAttributes(element, attributeType);
            }
            else if (attributeProvider is MemberInfo)
            {
                MemberInfo element2 = (MemberInfo)attributeProvider;
                if (attributeType == null)
                {
                    return Attribute.GetCustomAttributes(element2, inherit);
                }
                return Attribute.GetCustomAttributes(element2, attributeType, inherit);
            }
            else if (attributeProvider is Module)
            {
                Module element3 = (Module)attributeProvider;
                if (attributeType == null)
                {
                    return Attribute.GetCustomAttributes(element3, inherit);
                }
                return Attribute.GetCustomAttributes(element3, attributeType, inherit);
            }
            else
            {
                if (!(attributeProvider is ParameterInfo))
                {
                    ICustomAttributeProvider customAttributeProvider = (ICustomAttributeProvider)attributeProvider;
                    return (Attribute[])((attributeType != null) ? customAttributeProvider.GetCustomAttributes(attributeType, inherit) : customAttributeProvider.GetCustomAttributes(inherit));
                }
                ParameterInfo element4 = (ParameterInfo)attributeProvider;
                if (attributeType == null)
                {
                    return Attribute.GetCustomAttributes(element4, inherit);
                }
                return Attribute.GetCustomAttributes(element4, attributeType, inherit);
            }
        }

        public static void SplitFullyQualifiedTypeName(string fullyQualifiedTypeName, out string typeName, out string assemblyName)
        {
            int? assemblyDelimiterIndex = ReflectionUtils.GetAssemblyDelimiterIndex(fullyQualifiedTypeName);
            if (assemblyDelimiterIndex != null)
            {
                typeName = fullyQualifiedTypeName.Substring(0, assemblyDelimiterIndex.GetValueOrDefault()).Trim();
                assemblyName = fullyQualifiedTypeName.Substring(assemblyDelimiterIndex.GetValueOrDefault() + 1, fullyQualifiedTypeName.Length - assemblyDelimiterIndex.GetValueOrDefault() - 1).Trim();
                return;
            }
            typeName = fullyQualifiedTypeName;
            assemblyName = null;
        }

        private static int? GetAssemblyDelimiterIndex(string fullyQualifiedTypeName)
        {
            int num = 0;
            for (int i = 0; i < fullyQualifiedTypeName.Length; i++)
            {
                char c = fullyQualifiedTypeName[i];
                if (c != ',')
                {
                    if (c != '[')
                    {
                        if (c == ']')
                        {
                            num--;
                        }
                    }
                    else
                    {
                        num++;
                    }
                }
                else if (num == 0)
                {
                    return new int?(i);
                }
            }
            return null;
        }

        public static MemberInfo GetMemberInfoFromType(Type targetType, MemberInfo memberInfo)
        {
            MemberTypes memberTypes = memberInfo.MemberType();
            if (memberTypes == MemberTypes.Property)
            {
                PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
                Type[] types = (from p in propertyInfo.GetIndexParameters()
                                select p.ParameterType).ToArray<Type>();
                return targetType.GetProperty(propertyInfo.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, propertyInfo.PropertyType, types, null);
            }
            return targetType.GetMember(memberInfo.Name, memberInfo.MemberType(), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SingleOrDefault<MemberInfo>();
        }

        public static IEnumerable<FieldInfo> GetFields(Type targetType, BindingFlags bindingAttr)
        {
            ValidationUtils.ArgumentNotNull(targetType, "targetType");
            List<MemberInfo> list = new List<MemberInfo>(targetType.GetFields(bindingAttr));
            ReflectionUtils.GetChildPrivateFields(list, targetType, bindingAttr);
            return list.Cast<FieldInfo>();
        }

        private static void GetChildPrivateFields(IList<MemberInfo> initialFields, Type targetType, BindingFlags bindingAttr)
        {
            if ((bindingAttr & BindingFlags.NonPublic) != BindingFlags.Default)
            {
                BindingFlags bindingAttr2 = bindingAttr.RemoveFlag(BindingFlags.Public);
                while ((targetType = targetType.BaseType()) != null)
                {
                    IEnumerable<MemberInfo> collection = (from f in targetType.GetFields(bindingAttr2)
                                                          where f.IsPrivate
                                                          select f).Cast<MemberInfo>();
                    initialFields.AddRange(collection);
                }
            }
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type targetType, BindingFlags bindingAttr)
        {
            ValidationUtils.ArgumentNotNull(targetType, "targetType");
            List<PropertyInfo> list = new List<PropertyInfo>(targetType.GetProperties(bindingAttr));
            if (targetType.IsInterface())
            {
                foreach (Type type in targetType.GetInterfaces())
                {
                    list.AddRange(type.GetProperties(bindingAttr));
                }
            }
            ReflectionUtils.GetChildPrivateProperties(list, targetType, bindingAttr);
            for (int j = 0; j < list.Count; j++)
            {
                PropertyInfo propertyInfo = list[j];
                if (propertyInfo.DeclaringType != targetType)
                {
                    PropertyInfo value = (PropertyInfo)ReflectionUtils.GetMemberInfoFromType(propertyInfo.DeclaringType, propertyInfo);
                    list[j] = value;
                }
            }
            return list;
        }

        public static BindingFlags RemoveFlag(this BindingFlags bindingAttr, BindingFlags flag)
        {
            if ((bindingAttr & flag) != flag)
            {
                return bindingAttr;
            }
            return bindingAttr ^ flag;
        }

        private static void GetChildPrivateProperties(IList<PropertyInfo> initialProperties, Type targetType, BindingFlags bindingAttr)
        {
            while ((targetType = targetType.BaseType()) != null)
            {
                PropertyInfo[] properties = targetType.GetProperties(bindingAttr);
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo subTypeProperty2 = properties[i];
                    PropertyInfo subTypeProperty = subTypeProperty2;
                    if (!ReflectionUtils.IsPublic(subTypeProperty))
                    {
                        int num = initialProperties.IndexOf((PropertyInfo p) => p.Name == subTypeProperty.Name);
                        if (num == -1)
                        {
                            initialProperties.Add(subTypeProperty);
                        }
                        else if (!ReflectionUtils.IsPublic(initialProperties[num]))
                        {
                            initialProperties[num] = subTypeProperty;
                        }
                    }
                    else if (!subTypeProperty.IsVirtual())
                    {
                        if (initialProperties.IndexOf((PropertyInfo p) => p.Name == subTypeProperty.Name && p.DeclaringType == subTypeProperty.DeclaringType) == -1)
                        {
                            initialProperties.Add(subTypeProperty);
                        }
                    }
                    else if (initialProperties.IndexOf((PropertyInfo p) => p.Name == subTypeProperty.Name && p.IsVirtual() && p.GetBaseDefinition() != null && p.GetBaseDefinition().DeclaringType.IsAssignableFrom(subTypeProperty.GetBaseDefinition().DeclaringType)) == -1)
                    {
                        initialProperties.Add(subTypeProperty);
                    }
                }
            }
        }

        public static bool IsMethodOverridden(Type currentType, Type methodDeclaringType, string method)
        {
            return currentType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any((MethodInfo info) => info.Name == method && info.DeclaringType != methodDeclaringType && info.GetBaseDefinition().DeclaringType == methodDeclaringType);
        }

        public static object GetDefaultValue(Type type)
        {
            if (!type.IsValueType())
            {
                return null;
            }
            switch (ConvertUtils.GetTypeCode(type))
            {
                case PrimitiveTypeCode.Char:
                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.UInt32:
                    return 0;
                case PrimitiveTypeCode.Boolean:
                    return false;
                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                    return 0L;
                case PrimitiveTypeCode.Single:
                    return 0f;
                case PrimitiveTypeCode.Double:
                    return 0.0;
                case PrimitiveTypeCode.DateTime:
                    return default(DateTime);
                case PrimitiveTypeCode.DateTimeOffset:
                    return default(DateTimeOffset);
                case PrimitiveTypeCode.Decimal:
                    return 0m;
                case PrimitiveTypeCode.Guid:
                    return default(Guid);
            }
            if (ReflectionUtils.IsNullable(type))
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }

        public static readonly Type[] EmptyTypes = Type.EmptyTypes;
    }
}
