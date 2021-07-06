using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
    /// </summary>
    public enum FloatParseHandling
    {
        /// <summary>
        /// Floating point numbers are parsed to <see cref="F:Newtonsoft.Json.FloatParseHandling.Double" />.
        /// </summary>
        Double,
        /// <summary>
        /// Floating point numbers are parsed to <see cref="F:Newtonsoft.Json.FloatParseHandling.Decimal" />.
        /// </summary>
        Decimal
    }
}
