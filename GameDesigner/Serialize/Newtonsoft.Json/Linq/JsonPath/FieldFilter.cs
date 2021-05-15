using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft.Json.Linq.JsonPath
{
    internal class FieldFilter : PathFilter
    {
        public string Name { get; set; }

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken t in current)
            {
                JObject o = t as JObject;
                if (o != null)
                {
                    if (Name != null)
                    {
                        JToken jtoken = o[Name];
                        if (jtoken != null)
                        {
                            yield return jtoken;
                        }
                        else if (errorWhenNoMatch)
                        {
                            throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, Name));
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string, JToken> keyValuePair in o)
                        {
                            yield return keyValuePair.Value;
                        }
                        IEnumerator<KeyValuePair<string, JToken>> enumerator2 = null;
                    }
                }
                else if (errorWhenNoMatch)
                {
                    throw new JsonException("Property '{0}' not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, Name ?? "*", t.GetType().Name));
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
