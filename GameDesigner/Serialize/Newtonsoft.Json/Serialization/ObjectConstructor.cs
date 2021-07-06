using System;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Represents a method that constructs an object.
    /// </summary>
    /// <typeparam name="T">The object type to create.</typeparam>
    public delegate object ObjectConstructor<T>(params object[] args);
}
