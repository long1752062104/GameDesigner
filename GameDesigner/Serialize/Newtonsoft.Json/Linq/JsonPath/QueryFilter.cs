using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Linq.JsonPath
{
    internal class QueryFilter : PathFilter
    {
        public QueryExpression Expression { get; set; }

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken jtoken in current)
            {
                foreach (JToken jtoken2 in jtoken)
                {
                    if (Expression.IsMatch(jtoken2))
                    {
                        yield return jtoken2;
                    }
                }
                IEnumerator<JToken> enumerator2 = null;
            }
            IEnumerator<JToken> enumerator = null;
            yield break;
            yield break;
        }
    }
}
