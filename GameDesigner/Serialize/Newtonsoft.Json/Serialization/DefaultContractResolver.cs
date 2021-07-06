using Newtonsoft_X.Json.Converters;
using Newtonsoft_X.Json.Linq;
using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Used by <see cref="T:Newtonsoft.Json.JsonSerializer" /> to resolve a <see cref="T:Newtonsoft.Json.Serialization.JsonContract" /> for a given <see cref="T:System.Type" />.
    /// </summary>
    public class DefaultContractResolver : IContractResolver
    {
        internal static IContractResolver Instance
        {
            get
            {
                return DefaultContractResolver._instance;
            }
        }

        /// <summary>
        /// Gets a value indicating whether members are being get and set using dynamic code generation.
        /// This value is determined by the runtime permissions available.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if using dynamic code generation; otherwise, <c>false</c>.
        /// </value>
        public bool DynamicCodeGeneration
        {
            get
            {
                return JsonTypeReflector.DynamicCodeGeneration;
            }
        }

        /// <summary>
        /// Gets or sets the default members search flags.
        /// </summary>
        /// <value>The default members search flags.</value>
        [Obsolete("DefaultMembersSearchFlags is obsolete. To modify the members serialized inherit from DefaultContractResolver and override the GetSerializableMembers method instead.")]
        public BindingFlags DefaultMembersSearchFlags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether compiler generated members should be serialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if serialized compiler generated members; otherwise, <c>false</c>.
        /// </value>
        public bool SerializeCompilerGeneratedMembers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface when serializing and deserializing types.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface will be ignored when serializing and deserializing types; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreSerializableInterface { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the <see cref="T:System.SerializableAttribute" /> attribute when serializing and deserializing types.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="T:System.SerializableAttribute" /> attribute will be ignored when serializing and deserializing types; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreSerializableAttribute { get; set; }

        /// <summary>
        /// Gets or sets the naming strategy used to resolve how property names and dictionary keys are serialized.
        /// </summary>
        /// <value>The naming strategy used to resolve how property names and dictionary keys are serialized.</value>
        public NamingStrategy NamingStrategy { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Serialization.DefaultContractResolver" /> class.
        /// </summary>
        public DefaultContractResolver()
        {
            IgnoreSerializableAttribute = true;
            DefaultMembersSearchFlags = (BindingFlags.Instance | BindingFlags.Public);
        }

        [Obsolete("DefaultContractResolver(bool) is obsolete. Use the parameterless constructor and cache instances of the contract resolver within your application for optimal performance.")]
        public DefaultContractResolver(bool shareCache) : this()
        {
            _sharedCache = shareCache;
        }

        internal DefaultContractResolverState GetState()
        {
            if (_sharedCache)
            {
                return _sharedState;
            }
            return _instanceState;
        }

        /// <summary>
        /// Resolves the contract for a given type.
        /// </summary>
        /// <param name="type">The type to resolve a contract for.</param>
        /// <returns>The contract for a given type.</returns>
        public virtual JsonContract ResolveContract(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
#if !CLOSE_ILR
            type = Net.Share.ObjectExtensions.GetType(type);
#endif
            DefaultContractResolverState state = GetState();
            ResolverContractKey key = new ResolverContractKey(GetType(), type);
            Dictionary<ResolverContractKey, JsonContract> contractCache = state.ContractCache;
            if (contractCache == null || !contractCache.TryGetValue(key, out JsonContract jsonContract))
            {
                jsonContract = CreateContract(type);
                object typeContractCacheLock = TypeContractCacheLock;
                lock (typeContractCacheLock)
                {
                    contractCache = state.ContractCache;
                    Dictionary<ResolverContractKey, JsonContract> dictionary = (contractCache != null) ? new Dictionary<ResolverContractKey, JsonContract>(contractCache) : new Dictionary<ResolverContractKey, JsonContract>();
                    dictionary[key] = jsonContract;
                    state.ContractCache = dictionary;
                }
            }
            return jsonContract;
        }

        /// <summary>
        /// Gets the serializable members for the type.
        /// </summary>
        /// <param name="objectType">The type to get serializable members for.</param>
        /// <returns>The serializable members for the type.</returns>
        protected virtual List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            bool ignoreSerializableAttribute = IgnoreSerializableAttribute;
            MemberSerialization objectMemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(objectType, ignoreSerializableAttribute);
            List<MemberInfo> list = (from m in ReflectionUtils.GetFieldsAndProperties(objectType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                     where !ReflectionUtils.IsIndexedProperty(m)
                                     select m).ToList<MemberInfo>();
            List<MemberInfo> list2 = new List<MemberInfo>();
            if (objectMemberSerialization != MemberSerialization.Fields)
            {
                DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(objectType);
                List<MemberInfo> list3 = (from m in ReflectionUtils.GetFieldsAndProperties(objectType, DefaultMembersSearchFlags)
                                          where !ReflectionUtils.IsIndexedProperty(m)
                                          select m).ToList<MemberInfo>();
                foreach (MemberInfo memberInfo in list)
                {
                    if (SerializeCompilerGeneratedMembers || !memberInfo.IsDefined(typeof(CompilerGeneratedAttribute), true))
                    {
                        if (list3.Contains(memberInfo))
                        {
                            list2.Add(memberInfo);
                        }
                        else if (JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(memberInfo) != null)
                        {
                            list2.Add(memberInfo);
                        }
                        else if (JsonTypeReflector.GetAttribute<JsonRequiredAttribute>(memberInfo) != null)
                        {
                            list2.Add(memberInfo);
                        }
                        else if (dataContractAttribute != null && JsonTypeReflector.GetAttribute<DataMemberAttribute>(memberInfo) != null)
                        {
                            list2.Add(memberInfo);
                        }
                        else if (objectMemberSerialization == MemberSerialization.Fields && memberInfo.MemberType() == MemberTypes.Field)
                        {
                            list2.Add(memberInfo);
                        }
                    }
                }
                if (objectType.AssignableToTypeName("System.Data.Objects.DataClasses.EntityObject", out Type type))
                {
                    list2 = list2.Where(new Func<MemberInfo, bool>(ShouldSerializeEntityMember)).ToList<MemberInfo>();
                }
            }
            else
            {
                foreach (MemberInfo memberInfo2 in list)
                {
                    FieldInfo fieldInfo = memberInfo2 as FieldInfo;
                    if (fieldInfo != null && !fieldInfo.IsStatic)
                    {
                        list2.Add(memberInfo2);
                    }
                }
            }
            return list2;
        }

        private bool ShouldSerializeEntityMember(MemberInfo memberInfo)
        {
            PropertyInfo propertyInfo = memberInfo as PropertyInfo;
            return propertyInfo == null || !propertyInfo.PropertyType.IsGenericType() || !(propertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "System.Data.Objects.DataClasses.EntityReference`1");
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonObjectContract" /> for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonObjectContract" /> for the given type.</returns>
        protected virtual JsonObjectContract CreateObjectContract(Type objectType)
        {
            JsonObjectContract jsonObjectContract = new JsonObjectContract(objectType);
            InitializeContract(jsonObjectContract);
            bool ignoreSerializableAttribute = IgnoreSerializableAttribute;
            jsonObjectContract.MemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(jsonObjectContract.NonNullableUnderlyingType, ignoreSerializableAttribute);
            jsonObjectContract.Properties.AddRange(CreateProperties(jsonObjectContract.NonNullableUnderlyingType, jsonObjectContract.MemberSerialization));
            JsonObjectAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonObjectAttribute>(jsonObjectContract.NonNullableUnderlyingType);
            if (cachedAttribute != null)
            {
                jsonObjectContract.ItemRequired = cachedAttribute._itemRequired;
            }
            if (jsonObjectContract.IsInstantiable)
            {
                ConstructorInfo attributeConstructor = GetAttributeConstructor(jsonObjectContract.NonNullableUnderlyingType);
                if (attributeConstructor != null)
                {
                    jsonObjectContract.OverrideConstructor = attributeConstructor;
                    jsonObjectContract.CreatorParameters.AddRange(CreateConstructorParameters(attributeConstructor, jsonObjectContract.Properties));
                }
                else if (jsonObjectContract.MemberSerialization == MemberSerialization.Fields)
                {
                    if (JsonTypeReflector.FullyTrusted)
                    {
                        jsonObjectContract.DefaultCreator = new Func<object>(jsonObjectContract.GetUninitializedObject);
                    }
                }
                else if (jsonObjectContract.DefaultCreator == null || jsonObjectContract.DefaultCreatorNonPublic)
                {
                    ConstructorInfo parameterizedConstructor = GetParameterizedConstructor(jsonObjectContract.NonNullableUnderlyingType);
                    if (parameterizedConstructor != null)
                    {
                        jsonObjectContract.ParametrizedConstructor = parameterizedConstructor;
                        jsonObjectContract.CreatorParameters.AddRange(CreateConstructorParameters(parameterizedConstructor, jsonObjectContract.Properties));
                    }
                }
            }
            MemberInfo extensionDataMemberForType = GetExtensionDataMemberForType(jsonObjectContract.NonNullableUnderlyingType);
            if (extensionDataMemberForType != null)
            {
                SetExtensionDataDelegates(jsonObjectContract, extensionDataMemberForType);
            }
            return jsonObjectContract;
        }

        private MemberInfo GetExtensionDataMemberForType(Type type)
        {
            return GetClassHierarchyForType(type).SelectMany(delegate (Type baseType)
            {
                List<MemberInfo> list = new List<MemberInfo>();
                list.AddRange(baseType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                list.AddRange(baseType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                return list;
            }).LastOrDefault(delegate (MemberInfo m)
            {
                MemberTypes memberTypes = m.MemberType();
                if (memberTypes != MemberTypes.Property && memberTypes != MemberTypes.Field)
                {
                    return false;
                }
                if (!m.IsDefined(typeof(JsonExtensionDataAttribute), false))
                {
                    return false;
                }
                if (!ReflectionUtils.CanReadMemberValue(m, true))
                {
                    throw new JsonException("Invalid extension data attribute on '{0}'. Member '{1}' must have a getter.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(m.DeclaringType), m.Name));
                }
                if (ReflectionUtils.ImplementsGenericDefinition(ReflectionUtils.GetMemberUnderlyingType(m), typeof(IDictionary<,>), out Type type2))
                {
                    Type type3 = type2.GetGenericArguments()[0];
                    Type type4 = type2.GetGenericArguments()[1];
                    if (type3.IsAssignableFrom(typeof(string)) && type4.IsAssignableFrom(typeof(JToken)))
                    {
                        return true;
                    }
                }
                throw new JsonException("Invalid extension data attribute on '{0}'. Member '{1}' type must implement IDictionary<string, JToken>.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(m.DeclaringType), m.Name));
            });
        }

        private static void SetExtensionDataDelegates(JsonObjectContract contract, MemberInfo member)
        {
            JsonExtensionDataAttribute attribute = ReflectionUtils.GetAttribute<JsonExtensionDataAttribute>(member);
            if (attribute == null)
            {
                return;
            }
            Type memberUnderlyingType = ReflectionUtils.GetMemberUnderlyingType(member);
            ReflectionUtils.ImplementsGenericDefinition(memberUnderlyingType, typeof(IDictionary<,>), out Type type);
            Type type2 = type.GetGenericArguments()[0];
            Type type3 = type.GetGenericArguments()[1];
            Type type4;
            if (ReflectionUtils.IsGenericDefinition(memberUnderlyingType, typeof(IDictionary<,>)))
            {
                type4 = typeof(Dictionary<,>).MakeGenericType(new Type[]
                {
                    type2,
                    type3
                });
            }
            else
            {
                type4 = memberUnderlyingType;
            }
            Func<object, object> getExtensionDataDictionary = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(member);
            if (attribute.ReadData)
            {
                Action<object, object> setExtensionDataDictionary = ReflectionUtils.CanSetMemberValue(member, true, false) ? JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(member) : null;
                Func<object> createExtensionDataDictionary = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type4);
                MethodInfo method = memberUnderlyingType.GetMethod("Add", new Type[]
                {
                    type2,
                    type3
                });
                MethodCall<object, object> setExtensionDataDictionaryValue = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
                ExtensionDataSetter extensionDataSetter = delegate (object o, string key, object value)
                {
                    object obj = getExtensionDataDictionary(o);
                    if (obj == null)
                    {
                        if (setExtensionDataDictionary == null)
                        {
                            throw new JsonSerializationException("Cannot set value onto extension data member '{0}'. The extension data collection is null and it cannot be set.".FormatWith(CultureInfo.InvariantCulture, member.Name));
                        }
                        obj = createExtensionDataDictionary();
                        setExtensionDataDictionary(o, obj);
                    }
                    setExtensionDataDictionaryValue(obj, new object[]
                    {
                        key,
                        value
                    });
                };
                contract.ExtensionDataSetter = extensionDataSetter;
            }
            if (attribute.WriteData)
            {
                ConstructorInfo method2 = typeof(EnumerableDictionaryWrapper<,>).MakeGenericType(new Type[]
                {
                    type2,
                    type3
                }).GetConstructors().First();
                ObjectConstructor<object> createEnumerableWrapper = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(method2);
                ExtensionDataGetter extensionDataGetter = delegate (object o)
                {
                    object obj = getExtensionDataDictionary(o);
                    if (obj == null)
                    {
                        return null;
                    }
                    return (IEnumerable<KeyValuePair<object, object>>)createEnumerableWrapper(new object[]
                    {
                        obj
                    });
                };
                contract.ExtensionDataGetter = extensionDataGetter;
            }
            contract.ExtensionDataValueType = type3;
        }

        private ConstructorInfo GetAttributeConstructor(Type objectType)
        {
            IList<ConstructorInfo> list = (from c in objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                           where c.IsDefined(typeof(JsonConstructorAttribute), true)
                                           select c).ToList<ConstructorInfo>();
            if (list.Count > 1)
            {
                throw new JsonException("Multiple constructors with the JsonConstructorAttribute.");
            }
            if (list.Count == 1)
            {
                return list[0];
            }
            if (objectType == typeof(Version))
            {
                return objectType.GetConstructor(new Type[]
                {
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int)
                });
            }
            return null;
        }

        private ConstructorInfo GetParameterizedConstructor(Type objectType)
        {
            IList<ConstructorInfo> list = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).ToList<ConstructorInfo>();
            if (list.Count == 1)
            {
                return list[0];
            }
            return null;
        }

        /// <summary>
        /// Creates the constructor parameters.
        /// </summary>
        /// <param name="constructor">The constructor to create properties for.</param>
        /// <param name="memberProperties">The type's member properties.</param>
        /// <returns>Properties for the given <see cref="T:System.Reflection.ConstructorInfo" />.</returns>
        protected virtual IList<JsonProperty> CreateConstructorParameters(ConstructorInfo constructor, JsonPropertyCollection memberProperties)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(constructor.DeclaringType);
            foreach (ParameterInfo parameterInfo in parameters)
            {
                JsonProperty jsonProperty = (parameterInfo.Name != null) ? memberProperties.GetClosestMatchProperty(parameterInfo.Name) : null;
                if (jsonProperty != null && jsonProperty.PropertyType != parameterInfo.ParameterType)
                {
                    jsonProperty = null;
                }
                if (jsonProperty != null || parameterInfo.Name != null)
                {
                    JsonProperty jsonProperty2 = CreatePropertyFromConstructorParameter(jsonProperty, parameterInfo);
                    if (jsonProperty2 != null)
                    {
                        jsonPropertyCollection.AddProperty(jsonProperty2);
                    }
                }
            }
            return jsonPropertyCollection;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.ParameterInfo" />.
        /// </summary>
        /// <param name="matchingMemberProperty">The matching member property.</param>
        /// <param name="parameterInfo">The constructor parameter.</param>
        /// <returns>A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.ParameterInfo" />.</returns>
        protected virtual JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
        {
            JsonProperty jsonProperty = new JsonProperty();
            jsonProperty.PropertyType = parameterInfo.ParameterType;
            jsonProperty.AttributeProvider = new ReflectionAttributeProvider(parameterInfo);
            SetPropertySettingsFromAttributes(jsonProperty, parameterInfo, parameterInfo.Name, parameterInfo.Member.DeclaringType, MemberSerialization.OptOut, out bool flag);
            jsonProperty.Readable = false;
            jsonProperty.Writable = true;
            if (matchingMemberProperty != null)
            {
                jsonProperty.PropertyName = ((jsonProperty.PropertyName != parameterInfo.Name) ? jsonProperty.PropertyName : matchingMemberProperty.PropertyName);
                jsonProperty.Converter = (jsonProperty.Converter ?? matchingMemberProperty.Converter);
                jsonProperty.MemberConverter = (jsonProperty.MemberConverter ?? matchingMemberProperty.MemberConverter);
                if (!jsonProperty._hasExplicitDefaultValue && matchingMemberProperty._hasExplicitDefaultValue)
                {
                    jsonProperty.DefaultValue = matchingMemberProperty.DefaultValue;
                }
                JsonProperty jsonProperty2 = jsonProperty;
                Required? required = jsonProperty._required;
                jsonProperty2._required = ((required != null) ? required : matchingMemberProperty._required);
                JsonProperty jsonProperty3 = jsonProperty;
                bool? isReference = jsonProperty.IsReference;
                jsonProperty3.IsReference = ((isReference != null) ? isReference : matchingMemberProperty.IsReference);
                JsonProperty jsonProperty4 = jsonProperty;
                NullValueHandling? nullValueHandling = jsonProperty.NullValueHandling;
                jsonProperty4.NullValueHandling = ((nullValueHandling != null) ? nullValueHandling : matchingMemberProperty.NullValueHandling);
                JsonProperty jsonProperty5 = jsonProperty;
                DefaultValueHandling? defaultValueHandling = jsonProperty.DefaultValueHandling;
                jsonProperty5.DefaultValueHandling = ((defaultValueHandling != null) ? defaultValueHandling : matchingMemberProperty.DefaultValueHandling);
                JsonProperty jsonProperty6 = jsonProperty;
                ReferenceLoopHandling? referenceLoopHandling = jsonProperty.ReferenceLoopHandling;
                jsonProperty6.ReferenceLoopHandling = ((referenceLoopHandling != null) ? referenceLoopHandling : matchingMemberProperty.ReferenceLoopHandling);
                JsonProperty jsonProperty7 = jsonProperty;
                ObjectCreationHandling? objectCreationHandling = jsonProperty.ObjectCreationHandling;
                jsonProperty7.ObjectCreationHandling = ((objectCreationHandling != null) ? objectCreationHandling : matchingMemberProperty.ObjectCreationHandling);
                JsonProperty jsonProperty8 = jsonProperty;
                TypeNameHandling? typeNameHandling = jsonProperty.TypeNameHandling;
                jsonProperty8.TypeNameHandling = ((typeNameHandling != null) ? typeNameHandling : matchingMemberProperty.TypeNameHandling);
            }
            return jsonProperty;
        }

        /// <summary>
        /// Resolves the default <see cref="T:Newtonsoft.Json.JsonConverter" /> for the contract.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>The contract's default <see cref="T:Newtonsoft.Json.JsonConverter" />.</returns>
        protected virtual JsonConverter ResolveContractConverter(Type objectType)
        {
            return JsonTypeReflector.GetJsonConverter(objectType);
        }

        private Func<object> GetDefaultCreator(Type createdType)
        {
            return JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(createdType);
        }

        private void InitializeContract(JsonContract contract)
        {
            JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(contract.NonNullableUnderlyingType);
            if (cachedAttribute != null)
            {
                contract.IsReference = cachedAttribute._isReference;
            }
            else
            {
                DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(contract.NonNullableUnderlyingType);
                if (dataContractAttribute != null && dataContractAttribute.IsReference)
                {
                    contract.IsReference = new bool?(true);
                }
            }
            contract.Converter = ResolveContractConverter(contract.NonNullableUnderlyingType);
            contract.InternalConverter = JsonSerializer.GetMatchingConverter(DefaultContractResolver.BuiltInConverters, contract.NonNullableUnderlyingType);
            if (contract.IsInstantiable && (ReflectionUtils.HasDefaultConstructor(contract.CreatedType, true) || contract.CreatedType.IsValueType()))
            {
                contract.DefaultCreator = GetDefaultCreator(contract.CreatedType);
                contract.DefaultCreatorNonPublic = (!contract.CreatedType.IsValueType() && ReflectionUtils.GetDefaultConstructor(contract.CreatedType) == null);
            }
            ResolveCallbackMethods(contract, contract.NonNullableUnderlyingType);
        }

        private void ResolveCallbackMethods(JsonContract contract, Type t)
        {
            GetCallbackMethodsForType(t, out List<SerializationCallback> list, out List<SerializationCallback> list2, out List<SerializationCallback> list3, out List<SerializationCallback> list4, out List<SerializationErrorCallback> list5);
            if (list != null)
            {
                contract.OnSerializingCallbacks.AddRange(list);
            }
            if (list2 != null)
            {
                contract.OnSerializedCallbacks.AddRange(list2);
            }
            if (list3 != null)
            {
                contract.OnDeserializingCallbacks.AddRange(list3);
            }
            if (list4 != null)
            {
                contract.OnDeserializedCallbacks.AddRange(list4);
            }
            if (list5 != null)
            {
                contract.OnErrorCallbacks.AddRange(list5);
            }
        }

        private void GetCallbackMethodsForType(Type type, out List<SerializationCallback> onSerializing, out List<SerializationCallback> onSerialized, out List<SerializationCallback> onDeserializing, out List<SerializationCallback> onDeserialized, out List<SerializationErrorCallback> onError)
        {
            onSerializing = null;
            onSerialized = null;
            onDeserializing = null;
            onDeserialized = null;
            onError = null;
            foreach (Type type2 in GetClassHierarchyForType(type))
            {
                MethodInfo currentCallback = null;
                MethodInfo currentCallback2 = null;
                MethodInfo currentCallback3 = null;
                MethodInfo currentCallback4 = null;
                MethodInfo currentCallback5 = null;
                bool flag = DefaultContractResolver.ShouldSkipSerializing(type2);
                bool flag2 = DefaultContractResolver.ShouldSkipDeserialized(type2);
                foreach (MethodInfo methodInfo in type2.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!methodInfo.ContainsGenericParameters)
                    {
                        Type type3 = null;
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        if (!flag && DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnSerializingAttribute), currentCallback, ref type3))
                        {
                            onSerializing = (onSerializing ?? new List<SerializationCallback>());
                            onSerializing.Add(JsonContract.CreateSerializationCallback(methodInfo));
                            currentCallback = methodInfo;
                        }
                        if (DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnSerializedAttribute), currentCallback2, ref type3))
                        {
                            onSerialized = (onSerialized ?? new List<SerializationCallback>());
                            onSerialized.Add(JsonContract.CreateSerializationCallback(methodInfo));
                            currentCallback2 = methodInfo;
                        }
                        if (DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnDeserializingAttribute), currentCallback3, ref type3))
                        {
                            onDeserializing = (onDeserializing ?? new List<SerializationCallback>());
                            onDeserializing.Add(JsonContract.CreateSerializationCallback(methodInfo));
                            currentCallback3 = methodInfo;
                        }
                        if (!flag2 && DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnDeserializedAttribute), currentCallback4, ref type3))
                        {
                            onDeserialized = (onDeserialized ?? new List<SerializationCallback>());
                            onDeserialized.Add(JsonContract.CreateSerializationCallback(methodInfo));
                            currentCallback4 = methodInfo;
                        }
                        if (DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnErrorAttribute), currentCallback5, ref type3))
                        {
                            onError = (onError ?? new List<SerializationErrorCallback>());
                            onError.Add(JsonContract.CreateSerializationErrorCallback(methodInfo));
                            currentCallback5 = methodInfo;
                        }
                    }
                }
            }
        }

        private static bool ShouldSkipDeserialized(Type t)
        {
            return false;
        }

        private static bool ShouldSkipSerializing(Type t)
        {
            return false;
        }

        private List<Type> GetClassHierarchyForType(Type type)
        {
            List<Type> list = new List<Type>();
            Type type2 = type;
            while (type2 != null && type2 != typeof(object))
            {
                list.Add(type2);
                type2 = type2.BaseType();
            }
            list.Reverse();
            return list;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonDictionaryContract" /> for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonDictionaryContract" /> for the given type.</returns>
        protected virtual JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            JsonDictionaryContract jsonDictionaryContract = new JsonDictionaryContract(objectType);
            InitializeContract(jsonDictionaryContract);
            JsonContainerAttribute attribute = JsonTypeReflector.GetAttribute<JsonContainerAttribute>(objectType);
            if (((attribute != null) ? attribute.NamingStrategyType : null) != null)
            {
                NamingStrategy namingStrategy = JsonTypeReflector.GetContainerNamingStrategy(attribute);
                jsonDictionaryContract.DictionaryKeyResolver = ((string s) => namingStrategy.GetDictionaryKey(s));
            }
            else
            {
                jsonDictionaryContract.DictionaryKeyResolver = new Func<string, string>(ResolveDictionaryKey);
            }
            ConstructorInfo attributeConstructor = GetAttributeConstructor(jsonDictionaryContract.NonNullableUnderlyingType);
            if (attributeConstructor != null)
            {
                ParameterInfo[] parameters = attributeConstructor.GetParameters();
                Type type = (jsonDictionaryContract.DictionaryKeyType != null && jsonDictionaryContract.DictionaryValueType != null) ? typeof(IEnumerable<>).MakeGenericType(new Type[]
                {
                    typeof(KeyValuePair<, >).MakeGenericType(new Type[]
                    {
                        jsonDictionaryContract.DictionaryKeyType,
                        jsonDictionaryContract.DictionaryValueType
                    })
                }) : typeof(IDictionary);
                if (parameters.Length == 0)
                {
                    jsonDictionaryContract.HasParameterizedCreator = false;
                }
                else
                {
                    if (parameters.Length != 1 || !type.IsAssignableFrom(parameters[0].ParameterType))
                    {
                        throw new JsonException("Constructor for '{0}' must have no parameters or a single parameter that implements '{1}'.".FormatWith(CultureInfo.InvariantCulture, jsonDictionaryContract.UnderlyingType, type));
                    }
                    jsonDictionaryContract.HasParameterizedCreator = true;
                }
                jsonDictionaryContract.OverrideCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(attributeConstructor);
            }
            return jsonDictionaryContract;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonArrayContract" /> for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonArrayContract" /> for the given type.</returns>
        protected virtual JsonArrayContract CreateArrayContract(Type objectType)
        {
            JsonArrayContract jsonArrayContract = new JsonArrayContract(objectType);
            InitializeContract(jsonArrayContract);
            ConstructorInfo attributeConstructor = GetAttributeConstructor(jsonArrayContract.NonNullableUnderlyingType);
            if (attributeConstructor != null)
            {
                ParameterInfo[] parameters = attributeConstructor.GetParameters();
                Type type = (jsonArrayContract.CollectionItemType != null) ? typeof(IEnumerable<>).MakeGenericType(new Type[]
                {
                    jsonArrayContract.CollectionItemType
                }) : typeof(IEnumerable);
                if (parameters.Length == 0)
                {
                    jsonArrayContract.HasParameterizedCreator = false;
                }
                else
                {
                    if (parameters.Length != 1 || !type.IsAssignableFrom(parameters[0].ParameterType))
                    {
                        throw new JsonException("Constructor for '{0}' must have no parameters or a single parameter that implements '{1}'.".FormatWith(CultureInfo.InvariantCulture, jsonArrayContract.UnderlyingType, type));
                    }
                    jsonArrayContract.HasParameterizedCreator = true;
                }
                jsonArrayContract.OverrideCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(attributeConstructor);
            }
            return jsonArrayContract;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonPrimitiveContract" /> for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonPrimitiveContract" /> for the given type.</returns>
        protected virtual JsonPrimitiveContract CreatePrimitiveContract(Type objectType)
        {
            JsonPrimitiveContract jsonPrimitiveContract = new JsonPrimitiveContract(objectType);
            InitializeContract(jsonPrimitiveContract);
            return jsonPrimitiveContract;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonLinqContract" /> for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonLinqContract" /> for the given type.</returns>
        protected virtual JsonLinqContract CreateLinqContract(Type objectType)
        {
            JsonLinqContract jsonLinqContract = new JsonLinqContract(objectType);
            InitializeContract(jsonLinqContract);
            return jsonLinqContract;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonISerializableContract" /> for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonISerializableContract" /> for the given type.</returns>
        protected virtual JsonISerializableContract CreateISerializableContract(Type objectType)
        {
            JsonISerializableContract jsonISerializableContract = new JsonISerializableContract(objectType);
            InitializeContract(jsonISerializableContract);
            ConstructorInfo constructor = jsonISerializableContract.NonNullableUnderlyingType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
            {
                typeof(SerializationInfo),
                typeof(StreamingContext)
            }, null);
            if (constructor != null)
            {
                ObjectConstructor<object> iserializableCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
                jsonISerializableContract.ISerializableCreator = iserializableCreator;
            }
            return jsonISerializableContract;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonStringContract" /> for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonStringContract" /> for the given type.</returns>
        protected virtual JsonStringContract CreateStringContract(Type objectType)
        {
            JsonStringContract jsonStringContract = new JsonStringContract(objectType);
            InitializeContract(jsonStringContract);
            return jsonStringContract;
        }

        /// <summary>
        /// Determines which contract type is created for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Serialization.JsonContract" /> for the given type.</returns>
        protected virtual JsonContract CreateContract(Type objectType)
        {
            if (DefaultContractResolver.IsJsonPrimitiveType(objectType))
            {
                return CreatePrimitiveContract(objectType);
            }
            Type type = ReflectionUtils.EnsureNotNullableType(objectType);
            JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);
            if (cachedAttribute is JsonObjectAttribute)
            {
                return CreateObjectContract(objectType);
            }
            if (cachedAttribute is JsonArrayAttribute)
            {
                return CreateArrayContract(objectType);
            }
            if (cachedAttribute is JsonDictionaryAttribute)
            {
                return CreateDictionaryContract(objectType);
            }
            if (type == typeof(JToken) || type.IsSubclassOf(typeof(JToken)))
            {
                return CreateLinqContract(objectType);
            }
            if (CollectionUtils.IsDictionaryType(type))
            {
                return CreateDictionaryContract(objectType);
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return CreateArrayContract(objectType);
            }
            if (DefaultContractResolver.CanConvertToString(type))
            {
                return CreateStringContract(objectType);
            }
            if (!IgnoreSerializableInterface && typeof(ISerializable).IsAssignableFrom(type))
            {
                return CreateISerializableContract(objectType);
            }
            if (DefaultContractResolver.IsIConvertible(type))
            {
                return CreatePrimitiveContract(type);
            }
            return CreateObjectContract(objectType);
        }

        internal static bool IsJsonPrimitiveType(Type t)
        {
            PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(t);
            return typeCode != PrimitiveTypeCode.Empty && typeCode != PrimitiveTypeCode.Object;
        }

        internal static bool IsIConvertible(Type t)
        {
            return (typeof(IConvertible).IsAssignableFrom(t) || (ReflectionUtils.IsNullableType(t) && typeof(IConvertible).IsAssignableFrom(Nullable.GetUnderlyingType(t)))) && !typeof(JToken).IsAssignableFrom(t);
        }

        internal static bool CanConvertToString(Type type)
        {
            TypeConverter converter = ConvertUtils.GetConverter(type);
            return (converter != null && !(converter is ComponentConverter) && !(converter is ReferenceConverter) && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string))) || (type == typeof(Type) || type.IsSubclassOf(typeof(Type)));
        }

        private static bool IsValidCallback(MethodInfo method, ParameterInfo[] parameters, Type attributeType, MethodInfo currentCallback, ref Type prevAttributeType)
        {
            if (!method.IsDefined(attributeType, false))
            {
                return false;
            }
            if (currentCallback != null)
            {
                throw new JsonException("Invalid attribute. Both '{0}' and '{1}' in type '{2}' have '{3}'.".FormatWith(CultureInfo.InvariantCulture, method, currentCallback, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), attributeType));
            }
            if (prevAttributeType != null)
            {
                throw new JsonException("Invalid Callback. Method '{3}' in type '{2}' has both '{0}' and '{1}'.".FormatWith(CultureInfo.InvariantCulture, prevAttributeType, attributeType, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method));
            }
            if (method.IsVirtual)
            {
                throw new JsonException("Virtual Method '{0}' of type '{1}' cannot be marked with '{2}' attribute.".FormatWith(CultureInfo.InvariantCulture, method, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), attributeType));
            }
            if (method.ReturnType != typeof(void))
            {
                throw new JsonException("Serialization Callback '{1}' in type '{0}' must return void.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method));
            }
            if (attributeType == typeof(OnErrorAttribute))
            {
                if (parameters == null || parameters.Length != 2 || parameters[0].ParameterType != typeof(StreamingContext) || parameters[1].ParameterType != typeof(ErrorContext))
                {
                    throw new JsonException("Serialization Error Callback '{1}' in type '{0}' must have two parameters of type '{2}' and '{3}'.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext), typeof(ErrorContext)));
                }
            }
            else if (parameters == null || parameters.Length != 1 || parameters[0].ParameterType != typeof(StreamingContext))
            {
                throw new JsonException("Serialization Callback '{1}' in type '{0}' must have a single parameter of type '{2}'.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext)));
            }
            prevAttributeType = attributeType;
            return true;
        }

        internal static string GetClrTypeFullName(Type type)
        {
            if (type.IsGenericTypeDefinition() || !type.ContainsGenericParameters())
            {
                return type.FullName;
            }
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
            {
                type.Namespace,
                type.Name
            });
        }

        /// <summary>
        /// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.
        /// </summary>
        /// <param name="type">The type to create properties for.</param>
        /// /// <param name="memberSerialization">The member serialization mode for the type.</param>
        /// <returns>Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.</returns>
        protected virtual IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<MemberInfo> serializableMembers = GetSerializableMembers(type);
            if (serializableMembers == null)
            {
                throw new JsonSerializationException("Null collection of seralizable members returned.");
            }
            JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(type);
            foreach (MemberInfo member in serializableMembers)
            {
                JsonProperty jsonProperty = CreateProperty(member, memberSerialization);
                if (jsonProperty != null)
                {
                    DefaultContractResolverState state = GetState();
                    PropertyNameTable nameTable = state.NameTable;
                    lock (nameTable)
                    {
                        jsonProperty.PropertyName = state.NameTable.Add(jsonProperty.PropertyName);
                    }
                    jsonPropertyCollection.AddProperty(jsonProperty);
                }
            }
            return jsonPropertyCollection.OrderBy(delegate (JsonProperty p)
            {
                int? order = p.Order;
                if (order == null)
                {
                    return -1;
                }
                return order.GetValueOrDefault();
            }).ToList<JsonProperty>();
        }

        /// <summary>
        /// Creates the <see cref="T:Newtonsoft.Json.Serialization.IValueProvider" /> used by the serializer to get and set values from a member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Serialization.IValueProvider" /> used by the serializer to get and set values from a member.</returns>
        protected virtual IValueProvider CreateMemberValueProvider(MemberInfo member)
        {
            return new ReflectionValueProvider(member);
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.
        /// </summary>
        /// <param name="memberSerialization">The member's parent <see cref="T:Newtonsoft.Json.MemberSerialization" />.</param>
        /// <param name="member">The member to create a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for.</param>
        /// <returns>A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.</returns>
        protected virtual JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jsonProperty = new JsonProperty();
            jsonProperty.PropertyType = ReflectionUtils.GetMemberUnderlyingType(member);
            jsonProperty.DeclaringType = member.DeclaringType;
            jsonProperty.ValueProvider = CreateMemberValueProvider(member);
            jsonProperty.AttributeProvider = new ReflectionAttributeProvider(member);
            SetPropertySettingsFromAttributes(jsonProperty, member, member.Name, member.DeclaringType, memberSerialization, out bool flag);
            if (memberSerialization != MemberSerialization.Fields)
            {
                jsonProperty.Readable = ReflectionUtils.CanReadMemberValue(member, flag);
                jsonProperty.Writable = ReflectionUtils.CanSetMemberValue(member, flag, jsonProperty.HasMemberAttribute);
            }
            else
            {
                jsonProperty.Readable = true;
                jsonProperty.Writable = true;
            }
            jsonProperty.ShouldSerialize = CreateShouldSerializeTest(member);
            SetIsSpecifiedActions(jsonProperty, member, flag);
            return jsonProperty;
        }

        private void SetPropertySettingsFromAttributes(JsonProperty property, object attributeProvider, string name, Type declaringType, MemberSerialization memberSerialization, out bool allowNonPublicAccess)
        {
            bool dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(declaringType) != null;
            MemberInfo memberInfo = attributeProvider as MemberInfo;
            DataMemberAttribute dataMemberAttribute;
            if (dataContractAttribute && memberInfo != null)
            {
                dataMemberAttribute = JsonTypeReflector.GetDataMemberAttribute(memberInfo);
            }
            else
            {
                dataMemberAttribute = null;
            }
            JsonPropertyAttribute attribute = JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(attributeProvider);
            bool attribute2 = JsonTypeReflector.GetAttribute<JsonRequiredAttribute>(attributeProvider) != null;
            string text;
            bool hasSpecifiedName;
            if (attribute != null && attribute.PropertyName != null)
            {
                text = attribute.PropertyName;
                hasSpecifiedName = true;
            }
            else if (dataMemberAttribute != null && dataMemberAttribute.Name != null)
            {
                text = dataMemberAttribute.Name;
                hasSpecifiedName = true;
            }
            else
            {
                text = name;
                hasSpecifiedName = false;
            }
            JsonContainerAttribute attribute3 = JsonTypeReflector.GetAttribute<JsonContainerAttribute>(declaringType);
            NamingStrategy namingStrategy;
            if (((attribute != null) ? attribute.NamingStrategyType : null) != null)
            {
                namingStrategy = JsonTypeReflector.CreateNamingStrategyInstance(attribute.NamingStrategyType, attribute.NamingStrategyParameters);
            }
            else if (((attribute3 != null) ? attribute3.NamingStrategyType : null) != null)
            {
                namingStrategy = JsonTypeReflector.GetContainerNamingStrategy(attribute3);
            }
            else
            {
                namingStrategy = NamingStrategy;
            }
            if (namingStrategy != null)
            {
                property.PropertyName = namingStrategy.GetPropertyName(text, hasSpecifiedName);
            }
            else
            {
                property.PropertyName = ResolvePropertyName(text);
            }
            property.UnderlyingName = name;
            bool flag = false;
            if (attribute != null)
            {
                property._required = attribute._required;
                property.Order = attribute._order;
                property.DefaultValueHandling = attribute._defaultValueHandling;
                flag = true;
            }
            else if (dataMemberAttribute != null)
            {
                property._required = new Required?(dataMemberAttribute.IsRequired ? Required.AllowNull : Required.Default);
                property.Order = ((dataMemberAttribute.Order != -1) ? new int?(dataMemberAttribute.Order) : null);
                property.DefaultValueHandling = ((!dataMemberAttribute.EmitDefaultValue) ? new DefaultValueHandling?(DefaultValueHandling.Ignore) : null);
                flag = true;
            }
            if (attribute2)
            {
                property._required = new Required?(Required.Always);
                flag = true;
            }
            property.HasMemberAttribute = flag;
            bool flag2 = JsonTypeReflector.GetAttribute<JsonIgnoreAttribute>(attributeProvider) != null || JsonTypeReflector.GetAttribute<JsonExtensionDataAttribute>(attributeProvider) != null || JsonTypeReflector.GetAttribute<NonSerializedAttribute>(attributeProvider) != null;
            if (memberSerialization != MemberSerialization.OptIn)
            {
                bool flag3 = false;
                property.Ignored = (flag2 || flag3);
            }
            else
            {
                property.Ignored = (flag2 || !flag);
            }
            property.Converter = JsonTypeReflector.GetJsonConverter(attributeProvider);
            property.MemberConverter = JsonTypeReflector.GetJsonConverter(attributeProvider);
            DefaultValueAttribute attribute4 = JsonTypeReflector.GetAttribute<DefaultValueAttribute>(attributeProvider);
            if (attribute4 != null)
            {
                property.DefaultValue = attribute4.Value;
            }
            property.NullValueHandling = ((attribute != null) ? attribute._nullValueHandling : null);
            property.ReferenceLoopHandling = ((attribute != null) ? attribute._referenceLoopHandling : null);
            property.ObjectCreationHandling = ((attribute != null) ? attribute._objectCreationHandling : null);
            property.TypeNameHandling = ((attribute != null) ? attribute._typeNameHandling : null);
            property.IsReference = ((attribute != null) ? attribute._isReference : null);
            property.ItemIsReference = ((attribute != null) ? attribute._itemIsReference : null);
            property.ItemConverter = ((attribute != null && attribute.ItemConverterType != null) ? JsonTypeReflector.CreateJsonConverterInstance(attribute.ItemConverterType, attribute.ItemConverterParameters) : null);
            property.ItemReferenceLoopHandling = ((attribute != null) ? attribute._itemReferenceLoopHandling : null);
            property.ItemTypeNameHandling = (attribute != null) ? attribute._itemTypeNameHandling : null;
            allowNonPublicAccess = false;
            if ((DefaultMembersSearchFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic)
            {
                allowNonPublicAccess = true;
            }
            if (flag)
            {
                allowNonPublicAccess = true;
            }
            if (memberSerialization == MemberSerialization.Fields)
            {
                allowNonPublicAccess = true;
            }
        }

        private Predicate<object> CreateShouldSerializeTest(MemberInfo member)
        {
            MethodInfo method = member.DeclaringType.GetMethod("ShouldSerialize" + member.Name, ReflectionUtils.EmptyTypes);
            if (method == null || method.ReturnType != typeof(bool))
            {
                return null;
            }
            MethodCall<object, object> shouldSerializeCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
            return (object o) => (bool)shouldSerializeCall(o, new object[0]);
        }

        private void SetIsSpecifiedActions(JsonProperty property, MemberInfo member, bool allowNonPublicAccess)
        {
            MemberInfo memberInfo = member.DeclaringType.GetProperty(member.Name + "Specified");
            if (memberInfo == null)
            {
                memberInfo = member.DeclaringType.GetField(member.Name + "Specified");
            }
            if (memberInfo == null || ReflectionUtils.GetMemberUnderlyingType(memberInfo) != typeof(bool))
            {
                return;
            }
            Func<object, object> specifiedPropertyGet = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(memberInfo);
            property.GetIsSpecified = ((object o) => (bool)specifiedPropertyGet(o));
            if (ReflectionUtils.CanSetMemberValue(memberInfo, allowNonPublicAccess, false))
            {
                property.SetIsSpecified = JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(memberInfo);
            }
        }

        /// <summary>
        /// Resolves the name of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Resolved name of the property.</returns>
        protected virtual string ResolvePropertyName(string propertyName)
        {
            if (NamingStrategy != null)
            {
                return NamingStrategy.GetPropertyName(propertyName, false);
            }
            return propertyName;
        }

        /// <summary>
        /// Resolves the key of the dictionary. By default <see cref="M:Newtonsoft.Json.Serialization.DefaultContractResolver.ResolvePropertyName(System.String)" /> is used to resolve dictionary keys.
        /// </summary>
        /// <param name="dictionaryKey">Key of the dictionary.</param>
        /// <returns>Resolved key of the dictionary.</returns>
        protected virtual string ResolveDictionaryKey(string dictionaryKey)
        {
            if (NamingStrategy != null)
            {
                return NamingStrategy.GetDictionaryKey(dictionaryKey);
            }
            return ResolvePropertyName(dictionaryKey);
        }

        /// <summary>
        /// Gets the resolved name of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Name of the property.</returns>
        public string GetResolvedPropertyName(string propertyName)
        {
            return ResolvePropertyName(propertyName);
        }

        private static readonly IContractResolver _instance = new DefaultContractResolver(true);

        private static readonly JsonConverter[] BuiltInConverters = new JsonConverter[]
        {
            new BinaryConverter(),
            new KeyValuePairConverter(),
            new BsonObjectIdConverter(),
            new RegexConverter()
        };

        private static readonly object TypeContractCacheLock = new object();

        private static readonly DefaultContractResolverState _sharedState = new DefaultContractResolverState();

        private readonly DefaultContractResolverState _instanceState = new DefaultContractResolverState();

        private readonly bool _sharedCache;

        internal class EnumerableDictionaryWrapper<TEnumeratorKey, TEnumeratorValue> : IEnumerable<KeyValuePair<object, object>>, IEnumerable
        {
            public EnumerableDictionaryWrapper(IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
            {
                ValidationUtils.ArgumentNotNull(e, "e");
                _e = e;
            }

            public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
            {
                foreach (KeyValuePair<TEnumeratorKey, TEnumeratorValue> keyValuePair in _e)
                {
                    yield return new KeyValuePair<object, object>(keyValuePair.Key, keyValuePair.Value);
                }
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private readonly IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;
        }
    }
}
