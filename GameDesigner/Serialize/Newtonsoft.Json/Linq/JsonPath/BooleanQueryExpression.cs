using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Newtonsoft_X.Json.Linq.JsonPath
{
    internal class BooleanQueryExpression : QueryExpression
    {
        public List<PathFilter> Path { get; set; }

        public JValue Value { get; set; }

        public override bool IsMatch(JToken t)
        {
            foreach (JToken jtoken in JPath.Evaluate(Path, t, false))
            {
                JValue jvalue = jtoken as JValue;
                if (jvalue != null)
                {
                    switch (base.Operator)
                    {
                        case QueryOperator.Equals:
                            if (EqualsWithStringCoercion(jvalue, Value))
                            {
                                return true;
                            }
                            break;
                        case QueryOperator.NotEquals:
                            if (!EqualsWithStringCoercion(jvalue, Value))
                            {
                                return true;
                            }
                            break;
                        case QueryOperator.Exists:
                            return true;
                        case QueryOperator.LessThan:
                            if (jvalue.CompareTo(Value) < 0)
                            {
                                return true;
                            }
                            break;
                        case QueryOperator.LessThanOrEquals:
                            if (jvalue.CompareTo(Value) <= 0)
                            {
                                return true;
                            }
                            break;
                        case QueryOperator.GreaterThan:
                            if (jvalue.CompareTo(Value) > 0)
                            {
                                return true;
                            }
                            break;
                        case QueryOperator.GreaterThanOrEquals:
                            if (jvalue.CompareTo(Value) >= 0)
                            {
                                return true;
                            }
                            break;
                    }
                }
                else
                {
                    QueryOperator @operator = base.Operator;
                    if (@operator == QueryOperator.NotEquals || @operator == QueryOperator.Exists)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool EqualsWithStringCoercion(JValue value, JValue queryValue)
        {
            if (value.Equals(queryValue))
            {
                return true;
            }
            if (queryValue.Type != JTokenType.String)
            {
                return false;
            }
            string b = (string)queryValue.Value;
            string a;
            switch (value.Type)
            {
                case JTokenType.Date:
                    using (StringWriter stringWriter = StringUtils.CreateStringWriter(64))
                    {
                        if (value.Value is DateTimeOffset offset)
                        {
                            DateTimeUtils.WriteDateTimeOffsetString(stringWriter, offset, DateFormatHandling.IsoDateFormat, null, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            DateTimeUtils.WriteDateTimeString(stringWriter, (DateTime)value.Value, DateFormatHandling.IsoDateFormat, null, CultureInfo.InvariantCulture);
                        }
                        a = stringWriter.ToString();
                        goto IL_DF;
                    }
                case JTokenType.Raw:
                    return false;
                case JTokenType.Bytes:
                    break;
                case JTokenType.Guid:
                case JTokenType.TimeSpan:
                    a = value.Value.ToString();
                    goto IL_DF;
                case JTokenType.Uri:
                    a = ((Uri)value.Value).OriginalString;
                    goto IL_DF;
                default:
                    return false;
            }
            a = Convert.ToBase64String((byte[])value.Value);
        IL_DF:
            return string.Equals(a, b, StringComparison.Ordinal);
        }
    }
}
