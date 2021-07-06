using System;

namespace Newtonsoft_X.Json.Converters
{
    /// <summary>
    /// Provides a base class for converting a <see cref="T:System.DateTime" /> to and from JSON.
    /// </summary>
    public abstract class DateTimeConverterBase : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?) || (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?));
        }
    }
}
