using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies null value handling options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </summary>
    /// <example>
    ///   <code lang="cs" source="..\Src\Newtonsoft.Json.Tests\Documentation\SerializationTests.cs" region="ReducingSerializedJsonSizeNullValueHandlingObject" title="NullValueHandling Class" />
    ///   <code lang="cs" source="..\Src\Newtonsoft.Json.Tests\Documentation\SerializationTests.cs" region="ReducingSerializedJsonSizeNullValueHandlingExample" title="NullValueHandling Ignore Example" />
    /// </example>
    public enum NullValueHandling
    {
        /// <summary>
        /// Include null values when serializing and deserializing objects.
        /// </summary>
        Include,
        /// <summary>
        /// Ignore null values when serializing and deserializing objects.
        /// </summary>
        Ignore
    }
}
