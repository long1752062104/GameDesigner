using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies reference handling options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// Note that references cannot be preserved when a value is set via a non-default constructor such as types that implement <see cref="T:System.Runtime.Serialization.ISerializable" />.
    /// </summary>
    /// <example>
    ///   <code lang="cs" source="..\Src\Newtonsoft.Json.Tests\Documentation\SerializationTests.cs" region="PreservingObjectReferencesOn" title="Preserve Object References" />       
    /// </example>
    [Flags]
    public enum PreserveReferencesHandling
    {
        /// <summary>
        /// Do not preserve references when serializing types.
        /// </summary>
        None = 0,
        /// <summary>
        /// Preserve references when serializing into a JSON object structure.
        /// </summary>
        Objects = 1,
        /// <summary>
        /// Preserve references when serializing into a JSON array structure.
        /// </summary>
        Arrays = 2,
        /// <summary>
        /// Preserve references when serializing.
        /// </summary>
        All = 3
    }
}
