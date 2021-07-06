using Newtonsoft_X.Json.Utilities;
using System;
using System.Globalization;

namespace Newtonsoft_X.Json.Converters
{
    /// <summary>
    /// Converts a <see cref="T:System.Version" /> to and from a string (e.g. <c>"1.2.3.4"</c>).
    /// </summary>
    public class VersionConverter : JsonConverter
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
            if (value is Version)
            {
                writer.WriteValue(value.ToString());
                return;
            }
            throw new JsonSerializationException("Expected Version object value");
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing property value of the JSON that is being converted.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.TokenType == JsonToken.String)
            {
                try
                {
                    return new Version((string)reader.Value);
                }
                catch (Exception ex)
                {
                    throw JsonSerializationException.Create(reader, "Error parsing version string: {0}".FormatWith(CultureInfo.InvariantCulture, reader.Value), ex);
                }
            }
            throw JsonSerializationException.Create(reader, "Unexpected token or value when parsing version. Token: {0}, Value: {1}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType, reader.Value));
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
            return objectType == typeof(Version);
        }
    }
}
