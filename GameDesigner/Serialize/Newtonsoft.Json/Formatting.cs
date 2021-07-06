using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies formatting options for the <see cref="T:Newtonsoft.Json.JsonTextWriter" />.
    /// </summary>
    public enum Formatting
    {
        /// <summary>
        /// No special formatting is applied. This is the default.
        /// </summary>
        None,
        /// <summary>
        /// Causes child objects to be indented according to the <see cref="P:Newtonsoft.Json.JsonTextWriter.Indentation" /> and <see cref="P:Newtonsoft.Json.JsonTextWriter.IndentChar" /> settings.
        /// </summary>
        Indented
    }
}
