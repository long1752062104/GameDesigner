using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to serialized JSON data.
    /// </summary>
    public abstract class JsonReader : IDisposable
    {
        /// <summary>
        /// Gets the current reader state.
        /// </summary>
        /// <value>The current reader state.</value>
        protected JsonReader.State CurrentState
        {
            get
            {
                return _currentState;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the source should be closed when this reader is closed.
        /// </summary>
        /// <value>
        /// <c>true</c> to close the source when this reader is closed; otherwise <c>false</c>. The default is <c>true</c>.
        /// </value>
        public bool CloseInput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple pieces of JSON content can
        /// be read from a continuous stream without erroring.
        /// </summary>
        /// <value>
        /// <c>true</c> to support reading multiple pieces of JSON content; otherwise <c>false</c>.
        /// The default is <c>false</c>.
        /// </value>
        public bool SupportMultipleContent { get; set; }

        /// <summary>
        /// Gets the quotation mark character used to enclose the value of a string.
        /// </summary>
        public virtual char QuoteChar
        {
            get
            {
                return _quoteChar;
            }
            protected internal set
            {
                _quoteChar = value;
            }
        }

        /// <summary>
        /// Gets or sets how <see cref="T:System.DateTime" /> time zones are handled when reading JSON.
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
        /// Gets or sets how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed when reading JSON.
        /// </summary>
        public DateParseHandling DateParseHandling
        {
            get
            {
                return _dateParseHandling;
            }
            set
            {
                if (value < DateParseHandling.None || value > DateParseHandling.DateTimeOffset)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _dateParseHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
        /// </summary>
        public FloatParseHandling FloatParseHandling
        {
            get
            {
                return _floatParseHandling;
            }
            set
            {
                if (value < FloatParseHandling.Double || value > FloatParseHandling.Decimal)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _floatParseHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets how custom date formatted strings are parsed when reading JSON.
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
        /// Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a <see cref="T:Newtonsoft.Json.JsonReaderException" />.
        /// </summary>
        public int? MaxDepth
        {
            get
            {
                return _maxDepth;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Value must be positive.", "value");
                }
                _maxDepth = value;
            }
        }

        /// <summary>
        /// Gets the type of the current JSON token. 
        /// </summary>
        public virtual JsonToken TokenType
        {
            get
            {
                return _tokenType;
            }
        }

        /// <summary>
        /// Gets the text value of the current JSON token.
        /// </summary>
        public virtual object Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Gets the .NET type for the current JSON token.
        /// </summary>
        public virtual Type ValueType
        {
            get
            {
                if (_value == null)
                {
                    return null;
                }
                return _value.GetType();
            }
        }

        /// <summary>
        /// Gets the depth of the current token in the JSON document.
        /// </summary>
        /// <value>The depth of the current token in the JSON document.</value>
        public virtual int Depth
        {
            get
            {
                int num = (_stack != null) ? _stack.Count : 0;
                if (JsonTokenUtils.IsStartToken(TokenType) || _currentPosition.Type == JsonContainerType.None)
                {
                    return num;
                }
                return num + 1;
            }
        }

        /// <summary>
        /// Gets the path of the current JSON token. 
        /// </summary>
        public virtual string Path
        {
            get
            {
                if (_currentPosition.Type == JsonContainerType.None)
                {
                    return string.Empty;
                }
                JsonPosition? currentPosition = (_currentState != JsonReader.State.ArrayStart && _currentState != JsonReader.State.ConstructorStart && _currentState != JsonReader.State.ObjectStart) ? new JsonPosition?(_currentPosition) : null;
                return JsonPosition.BuildPath(_stack, currentPosition);
            }
        }

        /// <summary>
        /// Gets or sets the culture used when reading JSON. Defaults to <see cref="P:System.Globalization.CultureInfo.InvariantCulture" />.
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

        internal JsonPosition GetPosition(int depth)
        {
            if (_stack != null && depth < _stack.Count)
            {
                return _stack[depth];
            }
            return _currentPosition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonReader" /> class.
        /// </summary>
        protected JsonReader()
        {
            _currentState = JsonReader.State.Start;
            _dateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
            _dateParseHandling = DateParseHandling.DateTime;
            _floatParseHandling = FloatParseHandling.Double;
            CloseInput = true;
        }

        private void Push(JsonContainerType value)
        {
            UpdateScopeWithFinishedValue();
            if (_currentPosition.Type == JsonContainerType.None)
            {
                _currentPosition = new JsonPosition(value);
                return;
            }
            if (_stack == null)
            {
                _stack = new List<JsonPosition>();
            }
            _stack.Add(_currentPosition);
            _currentPosition = new JsonPosition(value);
            if (_maxDepth != null && Depth + 1 > _maxDepth && !_hasExceededMaxDepth)
            {
                _hasExceededMaxDepth = true;
                throw JsonReaderException.Create(this, "The reader's MaxDepth of {0} has been exceeded.".FormatWith(CultureInfo.InvariantCulture, _maxDepth));
            }
        }

        private JsonContainerType Pop()
        {
            JsonPosition currentPosition;
            if (_stack != null && _stack.Count > 0)
            {
                currentPosition = _currentPosition;
                _currentPosition = _stack[_stack.Count - 1];
                _stack.RemoveAt(_stack.Count - 1);
            }
            else
            {
                currentPosition = _currentPosition;
                _currentPosition = default(JsonPosition);
            }
            if (_maxDepth != null && Depth <= _maxDepth)
            {
                _hasExceededMaxDepth = false;
            }
            return currentPosition.Type;
        }

        private JsonContainerType Peek()
        {
            return _currentPosition.Type;
        }

        /// <summary>
        /// Reads the next JSON token from the source.
        /// </summary>
        /// <returns><c>true</c> if the next token was read successfully; <c>false</c> if there are no more tokens to read.</returns>
        public abstract bool Read();

        /// <summary>
        /// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />. This method will return <c>null</c> at the end of an array.</returns>
        public virtual int? ReadAsInt32()
        {
            JsonToken contentToken = GetContentToken();
            if (contentToken != JsonToken.None)
            {
                switch (contentToken)
                {
                    case JsonToken.Integer:
                    case JsonToken.Float:
                        if (!(Value is int))
                        {
                            SetToken(JsonToken.Integer, Convert.ToInt32(Value, CultureInfo.InvariantCulture), false);
                        }
                        return new int?((int)Value);
                    case JsonToken.String:
                        {
                            string s = (string)Value;
                            return ReadInt32String(s);
                        }
                    case JsonToken.Null:
                    case JsonToken.EndArray:
                        goto IL_34;
                }
                throw JsonReaderException.Create(this, "Error reading integer. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
            }
        IL_34:
            return null;
        }

        internal int? ReadInt32String(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (int.TryParse(s, NumberStyles.Integer, Culture, out int num))
            {
                SetToken(JsonToken.Integer, num, false);
                return new int?(num);
            }
            SetToken(JsonToken.String, s, false);
            throw JsonReaderException.Create(this, "Could not convert string to integer: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
        }

        /// <summary>
        /// Reads the next JSON token from the source as a <see cref="T:System.String" />.
        /// </summary>
        /// <returns>A <see cref="T:System.String" />. This method will return <c>null</c> at the end of an array.</returns>
        public virtual string ReadAsString()
        {
            JsonToken contentToken = GetContentToken();
            if (contentToken <= JsonToken.String)
            {
                if (contentToken != JsonToken.None)
                {
                    if (contentToken != JsonToken.String)
                    {
                        goto IL_2E;
                    }
                    return (string)Value;
                }
            }
            else if (contentToken != JsonToken.Null && contentToken != JsonToken.EndArray)
            {
                goto IL_2E;
            }
            return null;
        IL_2E:
            if (JsonTokenUtils.IsPrimitiveToken(contentToken) && Value != null)
            {
                string text;
                if (Value is IFormattable)
                {
                    text = ((IFormattable)Value).ToString(null, Culture);
                }
                else if (Value is Uri)
                {
                    text = ((Uri)Value).OriginalString;
                }
                else
                {
                    text = Value.ToString();
                }
                SetToken(JsonToken.String, text, false);
                return text;
            }
            throw JsonReaderException.Create(this, "Error reading string. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
        }

        /// <summary>
        /// Reads the next JSON token from the source as a <see cref="T:System.Byte" />[].
        /// </summary>
        /// <returns>A <see cref="T:System.Byte" />[] or <c>null</c> if the next JSON token is null. This method will return <c>null</c> at the end of an array.</returns>
        public virtual byte[] ReadAsBytes()
        {
            JsonToken contentToken = GetContentToken();
            if (contentToken == JsonToken.None)
            {
                return null;
            }
            if (TokenType != JsonToken.StartObject)
            {
                if (contentToken <= JsonToken.String)
                {
                    if (contentToken == JsonToken.StartArray)
                    {
                        return ReadArrayIntoByteArray();
                    }
                    if (contentToken == JsonToken.String)
                    {
                        string text = (string)Value;
                        byte[] array;
                        if (text.Length == 0)
                        {
                            array = new byte[0];
                        }
                        else if (ConvertUtils.TryConvertGuid(text, out Guid guid))
                        {
                            array = guid.ToByteArray();
                        }
                        else
                        {
                            array = Convert.FromBase64String(text);
                        }
                        SetToken(JsonToken.Bytes, array, false);
                        return array;
                    }
                }
                else
                {
                    if (contentToken == JsonToken.Null || contentToken == JsonToken.EndArray)
                    {
                        return null;
                    }
                    if (contentToken == JsonToken.Bytes)
                    {
                        if (ValueType == typeof(Guid))
                        {
                            byte[] array2 = ((Guid)Value).ToByteArray();
                            SetToken(JsonToken.Bytes, array2, false);
                            return array2;
                        }
                        return (byte[])Value;
                    }
                }
                throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
            }
            ReadIntoWrappedTypeObject();
            byte[] array3 = ReadAsBytes();
            ReaderReadAndAssert();
            if (TokenType != JsonToken.EndObject)
            {
                throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
            }
            SetToken(JsonToken.Bytes, array3, false);
            return array3;
        }

        internal byte[] ReadArrayIntoByteArray()
        {
            List<byte> list = new List<byte>();
            JsonToken contentToken;
            for (; ; )
            {
                contentToken = GetContentToken();
                if (contentToken == JsonToken.None)
                {
                    goto IL_1B;
                }
                if (contentToken != JsonToken.Integer)
                {
                    break;
                }
                list.Add(Convert.ToByte(Value, CultureInfo.InvariantCulture));
            }
            if (contentToken != JsonToken.EndArray)
            {
                throw JsonReaderException.Create(this, "Unexpected token when reading bytes: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
            }
            byte[] array = list.ToArray();
            SetToken(JsonToken.Bytes, array, false);
            return array;
        IL_1B:
            throw JsonReaderException.Create(this, "Unexpected end when reading bytes.");
        }

        /// <summary>
        /// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />. This method will return <c>null</c> at the end of an array.</returns>
        public virtual double? ReadAsDouble()
        {
            JsonToken contentToken = GetContentToken();
            if (contentToken != JsonToken.None)
            {
                switch (contentToken)
                {
                    case JsonToken.Integer:
                    case JsonToken.Float:
                        if (!(Value is double))
                        {
                            double num = Convert.ToDouble(Value, CultureInfo.InvariantCulture);
                            SetToken(JsonToken.Float, num, false);
                        }
                        return new double?((double)Value);
                    case JsonToken.String:
                        return ReadDoubleString((string)Value);
                    case JsonToken.Null:
                    case JsonToken.EndArray:
                        goto IL_34;
                }
                throw JsonReaderException.Create(this, "Error reading double. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
            }
        IL_34:
            return null;
        }

        internal double? ReadDoubleString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (double.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, Culture, out double num))
            {
                SetToken(JsonToken.Float, num, false);
                return new double?(num);
            }
            SetToken(JsonToken.String, s, false);
            throw JsonReaderException.Create(this, "Could not convert string to double: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
        }

        /// <summary>
        /// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />. This method will return <c>null</c> at the end of an array.</returns>
        public virtual bool? ReadAsBoolean()
        {
            JsonToken contentToken = GetContentToken();
            if (contentToken != JsonToken.None)
            {
                switch (contentToken)
                {
                    case JsonToken.Integer:
                    case JsonToken.Float:
                        {
                            bool flag = Convert.ToBoolean(Value, CultureInfo.InvariantCulture);
                            SetToken(JsonToken.Boolean, flag, false);
                            return new bool?(flag);
                        }
                    case JsonToken.String:
                        return ReadBooleanString((string)Value);
                    case JsonToken.Boolean:
                        return new bool?((bool)Value);
                    case JsonToken.Null:
                    case JsonToken.EndArray:
                        goto IL_34;
                }
                throw JsonReaderException.Create(this, "Error reading boolean. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
            }
        IL_34:
            return null;
        }

        internal bool? ReadBooleanString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (bool.TryParse(s, out bool flag))
            {
                SetToken(JsonToken.Boolean, flag, false);
                return new bool?(flag);
            }
            SetToken(JsonToken.String, s, false);
            throw JsonReaderException.Create(this, "Could not convert string to boolean: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
        }

        /// <summary>
        /// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />. This method will return <c>null</c> at the end of an array.</returns>
        public virtual decimal? ReadAsDecimal()
        {
            JsonToken contentToken = GetContentToken();
            if (contentToken != JsonToken.None)
            {
                switch (contentToken)
                {
                    case JsonToken.Integer:
                    case JsonToken.Float:
                        if (!(Value is decimal))
                        {
                            SetToken(JsonToken.Float, Convert.ToDecimal(Value, CultureInfo.InvariantCulture), false);
                        }
                        return new decimal?((decimal)Value);
                    case JsonToken.String:
                        return ReadDecimalString((string)Value);
                    case JsonToken.Null:
                    case JsonToken.EndArray:
                        goto IL_34;
                }
                throw JsonReaderException.Create(this, "Error reading decimal. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
            }
        IL_34:
            return null;
        }

        internal decimal? ReadDecimalString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (decimal.TryParse(s, NumberStyles.Number, Culture, out decimal num))
            {
                SetToken(JsonToken.Float, num, false);
                return new decimal?(num);
            }
            SetToken(JsonToken.String, s, false);
            throw JsonReaderException.Create(this, "Could not convert string to decimal: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
        }

        /// <summary>
        /// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />. This method will return <c>null</c> at the end of an array.</returns>
        public virtual DateTime? ReadAsDateTime()
        {
            JsonToken contentToken = GetContentToken();
            if (contentToken <= JsonToken.String)
            {
                if (contentToken != JsonToken.None)
                {
                    if (contentToken != JsonToken.String)
                    {
                        goto IL_84;
                    }
                    string s = (string)Value;
                    return ReadDateTimeString(s);
                }
            }
            else if (contentToken != JsonToken.Null && contentToken != JsonToken.EndArray)
            {
                if (contentToken != JsonToken.Date)
                {
                    goto IL_84;
                }
                if (Value is DateTimeOffset)
                {
                    SetToken(JsonToken.Date, ((DateTimeOffset)Value).DateTime, false);
                }
                return new DateTime?((DateTime)Value);
            }
            return null;
        IL_84:
            throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
        }

        internal DateTime? ReadDateTimeString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (DateTimeUtils.TryParseDateTime(s, DateTimeZoneHandling, _dateFormatString, Culture, out DateTime dateTime))
            {
                dateTime = DateTimeUtils.EnsureDateTime(dateTime, DateTimeZoneHandling);
                SetToken(JsonToken.Date, dateTime, false);
                return new DateTime?(dateTime);
            }
            if (DateTime.TryParse(s, Culture, DateTimeStyles.RoundtripKind, out dateTime))
            {
                dateTime = DateTimeUtils.EnsureDateTime(dateTime, DateTimeZoneHandling);
                SetToken(JsonToken.Date, dateTime, false);
                return new DateTime?(dateTime);
            }
            throw JsonReaderException.Create(this, "Could not convert string to DateTime: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
        }

        /// <summary>
        /// Reads the next JSON token from the source as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />. This method will return <c>null</c> at the end of an array.</returns>
        public virtual DateTimeOffset? ReadAsDateTimeOffset()
        {
            JsonToken contentToken = GetContentToken();
            if (contentToken <= JsonToken.String)
            {
                if (contentToken != JsonToken.None)
                {
                    if (contentToken != JsonToken.String)
                    {
                        goto IL_81;
                    }
                    string s = (string)Value;
                    return ReadDateTimeOffsetString(s);
                }
            }
            else if (contentToken != JsonToken.Null && contentToken != JsonToken.EndArray)
            {
                if (contentToken != JsonToken.Date)
                {
                    goto IL_81;
                }
                if (Value is DateTime)
                {
                    SetToken(JsonToken.Date, new DateTimeOffset((DateTime)Value), false);
                }
                return new DateTimeOffset?((DateTimeOffset)Value);
            }
            return null;
        IL_81:
            throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
        }

        internal DateTimeOffset? ReadDateTimeOffsetString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                SetToken(JsonToken.Null, null, false);
                return null;
            }
            if (DateTimeUtils.TryParseDateTimeOffset(s, _dateFormatString, Culture, out DateTimeOffset dateTimeOffset))
            {
                SetToken(JsonToken.Date, dateTimeOffset, false);
                return new DateTimeOffset?(dateTimeOffset);
            }
            if (DateTimeOffset.TryParse(s, Culture, DateTimeStyles.RoundtripKind, out dateTimeOffset))
            {
                SetToken(JsonToken.Date, dateTimeOffset, false);
                return new DateTimeOffset?(dateTimeOffset);
            }
            SetToken(JsonToken.String, s, false);
            throw JsonReaderException.Create(this, "Could not convert string to DateTimeOffset: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
        }

        internal void ReaderReadAndAssert()
        {
            if (!Read())
            {
                throw CreateUnexpectedEndException();
            }
        }

        internal JsonReaderException CreateUnexpectedEndException()
        {
            return JsonReaderException.Create(this, "Unexpected end when reading JSON.");
        }

        internal void ReadIntoWrappedTypeObject()
        {
            ReaderReadAndAssert();
            if (Value.ToString() == "$type")
            {
                ReaderReadAndAssert();
                if (Value != null && Value.ToString().StartsWith("System.Byte[]", StringComparison.Ordinal))
                {
                    ReaderReadAndAssert();
                    if (Value.ToString() == "$value")
                    {
                        return;
                    }
                }
            }
            throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, JsonToken.StartObject));
        }

        /// <summary>
        /// Skips the children of the current token.
        /// </summary>
        public void Skip()
        {
            if (TokenType == JsonToken.PropertyName)
            {
                Read();
            }
            if (JsonTokenUtils.IsStartToken(TokenType))
            {
                int depth = Depth;
                while (Read() && depth < Depth)
                {
                }
            }
        }

        /// <summary>
        /// Sets the current token.
        /// </summary>
        /// <param name="newToken">The new token.</param>
        protected void SetToken(JsonToken newToken)
        {
            SetToken(newToken, null, true);
        }

        /// <summary>
        /// Sets the current token and value.
        /// </summary>
        /// <param name="newToken">The new token.</param>
        /// <param name="value">The value.</param>
        protected void SetToken(JsonToken newToken, object value)
        {
            SetToken(newToken, value, true);
        }

        /// <summary>
        /// Sets the current token and value.
        /// </summary>
        /// <param name="newToken">The new token.</param>
        /// <param name="value">The value.</param>
        /// <param name="updateIndex">A flag indicating whether the position index inside an array should be updated.</param>
        internal void SetToken(JsonToken newToken, object value, bool updateIndex)
        {
            _tokenType = newToken;
            _value = value;
            switch (newToken)
            {
                case JsonToken.StartObject:
                    _currentState = JsonReader.State.ObjectStart;
                    Push(JsonContainerType.Object);
                    return;
                case JsonToken.StartArray:
                    _currentState = JsonReader.State.ArrayStart;
                    Push(JsonContainerType.Array);
                    return;
                case JsonToken.StartConstructor:
                    _currentState = JsonReader.State.ConstructorStart;
                    Push(JsonContainerType.Constructor);
                    return;
                case JsonToken.PropertyName:
                    _currentState = JsonReader.State.Property;
                    _currentPosition.PropertyName = (string)value;
                    return;
                case JsonToken.Comment:
                    break;
                case JsonToken.Raw:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Null:
                case JsonToken.Undefined:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    SetPostValueState(updateIndex);
                    break;
                case JsonToken.EndObject:
                    ValidateEnd(JsonToken.EndObject);
                    return;
                case JsonToken.EndArray:
                    ValidateEnd(JsonToken.EndArray);
                    return;
                case JsonToken.EndConstructor:
                    ValidateEnd(JsonToken.EndConstructor);
                    return;
                default:
                    return;
            }
        }

        internal void SetPostValueState(bool updateIndex)
        {
            if (Peek() != JsonContainerType.None)
            {
                _currentState = JsonReader.State.PostValue;
            }
            else
            {
                SetFinished();
            }
            if (updateIndex)
            {
                UpdateScopeWithFinishedValue();
            }
        }

        private void UpdateScopeWithFinishedValue()
        {
            if (_currentPosition.HasIndex)
            {
                _currentPosition.Position = _currentPosition.Position + 1;
            }
        }

        private void ValidateEnd(JsonToken endToken)
        {
            JsonContainerType jsonContainerType = Pop();
            if (GetTypeForCloseToken(endToken) != jsonContainerType)
            {
                throw JsonReaderException.Create(this, "JsonToken {0} is not valid for closing JsonType {1}.".FormatWith(CultureInfo.InvariantCulture, endToken, jsonContainerType));
            }
            if (Peek() != JsonContainerType.None)
            {
                _currentState = JsonReader.State.PostValue;
                return;
            }
            SetFinished();
        }

        /// <summary>
        /// Sets the state based on current token type.
        /// </summary>
        protected void SetStateBasedOnCurrent()
        {
            JsonContainerType jsonContainerType = Peek();
            switch (jsonContainerType)
            {
                case JsonContainerType.None:
                    SetFinished();
                    return;
                case JsonContainerType.Object:
                    _currentState = JsonReader.State.Object;
                    return;
                case JsonContainerType.Array:
                    _currentState = JsonReader.State.Array;
                    return;
                case JsonContainerType.Constructor:
                    _currentState = JsonReader.State.Constructor;
                    return;
                default:
                    throw JsonReaderException.Create(this, "While setting the reader state back to current object an unexpected JsonType was encountered: {0}".FormatWith(CultureInfo.InvariantCulture, jsonContainerType));
            }
        }

        private void SetFinished()
        {
            if (SupportMultipleContent)
            {
                _currentState = JsonReader.State.Start;
                return;
            }
            _currentState = JsonReader.State.Finished;
        }

        private JsonContainerType GetTypeForCloseToken(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.EndObject:
                    return JsonContainerType.Object;
                case JsonToken.EndArray:
                    return JsonContainerType.Array;
                case JsonToken.EndConstructor:
                    return JsonContainerType.Constructor;
                default:
                    throw JsonReaderException.Create(this, "Not a valid close JsonToken: {0}".FormatWith(CultureInfo.InvariantCulture, token));
            }
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
            if (_currentState != JsonReader.State.Closed && disposing)
            {
                Close();
            }
        }

        /// <summary>
        /// Changes the reader's state to <see cref="F:Newtonsoft.Json.JsonReader.State.Closed" />.
        /// If <see cref="P:Newtonsoft.Json.JsonReader.CloseInput" /> is set to <c>true</c>, the source is also closed.
        /// </summary>
        public virtual void Close()
        {
            _currentState = JsonReader.State.Closed;
            _tokenType = JsonToken.None;
            _value = null;
        }

        internal void ReadAndAssert()
        {
            if (!Read())
            {
                throw JsonSerializationException.Create(this, "Unexpected end when reading JSON.");
            }
        }

        internal bool ReadAndMoveToContent()
        {
            return Read() && MoveToContent();
        }

        internal bool MoveToContent()
        {
            JsonToken tokenType = TokenType;
            while (tokenType == JsonToken.None || tokenType == JsonToken.Comment)
            {
                if (!Read())
                {
                    return false;
                }
                tokenType = TokenType;
            }
            return true;
        }

        private JsonToken GetContentToken()
        {
            while (Read())
            {
                JsonToken tokenType = TokenType;
                if (tokenType != JsonToken.Comment)
                {
                    return tokenType;
                }
            }
            SetToken(JsonToken.None);
            return JsonToken.None;
        }

        private JsonToken _tokenType;

        private object _value;

        internal char _quoteChar;

        internal JsonReader.State _currentState;

        private JsonPosition _currentPosition;

        private CultureInfo _culture;

        private DateTimeZoneHandling _dateTimeZoneHandling;

        private int? _maxDepth;

        private bool _hasExceededMaxDepth;

        internal DateParseHandling _dateParseHandling;

        internal FloatParseHandling _floatParseHandling;

        private string _dateFormatString;

        private List<JsonPosition> _stack;

        /// <summary>
        /// Specifies the state of the reader.
        /// </summary>
        protected internal enum State
        {
            /// <summary>
            /// A <see cref="T:Newtonsoft.Json.JsonReader" /> read method has not been called.
            /// </summary>
            Start,
            /// <summary>
            /// The end of the file has been reached successfully.
            /// </summary>
            Complete,
            /// <summary>
            /// Reader is at a property.
            /// </summary>
            Property,
            /// <summary>
            /// Reader is at the start of an object.
            /// </summary>
            ObjectStart,
            /// <summary>
            /// Reader is in an object.
            /// </summary>
            Object,
            /// <summary>
            /// Reader is at the start of an array.
            /// </summary>
            ArrayStart,
            /// <summary>
            /// Reader is in an array.
            /// </summary>
            Array,
            /// <summary>
            /// The <see cref="M:Newtonsoft.Json.JsonReader.Close" /> method has been called.
            /// </summary>
            Closed,
            /// <summary>
            /// Reader has just read a value.
            /// </summary>
            PostValue,
            /// <summary>
            /// Reader is at the start of a constructor.
            /// </summary>
            ConstructorStart,
            /// <summary>
            /// Reader is in a constructor.
            /// </summary>
            Constructor,
            /// <summary>
            /// An error occurred that prevents the read operation from continuing.
            /// </summary>
            Error,
            /// <summary>
            /// The end of the file has been reached successfully.
            /// </summary>
            Finished
        }
    }
}
