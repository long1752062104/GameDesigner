using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Newtonsoft.Json.Linq.JsonPath
{
    internal class FieldMultipleFilter : PathFilter
    {
        public List<string> Names { get; set; }

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken t in current)
            {
                JObject o = t as JObject;
                if (o != null)
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
                        //name = null;
                    }
                    List<string>.Enumerator enumerator2 = default(List<string>.Enumerator);
                }
                else if (errorWhenNoMatch)
                {
                    throw new JsonException("Properties {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", (from n in Names
                                                                                                                                           select "'" + n + "'").ToArray<string>()), t.GetType().Name));
                }
                o = null;
                //t = null;
            }
            IEnumerator<JToken> enumerator = null;
            yield break;
            yield break;
        }
    }
}
