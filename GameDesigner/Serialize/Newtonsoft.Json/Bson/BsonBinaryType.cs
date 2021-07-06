using System;

namespace Newtonsoft_X.Json.Bson
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
