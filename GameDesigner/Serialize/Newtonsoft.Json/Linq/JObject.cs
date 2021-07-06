using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Represents a JSON object.
    /// </summary>
    /// <example>
    ///   <code lang="cs" source="..\Src\Newtonsoft.Json.Tests\Documentation\LinqToJsonTests.cs" region="LinqToJsonCreateParse" title="Parsing a JSON Object from Text" />
    /// </example>
    public class JObject : JContainer, IDictionary<string, JToken>, ICollection<KeyValuePair<string, JToken>>, IEnumerable<KeyValuePair<string, JToken>>, IEnumerable, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the container's children tokens.
        /// </summary>
        /// <value>The container's children tokens.</value>
        protected override IList<JToken> ChildrenTokens
        {
            get
            {
                return _properties;
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JObject" /> class.
        /// </summary>
        public JObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JObject" /> class from another <see cref="T:Newtonsoft.Json.Linq.JObject" /> object.
        /// </summary>
        /// <param name="other">A <see cref="T:Newtonsoft.Json.Linq.JObject" /> object to copy from.</param>
        public JObject(JObject other) : base(other)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JObject" /> class with the specified content.
        /// </summary>
        /// <param name="content">The contents of the object.</param>
        public JObject(params object[] content) : this(content[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JObject" /> class with the specified content.
        /// </summary>
        /// <param name="content">The contents of the object.</param>
        public JObject(object content)
        {
            Add(content);
        }

        internal override bool DeepEquals(JToken node)
        {
            JObject jobject = node as JObject;
            return jobject != null && _properties.Compare(jobject._properties);
        }

        internal override int IndexOfItem(JToken item)
        {
            return _properties.IndexOfReference(item);
        }

        internal override void InsertItem(int index, JToken item, bool skipParentCheck)
        {
            if (item != null && item.Type == JTokenType.Comment)
            {
                return;
            }
            base.InsertItem(index, item, skipParentCheck);
        }

        internal override void ValidateToken(JToken o, JToken existing)
        {
            ValidationUtils.ArgumentNotNull(o, "o");
            if (o.Type != JTokenType.Property)
            {
                throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), base.GetType()));
            }
            JProperty jproperty = (JProperty)o;
            if (existing != null)
            {
                JProperty jproperty2 = (JProperty)existing;
                if (jproperty.Name == jproperty2.Name)
                {
                    return;
                }
            }
            if (_properties.TryGetValue(jproperty.Name, out existing))
            {
                throw new ArgumentException("Can not add property {0} to {1}. Property with the same name already exists on object.".FormatWith(CultureInfo.InvariantCulture, jproperty.Name, base.GetType()));
            }
        }

        internal override void MergeItem(object content, JsonMergeSettings settings)
        {
            JObject jobject = content as JObject;
            if (jobject == null)
            {
                return;
            }
            foreach (KeyValuePair<string, JToken> keyValuePair in jobject)
            {
                JProperty jproperty = Property(keyValuePair.Key);
                if (jproperty == null)
                {
                    Add(keyValuePair.Key, keyValuePair.Value);
                }
                else if (keyValuePair.Value != null)
                {
                    JContainer jcontainer = jproperty.Value as JContainer;
                    if (jcontainer == null)
                    {
                        if (keyValuePair.Value.Type != JTokenType.Null || (settings != null && settings.MergeNullValueHandling == MergeNullValueHandling.Merge))
                        {
                            jproperty.Value = keyValuePair.Value;
                        }
                    }
                    else if (jcontainer.Type != keyValuePair.Value.Type)
                    {
                        jproperty.Value = keyValuePair.Value;
                    }
                    else
                    {
                        jcontainer.Merge(keyValuePair.Value, settings);
                    }
                }
            }
        }

        internal void InternalPropertyChanged(JProperty childProperty)
        {
            OnPropertyChanged(childProperty.Name);
        }

        internal void InternalPropertyChanging(JProperty childProperty)
        {
        }

        internal override JToken CloneToken()
        {
            return new JObject(this);
        }

        /// <summary>
        /// Gets the node type for this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <value>The type.</value>
        public override JTokenType Type
        {
            get
            {
                return JTokenType.Object;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JProperty" /> of this object's properties.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JProperty" /> of this object's properties.</returns>
        public IEnumerable<JProperty> Properties()
        {
            return _properties.Cast<JProperty>();
        }

        /// <summary>
        /// Gets a <see cref="T:Newtonsoft.Json.Linq.JProperty" /> with the specified name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JProperty" /> with the specified name or <c>null</c>.</returns>
        public JProperty Property(string name)
        {
            if (name == null)
            {
                return null;
            }
            _properties.TryGetValue(name, out JToken jtoken);
            return (JProperty)jtoken;
        }

        /// <summary>
        /// Gets a <see cref="T:Newtonsoft.Json.Linq.JEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> of this object's property values.
        /// </summary>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JEnumerable`1" /> of <see cref="T:Newtonsoft.Json.Linq.JToken" /> of this object's property values.</returns>
        public JEnumerable<JToken> PropertyValues()
        {
            return new JEnumerable<JToken>(from p in Properties()
                                           select p.Value);
        }

        /// <summary>
        /// Gets the <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified key.
        /// </summary>
        /// <value>The <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified key.</value>
        public override JToken this[object key]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(key, "key");
                string text = key as string;
                if (text == null)
                {
                    throw new ArgumentException("Accessed JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }
                return this[text];
            }
            set
            {
                ValidationUtils.ArgumentNotNull(key, "key");
                string text = key as string;
                if (text == null)
                {
                    throw new ArgumentException("Set JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }
                this[text] = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified property name.
        /// </summary>
        /// <value></value>
        public JToken this[string propertyName]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(propertyName, "propertyName");
                JProperty jproperty = Property(propertyName);
                if (jproperty == null)
                {
                    return null;
                }
                return jproperty.Value;
            }
            set
            {
                JProperty jproperty = Property(propertyName);
                if (jproperty != null)
                {
                    jproperty.Value = value;
                    return;
                }
                Add(new JProperty(propertyName, value));
                OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Loads a <see cref="T:Newtonsoft.Json.Linq.JObject" /> from a <see cref="T:Newtonsoft.Json.JsonReader" />.
        /// </summary>
        /// <param name="reader">A <see cref="T:Newtonsoft.Json.JsonReader" /> that will be read for the content of the <see cref="T:Newtonsoft.Json.Linq.JObject" />.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JObject" /> that contains the JSON that was read from the specified <see cref="T:Newtonsoft.Json.JsonReader" />.</returns>
        /// <exception cref="T:Newtonsoft.Json.JsonReaderException">
        ///     <paramref name="reader" /> is not valid JSON.
        /// </exception>
        public new static JObject Load(JsonReader reader)
        {
            return JObject.Load(reader, null);
        }

        /// <summary>
        /// Loads a <see cref="T:Newtonsoft.Json.Linq.JObject" /> from a <see cref="T:Newtonsoft.Json.JsonReader" />.
        /// </summary>
        /// <param name="reader">A <see cref="T:Newtonsoft.Json.JsonReader" /> that will be read for the content of the <see cref="T:Newtonsoft.Json.Linq.JObject" />.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.Linq.JsonLoadSettings" /> used to load the JSON.
        /// If this is <c>null</c>, default load settings will be used.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JObject" /> that contains the JSON that was read from the specified <see cref="T:Newtonsoft.Json.JsonReader" />.</returns>
        /// <exception cref="T:Newtonsoft.Json.JsonReaderException">
        ///     <paramref name="reader" /> is not valid JSON.
        /// </exception>
        public new static JObject Load(JsonReader reader, JsonLoadSettings settings)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            if (reader.TokenType == JsonToken.None && !reader.Read())
            {
                throw JsonReaderException.Create(reader, "Error reading JObject from JsonReader.");
            }
            reader.MoveToContent();
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw JsonReaderException.Create(reader, "Error reading JObject from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            JObject jobject = new JObject();
            jobject.SetLineInfo(reader as IJsonLineInfo, settings);
            jobject.ReadTokenFrom(reader, settings);
            return jobject;
        }

        /// <summary>
        /// Load a <see cref="T:Newtonsoft.Json.Linq.JObject" /> from a string that contains JSON.
        /// </summary>
        /// <param name="json">A <see cref="T:System.String" /> that contains JSON.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JObject" /> populated from the string that contains JSON.</returns>
        /// <exception cref="T:Newtonsoft.Json.JsonReaderException">
        ///     <paramref name="json" /> is not valid JSON.
        /// </exception>
        /// <example>
        ///   <code lang="cs" source="..\Src\Newtonsoft.Json.Tests\Documentation\LinqToJsonTests.cs" region="LinqToJsonCreateParse" title="Parsing a JSON Object from Text" />
        /// </example>
        public new static JObject Parse(string json)
        {
            return JObject.Parse(json, null);
        }

        /// <summary>
        /// Load a <see cref="T:Newtonsoft.Json.Linq.JObject" /> from a string that contains JSON.
        /// </summary>
        /// <param name="json">A <see cref="T:System.String" /> that contains JSON.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.Linq.JsonLoadSettings" /> used to load the JSON.
        /// If this is <c>null</c>, default load settings will be used.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JObject" /> populated from the string that contains JSON.</returns>
        /// <exception cref="T:Newtonsoft.Json.JsonReaderException">
        ///     <paramref name="json" /> is not valid JSON.
        /// </exception>
        /// <example>
        ///   <code lang="cs" source="..\Src\Newtonsoft.Json.Tests\Documentation\LinqToJsonTests.cs" region="LinqToJsonCreateParse" title="Parsing a JSON Object from Text" />
        /// </example>
        public new static JObject Parse(string json, JsonLoadSettings settings)
        {
            JObject result;
            using (JsonReader jsonReader = new JsonTextReader(new StringReader(json)))
            {
                JObject jobject = JObject.Load(jsonReader, settings);
                if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
                {
                    throw JsonReaderException.Create(jsonReader, "Additional text found in JSON string after parsing content.");
                }
                result = jobject;
            }
            return result;
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JObject" /> from an object.
        /// </summary>
        /// <param name="o">The object that will be used to create <see cref="T:Newtonsoft.Json.Linq.JObject" />.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JObject" /> with the values of the specified object.</returns>
        public new static JObject FromObject(object o)
        {
            return JObject.FromObject(o, JsonSerializer.CreateDefault());
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JObject" /> from an object.
        /// </summary>
        /// <param name="o">The object that will be used to create <see cref="T:Newtonsoft.Json.Linq.JObject" />.</param>
        /// <param name="jsonSerializer">The <see cref="T:Newtonsoft.Json.JsonSerializer" /> that will be used to read the object.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JObject" /> with the values of the specified object.</returns>
        public new static JObject FromObject(object o, JsonSerializer jsonSerializer)
        {
            JToken jtoken = JToken.FromObjectInternal(o, jsonSerializer);
            if (jtoken != null && jtoken.Type != JTokenType.Object)
            {
                throw new ArgumentException("Object serialized to {0}. JObject instance expected.".FormatWith(CultureInfo.InvariantCulture, jtoken.Type));
            }
            return (JObject)jtoken;
        }

        /// <summary>
        /// Writes this token to a <see cref="T:Newtonsoft.Json.JsonWriter" />.
        /// </summary>
        /// <param name="writer">A <see cref="T:Newtonsoft.Json.JsonWriter" /> into which this method will write.</param>
        /// <param name="converters">A collection of <see cref="T:Newtonsoft.Json.JsonConverter" /> which will be used when writing the token.</param>
        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WriteStartObject();
            for (int i = 0; i < _properties.Count; i++)
            {
                _properties[i].WriteTo(writer, converters);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Gets the <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified property name.</returns>
        public JToken GetValue(string propertyName)
        {
            return GetValue(propertyName, StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified property name.
        /// The exact property name will be searched for first and if no matching property is found then
        /// the <see cref="T:System.StringComparison" /> will be used to match a property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="comparison">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <returns>The <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified property name.</returns>
        public JToken GetValue(string propertyName, StringComparison comparison)
        {
            if (propertyName == null)
            {
                return null;
            }
            JProperty jproperty = Property(propertyName);
            if (jproperty != null)
            {
                return jproperty.Value;
            }
            if (comparison != StringComparison.Ordinal)
            {
                foreach (JToken jtoken in _properties)
                {
                    JProperty jproperty2 = (JProperty)jtoken;
                    if (string.Equals(jproperty2.Name, propertyName, comparison))
                    {
                        return jproperty2.Value;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Tries to get the <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified property name.
        /// The exact property name will be searched for first and if no matching property is found then
        /// the <see cref="T:System.StringComparison" /> will be used to match a property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparison">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <returns><c>true</c> if a value was successfully retrieved; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(string propertyName, StringComparison comparison, out JToken value)
        {
            value = GetValue(propertyName, comparison);
            return value != null;
        }

        /// <summary>
        /// Adds the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void Add(string propertyName, JToken value)
        {
            Add(new JProperty(propertyName, value));
        }

        bool IDictionary<string, JToken>.ContainsKey(string key)
        {
            return _properties.Contains(key);
        }

        ICollection<string> IDictionary<string, JToken>.Keys
        {
            get
            {
                return _properties.Keys;
            }
        }

        /// <summary>
        /// Removes the property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if item was successfully removed; otherwise, <c>false</c>.</returns>
        public bool Remove(string propertyName)
        {
            JProperty jproperty = Property(propertyName);
            if (jproperty == null)
            {
                return false;
            }
            jproperty.Remove();
            return true;
        }

        /// <summary>
        /// Tries to get the <see cref="T:Newtonsoft.Json.Linq.JToken" /> with the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if a value was successfully retrieved; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(string propertyName, out JToken value)
        {
            JProperty jproperty = Property(propertyName);
            if (jproperty == null)
            {
                value = null;
                return false;
            }
            value = jproperty.Value;
            return true;
        }

        ICollection<JToken> IDictionary<string, JToken>.Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        void ICollection<KeyValuePair<string, JToken>>.Add(KeyValuePair<string, JToken> item)
        {
            Add(new JProperty(item.Key, item.Value));
        }

        void ICollection<KeyValuePair<string, JToken>>.Clear()
        {
            base.RemoveAll();
        }

        bool ICollection<KeyValuePair<string, JToken>>.Contains(KeyValuePair<string, JToken> item)
        {
            JProperty jproperty = Property(item.Key);
            return jproperty != null && jproperty.Value == item.Value;
        }

        void ICollection<KeyValuePair<string, JToken>>.CopyTo(KeyValuePair<string, JToken>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
            }
            if (arrayIndex >= array.Length && arrayIndex != 0)
            {
                throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
            }
            if (base.Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
            }
            int num = 0;
            foreach (JToken jtoken in _properties)
            {
                JProperty jproperty = (JProperty)jtoken;
                array[arrayIndex + num] = new KeyValuePair<string, JToken>(jproperty.Name, jproperty.Value);
                num++;
            }
        }

        bool ICollection<KeyValuePair<string, JToken>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection<KeyValuePair<string, JToken>>.Remove(KeyValuePair<string, JToken> item)
        {
            if (!((ICollection<KeyValuePair<string, JToken>>)this).Contains(item))
            {
                return false;
            }
            ((IDictionary<string, JToken>)this).Remove(item.Key);
            return true;
        }

        internal override int GetDeepHashCode()
        {
            return base.ContentsHashCode();
        }

        /// <summary>
        /// Returns an enumerator that can be used to iterate through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, JToken>> GetEnumerator()
        {
            foreach (JToken jtoken in _properties)
            {
                JProperty jproperty = (JProperty)jtoken;
                yield return new KeyValuePair<string, JToken>(jproperty.Name, jproperty.Value);
            }
            yield break;
        }

        /// <summary>
        /// Raises the <see cref="E:Newtonsoft.Json.Linq.JObject.PropertyChanged" /> event with the provided arguments.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private readonly JPropertyKeyedCollection _properties = new JPropertyKeyedCollection();
    }
}
