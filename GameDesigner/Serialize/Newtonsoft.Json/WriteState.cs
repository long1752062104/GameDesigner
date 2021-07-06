using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies the state of the <see cref="T:Newtonsoft.Json.JsonWriter" />.
    /// </summary>
    public enum WriteState
    {
        /// <summary>
        /// An exception has been thrown, which has left the <see cref="T:Newtonsoft.Json.JsonWriter" /> in an invalid state.
        /// You may call the <see cref="M:Newtonsoft.Json.JsonWriter.Close" /> method to put the <see cref="T:Newtonsoft.Json.JsonWriter" /> in the <c>Closed</c> state.
        /// Any other <see cref="T:Newtonsoft.Json.JsonWriter" /> method calls result in an <see cref="T:System.InvalidOperationException" /> being thrown.
        /// </summary>
        Error,
        /// <summary>
        /// The <see cref="M:Newtonsoft.Json.JsonWriter.Close" /> method has been called.
        /// </summary>
        Closed,
        /// <summary>
        /// An object is being written. 
        /// </summary>
        Object,
        /// <summary>
        /// An array is being written.
        /// </summary>
        Array,
        /// <summary>
        /// A constructor is being written.
        /// </summary>
        Constructor,
        /// <summary>
        /// A property is being written.
        /// </summary>
        Property,
        /// <summary>
        /// A <see cref="T:Newtonsoft.Json.JsonWriter" /> write method has not been called.
        /// </summary>
        Start
    }
}
