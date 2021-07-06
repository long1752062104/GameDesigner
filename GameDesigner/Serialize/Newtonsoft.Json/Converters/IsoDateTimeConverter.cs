using Newtonsoft_X.Json.Utilities;
using System;
using System.Globalization;

namespace Newtonsoft_X.Json.Converters
{
    /// <summary>
    /// Converts a <see cref="T:System.DateTime" /> to and from the ISO 8601 date format (e.g. <c>"2008-04-12T12:53Z"</c>).
    /// </summary>
    public class IsoDateTimeConverter : DateTimeConverterBase
    {
        /// <summary>
        /// Gets or sets the date time styles used when converting a date to and from JSON.
        /// </summary>
        /// <value>The date time styles used when converting a date to and from JSON.</value>
        public DateTimeStyles DateTimeStyles
        {
            get
            {
                return _dateTimeStyles;
            }
            set
            {
                _dateTimeStyles = value;
            }
        }

        /// <summary>
        /// Gets or sets the date time format used when converting a date to and from JSON.
        /// </summary>
        /// <value>The date time format used when converting a date to and from JSON.</value>
        public string DateTimeFormat
        {
            get
            {
                return _dateTimeFormat ?? string.Empty;
            }
            set
            {
                _dateTimeFormat = (string.IsNullOrEmpty(value) ? null : value);
            }
        }

        /// <summary>
        /// Gets or sets the culture used when converting a date to and from JSON.
        /// </summary>
        /// <value>The culture used when converting a date to and from JSON.</value>
        public CultureInfo Culture
        {
            get
            {
                return _culture ?? CultureInfo.CurrentCulture;
            }
            set
            {
                _culture = value;
            }
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string value2;
            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                {
                    dateTime = dateTime.ToUniversalTime();
                }
                value2 = dateTime.ToString(_dateTimeFormat ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", Culture);
            }
            else
            {
                if (!(value is DateTimeOffset))
                {
                    throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.".FormatWith(CultureInfo.InvariantCulture, ReflectionUtils.GetObjectType(value)));
                }
                DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
                if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                {
                    dateTimeOffset = dateTimeOffset.ToUniversalTime();
                }
                value2 = dateTimeOffset.ToString(_dateTimeFormat ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", Culture);
            }
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
            bool flag = ReflectionUtils.IsNullableType(objectType);
            Type type = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullableType(objectType))
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
                }
                return null;
            }
            else if (reader.TokenType == JsonToken.Date)
            {
                if (type == typeof(DateTimeOffset))
                {
                    if (!(reader.Value is DateTimeOffset))
                    {
                        return new DateTimeOffset((DateTime)reader.Value);
                    }
                    return reader.Value;
                }
                else
                {
                    if (reader.Value is DateTimeOffset)
                    {
                        return ((DateTimeOffset)reader.Value).DateTime;
                    }
                    return reader.Value;
                }
            }
            else
            {
                if (reader.TokenType != JsonToken.String)
                {
                    throw JsonSerializationException.Create(reader, "Unexpected token parsing date. Expected String, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
                string text = reader.Value.ToString();
                if (string.IsNullOrEmpty(text) && flag)
                {
                    return null;
                }
                if (type == typeof(DateTimeOffset))
                {
                    if (!string.IsNullOrEmpty(_dateTimeFormat))
                    {
                        return DateTimeOffset.ParseExact(text, _dateTimeFormat, Culture, _dateTimeStyles);
                    }
                    return DateTimeOffset.Parse(text, Culture, _dateTimeStyles);
                }
                else
                {
                    if (!string.IsNullOrEmpty(_dateTimeFormat))
                    {
                        return DateTime.ParseExact(text, _dateTimeFormat, Culture, _dateTimeStyles);
                    }
                    return DateTime.Parse(text, Culture, _dateTimeStyles);
                }
            }
        }

        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;

        private string _dateTimeFormat;

        private CultureInfo _culture;
    }
}
