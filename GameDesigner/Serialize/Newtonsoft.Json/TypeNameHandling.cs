using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies type name handling options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </summary>
    /// <remarks>
    /// <see cref="P:Newtonsoft.Json.JsonSerializer.TypeNameHandling" /> should be used with caution when your application deserializes JSON from an external source.
    /// Incoming types should be validated with a custom <see cref="P:Newtonsoft.Json.JsonSerializer.SerializationBinder" />
    /// when deserializing with a value other than <see cref="F:Newtonsoft.Json.TypeNameHandling.None" />.
    /// </remarks>
    [Flags]
    public enum TypeNameHandling
    {
        /// <summary>
        /// Do not include the .NET type name when serializing types.
        /// </summary>
        None = 0,
        /// <summary>
        /// Include the .NET type name when serializing into a JSON object structure.
        /// </summary>
        Objects = 1,
        /// <summary>
        /// Include the .NET type name when serializing into a JSON array structure.
        /// </summary>
        Arrays = 2,
        /// <summary>
        /// Always include the .NET type name when serializing.
        /// </summary>
        All = 3,
        /// <summary>
        /// Include the .NET type name when the type of the object being serialized is not the same as its declared type.
        /// Note that this doesn't include the root serialized object by default. To include the root object's type name in JSON
        /// you must specify a root type object with <see cref="M:Newtonsoft.Json.JsonConvert.SerializeObject(System.Object,System.Type,Newtonsoft.Json.JsonSerializerSettings)" />
        /// or <see cref="M:Newtonsoft.Json.JsonSerializer.Serialize(Newtonsoft.Json.JsonWriter,System.Object,System.Type)" />.
        /// </summary>
        Auto = 4
    }
}
