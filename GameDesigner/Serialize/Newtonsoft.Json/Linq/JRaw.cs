using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Represents a raw JSON string.
    /// </summary>
    public class JRaw : JValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JRaw" /> class from another <see cref="T:Newtonsoft.Json.Linq.JRaw" /> object.
        /// </summary>
        /// <param name="other">A <see cref="T:Newtonsoft.Json.Linq.JRaw" /> object to copy from.</param>
        public JRaw(JRaw other) : base(other)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JRaw" /> class.
        /// </summary>
        /// <param name="rawJson">The raw json.</param>
        public JRaw(object rawJson) : base(rawJson, JTokenType.Raw)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="T:Newtonsoft.Json.Linq.JRaw" /> with the content of the reader's current token.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>An instance of <see cref="T:Newtonsoft.Json.Linq.JRaw" /> with the content of the reader's current token.</returns>
        public static JRaw Create(JsonReader reader)
        {
            JRaw result;
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.WriteToken(reader);
                    result = new JRaw(stringWriter.ToString());
                }
            }
            return result;
        }

        internal override JToken CloneToken()
        {
            return new JRaw(this);
        }
    }
}
