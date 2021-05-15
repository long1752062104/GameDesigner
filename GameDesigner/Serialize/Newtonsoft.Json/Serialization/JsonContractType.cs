using System;

namespace Newtonsoft.Json.Serialization
{
    internal enum JsonContractType
    {
        None,
        Object,
        Array,
        Primitive,
        String,
        Dictionary,
        Dynamic,
        Serializable,
        Linq
    }
}
