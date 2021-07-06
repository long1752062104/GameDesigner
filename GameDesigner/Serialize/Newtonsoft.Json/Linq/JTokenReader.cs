using Newtonsoft_X.Json.Utilities;
using System;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to serialized JSON data.
    /// </summary>
    public class JTokenReader : JsonReader, IJsonLineInfo
    {
        /// <summary>
        /// Gets the <see cref="T:Newtonsoft.Json.Linq.JToken" /> at the reader's current position.
        /// </summary>
        public JToken CurrentToken
        {
            get
            {
                return _current;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JTokenReader" /> class.
        /// </summary>
        /// <param name="token">The token to read from.</param>
        public JTokenReader(JToken token)
        {
            ValidationUtils.ArgumentNotNull(token, "token");
            _root = token;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JTokenReader" /> class.
        /// </summary>
        /// <param name="token">The token to read from.</param>
        /// <param name="initialPath">The initial path of the token. It is prepended to the returned <see cref="P:Newtonsoft.Json.Linq.JTokenReader.Path" />.</param>
        internal JTokenReader(JToken token, string initialPath) : this(token)
        {
            _initialPath = initialPath;
        }

        /// <summary>
        /// Reads the next JSON token from the underlying <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the next token was read successfully; <c>false</c> if there are no more tokens to read.
        /// </returns>
        public override bool Read()
        {
            if (base.CurrentState == JsonReader.State.Start)
            {
                _current = _root;
                SetToken(_current);
                return true;
            }
            if (_current == null)
            {
                return false;
            }
            JContainer jcontainer = _current as JContainer;
            if (jcontainer != null && _parent != jcontainer)
            {
                return ReadInto(jcontainer);
            }
            return ReadOver(_current);
        }

        private bool ReadOver(JToken t)
        {
            if (t == _root)
            {
                return ReadToEnd();
            }
            JToken next = t.Next;
            if (next != null && next != t && t != t.Parent.Last)
            {
                _current = next;
                SetToken(_current);
                return true;
            }
            if (t.Parent == null)
            {
                return ReadToEnd();
            }
            return SetEnd(t.Parent);
        }

        private bool ReadToEnd()
        {
            _current = null;
            base.SetToken(JsonToken.None);
            return false;
        }

        private JsonToken? GetEndToken(JContainer c)
        {
            switch (c.Type)
            {
                case JTokenType.Object:
                    return new JsonToken?(JsonToken.EndObject);
                case JTokenType.Array:
                    return new JsonToken?(JsonToken.EndArray);
                case JTokenType.Constructor:
                    return new JsonToken?(JsonToken.EndConstructor);
                case JTokenType.Property:
                    return null;
                default:
                    throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", c.Type, "Unexpected JContainer type.");
            }
        }

        private bool ReadInto(JContainer c)
        {
            JToken first = c.First;
            if (first == null)
            {
                return SetEnd(c);
            }
            SetToken(first);
            _current = first;
            _parent = c;
            return true;
        }

        private bool SetEnd(JContainer c)
        {
            JsonToken? endToken = GetEndToken(c);
            if (endToken != null)
            {
                base.SetToken(endToken.GetValueOrDefault());
                _current = c;
                _parent = c;
                return true;
            }
            return ReadOver(c);
        }

        private void SetToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    base.SetToken(JsonToken.StartObject);
                    return;
                case JTokenType.Array:
                    base.SetToken(JsonToken.StartArray);
                    return;
                case JTokenType.Constructor:
                    base.SetToken(JsonToken.StartConstructor, ((JConstructor)token).Name);
                    return;
                case JTokenType.Property:
                    base.SetToken(JsonToken.PropertyName, ((JProperty)token).Name);
                    return;
                case JTokenType.Comment:
                    base.SetToken(JsonToken.Comment, ((JValue)token).Value);
                    return;
                case JTokenType.Integer:
                    base.SetToken(JsonToken.Integer, ((JValue)token).Value);
                    return;
                case JTokenType.Float:
                    base.SetToken(JsonToken.Float, ((JValue)token).Value);
                    return;
                case JTokenType.String:
                    base.SetToken(JsonToken.String, ((JValue)token).Value);
                    return;
                case JTokenType.Boolean:
                    base.SetToken(JsonToken.Boolean, ((JValue)token).Value);
                    return;
                case JTokenType.Null:
                    base.SetToken(JsonToken.Null, ((JValue)token).Value);
                    return;
                case JTokenType.Undefined:
                    base.SetToken(JsonToken.Undefined, ((JValue)token).Value);
                    return;
                case JTokenType.Date:
                    base.SetToken(JsonToken.Date, ((JValue)token).Value);
                    return;
                case JTokenType.Raw:
                    base.SetToken(JsonToken.Raw, ((JValue)token).Value);
                    return;
                case JTokenType.Bytes:
                    base.SetToken(JsonToken.Bytes, ((JValue)token).Value);
                    return;
                case JTokenType.Guid:
                    base.SetToken(JsonToken.String, SafeToString(((JValue)token).Value));
                    return;
                case JTokenType.Uri:
                    {
                        object value = ((JValue)token).Value;
                        if (value is Uri)
                        {
                            base.SetToken(JsonToken.String, ((Uri)value).OriginalString);
                            return;
                        }
                        base.SetToken(JsonToken.String, SafeToString(value));
                        return;
                    }
                case JTokenType.TimeSpan:
                    base.SetToken(JsonToken.String, SafeToString(((JValue)token).Value));
                    return;
                default:
                    throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", token.Type, "Unexpected JTokenType.");
            }
        }

        private string SafeToString(object value)
        {
            if (value == null)
            {
                return null;
            }
            return value.ToString();
        }

        bool IJsonLineInfo.HasLineInfo()
        {
            if (base.CurrentState == JsonReader.State.Start)
            {
                return false;
            }
            IJsonLineInfo current = _current;
            return current != null && current.HasLineInfo();
        }

        int IJsonLineInfo.LineNumber
        {
            get
            {
                if (base.CurrentState == JsonReader.State.Start)
                {
                    return 0;
                }
                IJsonLineInfo current = _current;
                if (current != null)
                {
                    return current.LineNumber;
                }
                return 0;
            }
        }

        int IJsonLineInfo.LinePosition
        {
            get
            {
                if (base.CurrentState == JsonReader.State.Start)
                {
                    return 0;
                }
                IJsonLineInfo current = _current;
                if (current != null)
                {
                    return current.LinePosition;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the path of the current JSON token. 
        /// </summary>
        public override string Path
        {
            get
            {
                string text = base.Path;
                if (_initialPath == null)
                {
                    _initialPath = _root.Path;
                }
                if (!string.IsNullOrEmpty(_initialPath))
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        return _initialPath;
                    }
                    if (text.StartsWith('['))
                    {
                        text = _initialPath + text;
                    }
                    else
                    {
                        text = _initialPath + "." + text;
                    }
                }
                return text;
            }
        }

        private readonly JToken _root;

        private string _initialPath;

        private JToken _parent;

        private JToken _current;
    }
}
