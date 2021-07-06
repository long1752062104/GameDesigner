using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies missing member handling options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </summary>
    public enum MissingMemberHandling
    {
        /// <summary>
        /// Ignore a missing member and do not attempt to deserialize it.
        /// </summary>
        Ignore,
        /// <summary>
        /// Throw a <see cref="T:Newtonsoft.Json.JsonSerializationException" /> when a missing member is encountered during deserialization.
        /// </summary>
        Error
    }
}
