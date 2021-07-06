using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft_X.Json.Linq.JsonPath
{
    internal class ArrayIndexFilter : PathFilter
    {
        public int? Index { get; set; }

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken t in current)
            {
                if (Index != null)
                {
                    JToken tokenIndex = GetTokenIndex(t, errorWhenNoMatch, Index.GetValueOrDefault());
                    if (tokenIndex != null)
                    {
                        yield return tokenIndex;
                    }
                }
                else if (t is JArray || t is JConstructor)
                {
                    foreach (JToken jtoken in t)
                    {
                        yield return jtoken;
                    }
                }
                else if (errorWhenNoMatch)
                {
                    throw new JsonException("Index * not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, t.GetType().Name));
                }
            }
            yield break;
        }
    }
}
