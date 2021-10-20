using System;

namespace Net.Serialize
{
    /// <summary>
    /// 极速序列化不序列化的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NonSerialized : Attribute
    {
    }
}
