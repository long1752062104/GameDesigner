using System;
using System.Runtime.Serialization;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// The exception thrown when an error occurs while reading JSON text.
    /// </summary>
    [Serializable]
    public class JsonReaderException : JsonException
    {
        /// <summary>
        /// Gets the line number indicating where the error occurred.
        /// </summary>
        /// <value>The line number indicating where the error occurred.</value>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Gets the line position indicating where the error occurred.
        /// </summary>
        /// <value>The line position indicating where the error occurred.</value>
        public int LinePosition { get; private set; }

        /// <summary>
        /// Gets the path to the JSON where the error occurred.
        /// </summary>
        /// <value>The path to the JSON where the error occurred.</value>
        public string Path { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonReaderException" /> class.
        /// </summary>
        public JsonReaderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonReaderException" /> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public JsonReaderException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonReaderException" /> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
        public JsonReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonReaderException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> parameter is <c>null</c>.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is <c>null</c> or <see cref="P:System.Exception.HResult" /> is zero (0).</exception>
        public JsonReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal JsonReaderException(string message, Exception innerException, string path, int lineNumber, int linePosition) : base(message, innerException)
        {
            Path = path;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        internal static JsonReaderException Create(JsonReader reader, string message)
        {
            return JsonReaderException.Create(reader, message, null);
        }

        internal static JsonReaderException Create(JsonReader reader, string message, Exception ex)
        {
            return JsonReaderException.Create(reader as IJsonLineInfo, reader.Path, message, ex);
        }

        internal static JsonReaderException Create(IJsonLineInfo lineInfo, string path, string message, Exception ex)
        {
            message = JsonPosition.FormatMessage(lineInfo, path, message);
            int lineNumber;
            int linePosition;
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                lineNumber = lineInfo.LineNumber;
                linePosition = lineInfo.LinePosition;
            }
            else
            {
                lineNumber = 0;
                linePosition = 0;
            }
            return new JsonReaderException(message, ex, path, lineNumber, linePosition);
        }
    }
}
