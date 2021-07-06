using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Instructs the <see cref="T:Newtonsoft.Json.JsonSerializer" /> not to serialize the public field or public read/write property value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonIgnoreAttribute : Attribute
    {
    }
}
