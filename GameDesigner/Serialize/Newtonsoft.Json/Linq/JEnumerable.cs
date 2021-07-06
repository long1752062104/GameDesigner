using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Represents a collection of <see cref="T:Newtonsoft.Json.Linq.JToken" /> objects.
    /// </summary>
    /// <typeparam name="T">The type of token.</typeparam>
    public struct JEnumerable<T> : IJEnumerable<T>, IEnumerable<T>, IEnumerable, IEquatable<JEnumerable<T>> where T : JToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JEnumerable`1" /> struct.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public JEnumerable(IEnumerable<T> enumerable)
        {
            ValidationUtils.ArgumentNotNull(enumerable, "enumerable");
            _enumerable = enumerable;
        }

        /// <summary>
        /// Returns an enumerator that can be used to iterate through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (_enumerable == null)
            {
                return JEnumerable<T>.Empty.GetEnumerator();
            }
            return _enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the <see cref="T:Newtonsoft.Json.Linq.IJEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified key.
        /// </summary>
        /// <value></value>
        public IJEnumerable<JToken> this[object key]
        {
            get
            {
                if (_enumerable == null)
                {
                    return JEnumerable<JToken>.Empty;
                }
                return new JEnumerable<JToken>(_enumerable.Values(key));
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:Newtonsoft.Json.Linq.JEnumerable`1" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="T:Newtonsoft.Json.Linq.JEnumerable`1" /> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="T:Newtonsoft.Json.Linq.JEnumerable`1" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(JEnumerable<T> other)
        {
            return object.Equals(_enumerable, other._enumerable);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is JEnumerable<T> && Equals((JEnumerable<T>)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (_enumerable == null)
            {
                return 0;
            }
            return _enumerable.GetHashCode();
        }

        /// <summary>
        /// An empty collection of <see cref="T:Newtonsoft.Json.Linq.JToken" /> objects.
        /// </summary>
        public static readonly JEnumerable<T> Empty = new JEnumerable<T>(Enumerable.Empty<T>());

        private readonly IEnumerable<T> _enumerable;
    }
}
