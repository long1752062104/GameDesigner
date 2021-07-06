using System;

namespace Newtonsoft_X.Json.Bson
{
    internal class BsonRegex : BsonToken
    {
        public BsonString Pattern { get; set; }

        public BsonString Options { get; set; }

        public BsonRegex(string pattern, string options)
        {
            Pattern = new BsonString(pattern, false);
            Options = new BsonString(options, false);
        }

        public override BsonType Type
        {
            get
            {
                return BsonType.Regex;
            }
        }
    }
}
