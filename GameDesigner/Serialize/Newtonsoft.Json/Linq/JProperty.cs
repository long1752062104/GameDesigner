using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Represents a JSON property.
    /// </summary>
    public class JProperty : JContainer
    {
        /// <summary>
        /// Gets the container's children tokens.
        /// </summary>
        /// <value>The container's children tokens.</value>
        protected override IList<JToken> ChildrenTokens
        {
            get
            {
                return _content;
            }
        }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        /// <value>The property name.</value>
        public string Name
        {
            [DebuggerStepThrough]
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        /// <value>The property value.</value>
        public JToken Value
        {
            [DebuggerStepThrough]
            get
            {
                return _content._token;
            }
            set
            {
                base.CheckReentrancy();
                JToken item = value ?? JValue.CreateNull();
                if (_content._token == null)
                {
                    InsertItem(0, item, false);
                    return;
                }
                SetItem(0, item);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JProperty" /> class from another <see cref="T:Newtonsoft.Json.Linq.JProperty" /> object.
        /// </summary>
        /// <param name="other">A <see cref="T:Newtonsoft.Json.Linq.JProperty" /> object to copy from.</param>
        public JProperty(JProperty other) : base(other)
        {
            _name = other.Name;
        }

        internal override JToken GetItem(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            return Value;
        }

        internal override void SetItem(int index, JToken item)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (JContainer.IsTokenUnchanged(Value, item))
            {
                return;
            }
            if (base.Parent != null)
            {
                ((JObject)base.Parent).InternalPropertyChanging(this);
            }
            base.SetItem(0, item);
            if (base.Parent != null)
            {
                ((JObject)base.Parent).InternalPropertyChanged(this);
            }
        }

        internal override bool RemoveItem(JToken item)
        {
            throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
        }

        internal override void RemoveItemAt(int index)
        {
            throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
        }

        internal override int IndexOfItem(JToken item)
        {
            return _content.IndexOf(item);
        }

        internal override void InsertItem(int index, JToken item, bool skipParentCheck)
        {
            if (item != null && item.Type == JTokenType.Comment)
            {
                return;
            }
            if (Value != null)
            {
                throw new JsonException("{0} cannot have multiple values.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
            }
            base.InsertItem(0, item, false);
        }

        internal override bool ContainsItem(JToken item)
        {
            return Value == item;
        }

        internal override void MergeItem(object content, JsonMergeSettings settings)
        {
            JProperty jproperty = content as JProperty;
            if (jproperty == null)
            {
                return;
            }
            if (jproperty.Value != null && jproperty.Value.Type != JTokenType.Null)
            {
                Value = jproperty.Value;
            }
        }

        internal override void ClearItems()
        {
            throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
        }

        internal override bool DeepEquals(JToken node)
        {
            JProperty jproperty = node as JProperty;
            return jproperty != null && _name == jproperty.Name && base.ContentsEqual(jproperty);
        }

        internal override JToken CloneToken()
        {
            return new JProperty(this);
        }

        /// <summary>
        /// Gets the node type for this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <value>The type.</value>
        public override JTokenType Type
        {
            [DebuggerStepThrough]
            get
            {
                return JTokenType.Property;
            }
        }

        internal JProperty(string name)
        {
            ValidationUtils.ArgumentNotNull(name, "name");
            _name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JProperty" /> class.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="content">The property content.</param>
        public JProperty(string name, params object[] content) : this(name/*, content*/)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JProperty" /> class.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="content">The property content.</param>
        public JProperty(string name, object content)
        {
            ValidationUtils.ArgumentNotNull(name, "name");
            _name = name;
            Value = (base.IsMultiContent(content) ? new JArray(content) : JContainer.CreateFromContent(content));
        }

        /// <summary>
        /// Writes this token to a <see cref="T:Newtonsoft.Json.JsonWriter" />.
        /// </summary>
        /// <param name="writer">A <see cref="T:Newtonsoft.Json.JsonWriter" /> into which this method will write.</param>
        /// <param name="converters">A collection of <see cref="T:Newtonsoft.Json.JsonConverter" /> which will be used when writing the token.</param>
        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WritePropertyName(_name);
            JToken value = Value;
            if (value != null)
            {
                value.WriteTo(writer, converters);
                return;
            }
            writer.WriteNull();
        }

        internal override int GetDeepHashCode()
        {
            return _name.GetHashCode() ^ ((Value != null) ? Value.GetDeepHashCode() : 0);
        }

        /// <summary>
        /// Loads a <see cref="T:Newtonsoft.Json.Linq.JProperty" /> from a <see cref="T:Newtonsoft.Json.JsonReader" />.
        /// </summary>
        /// <param name="reader">A <see cref="T:Newtonsoft.Json.JsonReader" /> that will be read for the content of the <see cref="T:Newtonsoft.Json.Linq.JProperty" />.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JProperty" /> that contains the JSON that was read from the specified <see cref="T:Newtonsoft.Json.JsonReader" />.</returns>
        public new static JProperty Load(JsonReader reader)
        {
            return JProperty.Load(reader, null);
        }

        /// <summary>
        /// Loads a <see cref="T:Newtonsoft.Json.Linq.JProperty" /> from a <see cref="T:Newtonsoft.Json.JsonReader" />.
        /// </summary>
        /// <param name="reader">A <see cref="T:Newtonsoft.Json.JsonReader" /> that will be read for the content of the <see cref="T:Newtonsoft.Json.Linq.JProperty" />.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.Linq.JsonLoadSettings" /> used to load the JSON.
        /// If this is <c>null</c>, default load settings will be used.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JProperty" /> that contains the JSON that was read from the specified <see cref="T:Newtonsoft.Json.JsonReader" />.</returns>
        public new static JProperty Load(JsonReader reader, JsonLoadSettings settings)
        {
            if (reader.TokenType == JsonToken.None && !reader.Read())
            {
                throw JsonReaderException.Create(reader, "Error reading JProperty from JsonReader.");
            }
            reader.MoveToContent();
            if (reader.TokenType != JsonToken.PropertyName)
            {
                throw JsonReaderException.Create(reader, "Error reading JProperty from JsonReader. Current JsonReader item is not a property: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            JProperty jproperty = new JProperty((string)reader.Value);
            jproperty.SetLineInfo(reader as IJsonLineInfo, settings);
            jproperty.ReadTokenFrom(reader, settings);
            return jproperty;
        }

        private readonly JProperty.JPropertyList _content = new JProperty.JPropertyList();

        private readonly string _name;

        private class JPropertyList : IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable
        {
            public IEnumerator<JToken> GetEnumerator()
            {
                if (_token != null)
                {
                    yield return _token;
                }
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(JToken item)
            {
                _token = item;
            }

            public void Clear()
            {
                _token = null;
            }

            public bool Contains(JToken item)
            {
                return _token == item;
            }

            public void CopyTo(JToken[] array, int arrayIndex)
            {
                if (_token != null)
                {
                    array[arrayIndex] = _token;
                }
            }

            public bool Remove(JToken item)
            {
                if (_token == item)
                {
                    _token = null;
                    return true;
                }
                return false;
            }

            public int Count
            {
                get
                {
                    if (_token == null)
                    {
                        return 0;
                    }
                    return 1;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public int IndexOf(JToken item)
            {
                if (_token != item)
                {
                    return -1;
                }
                return 0;
            }

            public void Insert(int index, JToken item)
            {
                if (index == 0)
                {
                    _token = item;
                }
            }

            public void RemoveAt(int index)
            {
                if (index == 0)
                {
                    _token = null;
                }
            }

            public JToken this[int index]
            {
                get
                {
                    if (index != 0)
                    {
                        return null;
                    }
                    return _token;
                }
                set
                {
                    if (index == 0)
                    {
                        _token = value;
                    }
                }
            }

            internal JToken _token;
        }
    }
}
