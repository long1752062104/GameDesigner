using System;

namespace Newtonsoft.Json
{
    internal enum ReadType
    {
        Read,
        ReadAsInt32,
        ReadAsBytes,
        ReadAsString,
        ReadAsDecimal,
        ReadAsDateTime,
        ReadAsDateTimeOffset,
        ReadAsDouble,
        ReadAsBoolean
    }
}
