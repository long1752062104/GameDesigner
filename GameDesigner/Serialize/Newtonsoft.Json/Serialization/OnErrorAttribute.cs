using System;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// When applied to a method, specifies that the method is called when an error occurs serializing an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class OnErrorAttribute : Attribute
    {
    }
}
