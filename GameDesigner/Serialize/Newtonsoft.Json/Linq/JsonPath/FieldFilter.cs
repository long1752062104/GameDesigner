using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft_X.Json.Linq.JsonPath
{
    internal class FieldFilter : PathFilter
    {
        public string Name { get; set; }

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken t in current)
            {
                if (t is JObject o)
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
                    }
                }
                else if (errorWhenNoMatch)
                {
                    throw new JsonException("Property '{0}' not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, Name ?? "*", t.GetType().Name));
                }
            }
            yield break;
        }
    }
}
