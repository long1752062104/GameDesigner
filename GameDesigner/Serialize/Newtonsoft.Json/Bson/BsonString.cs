using System;

namespace Newtonsoft.Json.Bson
{
    internal class BsonString : BsonValue
    {
        public int ByteCount { get; set; }

        public bool IncludeLength { get; set; }

        public BsonString(object value, bool includeLength) : base(value, BsonType.String)
        {
            IncludeLength = includeLength;
        }
    }
}
