using System;

namespace Newtonsoft_X.Json.Bson
{
    internal enum BsonType : sbyte
    {
        Number = 1,
        String,
        Object,
        Array,
        Binary,
        Undefined,
        Oid,
        Boolean,
        Date,
        Null,
        Regex,
        Reference,
        Code,
        Symbol,
        CodeWScope,
        Integer,
        TimeStamp,
        Long,
        MinKey = -1,
        MaxKey = 127
    }
}
