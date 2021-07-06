using System;
using System.Collections.Generic;

namespace Newtonsoft_X.Json.Serialization
{
    /// <summary>
    /// Gets extension data for an object during serialization.
    /// </summary>
    /// <param name="o">The object to set extension data on.</param>
    public delegate IEnumerable<KeyValuePair<object, object>> ExtensionDataGetter(object o);
}
