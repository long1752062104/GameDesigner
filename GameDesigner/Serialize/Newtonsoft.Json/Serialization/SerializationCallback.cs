using System;
using System.Runtime.Serialization;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Handles <see cref="T:Newtonsoft.Json.JsonSerializer" /> serialization callback events.
    /// </summary>
    /// <param name="o">The object that raised the callback event.</param>
    /// <param name="context">The streaming context.</param>
    public delegate void SerializationCallback(object o, StreamingContext context);
}
