using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Represents a JSON constructor.
    /// </summary>
    public class JConstructor : JContainer
    {
        /// <summary>
        /// Gets the container's children tokens.
        /// </summary>
        /// <value>The container's children tokens.</value>
        protected override IList<JToken> ChildrenTokens
        {
            get
            {
                return _values;
            }
        }

        internal override int IndexOfItem(JToken item)
        {
            return _values.IndexOfReference(item);
        }

        internal override void MergeItem(object content, JsonMergeSettings settings)
        {
            JConstructor jconstructor = content as JConstructor;
            if (jconstructor == null)
            {
                return;
            }
            if (jconstructor.Name != null)
            {
                Name = jconstructor.Name;
            }
            JContainer.MergeEnumerableContent(this, jconstructor, settings);
        }

        /// <summary>
        /// Gets or sets the name of this constructor.
        /// </summary>
        /// <value>The constructor name.</value>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets the node type for this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <value>The type.</value>
        public override JTokenType Type
        {
            get
            {
                return JTokenType.Constructor;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JConstructor" /> class.
        /// </summary>
        public JConstructor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JConstructor" /> class from another <see cref="T:Newtonsoft.Json.Linq.JConstructor" /> object.
        /// </summary>
        /// <param name="other">A <see cref="T:Newtonsoft.Json.Linq.JConstructor" /> object to copy from.</param>
        public JConstructor(JConstructor other) : base(other)
        {
            _name = other.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JConstructor" /> class with the specified name and content.
        /// </summary>
        /// <param name="name">The constructor name.</param>
        /// <param name="content">The contents of the constructor.</param>
        public JConstructor(string name, params object[] content) : this(name, content[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JConstructor" /> class with the specified name and content.
        /// </summary>
        /// <param name="name">The constructor name.</param>
        /// <param name="content">The contents of the constructor.</param>
        public JConstructor(string name, object content) : this(name)
        {
            Add(content);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JConstructor" /> class with the specified name.
        /// </summary>
        /// <param name="name">The constructor name.</param>
        public JConstructor(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("Constructor name cannot be empty.", "name");
            }
            _name = name;
        }

        internal override bool DeepEquals(JToken node)
        {
            JConstructor jconstructor = node as JConstructor;
            return jconstructor != null && _name == jconstructor.Name && base.ContentsEqual(jconstructor);
        }

        internal override JToken CloneToken()
        {
            return new JConstructor(this);
        }

        /// <summary>
        /// Writes this token to a <see cref="T:Newtonsoft.Json.JsonWriter" />.
        /// </summary>
        /// <param name="writer">A <see cref="T:Newtonsoft.Json.JsonWriter" /> into which this method will write.</param>
        /// <param name="converters">A collection of <see cref="T:Newtonsoft.Json.JsonConverter" /> which will be used when writing the token.</param>
        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WriteStartConstructor(_name);
            foreach (JToken jtoken in Children())
            {
                jtoken.WriteTo(writer, converters);
            }
            writer.WriteEndConstructor();
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
                if (!(key is int))
                {
                    throw new ArgumentException("Accessed JConstructor values with invalid key value: {0}. Argument position index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }
                return GetItem((int)key);
            }
            set
            {
                ValidationUtils.ArgumentNotNull(key, "key");
                if (!(key is int))
                {
                    throw new ArgumentException("Set JConstructor values with invalid key value: {0}. Argument position index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }
                SetItem((int)key, value);
            }
        }

        internal override int GetDeepHashCode()
        {
            return _name.GetHashCode() ^ base.ContentsHashCode();
        }

        /// <summary>
        /// Loads a <see cref="T:Newtonsoft.Json.Linq.JConstructor" /> from a <see cref="T:Newtonsoft.Json.JsonReader" />.
        /// </summary>
        /// <param name="reader">A <see cref="T:Newtonsoft.Json.JsonReader" /> that will be read for the content of the <see cref="T:Newtonsoft.Json.Linq.JConstructor" />.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JConstructor" /> that contains the JSON that was read from the specified <see cref="T:Newtonsoft.Json.JsonReader" />.</returns>
        public new static JConstructor Load(JsonReader reader)
        {
            return JConstructor.Load(reader, null);
        }

        /// <summary>
        /// Loads a <see cref="T:Newtonsoft.Json.Linq.JConstructor" /> from a <see cref="T:Newtonsoft.Json.JsonReader" />.
        /// </summary>
        /// <param name="reader">A <see cref="T:Newtonsoft.Json.JsonReader" /> that will be read for the content of the <see cref="T:Newtonsoft.Json.Linq.JConstructor" />.</param>
        /// <param name="settings">The <see cref="T:Newtonsoft.Json.Linq.JsonLoadSettings" /> used to load the JSON.
        /// If this is <c>null</c>, default load settings will be used.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JConstructor" /> that contains the JSON that was read from the specified <see cref="T:Newtonsoft.Json.JsonReader" />.</returns>
        public new static JConstructor Load(JsonReader reader, JsonLoadSettings settings)
        {
            if (reader.TokenType == JsonToken.None && !reader.Read())
            {
                throw JsonReaderException.Create(reader, "Error reading JConstructor from JsonReader.");
            }
            reader.MoveToContent();
            if (reader.TokenType != JsonToken.StartConstructor)
            {
                throw JsonReaderException.Create(reader, "Error reading JConstructor from JsonReader. Current JsonReader item is not a constructor: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
            JConstructor jconstructor = new JConstructor((string)reader.Value);
            jconstructor.SetLineInfo(reader as IJsonLineInfo, settings);
            jconstructor.ReadTokenFrom(reader, settings);
            return jconstructor;
        }

        private string _name;

        private readonly List<JToken> _values = new List<JToken>();
    }
}
