using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies how object creation is handled by the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </summary>
    public enum ObjectCreationHandling
    {
        /// <summary>
        /// Reuse existing objects, create new objects when needed.
        /// </summary>
        Auto,
        /// <summary>
        /// Only reuse existing objects.
        /// </summary>
        Reuse,
        /// <summary>
        /// Always create new objects.
        /// </summary>
        Replace
    }
}
