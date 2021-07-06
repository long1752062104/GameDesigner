using System;
using System.Runtime.Serialization;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// The exception thrown when an error occurs during JSON serialization or deserialization.
    /// </summary>
    [Serializable]
    public class JsonException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonException" /> class.
        /// </summary>
        public JsonException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonException" /> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public JsonException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonException" /> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
        public JsonException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> parameter is <c>null</c>.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is <c>null</c> or <see cref="P:System.Exception.HResult" /> is zero (0).</exception>
        public JsonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal static JsonException Create(IJsonLineInfo lineInfo, string path, string message)
        {
            message = JsonPosition.FormatMessage(lineInfo, path, message);
            return new JsonException(message);
        }
    }
}
