using Newtonsoft_X.Json.Linq.JsonPath;
using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Represents an abstract JSON token.
    /// </summary>
    public abstract class JToken : IJEnumerable<JToken>, IEnumerable<JToken>, IEnumerable, IJsonLineInfo, ICloneable
    {
        /// <summary>
        /// Gets a comparer that can compare two tokens for value equality.
        /// </summary>
        /// <value>A <see cref="T:Newtonsoft.Json.Linq.JTokenEqualityComparer" /> that can compare two nodes for value equality.</value>
        public static JTokenEqualityComparer EqualityComparer
        {
            get
            {
                if (_equalityComparer == null)
                {
                    _equalityComparer = new JTokenEqualityComparer();
                }
                return _equalityComparer;
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public JContainer Parent
        {
            [DebuggerStepThrough]
            get
            {
                return _parent;
            }
            internal set
            {
                _parent = value;
            }
        }

        /// <summary>
        /// Gets the root <see cref="T:Newtonsoft.Json.Linq.JToken" /> of this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <value>The root <see cref="T:Newtonsoft.Json.Linq.JToken" /> of this <see cref="T:Newtonsoft.Json.Linq.JToken" />.</value>
        public JToken Root
        {
            get
            {
                JContainer parent = Parent;
                if (parent == null)
                {
                    return this;
                }
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }
                return parent;
            }
        }

        internal abstract JToken CloneToken();

        internal abstract bool DeepEquals(JToken node);

        /// <summary>
        /// Gets the node type for this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <value>The type.</value>
        public abstract JTokenType Type { get; }

        /// <summary>
        /// Gets a value indicating whether this token has child tokens.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this token has child values; otherwise, <c>false</c>.
        /// </value>
        public abstract bool HasValues { get; }

        /// <summary>
        /// Compares the values of two tokens, including the values of all descendant tokens.
        /// </summary>
        /// <param name="t1">The first <see cref="T:Newtonsoft.Json.Linq.JToken" /> to compare.</param>
        /// <param name="t2">The second <see cref="T:Newtonsoft.Json.Linq.JToken" /> to compare.</param>
        /// <returns><c>true</c> if the tokens are equal; otherwise <c>false</c>.</returns>
        public static bool DeepEquals(JToken t1, JToken t2)
        {
            return t1 == t2 || (t1 != null && t2 != null && t1.DeepEquals(t2));
        }

        /// <summary>
        /// Gets the next sibling token of this node.
        /// </summary>
        /// <value>The <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the next sibling token.</value>
        public JToken Next
        {
            get
            {
                return _next;
            }
            internal set
            {
                _next = value;
            }
        }

        /// <summary>
        /// Gets the previous sibling token of this node.
        /// </summary>
        /// <value>The <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the previous sibling token.</value>
        public JToken Previous
        {
            get
            {
                return _previous;
            }
            internal set
            {
                _previous = value;
            }
        }

        /// <summary>
        /// Gets the path of the JSON token. 
        /// </summary>
        public string Path
        {
            get
            {
                if (Parent == null)
                {
                    return string.Empty;
                }
                List<JsonPosition> list = new List<JsonPosition>();
                JToken jtoken = null;
                for (JToken jtoken2 = this; jtoken2 != null; jtoken2 = jtoken2.Parent)
                {
                    switch (jtoken2.Type)
                    {
                        case JTokenType.Array:
                        case JTokenType.Constructor:
                            if (jtoken != null)
                            {
                                int position = ((IList<JToken>)jtoken2).IndexOf(jtoken);
                                List<JsonPosition> list2 = list;
                                JsonPosition item = new JsonPosition(JsonContainerType.Array)
                                {
                                    Position = position
                                };
                                list2.Add(item);
                            }
                            break;
                        case JTokenType.Property:
                            {
                                JProperty jproperty = (JProperty)jtoken2;
                                List<JsonPosition> list3 = list;
                                JsonPosition item = new JsonPosition(JsonContainerType.Object)
                                {
                                    PropertyName = jproperty.Name
                                };
                                list3.Add(item);
                                break;
                            }
                    }
                    jtoken = jtoken2;
                }
                list.Reverse();
                return JsonPosition.BuildPath(list, null);
            }
        }

        internal JToken()
        {
        }

        /// <summary>
        /// Adds the specified content immediately after this token.
        /// </summary>
        /// <param name="content">A content object that contains simple content or a collection of content objects to be added after this token.</param>
        public void AddAfterSelf(object content)
        {
            if (_parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }
            int num = _parent.IndexOfItem(this);
            _parent.AddInternal(num + 1, content, false);
        }

        /// <summary>
        /// Adds the specified content immediately before this token.
        /// </summary>
        /// <param name="content">A content object that contains simple content or a collection of content objects to be added before this token.</param>
        public void AddBeforeSelf(object content)
        {
            if (_parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }
            int index = _parent.IndexOfItem(this);
            _parent.AddInternal(index, content, false);
        }

        /// <summary>
        /// Returns a collection of the ancestor tokens of this token.
        /// </summary>
        /// <returns>A collection of the ancestor tokens of this token.</returns>
        public IEnumerable<JToken> Ancestors()
        {
            return GetAncestors(false);
        }

        /// <summary>
        /// Returns a collection of tokens that contain this token, and the ancestors of this token.
        /// </summary>
        /// <returns>A collection of tokens that contain this token, and the ancestors of this token.</returns>
        public IEnumerable<JToken> AncestorsAndSelf()
        {
            return GetAncestors(true);
        }

        internal IEnumerable<JToken> GetAncestors(bool self)
        {
            JToken current;
            for (current = (self ? this : Parent); current != null; current = current.Parent)
            {
                yield return current;
            }
            yield break;
        }

        /// <summary>
        /// Returns a collection of the sibling tokens after this token, in document order.
        /// </summary>
        /// <returns>A collection of the sibling tokens after this tokens, in document order.</returns>
        public IEnumerable<JToken> AfterSelf()
        {
            if (Parent == null)
            {
                yield break;
            }
            JToken o;
            for (o = Next; o != null; o = o.Next)
            {
                yield return o;
            }
            yield break;
        }

        /// <summary>
        /// Returns a collection of the sibling tokens before this token, in document order.
        /// </summary>
        /// <returns>A collection of the sibling tokens before this token, in document order.</returns>
        public IEnumerable<JToken> BeforeSelf()
        {
            JToken o;
            for (o = Parent.First; o != this; o = o.Next)
            {
                yield return o;
            }

            yield break;
        }

        /// <summary>
        /// Gets the <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified key.
        /// </summary>
        /// <value>The <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified key.</value>
        public virtual JToken this[object key]
        {
            get
            {
                throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
            }
            set
            {
                throw new InvalidOperationException("Cannot set child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
            }
        }

        /// <summary>
        /// Gets the <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified key converted to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to convert the token to.</typeparam>
        /// <param name="key">The token key.</param>
        /// <returns>The converted token value.</returns>
        public virtual T Value<T>(object key)
        {
            JToken jtoken = this[key];
            if (jtoken != null)
            {
                return jtoken.Convert<JToken, T>();
            }
            return default;
        }

        /// <summary>
        /// Get the first child token of this token.
        /// </summary>
        /// <value>A <see cref="T:Newtonsoft.Json.Linq.JToken" /> containing the first child token of the <see cref="T:Newtonsoft.Json.Linq.JToken" />.</value>
        public virtual JToken First
        {
            get
            {
                throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
            }
        }

        /// <summary>
        /// Get the last child token of this token.
        /// </summary>
        /// <value>A <see cref="T:Newtonsoft.Json.Linq.JToken" /> containing the last child token of the <see cref="T:Newtonsoft.Json.Linq.JToken" />.</value>
        public virtual JToken Last
        {
            get
            {
                throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
            }
        }

        /// <summary>
        /// Returns a collection of the child tokens of this token, in document order.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> containing the child tokens of this <see cref="T:Newtonsoft.Json.Linq.JToken" />, in document order.</returns>
        public virtual JEnumerable<JToken> Children()
        {
            return JEnumerable<JToken>.Empty;
        }

        /// <summary>
        /// Returns a collection of the child tokens of this token, in document order, filtered by the specified type.
        /// </summary>
        /// <typeparam name="T">The type to filter the child tokens on.</typeparam>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JEnumerable`1" /> containing the child tokens of this <see cref="T:Newtonsoft.Json.Linq.JToken" />, in document order.</returns>
        public JEnumerable<T> Children<T>() where T : JToken
        {
            return new JEnumerable<T>(Children().OfType<T>());
        }

        /// <summary>
        /// Returns a collection of the child values of this token, in document order.
        /// </summary>
        /// <typeparam name="T">The type to convert the values to.</typeparam>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerable`1" /> containing the child values of this <see cref="T:Newtonsoft.Json.Linq.JToken" />, in document order.</returns>
        public virtual IEnumerable<T> Values<T>()
        {
            throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
        }

        /// <summary>
        /// Removes this token from its parent.
        /// </summary>
        public void Remove()
        {
            if (_parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }
            _parent.RemoveItem(this);
        }

        /// <summary>
        /// Replaces this token with the specified token.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Replace(JToken value)
        {
            if (_parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }
            _parent.ReplaceItem(this, value);
        }

        /// <summary>
        /// Writes this token to a <see cref="T:Newtonsoft.Json.JsonWriter" />.
        /// </summary>
        /// <param name="writer">A <see cref="T:Newtonsoft.Json.JsonWriter" /> into which this method will write.</param>
        /// <param name="converters">A collection of <see cref="T:Newtonsoft.Json.JsonConverter" /> which will be used when writing the token.</param>
        public abstract void WriteTo(JsonWriter writer, params JsonConverter[] converters);

        /// <summary>
        /// Returns the indented JSON for this token.
        /// </summary>
        /// <remarks>
        /// <c>ToString()</c> returns a non-JSON string value for tokens with a type of <see cref="F:Newtonsoft.Json.Linq.JTokenType.String" />.
        /// If you want the JSON for all token types then you should use <see cref="M:Newtonsoft.Json.Linq.WriteTo(Newtonsoft.Json.JsonWriter,Newtonsoft.Json.JsonConverter[])" />.
        /// </remarks>
        /// <returns>
        /// The indented JSON for this token.
        /// </returns>
        public override string ToString()
        {
            return ToString(Formatting.Indented, new JsonConverter[0]);
        }

        /// <summary>
        /// Returns the JSON for this token using the given formatting and converters.
        /// </summary>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="converters">A collection of <see cref="T:Newtonsoft.Json.JsonConverter" />s which will be used when writing the token.</param>
        /// <returns>The JSON for this token using the given formatting and converters.</returns>
        public string ToString(Formatting formatting, params JsonConverter[] converters)
        {
            string result;
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                WriteTo(new JsonTextWriter(stringWriter)
                {
                    Formatting = formatting
                }, converters);
                result = stringWriter.ToString();
            }
            return result;
        }

        private static JValue EnsureValue(JToken value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value is JProperty)
            {
                value = ((JProperty)value).Value;
            }
            return value as JValue;
        }

        private static string GetType(JToken token)
        {
            ValidationUtils.ArgumentNotNull(token, "token");
            if (token is JProperty)
            {
                token = ((JProperty)token).Value;
            }
            return token.Type.ToString();
        }

        private static bool ValidateToken(JToken o, JTokenType[] validTypes, bool nullable)
        {
            return Array.IndexOf(validTypes, o.Type) != -1 || (nullable && (o.Type == JTokenType.Null || o.Type == JTokenType.Undefined));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Boolean" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator bool(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, BooleanTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToBoolean(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.DateTimeOffset" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator DateTimeOffset(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, DateTimeTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value is DateTimeOffset)
            {
                return (DateTimeOffset)jvalue.Value;
            }
            if (jvalue.Value is string)
            {
                return DateTimeOffset.Parse((string)jvalue.Value, CultureInfo.InvariantCulture);
            }
            return new DateTimeOffset(Convert.ToDateTime(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator bool?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, BooleanTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new bool?(Convert.ToBoolean(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator long(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToInt64(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator DateTime?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, DateTimeTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value is DateTimeOffset)
            {
                return new DateTime?(((DateTimeOffset)jvalue.Value).DateTime);
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new DateTime?(Convert.ToDateTime(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator DateTimeOffset?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, DateTimeTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            if (jvalue.Value is DateTimeOffset)
            {
                return (DateTimeOffset?)jvalue.Value;
            }
            if (jvalue.Value is string)
            {
                return new DateTimeOffset?(DateTimeOffset.Parse((string)jvalue.Value, CultureInfo.InvariantCulture));
            }
            return new DateTimeOffset?(new DateTimeOffset(Convert.ToDateTime(jvalue.Value, CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator decimal?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new decimal?(Convert.ToDecimal(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator double?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new double?(Convert.ToDouble(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Char" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator char?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, CharTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new char?(Convert.ToChar(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Int32" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator int(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToInt32(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Int16" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator short(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToInt16(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.UInt16" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        //[CLSCompliant(false)]
        public static explicit operator ushort(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToUInt16(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Char" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        //[CLSCompliant(false)]
        public static explicit operator char(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, CharTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToChar(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Byte" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator byte(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToByte(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.SByte" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        //[CLSCompliant(false)]
        public static explicit operator sbyte(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToSByte(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" /> .
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator int?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new int?(Convert.ToInt32(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int16" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator short?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new short?(Convert.ToInt16(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt16" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        //[CLSCompliant(false)]
        public static explicit operator ushort?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new ushort?(Convert.ToUInt16(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Byte" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator byte?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new byte?(Convert.ToByte(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.SByte" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        //[CLSCompliant(false)]
        public static explicit operator sbyte?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new sbyte?(Convert.ToSByte(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator DateTime(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, DateTimeTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value is DateTimeOffset)
            {
                return ((DateTimeOffset)jvalue.Value).DateTime;
            }
            return Convert.ToDateTime(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator long?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new long?(Convert.ToInt64(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator float?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new float?(Convert.ToSingle(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Decimal" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator decimal(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToDecimal(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt32" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        //[CLSCompliant(false)]
        public static explicit operator uint?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new uint?(Convert.ToUInt32(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt64" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        //[CLSCompliant(false)]
        public static explicit operator ulong?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new ulong?(Convert.ToUInt64(jvalue.Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Double" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator double(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToDouble(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Single" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator float(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToSingle(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.String" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator string(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, StringTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to String.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            if (jvalue.Value is byte[])
            {
                return Convert.ToBase64String((byte[])jvalue.Value);
            }
            return Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.UInt32" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        //[CLSCompliant(false)]
        public static explicit operator uint(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToUInt32(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.UInt64" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        //[CLSCompliant(false)]
        public static explicit operator ulong(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            return Convert.ToUInt64(jvalue.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Byte" />[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator byte[](JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, BytesTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value is string)
            {
                return Convert.FromBase64String(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture));
            }
            if (jvalue.Value is byte[])
            {
                return (byte[])jvalue.Value;
            }
            throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Guid" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Guid(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, GuidTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value is byte[])
            {
                return new Guid((byte[])jvalue.Value);
            }
            if (!(jvalue.Value is Guid))
            {
                return new Guid(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture));
            }
            return (Guid)jvalue.Value;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Guid" /> .
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Guid?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, GuidTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            if (jvalue.Value is byte[])
            {
                return new Guid?(new Guid((byte[])jvalue.Value));
            }
            return new Guid?((jvalue.Value is Guid) ? ((Guid)jvalue.Value) : new Guid(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.TimeSpan" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator TimeSpan(JToken value)
        {
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, TimeSpanTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (!(jvalue.Value is TimeSpan))
            {
                return ConvertUtils.ParseTimeSpan(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture));
            }
            return (TimeSpan)jvalue.Value;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Nullable`1" /> of <see cref="T:System.TimeSpan" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator TimeSpan?(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, TimeSpanTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            return new TimeSpan?((jvalue.Value is TimeSpan) ? ((TimeSpan)jvalue.Value) : ConvertUtils.ParseTimeSpan(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:Newtonsoft.Json.Linq.JToken" /> to <see cref="T:System.Uri" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Uri(JToken value)
        {
            if (value == null)
            {
                return null;
            }
            JValue jvalue = EnsureValue(value);
            if (jvalue == null || !ValidateToken(jvalue, UriTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Uri.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }
            if (jvalue.Value == null)
            {
                return null;
            }
            if (!(jvalue.Value is Uri))
            {
                return new Uri(Convert.ToString(jvalue.Value, CultureInfo.InvariantCulture));
            }
            return (Uri)jvalue.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Boolean" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(bool value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.DateTimeOffset" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(DateTimeOffset value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Byte" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(byte value)
        {
            return new JValue((long)((ulong)value));
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.Byte" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(byte? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.SByte" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        //[CLSCompliant(false)]
        public static implicit operator JToken(sbyte value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.SByte" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        //[CLSCompliant(false)]
        public static implicit operator JToken(sbyte? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(bool? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(long value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(DateTime? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(DateTimeOffset? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(decimal? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(double? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Int16" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        //[CLSCompliant(false)]
        public static implicit operator JToken(short value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.UInt16" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        //[CLSCompliant(false)]
        public static implicit operator JToken(ushort value)
        {
            return new JValue((long)((ulong)value));
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Int32" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(int value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(int? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.DateTime" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(DateTime value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(long? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(float? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Decimal" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(decimal value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int16" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        //[CLSCompliant(false)]
        public static implicit operator JToken(short? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt16" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        //[CLSCompliant(false)]
        public static implicit operator JToken(ushort? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt32" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        //[CLSCompliant(false)]
        public static implicit operator JToken(uint? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt64" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        //[CLSCompliant(false)]
        public static implicit operator JToken(ulong? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Double" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(double value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Single" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(float value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.String" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(string value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.UInt32" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        //[CLSCompliant(false)]
        public static implicit operator JToken(uint value)
        {
            return new JValue((long)((ulong)value));
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.UInt64" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        //[CLSCompliant(false)]
        public static implicit operator JToken(ulong value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Byte" />[] to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(byte[] value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Uri" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(Uri value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.TimeSpan" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(TimeSpan value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.TimeSpan" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(TimeSpan? value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Guid" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(Guid value)
        {
            return new JValue(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Nullable`1" /> of <see cref="T:System.Guid" /> to <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="value">The value to create a <see cref="T:Newtonsoft.Json.Linq.JValue" /> from.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JValue" /> initialized with the specified value.</returns>
        public static implicit operator JToken(Guid? value)
        {
            return new JValue(value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<JToken>)this).GetEnumerator();
        }

        IEnumerator<JToken> IEnumerable<JToken>.GetEnumerator()
        {
            return Children().GetEnumerator();
        }

        internal abstract int GetDeepHashCode();

        IJEnumerable<JToken> IJEnumerable<JToken>.this[object key]
        {
            get
            {
                return this[key];
            }
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.JsonReader" /> for this token.
        /// </summary>
        /// <returns>A <see cref="T:Newtonsoft.Json.JsonReader" /> that can be used to read this token and its descendants.</returns>
        public JsonReader CreateReader()
        {
            return new JTokenReader(this);
        }

        internal static JToken FromObjectInternal(object o, JsonSerializer jsonSerializer)
        {
            ValidationUtils.ArgumentNotNull(o, "o");
            ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");
            JToken token;
            using (JTokenWriter jtokenWriter = new JTokenWriter())
            {
                jsonSerializer.Serialize(jtokenWriter, o);
                token = jtokenWriter.Token;
            }
            return token;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JToken" /> from an object.
        /// </summary>
        /// <param name="o">The object that will be used to create <see cref="T:Newtonsoft.Json.Linq.JToken" />.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the value of the specified object.</returns>
        public static JToken FromObject(object o)
        {
            return FromObjectInternal(o, JsonSerializer.CreateDefault());
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JToken" /> from an object using the specified <see cref="T:Newtonsoft.Json.JsonSerializer" />.
        /// </summary>
        /// <param name="o">The object that will be used to create <see cref="T:Newtonsoft.Json.Linq.JToken" />.</param>
        /// <param name="jsonSerializer">The <see cref="T:Newtonsoft.Json.JsonSerializer" /> that will be used when reading the object.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the value of the specified object.</returns>
        public static JToken FromObject(object o, JsonSerializer jsonSerializer)
        {
            return FromObjectInternal(o, jsonSerializer);
        }

        /// <summary>
        /// Creates an instance of the specified .NET type from the <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <typeparam name="T">The object type that the token will be deserialized to.</typeparam>
        /// <returns>The new object created from the JSON value.</returns>
        public T ToObject<T>()
        {
            return (T)ToObject(typeof(T));
        }

        /// <summary>
        /// Creates an instance of the specified .NET type from the <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="objectType">The object type that the token will be deserialized to.</param>
        /// <returns>The new object created from the JSON value.</returns>
        public object ToObject(Type objectType)
        {
            if (JsonConvert.DefaultSettings == null)
            {
                PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(objectType, out bool flag);
                if (flag)
                {
                    if (Type == JTokenType.String)
                    {
                        try
                        {
                            return ToObject(objectType, JsonSerializer.CreateDefault());
                        }
                        catch (Exception innerException)
                        {
                            Type type = objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType);
                            throw new ArgumentException("Could not convert '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, (string)this, type.Name), innerException);
                        }
                    }
                    if (Type == JTokenType.Integer)
                    {
                        return Enum.ToObject(objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType), ((JValue)this).Value);
                    }
                }
                switch (typeCode)
                {
                    case PrimitiveTypeCode.Char:
                        return (char)this;
                    case PrimitiveTypeCode.CharNullable:
                        return (char?)this;
                    case PrimitiveTypeCode.Boolean:
                        return (bool)this;
                    case PrimitiveTypeCode.BooleanNullable:
                        return (bool?)this;
                    case PrimitiveTypeCode.SByte:
                        return (sbyte?)this;
                    case PrimitiveTypeCode.SByteNullable:
                        return (sbyte)this;
                    case PrimitiveTypeCode.Int16:
                        return (short)this;
                    case PrimitiveTypeCode.Int16Nullable:
                        return (short?)this;
                    case PrimitiveTypeCode.UInt16:
                        return (ushort)this;
                    case PrimitiveTypeCode.UInt16Nullable:
                        return (ushort?)this;
                    case PrimitiveTypeCode.Int32:
                        return (int)this;
                    case PrimitiveTypeCode.Int32Nullable:
                        return (int?)this;
                    case PrimitiveTypeCode.Byte:
                        return (byte)this;
                    case PrimitiveTypeCode.ByteNullable:
                        return (byte?)this;
                    case PrimitiveTypeCode.UInt32:
                        return (uint)this;
                    case PrimitiveTypeCode.UInt32Nullable:
                        return (uint?)this;
                    case PrimitiveTypeCode.Int64:
                        return (long)this;
                    case PrimitiveTypeCode.Int64Nullable:
                        return (long?)this;
                    case PrimitiveTypeCode.UInt64:
                        return (ulong)this;
                    case PrimitiveTypeCode.UInt64Nullable:
                        return (ulong?)this;
                    case PrimitiveTypeCode.Single:
                        return (float)this;
                    case PrimitiveTypeCode.SingleNullable:
                        return (float?)this;
                    case PrimitiveTypeCode.Double:
                        return (double)this;
                    case PrimitiveTypeCode.DoubleNullable:
                        return (double?)this;
                    case PrimitiveTypeCode.DateTime:
                        return (DateTime)this;
                    case PrimitiveTypeCode.DateTimeNullable:
                        return (DateTime?)this;
                    case PrimitiveTypeCode.DateTimeOffset:
                        return (DateTimeOffset)this;
                    case PrimitiveTypeCode.DateTimeOffsetNullable:
                        return (DateTimeOffset?)this;
                    case PrimitiveTypeCode.Decimal:
                        return (decimal)this;
                    case PrimitiveTypeCode.DecimalNullable:
                        return (decimal?)this;
                    case PrimitiveTypeCode.Guid:
                        return (Guid)this;
                    case PrimitiveTypeCode.GuidNullable:
                        return (Guid?)this;
                    case PrimitiveTypeCode.TimeSpan:
                        return (TimeSpan)this;
                    case PrimitiveTypeCode.TimeSpanNullable:
                        return (TimeSpan?)this;
                    case PrimitiveTypeCode.Uri:
                        return (Uri)this;
                    case PrimitiveTypeCode.String:
                        return (string)this;
                }
            }
            return ToObject(objectType, JsonSerializer.CreateDefault());
        }

        /// <summary>
        /// Creates an instance of the specified .NET type from the <see cref="T:Newtonsoft.Json.Linq.JToken" /> using the specified <see cref="T:Newtonsoft.Json.JsonSerializer" />.
        /// </summary>
        /// <typeparam name="T">The object type that the token will be deserialized to.</typeparam>
        /// <param name="jsonSerializer">The <see cref="T:Newtonsoft.Json.JsonSerializer" /> that will be used when creating the object.</param>
        /// <returns>The new object created from the JSON value.</returns>
        public T ToObject<T>(JsonSerializer jsonSerializer)
        {
            return (T)ToObject(typeof(T), jsonSerializer);
        }

        /// <summary>
        /// Creates an instance of the specified .NET type from the <see cref="T:Newtonsoft.Json.Linq.JToken" /> using the specified <see cref="T:Newtonsoft.Json.JsonSerializer" />.
        /// </summary>
        /// <param name="objectType">The object type that the token will be deserialized to.</param>
        /// <param name="jsonSerializer">The <see cref="T:Newtonsoft.Json.JsonSerializer" /> that will be used when creating the object.</param>
        /// <returns>The new object created from the JSON value.</returns>
        public object ToObject(Type objectType, JsonSerializer jsonSerializer)
        {
            ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");
            object result;
            using (JTokenReader jtokenReader = new JTokenReader(this))
            {
                result = jsonSerializer.Deserialize(jtokenReader, objectType);
            }
            return result;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JToken" /> from a <see cref="T:Newtonsoft.Json.JsonReader" />.
        /// </summary>
        /// <param name="reader">A <see cref="T:Newtonsoft.Json.JsonReader" /> positioned at the token to read into this <see cref="T:Newtonsoft.Json.Linq.JToken" />.</param>
        /// <returns>
        /// A <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the token and its descendant tokens
        /// that were read from the reader. The runtime type of the token is determined
        /// by the token type of the first token encountered in the reader.
        /// </returns>
        public static JToken ReadFrom(JsonReader reader)
        {
            return ReadFrom(reader, null);
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JToken" /> from a <see cref="T:Newtonsoft.Json.JsonReader" />.
        /// </summary>
        /// <param name="reader">An <see cref="T:Newtonsoft.Json.JsonReader" /> positioned at the token to read into this <see cref="T:Newtonsoft.Json.Linq.JToken" />.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.Linq.JsonLoadSettings" /> used to load the JSON.
        /// If this is <c>null</c>, default load settings will be used.</param>
        /// <returns>
        /// A <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the token and its descendant tokens
        /// that were read from the reader. The runtime type of the token is determined
        /// by the token type of the first token encountered in the reader.
        /// </returns>
        public static JToken ReadFrom(JsonReader reader, JsonLoadSettings settings)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            if (reader.TokenType == JsonToken.None && !((settings != null && settings.CommentHandling == CommentHandling.Ignore) ? reader.ReadAndMoveToContent() : reader.Read()))
            {
                throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader.");
            }
            IJsonLineInfo lineInfo = reader as IJsonLineInfo;
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return JObject.Load(reader, settings);
                case JsonToken.StartArray:
                    return JArray.Load(reader, settings);
                case JsonToken.StartConstructor:
                    return JConstructor.Load(reader, settings);
                case JsonToken.PropertyName:
                    return JProperty.Load(reader, settings);
                case JsonToken.Comment:
                    {
                        JValue jvalue = JValue.CreateComment(reader.Value.ToString());
                        jvalue.SetLineInfo(lineInfo, settings);
                        return jvalue;
                    }
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    {
                        JValue jvalue2 = new JValue(reader.Value);
                        jvalue2.SetLineInfo(lineInfo, settings);
                        return jvalue2;
                    }
                case JsonToken.Null:
                    {
                        JValue jvalue3 = JValue.CreateNull();
                        jvalue3.SetLineInfo(lineInfo, settings);
                        return jvalue3;
                    }
                case JsonToken.Undefined:
                    {
                        JValue jvalue4 = JValue.CreateUndefined();
                        jvalue4.SetLineInfo(lineInfo, settings);
                        return jvalue4;
                    }
            }
            throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader. Unexpected token: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
        }

        /// <summary>
        /// Load a <see cref="T:Newtonsoft.Json.Linq.JToken" /> from a string that contains JSON.
        /// </summary>
        /// <param name="json">A <see cref="T:System.String" /> that contains JSON.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JToken" /> populated from the string that contains JSON.</returns>
        public static JToken Parse(string json)
        {
            return Parse(json, null);
        }

        /// <summary>
        /// Load a <see cref="T:Newtonsoft.Json.Linq.JToken" /> from a string that contains JSON.
        /// </summary>
        /// <param name="json">A <see cref="T:System.String" /> that contains JSON.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.Linq.JsonLoadSettings" /> used to load the JSON.
        /// If this is <c>null</c>, default load settings will be used.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JToken" /> populated from the string that contains JSON.</returns>
        public static JToken Parse(string json, JsonLoadSettings settings)
        {
            JToken result;
            using (JsonReader jsonReader = new JsonTextReader(new StringReader(json)))
            {
                JToken jtoken = Load(jsonReader, settings);
                if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
                {
                    throw JsonReaderException.Create(jsonReader, "Additional text found in JSON string after parsing content.");
                }
                result = jtoken;
            }
            return result;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JToken" /> from a <see cref="T:Newtonsoft.Json.JsonReader" />.
        /// </summary>
        /// <param name="reader">A <see cref="T:Newtonsoft.Json.JsonReader" /> positioned at the token to read into this <see cref="T:Newtonsoft.Json.Linq.JToken" />.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.Linq.JsonLoadSettings" /> used to load the JSON.
        /// If this is <c>null</c>, default load settings will be used.</param>
        /// <returns>
        /// A <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the token and its descendant tokens
        /// that were read from the reader. The runtime type of the token is determined
        /// by the token type of the first token encountered in the reader.
        /// </returns>
        public static JToken Load(JsonReader reader, JsonLoadSettings settings)
        {
            return ReadFrom(reader, settings);
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JToken" /> from a <see cref="T:Newtonsoft.Json.JsonReader" />.
        /// </summary>
        /// <param name="reader">A <see cref="T:Newtonsoft.Json.JsonReader" /> positioned at the token to read into this <see cref="T:Newtonsoft.Json.Linq.JToken" />.</param>
        /// <returns>
        /// A <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the token and its descendant tokens
        /// that were read from the reader. The runtime type of the token is determined
        /// by the token type of the first token encountered in the reader.
        /// </returns>
        public static JToken Load(JsonReader reader)
        {
            return Load(reader, null);
        }

        internal void SetLineInfo(IJsonLineInfo lineInfo, JsonLoadSettings settings)
        {
            if (settings != null && settings.LineInfoHandling == LineInfoHandling.Load)
            {
                return;
            }
            if (lineInfo == null || !lineInfo.HasLineInfo())
            {
                return;
            }
            SetLineInfo(lineInfo.LineNumber, lineInfo.LinePosition);
        }

        internal void SetLineInfo(int lineNumber, int linePosition)
        {
            AddAnnotation(new LineInfoAnnotation(lineNumber, linePosition));
        }

        bool IJsonLineInfo.HasLineInfo()
        {
            return Annotation<LineInfoAnnotation>() != null;
        }

        int IJsonLineInfo.LineNumber
        {
            get
            {
                LineInfoAnnotation lineInfoAnnotation = Annotation<LineInfoAnnotation>();
                if (lineInfoAnnotation != null)
                {
                    return lineInfoAnnotation.LineNumber;
                }
                return 0;
            }
        }

        int IJsonLineInfo.LinePosition
        {
            get
            {
                LineInfoAnnotation lineInfoAnnotation = Annotation<LineInfoAnnotation>();
                if (lineInfoAnnotation != null)
                {
                    return lineInfoAnnotation.LinePosition;
                }
                return 0;
            }
        }

        /// <summary>
        /// Selects a <see cref="T:Newtonsoft.Json.Linq.JToken" /> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="T:System.String" /> that contains a JSONPath expression.
        /// </param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JToken" />, or <c>null</c>.</returns>
        public JToken SelectToken(string path)
        {
            return SelectToken(path, false);
        }

        /// <summary>
        /// Selects a <see cref="T:Newtonsoft.Json.Linq.JToken" /> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="T:System.String" /> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JToken" />.</returns>
        public JToken SelectToken(string path, bool errorWhenNoMatch)
        {
            JPath jpath = new JPath(path);
            JToken jtoken = null;
            foreach (JToken jtoken2 in jpath.Evaluate(this, errorWhenNoMatch))
            {
                if (jtoken != null)
                {
                    throw new JsonException("Path returned multiple tokens.");
                }
                jtoken = jtoken2;
            }
            return jtoken;
        }

        /// <summary>
        /// Selects a collection of elements using a JSONPath expression.
        /// </summary>
        /// <param name="path">
        /// A <see cref="T:System.String" /> that contains a JSONPath expression.
        /// </param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the selected elements.</returns>
        public IEnumerable<JToken> SelectTokens(string path)
        {
            return SelectTokens(path, false);
        }

        /// <summary>
        /// Selects a collection of elements using a JSONPath expression.
        /// </summary>
        /// <param name="path">
        /// A <see cref="T:System.String" /> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> that contains the selected elements.</returns>
        public IEnumerable<JToken> SelectTokens(string path, bool errorWhenNoMatch)
        {
            return new JPath(path).Evaluate(this, errorWhenNoMatch);
        }

        object ICloneable.Clone()
        {
            return DeepClone();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Newtonsoft.Json.Linq.JToken" />. All child tokens are recursively cloned.
        /// </summary>
        /// <returns>A new instance of the <see cref="T:Newtonsoft.Json.Linq.JToken" />.</returns>
        public JToken DeepClone()
        {
            return CloneToken();
        }

        /// <summary>
        /// Adds an object to the annotation list of this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="annotation">The annotation to add.</param>
        public void AddAnnotation(object annotation)
        {
            if (annotation == null)
            {
                throw new ArgumentNullException("annotation");
            }
            if (_annotations == null)
            {
                object annotations;
                if (!(annotation is object[]))
                {
                    annotations = annotation;
                }
                else
                {
                    annotations = new object[] { annotation };
                }
                _annotations = annotations;
                return;
            }
            object[] array = _annotations as object[];
            if (array == null)
            {
                _annotations = new object[]
                {
                    _annotations,
                    annotation
                };
                return;
            }
            int num = 0;
            while (num < array.Length && array[num] != null)
            {
                num++;
            }
            if (num == array.Length)
            {
                Array.Resize<object>(ref array, num * 2);
                _annotations = array;
            }
            array[num] = annotation;
        }

        /// <summary>
        /// Get the first annotation object of the specified type from this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <typeparam name="T">The type of the annotation to retrieve.</typeparam>
        /// <returns>The first annotation object that matches the specified type, or <c>null</c> if no annotation is of the specified type.</returns>
        public T Annotation<T>() where T : class
        {
            if (_annotations != null)
            {
                object[] array = _annotations as object[];
                if (array == null)
                {
                    return _annotations as T;
                }
                foreach (object obj in array)
                {
                    if (obj == null)
                    {
                        break;
                    }
                    T t = obj as T;
                    if (t != null)
                    {
                        return t;
                    }
                }
            }
            return default(T);
        }

        /// <summary>
        /// Gets the first annotation object of the specified type from this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="type">The <see cref="P:Newtonsoft.Json.Linq.Type" /> of the annotation to retrieve.</param>
        /// <returns>The first annotation object that matches the specified type, or <c>null</c> if no annotation is of the specified type.</returns>
        public object Annotation(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (_annotations != null)
            {
                object[] array = _annotations as object[];
                if (array == null)
                {
                    if (type.IsInstanceOfType(_annotations))
                    {
                        return _annotations;
                    }
                }
                else
                {
                    foreach (object obj in array)
                    {
                        if (obj == null)
                        {
                            break;
                        }
                        if (type.IsInstanceOfType(obj))
                        {
                            return obj;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a collection of annotations of the specified type for this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <typeparam name="T">The type of the annotations to retrieve.</typeparam>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the annotations for this <see cref="T:Newtonsoft.Json.Linq.JToken" />.</returns>
        public IEnumerable<T> Annotations<T>() where T : class
        {
            if (_annotations == null)
            {
                yield break;
            }
            object[] annotations = _annotations as object[];
            if (annotations != null)
            {
                int num;
                for (int i = 0; i < annotations.Length; i = num + 1)
                {
                    object obj = annotations[i];
                    if (obj == null)
                    {
                        break;
                    }
                    T t = obj as T;
                    if (t != null)
                    {
                        yield return t;
                    }
                    num = i;
                }
                yield break;
            }
            T t2 = _annotations as T;
            if (t2 == null)
            {
                yield break;
            }
            yield return t2;
            yield break;
        }

        /// <summary>
        /// Gets a collection of annotations of the specified type for this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="type">The <see cref="P:Newtonsoft.Json.Linq.Type" /> of the annotations to retrieve.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Object" /> that contains the annotations that match the specified type for this <see cref="T:Newtonsoft.Json.Linq.JToken" />.</returns>
        public IEnumerable<object> Annotations(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (_annotations == null)
            {
                yield break;
            }
            object[] annotations = _annotations as object[];
            if (annotations != null)
            {
                int num;
                for (int i = 0; i < annotations.Length; i = num + 1)
                {
                    object obj = annotations[i];
                    if (obj == null)
                    {
                        break;
                    }
                    if (type.IsInstanceOfType(obj))
                    {
                        yield return obj;
                    }
                    num = i;
                }
                yield break;
            }
            if (!type.IsInstanceOfType(_annotations))
            {
                yield break;
            }
            yield return _annotations;
            yield break;
        }

        /// <summary>
        /// Removes the annotations of the specified type from this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <typeparam name="T">The type of annotations to remove.</typeparam>
        public void RemoveAnnotations<T>() where T : class
        {
            if (_annotations != null)
            {
                object[] array = _annotations as object[];
                if (array == null)
                {
                    if (_annotations is T)
                    {
                        _annotations = null;
                        return;
                    }
                }
                else
                {
                    int i = 0;
                    int j = 0;
                    while (i < array.Length)
                    {
                        object obj = array[i];
                        if (obj == null)
                        {
                            break;
                        }
                        if (!(obj is T))
                        {
                            array[j++] = obj;
                        }
                        i++;
                    }
                    if (j != 0)
                    {
                        while (j < i)
                        {
                            array[j++] = null;
                        }
                        return;
                    }
                    _annotations = null;
                }
            }
        }

        /// <summary>
        /// Removes the annotations of the specified type from this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <param name="type">The <see cref="P:Newtonsoft.Json.Linq.Type" /> of annotations to remove.</param>
        public void RemoveAnnotations(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (_annotations != null)
            {
                object[] array = _annotations as object[];
                if (array == null)
                {
                    if (type.IsInstanceOfType(_annotations))
                    {
                        _annotations = null;
                        return;
                    }
                }
                else
                {
                    int i = 0;
                    int j = 0;
                    while (i < array.Length)
                    {
                        object obj = array[i];
                        if (obj == null)
                        {
                            break;
                        }
                        if (!type.IsInstanceOfType(obj))
                        {
                            array[j++] = obj;
                        }
                        i++;
                    }
                    if (j != 0)
                    {
                        while (j < i)
                        {
                            array[j++] = null;
                        }
                        return;
                    }
                    _annotations = null;
                }
            }
        }

        private static JTokenEqualityComparer _equalityComparer;

        private JContainer _parent;

        private JToken _previous;

        private JToken _next;

        private object _annotations;

        private static readonly JTokenType[] BooleanTypes = new JTokenType[]
        {
            JTokenType.Integer,
            JTokenType.Float,
            JTokenType.String,
            JTokenType.Comment,
            JTokenType.Raw,
            JTokenType.Boolean
        };

        private static readonly JTokenType[] NumberTypes = new JTokenType[]
        {
            JTokenType.Integer,
            JTokenType.Float,
            JTokenType.String,
            JTokenType.Comment,
            JTokenType.Raw,
            JTokenType.Boolean
        };

        private static readonly JTokenType[] StringTypes = new JTokenType[]
        {
            JTokenType.Date,
            JTokenType.Integer,
            JTokenType.Float,
            JTokenType.String,
            JTokenType.Comment,
            JTokenType.Raw,
            JTokenType.Boolean,
            JTokenType.Bytes,
            JTokenType.Guid,
            JTokenType.TimeSpan,
            JTokenType.Uri
        };

        private static readonly JTokenType[] GuidTypes = new JTokenType[]
        {
            JTokenType.String,
            JTokenType.Comment,
            JTokenType.Raw,
            JTokenType.Guid,
            JTokenType.Bytes
        };

        private static readonly JTokenType[] TimeSpanTypes = new JTokenType[]
        {
            JTokenType.String,
            JTokenType.Comment,
            JTokenType.Raw,
            JTokenType.TimeSpan
        };

        private static readonly JTokenType[] UriTypes = new JTokenType[]
        {
            JTokenType.String,
            JTokenType.Comment,
            JTokenType.Raw,
            JTokenType.Uri
        };

        private static readonly JTokenType[] CharTypes = new JTokenType[]
        {
            JTokenType.Integer,
            JTokenType.Float,
            JTokenType.String,
            JTokenType.Comment,
            JTokenType.Raw
        };

        private static readonly JTokenType[] DateTimeTypes = new JTokenType[]
        {
            JTokenType.Date,
            JTokenType.String,
            JTokenType.Comment,
            JTokenType.Raw
        };

        private static readonly JTokenType[] BytesTypes = new JTokenType[]
        {
            JTokenType.Bytes,
            JTokenType.String,
            JTokenType.Comment,
            JTokenType.Raw,
            JTokenType.Integer
        };

        private class LineInfoAnnotation
        {
            public LineInfoAnnotation(int lineNumber, int linePosition)
            {
                LineNumber = lineNumber;
                LinePosition = linePosition;
            }

            internal readonly int LineNumber;

            internal readonly int LinePosition;
        }
    }
}
