using Newtonsoft_X.Json.Bson;
using Newtonsoft_X.Json.Serialization;
using System;
using System.Text.RegularExpressions;

namespace Newtonsoft_X.Json.Converters
{
    /// <summary>
    /// Converts a <see cref="T:System.Text.RegularExpressions.Regex" /> to and from JSON and BSON.
    /// </summary>
    public class RegexConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Regex regex = (Regex)value;
            BsonWriter bsonWriter = writer as BsonWriter;
            if (bsonWriter != null)
            {
                WriteBson(bsonWriter, regex);
                return;
            }
            WriteJson(writer, regex, serializer);
        }

        private bool HasFlag(RegexOptions options, RegexOptions flag)
        {
            return (options & flag) == flag;
        }

        private void WriteBson(BsonWriter writer, Regex regex)
        {
            string text = null;
            if (HasFlag(regex.Options, RegexOptions.IgnoreCase))
            {
                text += "i";
            }
            if (HasFlag(regex.Options, RegexOptions.Multiline))
            {
                text += "m";
            }
            if (HasFlag(regex.Options, RegexOptions.Singleline))
            {
                text += "s";
            }
            text += "u";
            if (HasFlag(regex.Options, RegexOptions.ExplicitCapture))
            {
                text += "x";
            }
            writer.WriteRegex(regex.ToString(), text);
        }

        private void WriteJson(JsonWriter writer, Regex regex, JsonSerializer serializer)
        {
            DefaultContractResolver defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;
            writer.WriteStartObject();
            writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Pattern") : "Pattern");
            writer.WriteValue(regex.ToString());
            writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Options") : "Options");
            serializer.Serialize(writer, regex.Options);
            writer.WriteEndObject();
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
            if (reader.TokenType == JsonToken.StartObject)
            {
                return ReadRegexObject(reader, serializer);
            }
            if (reader.TokenType == JsonToken.String)
            {
                return ReadRegexString(reader);
            }
            throw JsonSerializationException.Create(reader, "Unexpected token when reading Regex.");
        }

        private object ReadRegexString(JsonReader reader)
        {
            string text = (string)reader.Value;
            int num = text.LastIndexOf('/');
            string pattern = text.Substring(1, num - 1);
            string text2 = text.Substring(num + 1);
            RegexOptions regexOptions = RegexOptions.None;
            foreach (char c in text2)
            {
                if (c <= 'm')
                {
                    if (c != 'i')
                    {
                        if (c == 'm')
                        {
                            regexOptions |= RegexOptions.Multiline;
                        }
                    }
                    else
                    {
                        regexOptions |= RegexOptions.IgnoreCase;
                    }
                }
                else if (c != 's')
                {
                    if (c == 'x')
                    {
                        regexOptions |= RegexOptions.ExplicitCapture;
                    }
                }
                else
                {
                    regexOptions |= RegexOptions.Singleline;
                }
            }
            return new Regex(pattern, regexOptions);
        }

        private Regex ReadRegexObject(JsonReader reader, JsonSerializer serializer)
        {
            string text = null;
            RegexOptions? regexOptions = null;
            while (reader.Read())
            {
                JsonToken tokenType = reader.TokenType;
                if (tokenType != JsonToken.PropertyName)
                {
                    if (tokenType != JsonToken.Comment)
                    {
                        if (tokenType == JsonToken.EndObject)
                        {
                            if (text == null)
                            {
                                throw JsonSerializationException.Create(reader, "Error deserializing Regex. No pattern found.");
                            }
                            return new Regex(text, regexOptions ?? RegexOptions.None);
                        }
                    }
                }
                else
                {
                    string a = reader.Value.ToString();
                    if (!reader.Read())
                    {
                        throw JsonSerializationException.Create(reader, "Unexpected end when reading Regex.");
                    }
                    if (string.Equals(a, "Pattern", StringComparison.OrdinalIgnoreCase))
                    {
                        text = (string)reader.Value;
                    }
                    else if (string.Equals(a, "Options", StringComparison.OrdinalIgnoreCase))
                    {
                        regexOptions = new RegexOptions?(serializer.Deserialize<RegexOptions>(reader));
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
            }
            throw JsonSerializationException.Create(reader, "Unexpected end when reading Regex.");
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
            return objectType == typeof(Regex);
        }

        private const string PatternName = "Pattern";

        private const string OptionsName = "Options";
    }
}
