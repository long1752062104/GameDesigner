using Newtonsoft_X.Json.Linq;
using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Newtonsoft_X.Json.Serialization
{
    internal class JsonSerializerInternalWriter : JsonSerializerInternalBase
    {
        public JsonSerializerInternalWriter(JsonSerializer serializer) : base(serializer)
        {
        }

        public void Serialize(JsonWriter jsonWriter, object value, Type objectType)
        {
            if (jsonWriter == null)
            {
                throw new ArgumentNullException("jsonWriter");
            }
            _rootType = objectType;
            _rootLevel = _serializeStack.Count + 1;
            JsonContract contractSafe = GetContractSafe(value);
            try
            {
                if (ShouldWriteReference(value, null, contractSafe, null, null))
                {
                    WriteReference(jsonWriter, value);
                }
                else
                {
                    SerializeValue(jsonWriter, value, contractSafe, null, null, null);
                }
            }
            catch (Exception ex)
            {
                if (!base.IsErrorHandled(null, contractSafe, null, null, jsonWriter.Path, ex))
                {
                    base.ClearErrorContext();
                    throw;
                }
                HandleError(jsonWriter, 0);
            }
            finally
            {
                _rootType = null;
            }
        }

        private JsonSerializerProxy GetInternalSerializer()
        {
            if (InternalSerializer == null)
            {
                InternalSerializer = new JsonSerializerProxy(this);
            }
            return InternalSerializer;
        }

        private JsonContract GetContractSafe(object value)
        {
            if (value == null)
            {
                return null;
            }
#if CLOSE_ILR
            var type = value.GetType();
#else
            var type = Net.Share.ObjectExtensions.GetType(value);
#endif
            return Serializer._contractResolver.ResolveContract(type);
        }

        private void SerializePrimitive(JsonWriter writer, object value, JsonPrimitiveContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
        {
            if (contract.TypeCode == PrimitiveTypeCode.Bytes && ShouldWriteType(TypeNameHandling.Objects, contract, member, containerContract, containerProperty))
            {
                writer.WriteStartObject();
                WriteTypeProperty(writer, contract.CreatedType);
                writer.WritePropertyName("$value", false);
                JsonWriter.WriteValue(writer, contract.TypeCode, value);
                writer.WriteEndObject();
                return;
            }
            JsonWriter.WriteValue(writer, contract.TypeCode, value);
        }

        private void SerializeValue(JsonWriter writer, object value, JsonContract valueContract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            JsonConverter jsonConverter;
            if ((jsonConverter = ((member != null) ? member.Converter : null)) == null && (jsonConverter = ((containerProperty != null) ? containerProperty.ItemConverter : null)) == null && (jsonConverter = ((containerContract != null) ? containerContract.ItemConverter : null)) == null && (jsonConverter = valueContract.Converter) == null)
            {
                jsonConverter = (Serializer.GetMatchingConverter(valueContract.UnderlyingType) ?? valueContract.InternalConverter);
            }
            JsonConverter jsonConverter2 = jsonConverter;
            if (jsonConverter2 != null && jsonConverter2.CanWrite)
            {
                SerializeConvertable(writer, jsonConverter2, value, valueContract, containerContract, containerProperty);
                return;
            }
            switch (valueContract.ContractType)
            {
                case JsonContractType.Object:
                    SerializeObject(writer, value, (JsonObjectContract)valueContract, member, containerContract, containerProperty);
                    return;
                case JsonContractType.Array:
                    {
                        JsonArrayContract jsonArrayContract = (JsonArrayContract)valueContract;
                        if (!jsonArrayContract.IsMultidimensionalArray)
                        {
                            SerializeList(writer, (IEnumerable)value, jsonArrayContract, member, containerContract, containerProperty);
                            return;
                        }
                        SerializeMultidimensionalArray(writer, (Array)value, jsonArrayContract, member, containerContract, containerProperty);
                        return;
                    }
                case JsonContractType.Primitive:
                    SerializePrimitive(writer, value, (JsonPrimitiveContract)valueContract, member, containerContract, containerProperty);
                    return;
                case JsonContractType.String:
                    SerializeString(writer, value, (JsonStringContract)valueContract);
                    return;
                case JsonContractType.Dictionary:
                    {
                        JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)valueContract;
                        IDictionary values;
                        if (!(value is IDictionary))
                        {
                            IDictionary dictionary = jsonDictionaryContract.CreateWrapper(value);
                            values = dictionary;
                        }
                        else
                        {
                            values = (IDictionary)value;
                        }
                        SerializeDictionary(writer, values, jsonDictionaryContract, member, containerContract, containerProperty);
                        return;
                    }
                case JsonContractType.Dynamic:
                    break;
                case JsonContractType.Serializable:
                    SerializeISerializable(writer, (ISerializable)value, (JsonISerializableContract)valueContract, member, containerContract, containerProperty);
                    return;
                case JsonContractType.Linq:
                    ((JToken)value).WriteTo(writer, Serializer.Converters.ToArray<JsonConverter>());
                    break;
                default:
                    return;
            }
        }

        private bool? ResolveIsReference(JsonContract contract, JsonProperty property, JsonContainerContract collectionContract, JsonProperty containerProperty)
        {
            bool? result = null;
            if (property != null)
            {
                result = property.IsReference;
            }
            if (result == null && containerProperty != null)
            {
                result = containerProperty.ItemIsReference;
            }
            if (result == null && collectionContract != null)
            {
                result = collectionContract.ItemIsReference;
            }
            if (result == null)
            {
                result = contract.IsReference;
            }
            return result;
        }

        private bool ShouldWriteReference(object value, JsonProperty property, JsonContract valueContract, JsonContainerContract collectionContract, JsonProperty containerProperty)
        {
            if (value == null)
            {
                return false;
            }
            if (valueContract.ContractType == JsonContractType.Primitive || valueContract.ContractType == JsonContractType.String)
            {
                return false;
            }
            bool? flag = ResolveIsReference(valueContract, property, collectionContract, containerProperty);
            if (flag == null)
            {
                if (valueContract.ContractType == JsonContractType.Array)
                {
                    flag = new bool?(HasFlag(Serializer._preserveReferencesHandling, PreserveReferencesHandling.Arrays));
                }
                else
                {
                    flag = new bool?(HasFlag(Serializer._preserveReferencesHandling, PreserveReferencesHandling.Objects));
                }
            }
            return flag.GetValueOrDefault() && Serializer.GetReferenceResolver().IsReferenced(this, value);
        }

        private bool ShouldWriteProperty(object memberValue, JsonProperty property)
        {
            return (property.NullValueHandling.GetValueOrDefault(Serializer._nullValueHandling) != NullValueHandling.Ignore || memberValue != null) && (!HasFlag(property.DefaultValueHandling.GetValueOrDefault(Serializer._defaultValueHandling), DefaultValueHandling.Ignore) || !MiscellaneousUtils.ValueEquals(memberValue, property.GetResolvedDefaultValue()));
        }

        private bool CheckForCircularReference(JsonWriter writer, object value, JsonProperty property, JsonContract contract, JsonContainerContract containerContract, JsonProperty containerProperty)
        {
            if (value == null || contract.ContractType == JsonContractType.Primitive || contract.ContractType == JsonContractType.String)
            {
                return true;
            }
            ReferenceLoopHandling? referenceLoopHandling = null;
            if (property != null)
            {
                referenceLoopHandling = property.ReferenceLoopHandling;
            }
            if (referenceLoopHandling == null && containerProperty != null)
            {
                referenceLoopHandling = containerProperty.ItemReferenceLoopHandling;
            }
            if (referenceLoopHandling == null && containerContract != null)
            {
                referenceLoopHandling = containerContract.ItemReferenceLoopHandling;
            }
            if ((Serializer._equalityComparer != null) ? _serializeStack.Contains(value, Serializer._equalityComparer) : _serializeStack.Contains(value))
            {
                string text = "Self referencing loop detected";
                if (property != null)
                {
                    text += " for property '{0}'".FormatWith(CultureInfo.InvariantCulture, property.PropertyName);
                }
                text += " with type '{0}'.".FormatWith(CultureInfo.InvariantCulture, value.GetType());
                switch (referenceLoopHandling.GetValueOrDefault(Serializer._referenceLoopHandling))
                {
                    case ReferenceLoopHandling.Error:
                        throw JsonSerializationException.Create(null, writer.ContainerPath, text, null);
                    case ReferenceLoopHandling.Ignore:
                        if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
                        {
                            TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, text + ". Skipping serializing self referenced value."), null);
                        }
                        return false;
                    case ReferenceLoopHandling.Serialize:
                        if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
                        {
                            TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, text + ". Serializing self referenced value."), null);
                        }
                        return true;
                }
            }
            return true;
        }

        private void WriteReference(JsonWriter writer, object value)
        {
            string reference = GetReference(writer, value);
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
            {
                TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Writing object reference to Id '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, reference, value.GetType())), null);
            }
            writer.WriteStartObject();
            writer.WritePropertyName("$ref", false);
            writer.WriteValue(reference);
            writer.WriteEndObject();
        }

        private string GetReference(JsonWriter writer, object value)
        {
            string reference;
            try
            {
                reference = Serializer.GetReferenceResolver().GetReference(this, value);
            }
            catch (Exception ex)
            {
                throw JsonSerializationException.Create(null, writer.ContainerPath, "Error writing object reference for '{0}'.".FormatWith(CultureInfo.InvariantCulture, value.GetType()), ex);
            }
            return reference;
        }

        internal static bool TryConvertToString(object value, Type type, out string s)
        {
            TypeConverter converter = ConvertUtils.GetConverter(type);
            if (converter != null && !(converter is ComponentConverter) && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)))
            {
                s = converter.ConvertToInvariantString(value);
                return true;
            }
            if (value is Type)
            {
                s = ((Type)value).AssemblyQualifiedName;
                return true;
            }
            s = null;
            return false;
        }

        private void SerializeString(JsonWriter writer, object value, JsonStringContract contract)
        {
            OnSerializing(writer, contract, value);
            JsonSerializerInternalWriter.TryConvertToString(value, contract.UnderlyingType, out string value2);
            writer.WriteValue(value2);
            OnSerialized(writer, contract, value);
        }

        private void OnSerializing(JsonWriter writer, JsonContract contract, object value)
        {
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
            {
                TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Started serializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
            }
            contract.InvokeOnSerializing(value, Serializer._context);
        }

        private void OnSerialized(JsonWriter writer, JsonContract contract, object value)
        {
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
            {
                TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Finished serializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
            }
            contract.InvokeOnSerialized(value, Serializer._context);
        }

        private void SerializeObject(JsonWriter writer, object value, JsonObjectContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
        {
            OnSerializing(writer, contract, value);
            _serializeStack.Add(value);
            WriteObjectStart(writer, value, contract, member, collectionContract, containerProperty);
            int top = writer.Top;
            for (int i = 0; i < contract.Properties.Count; i++)
            {
                JsonProperty jsonProperty = contract.Properties[i];
                try
                {
                    if (CalculatePropertyValues(writer, value, contract, member, jsonProperty, out JsonContract valueContract, out object value2))
                    {
                        jsonProperty.WritePropertyName(writer);
                        SerializeValue(writer, value2, valueContract, jsonProperty, contract, member);
                    }
                }
                catch (Exception ex)
                {
                    if (!base.IsErrorHandled(value, contract, jsonProperty.PropertyName, null, writer.ContainerPath, ex))
                    {
                        throw;
                    }
                    HandleError(writer, top);
                }
            }
            if (contract.ExtensionDataGetter != null)
            {
                IEnumerable<KeyValuePair<object, object>> enumerable = contract.ExtensionDataGetter(value);
                if (enumerable != null)
                {
                    foreach (KeyValuePair<object, object> keyValuePair in enumerable)
                    {
                        JsonContract contractSafe = GetContractSafe(keyValuePair.Key);
                        JsonContract contractSafe2 = GetContractSafe(keyValuePair.Value);
                        string propertyName = GetPropertyName(writer, keyValuePair.Key, contractSafe, out bool flag);
                        if (ShouldWriteReference(keyValuePair.Value, null, contractSafe2, contract, member))
                        {
                            writer.WritePropertyName(propertyName);
                            WriteReference(writer, keyValuePair.Value);
                        }
                        else if (CheckForCircularReference(writer, keyValuePair.Value, null, contractSafe2, contract, member))
                        {
                            writer.WritePropertyName(propertyName);
                            SerializeValue(writer, keyValuePair.Value, contractSafe2, null, contract, member);
                        }
                    }
                }
            }
            writer.WriteEndObject();
            _serializeStack.RemoveAt(_serializeStack.Count - 1);
            OnSerialized(writer, contract, value);
        }

        private bool CalculatePropertyValues(JsonWriter writer, object value, JsonContainerContract contract, JsonProperty member, JsonProperty property, out JsonContract memberContract, out object memberValue)
        {
            if (!property.Ignored && property.Readable && ShouldSerialize(writer, property, value) && IsSpecified(writer, property, value))
            {
                if (property.PropertyContract == null)
                {
                    property.PropertyContract = Serializer._contractResolver.ResolveContract(property.PropertyType);
                }
                memberValue = property.ValueProvider.GetValue(value);
                memberContract = (property.PropertyContract.IsSealed ? property.PropertyContract : GetContractSafe(memberValue));
                if (ShouldWriteProperty(memberValue, property))
                {
                    if (ShouldWriteReference(memberValue, property, memberContract, contract, member))
                    {
                        property.WritePropertyName(writer);
                        WriteReference(writer, memberValue);
                        return false;
                    }
                    if (!CheckForCircularReference(writer, memberValue, property, memberContract, contract, member))
                    {
                        return false;
                    }
                    if (memberValue == null)
                    {
                        JsonObjectContract jsonObjectContract = contract as JsonObjectContract;
                        Required required = property._required ?? (((jsonObjectContract != null) ? jsonObjectContract.ItemRequired : null) ?? Required.Default);
                        if (required == Required.Always)
                        {
                            throw JsonSerializationException.Create(null, writer.ContainerPath, "Cannot write a null value for property '{0}'. Property requires a value.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName), null);
                        }
                        if (required == Required.DisallowNull)
                        {
                            throw JsonSerializationException.Create(null, writer.ContainerPath, "Cannot write a null value for property '{0}'. Property requires a non-null value.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName), null);
                        }
                    }
                    return true;
                }
            }
            memberContract = null;
            memberValue = null;
            return false;
        }

        private void WriteObjectStart(JsonWriter writer, object value, JsonContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
        {
            writer.WriteStartObject();
            if ((ResolveIsReference(contract, member, collectionContract, containerProperty) ?? HasFlag(Serializer._preserveReferencesHandling, PreserveReferencesHandling.Objects)) && (member == null || member.Writable))
            {
                WriteReferenceIdProperty(writer, contract.UnderlyingType, value);
            }
            if (ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionContract, containerProperty))
            {
                WriteTypeProperty(writer, contract.UnderlyingType);
            }
        }

        private void WriteReferenceIdProperty(JsonWriter writer, Type type, object value)
        {
            string reference = GetReference(writer, value);
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
            {
                TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "Writing object reference Id '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, reference, type)), null);
            }
            writer.WritePropertyName("$id", false);
            writer.WriteValue(reference);
        }

        private void WriteTypeProperty(JsonWriter writer, Type type)
        {
            string typeName = ReflectionUtils.GetTypeName(type, Serializer._typeNameAssemblyFormat, Serializer._binder);
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
            {
                TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "Writing type name '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, typeName, type)), null);
            }
            writer.WritePropertyName("$type", false);
            writer.WriteValue(typeName);
        }

        private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
        {
            return (value & flag) == flag;
        }

        private bool HasFlag(PreserveReferencesHandling value, PreserveReferencesHandling flag)
        {
            return (value & flag) == flag;
        }

        private bool HasFlag(TypeNameHandling value, TypeNameHandling flag)
        {
            return (value & flag) == flag;
        }

        private void SerializeConvertable(JsonWriter writer, JsonConverter converter, object value, JsonContract contract, JsonContainerContract collectionContract, JsonProperty containerProperty)
        {
            if (ShouldWriteReference(value, null, contract, collectionContract, containerProperty))
            {
                WriteReference(writer, value);
                return;
            }
            if (!CheckForCircularReference(writer, value, null, contract, collectionContract, containerProperty))
            {
                return;
            }
            _serializeStack.Add(value);
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
            {
                TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Started serializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, value.GetType(), converter.GetType())), null);
            }
            converter.WriteJson(writer, value, GetInternalSerializer());
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Info)
            {
                TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Finished serializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, value.GetType(), converter.GetType())), null);
            }
            _serializeStack.RemoveAt(_serializeStack.Count - 1);
        }

        private void SerializeList(JsonWriter writer, IEnumerable values, JsonArrayContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
        {
            IWrappedCollection wrappedCollection = values as IWrappedCollection;
            object obj = (wrappedCollection != null) ? wrappedCollection.UnderlyingCollection : values;
            OnSerializing(writer, contract, obj);
            _serializeStack.Add(obj);
            bool flag = WriteStartArray(writer, obj, contract, member, collectionContract, containerProperty);
            writer.WriteStartArray();
            int top = writer.Top;
            int num = 0;
            foreach (object value in values)
            {
                try
                {
                    JsonContract jsonContract = contract.FinalItemContract ?? GetContractSafe(value);
                    if (ShouldWriteReference(value, null, jsonContract, contract, member))
                    {
                        WriteReference(writer, value);
                    }
                    else if (CheckForCircularReference(writer, value, null, jsonContract, contract, member))
                    {
                        SerializeValue(writer, value, jsonContract, null, contract, member);
                    }
                }
                catch (Exception ex)
                {
                    if (!base.IsErrorHandled(obj, contract, num, null, writer.ContainerPath, ex))
                    {
                        throw;
                    }
                    HandleError(writer, top);
                }
                finally
                {
                    num++;
                }
            }
            writer.WriteEndArray();
            if (flag)
            {
                writer.WriteEndObject();
            }
            _serializeStack.RemoveAt(_serializeStack.Count - 1);
            OnSerialized(writer, contract, obj);
        }

        private void SerializeMultidimensionalArray(JsonWriter writer, Array values, JsonArrayContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
        {
            OnSerializing(writer, contract, values);
            _serializeStack.Add(values);
            bool flag = WriteStartArray(writer, values, contract, member, collectionContract, containerProperty);
            SerializeMultidimensionalArray(writer, values, contract, member, writer.Top, new int[0]);
            if (flag)
            {
                writer.WriteEndObject();
            }
            _serializeStack.RemoveAt(_serializeStack.Count - 1);
            OnSerialized(writer, contract, values);
        }

        private void SerializeMultidimensionalArray(JsonWriter writer, Array values, JsonArrayContract contract, JsonProperty member, int initialDepth, int[] indices)
        {
            int num = indices.Length;
            int[] array = new int[num + 1];
            for (int i = 0; i < num; i++)
            {
                array[i] = indices[i];
            }
            writer.WriteStartArray();
            int j = values.GetLowerBound(num);
            while (j <= values.GetUpperBound(num))
            {
                array[num] = j;
                if (array.Length == values.Rank)
                {
                    object value = values.GetValue(array);
                    try
                    {
                        JsonContract jsonContract = contract.FinalItemContract ?? GetContractSafe(value);
                        if (ShouldWriteReference(value, null, jsonContract, contract, member))
                        {
                            WriteReference(writer, value);
                        }
                        else if (CheckForCircularReference(writer, value, null, jsonContract, contract, member))
                        {
                            SerializeValue(writer, value, jsonContract, null, contract, member);
                        }
                        goto IL_DE;
                    }
                    catch (Exception ex)
                    {
                        if (base.IsErrorHandled(values, contract, j, null, writer.ContainerPath, ex))
                        {
                            HandleError(writer, initialDepth + 1);
                            goto IL_DE;
                        }
                        throw;
                    }
                }
                goto IL_CE;
            IL_DE:
                j++;
                continue;
            IL_CE:
                SerializeMultidimensionalArray(writer, values, contract, member, initialDepth + 1, array);
                goto IL_DE;
            }
            writer.WriteEndArray();
        }

        private bool WriteStartArray(JsonWriter writer, object values, JsonArrayContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
        {
            bool flag = ResolveIsReference(contract, member, containerContract, containerProperty) ?? HasFlag(Serializer._preserveReferencesHandling, PreserveReferencesHandling.Arrays);
            flag = (flag && (member == null || member.Writable));
            bool flag2 = ShouldWriteType(TypeNameHandling.Arrays, contract, member, containerContract, containerProperty);
            bool flag3 = flag || flag2;
            if (flag3)
            {
                writer.WriteStartObject();
                if (flag)
                {
                    WriteReferenceIdProperty(writer, contract.UnderlyingType, values);
                }
                if (flag2)
                {
                    WriteTypeProperty(writer, values.GetType());
                }
                writer.WritePropertyName("$values", false);
            }
            if (contract.ItemContract == null)
            {
                contract.ItemContract = Serializer._contractResolver.ResolveContract(contract.CollectionItemType ?? typeof(object));
            }
            return flag3;
        }

        private void SerializeISerializable(JsonWriter writer, ISerializable value, JsonISerializableContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
        {
            if (!JsonTypeReflector.FullyTrusted)
            {
                string text = "Type '{0}' implements ISerializable but cannot be serialized using the ISerializable interface because the current application is not fully trusted and ISerializable can expose secure data." + Environment.NewLine + "To fix this error either change the environment to be fully trusted, change the application to not deserialize the type, add JsonObjectAttribute to the type or change the JsonSerializer setting ContractResolver to use a new DefaultContractResolver with IgnoreSerializableInterface set to true." + Environment.NewLine;
                text = text.FormatWith(CultureInfo.InvariantCulture, value.GetType());
                throw JsonSerializationException.Create(null, writer.ContainerPath, text, null);
            }
            OnSerializing(writer, contract, value);
            _serializeStack.Add(value);
            WriteObjectStart(writer, value, contract, member, collectionContract, containerProperty);
            SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, new FormatterConverter());
            value.GetObjectData(serializationInfo, Serializer._context);
            foreach (SerializationEntry serializationEntry in serializationInfo)
            {
                JsonContract contractSafe = GetContractSafe(serializationEntry.Value);
                if (ShouldWriteReference(serializationEntry.Value, null, contractSafe, contract, member))
                {
                    writer.WritePropertyName(serializationEntry.Name);
                    WriteReference(writer, serializationEntry.Value);
                }
                else if (CheckForCircularReference(writer, serializationEntry.Value, null, contractSafe, contract, member))
                {
                    writer.WritePropertyName(serializationEntry.Name);
                    SerializeValue(writer, serializationEntry.Value, contractSafe, null, contract, member);
                }
            }
            writer.WriteEndObject();
            _serializeStack.RemoveAt(_serializeStack.Count - 1);
            OnSerialized(writer, contract, value);
        }

        private bool ShouldWriteDynamicProperty(object memberValue)
        {
            return (Serializer._nullValueHandling != NullValueHandling.Ignore || memberValue != null) && (!HasFlag(Serializer._defaultValueHandling, DefaultValueHandling.Ignore) || (memberValue != null && !MiscellaneousUtils.ValueEquals(memberValue, ReflectionUtils.GetDefaultValue(memberValue.GetType()))));
        }

        private bool ShouldWriteType(TypeNameHandling typeNameHandlingFlag, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
        {
            TypeNameHandling value = ((member != null) ? member.TypeNameHandling : null) ?? (((containerProperty != null) ? containerProperty.ItemTypeNameHandling : null) ?? (((containerContract != null) ? containerContract.ItemTypeNameHandling : null) ?? Serializer._typeNameHandling));
            if (HasFlag(value, typeNameHandlingFlag))
            {
                return true;
            }
            if (HasFlag(value, TypeNameHandling.Auto))
            {
                if (member != null)
                {
                    if (contract.UnderlyingType != member.PropertyContract.CreatedType)
                    {
                        return true;
                    }
                }
                else if (containerContract != null)
                {
                    if (containerContract.ItemContract == null || contract.UnderlyingType != containerContract.ItemContract.CreatedType)
                    {
                        return true;
                    }
                }
                else if (_rootType != null && _serializeStack.Count == _rootLevel)
                {
                    JsonContract jsonContract = Serializer._contractResolver.ResolveContract(_rootType);
                    if (contract.UnderlyingType != jsonContract.CreatedType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void SerializeDictionary(JsonWriter writer, IDictionary values, JsonDictionaryContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
        {
            IWrappedDictionary wrappedDictionary = values as IWrappedDictionary;
            object obj = (wrappedDictionary != null) ? wrappedDictionary.UnderlyingDictionary : values;
            OnSerializing(writer, contract, obj);
            _serializeStack.Add(obj);
            WriteObjectStart(writer, obj, contract, member, collectionContract, containerProperty);
            if (contract.ItemContract == null)
            {
                contract.ItemContract = Serializer._contractResolver.ResolveContract(contract.DictionaryValueType ?? typeof(object));
            }
            if (contract.KeyContract == null)
            {
                contract.KeyContract = Serializer._contractResolver.ResolveContract(contract.DictionaryKeyType ?? typeof(object));
            }
            int top = writer.Top;

            {
                IDictionaryEnumerator enumerator = values.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DictionaryEntry entry = enumerator.Entry;
                    string text = GetPropertyName(writer, entry.Key, contract.KeyContract, out bool escape);
                    text = ((contract.DictionaryKeyResolver != null) ? contract.DictionaryKeyResolver(text) : text);
                    try
                    {
                        object value = entry.Value;
                        JsonContract jsonContract = contract.FinalItemContract ?? GetContractSafe(value);
                        if (ShouldWriteReference(value, null, jsonContract, contract, member))
                        {
                            writer.WritePropertyName(text, escape);
                            WriteReference(writer, value);
                        }
                        else if (CheckForCircularReference(writer, value, null, jsonContract, contract, member))
                        {
                            writer.WritePropertyName(text, escape);
                            SerializeValue(writer, value, jsonContract, null, contract, member);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!base.IsErrorHandled(obj, contract, text, null, writer.ContainerPath, ex))
                        {
                            throw;
                        }
                        HandleError(writer, top);
                    }
                }
            }
            writer.WriteEndObject();
            _serializeStack.RemoveAt(_serializeStack.Count - 1);
            OnSerialized(writer, contract, obj);
        }

        private string GetPropertyName(JsonWriter writer, object name, JsonContract contract, out bool escape)
        {
            if (contract.ContractType == JsonContractType.Primitive)
            {
                JsonPrimitiveContract jsonPrimitiveContract = (JsonPrimitiveContract)contract;
                if (jsonPrimitiveContract.TypeCode == PrimitiveTypeCode.DateTime || jsonPrimitiveContract.TypeCode == PrimitiveTypeCode.DateTimeNullable)
                {
                    DateTime value = DateTimeUtils.EnsureDateTime((DateTime)name, writer.DateTimeZoneHandling);
                    escape = false;
                    StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
                    DateTimeUtils.WriteDateTimeString(stringWriter, value, writer.DateFormatHandling, writer.DateFormatString, writer.Culture);
                    return stringWriter.ToString();
                }
                if (jsonPrimitiveContract.TypeCode == PrimitiveTypeCode.DateTimeOffset || jsonPrimitiveContract.TypeCode == PrimitiveTypeCode.DateTimeOffsetNullable)
                {
                    escape = false;
                    StringWriter stringWriter2 = new StringWriter(CultureInfo.InvariantCulture);
                    DateTimeUtils.WriteDateTimeOffsetString(stringWriter2, (DateTimeOffset)name, writer.DateFormatHandling, writer.DateFormatString, writer.Culture);
                    return stringWriter2.ToString();
                }
                escape = true;
                return Convert.ToString(name, CultureInfo.InvariantCulture);
            }
            else
            {
                if (JsonSerializerInternalWriter.TryConvertToString(name, name.GetType(), out string result))
                {
                    escape = true;
                    return result;
                }
                escape = true;
                return name.ToString();
            }
        }

        private void HandleError(JsonWriter writer, int initialDepth)
        {
            base.ClearErrorContext();
            if (writer.WriteState == WriteState.Property)
            {
                writer.WriteNull();
            }
            while (writer.Top > initialDepth)
            {
                writer.WriteEnd();
            }
        }

        private bool ShouldSerialize(JsonWriter writer, JsonProperty property, object target)
        {
            if (property.ShouldSerialize == null)
            {
                return true;
            }
            bool flag = property.ShouldSerialize(target);
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
            {
                TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "ShouldSerialize result for property '{0}' on {1}: {2}".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType, flag)), null);
            }
            return flag;
        }

        private bool IsSpecified(JsonWriter writer, JsonProperty property, object target)
        {
            if (property.GetIsSpecified == null)
            {
                return true;
            }
            bool flag = property.GetIsSpecified(target);
            if (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
            {
                TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "IsSpecified result for property '{0}' on {1}: {2}".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType, flag)), null);
            }
            return flag;
        }

        private Type _rootType;

        private int _rootLevel;

        private readonly List<object> _serializeStack = new List<object>();
    }
}
