using Newtonsoft_X.Json.Utilities;
using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to JSON text data.
    /// </summary>
    public class JsonTextReader : JsonReader, IJsonLineInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonTextReader" /> class with the specified <see cref="T:System.IO.TextReader" />.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.IO.TextReader" /> containing the JSON data to read.</param>
        public JsonTextReader(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            _reader = reader;
            _lineNumber = 1;
        }

        /// <summary>
        /// Gets or sets the reader's character buffer pool.
        /// </summary>
        public IArrayPool<char> ArrayPool
        {
            get
            {
                return _arrayPool;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _arrayPool = value;
            }
        }

        private void EnsureBufferNotEmpty()
        {
            if (_stringBuffer.IsEmpty)
            {
                _stringBuffer = new StringBuffer(_arrayPool, 1024);
            }
        }

        private void OnNewLine(int pos)
        {
            _lineNumber++;
            _lineStartPos = pos;
        }

        private void ParseString(char quote, ReadType readType)
        {
            _charPos++;
            ShiftBufferIfNeeded();
            ReadStringIntoBuffer(quote);
            base.SetPostValueState(true);
            switch (readType)
            {
                case ReadType.ReadAsInt32:
                case ReadType.ReadAsDecimal:
                case ReadType.ReadAsBoolean:
                    return;
                case ReadType.ReadAsBytes:
                    {
                        byte[] value;
                        if (_stringReference.Length == 0)
                        {
                            value = new byte[0];
                        }
                        else if (_stringReference.Length == 36 && ConvertUtils.TryConvertGuid(_stringReference.ToString(), out Guid guid))
                        {
                            value = guid.ToByteArray();
                        }
                        else
                        {
                            value = Convert.FromBase64CharArray(_stringReference.Chars, _stringReference.StartIndex, _stringReference.Length);
                        }
                        base.SetToken(JsonToken.Bytes, value, false);
                        return;
                    }
                case ReadType.ReadAsString:
                    {
                        string value2 = _stringReference.ToString();
                        base.SetToken(JsonToken.String, value2, false);
                        _quoteChar = quote;
                        return;
                    }
            }
            if (_dateParseHandling != DateParseHandling.None)
            {
                DateParseHandling dateParseHandling;
                if (readType == ReadType.ReadAsDateTime)
                {
                    dateParseHandling = DateParseHandling.DateTime;
                }
                else if (readType == ReadType.ReadAsDateTimeOffset)
                {
                    dateParseHandling = DateParseHandling.DateTimeOffset;
                }
                else
                {
                    dateParseHandling = _dateParseHandling;
                }
                if (dateParseHandling == DateParseHandling.DateTime)
                {
                    if (DateTimeUtils.TryParseDateTime(_stringReference, base.DateTimeZoneHandling, base.DateFormatString, base.Culture, out DateTime dateTime))
                    {
                        base.SetToken(JsonToken.Date, dateTime, false);
                        return;
                    }
                }
                else if (DateTimeUtils.TryParseDateTimeOffset(_stringReference, base.DateFormatString, base.Culture, out DateTimeOffset dateTimeOffset))
                {
                    base.SetToken(JsonToken.Date, dateTimeOffset, false);
                    return;
                }
            }
            base.SetToken(JsonToken.String, _stringReference.ToString(), false);
            _quoteChar = quote;
        }

        private static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
        {
            Buffer.BlockCopy(src, srcOffset * 2, dst, dstOffset * 2, count * 2);
        }

        private void ShiftBufferIfNeeded()
        {
            int num = _chars.Length;
            if (num - _charPos <= num * 0.1)
            {
                int num2 = _charsUsed - _charPos;
                if (num2 > 0)
                {
                    JsonTextReader.BlockCopyChars(_chars, _charPos, _chars, 0, num2);
                }
                _lineStartPos -= _charPos;
                _charPos = 0;
                _charsUsed = num2;
                _chars[_charsUsed] = '\0';
            }
        }

        private int ReadData(bool append)
        {
            return ReadData(append, 0);
        }

        private int ReadData(bool append, int charsRequired)
        {
            if (_isEndOfFile)
            {
                return 0;
            }
            if (_charsUsed + charsRequired >= _chars.Length - 1)
            {
                if (append)
                {
                    int minSize = Math.Max(_chars.Length * 2, _charsUsed + charsRequired + 1);
                    char[] array = BufferUtils.RentBuffer(_arrayPool, minSize);
                    JsonTextReader.BlockCopyChars(_chars, 0, array, 0, _chars.Length);
                    BufferUtils.ReturnBuffer(_arrayPool, _chars);
                    _chars = array;
                }
                else
                {
                    int num = _charsUsed - _charPos;
                    if (num + charsRequired + 1 >= _chars.Length)
                    {
                        char[] array2 = BufferUtils.RentBuffer(_arrayPool, num + charsRequired + 1);
                        if (num > 0)
                        {
                            JsonTextReader.BlockCopyChars(_chars, _charPos, array2, 0, num);
                        }
                        BufferUtils.ReturnBuffer(_arrayPool, _chars);
                        _chars = array2;
                    }
                    else if (num > 0)
                    {
                        JsonTextReader.BlockCopyChars(_chars, _charPos, _chars, 0, num);
                    }
                    _lineStartPos -= _charPos;
                    _charPos = 0;
                    _charsUsed = num;
                }
            }
            int count = _chars.Length - _charsUsed - 1;
            int num2 = _reader.Read(_chars, _charsUsed, count);
            _charsUsed += num2;
            if (num2 == 0)
            {
                _isEndOfFile = true;
            }
            _chars[_charsUsed] = '\0';
            return num2;
        }

        private bool EnsureChars(int relativePosition, bool append)
        {
            return _charPos + relativePosition < _charsUsed || ReadChars(relativePosition, append);
        }

        private bool ReadChars(int relativePosition, bool append)
        {
            if (_isEndOfFile)
            {
                return false;
            }
            int num = _charPos + relativePosition - _charsUsed + 1;
            int num2 = 0;
            do
            {
                int num3 = ReadData(append, num - num2);
                if (num3 == 0)
                {
                    break;
                }
                num2 += num3;
            }
            while (num2 < num);
            return num2 >= num;
        }

        /// <summary>
        /// Reads the next JSON token from the underlying <see cref="T:System.IO.TextReader" />.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the next token was read successfully; <c>false</c> if there are no more tokens to read.
        /// </returns>
        public override bool Read()
        {
            EnsureBuffer();
            for (; ; )
            {
                switch (_currentState)
                {
                    case JsonReader.State.Start:
                    case JsonReader.State.Property:
                    case JsonReader.State.ArrayStart:
                    case JsonReader.State.Array:
                    case JsonReader.State.ConstructorStart:
                    case JsonReader.State.Constructor:
                        goto IL_4C;
                    case JsonReader.State.ObjectStart:
                    case JsonReader.State.Object:
                        goto IL_53;
                    case JsonReader.State.PostValue:
                        if (ParsePostValue())
                        {
                            return true;
                        }
                        continue;
                    case JsonReader.State.Finished:
                        goto IL_64;
                }
                break;
            }
            goto IL_D2;
        IL_4C:
            return ParseValue();
        IL_53:
            return ParseObject();
        IL_64:
            if (!EnsureChars(0, false))
            {
                base.SetToken(JsonToken.None);
                return false;
            }
            EatWhitespace(false);
            if (_isEndOfFile)
            {
                base.SetToken(JsonToken.None);
                return false;
            }
            if (_chars[_charPos] == '/')
            {
                ParseComment(true);
                return true;
            }
            throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
        IL_D2:
            throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
        }

        /// <summary>
        /// Reads the next JSON token from the underlying <see cref="T:System.IO.TextReader" /> as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />. This method will return <c>null</c> at the end of an array.</returns>
        public override int? ReadAsInt32()
        {
            return (int?)ReadNumberValue(ReadType.ReadAsInt32);
        }

        /// <summary>
        /// Reads the next JSON token from the underlying <see cref="T:System.IO.TextReader" /> as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />. This method will return <c>null</c> at the end of an array.</returns>
        public override DateTime? ReadAsDateTime()
        {
            return (DateTime?)ReadStringValue(ReadType.ReadAsDateTime);
        }

        /// <summary>
        /// Reads the next JSON token from the underlying <see cref="T:System.IO.TextReader" /> as a <see cref="T:System.String" />.
        /// </summary>
        /// <returns>A <see cref="T:System.String" />. This method will return <c>null</c> at the end of an array.</returns>
        public override string ReadAsString()
        {
            return (string)ReadStringValue(ReadType.ReadAsString);
        }

        /// <summary>
        /// Reads the next JSON token from the underlying <see cref="T:System.IO.TextReader" /> as a <see cref="T:System.Byte" />[].
        /// </summary>
        /// <returns>A <see cref="T:System.Byte" />[] or <c>null</c> if the next JSON token is null. This method will return <c>null</c> at the end of an array.</returns>
        public override byte[] ReadAsBytes()
        {
            EnsureBuffer();
            bool flag = false;
            switch (_currentState)
            {
                case JsonReader.State.Start:
                case JsonReader.State.Property:
                case JsonReader.State.ArrayStart:
                case JsonReader.State.Array:
                case JsonReader.State.PostValue:
                case JsonReader.State.ConstructorStart:
                case JsonReader.State.Constructor:
                    {
                        char c;
                        for (; ; )
                        {
                            c = _chars[_charPos];
                            if (c <= '\'')
                            {
                                if (c <= '\r')
                                {
                                    if (c != '\0')
                                    {
                                        switch (c)
                                        {
                                            case '\t':
                                                break;
                                            case '\n':
                                                ProcessLineFeed();
                                                continue;
                                            case '\v':
                                            case '\f':
                                                goto IL_20A;
                                            case '\r':
                                                ProcessCarriageReturn(false);
                                                continue;
                                            default:
                                                goto IL_20A;
                                        }
                                    }
                                    else
                                    {
                                        if (ReadNullChar())
                                        {
                                            break;
                                        }
                                        continue;
                                    }
                                }
                                else if (c != ' ')
                                {
                                    if (c != '"' && c != '\'')
                                    {
                                        goto IL_20A;
                                    }
                                    goto IL_F4;
                                }
                                _charPos++;
                                continue;
                            }
                            if (c <= '[')
                            {
                                if (c == ',')
                                {
                                    ProcessValueComma();
                                    continue;
                                }
                                if (c == '/')
                                {
                                    ParseComment(false);
                                    continue;
                                }
                                if (c == '[')
                                {
                                    goto IL_16A;
                                }
                            }
                            else
                            {
                                if (c == ']')
                                {
                                    goto IL_1A5;
                                }
                                if (c == 'n')
                                {
                                    goto IL_186;
                                }
                                if (c == '{')
                                {
                                    _charPos++;
                                    base.SetToken(JsonToken.StartObject);
                                    base.ReadIntoWrappedTypeObject();
                                    flag = true;
                                    continue;
                                }
                            }
                        IL_20A:
                            _charPos++;
                            if (!char.IsWhiteSpace(c))
                            {
                                goto Block_21;
                            }
                        }
                        base.SetToken(JsonToken.None, null, false);
                        return null;
                    IL_F4:
                        ParseString(c, ReadType.ReadAsBytes);
                        byte[] array = (byte[])Value;
                        if (flag)
                        {
                            base.ReaderReadAndAssert();
                            if (TokenType != JsonToken.EndObject)
                            {
                                throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, TokenType));
                            }
                            base.SetToken(JsonToken.Bytes, array, false);
                        }
                        return array;
                    IL_16A:
                        _charPos++;
                        base.SetToken(JsonToken.StartArray);
                        return base.ReadArrayIntoByteArray();
                    IL_186:
                        HandleNull();
                        return null;
                    IL_1A5:
                        _charPos++;
                        if (_currentState == JsonReader.State.Array || _currentState == JsonReader.State.ArrayStart || _currentState == JsonReader.State.PostValue)
                        {
                            base.SetToken(JsonToken.EndArray);
                            return null;
                        }
                        throw CreateUnexpectedCharacterException(c);
                    Block_21:
                        throw CreateUnexpectedCharacterException(c);
                    }
                case JsonReader.State.Finished:
                    ReadFinished();
                    return null;
            }
            throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
        }

        private object ReadStringValue(ReadType readType)
        {
            EnsureBuffer();
            switch (_currentState)
            {
                case JsonReader.State.Start:
                case JsonReader.State.Property:
                case JsonReader.State.ArrayStart:
                case JsonReader.State.Array:
                case JsonReader.State.PostValue:
                case JsonReader.State.ConstructorStart:
                case JsonReader.State.Constructor:
                    {
                        char c;
                        for (; ; )
                        {
                            c = _chars[_charPos];
                            if (c <= 'I')
                            {
                                if (c <= '\r')
                                {
                                    if (c != '\0')
                                    {
                                        switch (c)
                                        {
                                            case '\t':
                                                break;
                                            case '\n':
                                                ProcessLineFeed();
                                                continue;
                                            case '\v':
                                            case '\f':
                                                goto IL_346;
                                            case '\r':
                                                ProcessCarriageReturn(false);
                                                continue;
                                            default:
                                                goto IL_346;
                                        }
                                    }
                                    else
                                    {
                                        if (ReadNullChar())
                                        {
                                            break;
                                        }
                                        continue;
                                    }
                                }
                                else
                                {
                                    switch (c)
                                    {
                                        case ' ':
                                            break;
                                        case '!':
                                        case '#':
                                        case '$':
                                        case '%':
                                        case '&':
                                        case '(':
                                        case ')':
                                        case '*':
                                        case '+':
                                            goto IL_346;
                                        case '"':
                                        case '\'':
                                            goto IL_15A;
                                        case ',':
                                            ProcessValueComma();
                                            continue;
                                        case '-':
                                            goto IL_203;
                                        case '.':
                                        case '0':
                                        case '1':
                                        case '2':
                                        case '3':
                                        case '4':
                                        case '5':
                                        case '6':
                                        case '7':
                                        case '8':
                                        case '9':
                                            goto IL_236;
                                        case '/':
                                            ParseComment(false);
                                            continue;
                                        default:
                                            if (c != 'I')
                                            {
                                                goto IL_346;
                                            }
                                            goto IL_2B2;
                                    }
                                }
                                _charPos++;
                                continue;
                            }
                            if (c <= ']')
                            {
                                if (c == 'N')
                                {
                                    goto IL_2BA;
                                }
                                if (c == ']')
                                {
                                    goto IL_2E1;
                                }
                            }
                            else
                            {
                                if (c == 'f')
                                {
                                    goto IL_25E;
                                }
                                if (c == 'n')
                                {
                                    goto IL_2C2;
                                }
                                if (c == 't')
                                {
                                    goto IL_25E;
                                }
                            }
                        IL_346:
                            _charPos++;
                            if (!char.IsWhiteSpace(c))
                            {
                                goto Block_26;
                            }
                        }
                        base.SetToken(JsonToken.None, null, false);
                        return null;
                    IL_15A:
                        ParseString(c, readType);
                        switch (readType)
                        {
                            case ReadType.ReadAsBytes:
                                return Value;
                            case ReadType.ReadAsString:
                                return Value;
                            case ReadType.ReadAsDateTime:
                                if (Value is DateTime)
                                {
                                    return (DateTime)Value;
                                }
                                return base.ReadDateTimeString((string)Value);
                            case ReadType.ReadAsDateTimeOffset:
                                if (Value is DateTimeOffset)
                                {
                                    return (DateTimeOffset)Value;
                                }
                                return base.ReadDateTimeOffsetString((string)Value);
                        }
                        throw new ArgumentOutOfRangeException("readType");
                    IL_203:
                        if (EnsureChars(1, true) && _chars[_charPos + 1] == 'I')
                        {
                            return ParseNumberNegativeInfinity(readType);
                        }
                        ParseNumber(readType);
                        return Value;
                    IL_236:
                        if (readType != ReadType.ReadAsString)
                        {
                            _charPos++;
                            throw CreateUnexpectedCharacterException(c);
                        }
                        ParseNumber(ReadType.ReadAsString);
                        return Value;
                    IL_25E:
                        if (readType != ReadType.ReadAsString)
                        {
                            _charPos++;
                            throw CreateUnexpectedCharacterException(c);
                        }
                        string text = (c == 't') ? JsonConvert.True : JsonConvert.False;
                        if (!MatchValueWithTrailingSeparator(text))
                        {
                            throw CreateUnexpectedCharacterException(_chars[_charPos]);
                        }
                        base.SetToken(JsonToken.String, text);
                        return text;
                    IL_2B2:
                        return ParseNumberPositiveInfinity(readType);
                    IL_2BA:
                        return ParseNumberNaN(readType);
                    IL_2C2:
                        HandleNull();
                        return null;
                    IL_2E1:
                        _charPos++;
                        if (_currentState == JsonReader.State.Array || _currentState == JsonReader.State.ArrayStart || _currentState == JsonReader.State.PostValue)
                        {
                            base.SetToken(JsonToken.EndArray);
                            return null;
                        }
                        throw CreateUnexpectedCharacterException(c);
                    Block_26:
                        throw CreateUnexpectedCharacterException(c);
                    }
                case JsonReader.State.Finished:
                    ReadFinished();
                    return null;
            }
            throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
        }

        private JsonReaderException CreateUnexpectedCharacterException(char c)
        {
            return JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
        }

        /// <summary>
        /// Reads the next JSON token from the underlying <see cref="T:System.IO.TextReader" /> as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />. This method will return <c>null</c> at the end of an array.</returns>
        public override bool? ReadAsBoolean()
        {
            EnsureBuffer();
            switch (_currentState)
            {
                case JsonReader.State.Start:
                case JsonReader.State.Property:
                case JsonReader.State.ArrayStart:
                case JsonReader.State.Array:
                case JsonReader.State.PostValue:
                case JsonReader.State.ConstructorStart:
                case JsonReader.State.Constructor:
                    {
                        char c;
                        for (; ; )
                        {
                            c = _chars[_charPos];
                            if (c <= '9')
                            {
                                if (c != '\0')
                                {
                                    switch (c)
                                    {
                                        case '\t':
                                            break;
                                        case '\n':
                                            ProcessLineFeed();
                                            continue;
                                        case '\v':
                                        case '\f':
                                            goto IL_274;
                                        case '\r':
                                            ProcessCarriageReturn(false);
                                            continue;
                                        default:
                                            switch (c)
                                            {
                                                case ' ':
                                                    break;
                                                case '!':
                                                case '#':
                                                case '$':
                                                case '%':
                                                case '&':
                                                case '(':
                                                case ')':
                                                case '*':
                                                case '+':
                                                    goto IL_274;
                                                case '"':
                                                case '\'':
                                                    goto IL_146;
                                                case ',':
                                                    ProcessValueComma();
                                                    continue;
                                                case '-':
                                                case '.':
                                                case '0':
                                                case '1':
                                                case '2':
                                                case '3':
                                                case '4':
                                                case '5':
                                                case '6':
                                                case '7':
                                                case '8':
                                                case '9':
                                                    goto IL_177;
                                                case '/':
                                                    ParseComment(false);
                                                    continue;
                                                default:
                                                    goto IL_274;
                                            }
                                            break;
                                    }
                                    _charPos++;
                                    continue;
                                }
                                if (ReadNullChar())
                                {
                                    break;
                                }
                                continue;
                            }
                            else if (c <= 'f')
                            {
                                if (c == ']')
                                {
                                    goto IL_206;
                                }
                                if (c == 'f')
                                {
                                    goto IL_1A5;
                                }
                            }
                            else
                            {
                                if (c == 'n')
                                {
                                    goto IL_166;
                                }
                                if (c == 't')
                                {
                                    goto IL_1A5;
                                }
                            }
                        IL_274:
                            _charPos++;
                            if (!char.IsWhiteSpace(c))
                            {
                                goto Block_16;
                            }
                        }
                        base.SetToken(JsonToken.None, null, false);
                        return null;
                    IL_146:
                        ParseString(c, ReadType.Read);
                        return base.ReadBooleanString(_stringReference.ToString());
                    IL_166:
                        HandleNull();
                        return null;
                    IL_177:
                        ParseNumber(ReadType.Read);
                        bool flag = Convert.ToBoolean(Value, CultureInfo.InvariantCulture);
                        base.SetToken(JsonToken.Boolean, flag, false);
                        return new bool?(flag);
                    IL_1A5:
                        bool flag2 = c == 't';
                        string value = flag2 ? JsonConvert.True : JsonConvert.False;
                        if (!MatchValueWithTrailingSeparator(value))
                        {
                            throw CreateUnexpectedCharacterException(_chars[_charPos]);
                        }
                        base.SetToken(JsonToken.Boolean, flag2);
                        return new bool?(flag2);
                    IL_206:
                        _charPos++;
                        if (_currentState == JsonReader.State.Array || _currentState == JsonReader.State.ArrayStart || _currentState == JsonReader.State.PostValue)
                        {
                            base.SetToken(JsonToken.EndArray);
                            return null;
                        }
                        throw CreateUnexpectedCharacterException(c);
                    Block_16:
                        throw CreateUnexpectedCharacterException(c);
                    }
                case JsonReader.State.Finished:
                    ReadFinished();
                    return null;
            }
            throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
        }

        private void ProcessValueComma()
        {
            _charPos++;
            if (_currentState != JsonReader.State.PostValue)
            {
                base.SetToken(JsonToken.Undefined);
                throw CreateUnexpectedCharacterException(',');
            }
            base.SetStateBasedOnCurrent();
        }

        private object ReadNumberValue(ReadType readType)
        {
            EnsureBuffer();
            switch (_currentState)
            {
                case JsonReader.State.Start:
                case JsonReader.State.Property:
                case JsonReader.State.ArrayStart:
                case JsonReader.State.Array:
                case JsonReader.State.PostValue:
                case JsonReader.State.ConstructorStart:
                case JsonReader.State.Constructor:
                    {
                        char c;
                        for (; ; )
                        {
                            c = _chars[_charPos];
                            if (c <= '9')
                            {
                                if (c != '\0')
                                {
                                    switch (c)
                                    {
                                        case '\t':
                                            break;
                                        case '\n':
                                            ProcessLineFeed();
                                            continue;
                                        case '\v':
                                        case '\f':
                                            goto IL_28D;
                                        case '\r':
                                            ProcessCarriageReturn(false);
                                            continue;
                                        default:
                                            switch (c)
                                            {
                                                case ' ':
                                                    break;
                                                case '!':
                                                case '#':
                                                case '$':
                                                case '%':
                                                case '&':
                                                case '(':
                                                case ')':
                                                case '*':
                                                case '+':
                                                    goto IL_28D;
                                                case '"':
                                                case '\'':
                                                    goto IL_140;
                                                case ',':
                                                    ProcessValueComma();
                                                    continue;
                                                case '-':
                                                    goto IL_1D0;
                                                case '.':
                                                case '0':
                                                case '1':
                                                case '2':
                                                case '3':
                                                case '4':
                                                case '5':
                                                case '6':
                                                case '7':
                                                case '8':
                                                case '9':
                                                    goto IL_203;
                                                case '/':
                                                    ParseComment(false);
                                                    continue;
                                                default:
                                                    goto IL_28D;
                                            }
                                            break;
                                    }
                                    _charPos++;
                                    continue;
                                }
                                if (ReadNullChar())
                                {
                                    break;
                                }
                                continue;
                            }
                            else if (c <= 'N')
                            {
                                if (c == 'I')
                                {
                                    goto IL_1C8;
                                }
                                if (c == 'N')
                                {
                                    goto IL_1C0;
                                }
                            }
                            else
                            {
                                if (c == ']')
                                {
                                    goto IL_228;
                                }
                                if (c == 'n')
                                {
                                    goto IL_1B8;
                                }
                            }
                        IL_28D:
                            _charPos++;
                            if (!char.IsWhiteSpace(c))
                            {
                                goto Block_19;
                            }
                        }
                        base.SetToken(JsonToken.None, null, false);
                        return null;
                    IL_140:
                        ParseString(c, readType);
                        if (readType == ReadType.ReadAsInt32)
                        {
                            return base.ReadInt32String(_stringReference.ToString());
                        }
                        if (readType == ReadType.ReadAsDecimal)
                        {
                            return base.ReadDecimalString(_stringReference.ToString());
                        }
                        if (readType != ReadType.ReadAsDouble)
                        {
                            throw new ArgumentOutOfRangeException("readType");
                        }
                        return base.ReadDoubleString(_stringReference.ToString());
                    IL_1B8:
                        HandleNull();
                        return null;
                    IL_1C0:
                        return ParseNumberNaN(readType);
                    IL_1C8:
                        return ParseNumberPositiveInfinity(readType);
                    IL_1D0:
                        if (EnsureChars(1, true) && _chars[_charPos + 1] == 'I')
                        {
                            return ParseNumberNegativeInfinity(readType);
                        }
                        ParseNumber(readType);
                        return Value;
                    IL_203:
                        ParseNumber(readType);
                        return Value;
                    IL_228:
                        _charPos++;
                        if (_currentState == JsonReader.State.Array || _currentState == JsonReader.State.ArrayStart || _currentState == JsonReader.State.PostValue)
                        {
                            base.SetToken(JsonToken.EndArray);
                            return null;
                        }
                        throw CreateUnexpectedCharacterException(c);
                    Block_19:
                        throw CreateUnexpectedCharacterException(c);
                    }
                case JsonReader.State.Finished:
                    ReadFinished();
                    return null;
            }
            throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
        }

        /// <summary>
        /// Reads the next JSON token from the underlying <see cref="T:System.IO.TextReader" /> as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />. This method will return <c>null</c> at the end of an array.</returns>
        public override DateTimeOffset? ReadAsDateTimeOffset()
        {
            return (DateTimeOffset?)ReadStringValue(ReadType.ReadAsDateTimeOffset);
        }

        /// <summary>
        /// Reads the next JSON token from the underlying <see cref="T:System.IO.TextReader" /> as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />. This method will return <c>null</c> at the end of an array.</returns>
        public override decimal? ReadAsDecimal()
        {
            return (decimal?)ReadNumberValue(ReadType.ReadAsDecimal);
        }

        /// <summary>
        /// Reads the next JSON token from the underlying <see cref="T:System.IO.TextReader" /> as a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />.
        /// </summary>
        /// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />. This method will return <c>null</c> at the end of an array.</returns>
        public override double? ReadAsDouble()
        {
            return (double?)ReadNumberValue(ReadType.ReadAsDouble);
        }

        private void HandleNull()
        {
            if (!EnsureChars(1, true))
            {
                _charPos = _charsUsed;
                throw base.CreateUnexpectedEndException();
            }
            if (_chars[_charPos + 1] == 'u')
            {
                ParseNull();
                return;
            }
            _charPos += 2;
            throw CreateUnexpectedCharacterException(_chars[_charPos - 1]);
        }

        private void ReadFinished()
        {
            if (EnsureChars(0, false))
            {
                EatWhitespace(false);
                if (_isEndOfFile)
                {
                    return;
                }
                if (_chars[_charPos] != '/')
                {
                    throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
                }
                ParseComment(false);
            }
            base.SetToken(JsonToken.None);
        }

        private bool ReadNullChar()
        {
            if (_charsUsed == _charPos)
            {
                if (ReadData(false) == 0)
                {
                    _isEndOfFile = true;
                    return true;
                }
            }
            else
            {
                _charPos++;
            }
            return false;
        }

        private void EnsureBuffer()
        {
            if (_chars == null)
            {
                _chars = BufferUtils.RentBuffer(_arrayPool, 1024);
                _chars[0] = '\0';
            }
        }

        private void ReadStringIntoBuffer(char quote)
        {
            int num = _charPos;
            int charPos = _charPos;
            int num2 = _charPos;
            _stringBuffer.Position = 0;
            char c2;
            for (; ; )
            {
                char c = _chars[num++];
                if (c <= '\r')
                {
                    if (c != '\0')
                    {
                        if (c != '\n')
                        {
                            if (c == '\r')
                            {
                                _charPos = num - 1;
                                ProcessCarriageReturn(true);
                                num = _charPos;
                            }
                        }
                        else
                        {
                            _charPos = num - 1;
                            ProcessLineFeed();
                            num = _charPos;
                        }
                    }
                    else if (_charsUsed == num - 1)
                    {
                        num--;
                        if (ReadData(true) == 0)
                        {
                            break;
                        }
                    }
                }
                else if (c != '"' && c != '\'')
                {
                    if (c == '\\')
                    {
                        _charPos = num;
                        if (!EnsureChars(0, true))
                        {
                            goto Block_10;
                        }
                        int writeToPosition = num - 1;
                        c2 = _chars[num];
                        num++;
                        char c3;
                        if (c2 <= '\\')
                        {
                            if (c2 <= '\'')
                            {
                                if (c2 != '"' && c2 != '\'')
                                {
                                    goto Block_14;
                                }
                            }
                            else if (c2 != '/')
                            {
                                if (c2 != '\\')
                                {
                                    goto Block_16;
                                }
                                c3 = '\\';
                                goto IL_287;
                            }
                            c3 = c2;
                        }
                        else if (c2 <= 'f')
                        {
                            if (c2 != 'b')
                            {
                                if (c2 != 'f')
                                {
                                    goto Block_19;
                                }
                                c3 = '\f';
                            }
                            else
                            {
                                c3 = '\b';
                            }
                        }
                        else
                        {
                            if (c2 != 'n')
                            {
                                switch (c2)
                                {
                                    case 'r':
                                        c3 = '\r';
                                        goto IL_287;
                                    case 't':
                                        c3 = '\t';
                                        goto IL_287;
                                    case 'u':
                                        _charPos = num;
                                        c3 = ParseUnicode();
                                        if (StringUtils.IsLowSurrogate(c3))
                                        {
                                            c3 = '�';
                                        }
                                        else if (StringUtils.IsHighSurrogate(c3))
                                        {
                                            bool flag;
                                            do
                                            {
                                                flag = false;
                                                if (EnsureChars(2, true) && _chars[_charPos] == '\\' && _chars[_charPos + 1] == 'u')
                                                {
                                                    char writeChar = c3;
                                                    _charPos += 2;
                                                    c3 = ParseUnicode();
                                                    if (!StringUtils.IsLowSurrogate(c3))
                                                    {
                                                        if (StringUtils.IsHighSurrogate(c3))
                                                        {
                                                            writeChar = '�';
                                                            flag = true;
                                                        }
                                                        else
                                                        {
                                                            writeChar = '�';
                                                        }
                                                    }
                                                    EnsureBufferNotEmpty();
                                                    WriteCharToBuffer(writeChar, num2, writeToPosition);
                                                    num2 = _charPos;
                                                }
                                                else
                                                {
                                                    c3 = '�';
                                                }
                                            }
                                            while (flag);
                                        }
                                        num = _charPos;
                                        goto IL_287;
                                }
                                goto Block_21;
                            }
                            c3 = '\n';
                        }
                    IL_287:
                        EnsureBufferNotEmpty();
                        WriteCharToBuffer(c3, num2, writeToPosition);
                        num2 = num;
                    }
                }
                else if (_chars[num - 1] == quote)
                {
                    goto Block_28;
                }
            }
            _charPos = num;
            throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
        Block_10:
            throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
        Block_14:
        Block_16:
        Block_19:
        Block_21:
            _charPos = num;
            throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, "\\" + c2.ToString()));
        Block_28:
            num--;
            if (charPos == num2)
            {
                _stringReference = new StringReference(_chars, charPos, num - charPos);
            }
            else
            {
                EnsureBufferNotEmpty();
                if (num > num2)
                {
                    _stringBuffer.Append(_arrayPool, _chars, num2, num - num2);
                }
                _stringReference = new StringReference(_stringBuffer.InternalBuffer, 0, _stringBuffer.Position);
            }
            num++;
            _charPos = num;
        }

        private void WriteCharToBuffer(char writeChar, int lastWritePosition, int writeToPosition)
        {
            if (writeToPosition > lastWritePosition)
            {
                _stringBuffer.Append(_arrayPool, _chars, lastWritePosition, writeToPosition - lastWritePosition);
            }
            _stringBuffer.Append(_arrayPool, writeChar);
        }

        private char ParseUnicode()
        {
            if (EnsureChars(4, true))
            {
                char result = Convert.ToChar(ConvertUtils.HexTextToInt(_chars, _charPos, _charPos + 4));
                _charPos += 4;
                return result;
            }
            throw JsonReaderException.Create(this, "Unexpected end while parsing unicode character.");
        }

        private void ReadNumberIntoBuffer()
        {
            int num = _charPos;
            for (; ; )
            {
                char c = _chars[num];
                if (c <= 'F')
                {
                    if (c != '\0')
                    {
                        switch (c)
                        {
                            case '+':
                            case '-':
                            case '.':
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                            case 'A':
                            case 'B':
                            case 'C':
                            case 'D':
                            case 'E':
                            case 'F':
                                goto IL_E4;
                        }
                        break;
                    }
                    _charPos = num;
                    if (_charsUsed != num)
                    {
                        return;
                    }
                    if (ReadData(true) == 0)
                    {
                        return;
                    }
                    continue;
                }
                else if (c != 'X')
                {
                    switch (c)
                    {
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                            break;
                        default:
                            if (c != 'x')
                            {
                                goto Block_6;
                            }
                            break;
                    }
                }
            IL_E4:
                num++;
            }
        Block_6:
            _charPos = num;
            char c2 = _chars[_charPos];
            if (char.IsWhiteSpace(c2) || c2 == ',' || c2 == '}' || c2 == ']' || c2 == ')' || c2 == '/')
            {
                return;
            }
            throw JsonReaderException.Create(this, "Unexpected character encountered while parsing number: {0}.".FormatWith(CultureInfo.InvariantCulture, c2));
        }

        private void ClearRecentString()
        {
            _stringBuffer.Position = 0;
            _stringReference = default(StringReference);
        }

        private bool ParsePostValue()
        {
            char c;
            for (; ; )
            {
                c = _chars[_charPos];
                if (c <= ')')
                {
                    if (c <= '\r')
                    {
                        if (c != '\0')
                        {
                            switch (c)
                            {
                                case '\t':
                                    break;
                                case '\n':
                                    ProcessLineFeed();
                                    continue;
                                case '\v':
                                case '\f':
                                    goto IL_143;
                                case '\r':
                                    ProcessCarriageReturn(false);
                                    continue;
                                default:
                                    goto IL_143;
                            }
                        }
                        else
                        {
                            if (_charsUsed != _charPos)
                            {
                                _charPos++;
                                continue;
                            }
                            if (ReadData(false) == 0)
                            {
                                break;
                            }
                            continue;
                        }
                    }
                    else if (c != ' ')
                    {
                        if (c != ')')
                        {
                            goto IL_143;
                        }
                        goto IL_E2;
                    }
                    _charPos++;
                    continue;
                }
                if (c <= '/')
                {
                    if (c == ',')
                    {
                        goto IL_103;
                    }
                    if (c == '/')
                    {
                        goto IL_FA;
                    }
                }
                else
                {
                    if (c == ']')
                    {
                        goto IL_CA;
                    }
                    if (c == '}')
                    {
                        goto IL_B2;
                    }
                }
            IL_143:
                if (!char.IsWhiteSpace(c))
                {
                    goto IL_15E;
                }
                _charPos++;
            }
            _currentState = JsonReader.State.Finished;
            return false;
        IL_B2:
            _charPos++;
            base.SetToken(JsonToken.EndObject);
            return true;
        IL_CA:
            _charPos++;
            base.SetToken(JsonToken.EndArray);
            return true;
        IL_E2:
            _charPos++;
            base.SetToken(JsonToken.EndConstructor);
            return true;
        IL_FA:
            ParseComment(true);
            return true;
        IL_103:
            _charPos++;
            base.SetStateBasedOnCurrent();
            return false;
        IL_15E:
            throw JsonReaderException.Create(this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
        }

        private bool ParseObject()
        {
            for (; ; )
            {
                char c = _chars[_charPos];
                if (c <= '\r')
                {
                    if (c != '\0')
                    {
                        switch (c)
                        {
                            case '\t':
                                break;
                            case '\n':
                                ProcessLineFeed();
                                continue;
                            case '\v':
                            case '\f':
                                goto IL_BD;
                            case '\r':
                                ProcessCarriageReturn(false);
                                continue;
                            default:
                                goto IL_BD;
                        }
                    }
                    else
                    {
                        if (_charsUsed != _charPos)
                        {
                            _charPos++;
                            continue;
                        }
                        if (ReadData(false) == 0)
                        {
                            break;
                        }
                        continue;
                    }
                }
                else if (c != ' ')
                {
                    if (c == '/')
                    {
                        goto IL_8A;
                    }
                    if (c != '}')
                    {
                        goto IL_BD;
                    }
                    goto IL_72;
                }
                _charPos++;
                continue;
            IL_BD:
                if (!char.IsWhiteSpace(c))
                {
                    goto IL_D8;
                }
                _charPos++;
            }
            return false;
        IL_72:
            base.SetToken(JsonToken.EndObject);
            _charPos++;
            return true;
        IL_8A:
            ParseComment(true);
            return true;
        IL_D8:
            return ParseProperty();
        }

        private bool ParseProperty()
        {
            char c = _chars[_charPos];
            char c2;
            if (c == '"' || c == '\'')
            {
                _charPos++;
                c2 = c;
                ShiftBufferIfNeeded();
                ReadStringIntoBuffer(c2);
            }
            else
            {
                if (!ValidIdentifierChar(c))
                {
                    throw JsonReaderException.Create(this, "Invalid property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
                }
                c2 = '\0';
                ShiftBufferIfNeeded();
                ParseUnquotedProperty();
            }
            string text;
            if (NameTable != null)
            {
                text = NameTable.Get(_stringReference.Chars, _stringReference.StartIndex, _stringReference.Length);
                if (text == null)
                {
                    text = _stringReference.ToString();
                }
            }
            else
            {
                text = _stringReference.ToString();
            }
            EatWhitespace(false);
            if (_chars[_charPos] != ':')
            {
                throw JsonReaderException.Create(this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
            }
            _charPos++;
            base.SetToken(JsonToken.PropertyName, text);
            _quoteChar = c2;
            ClearRecentString();
            return true;
        }

        private bool ValidIdentifierChar(char value)
        {
            return char.IsLetterOrDigit(value) || value == '_' || value == '$';
        }

        private void ParseUnquotedProperty()
        {
            int charPos = _charPos;
            char c;
            for (; ; )
            {
                if (_chars[_charPos] == '\0')
                {
                    if (_charsUsed != _charPos)
                    {
                        goto IL_3B;
                    }
                    if (ReadData(true) == 0)
                    {
                        break;
                    }
                }
                else
                {
                    c = _chars[_charPos];
                    if (!ValidIdentifierChar(c))
                    {
                        goto IL_7D;
                    }
                    _charPos++;
                }
            }
            throw JsonReaderException.Create(this, "Unexpected end while parsing unquoted property name.");
        IL_3B:
            _stringReference = new StringReference(_chars, charPos, _charPos - charPos);
            return;
        IL_7D:
            if (char.IsWhiteSpace(c) || c == ':')
            {
                _stringReference = new StringReference(_chars, charPos, _charPos - charPos);
                return;
            }
            throw JsonReaderException.Create(this, "Invalid JavaScript property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
        }

        private bool ParseValue()
        {
            char c;
            for (; ; )
            {
                c = _chars[_charPos];
                if (c <= 'N')
                {
                    if (c <= ' ')
                    {
                        if (c != '\0')
                        {
                            switch (c)
                            {
                                case '\t':
                                    break;
                                case '\n':
                                    ProcessLineFeed();
                                    continue;
                                case '\v':
                                case '\f':
                                    goto IL_276;
                                case '\r':
                                    ProcessCarriageReturn(false);
                                    continue;
                                default:
                                    if (c != ' ')
                                    {
                                        goto IL_276;
                                    }
                                    break;
                            }
                            _charPos++;
                            continue;
                        }
                        if (_charsUsed != _charPos)
                        {
                            _charPos++;
                            continue;
                        }
                        if (ReadData(false) == 0)
                        {
                            break;
                        }
                        continue;
                    }
                    else if (c <= '/')
                    {
                        if (c == '"')
                        {
                            goto IL_116;
                        }
                        switch (c)
                        {
                            case '\'':
                                goto IL_116;
                            case ')':
                                goto IL_234;
                            case ',':
                                goto IL_22A;
                            case '-':
                                goto IL_1A3;
                            case '/':
                                goto IL_1D3;
                        }
                    }
                    else
                    {
                        if (c == 'I')
                        {
                            goto IL_199;
                        }
                        if (c == 'N')
                        {
                            goto IL_18F;
                        }
                    }
                }
                else if (c <= 'f')
                {
                    if (c == '[')
                    {
                        goto IL_1FB;
                    }
                    if (c == ']')
                    {
                        goto IL_212;
                    }
                    if (c == 'f')
                    {
                        goto IL_128;
                    }
                }
                else if (c <= 't')
                {
                    if (c == 'n')
                    {
                        goto IL_130;
                    }
                    if (c == 't')
                    {
                        goto IL_120;
                    }
                }
                else
                {
                    if (c == 'u')
                    {
                        goto IL_1DC;
                    }
                    if (c == '{')
                    {
                        goto IL_1E4;
                    }
                }
            IL_276:
                if (!char.IsWhiteSpace(c))
                {
                    goto IL_291;
                }
                _charPos++;
            }
            return false;
        IL_116:
            ParseString(c, ReadType.Read);
            return true;
        IL_120:
            ParseTrue();
            return true;
        IL_128:
            ParseFalse();
            return true;
        IL_130:
            if (EnsureChars(1, true))
            {
                char c2 = _chars[_charPos + 1];
                if (c2 == 'u')
                {
                    ParseNull();
                }
                else
                {
                    if (c2 != 'e')
                    {
                        throw CreateUnexpectedCharacterException(_chars[_charPos]);
                    }
                    ParseConstructor();
                }
                return true;
            }
            _charPos++;
            throw base.CreateUnexpectedEndException();
        IL_18F:
            ParseNumberNaN(ReadType.Read);
            return true;
        IL_199:
            ParseNumberPositiveInfinity(ReadType.Read);
            return true;
        IL_1A3:
            if (EnsureChars(1, true) && _chars[_charPos + 1] == 'I')
            {
                ParseNumberNegativeInfinity(ReadType.Read);
            }
            else
            {
                ParseNumber(ReadType.Read);
            }
            return true;
        IL_1D3:
            ParseComment(true);
            return true;
        IL_1DC:
            ParseUndefined();
            return true;
        IL_1E4:
            _charPos++;
            base.SetToken(JsonToken.StartObject);
            return true;
        IL_1FB:
            _charPos++;
            base.SetToken(JsonToken.StartArray);
            return true;
        IL_212:
            _charPos++;
            base.SetToken(JsonToken.EndArray);
            return true;
        IL_22A:
            base.SetToken(JsonToken.Undefined);
            return true;
        IL_234:
            _charPos++;
            base.SetToken(JsonToken.EndConstructor);
            return true;
        IL_291:
            if (char.IsNumber(c) || c == '-' || c == '.')
            {
                ParseNumber(ReadType.Read);
                return true;
            }
            throw CreateUnexpectedCharacterException(c);
        }

        private void ProcessLineFeed()
        {
            _charPos++;
            OnNewLine(_charPos);
        }

        private void ProcessCarriageReturn(bool append)
        {
            _charPos++;
            if (EnsureChars(1, append) && _chars[_charPos] == '\n')
            {
                _charPos++;
            }
            OnNewLine(_charPos);
        }

        private bool EatWhitespace(bool oneOrMore)
        {
            bool flag = false;
            bool flag2 = false;
            while (!flag)
            {
                char c = _chars[_charPos];
                if (c != '\0')
                {
                    if (c != '\n')
                    {
                        if (c != '\r')
                        {
                            if (c == ' ' || char.IsWhiteSpace(c))
                            {
                                flag2 = true;
                                _charPos++;
                            }
                            else
                            {
                                flag = true;
                            }
                        }
                        else
                        {
                            ProcessCarriageReturn(false);
                        }
                    }
                    else
                    {
                        ProcessLineFeed();
                    }
                }
                else if (_charsUsed == _charPos)
                {
                    if (ReadData(false) == 0)
                    {
                        flag = true;
                    }
                }
                else
                {
                    _charPos++;
                }
            }
            return !oneOrMore || flag2;
        }

        private void ParseConstructor()
        {
            if (!MatchValueWithTrailingSeparator("new"))
            {
                throw JsonReaderException.Create(this, "Unexpected content while parsing JSON.");
            }
            EatWhitespace(false);
            int charPos = _charPos;
            char c;
            for (; ; )
            {
                c = _chars[_charPos];
                if (c == '\0')
                {
                    if (_charsUsed != _charPos)
                    {
                        goto IL_53;
                    }
                    if (ReadData(true) == 0)
                    {
                        break;
                    }
                }
                else
                {
                    if (!char.IsLetterOrDigit(c))
                    {
                        goto IL_85;
                    }
                    _charPos++;
                }
            }
            throw JsonReaderException.Create(this, "Unexpected end while parsing constructor.");
        IL_53:
            int charPos2 = _charPos;
            _charPos++;
            goto IL_F7;
        IL_85:
            if (c == '\r')
            {
                charPos2 = _charPos;
                ProcessCarriageReturn(true);
            }
            else if (c == '\n')
            {
                charPos2 = _charPos;
                ProcessLineFeed();
            }
            else if (char.IsWhiteSpace(c))
            {
                charPos2 = _charPos;
                _charPos++;
            }
            else
            {
                if (c != '(')
                {
                    throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
                }
                charPos2 = _charPos;
            }
        IL_F7:
            _stringReference = new StringReference(_chars, charPos, charPos2 - charPos);
            string value = _stringReference.ToString();
            EatWhitespace(false);
            if (_chars[_charPos] != '(')
            {
                throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
            }
            _charPos++;
            ClearRecentString();
            base.SetToken(JsonToken.StartConstructor, value);
        }

        private void ParseNumber(ReadType readType)
        {
            ShiftBufferIfNeeded();
            char c = _chars[_charPos];
            int charPos = _charPos;
            ReadNumberIntoBuffer();
            base.SetPostValueState(true);
            _stringReference = new StringReference(_chars, charPos, _charPos - charPos);
            bool flag = char.IsDigit(c) && _stringReference.Length == 1;
            bool flag2 = c == '0' && _stringReference.Length > 1 && _stringReference.Chars[_stringReference.StartIndex + 1] != '.' && _stringReference.Chars[_stringReference.StartIndex + 1] != 'e' && _stringReference.Chars[_stringReference.StartIndex + 1] != 'E';
            JsonToken newToken;
            object value;
            if (readType == ReadType.ReadAsString)
            {
                string text = _stringReference.ToString();
                if (flag2)
                {
                    try
                    {
                        if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            Convert.ToInt64(text, 16);
                        }
                        else
                        {
                            Convert.ToInt64(text, 8);
                        }
                        goto IL_16B;
                    }
                    catch (Exception ex)
                    {
                        throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text), ex);
                    }
                }
                if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
                {
                    throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, _stringReference.ToString()));
                }
            IL_16B:
                newToken = JsonToken.String;
                value = text;
            }
            else if (readType == ReadType.ReadAsInt32)
            {
                if (flag)
                {
                    value = c - '0';
                }
                else
                {
                    if (flag2)
                    {
                        string text2 = _stringReference.ToString();
                        try
                        {
                            value = (text2.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(text2, 16) : Convert.ToInt32(text2, 8));
                            goto IL_27C;
                        }
                        catch (Exception ex2)
                        {
                            throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, text2), ex2);
                        }
                    }
                    ParseResult parseResult = ConvertUtils.Int32TryParse(_stringReference.Chars, _stringReference.StartIndex, _stringReference.Length, out int num2);
                    if (parseResult == ParseResult.Success)
                    {
                        value = num2;
                    }
                    else
                    {
                        if (parseResult == ParseResult.Overflow)
                        {
                            throw JsonReaderException.Create(this, "JSON integer {0} is too large or small for an Int32.".FormatWith(CultureInfo.InvariantCulture, _stringReference.ToString()));
                        }
                        throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, _stringReference.ToString()));
                    }
                }
            IL_27C:
                newToken = JsonToken.Integer;
            }
            else if (readType == ReadType.ReadAsDecimal)
            {
                if (flag)
                {
                    value = c - 48m;
                }
                else
                {
                    if (flag2)
                    {
                        string text3 = _stringReference.ToString();
                        try
                        {
                            value = Convert.ToDecimal(text3.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text3, 16) : Convert.ToInt64(text3, 8));
                            goto IL_362;
                        }
                        catch (Exception ex3)
                        {
                            throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, text3), ex3);
                        }
                    }
                    if (!decimal.TryParse(_stringReference.ToString(), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out decimal num3))
                    {
                        throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, _stringReference.ToString()));
                    }
                    value = num3;
                }
            IL_362:
                newToken = JsonToken.Float;
            }
            else if (readType == ReadType.ReadAsDouble)
            {
                if (flag)
                {
                    value = c - 48.0;
                }
                else
                {
                    if (flag2)
                    {
                        string text4 = _stringReference.ToString();
                        try
                        {
                            value = Convert.ToDouble(text4.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text4, 16) : Convert.ToInt64(text4, 8));
                            goto IL_442;
                        }
                        catch (Exception ex4)
                        {
                            throw JsonReaderException.Create(this, "Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, text4), ex4);
                        }
                    }
                    if (!double.TryParse(_stringReference.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double num4))
                    {
                        throw JsonReaderException.Create(this, "Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, _stringReference.ToString()));
                    }
                    value = num4;
                }
            IL_442:
                newToken = JsonToken.Float;
            }
            else if (flag)
            {
                value = (long)(c - 48UL);
                newToken = JsonToken.Integer;
            }
            else if (flag2)
            {
                string text5 = _stringReference.ToString();
                try
                {
                    value = (text5.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text5, 16) : Convert.ToInt64(text5, 8));
                }
                catch (Exception ex5)
                {
                    throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text5), ex5);
                }
                newToken = JsonToken.Integer;
            }
            else
            {
                ParseResult parseResult2 = ConvertUtils.Int64TryParse(_stringReference.Chars, _stringReference.StartIndex, _stringReference.Length, out long num5);
                if (parseResult2 == ParseResult.Success)
                {
                    value = num5;
                    newToken = JsonToken.Integer;
                }
                else
                {
                    if (parseResult2 == ParseResult.Overflow)
                    {
                        throw JsonReaderException.Create(this, "JSON integer {0} is too large or small for an Int64.".FormatWith(CultureInfo.InvariantCulture, _stringReference.ToString()));
                    }
                    string text6 = _stringReference.ToString();
                    if (_floatParseHandling == FloatParseHandling.Decimal)
                    {
                        if (!decimal.TryParse(text6, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out decimal num6))
                        {
                            throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, text6));
                        }
                        value = num6;
                    }
                    else
                    {
                        if (!double.TryParse(text6, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out double num7))
                        {
                            throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text6));
                        }
                        value = num7;
                    }
                    newToken = JsonToken.Float;
                }
            }
            ClearRecentString();
            base.SetToken(newToken, value, false);
        }

        private void ParseComment(bool setToken)
        {
            _charPos++;
            if (!EnsureChars(1, false))
            {
                throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
            }
            bool flag;
            if (_chars[_charPos] == '*')
            {
                flag = false;
            }
            else
            {
                if (_chars[_charPos] != '/')
                {
                    throw JsonReaderException.Create(this, "Error parsing comment. Expected: *, got {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
                }
                flag = true;
            }
            _charPos++;
            int charPos = _charPos;
            for (; ; )
            {
                char c = _chars[_charPos];
                if (c <= '\n')
                {
                    if (c != '\0')
                    {
                        if (c == '\n')
                        {
                            if (flag)
                            {
                                goto Block_16;
                            }
                            ProcessLineFeed();
                            continue;
                        }
                    }
                    else
                    {
                        if (_charsUsed != _charPos)
                        {
                            _charPos++;
                            continue;
                        }
                        if (ReadData(true) == 0)
                        {
                            break;
                        }
                        continue;
                    }
                }
                else if (c != '\r')
                {
                    if (c == '*')
                    {
                        _charPos++;
                        if (!flag && EnsureChars(0, true) && _chars[_charPos] == '/')
                        {
                            goto Block_14;
                        }
                        continue;
                    }
                }
                else
                {
                    if (flag)
                    {
                        goto Block_15;
                    }
                    ProcessCarriageReturn(true);
                    continue;
                }
                _charPos++;
            }
            if (!flag)
            {
                throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
            }
            EndComment(setToken, charPos, _charPos);
            return;
        Block_14:
            EndComment(setToken, charPos, _charPos - 1);
            _charPos++;
            return;
        Block_15:
            EndComment(setToken, charPos, _charPos);
            return;
        Block_16:
            EndComment(setToken, charPos, _charPos);
        }

        private void EndComment(bool setToken, int initialPosition, int endPosition)
        {
            if (setToken)
            {
                base.SetToken(JsonToken.Comment, new string(_chars, initialPosition, endPosition - initialPosition));
            }
        }

        private bool MatchValue(string value)
        {
            if (!EnsureChars(value.Length - 1, true))
            {
                _charPos = _charsUsed;
                throw base.CreateUnexpectedEndException();
            }
            for (int i = 0; i < value.Length; i++)
            {
                if (_chars[_charPos + i] != value[i])
                {
                    _charPos += i;
                    return false;
                }
            }
            _charPos += value.Length;
            return true;
        }

        private bool MatchValueWithTrailingSeparator(string value)
        {
            return MatchValue(value) && (!EnsureChars(0, false) || IsSeparator(_chars[_charPos]) || _chars[_charPos] == '\0');
        }

        private bool IsSeparator(char c)
        {
            if (c <= ')')
            {
                switch (c)
                {
                    case '\t':
                    case '\n':
                    case '\r':
                        break;
                    case '\v':
                    case '\f':
                        goto IL_8C;
                    default:
                        if (c != ' ')
                        {
                            if (c != ')')
                            {
                                goto IL_8C;
                            }
                            if (base.CurrentState == JsonReader.State.Constructor || base.CurrentState == JsonReader.State.ConstructorStart)
                            {
                                return true;
                            }
                            return false;
                        }
                        break;
                }
                return true;
            }
            if (c <= '/')
            {
                if (c != ',')
                {
                    if (c != '/')
                    {
                        goto IL_8C;
                    }
                    if (!EnsureChars(1, false))
                    {
                        return false;
                    }
                    char c2 = _chars[_charPos + 1];
                    return c2 == '*' || c2 == '/';
                }
            }
            else if (c != ']' && c != '}')
            {
                goto IL_8C;
            }
            return true;
        IL_8C:
            if (char.IsWhiteSpace(c))
            {
                return true;
            }
            return false;
        }

        private void ParseTrue()
        {
            if (MatchValueWithTrailingSeparator(JsonConvert.True))
            {
                base.SetToken(JsonToken.Boolean, true);
                return;
            }
            throw JsonReaderException.Create(this, "Error parsing boolean value.");
        }

        private void ParseNull()
        {
            if (MatchValueWithTrailingSeparator(JsonConvert.Null))
            {
                base.SetToken(JsonToken.Null);
                return;
            }
            throw JsonReaderException.Create(this, "Error parsing null value.");
        }

        private void ParseUndefined()
        {
            if (MatchValueWithTrailingSeparator(JsonConvert.Undefined))
            {
                base.SetToken(JsonToken.Undefined);
                return;
            }
            throw JsonReaderException.Create(this, "Error parsing undefined value.");
        }

        private void ParseFalse()
        {
            if (MatchValueWithTrailingSeparator(JsonConvert.False))
            {
                base.SetToken(JsonToken.Boolean, false);
                return;
            }
            throw JsonReaderException.Create(this, "Error parsing boolean value.");
        }

        private object ParseNumberNegativeInfinity(ReadType readType)
        {
            if (MatchValueWithTrailingSeparator(JsonConvert.NegativeInfinity))
            {
                if (readType != ReadType.Read)
                {
                    if (readType == ReadType.ReadAsString)
                    {
                        base.SetToken(JsonToken.String, JsonConvert.NegativeInfinity);
                        return JsonConvert.NegativeInfinity;
                    }
                    if (readType != ReadType.ReadAsDouble)
                    {
                        goto IL_57;
                    }
                }
                if (_floatParseHandling == FloatParseHandling.Double)
                {
                    base.SetToken(JsonToken.Float, double.NegativeInfinity);
                    return double.NegativeInfinity;
                }
            IL_57:
                throw JsonReaderException.Create(this, "Cannot read -Infinity value.");
            }
            throw JsonReaderException.Create(this, "Error parsing -Infinity value.");
        }

        private object ParseNumberPositiveInfinity(ReadType readType)
        {
            if (MatchValueWithTrailingSeparator(JsonConvert.PositiveInfinity))
            {
                if (readType != ReadType.Read)
                {
                    if (readType == ReadType.ReadAsString)
                    {
                        base.SetToken(JsonToken.String, JsonConvert.PositiveInfinity);
                        return JsonConvert.PositiveInfinity;
                    }
                    if (readType != ReadType.ReadAsDouble)
                    {
                        goto IL_57;
                    }
                }
                if (_floatParseHandling == FloatParseHandling.Double)
                {
                    base.SetToken(JsonToken.Float, double.PositiveInfinity);
                    return double.PositiveInfinity;
                }
            IL_57:
                throw JsonReaderException.Create(this, "Cannot read Infinity value.");
            }
            throw JsonReaderException.Create(this, "Error parsing Infinity value.");
        }

        private object ParseNumberNaN(ReadType readType)
        {
            if (MatchValueWithTrailingSeparator(JsonConvert.NaN))
            {
                if (readType != ReadType.Read)
                {
                    if (readType == ReadType.ReadAsString)
                    {
                        base.SetToken(JsonToken.String, JsonConvert.NaN);
                        return JsonConvert.NaN;
                    }
                    if (readType != ReadType.ReadAsDouble)
                    {
                        goto IL_57;
                    }
                }
                if (_floatParseHandling == FloatParseHandling.Double)
                {
                    base.SetToken(JsonToken.Float, double.NaN);
                    return double.NaN;
                }
            IL_57:
                throw JsonReaderException.Create(this, "Cannot read NaN value.");
            }
            throw JsonReaderException.Create(this, "Error parsing NaN value.");
        }

        /// <summary>
        /// Changes the reader's state to <see cref="F:Newtonsoft.Json.JsonReader.State.Closed" />.
        /// If <see cref="P:Newtonsoft.Json.JsonReader.CloseInput" /> is set to <c>true</c>, the underlying <see cref="T:System.IO.TextReader" /> is also closed.
        /// </summary>
        public override void Close()
        {
            base.Close();
            if (_chars != null)
            {
                BufferUtils.ReturnBuffer(_arrayPool, _chars);
                _chars = null;
            }
            if (base.CloseInput && _reader != null)
            {
                _reader.Close();
            }
            _stringBuffer.Clear(_arrayPool);
        }

        /// <summary>
        /// Gets a value indicating whether the class can return line information.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if <see cref="P:Newtonsoft.Json.JsonTextReader.LineNumber" /> and <see cref="P:Newtonsoft.Json.JsonTextReader.LinePosition" /> can be provided; otherwise, <c>false</c>.
        /// </returns>
        public bool HasLineInfo()
        {
            return true;
        }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        /// <value>
        /// The current line number or 0 if no line information is available (for example, <see cref="M:Newtonsoft.Json.JsonTextReader.HasLineInfo" /> returns <c>false</c>).
        /// </value>
        public int LineNumber
        {
            get
            {
                if (base.CurrentState == JsonReader.State.Start && LinePosition == 0 && TokenType != JsonToken.Comment)
                {
                    return 0;
                }
                return _lineNumber;
            }
        }

        /// <summary>
        /// Gets the current line position.
        /// </summary>
        /// <value>
        /// The current line position or 0 if no line information is available (for example, <see cref="M:Newtonsoft.Json.JsonTextReader.HasLineInfo" /> returns <c>false</c>).
        /// </value>
        public int LinePosition
        {
            get
            {
                return _charPos - _lineStartPos;
            }
        }

        private const char UnicodeReplacementChar = '�';

        private const int MaximumJavascriptIntegerCharacterLength = 380;

        private readonly TextReader _reader;

        private char[] _chars;

        private int _charsUsed;

        private int _charPos;

        private int _lineStartPos;

        private int _lineNumber;

        private bool _isEndOfFile;

        private StringBuffer _stringBuffer;

        private StringReference _stringReference;

        private IArrayPool<char> _arrayPool;

        internal PropertyNameTable NameTable;
    }
}
