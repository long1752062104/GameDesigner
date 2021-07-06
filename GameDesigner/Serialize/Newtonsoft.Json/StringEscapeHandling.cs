using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies how strings are escaped when writing JSON text.
    /// </summary>
    public enum StringEscapeHandling
    {
        /// <summary>
        /// Only control characters (e.g. newline) are escaped.
        /// </summary>
        Default,
        /// <summary>
        /// All non-ASCII and control characters (e.g. newline) are escaped.
        /// </summary>
        EscapeNonAscii,
        /// <summary>
        /// HTML (&lt;, &gt;, &amp;, ', ") and control characters (e.g. newline) are escaped.
        /// </summary>
        EscapeHtml
    }
}
