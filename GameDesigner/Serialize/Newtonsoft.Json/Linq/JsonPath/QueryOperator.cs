using System;

namespace Newtonsoft_X.Json.Linq.JsonPath
{
    internal enum QueryOperator
    {
        None,
        Equals,
        NotEquals,
        Exists,
        LessThan,
        LessThanOrEquals,
        GreaterThan,
        GreaterThanOrEquals,
        And,
        Or
    }
}
