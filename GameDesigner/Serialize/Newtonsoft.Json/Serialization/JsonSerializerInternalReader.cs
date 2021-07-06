using Newtonsoft_X.Json.Linq;
using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Newtonsoft_X.Json.Serialization
{
    internal class JsonSerializerInternalReader : JsonSerializerInternalBase
    {
        public JsonSerializerInternalReader(JsonSerializer serializer) : base(serializer)
        {
        }

        public void Populate(JsonReader reader, object target)
        {
            ValidationUtils.ArgumentNotNull(target, "target");
            Type type = target.GetType();
            JsonContract jsonContract = Serializer._contractResolver.ResolveContract(type);
            if (!reader.MoveToContent())
            {
                throw JsonSerializationException.Create(reader, "No JSON content found.");
            }
            if (reader.TokenType == JsonToken.StartArray)
            {
                if (jsonContract.ContractType == JsonContractType.Array)
                {
                    JsonArrayContract jsonArrayContract = (JsonArrayContract)jsonContract;
                    IList list;
                    if (!jsonArrayContract.ShouldCreateWrapper)
                    {
                        list = (IList)target;
                    }
                    else
                    {
                        IList list2 = jsonArrayContract.CreateWrapper(target);
                        list = list2;
                    }
                    PopulateList(list, reader, jsonArrayContract, null, null);
                    return;
                }
                throw JsonSerializationException.Create(reader, "Cannot populate JSON array onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
            }
            else
            {
                if (reader.TokenType != JsonToken.StartObject)
                {
                    throw JsonSerializationException.Create(reader, "Unexpected initial token '{0}' when populating object. Expected JSON object or array.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
                reader.ReadAndAssert();
                string id = null;
                if (Serializer.MetadataPropertyHandling != MetadataPropertyHandling.Ignore && reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$id", StringComparison.Ordinal))
                {
                    reader.ReadAndAssert();
                    id = ((reader.Value != null) ? reader.Value.ToString() : null);
                    reader.ReadAndAssert();
                }
                if (jsonContract.ContractType == JsonContractType.Dictionary)
                {
                    JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)jsonContract;
                    IDictionary dictionary;
                    if (!jsonDictionaryContract.ShouldCreateWrapper)
                    {
                        dictionary = (IDictionary)target;
                    }
                    else
                    {
                        IDictionary dictionary2 = jsonDictionaryContract.CreateWrapper(target);
                        dictionary = dictionary2;
                    }
                    PopulateDictionary(dictionary, reader, jsonDictionaryContract, null, id);
                    return;
                }
                if (jsonContract.ContractType == JsonContractType.Object)
                {
                    PopulateObject(target, reader, (JsonObjectContract)jsonContract, null, id);
                    return;
                }
                throw JsonSerializationException.Create(reader, "Cannot populate JSON object onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
            }
        }

        private JsonContract GetContractSafe(Type type)
        {
            if (type == null)
            {
                return null;
            }
            return Serializer._contractResolver.ResolveContract(type);
        }

        public object Deserialize(JsonReader reader, Type objectType, bool checkAdditionalContent)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            JsonContract contractSafe = GetContractSafe(objectType);
            object result;
            try
            {
                JsonConverter converter = GetConverter(contractSafe, null, null, null);
                if (reader.TokenType == JsonToken.None && !ReadForType(reader, contractSafe, converter != null))
                {
                    if (contractSafe != null && !contractSafe.IsNullable)
                    {
                        throw JsonSerializationException.Create(reader, "No JSON content found and type '{0}' is not nullable.".FormatWith(CultureInfo.InvariantCulture, contractSafe.UnderlyingType));
                    }
                    result = null;
                }
                else
                {
                    object obj;
                    if (converter != null && converter.CanRead)
                    {
                        obj = DeserializeConvertable(converter, reader, objectType, null);
                    }
                    else
                    {
                        obj = CreateValueInternal(reader, objectType, contractSafe, null, null, null, null);
                    }
                    if (checkAdditionalContent && reader.Read() && reader.TokenType != JsonToken.Comment)
                    {
                        throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
                    }
                    result = obj;
                }
            }
            catch (Exception ex)
            {
                if (!base.IsErrorHandled(null, contractSafe, null, reader as IJsonLineInfo, reader.Path, ex))
                {
                    base.ClearErrorContext();
                    throw;
                }
                HandleError(reader, false, 0);
                result = null;
            }
            return result;
        }

        private JsonSerializerProxy GetInternalSerializer()
        {
            if (InternalSerializer == null)
            {
                InternalSerializer = new JsonSerializerProxy(this);
            }
            return InternalSerializer;
        }

        private JToken CreateJToken(JsonReader reader, JsonContract contract)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            if (contract != null)
            {
                if (contract.UnderlyingType == typeof(JRaw))
                {
                    return JRaw.Create(reader);
                }
                if (reader.TokenType == JsonToken.Null && contract.UnderlyingType != typeof(JValue) && contract.UnderlyingType != typeof(JToken))
                {
                    return null;
                }
            }
            JToken token;
            using (JTokenWriter jtokenWriter = new JTokenWriter())
            {
                jtokenWriter.WriteToken(reader);
                token = jtokenWriter.Token;
            }
            return token;
        }

        private JToken CreateJObject(JsonReader reader)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            using (JTokenWriter jtokenWriter = new JTokenWriter())
            {
                jtokenWriter.WriteStartObject();
                for (; ; )
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string text = (string)reader.Value;
                        if (!reader.ReadAndMoveToContent())
                        {
                            goto IL_71;
                        }
                        if (!CheckPropertyName(reader, text))
                        {
                            jtokenWriter.WritePropertyName(text);
                            jtokenWriter.WriteToken(reader, true, true, false);
                        }
                    }
                    else if (reader.TokenType != JsonToken.Comment)
                    {
                        break;
                    }
                    if (!reader.Read())
                    {
                        goto IL_71;
                    }
                }
                jtokenWriter.WriteEndObject();
                return jtokenWriter.Token;
            IL_71:
                throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
            }
        }

        private object CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue)
        {
            if (contract != null && contract.ContractType == JsonContractType.Linq)
            {
                return CreateJToken(reader, contract);
            }
            for (; ; )
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        goto IL_6D;
                    case JsonToken.StartArray:
                        goto IL_7F;
                    case JsonToken.StartConstructor:
                        goto IL_DF;
                    case JsonToken.Comment:
                        if (!reader.Read())
                        {
                            goto Block_7;
                        }
                        continue;
                    case JsonToken.Raw:
                        goto IL_123;
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.Boolean:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                        goto IL_8E;
                    case JsonToken.String:
                        goto IL_A3;
                    case JsonToken.Null:
                    case JsonToken.Undefined:
                        goto IL_FB;
                }
                break;
            }
            goto IL_134;
        IL_6D:
            return CreateObject(reader, objectType, contract, member, containerContract, containerMember, existingValue);
        IL_7F:
            return CreateList(reader, objectType, contract, member, existingValue, null);
        IL_8E:
            return EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
        IL_A3:
            string text = (string)reader.Value;
            if (JsonSerializerInternalReader.CoerceEmptyStringToNull(objectType, contract, text))
            {
                return null;
            }
            if (objectType == typeof(byte[]))
            {
                return Convert.FromBase64String(text);
            }
            return EnsureType(reader, text, CultureInfo.InvariantCulture, contract, objectType);
        IL_DF:
            string value = reader.Value.ToString();
            return EnsureType(reader, value, CultureInfo.InvariantCulture, contract, objectType);
        IL_FB:
            if (objectType == typeof(DBNull))
            {
                return DBNull.Value;
            }
            return EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
        IL_123:
            return new JRaw((string)reader.Value);
        IL_134:
            throw JsonSerializationException.Create(reader, "Unexpected token while deserializing object: " + reader.TokenType);
        Block_7:
            throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
        }

        private static bool CoerceEmptyStringToNull(Type objectType, JsonContract contract, string s)
        {
            return string.IsNullOrEmpty(s) && objectType != null && objectType != typeof(string) && objectType != typeof(object) && contract != null && contract.IsNullable;
        }

        internal string GetExpectedDescription(JsonContract contract)
        {
            switch (contract.ContractType)
            {
                case JsonContractType.Object:
                case JsonContractType.Dictionary:
                case JsonContractType.Serializable:
                    return "JSON object (e.g. {\"name\":\"value\"})";
                case JsonContractType.Array:
                    return "JSON array (e.g. [1,2,3])";
                case JsonContractType.Primitive:
                    return "JSON primitive value (e.g. string, number, boolean, null)";
                case JsonContractType.String:
                    return "JSON string value";
            }
            throw new ArgumentOutOfRangeException();
        }

        private JsonConverter GetConverter(JsonContract contract, JsonConverter memberConverter, JsonContainerContract containerContract, JsonProperty containerProperty)
        {
            JsonConverter result = null;
            if (memberConverter != null)
            {
                result = memberConverter;
            }
            else if (containerProperty != null && containerProperty.ItemConverter != null)
            {
                result = containerProperty.ItemConverter;
            }
            else if (containerContract != null && containerContract.ItemConverter != null)
            {
                result = containerContract.ItemConverter;
            }
            else if (contract != null)
            {
                JsonConverter matchingConverter;
                if (contract.Converter != null)
                {
                    result = contract.Converter;
                }
                else if ((matchingConverter = Serializer.GetMatchingConverter(contract.UnderlyingType)) != null)
                {
                    result = matchingConverter;
                }
                else if (contract.InternalConverter != null)
                {
                    result = contract.InternalConverter;
                }
            }
            return result;
        }

        private object CreateObject(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue)
        {
            Type type = objectType;
            string text;
            if (Serializer.MetadataPropertyHandling == MetadataPropertyHandling.Ignore)
            {
                reader.ReadAndAssert();
                text = null;
            }
            else if (Serializer.MetadataPropertyHandling == MetadataPropertyHandling.ReadAhead)
            {
                JTokenReader jtokenReader = reader as JTokenReader;
                if (jtokenReader == null)
                {
                    jtokenReader = (JTokenReader)JToken.ReadFrom(reader).CreateReader();
                    jtokenReader.Culture = reader.Culture;
                    jtokenReader.DateFormatString = reader.DateFormatString;
                    jtokenReader.DateParseHandling = reader.DateParseHandling;
                    jtokenReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
                    jtokenReader.FloatParseHandling = reader.FloatParseHandling;
                    jtokenReader.SupportMultipleContent = reader.SupportMultipleContent;
                    jtokenReader.ReadAndAssert();
                    reader = jtokenReader;
                }
                if (ReadMetadataPropertiesToken(jtokenReader, ref type, ref contract, member, containerContract, containerMember, existingValue, out object result, out text))
                {
                    return result;
                }
            }
            else
            {
                reader.ReadAndAssert();
                if (ReadMetadataProperties(reader, ref type, ref contract, member, containerContract, containerMember, existingValue, out object result2, out text))
                {
                    return result2;
                }
            }
            if (HasNoDefinedType(contract))
            {
                return CreateJObject(reader);
            }
            switch (contract.ContractType)
            {
                case JsonContractType.Object:
                    {
                        bool flag = false;
                        JsonObjectContract jsonObjectContract = (JsonObjectContract)contract;
                        object obj;
                        if (existingValue != null && (type == objectType || type.IsAssignableFrom(existingValue.GetType())))
                        {
                            obj = existingValue;
                        }
                        else
                        {
                            obj = CreateNewObject(reader, jsonObjectContract, member, containerMember, text, out flag);
                        }
                        if (flag)
                        {
                            return obj;
                        }
                        return PopulateObject(obj, reader, jsonObjectContract, member, text);
                    }
                case JsonContractType.Primitive:
                    {
                        JsonPrimitiveContract contract2 = (JsonPrimitiveContract)contract;
                        if (Serializer.MetadataPropertyHandling != MetadataPropertyHandling.Ignore && reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$value", StringComparison.Ordinal))
                        {
                            reader.ReadAndAssert();
                            if (reader.TokenType == JsonToken.StartObject)
                            {
                                throw JsonSerializationException.Create(reader, "Unexpected token when deserializing primitive value: " + reader.TokenType);
                            }
                            object result3 = CreateValueInternal(reader, type, contract2, member, null, null, existingValue);
                            reader.ReadAndAssert();
                            return result3;
                        }
                        break;
                    }
                case JsonContractType.Dictionary:
                    {
                        JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)contract;
                        object result4;
                        if (existingValue == null)
                        {
                            IDictionary dictionary = CreateNewDictionary(reader, jsonDictionaryContract, out bool flag2);
                            if (flag2)
                            {
                                if (text != null)
                                {
                                    throw JsonSerializationException.Create(reader, "Cannot preserve reference to readonly dictionary, or dictionary created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
                                }
                                if (contract.OnSerializingCallbacks.Count > 0)
                                {
                                    throw JsonSerializationException.Create(reader, "Cannot call OnSerializing on readonly dictionary, or dictionary created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
                                }
                                if (contract.OnErrorCallbacks.Count > 0)
                                {
                                    throw JsonSerializationException.Create(reader, "Cannot call OnError on readonly list, or dictionary created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
                                }
                                if (!jsonDictionaryContract.HasParameterizedCreatorInternal)
                                {
                                    throw JsonSerializationException.Create(reader, "Cannot deserialize readonly or fixed size dictionary: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
                                }
                            }
                            PopulateDictionary(dictionary, reader, jsonDictionaryContract, member, text);
                            if (flag2)
                            {
                                return (jsonDictionaryContract.OverrideCreator ?? jsonDictionaryContract.ParameterizedCreator)(new object[]
                                {
                            dictionary
                                });
                            }
                            if (dictionary is IWrappedDictionary)
                            {
                                return ((IWrappedDictionary)dictionary).UnderlyingDictionary;
                            }
                            result4 = dictionary;
                        }
                        else
                        {
                            IDictionary dictionary2;
                            if (!jsonDictionaryContract.ShouldCreateWrapper)
                            {
                                dictionary2 = (IDictionary)existingValue;
                            }
                            else
                            {
                                IDictionary dictionary3 = jsonDictionaryContract.CreateWrapper(existingValue);
                                dictionary2 = dictionary3;
                            }
                            result4 = PopulateDictionary(dictionary2, reader, jsonDictionaryContract, member, text);
                        }
                        return result4;
                    }
                case JsonContractType.Serializable:
                    {
                        JsonISerializableContract contract3 = (JsonISerializableContract)contract;
                        return CreateISerializable(reader, contract3, member, text);
                    }
            }
            string text2 = "Cannot deserialize the current JSON object (e.g. {{\"name\":\"value\"}}) into type '{0}' because the type requires a {1} to deserialize correctly." + Environment.NewLine + "To fix this error either change the JSON to a {1} or change the deserialized type so that it is a normal .NET type (e.g. not a primitive type like integer, not a collection type like an array or List<T>) that can be deserialized from a JSON object. JsonObjectAttribute can also be added to the type to force it to deserialize from a JSON object." + Environment.NewLine;
            text2 = text2.FormatWith(CultureInfo.InvariantCulture, type, GetExpectedDescription(contract));
            throw JsonSerializationException.Create(reader, text2);
        }

        private bool ReadMetadataPropertiesToken(JTokenReader reader, ref Type objectType, ref JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue, out object newValue, out string id)
        {
            id = null;
            newValue = null;
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject jobject = (JObject)reader.CurrentToken;
                JToken jtoken = jobject["$ref"];
                if (jtoken != null)
                {
                    if (jtoken.Type != JTokenType.String && jtoken.Type != JTokenType.Null)
                    {
                        throw JsonSerializationException.Create(jtoken, jtoken.Path, "JSON reference {0} property must have a string or null value.".FormatWith(CultureInfo.InvariantCulture, "$ref"), null);
                    }
                    JToken parent = jtoken.Parent;
                    JToken jtoken2 = null;
                    if (parent.Next != null)
                    {
                        jtoken2 = parent.Next;
                    }
                    else if (parent.Previous != null)
                    {
                        jtoken2 = parent.Previous;
                    }
                    string text = (string)jtoken;
                    if (text != null)
                    {
                        if (jtoken2 != null)
                        {
                            throw JsonSerializationException.Create(jtoken2, jtoken2.Path, "Additional content found in JSON reference object. A JSON reference object should only have a {0} property.".FormatWith(CultureInfo.InvariantCulture, "$ref"), null);
                        }
                        newValue = Serializer.GetReferenceResolver().ResolveReference(this, text);
                        if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
                        {
                            TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader, reader.Path, "Resolved object reference '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, text, newValue.GetType())), null);
                        }
                        reader.Skip();
                        return true;
                    }
                }
                JToken jtoken3 = jobject["$type"];
                if (jtoken3 != null)
                {
                    string qualifiedTypeName = (string)jtoken3;
                    JsonReader jsonReader = jtoken3.CreateReader();
                    jsonReader.ReadAndAssert();
                    ResolveTypeName(jsonReader, ref objectType, ref contract, member, containerContract, containerMember, qualifiedTypeName);
                    if (jobject["$value"] != null)
                    {
                        for (; ; )
                        {
                            reader.ReadAndAssert();
                            if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "$value")
                            {
                                break;
                            }
                            reader.ReadAndAssert();
                            reader.Skip();
                        }
                        return false;
                    }
                }
                JToken jtoken4 = jobject["$id"];
                if (jtoken4 != null)
                {
                    id = (string)jtoken4;
                }
                JToken jtoken5 = jobject["$values"];
                if (jtoken5 != null)
                {
                    JsonReader jsonReader2 = jtoken5.CreateReader();
                    jsonReader2.ReadAndAssert();
                    newValue = CreateList(jsonReader2, objectType, contract, member, existingValue, id);
                    reader.Skip();
                    return true;
                }
            }
            reader.ReadAndAssert();
            return false;
        }

        private bool ReadMetadataProperties(JsonReader reader, ref Type objectType, ref JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue, out object newValue, out string id)
        {
            id = null;
            newValue = null;
            if (reader.TokenType == JsonToken.PropertyName)
            {
                string text = reader.Value.ToString();
                if (text.Length > 0 && text[0] == '$')
                {
                    string text2;
                    for (; ; )
                    {
                        text = reader.Value.ToString();
                        bool flag;
                        if (string.Equals(text, "$ref", StringComparison.Ordinal))
                        {
                            reader.ReadAndAssert();
                            if (reader.TokenType != JsonToken.String && reader.TokenType != JsonToken.Null)
                            {
                                break;
                            }
                            text2 = ((reader.Value != null) ? reader.Value.ToString() : null);
                            reader.ReadAndAssert();
                            if (text2 != null)
                            {
                                goto Block_7;
                            }
                            flag = true;
                        }
                        else if (string.Equals(text, "$type", StringComparison.Ordinal))
                        {
                            reader.ReadAndAssert();
                            string qualifiedTypeName = reader.Value.ToString();
                            ResolveTypeName(reader, ref objectType, ref contract, member, containerContract, containerMember, qualifiedTypeName);
                            reader.ReadAndAssert();
                            flag = true;
                        }
                        else if (string.Equals(text, "$id", StringComparison.Ordinal))
                        {
                            reader.ReadAndAssert();
                            id = ((reader.Value != null) ? reader.Value.ToString() : null);
                            reader.ReadAndAssert();
                            flag = true;
                        }
                        else
                        {
                            if (string.Equals(text, "$values", StringComparison.Ordinal))
                            {
                                goto Block_14;
                            }
                            flag = false;
                        }
                        if (!flag || reader.TokenType != JsonToken.PropertyName)
                        {
                            return false;
                        }
                    }
                    throw JsonSerializationException.Create(reader, "JSON reference {0} property must have a string or null value.".FormatWith(CultureInfo.InvariantCulture, "$ref"));
                Block_7:
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        throw JsonSerializationException.Create(reader, "Additional content found in JSON reference object. A JSON reference object should only have a {0} property.".FormatWith(CultureInfo.InvariantCulture, "$ref"));
                    }
                    newValue = Serializer.GetReferenceResolver().ResolveReference(this, text2);
                    if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
                    {
                        TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Resolved object reference '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, text2, newValue.GetType())), null);
                    }
                    return true;
                Block_14:
                    reader.ReadAndAssert();
                    object obj = CreateList(reader, objectType, contract, member, existingValue, id);
                    reader.ReadAndAssert();
                    newValue = obj;
                    return true;
                }
            }
            return false;
        }

        private void ResolveTypeName(JsonReader reader, ref Type objectType, ref JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, string qualifiedTypeName)
        {
            if ((((member != null) ? member.TypeNameHandling : null) ?? (((containerContract != null) ? containerContract.ItemTypeNameHandling : null) ?? (((containerMember != null) ? containerMember.ItemTypeNameHandling : null) ?? Serializer._typeNameHandling))) != TypeNameHandling.None)
            {
                ReflectionUtils.SplitFullyQualifiedTypeName(qualifiedTypeName, out string typeName, out string assemblyName);
                Type type;
                try
                {
                    type = Serializer._binder.BindToType(assemblyName, typeName);
                }
                catch (Exception ex)
                {
                    throw JsonSerializationException.Create(reader, "Error resolving type specified in JSON '{0}'.".FormatWith(CultureInfo.InvariantCulture, qualifiedTypeName), ex);
                }
                if (type == null)
                {
                    throw JsonSerializationException.Create(reader, "Type specified in JSON '{0}' was not resolved.".FormatWith(CultureInfo.InvariantCulture, qualifiedTypeName));
                }
                if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
                {
                    TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Resolved type '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, qualifiedTypeName, type)), null);
                }
                if (objectType != null && !objectType.IsAssignableFrom(type))
                {
                    throw JsonSerializationException.Create(reader, "Type specified in JSON '{0}' is not compatible with '{1}'.".FormatWith(CultureInfo.InvariantCulture, type.AssemblyQualifiedName, objectType.AssemblyQualifiedName));
                }
                objectType = type;
                contract = GetContractSafe(type);
            }
        }

        private JsonArrayContract EnsureArrayContract(JsonReader reader, Type objectType, JsonContract contract)
        {
            if (contract == null)
            {
                throw JsonSerializationException.Create(reader, "Could not resolve type '{0}' to a JsonContract.".FormatWith(CultureInfo.InvariantCulture, objectType));
            }
            JsonArrayContract jsonArrayContract = contract as JsonArrayContract;
            if (jsonArrayContract == null)
            {
                string text = "Cannot deserialize the current JSON array (e.g. [1,2,3]) into type '{0}' because the type requires a {1} to deserialize correctly." + Environment.NewLine + "To fix this error either change the JSON to a {1} or change the deserialized type to an array or a type that implements a collection interface (e.g. ICollection, IList) like List<T> that can be deserialized from a JSON array. JsonArrayAttribute can also be added to the type to force it to deserialize from a JSON array." + Environment.NewLine;
                text = text.FormatWith(CultureInfo.InvariantCulture, objectType, GetExpectedDescription(contract));
                throw JsonSerializationException.Create(reader, text);
            }
            return jsonArrayContract;
        }

        private object CreateList(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue, string id)
        {
            if (HasNoDefinedType(contract))
            {
                return CreateJToken(reader, contract);
            }
            JsonArrayContract jsonArrayContract = EnsureArrayContract(reader, objectType, contract);
            object result;
            if (existingValue == null)
            {
                IList list = CreateNewList(reader, jsonArrayContract, out bool flag);
                if (flag)
                {
                    if (id != null)
                    {
                        throw JsonSerializationException.Create(reader, "Cannot preserve reference to array or readonly list, or list created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
                    }
                    if (contract.OnSerializingCallbacks.Count > 0)
                    {
                        throw JsonSerializationException.Create(reader, "Cannot call OnSerializing on an array or readonly list, or list created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
                    }
                    if (contract.OnErrorCallbacks.Count > 0)
                    {
                        throw JsonSerializationException.Create(reader, "Cannot call OnError on an array or readonly list, or list created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
                    }
                    if (!jsonArrayContract.HasParameterizedCreatorInternal && !jsonArrayContract.IsArray)
                    {
                        throw JsonSerializationException.Create(reader, "Cannot deserialize readonly or fixed size list: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
                    }
                }
                if (!jsonArrayContract.IsMultidimensionalArray)
                {
                    PopulateList(list, reader, jsonArrayContract, member, id);
                }
                else
                {
                    PopulateMultidimensionalArray(list, reader, jsonArrayContract, member, id);
                }
                if (flag)
                {
                    if (jsonArrayContract.IsMultidimensionalArray)
                    {
                        list = CollectionUtils.ToMultidimensionalArray(list, jsonArrayContract.CollectionItemType, contract.CreatedType.GetArrayRank());
                    }
                    else
                    {
                        if (!jsonArrayContract.IsArray)
                        {
                            return (jsonArrayContract.OverrideCreator ?? jsonArrayContract.ParameterizedCreator)(new object[]
                            {
                                list
                            });
                        }
                        Array array = Array.CreateInstance(jsonArrayContract.CollectionItemType, list.Count);
                        list.CopyTo(array, 0);
                        list = array;
                    }
                }
                else if (list is IWrappedCollection)
                {
                    return ((IWrappedCollection)list).UnderlyingCollection;
                }
                result = list;
            }
            else
            {
                if (!jsonArrayContract.CanDeserialize)
                {
                    throw JsonSerializationException.Create(reader, "Cannot populate list type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.CreatedType));
                }
                IList list2;
                if (!jsonArrayContract.ShouldCreateWrapper)
                {
                    list2 = (IList)existingValue;
                }
                else
                {
                    IList list3 = jsonArrayContract.CreateWrapper(existingValue);
                    list2 = list3;
                }
                result = PopulateList(list2, reader, jsonArrayContract, member, id);
            }
            return result;
        }

        private bool HasNoDefinedType(JsonContract contract)
        {
            return contract == null || contract.UnderlyingType == typeof(object) || contract.ContractType == JsonContractType.Linq;
        }

        private object EnsureType(JsonReader reader, object value, CultureInfo culture, JsonContract contract, Type targetType)
        {
            if (targetType == null)
            {
                return value;
            }
            if (ReflectionUtils.GetObjectType(value) != targetType)
            {
                if (value == null && contract.IsNullable)
                {
                    return null;
                }
                try
                {
                    if (contract.IsConvertable)
                    {
                        JsonPrimitiveContract jsonPrimitiveContract = (JsonPrimitiveContract)contract;
                        if (contract.IsEnum)
                        {
                            if (value is string)
                            {
                                return Enum.Parse(contract.NonNullableUnderlyingType, value.ToString(), true);
                            }
                            if (ConvertUtils.IsInteger(jsonPrimitiveContract.TypeCode))
                            {
                                return Enum.ToObject(contract.NonNullableUnderlyingType, value);
                            }
                        }
                        return Convert.ChangeType(value, contract.NonNullableUnderlyingType, culture);
                    }
                    return ConvertUtils.ConvertOrCast(value, culture, contract.NonNullableUnderlyingType);
                }
                catch (Exception ex)
                {
                    throw JsonSerializationException.Create(reader, "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.FormatValueForPrint(value), targetType), ex);
                }
            }
            return value;
        }

        private bool SetPropertyValue(JsonProperty property, JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, object target)
        {
            if (CalculatePropertyDetails(property, ref propertyConverter, containerContract, containerProperty, reader, target, out bool flag, out object value, out JsonContract contract, out bool flag2))
            {
                return false;
            }
            object obj;
            if (propertyConverter != null && propertyConverter.CanRead)
            {
                if (!flag2 && target != null && property.Readable)
                {
                    value = property.ValueProvider.GetValue(target);
                }
                obj = DeserializeConvertable(propertyConverter, reader, property.PropertyType, value);
            }
            else
            {
                obj = CreateValueInternal(reader, property.PropertyType, contract, property, containerContract, containerProperty, flag ? value : null);
            }
            if ((!flag || obj != value) && ShouldSetPropertyValue(property, obj))
            {
                property.ValueProvider.SetValue(target, obj);
                if (property.SetIsSpecified != null)
                {
                    if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
                    {
                        TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "IsSpecified for property '{0}' on {1} set to true.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType)), null);
                    }
                    property.SetIsSpecified(target, true);
                }
                return true;
            }
            return flag;
        }

        private bool CalculatePropertyDetails(JsonProperty property, ref JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, object target, out bool useExistingValue, out object currentValue, out JsonContract propertyContract, out bool gottenCurrentValue)
        {
            currentValue = null;
            useExistingValue = false;
            propertyContract = null;
            gottenCurrentValue = false;
            if (property.Ignored)
            {
                return true;
            }
            JsonToken tokenType = reader.TokenType;
            if (property.PropertyContract == null)
            {
                property.PropertyContract = GetContractSafe(property.PropertyType);
            }
            if (property.ObjectCreationHandling.GetValueOrDefault(Serializer._objectCreationHandling) != ObjectCreationHandling.Replace && (tokenType == JsonToken.StartArray || tokenType == JsonToken.StartObject) && property.Readable)
            {
                currentValue = property.ValueProvider.GetValue(target);
                gottenCurrentValue = true;
                if (currentValue != null)
                {
                    propertyContract = GetContractSafe(currentValue.GetType());
                    useExistingValue = (!propertyContract.IsReadOnlyOrFixedSize && !propertyContract.UnderlyingType.IsValueType());
                }
            }
            if (!property.Writable && !useExistingValue)
            {
                return true;
            }
            if (property.NullValueHandling.GetValueOrDefault(Serializer._nullValueHandling) == NullValueHandling.Ignore && tokenType == JsonToken.Null)
            {
                return true;
            }
            if (HasFlag(property.DefaultValueHandling.GetValueOrDefault(Serializer._defaultValueHandling), DefaultValueHandling.Ignore) && !HasFlag(property.DefaultValueHandling.GetValueOrDefault(Serializer._defaultValueHandling), DefaultValueHandling.Populate) && JsonTokenUtils.IsPrimitiveToken(tokenType) && MiscellaneousUtils.ValueEquals(reader.Value, property.GetResolvedDefaultValue()))
            {
                return true;
            }
            if (currentValue == null)
            {
                propertyContract = property.PropertyContract;
            }
            else
            {
                propertyContract = GetContractSafe(currentValue.GetType());
                if (propertyContract != property.PropertyContract)
                {
                    propertyConverter = GetConverter(propertyContract, property.MemberConverter, containerContract, containerProperty);
                }
            }
            return false;
        }

        private void AddReference(JsonReader reader, string id, object value)
        {
            try
            {
                if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
                {
                    TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Read object reference Id '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, id, value.GetType())), null);
                }
                Serializer.GetReferenceResolver().AddReference(this, id, value);
            }
            catch (Exception ex)
            {
                throw JsonSerializationException.Create(reader, "Error reading object reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, id), ex);
            }
        }

        private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
        {
            return (value & flag) == flag;
        }

        private bool ShouldSetPropertyValue(JsonProperty property, object value)
        {
            return (property.NullValueHandling.GetValueOrDefault(Serializer._nullValueHandling) != NullValueHandling.Ignore || value != null) && (!HasFlag(property.DefaultValueHandling.GetValueOrDefault(Serializer._defaultValueHandling), DefaultValueHandling.Ignore) || HasFlag(property.DefaultValueHandling.GetValueOrDefault(Serializer._defaultValueHandling), DefaultValueHandling.Populate) || !MiscellaneousUtils.ValueEquals(value, property.GetResolvedDefaultValue())) && property.Writable;
        }

        private IList CreateNewList(JsonReader reader, JsonArrayContract contract, out bool createdFromNonDefaultCreator)
        {
            if (!contract.CanDeserialize)
            {
                throw JsonSerializationException.Create(reader, "Cannot create and populate list type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.CreatedType));
            }
            if (contract.OverrideCreator != null)
            {
                if (contract.HasParameterizedCreator)
                {
                    createdFromNonDefaultCreator = true;
                    return contract.CreateTemporaryCollection();
                }
                createdFromNonDefaultCreator = false;
                return (IList)contract.OverrideCreator(new object[0]);
            }
            else
            {
                if (contract.IsReadOnlyOrFixedSize)
                {
                    createdFromNonDefaultCreator = true;
                    IList list = contract.CreateTemporaryCollection();
                    if (contract.ShouldCreateWrapper)
                    {
                        list = contract.CreateWrapper(list);
                    }
                    return list;
                }
                if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
                {
                    object obj = contract.DefaultCreator();
                    if (contract.ShouldCreateWrapper)
                    {
                        obj = contract.CreateWrapper(obj);
                    }
                    createdFromNonDefaultCreator = false;
                    return (IList)obj;
                }
                if (contract.HasParameterizedCreatorInternal)
                {
                    createdFromNonDefaultCreator = true;
                    return contract.CreateTemporaryCollection();
                }
                if (!contract.IsInstantiable)
                {
                    throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
                }
                throw JsonSerializationException.Create(reader, "Unable to find a constructor to use for type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
            }
        }

        private IDictionary CreateNewDictionary(JsonReader reader, JsonDictionaryContract contract, out bool createdFromNonDefaultCreator)
        {
            if (contract.OverrideCreator != null)
            {
                if (contract.HasParameterizedCreator)
                {
                    createdFromNonDefaultCreator = true;
                    return contract.CreateTemporaryDictionary();
                }
                createdFromNonDefaultCreator = false;
                return (IDictionary)contract.OverrideCreator(new object[0]);
            }
            else
            {
                if (contract.IsReadOnlyOrFixedSize)
                {
                    createdFromNonDefaultCreator = true;
                    return contract.CreateTemporaryDictionary();
                }
                if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
                {
                    object obj = contract.DefaultCreator();
                    if (contract.ShouldCreateWrapper)
                    {
                        obj = contract.CreateWrapper(obj);
                    }
                    createdFromNonDefaultCreator = false;
                    return (IDictionary)obj;
                }
                if (contract.HasParameterizedCreatorInternal)
                {
                    createdFromNonDefaultCreator = true;
                    return contract.CreateTemporaryDictionary();
                }
                if (!contract.IsInstantiable)
                {
                    throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
                }
                throw JsonSerializationException.Create(reader, "Unable to find a default constructor to use for type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
            }
        }

        private void OnDeserializing(JsonReader reader, JsonContract contract, object value)
        {
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
            {
                TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Started deserializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
            }
            contract.InvokeOnDeserializing(value, Serializer._context);
        }

        private void OnDeserialized(JsonReader reader, JsonContract contract, object value)
        {
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
            {
                TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Finished deserializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
            }
            contract.InvokeOnDeserialized(value, Serializer._context);
        }

        private object PopulateDictionary(IDictionary dictionary, JsonReader reader, JsonDictionaryContract contract, JsonProperty containerProperty, string id)
        {
            IWrappedDictionary wrappedDictionary = dictionary as IWrappedDictionary;
            object obj = (wrappedDictionary != null) ? wrappedDictionary.UnderlyingDictionary : dictionary;
            if (id != null)
            {
                AddReference(reader, id, obj);
            }
            OnDeserializing(reader, contract, obj);
            int depth = reader.Depth;
            if (contract.KeyContract == null)
            {
                contract.KeyContract = GetContractSafe(contract.DictionaryKeyType);
            }
            if (contract.ItemContract == null)
            {
                contract.ItemContract = GetContractSafe(contract.DictionaryValueType);
            }
            JsonConverter jsonConverter = contract.ItemConverter ?? GetConverter(contract.ItemContract, null, contract, containerProperty);
            PrimitiveTypeCode primitiveTypeCode = (contract.KeyContract is JsonPrimitiveContract) ? ((JsonPrimitiveContract)contract.KeyContract).TypeCode : PrimitiveTypeCode.Empty;
            bool flag = false;
            for (; ; )
            {
                JsonToken tokenType = reader.TokenType;
                if (tokenType != JsonToken.PropertyName)
                {
                    if (tokenType != JsonToken.Comment)
                    {
                        if (tokenType != JsonToken.EndObject)
                        {
                            break;
                        }
                        goto IL_266;
                    }
                }
                else
                {
                    object obj2 = reader.Value;
                    if (!CheckPropertyName(reader, obj2.ToString()))
                    {
                        try
                        {
                            try
                            {
                                switch (primitiveTypeCode)
                                {
                                    case PrimitiveTypeCode.DateTime:
                                    case PrimitiveTypeCode.DateTimeNullable:
                                        {
                                            if (DateTimeUtils.TryParseDateTime(obj2.ToString(), reader.DateTimeZoneHandling, reader.DateFormatString, reader.Culture, out DateTime dateTime))
                                            {
                                                obj2 = dateTime;
                                            }
                                            else
                                            {
                                                obj2 = EnsureType(reader, obj2, CultureInfo.InvariantCulture, contract.KeyContract, contract.DictionaryKeyType);
                                            }
                                            break;
                                        }
                                    case PrimitiveTypeCode.DateTimeOffset:
                                    case PrimitiveTypeCode.DateTimeOffsetNullable:
                                        {
                                            if (DateTimeUtils.TryParseDateTimeOffset(obj2.ToString(), reader.DateFormatString, reader.Culture, out DateTimeOffset dateTimeOffset))
                                            {
                                                obj2 = dateTimeOffset;
                                            }
                                            else
                                            {
                                                obj2 = EnsureType(reader, obj2, CultureInfo.InvariantCulture, contract.KeyContract, contract.DictionaryKeyType);
                                            }
                                            break;
                                        }
                                    default:
                                        obj2 = EnsureType(reader, obj2, CultureInfo.InvariantCulture, contract.KeyContract, contract.DictionaryKeyType);
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw JsonSerializationException.Create(reader, "Could not convert string '{0}' to dictionary key type '{1}'. Create a TypeConverter to convert from the string to the key type object.".FormatWith(CultureInfo.InvariantCulture, reader.Value, contract.DictionaryKeyType), ex);
                            }
                            if (!ReadForType(reader, contract.ItemContract, jsonConverter != null))
                            {
                                throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
                            }
                            object value;
                            if (jsonConverter != null && jsonConverter.CanRead)
                            {
                                value = DeserializeConvertable(jsonConverter, reader, contract.DictionaryValueType, null);
                            }
                            else
                            {
                                value = CreateValueInternal(reader, contract.DictionaryValueType, contract.ItemContract, null, contract, containerProperty, null);
                            }
                            dictionary[obj2] = value;
                            goto IL_287;
                        }
                        catch (Exception ex2)
                        {
                            if (base.IsErrorHandled(obj, contract, obj2, reader as IJsonLineInfo, reader.Path, ex2))
                            {
                                HandleError(reader, true, depth);
                                goto IL_287;
                            }
                            throw;
                        }
                    }
                }
            IL_287:
                if (flag || !reader.Read())
                {
                    goto IL_296;
                }
                continue;
            IL_266:
                flag = true;
                goto IL_287;
            }
            throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + reader.TokenType);
        IL_296:
            if (!flag)
            {
                ThrowUnexpectedEndException(reader, contract, obj, "Unexpected end when deserializing object.");
            }
            OnDeserialized(reader, contract, obj);
            return obj;
        }

        private object PopulateMultidimensionalArray(IList list, JsonReader reader, JsonArrayContract contract, JsonProperty containerProperty, string id)
        {
            int arrayRank = contract.UnderlyingType.GetArrayRank();
            if (id != null)
            {
                AddReference(reader, id, list);
            }
            OnDeserializing(reader, contract, list);
            JsonContract contractSafe = GetContractSafe(contract.CollectionItemType);
            JsonConverter converter = GetConverter(contractSafe, null, contract, containerProperty);
            int? num = null;
            Stack<IList> stack = new Stack<IList>();
            stack.Push(list);
            IList list2 = list;
            bool flag = false;
            for (; ; )
            {
                int depth = reader.Depth;
                JsonToken tokenType;
                if (stack.Count == arrayRank)
                {
                    try
                    {
                        if (ReadForType(reader, contractSafe, converter != null))
                        {
                            tokenType = reader.TokenType;
                            if (tokenType == JsonToken.EndArray)
                            {
                                stack.Pop();
                                list2 = stack.Peek();
                                num = null;
                            }
                            else
                            {
                                object value;
                                if (converter != null && converter.CanRead)
                                {
                                    value = DeserializeConvertable(converter, reader, contract.CollectionItemType, null);
                                }
                                else
                                {
                                    value = CreateValueInternal(reader, contract.CollectionItemType, contractSafe, null, contract, containerProperty, null);
                                }
                                list2.Add(value);
                            }
                            goto IL_1F9;
                        }
                        goto IL_200;
                    }
                    catch (Exception ex)
                    {
                        JsonPosition position = reader.GetPosition(depth);
                        if (!base.IsErrorHandled(list, contract, position.Position, reader as IJsonLineInfo, reader.Path, ex))
                        {
                            throw;
                        }
                        HandleError(reader, true, depth);
                        if (num != null && num == position.Position)
                        {
                            throw JsonSerializationException.Create(reader, "Infinite loop detected from error handling.", ex);
                        }
                        num = new int?(position.Position);
                        goto IL_1F9;
                    }
                }
                goto IL_179;
            IL_1F9:
                if (flag)
                {
                    goto IL_200;
                }
                continue;
            IL_179:
                if (!reader.Read())
                {
                    goto IL_200;
                }
                tokenType = reader.TokenType;
                if (tokenType == JsonToken.StartArray)
                {
                    IList list3 = new List<object>();
                    list2.Add(list3);
                    stack.Push(list3);
                    list2 = list3;
                    goto IL_1F9;
                }
                if (tokenType == JsonToken.Comment)
                {
                    goto IL_1F9;
                }
                if (tokenType != JsonToken.EndArray)
                {
                    break;
                }
                stack.Pop();
                if (stack.Count > 0)
                {
                    list2 = stack.Peek();
                    goto IL_1F9;
                }
                flag = true;
                goto IL_1F9;
            }
            throw JsonSerializationException.Create(reader, "Unexpected token when deserializing multidimensional array: " + reader.TokenType);
        IL_200:
            if (!flag)
            {
                ThrowUnexpectedEndException(reader, contract, list, "Unexpected end when deserializing array.");
            }
            OnDeserialized(reader, contract, list);
            return list;
        }

        private void ThrowUnexpectedEndException(JsonReader reader, JsonContract contract, object currentObject, string message)
        {
            try
            {
                throw JsonSerializationException.Create(reader, message);
            }
            catch (Exception ex)
            {
                if (!base.IsErrorHandled(currentObject, contract, null, reader as IJsonLineInfo, reader.Path, ex))
                {
                    throw;
                }
                HandleError(reader, false, 0);
            }
        }

        private object PopulateList(IList list, JsonReader reader, JsonArrayContract contract, JsonProperty containerProperty, string id)
        {
            IWrappedCollection wrappedCollection = list as IWrappedCollection;
            object obj = (wrappedCollection != null) ? wrappedCollection.UnderlyingCollection : list;
            if (id != null)
            {
                AddReference(reader, id, obj);
            }
            if (list.IsFixedSize)
            {
                reader.Skip();
                return obj;
            }
            OnDeserializing(reader, contract, obj);
            int depth = reader.Depth;
            if (contract.ItemContract == null)
            {
                contract.ItemContract = GetContractSafe(contract.CollectionItemType);
            }
            JsonConverter converter = GetConverter(contract.ItemContract, null, contract, containerProperty);
            int? num = null;
            bool flag = false;
            do
            {
                try
                {
                    if (!ReadForType(reader, contract.ItemContract, converter != null))
                    {
                        break;
                    }
                    JsonToken tokenType = reader.TokenType;
                    if (tokenType == JsonToken.EndArray)
                    {
                        flag = true;
                    }
                    else
                    {
                        object value;
                        if (converter != null && converter.CanRead)
                        {
                            value = DeserializeConvertable(converter, reader, contract.CollectionItemType, null);
                        }
                        else
                        {
                            value = CreateValueInternal(reader, contract.CollectionItemType, contract.ItemContract, null, contract, containerProperty, null);
                        }
                        list.Add(value);
                    }
                }
                catch (Exception ex)
                {
                    JsonPosition position = reader.GetPosition(depth);
                    if (!base.IsErrorHandled(obj, contract, position.Position, reader as IJsonLineInfo, reader.Path, ex))
                    {
                        throw;
                    }
                    HandleError(reader, true, depth);
                    if (num != null && num == position.Position)
                    {
                        throw JsonSerializationException.Create(reader, "Infinite loop detected from error handling.", ex);
                    }
                    num = new int?(position.Position);
                }
            }
            while (!flag);
            if (!flag)
            {
                ThrowUnexpectedEndException(reader, contract, obj, "Unexpected end when deserializing array.");
            }
            OnDeserialized(reader, contract, obj);
            return obj;
        }

        private object CreateISerializable(JsonReader reader, JsonISerializableContract contract, JsonProperty member, string id)
        {
            Type underlyingType = contract.UnderlyingType;
            if (!JsonTypeReflector.FullyTrusted)
            {
                string text = "Type '{0}' implements ISerializable but cannot be deserialized using the ISerializable interface because the current application is not fully trusted and ISerializable can expose secure data." + Environment.NewLine + "To fix this error either change the environment to be fully trusted, change the application to not deserialize the type, add JsonObjectAttribute to the type or change the JsonSerializer setting ContractResolver to use a new DefaultContractResolver with IgnoreSerializableInterface set to true." + Environment.NewLine;
                text = text.FormatWith(CultureInfo.InvariantCulture, underlyingType);
                throw JsonSerializationException.Create(reader, text);
            }
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
            {
                TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Deserializing {0} using ISerializable constructor.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
            }
            SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, new JsonFormatterConverter(this, contract, member));
            bool flag = false;
            string text2;
            for (; ; )
            {
                JsonToken tokenType = reader.TokenType;
                if (tokenType != JsonToken.PropertyName)
                {
                    if (tokenType != JsonToken.Comment)
                    {
                        if (tokenType != JsonToken.EndObject)
                        {
                            break;
                        }
                        flag = true;
                    }
                }
                else
                {
                    text2 = reader.Value.ToString();
                    if (!reader.Read())
                    {
                        goto Block_7;
                    }
                    serializationInfo.AddValue(text2, JToken.ReadFrom(reader));
                }
                if (flag || !reader.Read())
                {
                    goto IL_125;
                }
            }
            throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + reader.TokenType);
        Block_7:
            throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text2));
        IL_125:
            if (!flag)
            {
                ThrowUnexpectedEndException(reader, contract, serializationInfo, "Unexpected end when deserializing object.");
            }
            if (contract.ISerializableCreator == null)
            {
                throw JsonSerializationException.Create(reader, "ISerializable type '{0}' does not have a valid constructor. To correctly implement ISerializable a constructor that takes SerializationInfo and StreamingContext parameters should be present.".FormatWith(CultureInfo.InvariantCulture, underlyingType));
            }
            object obj = contract.ISerializableCreator(new object[]
            {
                serializationInfo,
                Serializer._context
            });
            if (id != null)
            {
                AddReference(reader, id, obj);
            }
            OnDeserializing(reader, contract, obj);
            OnDeserialized(reader, contract, obj);
            return obj;
        }

        internal object CreateISerializableItem(JToken token, Type type, JsonISerializableContract contract, JsonProperty member)
        {
            JsonContract contractSafe = GetContractSafe(type);
            JsonConverter converter = GetConverter(contractSafe, null, contract, member);
            JsonReader jsonReader = token.CreateReader();
            jsonReader.ReadAndAssert();
            object result;
            if (converter != null && converter.CanRead)
            {
                result = DeserializeConvertable(converter, jsonReader, type, null);
            }
            else
            {
                result = CreateValueInternal(jsonReader, type, contractSafe, null, contract, member, null);
            }
            return result;
        }

        private object CreateObjectUsingCreatorWithParameters(JsonReader reader, JsonObjectContract contract, JsonProperty containerProperty, ObjectConstructor<object> creator, string id)
        {
            ValidationUtils.ArgumentNotNull(creator, "creator");
            bool flag = contract.HasRequiredOrDefaultValueProperties || HasFlag(Serializer._defaultValueHandling, DefaultValueHandling.Populate);
            Type underlyingType = contract.UnderlyingType;
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
            {
                string arg = string.Join(", ", (from p in contract.CreatorParameters
                                                select p.PropertyName).ToArray<string>());
                TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Deserializing {0} using creator with parameters: {1}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType, arg)), null);
            }
            List<JsonSerializerInternalReader.CreatorPropertyContext> list = ResolvePropertyAndCreatorValues(contract, containerProperty, reader, underlyingType);
            if (flag)
            {
                using (IEnumerator<JsonProperty> enumerator = contract.Properties.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        JsonProperty property = enumerator.Current;
                        if (list.All((JsonSerializerInternalReader.CreatorPropertyContext p) => p.Property != property))
                        {
                            list.Add(new JsonSerializerInternalReader.CreatorPropertyContext
                            {
                                Property = property,
                                Name = property.PropertyName,
                                Presence = new JsonSerializerInternalReader.PropertyPresence?(JsonSerializerInternalReader.PropertyPresence.None)
                            });
                        }
                    }
                }
            }
            object[] array = new object[contract.CreatorParameters.Count];
            foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext in list)
            {
                if (flag && creatorPropertyContext.Property != null && creatorPropertyContext.Presence == null)
                {
                    object value = creatorPropertyContext.Value;
                    JsonSerializerInternalReader.PropertyPresence value2;
                    if (value == null)
                    {
                        value2 = JsonSerializerInternalReader.PropertyPresence.Null;
                    }
                    else if (value is string)
                    {
                        value2 = (JsonSerializerInternalReader.CoerceEmptyStringToNull(creatorPropertyContext.Property.PropertyType, creatorPropertyContext.Property.PropertyContract, (string)value) ? JsonSerializerInternalReader.PropertyPresence.Null : JsonSerializerInternalReader.PropertyPresence.Value);
                    }
                    else
                    {
                        value2 = JsonSerializerInternalReader.PropertyPresence.Value;
                    }
                    creatorPropertyContext.Presence = new JsonSerializerInternalReader.PropertyPresence?(value2);
                }
                JsonProperty jsonProperty = creatorPropertyContext.ConstructorProperty;
                if (jsonProperty == null && creatorPropertyContext.Property != null)
                {
                    jsonProperty = contract.CreatorParameters.ForgivingCaseSensitiveFind((JsonProperty p) => p.PropertyName, creatorPropertyContext.Property.UnderlyingName);
                }
                if (jsonProperty != null && !jsonProperty.Ignored)
                {
                    if (flag && (creatorPropertyContext.Presence == JsonSerializerInternalReader.PropertyPresence.None || creatorPropertyContext.Presence == JsonSerializerInternalReader.PropertyPresence.Null))
                    {
                        if (jsonProperty.PropertyContract == null)
                        {
                            jsonProperty.PropertyContract = GetContractSafe(jsonProperty.PropertyType);
                        }
                        if (HasFlag(jsonProperty.DefaultValueHandling.GetValueOrDefault(Serializer._defaultValueHandling), DefaultValueHandling.Populate))
                        {
                            creatorPropertyContext.Value = EnsureType(reader, jsonProperty.GetResolvedDefaultValue(), CultureInfo.InvariantCulture, jsonProperty.PropertyContract, jsonProperty.PropertyType);
                        }
                    }
                    int num = contract.CreatorParameters.IndexOf(jsonProperty);
                    array[num] = creatorPropertyContext.Value;
                    creatorPropertyContext.Used = true;
                }
            }
            object obj = creator(array);
            if (id != null)
            {
                AddReference(reader, id, obj);
            }
            OnDeserializing(reader, contract, obj);
            foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext2 in list)
            {
                if (!creatorPropertyContext2.Used && creatorPropertyContext2.Property != null && !creatorPropertyContext2.Property.Ignored && !(creatorPropertyContext2.Presence == JsonSerializerInternalReader.PropertyPresence.None))
                {
                    JsonProperty property2 = creatorPropertyContext2.Property;
                    object value3 = creatorPropertyContext2.Value;
                    if (ShouldSetPropertyValue(property2, value3))
                    {
                        property2.ValueProvider.SetValue(obj, value3);
                        creatorPropertyContext2.Used = true;
                    }
                    else if (!property2.Writable && value3 != null)
                    {
                        JsonContract jsonContract = Serializer._contractResolver.ResolveContract(property2.PropertyType);
                        if (jsonContract.ContractType != JsonContractType.Array)
                        {
                            goto IL_4A9;
                        }
                        JsonArrayContract jsonArrayContract = (JsonArrayContract)jsonContract;
                        object value4 = property2.ValueProvider.GetValue(obj);
                        if (value4 != null)
                        {
                            IWrappedCollection wrappedCollection = jsonArrayContract.CreateWrapper(value4);

                            {
                                IEnumerator enumerator3 = jsonArrayContract.CreateWrapper(value3).GetEnumerator();
                                while (enumerator3.MoveNext())
                                {
                                    object value5 = enumerator3.Current;
                                    wrappedCollection.Add(value5);
                                }
                                goto IL_55B;
                            }
                        }
                    IL_55B:
                        creatorPropertyContext2.Used = true;
                        continue;
                    IL_4A9:
                        if (jsonContract.ContractType != JsonContractType.Dictionary)
                        {
                            goto IL_55B;
                        }
                        JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)jsonContract;
                        object value6 = property2.ValueProvider.GetValue(obj);
                        if (value6 != null)
                        {
                            IDictionary dictionary;
                            if (!jsonDictionaryContract.ShouldCreateWrapper)
                            {
                                dictionary = (IDictionary)value6;
                            }
                            else
                            {
                                IDictionary dictionary2 = jsonDictionaryContract.CreateWrapper(value6);
                                dictionary = dictionary2;
                            }
                            IDictionary dictionary3 = dictionary;
                            IDictionary dictionary4;
                            if (!jsonDictionaryContract.ShouldCreateWrapper)
                            {
                                dictionary4 = (IDictionary)value3;
                            }
                            else
                            {
                                IDictionary dictionary2 = jsonDictionaryContract.CreateWrapper(value3);
                                dictionary4 = dictionary2;
                            }
                            IDictionaryEnumerator enumerator4 = dictionary4.GetEnumerator();
                            {
                                while (enumerator4.MoveNext())
                                {
                                    DictionaryEntry entry = enumerator4.Entry;
                                    dictionary3.Add(entry.Key, entry.Value);
                                }
                            }
                            goto IL_55B;
                        }
                        goto IL_55B;
                    }
                }
            }
            if (contract.ExtensionDataSetter != null)
            {
                foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext3 in list)
                {
                    if (!creatorPropertyContext3.Used)
                    {
                        contract.ExtensionDataSetter(obj, creatorPropertyContext3.Name, creatorPropertyContext3.Value);
                    }
                }
            }
            if (flag)
            {
                foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext4 in list)
                {
                    if (creatorPropertyContext4.Property != null)
                    {
                        EndProcessProperty(obj, reader, contract, reader.Depth, creatorPropertyContext4.Property, creatorPropertyContext4.Presence.GetValueOrDefault(), !creatorPropertyContext4.Used);
                    }
                }
            }
            OnDeserialized(reader, contract, obj);
            return obj;
        }

        private object DeserializeConvertable(JsonConverter converter, JsonReader reader, Type objectType, object existingValue)
        {
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
            {
                TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Started deserializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, objectType, converter.GetType())), null);
            }
            object result = converter.ReadJson(reader, objectType, existingValue, GetInternalSerializer());
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
            {
                TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Finished deserializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, objectType, converter.GetType())), null);
            }
            return result;
        }

        private List<JsonSerializerInternalReader.CreatorPropertyContext> ResolvePropertyAndCreatorValues(JsonObjectContract contract, JsonProperty containerProperty, JsonReader reader, Type objectType)
        {
            List<JsonSerializerInternalReader.CreatorPropertyContext> list = new List<JsonSerializerInternalReader.CreatorPropertyContext>();
            bool flag = false;
            string text;
            for (; ; )
            {
                JsonToken tokenType = reader.TokenType;
                if (tokenType != JsonToken.PropertyName)
                {
                    if (tokenType != JsonToken.Comment)
                    {
                        if (tokenType != JsonToken.EndObject)
                        {
                            break;
                        }
                        flag = true;
                    }
                }
                else
                {
                    text = reader.Value.ToString();
                    JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext = new JsonSerializerInternalReader.CreatorPropertyContext
                    {
                        Name = reader.Value.ToString(),
                        ConstructorProperty = contract.CreatorParameters.GetClosestMatchProperty(text),
                        Property = contract.Properties.GetClosestMatchProperty(text)
                    };
                    list.Add(creatorPropertyContext);
                    JsonProperty jsonProperty = creatorPropertyContext.ConstructorProperty ?? creatorPropertyContext.Property;
                    if (jsonProperty != null && !jsonProperty.Ignored)
                    {
                        if (jsonProperty.PropertyContract == null)
                        {
                            jsonProperty.PropertyContract = GetContractSafe(jsonProperty.PropertyType);
                        }
                        JsonConverter converter = GetConverter(jsonProperty.PropertyContract, jsonProperty.MemberConverter, contract, containerProperty);
                        if (!ReadForType(reader, jsonProperty.PropertyContract, converter != null))
                        {
                            goto Block_8;
                        }
                        if (converter != null && converter.CanRead)
                        {
                            creatorPropertyContext.Value = DeserializeConvertable(converter, reader, jsonProperty.PropertyType, null);
                        }
                        else
                        {
                            creatorPropertyContext.Value = CreateValueInternal(reader, jsonProperty.PropertyType, jsonProperty.PropertyContract, jsonProperty, contract, containerProperty, null);
                        }
                    }
                    else
                    {
                        if (!reader.Read())
                        {
                            goto Block_11;
                        }
                        if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
                        {
                            TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Could not find member '{0}' on {1}.".FormatWith(CultureInfo.InvariantCulture, text, contract.UnderlyingType)), null);
                        }
                        if (Serializer._missingMemberHandling == MissingMemberHandling.Error)
                        {
                            goto Block_14;
                        }
                        if (contract.ExtensionDataSetter != null)
                        {
                            creatorPropertyContext.Value = ReadExtensionDataValue(contract, containerProperty, reader);
                        }
                        else
                        {
                            reader.Skip();
                        }
                    }
                }
                if (flag || !reader.Read())
                {
                    return list;
                }
            }
            throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + reader.TokenType);
        Block_8:
            throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
        Block_11:
            throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
        Block_14:
            throw JsonSerializationException.Create(reader, "Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, text, objectType.Name));
        }

        private bool ReadForType(JsonReader reader, JsonContract contract, bool hasConverter)
        {
            if (hasConverter)
            {
                return reader.Read();
            }
            switch ((contract != null) ? contract.InternalReadType : ReadType.Read)
            {
                case ReadType.Read:
                    return reader.ReadAndMoveToContent();
                case ReadType.ReadAsInt32:
                    reader.ReadAsInt32();
                    break;
                case ReadType.ReadAsBytes:
                    reader.ReadAsBytes();
                    break;
                case ReadType.ReadAsString:
                    reader.ReadAsString();
                    break;
                case ReadType.ReadAsDecimal:
                    reader.ReadAsDecimal();
                    break;
                case ReadType.ReadAsDateTime:
                    reader.ReadAsDateTime();
                    break;
                case ReadType.ReadAsDateTimeOffset:
                    reader.ReadAsDateTimeOffset();
                    break;
                case ReadType.ReadAsDouble:
                    reader.ReadAsDouble();
                    break;
                case ReadType.ReadAsBoolean:
                    reader.ReadAsBoolean();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return reader.TokenType > JsonToken.None;
        }

        public object CreateNewObject(JsonReader reader, JsonObjectContract objectContract, JsonProperty containerMember, JsonProperty containerProperty, string id, out bool createdFromNonDefaultCreator)
        {
            object obj = null;
            if (objectContract.OverrideCreator != null)
            {
                if (objectContract.CreatorParameters.Count > 0)
                {
                    createdFromNonDefaultCreator = true;
                    return CreateObjectUsingCreatorWithParameters(reader, objectContract, containerMember, objectContract.OverrideCreator, id);
                }
                obj = objectContract.OverrideCreator(new object[0]);
            }
            else if (objectContract.DefaultCreator != null && (!objectContract.DefaultCreatorNonPublic || Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor || objectContract.ParameterizedCreator == null))
            {
                obj = objectContract.DefaultCreator();
            }
            else if (objectContract.ParameterizedCreator != null)
            {
                createdFromNonDefaultCreator = true;
                return CreateObjectUsingCreatorWithParameters(reader, objectContract, containerMember, objectContract.ParameterizedCreator, id);
            }
            if (obj != null)
            {
                createdFromNonDefaultCreator = false;
                return obj;
            }
            if (!objectContract.IsInstantiable)
            {
                throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, objectContract.UnderlyingType));
            }
            throw JsonSerializationException.Create(reader, "Unable to find a constructor to use for type {0}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.".FormatWith(CultureInfo.InvariantCulture, objectContract.UnderlyingType));
        }

        private object PopulateObject(object newObject, JsonReader reader, JsonObjectContract contract, JsonProperty member, string id)
        {
            OnDeserializing(reader, contract, newObject);
            Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> dictionary;
            if (!contract.HasRequiredOrDefaultValueProperties && !HasFlag(Serializer._defaultValueHandling, DefaultValueHandling.Populate))
            {
                dictionary = null;
            }
            else
            {
                dictionary = contract.Properties.ToDictionary((JsonProperty m) => m, (JsonProperty m) => JsonSerializerInternalReader.PropertyPresence.None);
            }
            Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> dictionary2 = dictionary;
            if (id != null)
            {
                AddReference(reader, id, newObject);
            }
            int depth = reader.Depth;
            bool flag = false;
            for (; ; )
            {
                JsonToken tokenType = reader.TokenType;
                if (tokenType != JsonToken.PropertyName)
                {
                    if (tokenType != JsonToken.Comment)
                    {
                        if (tokenType != JsonToken.EndObject)
                        {
                            break;
                        }
                        goto IL_26B;
                    }
                }
                else
                {
                    string text = reader.Value.ToString();
                    if (!CheckPropertyName(reader, text))
                    {
                        try
                        {
                            JsonProperty closestMatchProperty = contract.Properties.GetClosestMatchProperty(text);
                            if (closestMatchProperty != null)
                            {
                                if (closestMatchProperty.Ignored || !ShouldDeserialize(reader, closestMatchProperty, newObject))
                                {
                                    if (reader.Read())
                                    {
                                        SetPropertyPresence(reader, closestMatchProperty, dictionary2);
                                        SetExtensionData(contract, member, reader, text, newObject);
                                    }
                                }
                                else
                                {
                                    if (closestMatchProperty.PropertyContract == null)
                                    {
                                        closestMatchProperty.PropertyContract = GetContractSafe(closestMatchProperty.PropertyType);
                                    }
                                    JsonConverter converter = GetConverter(closestMatchProperty.PropertyContract, closestMatchProperty.MemberConverter, contract, member);
                                    if (!ReadForType(reader, closestMatchProperty.PropertyContract, converter != null))
                                    {
                                        throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, text));
                                    }
                                    SetPropertyPresence(reader, closestMatchProperty, dictionary2);
                                    if (!SetPropertyValue(closestMatchProperty, converter, contract, member, reader, newObject))
                                    {
                                        SetExtensionData(contract, member, reader, text, newObject);
                                    }
                                }
                                goto IL_28B;
                            }
                            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
                            {
                                TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Could not find member '{0}' on {1}".FormatWith(CultureInfo.InvariantCulture, text, contract.UnderlyingType)), null);
                            }
                            if (Serializer._missingMemberHandling == MissingMemberHandling.Error)
                            {
                                throw JsonSerializationException.Create(reader, "Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, text, contract.UnderlyingType.Name));
                            }
                            if (!reader.Read())
                            {
                                goto IL_28B;
                            }
                            SetExtensionData(contract, member, reader, text, newObject);
                            goto IL_28B;
                        }
                        catch (Exception ex)
                        {
                            if (base.IsErrorHandled(newObject, contract, text, reader as IJsonLineInfo, reader.Path, ex))
                            {
                                HandleError(reader, true, depth);
                                goto IL_28B;
                            }
                            throw;
                        }
                    }
                }
            IL_28B:
                if (flag || !reader.Read())
                {
                    goto IL_299;
                }
                continue;
            IL_26B:
                flag = true;
                goto IL_28B;
            }
            throw JsonSerializationException.Create(reader, "Unexpected token when deserializing object: " + reader.TokenType);
        IL_299:
            if (!flag)
            {
                ThrowUnexpectedEndException(reader, contract, newObject, "Unexpected end when deserializing object.");
            }
            if (dictionary2 != null)
            {
                foreach (KeyValuePair<JsonProperty, JsonSerializerInternalReader.PropertyPresence> keyValuePair in dictionary2)
                {
                    JsonProperty key = keyValuePair.Key;
                    JsonSerializerInternalReader.PropertyPresence value = keyValuePair.Value;
                    EndProcessProperty(newObject, reader, contract, depth, key, value, true);
                }
            }
            OnDeserialized(reader, contract, newObject);
            return newObject;
        }

        private bool ShouldDeserialize(JsonReader reader, JsonProperty property, object target)
        {
            if (property.ShouldDeserialize == null)
            {
                return true;
            }
            bool flag = property.ShouldDeserialize(target);
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
            {
                TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, reader.Path, "ShouldDeserialize result for property '{0}' on {1}: {2}".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType, flag)), null);
            }
            return flag;
        }

        private bool CheckPropertyName(JsonReader reader, string memberName)
        {
            if (Serializer.MetadataPropertyHandling == MetadataPropertyHandling.ReadAhead && (memberName == "$id" || memberName == "$ref" || memberName == "$type" || memberName == "$values"))
            {
                reader.Skip();
                return true;
            }
            return false;
        }

        private void SetExtensionData(JsonObjectContract contract, JsonProperty member, JsonReader reader, string memberName, object o)
        {
            if (contract.ExtensionDataSetter != null)
            {
                try
                {
                    object value = ReadExtensionDataValue(contract, member, reader);
                    contract.ExtensionDataSetter(o, memberName, value);
                    return;
                }
                catch (Exception ex)
                {
                    throw JsonSerializationException.Create(reader, "Error setting value in extension data for type '{0}'.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType), ex);
                }
            }
            reader.Skip();
        }

        private object ReadExtensionDataValue(JsonObjectContract contract, JsonProperty member, JsonReader reader)
        {
            object result;
            if (contract.ExtensionDataIsJToken)
            {
                result = JToken.ReadFrom(reader);
            }
            else
            {
                result = CreateValueInternal(reader, null, null, null, contract, member, null);
            }
            return result;
        }

        private void EndProcessProperty(object newObject, JsonReader reader, JsonObjectContract contract, int initialDepth, JsonProperty property, JsonSerializerInternalReader.PropertyPresence presence, bool setDefaultValue)
        {
            if (presence == JsonSerializerInternalReader.PropertyPresence.None || presence == JsonSerializerInternalReader.PropertyPresence.Null)
            {
                try
                {
                    Required required = property._required ?? (contract.ItemRequired ?? Required.Default);
                    if (presence != JsonSerializerInternalReader.PropertyPresence.None)
                    {
                        if (presence == JsonSerializerInternalReader.PropertyPresence.Null)
                        {
                            if (required == Required.Always)
                            {
                                throw JsonSerializationException.Create(reader, "Required property '{0}' expects a value but got null.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
                            }
                            if (required == Required.DisallowNull)
                            {
                                throw JsonSerializationException.Create(reader, "Required property '{0}' expects a non-null value.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
                            }
                        }
                    }
                    else
                    {
                        if (required == Required.AllowNull || required == Required.Always)
                        {
                            throw JsonSerializationException.Create(reader, "Required property '{0}' not found in JSON.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
                        }
                        if (setDefaultValue && !property.Ignored)
                        {
                            if (property.PropertyContract == null)
                            {
                                property.PropertyContract = GetContractSafe(property.PropertyType);
                            }
                            if (HasFlag(property.DefaultValueHandling.GetValueOrDefault(Serializer._defaultValueHandling), DefaultValueHandling.Populate) && property.Writable)
                            {
                                property.ValueProvider.SetValue(newObject, EnsureType(reader, property.GetResolvedDefaultValue(), CultureInfo.InvariantCulture, property.PropertyContract, property.PropertyType));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!base.IsErrorHandled(newObject, contract, property.PropertyName, reader as IJsonLineInfo, reader.Path, ex))
                    {
                        throw;
                    }
                    HandleError(reader, true, initialDepth);
                }
            }
        }

        private void SetPropertyPresence(JsonReader reader, JsonProperty property, Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> requiredProperties)
        {
            if (property != null && requiredProperties != null)
            {
                JsonSerializerInternalReader.PropertyPresence value;
                switch (reader.TokenType)
                {
                    case JsonToken.String:
                        value = (JsonSerializerInternalReader.CoerceEmptyStringToNull(property.PropertyType, property.PropertyContract, (string)reader.Value) ? JsonSerializerInternalReader.PropertyPresence.Null : JsonSerializerInternalReader.PropertyPresence.Value);
                        goto IL_53;
                    case JsonToken.Null:
                    case JsonToken.Undefined:
                        value = JsonSerializerInternalReader.PropertyPresence.Null;
                        goto IL_53;
                }
                value = JsonSerializerInternalReader.PropertyPresence.Value;
            IL_53:
                requiredProperties[property] = value;
            }
        }

        private void HandleError(JsonReader reader, bool readPastError, int initialDepth)
        {
            base.ClearErrorContext();
            if (readPastError)
            {
                reader.Skip();
                while (reader.Depth > initialDepth + 1 && reader.Read())
                {
                }
            }
        }

        internal enum PropertyPresence
        {
            None,
            Null,
            Value
        }

        internal class CreatorPropertyContext
        {
            public string Name;

            public JsonProperty Property;

            public JsonProperty ConstructorProperty;

            public JsonSerializerInternalReader.PropertyPresence? Presence;

            public object Value;

            public bool Used;
        }
    }
}
