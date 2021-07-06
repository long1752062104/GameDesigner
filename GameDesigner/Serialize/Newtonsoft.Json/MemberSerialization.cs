using System;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Specifies the member serialization options for the <see cref="T:Newtonsoft.Json.JsonSerializer" />.
    /// </summary>
    public enum MemberSerialization
    {
        /// <summary>
        /// All public members are serialized by default. Members can be excluded using <see cref="T:Newtonsoft.Json.JsonIgnoreAttribute" /> or <see cref="T:System.NonSerializedAttribute" />.
        /// This is the default member serialization mode.
        /// </summary>
        OptOut,
        /// <summary>
        /// Only members marked with <see cref="T:Newtonsoft.Json.JsonPropertyAttribute" /> or <see cref="T:System.Runtime.Serialization.DataMemberAttribute" /> are serialized.
        /// This member serialization mode can also be set by marking the class with <see cref="T:System.Runtime.Serialization.DataContractAttribute" />.
        /// </summary>
        OptIn,
        /// <summary>
        /// All public and private fields are serialized. Members can be excluded using <see cref="T:Newtonsoft.Json.JsonIgnoreAttribute" /> or <see cref="T:System.NonSerializedAttribute" />.
        /// This member serialization mode can also be set by marking the class with <see cref="T:System.SerializableAttribute" />
        /// and setting IgnoreSerializableAttribute on <see cref="T:Newtonsoft.Json.Serialization.DefaultContractResolver" /> to <c>false</c>.
        /// </summary>
        Fields
    }
}
