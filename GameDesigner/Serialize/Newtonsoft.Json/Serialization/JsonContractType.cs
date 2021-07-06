using System;

namespace Newtonsoft_X.Json.Serialization
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
