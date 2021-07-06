using Newtonsoft_X.Json.Utilities;
using System;
using System.Globalization;

namespace Newtonsoft_X.Json.Converters
{
    /// <summary>
    /// Converts an <see cref="T:System.Enum" /> to and from its name string value.
    /// </summary>
    public class StringEnumConverter : JsonConverter
    {
        /// <summary>
        /// Gets or sets a value indicating whether the written enum text should be camel case.
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if the written enum text will be camel case; otherwise, <c>false</c>.</value>
        public bool CamelCaseText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether integer values are allowed when serializing and deserializing.
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if integers are allowed when serializing and deserializing; otherwise, <c>false</c>.</value>
        public bool AllowIntegerValues { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Converters.StringEnumConverter" /> class.
        /// </summary>
        public StringEnumConverter()
        {
            AllowIntegerValues = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Converters.StringEnumConverter" /> class.
        /// </summary>
        /// <param name="camelCaseText"><c>true</c> if the written enum text will be camel case; otherwise, <c>false</c>.</param>
        public StringEnumConverter(bool camelCaseText) : this()
        {
            CamelCaseText = camelCaseText;
        }

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
            Enum @enum = (Enum)value;
            string text = @enum.ToString("G");
            if (char.IsNumber(text[0]) || text[0] == '-')
            {
                writer.WriteValue(value);
                return;
            }
            string value2 = EnumUtils.ToEnumName(@enum.GetType(), text, CamelCaseText);
            writer.WriteValue(value2);
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
            if (reader.TokenType != JsonToken.Null)
            {
                bool flag = ReflectionUtils.IsNullableType(objectType);
                Type type = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
                try
                {
                    if (reader.TokenType == JsonToken.String)
                    {
                        return EnumUtils.ParseEnumName(reader.Value.ToString(), flag, type);
                    }
                    if (reader.TokenType == JsonToken.Integer)
                    {
                        if (!AllowIntegerValues)
                        {
                            throw JsonSerializationException.Create(reader, "Integer value {0} is not allowed.".FormatWith(CultureInfo.InvariantCulture, reader.Value));
                        }
                        return ConvertUtils.ConvertOrCast(reader.Value, CultureInfo.InvariantCulture, type);
                    }
                }
                catch (Exception ex)
                {
                    throw JsonSerializationException.Create(reader, "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.FormatValueForPrint(reader.Value), objectType), ex);
                }
                throw JsonSerializationException.Create(reader, "Unexpected token {0} when parsing enum.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            if (!ReflectionUtils.IsNullableType(objectType))
            {
                throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
            }
            return null;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType).IsEnum();
        }
    }
}
