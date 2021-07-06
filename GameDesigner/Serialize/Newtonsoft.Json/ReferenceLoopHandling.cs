using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies reference loop handling options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </summary>
    public enum ReferenceLoopHandling
    {
        /// <summary>
        /// Throw a <see cref="T:Newtonsoft.Json.JsonSerializationException" /> when a loop is encountered.
        /// </summary>
        Error,
        /// <summary>
        /// Ignore loop references and do not serialize.
        /// </summary>
        Ignore,
        /// <summary>
        /// Serialize loop references.
        /// </summary>
        Serialize
    }
}
