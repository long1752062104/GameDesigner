using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Represents a writer that provides a fast, non-cached, forward-only way of generating JSON data.
    /// </summary>
    public abstract class JsonWriter : IDisposable
    {
        internal static JsonWriter.State[][] BuildStateArray()
        {
            List<JsonWriter.State[]> list = JsonWriter.StateArrayTempate.ToList<JsonWriter.State[]>();
            JsonWriter.State[] item = JsonWriter.StateArrayTempate[0];
            JsonWriter.State[] item2 = JsonWriter.StateArrayTempate[7];
            foreach (object obj in EnumUtils.GetValues(typeof(JsonToken)))
            {
                JsonToken jsonToken = (JsonToken)obj;
                if (list.Count <= (int)jsonToken)
                {
                    switch (jsonToken)
                    {
                        case JsonToken.Integer:
                        case JsonToken.Float:
                        case JsonToken.String:
                        case JsonToken.Boolean:
                        case JsonToken.Null:
                        case JsonToken.Undefined:
                        case JsonToken.Date:
                        case JsonToken.Bytes:
                            list.Add(item2);
                            continue;
                    }
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        static JsonWriter()
        {
            JsonWriter.StateArray = JsonWriter.BuildStateArray();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the destination should be closed when this writer is closed.
        /// </summary>
        /// <value>
        /// <c>true</c> to close the destination when this writer is closed; otherwise <c>false</c>. The default is <c>true</c>.
        /// </value>
        public bool CloseOutput { get; set; }

        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <value>The top.</value>
        protected internal int Top
        {
            get
            {
                int num = (_stack != null) ? _stack.Count : 0;
                if (Peek() != JsonContainerType.None)
                {
                    num++;
                }
                return num;
            }
        }

        /// <summary>
        /// Gets the state of the writer.
        /// </summary>
        public WriteState WriteState
        {
            get
            {
                switch (_currentState)
                {
                    case JsonWriter.State.Start:
                        return WriteState.Start;
                    case JsonWriter.State.Property:
                        return WriteState.Property;
                    case JsonWriter.State.ObjectStart:
                    case JsonWriter.State.Object:
                        return WriteState.Object;
                    case JsonWriter.State.ArrayStart:
                    case JsonWriter.State.Array:
                        return WriteState.Array;
                    case JsonWriter.State.ConstructorStart:
                    case JsonWriter.State.Constructor:
                        return WriteState.Constructor;
                    case JsonWriter.State.Closed:
                        return WriteState.Closed;
                    case JsonWriter.State.Error:
                        return WriteState.Error;
                    default:
                        throw JsonWriterException.Create(this, "Invalid state: " + _currentState, null);
                }
            }
        }

        internal string ContainerPath
        {
            get
            {
                if (_currentPosition.Type == JsonContainerType.None || _stack == null)
                {
                    return string.Empty;
                }
                return JsonPosition.BuildPath(_stack, null);
            }
        }

        /// <summary>
        /// Gets the path of the writer. 
        /// </summary>
        public string Path
        {
            get
            {
                if (_currentPosition.Type == JsonContainerType.None)
                {
                    return string.Empty;
                }
                JsonPosition? currentPosition = (_currentState != JsonWriter.State.ArrayStart && _currentState != JsonWriter.State.ConstructorStart && _currentState != JsonWriter.State.ObjectStart) ? new JsonPosition?(_currentPosition) : null;
                return JsonPosition.BuildPath(_stack, currentPosition);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how JSON text output should be formatted.
        /// </summary>
        public Formatting Formatting
        {
            get
            {
                return _formatting;
            }
            set
            {
                if (value < Formatting.None || value > Formatting.Indented)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _formatting = value;
            }
        }

        /// <summary>
        /// Gets or sets how dates are written to JSON text.
        /// </summary>
        public DateFormatHandling DateFormatHandling
        {
            get
            {
                return _dateFormatHandling;
            }
            set
            {
                if (value < DateFormatHandling.IsoDateFormat || value > DateFormatHandling.MicrosoftDateFormat)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _dateFormatHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how <see cref="T:System.DateTime" /> time zones are handled when writing JSON text.
        /// </summary>
        public DateTimeZoneHandling DateTimeZoneHandling
        {
            get
            {
                return _dateTimeZoneHandling;
            }
            set
            {
                if (value < DateTimeZoneHandling.Local || value > DateTimeZoneHandling.RoundtripKind)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _dateTimeZoneHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how strings are escaped when writing JSON text.
        /// </summary>
        public StringEscapeHandling StringEscapeHandling
        {
            get
            {
                return _stringEscapeHandling;
            }
            set
            {
                if (value < StringEscapeHandling.Default || value > StringEscapeHandling.EscapeHtml)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _stringEscapeHandling = value;
                OnStringEscapeHandlingChanged();
            }
        }

        internal virtual void OnStringEscapeHandlingChanged()
        {
        }

        /// <summary>
        /// Gets or sets how special floating point numbers, e.g. <see cref="F:System.Double.NaN" />,
        /// <see cref="F:System.Double.PositiveInfinity" /> and <see cref="F:System.Double.NegativeInfinity" />,
        /// are written to JSON text.
        /// </summary>
        public FloatFormatHandling FloatFormatHandling
        {
            get
            {
                return _floatFormatHandling;
            }
            set
            {
                if (value < FloatFormatHandling.String || value > FloatFormatHandling.DefaultValue)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _floatFormatHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how <see cref="T:System.DateTime" /> and <see cref="T:System.DateTimeOffset" /> values are formatted when writing JSON text.
        /// </summary>
        public string DateFormatString
        {
            get
            {
                return _dateFormatString;
            }
            set
            {
                _dateFormatString = value;
            }
        }

        /// <summary>
        /// Gets or sets the culture used when writing JSON. Defaults to <see cref="P:System.Globalization.CultureInfo.InvariantCulture" />.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return _culture ?? CultureInfo.InvariantCulture;
            }
            set
            {
                _culture = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonWriter" /> class.
        /// </summary>
        protected JsonWriter()
        {
            _currentState = JsonWriter.State.Start;
            _formatting = Formatting.None;
            _dateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
            CloseOutput = true;
        }

        internal void UpdateScopeWithFinishedValue()
        {
            if (_currentPosition.HasIndex)
            {
                _currentPosition.Position = _currentPosition.Position + 1;
            }
        }

        private void Push(JsonContainerType value)
        {
            if (_currentPosition.Type != JsonContainerType.None)
            {
                if (_stack == null)
                {
                    _stack = new List<JsonPosition>();
                }
                _stack.Add(_currentPosition);
            }
            _currentPosition = new JsonPosition(value);
        }

        private JsonContainerType Pop()
        {
            JsonPosition currentPosition = _currentPosition;
            if (_stack != null && _stack.Count > 0)
            {
                _currentPosition = _stack[_stack.Count - 1];
                _stack.RemoveAt(_stack.Count - 1);
            }
            else
            {
                _currentPosition = default(JsonPosition);
            }
            return currentPosition.Type;
        }

        private JsonContainerType Peek()
        {
            return _currentPosition.Type;
        }

        /// <summary>
        /// Flushes whatever is in the buffer to the destination and also flushes the destination.
        /// </summary>
        public abstract void Flush();

        /// <summary>
        /// Closes this writer.
        /// If <see cref="P:Newtonsoft.Json.JsonWriter.CloseOutput" /> is set to <c>true</c>, the destination is also closed.
        /// If <see cref="P:Newtonsoft.Json.JsonWriter.AutoCompleteOnClose" /> is set to <c>true</c>, the JSON is auto-completed.
        /// </summary>
        public virtual void Close()
        {
            AutoCompleteAll();
        }

        /// <summary>
        /// Writes the beginning of a JSON object.
        /// </summary>
        public virtual void WriteStartObject()
        {
            InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);
        }

        /// <summary>
        /// Writes the end of a JSON object.
        /// </summary>
        public virtual void WriteEndObject()
        {
            InternalWriteEnd(JsonContainerType.Object);
        }

        /// <summary>
        /// Writes the beginning of a JSON array.
        /// </summary>
        public virtual void WriteStartArray()
        {
            InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);
        }

        /// <summary>
        /// Writes the end of an array.
        /// </summary>
        public virtual void WriteEndArray()
        {
            InternalWriteEnd(JsonContainerType.Array);
        }

        /// <summary>
        /// Writes the start of a constructor with the given name.
        /// </summary>
        /// <param name="name">The name of the constructor.</param>
        public virtual void WriteStartConstructor(string name)
        {
            InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);
        }

        /// <summary>
        /// Writes the end constructor.
        /// </summary>
        public virtual void WriteEndConstructor()
        {
            InternalWriteEnd(JsonContainerType.Constructor);
        }

        /// <summary>
        /// Writes the property name of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        public virtual void WritePropertyName(string name)
        {
            InternalWritePropertyName(name);
        }

        /// <summary>
        /// Writes the property name of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="escape">A flag to indicate whether the text should be escaped when it is written as a JSON property name.</param>
        public virtual void WritePropertyName(string name, bool escape)
        {
            WritePropertyName(name);
        }

        /// <summary>
        /// Writes the end of the current JSON object or array.
        /// </summary>
        public virtual void WriteEnd()
        {
            WriteEnd(Peek());
        }

        /// <summary>
        /// Writes the current <see cref="T:Newtonsoft.Json.JsonReader" /> token and its children.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read the token from.</param>
        public void WriteToken(JsonReader reader)
        {
            WriteToken(reader, true);
        }

        /// <summary>
        /// Writes the current <see cref="T:Newtonsoft.Json.JsonReader" /> token.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read the token from.</param>
        /// <param name="writeChildren">A flag indicating whether the current token's children should be written.</param>
        public void WriteToken(JsonReader reader, bool writeChildren)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            WriteToken(reader, writeChildren, true, true);
        }

        /// <summary>
        /// Writes the <see cref="T:Newtonsoft.Json.JsonToken" /> token and its value.
        /// </summary>
        /// <param name="token">The <see cref="T:Newtonsoft.Json.JsonToken" /> to write.</param>
        /// <param name="value">
        /// The value to write.
        /// A value is only required for tokens that have an associated value, e.g. the <see cref="T:System.String" /> property name for <see cref="F:Newtonsoft.Json.JsonToken.PropertyName" />.
        /// <c>null</c> can be passed to the method for tokens that don't have a value, e.g. <see cref="F:Newtonsoft.Json.JsonToken.StartObject" />.
        /// </param>
        public void WriteToken(JsonToken token, object value)
        {
            switch (token)
            {
                case JsonToken.None:
                    return;
                case JsonToken.StartObject:
                    WriteStartObject();
                    return;
                case JsonToken.StartArray:
                    WriteStartArray();
                    return;
                case JsonToken.StartConstructor:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    WriteStartConstructor(value.ToString());
                    return;
                case JsonToken.PropertyName:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    WritePropertyName(value.ToString());
                    return;
                case JsonToken.Comment:
                    WriteComment((value != null) ? value.ToString() : null);
                    return;
                case JsonToken.Raw:
                    WriteRawValue((value != null) ? value.ToString() : null);
                    return;
                case JsonToken.Integer:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    WriteValue(Convert.ToInt64(value, CultureInfo.InvariantCulture));
                    return;
                case JsonToken.Float:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    if (value is decimal)
                    {
                        WriteValue((decimal)value);
                        return;
                    }
                    if (value is double)
                    {
                        WriteValue((double)value);
                        return;
                    }
                    if (value is float)
                    {
                        WriteValue((float)value);
                        return;
                    }
                    WriteValue(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                    return;
                case JsonToken.String:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    WriteValue(value.ToString());
                    return;
                case JsonToken.Boolean:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    WriteValue(Convert.ToBoolean(value, CultureInfo.InvariantCulture));
                    return;
                case JsonToken.Null:
                    WriteNull();
                    return;
                case JsonToken.Undefined:
                    WriteUndefined();
                    return;
                case JsonToken.EndObject:
                    WriteEndObject();
                    return;
                case JsonToken.EndArray:
                    WriteEndArray();
                    return;
                case JsonToken.EndConstructor:
                    WriteEndConstructor();
                    return;
                case JsonToken.Date:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    if (value is DateTimeOffset)
                    {
                        WriteValue((DateTimeOffset)value);
                        return;
                    }
                    WriteValue(Convert.ToDateTime(value, CultureInfo.InvariantCulture));
                    return;
                case JsonToken.Bytes:
                    ValidationUtils.ArgumentNotNull(value, "value");
                    if (value is Guid)
                    {
                        WriteValue((Guid)value);
                        return;
                    }
                    WriteValue((byte[])value);
                    return;
                default:
                    throw MiscellaneousUtils.CreateArgumentOutOfRangeException("token", token, "Unexpected token type.");
            }
        }

        /// <summary>
        /// Writes the <see cref="T:Newtonsoft.Json.JsonToken" /> token.
        /// </summary>
        /// <param name="token">The <see cref="T:Newtonsoft.Json.JsonToken" /> to write.</param>
        public void WriteToken(JsonToken token)
        {
            WriteToken(token, null);
        }

        internal virtual void WriteToken(JsonReader reader, bool writeChildren, bool writeDateConstructorAsDate, bool writeComments)
        {
            int num;
            if (reader.TokenType == JsonToken.None)
            {
                num = -1;
            }
            else if (!JsonTokenUtils.IsStartToken(reader.TokenType))
            {
                num = reader.Depth + 1;
            }
            else
            {
                num = reader.Depth;
            }
            do
            {
                if (writeDateConstructorAsDate && reader.TokenType == JsonToken.StartConstructor && string.Equals(reader.Value.ToString(), "Date", StringComparison.Ordinal))
                {
                    WriteConstructorDate(reader);
                }
                else if (writeComments || reader.TokenType != JsonToken.Comment)
                {
                    WriteToken(reader.TokenType, reader.Value);
                }
            }
            while (num - 1 < reader.Depth - (JsonTokenUtils.IsEndToken(reader.TokenType) ? 1 : 0) && writeChildren && reader.Read());
        }

        private void WriteConstructorDate(JsonReader reader)
        {
            if (!reader.Read())
            {
                throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
            }
            if (reader.TokenType != JsonToken.Integer)
            {
                throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected Integer, got " + reader.TokenType, null);
            }
            DateTime value = DateTimeUtils.ConvertJavaScriptTicksToDateTime((long)reader.Value);
            if (!reader.Read())
            {
                throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
            }
            if (reader.TokenType != JsonToken.EndConstructor)
            {
                throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected EndConstructor, got " + reader.TokenType, null);
            }
            WriteValue(value);
        }

        private void WriteEnd(JsonContainerType type)
        {
            switch (type)
            {
                case JsonContainerType.Object:
                    WriteEndObject();
                    return;
                case JsonContainerType.Array:
                    WriteEndArray();
                    return;
                case JsonContainerType.Constructor:
                    WriteEndConstructor();
                    return;
                default:
                    throw JsonWriterException.Create(this, "Unexpected type when writing end: " + type, null);
            }
        }

        private void AutoCompleteAll()
        {
            while (Top > 0)
            {
                WriteEnd();
            }
        }

        private JsonToken GetCloseTokenForType(JsonContainerType type)
        {
            switch (type)
            {
                case JsonContainerType.Object:
                    return JsonToken.EndObject;
                case JsonContainerType.Array:
                    return JsonToken.EndArray;
                case JsonContainerType.Constructor:
                    return JsonToken.EndConstructor;
                default:
                    throw JsonWriterException.Create(this, "No close token for type: " + type, null);
            }
        }

        private void AutoCompleteClose(JsonContainerType type)
        {
            int num = 0;
            if (_currentPosition.Type == type)
            {
                num = 1;
            }
            else
            {
                int num2 = Top - 2;
                for (int i = num2; i >= 0; i--)
                {
                    int index = num2 - i;
                    if (_stack[index].Type == type)
                    {
                        num = i + 2;
                        break;
                    }
                }
            }
            if (num == 0)
            {
                throw JsonWriterException.Create(this, "No token to close.", null);
            }
            for (int j = 0; j < num; j++)
            {
                JsonToken closeTokenForType = GetCloseTokenForType(Pop());
                if (_currentState == JsonWriter.State.Property)
                {
                    WriteNull();
                }
                if (_formatting == Formatting.Indented && _currentState != JsonWriter.State.ObjectStart && _currentState != JsonWriter.State.ArrayStart)
                {
                    WriteIndent();
                }
                WriteEnd(closeTokenForType);
                JsonContainerType jsonContainerType = Peek();
                switch (jsonContainerType)
                {
                    case JsonContainerType.None:
                        _currentState = JsonWriter.State.Start;
                        break;
                    case JsonContainerType.Object:
                        _currentState = JsonWriter.State.Object;
                        break;
                    case JsonContainerType.Array:
                        _currentState = JsonWriter.State.Array;
                        break;
                    case JsonContainerType.Constructor:
                        _currentState = JsonWriter.State.Array;
                        break;
                    default:
                        throw JsonWriterException.Create(this, "Unknown JsonType: " + jsonContainerType, null);
                }
            }
        }

        /// <summary>
        /// Writes the specified end token.
        /// </summary>
        /// <param name="token">The end token to write.</param>
        protected virtual void WriteEnd(JsonToken token)
        {
        }

        /// <summary>
        /// Writes indent characters.
        /// </summary>
        protected virtual void WriteIndent()
        {
        }

        /// <summary>
        /// Writes the JSON value delimiter.
        /// </summary>
        protected virtual void WriteValueDelimiter()
        {
        }

        /// <summary>
        /// Writes an indent space.
        /// </summary>
        protected virtual void WriteIndentSpace()
        {
        }

        internal void AutoComplete(JsonToken tokenBeingWritten)
        {
            JsonWriter.State state = JsonWriter.StateArray[(int)tokenBeingWritten][(int)_currentState];
            if (state == JsonWriter.State.Error)
            {
                throw JsonWriterException.Create(this, "Token {0} in state {1} would result in an invalid JSON object.".FormatWith(CultureInfo.InvariantCulture, tokenBeingWritten.ToString(), _currentState.ToString()), null);
            }
            if ((_currentState == JsonWriter.State.Object || _currentState == JsonWriter.State.Array || _currentState == JsonWriter.State.Constructor) && tokenBeingWritten != JsonToken.Comment)
            {
                WriteValueDelimiter();
            }
            if (_formatting == Formatting.Indented)
            {
                if (_currentState == JsonWriter.State.Property)
                {
                    WriteIndentSpace();
                }
                if (_currentState == JsonWriter.State.Array || _currentState == JsonWriter.State.ArrayStart || _currentState == JsonWriter.State.Constructor || _currentState == JsonWriter.State.ConstructorStart || (tokenBeingWritten == JsonToken.PropertyName && _currentState != JsonWriter.State.Start))
                {
                    WriteIndent();
                }
            }
            _currentState = state;
        }

        /// <summary>
        /// Writes a null value.
        /// </summary>
        public virtual void WriteNull()
        {
            InternalWriteValue(JsonToken.Null);
        }

        /// <summary>
        /// Writes an undefined value.
        /// </summary>
        public virtual void WriteUndefined()
        {
            InternalWriteValue(JsonToken.Undefined);
        }

        /// <summary>
        /// Writes raw JSON without changing the writer's state.
        /// </summary>
        /// <param name="json">The raw JSON to write.</param>
        public virtual void WriteRaw(string json)
        {
            InternalWriteRaw();
        }

        /// <summary>
        /// Writes raw JSON where a value is expected and updates the writer's state.
        /// </summary>
        /// <param name="json">The raw JSON to write.</param>
        public virtual void WriteRawValue(string json)
        {
            UpdateScopeWithFinishedValue();
            AutoComplete(JsonToken.Undefined);
            WriteRaw(json);
        }

        /// <summary>
        /// Writes a <see cref="T:System.String" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.String" /> value to write.</param>
        public virtual void WriteValue(string value)
        {
            InternalWriteValue(JsonToken.String);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Int32" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Int32" /> value to write.</param>
        public virtual void WriteValue(int value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.UInt32" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.UInt32" /> value to write.</param>
        //[CLSCompliant(false)]
        public virtual void WriteValue(uint value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Int64" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Int64" /> value to write.</param>
        public virtual void WriteValue(long value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.UInt64" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.UInt64" /> value to write.</param>
        //[CLSCompliant(false)]
        public virtual void WriteValue(ulong value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Single" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Single" /> value to write.</param>
        public virtual void WriteValue(float value)
        {
            InternalWriteValue(JsonToken.Float);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Double" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Double" /> value to write.</param>
        public virtual void WriteValue(double value)
        {
            InternalWriteValue(JsonToken.Float);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Boolean" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Boolean" /> value to write.</param>
        public virtual void WriteValue(bool value)
        {
            InternalWriteValue(JsonToken.Boolean);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Int16" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Int16" /> value to write.</param>
        public virtual void WriteValue(short value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.UInt16" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.UInt16" /> value to write.</param>
        //[CLSCompliant(false)]
        public virtual void WriteValue(ushort value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Char" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Char" /> value to write.</param>
        public virtual void WriteValue(char value)
        {
            InternalWriteValue(JsonToken.String);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Byte" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Byte" /> value to write.</param>
        public virtual void WriteValue(byte value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.SByte" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.SByte" /> value to write.</param>
        //[CLSCompliant(false)]
        public virtual void WriteValue(sbyte value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Decimal" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Decimal" /> value to write.</param>
        public virtual void WriteValue(decimal value)
        {
            InternalWriteValue(JsonToken.Float);
        }

        /// <summary>
        /// Writes a <see cref="T:System.DateTime" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.DateTime" /> value to write.</param>
        public virtual void WriteValue(DateTime value)
        {
            InternalWriteValue(JsonToken.Date);
        }

        /// <summary>
        /// Writes a <see cref="T:System.DateTimeOffset" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.DateTimeOffset" /> value to write.</param>
        public virtual void WriteValue(DateTimeOffset value)
        {
            InternalWriteValue(JsonToken.Date);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Guid" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Guid" /> value to write.</param>
        public virtual void WriteValue(Guid value)
        {
            InternalWriteValue(JsonToken.String);
        }

        /// <summary>
        /// Writes a <see cref="T:System.TimeSpan" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.TimeSpan" /> value to write.</param>
        public virtual void WriteValue(TimeSpan value)
        {
            InternalWriteValue(JsonToken.String);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" /> value to write.</param>
        public virtual void WriteValue(int? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt32" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt32" /> value to write.</param>
        //[CLSCompliant(false)]
        public virtual void WriteValue(uint? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" /> value to write.</param>
        public virtual void WriteValue(long? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt64" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt64" /> value to write.</param>
        //[CLSCompliant(false)]
        public virtual void WriteValue(ulong? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" /> value to write.</param>
        public virtual void WriteValue(float? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" /> value to write.</param>
        public virtual void WriteValue(double? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" /> value to write.</param>
        public virtual void WriteValue(bool? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int16" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int16" /> value to write.</param>
        public virtual void WriteValue(short? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt16" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt16" /> value to write.</param>
        //[CLSCompliant(false)]
        public virtual void WriteValue(ushort? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Char" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Char" /> value to write.</param>
        public virtual void WriteValue(char? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Byte" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Byte" /> value to write.</param>
        public virtual void WriteValue(byte? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.SByte" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.SByte" /> value to write.</param>
        //[CLSCompliant(false)]
        public virtual void WriteValue(sbyte? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" /> value to write.</param>
        public virtual void WriteValue(decimal? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" /> value to write.</param>
        public virtual void WriteValue(DateTime? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" /> value to write.</param>
        public virtual void WriteValue(DateTimeOffset? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Guid" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Guid" /> value to write.</param>
        public virtual void WriteValue(Guid? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.TimeSpan" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.TimeSpan" /> value to write.</param>
        public virtual void WriteValue(TimeSpan? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            WriteValue(value.GetValueOrDefault());
        }

        /// <summary>
        /// Writes a <see cref="T:System.Byte" />[] value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Byte" />[] value to write.</param>
        public virtual void WriteValue(byte[] value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            InternalWriteValue(JsonToken.Bytes);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Uri" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Uri" /> value to write.</param>
        public virtual void WriteValue(Uri value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            InternalWriteValue(JsonToken.String);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Object" /> value.
        /// An error will raised if the value cannot be written as a single JSON token.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object" /> value to write.</param>
        public virtual void WriteValue(object value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            JsonWriter.WriteValue(this, ConvertUtils.GetTypeCode(value.GetType()), value);
        }

        /// <summary>
        /// Writes a comment <c>/*...*/</c> containing the specified text.
        /// </summary>
        /// <param name="text">Text to place inside the comment.</param>
        public virtual void WriteComment(string text)
        {
            InternalWriteComment();
        }

        /// <summary>
        /// Writes the given white space.
        /// </summary>
        /// <param name="ws">The string of white space characters.</param>
        public virtual void WriteWhitespace(string ws)
        {
            InternalWriteWhitespace(ws);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_currentState != JsonWriter.State.Closed && disposing)
            {
                Close();
            }
        }

        internal static void WriteValue(JsonWriter writer, PrimitiveTypeCode typeCode, object value)
        {
            switch (typeCode)
            {
                case PrimitiveTypeCode.Char:
                    writer.WriteValue((char)value);
                    return;
                case PrimitiveTypeCode.CharNullable:
                    writer.WriteValue((value == null) ? null : new char?((char)value));
                    return;
                case PrimitiveTypeCode.Boolean:
                    writer.WriteValue((bool)value);
                    return;
                case PrimitiveTypeCode.BooleanNullable:
                    writer.WriteValue((value == null) ? null : new bool?((bool)value));
                    return;
                case PrimitiveTypeCode.SByte:
                    writer.WriteValue((sbyte)value);
                    return;
                case PrimitiveTypeCode.SByteNullable:
                    writer.WriteValue((value == null) ? null : new sbyte?((sbyte)value));
                    return;
                case PrimitiveTypeCode.Int16:
                    writer.WriteValue((short)value);
                    return;
                case PrimitiveTypeCode.Int16Nullable:
                    writer.WriteValue((value == null) ? null : new short?((short)value));
                    return;
                case PrimitiveTypeCode.UInt16:
                    writer.WriteValue((ushort)value);
                    return;
                case PrimitiveTypeCode.UInt16Nullable:
                    writer.WriteValue((value == null) ? null : new ushort?((ushort)value));
                    return;
                case PrimitiveTypeCode.Int32:
                    writer.WriteValue((int)value);
                    return;
                case PrimitiveTypeCode.Int32Nullable:
                    writer.WriteValue((value == null) ? null : new int?((int)value));
                    return;
                case PrimitiveTypeCode.Byte:
                    writer.WriteValue((byte)value);
                    return;
                case PrimitiveTypeCode.ByteNullable:
                    writer.WriteValue((value == null) ? null : new byte?((byte)value));
                    return;
                case PrimitiveTypeCode.UInt32:
                    writer.WriteValue((uint)value);
                    return;
                case PrimitiveTypeCode.UInt32Nullable:
                    writer.WriteValue((value == null) ? null : new uint?((uint)value));
                    return;
                case PrimitiveTypeCode.Int64:
                    writer.WriteValue((long)value);
                    return;
                case PrimitiveTypeCode.Int64Nullable:
                    writer.WriteValue((value == null) ? null : new long?((long)value));
                    return;
                case PrimitiveTypeCode.UInt64:
                    writer.WriteValue((ulong)value);
                    return;
                case PrimitiveTypeCode.UInt64Nullable:
                    writer.WriteValue((value == null) ? null : new ulong?((ulong)value));
                    return;
                case PrimitiveTypeCode.Single:
                    writer.WriteValue((float)value);
                    return;
                case PrimitiveTypeCode.SingleNullable:
                    writer.WriteValue((value == null) ? null : new float?((float)value));
                    return;
                case PrimitiveTypeCode.Double:
                    writer.WriteValue((double)value);
                    return;
                case PrimitiveTypeCode.DoubleNullable:
                    writer.WriteValue((value == null) ? null : new double?((double)value));
                    return;
                case PrimitiveTypeCode.DateTime:
                    writer.WriteValue((DateTime)value);
                    return;
                case PrimitiveTypeCode.DateTimeNullable:
                    writer.WriteValue((value == null) ? null : new DateTime?((DateTime)value));
                    return;
                case PrimitiveTypeCode.DateTimeOffset:
                    writer.WriteValue((DateTimeOffset)value);
                    return;
                case PrimitiveTypeCode.DateTimeOffsetNullable:
                    writer.WriteValue((value == null) ? null : new DateTimeOffset?((DateTimeOffset)value));
                    return;
                case PrimitiveTypeCode.Decimal:
                    writer.WriteValue((decimal)value);
                    return;
                case PrimitiveTypeCode.DecimalNullable:
                    writer.WriteValue((value == null) ? null : new decimal?((decimal)value));
                    return;
                case PrimitiveTypeCode.Guid:
                    writer.WriteValue((Guid)value);
                    return;
                case PrimitiveTypeCode.GuidNullable:
                    writer.WriteValue((value == null) ? null : new Guid?((Guid)value));
                    return;
                case PrimitiveTypeCode.TimeSpan:
                    writer.WriteValue((TimeSpan)value);
                    return;
                case PrimitiveTypeCode.TimeSpanNullable:
                    writer.WriteValue((value == null) ? null : new TimeSpan?((TimeSpan)value));
                    return;
                case PrimitiveTypeCode.Uri:
                    writer.WriteValue((Uri)value);
                    return;
                case PrimitiveTypeCode.String:
                    writer.WriteValue((string)value);
                    return;
                case PrimitiveTypeCode.Bytes:
                    writer.WriteValue((byte[])value);
                    return;
                case PrimitiveTypeCode.DBNull:
                    writer.WriteNull();
                    return;
            }
            if (value is IConvertible)
            {
                IConvertible convertible = (IConvertible)value;
                TypeInformation typeInformation = ConvertUtils.GetTypeInformation(convertible);
                PrimitiveTypeCode typeCode2 = (typeInformation.TypeCode == PrimitiveTypeCode.Object) ? PrimitiveTypeCode.String : typeInformation.TypeCode;
                Type conversionType = (typeInformation.TypeCode == PrimitiveTypeCode.Object) ? typeof(string) : typeInformation.Type;
                object value2 = convertible.ToType(conversionType, CultureInfo.InvariantCulture);
                JsonWriter.WriteValue(writer, typeCode2, value2);
                return;
            }
            throw JsonWriter.CreateUnsupportedTypeException(writer, value);
        }

        private static JsonWriterException CreateUnsupportedTypeException(JsonWriter writer, object value)
        {
            return JsonWriterException.Create(writer, "Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()), null);
        }

        /// <summary>
        /// Sets the state of the <see cref="T:Newtonsoft.Json.JsonWriter" />.
        /// </summary>
        /// <param name="token">The <see cref="T:Newtonsoft.Json.JsonToken" /> being written.</param>
        /// <param name="value">The value being written.</param>
        protected void SetWriteState(JsonToken token, object value)
        {
            switch (token)
            {
                case JsonToken.StartObject:
                    InternalWriteStart(token, JsonContainerType.Object);
                    return;
                case JsonToken.StartArray:
                    InternalWriteStart(token, JsonContainerType.Array);
                    return;
                case JsonToken.StartConstructor:
                    InternalWriteStart(token, JsonContainerType.Constructor);
                    return;
                case JsonToken.PropertyName:
                    if (!(value is string))
                    {
                        throw new ArgumentException("A name is required when setting property name state.", "value");
                    }
                    InternalWritePropertyName((string)value);
                    return;
                case JsonToken.Comment:
                    InternalWriteComment();
                    return;
                case JsonToken.Raw:
                    InternalWriteRaw();
                    return;
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Null:
                case JsonToken.Undefined:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    InternalWriteValue(token);
                    return;
                case JsonToken.EndObject:
                    InternalWriteEnd(JsonContainerType.Object);
                    return;
                case JsonToken.EndArray:
                    InternalWriteEnd(JsonContainerType.Array);
                    return;
                case JsonToken.EndConstructor:
                    InternalWriteEnd(JsonContainerType.Constructor);
                    return;
                default:
                    throw new ArgumentOutOfRangeException("token");
            }
        }

        internal void InternalWriteEnd(JsonContainerType container)
        {
            AutoCompleteClose(container);
        }

        internal void InternalWritePropertyName(string name)
        {
            _currentPosition.PropertyName = name;
            AutoComplete(JsonToken.PropertyName);
        }

        internal void InternalWriteRaw()
        {
        }

        internal void InternalWriteStart(JsonToken token, JsonContainerType container)
        {
            UpdateScopeWithFinishedValue();
            AutoComplete(token);
            Push(container);
        }

        internal void InternalWriteValue(JsonToken token)
        {
            UpdateScopeWithFinishedValue();
            AutoComplete(token);
        }

        internal void InternalWriteWhitespace(string ws)
        {
            if (ws != null && !StringUtils.IsWhiteSpace(ws))
            {
                throw JsonWriterException.Create(this, "Only white space characters should be used.", null);
            }
        }

        internal void InternalWriteComment()
        {
            AutoComplete(JsonToken.Comment);
        }

        private static readonly JsonWriter.State[][] StateArray;

        internal static readonly JsonWriter.State[][] StateArrayTempate = new JsonWriter.State[][]
        {
            new JsonWriter.State[]
            {
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error
            },
            new JsonWriter.State[]
            {
                JsonWriter.State.ObjectStart,
                JsonWriter.State.ObjectStart,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.ObjectStart,
                JsonWriter.State.ObjectStart,
                JsonWriter.State.ObjectStart,
                JsonWriter.State.ObjectStart,
                JsonWriter.State.Error,
                JsonWriter.State.Error
            },
            new JsonWriter.State[]
            {
                JsonWriter.State.ArrayStart,
                JsonWriter.State.ArrayStart,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.ArrayStart,
                JsonWriter.State.ArrayStart,
                JsonWriter.State.ArrayStart,
                JsonWriter.State.ArrayStart,
                JsonWriter.State.Error,
                JsonWriter.State.Error
            },
            new JsonWriter.State[]
            {
                JsonWriter.State.ConstructorStart,
                JsonWriter.State.ConstructorStart,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.ConstructorStart,
                JsonWriter.State.ConstructorStart,
                JsonWriter.State.ConstructorStart,
                JsonWriter.State.ConstructorStart,
                JsonWriter.State.Error,
                JsonWriter.State.Error
            },
            new JsonWriter.State[]
            {
                JsonWriter.State.Property,
                JsonWriter.State.Error,
                JsonWriter.State.Property,
                JsonWriter.State.Property,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Error
            },
            new JsonWriter.State[]
            {
                JsonWriter.State.Start,
                JsonWriter.State.Property,
                JsonWriter.State.ObjectStart,
                JsonWriter.State.Object,
                JsonWriter.State.ArrayStart,
                JsonWriter.State.Array,
                JsonWriter.State.Constructor,
                JsonWriter.State.Constructor,
                JsonWriter.State.Error,
                JsonWriter.State.Error
            },
            new JsonWriter.State[]
            {
                JsonWriter.State.Start,
                JsonWriter.State.Property,
                JsonWriter.State.ObjectStart,
                JsonWriter.State.Object,
                JsonWriter.State.ArrayStart,
                JsonWriter.State.Array,
                JsonWriter.State.Constructor,
                JsonWriter.State.Constructor,
                JsonWriter.State.Error,
                JsonWriter.State.Error
            },
            new JsonWriter.State[]
            {
                JsonWriter.State.Start,
                JsonWriter.State.Object,
                JsonWriter.State.Error,
                JsonWriter.State.Error,
                JsonWriter.State.Array,
                JsonWriter.State.Array,
                JsonWriter.State.Constructor,
                JsonWriter.State.Constructor,
                JsonWriter.State.Error,
                JsonWriter.State.Error
            }
        };

        private List<JsonPosition> _stack;

        private JsonPosition _currentPosition;

        private JsonWriter.State _currentState;

        private Formatting _formatting;

        private DateFormatHandling _dateFormatHandling;

        private DateTimeZoneHandling _dateTimeZoneHandling;

        private StringEscapeHandling _stringEscapeHandling;

        private FloatFormatHandling _floatFormatHandling;

        private string _dateFormatString;

        private CultureInfo _culture;

        internal enum State
        {
            Start,
            Property,
            ObjectStart,
            Object,
            ArrayStart,
            Array,
            ConstructorStart,
            Constructor,
            Closed,
            Error
        }
    }
}
