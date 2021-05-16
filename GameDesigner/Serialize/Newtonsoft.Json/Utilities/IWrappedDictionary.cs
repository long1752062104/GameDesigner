using System;
using System.Collections;

namespace Newtonsoft.Json.Utilities
{
    internal interface IWrappedDictionary : IDictionary, ICollection, IEnumerable
    {
        object UnderlyingDictionary { get; }
    }
}
