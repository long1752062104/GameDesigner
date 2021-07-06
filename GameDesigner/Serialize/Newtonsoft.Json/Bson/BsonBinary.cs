using System;

namespace Newtonsoft_X.Json.Bson
{
    internal class BsonBinary : BsonValue
    {
        public BsonBinaryType BinaryType { get; set; }

        public BsonBinary(byte[] value, BsonBinaryType binaryType) : base(value, BsonType.Binary)
        {
            BinaryType = binaryType;
        }
    }
}
