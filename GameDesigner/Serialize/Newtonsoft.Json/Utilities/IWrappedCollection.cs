using System;
using System.Collections;

namespace Newtonsoft_X.Json.Utilities
{
    internal interface IWrappedCollection : IList, ICollection, IEnumerable
    {
        object UnderlyingCollection { get; }
    }
}
