using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Contains the LINQ to JSON extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns a collection of tokens that contains the ancestors of every token in the source collection.
        /// </summary>
        /// <typeparam name="T">The type of the objects in source, constrained to <see cref="T:Newtonsoft.Json.Linq.JToken" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the ancestors of every token in the source collection.</returns>
        public static IJEnumerable<JToken> Ancestors<T>(this IEnumerable<T> source) where T : JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            return source.SelectMany((T j) => j.Ancestors()).AsJEnumerable();
        }

        /// <summary>
        /// Returns a collection of tokens that contains every token in the source collection, and the ancestors of every token in the source collection.
        /// </summary>
        /// <typeparam name="T">The type of the objects in source, constrained to <see cref="T:Newtonsoft.Json.Linq.JToken" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains every token in the source collection, the ancestors of every token in the source collection.</returns>
        public static IJEnumerable<JToken> AncestorsAndSelf<T>(this IEnumerable<T> source) where T : JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            return source.SelectMany((T j) => j.AncestorsAndSelf()).AsJEnumerable();
        }

        /// <summary>
        /// Returns a collection of tokens that contains the descendants of every token in the source collection.
        /// </summary>
        /// <typeparam name="T">The type of the objects in source, constrained to <see cref="T:Newtonsoft.Json.Linq.JContainer" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the descendants of every token in the source collection.</returns>
        public static IJEnumerable<JToken> Descendants<T>(this IEnumerable<T> source) where T : JContainer
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            return source.SelectMany((T j) => j.Descendants()).AsJEnumerable();
        }

        /// <summary>
        /// Returns a collection of tokens that contains every token in the source collection, and the descendants of every token in the source collection.
        /// </summary>
        /// <typeparam name="T">The type of the objects in source, constrained to <see cref="T:Newtonsoft.Json.Linq.JContainer" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains every token in the source collection, and the descendants of every token in the source collection.</returns>
        public static IJEnumerable<JToken> DescendantsAndSelf<T>(this IEnumerable<T> source) where T : JContainer
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            return source.SelectMany((T j) => j.DescendantsAndSelf()).AsJEnumerable();
        }

        /// <summary>
        /// Returns a collection of child properties of every object in the source collection.
        /// </summary>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JObject" /> that contains the source collection.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JProperty" /> that contains the properties of every object in the source collection.</returns>
        public static IJEnumerable<JProperty> Properties(this IEnumerable<JObject> source)
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            return source.SelectMany((JObject d) => d.Properties()).AsJEnumerable<JProperty>();
        }

        /// <summary>
        /// Returns a collection of child values of every object in the source collection with the given key.
        /// </summary>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <param name="key">The token key.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the values of every token in the source collection with the given key.</returns>
        public static IJEnumerable<JToken> Values(this IEnumerable<JToken> source, object key)
        {
            return source.Values(key).AsJEnumerable();
        }

        /// <summary>
        /// Returns a collection of child values of every object in the source collection.
        /// </summary>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the values of every token in the source collection.</returns>
        public static IJEnumerable<JToken> Values(this IEnumerable<JToken> source)
        {
            return source.Values(null);
        }

        /// <summary>
        /// Returns a collection of converted child values of every object in the source collection with the given key.
        /// </summary>
        /// <typeparam name="U">The type to convert the values to.</typeparam>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <param name="key">The token key.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the converted values of every token in the source collection with the given key.</returns>
        public static IEnumerable<U> Values<U>(this IEnumerable<JToken> source, object key)
        {
            return (IEnumerable<U>)source.Values(key);
        }

        /// <summary>
        /// Returns a collection of converted child values of every object in the source collection.
        /// </summary>
        /// <typeparam name="U">The type to convert the values to.</typeparam>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the converted values of every token in the source collection.</returns>
        public static IEnumerable<U> Values<U>(this IEnumerable<JToken> source)
        {
            return (IEnumerable<U>)source.Values(null);
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <typeparam name="U">The type to convert the value to.</typeparam>
        /// <param name="value">A <see cref="T:Newtonsoft.Json.Linq.JToken" /> cast as a <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" />.</param>
        /// <returns>A converted value.</returns>
        public static U Value<U>(this IEnumerable<JToken> value)
        {
            return value.Value<JToken, U>();
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <typeparam name="T">The source collection type.</typeparam>
        /// <typeparam name="U">The type to convert the value to.</typeparam>
        /// <param name="value">A <see cref="T:Newtonsoft.Json.Linq.JToken" /> cast as a <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" />.</param>
        /// <returns>A converted value.</returns>
        public static U Value<T, U>(this IEnumerable<T> value) where T : JToken
        {
            ValidationUtils.ArgumentNotNull(value, "value");
            JToken jtoken = value as JToken;
            if (jtoken == null)
            {
                throw new ArgumentException("Source value must be a JToken.");
            }
            return jtoken.Convert<JToken, U>();
        }

        internal static IEnumerable<U> Values<T, U>(this IEnumerable<T> source, object key) where T : JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            foreach (T t in source)
            {
                JToken token = t;
                if (key == null)
                {
                    if (token is JValue value)
                    {
                        yield return value.Convert<JValue, U>();
                    }
                    else
                    {
                        foreach (JToken token2 in token.Children())
                        {
                            yield return token2.Convert<JToken, U>();
                        }
                    }
                }
                else
                {
                    JToken jtoken = token[key];
                    if (jtoken != null)
                    {
                        yield return jtoken.Convert<JToken, U>();
                    }
                }
            }
            yield break;
        }

        /// <summary>
        /// Returns a collection of child tokens of every array in the source collection.
        /// </summary>
        /// <typeparam name="T">The source collection type.</typeparam>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the values of every token in the source collection.</returns>
        public static IJEnumerable<JToken> Children<T>(this IEnumerable<T> source) where T : JToken
        {
            return source.Children<T, JToken>().AsJEnumerable();
        }

        /// <summary>
        /// Returns a collection of converted child tokens of every array in the source collection.
        /// </summary>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <typeparam name="U">The type to convert the values to.</typeparam>
        /// <typeparam name="T">The source collection type.</typeparam>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the converted values of every token in the source collection.</returns>
        public static IEnumerable<U> Children<T, U>(this IEnumerable<T> source) where T : JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            return source.SelectMany((T c) => c.Children()).Convert<JToken, U>();
        }

        internal static IEnumerable<U> Convert<T, U>(this IEnumerable<T> source) where T : JToken
        {
            ValidationUtils.ArgumentNotNull(source, "source");
            foreach (T t in source)
            {
                yield return t.Convert<JToken, U>();
            }
            yield break;
        }

        internal static U Convert<T, U>(this T token) where T : JToken
        {
            if (token == null)
            {
                return default;
            }
            if (token is U && typeof(U) != typeof(IComparable) && typeof(U) != typeof(IFormattable))
            {
                return (U)(object)token;
            }
            if (!(token is JValue jvalue))
            {
                throw new InvalidCastException("Cannot cast {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, token.GetType(), typeof(T)));
            }
            if (jvalue.Value is U u)
            {
                return u;
            }
            Type type = typeof(U);
            if (ReflectionUtils.IsNullableType(type))
            {
                if (jvalue.Value == null)
                {
                    return default;
                }
                type = Nullable.GetUnderlyingType(type);
            }
            return (U)System.Convert.ChangeType(jvalue.Value, type, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the input typed as <see cref="T:Newtonsoft.Json.Linq.IJEnumerable`1" />.
        /// </summary>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <returns>The input typed as <see cref="T:Newtonsoft.Json.Linq.IJEnumerable`1" />.</returns>
        public static IJEnumerable<JToken> AsJEnumerable(this IEnumerable<JToken> source)
        {
            return source.AsJEnumerable<JToken>();
        }

        /// <summary>
        /// Returns the input typed as <see cref="T:Newtonsoft.Json.Linq.IJEnumerable`1" />.
        /// </summary>
        /// <typeparam name="T">The source collection type.</typeparam>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the source collection.</param>
        /// <returns>The input typed as <see cref="T:Newtonsoft.Json.Linq.IJEnumerable`1" />.</returns>
        public static IJEnumerable<T> AsJEnumerable<T>(this IEnumerable<T> source) where T : JToken
        {
            if (source == null)
            {
                return null;
            }
            if (source is IJEnumerable<T>)
            {
                return (IJEnumerable<T>)source;
            }
            return new JEnumerable<T>(source);
        }
    }
}
