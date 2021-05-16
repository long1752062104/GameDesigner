using System;

namespace Newtonsoft.Json.Bson
{
    internal enum BsonBinaryType : byte
    {
        Binary,
        Function,
        [Obsolete("This type has been deprecated in the BSON specification. Use Binary instead.")]
        BinaryOld,
        [Obsolete("This type has been deprecated in the BSON specification. Use Uuid instead.")]
        UuidOld,
        Uuid,
        Md5,
        UserDefined = 128
    }
}
