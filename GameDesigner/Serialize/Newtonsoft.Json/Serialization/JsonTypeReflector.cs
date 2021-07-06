using Newtonsoft_X.Json.Utilities;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Newtonsoft_X.Json.Serialization
{
    internal static class JsonTypeReflector
    {
        public static T GetCachedAttribute<T>(object attributeProvider) where T : Attribute
        {
            return CachedAttributeGetter<T>.GetAttribute(attributeProvider);
        }

        public static DataContractAttribute GetDataContractAttribute(Type type)
        {
            for (Type type2 = type; type2 != null; type2 = type2.BaseType())
            {
                DataContractAttribute attribute = CachedAttributeGetter<DataContractAttribute>.GetAttribute(type2);
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return null;
        }

        public static DataMemberAttribute GetDataMemberAttribute(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType() == MemberTypes.Field)
            {
                return CachedAttributeGetter<DataMemberAttribute>.GetAttribute(memberInfo);
            }
            PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
            var attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(propertyInfo);
            if (attribute == null && propertyInfo.IsVirtual())
            {
                Type type = propertyInfo.DeclaringType;
                while (attribute == null && type != null)
                {
                    PropertyInfo propertyInfo2 = (PropertyInfo)ReflectionUtils.GetMemberInfoFromType(type, propertyInfo);
                    if (propertyInfo2 != null && propertyInfo2.IsVirtual())
                    {
                        attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(propertyInfo2);
                    }
                    type = type.BaseType();
                }
            }
            return attribute;
        }

        public static MemberSerialization GetObjectMemberSerialization(Type objectType, bool ignoreSerializableAttribute)
        {
            JsonObjectAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonObjectAttribute>(objectType);
            if (cachedAttribute != null)
            {
                return cachedAttribute.MemberSerialization;
            }
            if (JsonTypeReflector.GetDataContractAttribute(objectType) != null)
            {
                return MemberSerialization.OptIn;
            }
            if (!ignoreSerializableAttribute && JsonTypeReflector.GetCachedAttribute<SerializableAttribute>(objectType) != null)
            {
                return MemberSerialization.Fields;
            }
            return MemberSerialization.OptOut;
        }

        public static JsonConverter GetJsonConverter(object attributeProvider)
        {
            JsonConverterAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonConverterAttribute>(attributeProvider);
            if (cachedAttribute != null)
            {
                Func<object[], object> func = JsonTypeReflector.CreatorCache.Get(cachedAttribute.ConverterType);
                if (func != null)
                {
                    return (JsonConverter)func(cachedAttribute.ConverterParameters);
                }
            }
            return null;
        }

        /// <summary>
        /// Lookup and create an instance of the <see cref="T:Newtonsoft.Json.JsonConverter" /> type described by the argument.
        /// </summary>
        /// <param name="converterType">The <see cref="T:Newtonsoft.Json.JsonConverter" /> type to create.</param>
        /// <param name="args">Optional arguments to pass to an initializing constructor of the JsonConverter.
        /// If <c>null</c>, the default constructor is used.</param>
        public static JsonConverter CreateJsonConverterInstance(Type converterType, object[] converterArgs)
        {
            return (JsonConverter)JsonTypeReflector.CreatorCache.Get(converterType)(converterArgs);
        }

        public static NamingStrategy CreateNamingStrategyInstance(Type namingStrategyType, object[] converterArgs)
        {
            return (NamingStrategy)JsonTypeReflector.CreatorCache.Get(namingStrategyType)(converterArgs);
        }

        public static NamingStrategy GetContainerNamingStrategy(JsonContainerAttribute containerAttribute)
        {
            if (containerAttribute.NamingStrategyInstance == null)
            {
                if (containerAttribute.NamingStrategyType == null)
                {
                    return null;
                }
                containerAttribute.NamingStrategyInstance = JsonTypeReflector.CreateNamingStrategyInstance(containerAttribute.NamingStrategyType, containerAttribute.NamingStrategyParameters);
            }
            return containerAttribute.NamingStrategyInstance;
        }

        private static Func<object[], object> GetCreator(Type type)
        {
            Func<object> defaultConstructor = ReflectionUtils.HasDefaultConstructor(type, false) ? JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type) : null;
            return delegate (object[] parameters)
            {
                object result;
                try
                {
                    if (parameters != null)
                    {
                        Type[] types = (from param in parameters
                                        select param.GetType()).ToArray<Type>();
                        ConstructorInfo constructor = type.GetConstructor(types);
                        if (constructor == null)
                        {
                            throw new JsonException("No matching parameterized constructor found for '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
                        }
                        result = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor)(parameters);
                    }
                    else
                    {
                        if (defaultConstructor == null)
                        {
                            throw new JsonException("No parameterless constructor defined for '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
                        }
                        result = defaultConstructor();
                    }
                }
                catch (Exception innerException)
                {
                    throw new JsonException("Error creating '{0}'.".FormatWith(CultureInfo.InvariantCulture, type), innerException);
                }
                return result;
            };
        }

        public static TypeConverter GetTypeConverter(Type type)
        {
            return TypeDescriptor.GetConverter(type);
        }

        private static Type GetAssociatedMetadataType(Type type)
        {
            return JsonTypeReflector.AssociatedMetadataTypesCache.Get(type);
        }

        private static Type GetAssociateMetadataTypeFromAttribute(Type type)
        {
            foreach (Attribute attribute in ReflectionUtils.GetAttributes(type, null, true))
            {
                Type type2 = attribute.GetType();
                if (string.Equals(type2.FullName, "System.ComponentModel.DataAnnotations.MetadataTypeAttribute", StringComparison.Ordinal))
                {
                    if (JsonTypeReflector._metadataTypeAttributeReflectionObject == null)
                    {
                        JsonTypeReflector._metadataTypeAttributeReflectionObject = ReflectionObject.Create(type2, new string[]
                        {
                            "MetadataClassType"
                        });
                    }
                    return (Type)JsonTypeReflector._metadataTypeAttributeReflectionObject.GetValue(attribute, "MetadataClassType");
                }
            }
            return null;
        }

        private static T GetAttribute<T>(Type type) where T : Attribute
        {
            Type associatedMetadataType = JsonTypeReflector.GetAssociatedMetadataType(type);
            T attribute;
            if (associatedMetadataType != null)
            {
                attribute = ReflectionUtils.GetAttribute<T>(associatedMetadataType, true);
                if (attribute != null)
                {
                    return attribute;
                }
            }
            attribute = ReflectionUtils.GetAttribute<T>(type, true);
            if (attribute != null)
            {
                return attribute;
            }
            Type[] interfaces = type.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                attribute = ReflectionUtils.GetAttribute<T>(interfaces[i], true);
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return default(T);
        }

        private static T GetAttribute<T>(MemberInfo memberInfo) where T : Attribute
        {
            Type associatedMetadataType = JsonTypeReflector.GetAssociatedMetadataType(memberInfo.DeclaringType);
            T attribute;
            if (associatedMetadataType != null)
            {
                MemberInfo memberInfoFromType = ReflectionUtils.GetMemberInfoFromType(associatedMetadataType, memberInfo);
                if (memberInfoFromType != null)
                {
                    attribute = ReflectionUtils.GetAttribute<T>(memberInfoFromType, true);
                    if (attribute != null)
                    {
                        return attribute;
                    }
                }
            }
            attribute = ReflectionUtils.GetAttribute<T>(memberInfo, true);
            if (attribute != null)
            {
                return attribute;
            }
            if (memberInfo.DeclaringType != null)
            {
                Type[] interfaces = memberInfo.DeclaringType.GetInterfaces();
                for (int i = 0; i < interfaces.Length; i++)
                {
                    MemberInfo memberInfoFromType2 = ReflectionUtils.GetMemberInfoFromType(interfaces[i], memberInfo);
                    if (memberInfoFromType2 != null)
                    {
                        attribute = ReflectionUtils.GetAttribute<T>(memberInfoFromType2, true);
                        if (attribute != null)
                        {
                            return attribute;
                        }
                    }
                }
            }
            return default(T);
        }

        public static T GetAttribute<T>(object provider) where T : Attribute
        {
            Type type = provider as Type;
            if (type != null)
            {
                return JsonTypeReflector.GetAttribute<T>(type);
            }
            MemberInfo memberInfo = provider as MemberInfo;
            if (memberInfo != null)
            {
                return JsonTypeReflector.GetAttribute<T>(memberInfo);
            }
            return ReflectionUtils.GetAttribute<T>(provider, true);
        }

        public static bool DynamicCodeGeneration
        {
            get
            {
                if (JsonTypeReflector._dynamicCodeGeneration == null)
                {
                    JsonTypeReflector._dynamicCodeGeneration = new bool?(false);
                }
                return JsonTypeReflector._dynamicCodeGeneration.GetValueOrDefault();
            }
        }

        public static bool FullyTrusted
        {
            get
            {
                if (JsonTypeReflector._fullyTrusted == null)
                {
                    try
                    {
#if !CORE
                        new SecurityPermission(PermissionState.Unrestricted).Demand();
#endif
                        JsonTypeReflector._fullyTrusted = new bool?(true);
                    }
                    catch (Exception)
                    {
                        JsonTypeReflector._fullyTrusted = new bool?(false);
                    }
                }
                return JsonTypeReflector._fullyTrusted.GetValueOrDefault();
            }
        }

        public static ReflectionDelegateFactory ReflectionDelegateFactory
        {
            get
            {
                return LateBoundReflectionDelegateFactory.Instance;
            }
        }

        private static bool? _dynamicCodeGeneration;

        private static bool? _fullyTrusted;

        public const string IdPropertyName = "$id";

        public const string RefPropertyName = "$ref";

        public const string TypePropertyName = "$type";

        public const string ValuePropertyName = "$value";

        public const string ArrayValuesPropertyName = "$values";

        public const string ShouldSerializePrefix = "ShouldSerialize";

        public const string SpecifiedPostfix = "Specified";

        private static readonly ThreadSafeStore<Type, Func<object[], object>> CreatorCache = new ThreadSafeStore<Type, Func<object[], object>>(new Func<Type, Func<object[], object>>(JsonTypeReflector.GetCreator));

        private static readonly ThreadSafeStore<Type, Type> AssociatedMetadataTypesCache = new ThreadSafeStore<Type, Type>(new Func<Type, Type>(JsonTypeReflector.GetAssociateMetadataTypeFromAttribute));

        private static ReflectionObject _metadataTypeAttributeReflectionObject;
    }
}
