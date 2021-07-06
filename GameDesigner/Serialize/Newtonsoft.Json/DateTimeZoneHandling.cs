using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies how to treat the time value when converting between string and <see cref="T:System.DateTime" />.
    /// </summary>
    public enum DateTimeZoneHandling
    {
        /// <summary>
        /// Treat as local time. If the <see cref="T:System.DateTime" /> object represents a Coordinated Universal Time (UTC), it is converted to the local time.
        /// </summary>
        Local,
        /// <summary>
        /// Treat as a UTC. If the <see cref="T:System.DateTime" /> object represents a local time, it is converted to a UTC.
        /// </summary>
        Utc,
        /// <summary>
        /// Treat as a local time if a <see cref="T:System.DateTime" /> is being converted to a string.
        /// If a string is being converted to <see cref="T:System.DateTime" />, convert to a local time if a time zone is specified.
        /// </summary>
        Unspecified,
        /// <summary>
        /// Time zone information should be preserved when converting.
        /// </summary>
        RoundtripKind
    }
}
