using System;
using System.Collections;

namespace Newtonsoft_X.Json.Utilities
{
    internal interface IWrappedDictionary : IDictionary, ICollection, IEnumerable
    {
        object UnderlyingDictionary { get; }
    }
}
