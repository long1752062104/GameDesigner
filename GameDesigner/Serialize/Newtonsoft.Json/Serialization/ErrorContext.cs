using System;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Provides information surrounding an error.
    /// </summary>
    public class ErrorContext
    {
        internal ErrorContext(object originalObject, object member, string path, Exception error)
        {
            OriginalObject = originalObject;
            Member = member;
            Error = error;
            Path = path;
        }

        internal bool Traced { get; set; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error.</value>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the original object that caused the error.
        /// </summary>
        /// <value>The original object that caused the error.</value>
        public object OriginalObject { get; private set; }

        /// <summary>
        /// Gets the member that caused the error.
        /// </summary>
        /// <value>The member that caused the error.</value>
        public object Member { get; private set; }

        /// <summary>
        /// Gets the path of the JSON location where the error occurred.
        /// </summary>
        /// <value>The path of the JSON location where the error occurred.</value>
        public string Path { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Newtonsoft.Json.Serialization.ErrorContext" /> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled { get; set; }
    }
}
