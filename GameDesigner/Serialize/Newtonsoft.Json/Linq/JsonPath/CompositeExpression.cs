using System;
using System.Collections.Generic;

namespace Newtonsoft_X.Json.Linq.JsonPath
{
    internal class CompositeExpression : QueryExpression
    {
        public List<QueryExpression> Expressions { get; set; }

        public CompositeExpression()
        {
            Expressions = new List<QueryExpression>();
        }

        public override bool IsMatch(JToken t)
        {
            QueryOperator @operator = base.Operator;
            if (@operator == QueryOperator.And)
            {
                using (List<QueryExpression>.Enumerator enumerator = Expressions.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (!enumerator.Current.IsMatch(t))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            if (@operator != QueryOperator.Or)
            {
                throw new ArgumentOutOfRangeException();
            }
            using (List<QueryExpression>.Enumerator enumerator = Expressions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsMatch(t))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
