using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Newtonsoft_X.Json.Linq.JsonPath
{
    internal class FieldMultipleFilter : PathFilter
    {
        public List<string> Names { get; set; }

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken t in current)
            {
                if (t is JObject o)
                {
                    foreach (string name in Names)
                    {
                        JToken jtoken = o[name];
                        if (jtoken != null)
                        {
                            yield return jtoken;
                        }
                        if (errorWhenNoMatch)
                        {
                            throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, name));
                        }
                    }
                }
                else if (errorWhenNoMatch)
                {
                    throw new JsonException("Properties {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", (from n in Names
                                                                                                                                           select "'" + n + "'").ToArray<string>()), t.GetType().Name));
                }
                o = null;
            }
            yield break;
        }
    }
}
