using System;
using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft_X.Json.Bson
{
    internal class BsonObject : BsonToken, IEnumerable<BsonProperty>, IEnumerable
    {
        public void Add(string name, BsonToken token)
        {
            _children.Add(new BsonProperty
            {
                Name = new BsonString(name, false),
                Value = token
            });
            token.Parent = this;
        }

        public override BsonType Type
        {
            get
            {
                return BsonType.Object;
            }
        }

        public IEnumerator<BsonProperty> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly List<BsonProperty> _children = new List<BsonProperty>();
    }
}
