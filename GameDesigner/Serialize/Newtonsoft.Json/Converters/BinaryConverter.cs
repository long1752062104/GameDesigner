using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft_X.Json.Converters
{
    /// <summary>
    /// Converts a binary value to and from a base 64 string value.
    /// </summary>
    public class BinaryConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            byte[] byteArray = GetByteArray(value);
            writer.WriteValue(byteArray);
        }

        private byte[] GetByteArray(object value)
        {
            if (value.GetType().AssignableToTypeName("System.Data.Linq.Binary"))
            {
                EnsureReflectionObject(value.GetType());
                return (byte[])_reflectionObject.GetValue(value, "ToArray");
            }
            throw new JsonSerializationException("Unexpected value type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
        }

        private void EnsureReflectionObject(Type t)
        {
            if (_reflectionObject == null)
            {
                _reflectionObject = ReflectionObject.Create(t, t.GetConstructor(new Type[]
                {
                    typeof(byte[])
                }), new string[]
                {
                    "ToArray"
                });
            }
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullable(objectType))
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
                }
                return null;
            }
            else
            {
                byte[] array;
                if (reader.TokenType == JsonToken.StartArray)
                {
                    array = ReadByteArray(reader);
                }
                else
                {
                    if (reader.TokenType != JsonToken.String)
                    {
                        throw JsonSerializationException.Create(reader, "Unexpected token parsing binary. Expected String or StartArray, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                    }
                    array = Convert.FromBase64String(reader.Value.ToString());
                }
                Type type = ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
                if (type.AssignableToTypeName("System.Data.Linq.Binary"))
                {
                    EnsureReflectionObject(type);
                    return _reflectionObject.Creator(new object[]
                    {
                        array
                    });
                }
                throw JsonSerializationException.Create(reader, "Unexpected object type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, objectType));
            }
        }

        private byte[] ReadByteArray(JsonReader reader)
        {
            List<byte> list = new List<byte>();
            while (reader.Read())
            {
                JsonToken tokenType = reader.TokenType;
                if (tokenType != JsonToken.Comment)
                {
                    if (tokenType != JsonToken.Integer)
                    {
                        if (tokenType != JsonToken.EndArray)
                        {
                            throw JsonSerializationException.Create(reader, "Unexpected token when reading bytes: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                        }
                        return list.ToArray();
                    }
                    else
                    {
                        list.Add(Convert.ToByte(reader.Value, CultureInfo.InvariantCulture));
                    }
                }
            }
            throw JsonSerializationException.Create(reader, "Unexpected end when reading bytes.");
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType.AssignableToTypeName("System.Data.Linq.Binary");
        }

        private const string BinaryTypeName = "System.Data.Linq.Binary";

        private const string BinaryToArrayName = "ToArray";

        private ReflectionObject _reflectionObject;
    }
}
