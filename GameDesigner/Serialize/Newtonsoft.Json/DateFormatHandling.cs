using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies how dates are formatted when writing JSON text.
    /// </summary>
    public enum DateFormatHandling
    {
        /// <summary>
        /// Dates are written in the ISO 8601 format, e.g. <c>"2012-03-21T05:40Z"</c>.
        /// </summary>
        IsoDateFormat,
        /// <summary>
        /// Dates are written in the Microsoft JSON format, e.g. <c>"\/Date(1198908717056)\/"</c>.
        /// </summary>
        MicrosoftDateFormat
    }
}
