using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies float format handling options when writing special floating point numbers, e.g. <see cref="F:System.Double.NaN" />,
    /// <see cref="F:System.Double.PositiveInfinity" /> and <see cref="F:System.Double.NegativeInfinity" /> with <see cref="T:Newtonsoft.Json.JsonWriter" />.
    /// </summary>
    public enum FloatFormatHandling
    {
        /// <summary>
        /// Write special floating point values as strings in JSON, e.g. <c>"NaN"</c>, <c>"Infinity"</c>, <c>"-Infinity"</c>.
        /// </summary>
        String,
        /// <summary>
        /// Write special floating point values as symbols in JSON, e.g. <c>NaN</c>, <c>Infinity</c>, <c>-Infinity</c>.
        /// Note that this will produce non-valid JSON.
        /// </summary>
        Symbol,
        /// <summary>
        /// Write special floating point values as the property's default value in JSON, e.g. 0.0 for a <see cref="T:System.Double" /> property, <c>null</c> for a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" /> property.
        /// </summary>
        DefaultValue
    }
}
