using System;
using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft_X.Json.Bson
{
    internal class BsonArray : BsonToken, IEnumerable<BsonToken>, IEnumerable
    {
        public void Add(BsonToken token)
        {
            _children.Add(token);
            token.Parent = this;
        }

        public override BsonType Type
        {
            get
            {
                return BsonType.Array;
            }
        }

        public IEnumerator<BsonToken> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly List<BsonToken> _children = new List<BsonToken>();
    }
}
