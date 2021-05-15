using System;
using System.Collections;

namespace Newtonsoft.Json.Utilities
{
    internal interface IWrappedCollection : IList, ICollection, IEnumerable
    {
        object UnderlyingCollection { get; }
    }
}
