using Newtonsoft.Json.Utilities;
using System;

namespace Newtonsoft.Json.Serialization
{
    internal static class CachedAttributeGetter<T> where T : Attribute
    {
        public static T GetAttribute(object type)
        {
            return CachedAttributeGetter<T>.TypeAttributeCache.Get(type);
        }

        private static readonly ThreadSafeStore<object, T> TypeAttributeCache = new ThreadSafeStore<object, T>(new Func<object, T>(JsonTypeReflector.GetAttribute<T>));
    }
}
