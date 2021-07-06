using System;

namespace Newtonsoft_X.Json.Bson
{
    internal class BsonValue : BsonToken
    {
        public BsonValue(object value, BsonType type)
        {
            _value = value;
            _type = type;
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }

        public override BsonType Type
        {
            get
            {
                return _type;
            }
        }

        private readonly object _value;

        private readonly BsonType _type;
    }
}
