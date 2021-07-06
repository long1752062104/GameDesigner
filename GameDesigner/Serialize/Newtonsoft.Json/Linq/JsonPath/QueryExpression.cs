using System;

namespace Newtonsoft_X.Json.Linq.JsonPath
{
    internal abstract class QueryExpression
    {
        public QueryOperator Operator { get; set; }

        public abstract bool IsMatch(JToken t);
    }
}
