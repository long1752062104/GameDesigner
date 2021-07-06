using Newtonsoft_X.Json.Utilities;
using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft_X.Json
{
    /// <summary>
    /// Represents a writer that provides a fast, non-cached, forward-only way of generating JSON data.
    /// </summary>
    public class JsonTextWriter : JsonWriter
    {
        private Base64Encoder Base64Encoder
        {
            get
            {
                if (_base64Encoder == null)
                {
                    _base64Encoder = new Base64Encoder(_writer);
                }
                return _base64Encoder;
            }
        }

        /// <summary>
        /// Gets or sets the writer's character array pool.
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

        /// <summary>
        /// Gets or sets how many <see cref="P:Newtonsoft.Json.JsonTextWriter.IndentChar" />s to write for each level in the hierarchy when <see cref="P:Newtonsoft.Json.JsonWriter.Formatting" /> is set to <see cref="F:Newtonsoft.Json.Formatting.Indented" />.
        /// </summary>
        public int Indentation
        {
            get
            {
                return _indentation;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Indentation value must be greater than 0.");
                }
                _indentation = value;
            }
        }

        /// <summary>
        /// Gets or sets which character to use to quote attribute values.
        /// </summary>
        public char QuoteChar
        {
            get
            {
                return _quoteChar;
            }
            set
            {
                if (value != '"' && value != '\'')
                {
                    throw new ArgumentException("Invalid JavaScript string quote character. Valid quote characters are ' and \".");
                }
                _quoteChar = value;
                UpdateCharEscapeFlags();
            }
        }

        /// <summary>
        /// Gets or sets which character to use for indenting when <see cref="P:Newtonsoft.Json.JsonWriter.Formatting" /> is set to <see cref="F:Newtonsoft.Json.Formatting.Indented" />.
        /// </summary>
        public char IndentChar
        {
            get
            {
                return _indentChar;
            }
            set
            {
                if (value != _indentChar)
                {
                    _indentChar = value;
                    _indentChars = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether object names will be surrounded with quotes.
        /// </summary>
        public bool QuoteName
        {
            get
            {
                return _quoteName;
            }
            set
            {
                _quoteName = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonTextWriter" /> class using the specified <see cref="T:System.IO.TextWriter" />.
        /// </summary>
        /// <param name="textWriter">The <see cref="T:System.IO.TextWriter" /> to write to.</param>
        public JsonTextWriter(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException("textWriter");
            }
            _writer = textWriter;
            _quoteChar = '"';
            _quoteName = true;
            _indentChar = ' ';
            _indentation = 2;
            UpdateCharEscapeFlags();
        }

        /// <summary>
        /// Flushes whatever is in the buffer to the underlying <see cref="T:System.IO.TextWriter" /> and also flushes the underlying <see cref="T:System.IO.TextWriter" />.
        /// </summary>
        public override void Flush()
        {
            _writer.Flush();
        }

        /// <summary>
        /// Closes this writer.
        /// If <see cref="P:Newtonsoft.Json.JsonWriter.CloseOutput" /> is set to <c>true</c>, the underlying <see cref="T:System.IO.TextWriter" /> is also closed.
        /// If <see cref="P:Newtonsoft.Json.JsonWriter.AutoCompleteOnClose" /> is set to <c>true</c>, the JSON is auto-completed.
        /// </summary>
        public override void Close()
        {
            base.Close();
            if (_writeBuffer != null)
            {
                BufferUtils.ReturnBuffer(_arrayPool, _writeBuffer);
                _writeBuffer = null;
            }
            if (base.CloseOutput && _writer != null)
            {
                _writer.Close();
            }
        }

        /// <summary>
        /// Writes the beginning of a JSON object.
        /// </summary>
        public override void WriteStartObject()
        {
            base.InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);
            _writer.Write('{');
        }

        /// <summary>
        /// Writes the beginning of a JSON array.
        /// </summary>
        public override void WriteStartArray()
        {
            base.InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);
            _writer.Write('[');
        }

        /// <summary>
        /// Writes the start of a constructor with the given name.
        /// </summary>
        /// <param name="name">The name of the constructor.</param>
        public override void WriteStartConstructor(string name)
        {
            base.InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);
            _writer.Write("new ");
            _writer.Write(name);
            _writer.Write('(');
        }

        /// <summary>
        /// Writes the specified end token.
        /// </summary>
        /// <param name="token">The end token to write.</param>
        protected override void WriteEnd(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.EndObject:
                    _writer.Write('}');
                    return;
                case JsonToken.EndArray:
                    _writer.Write(']');
                    return;
                case JsonToken.EndConstructor:
                    _writer.Write(')');
                    return;
                default:
                    throw JsonWriterException.Create(this, "Invalid JsonToken: " + token, null);
            }
        }

        /// <summary>
        /// Writes the property name of a name/value pair on a JSON object.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        public override void WritePropertyName(string name)
        {
            base.InternalWritePropertyName(name);
            WriteEscapedString(name, _quoteName);
            _writer.Write(':');
        }

        /// <summary>
        /// Writes the property name of a name/value pair on a JSON object.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="escape">A flag to indicate whether the text should be escaped when it is written as a JSON property name.</param>
        public override void WritePropertyName(string name, bool escape)
        {
            base.InternalWritePropertyName(name);
            if (escape)
            {
                WriteEscapedString(name, _quoteName);
            }
            else
            {
                if (_quoteName)
                {
                    _writer.Write(_quoteChar);
                }
                _writer.Write(name);
                if (_quoteName)
                {
                    _writer.Write(_quoteChar);
                }
            }
            _writer.Write(':');
        }

        internal override void OnStringEscapeHandlingChanged()
        {
            UpdateCharEscapeFlags();
        }

        private void UpdateCharEscapeFlags()
        {
            _charEscapeFlags = JavaScriptUtils.GetCharEscapeFlags(base.StringEscapeHandling, _quoteChar);
        }

        /// <summary>
        /// Writes indent characters.
        /// </summary>
        protected override void WriteIndent()
        {
            _writer.WriteLine();
            int i = base.Top * _indentation;
            if (i > 0)
            {
                if (_indentChars == null)
                {
                    _indentChars = new string(_indentChar, 10).ToCharArray();
                }
                while (i > 0)
                {
                    int num = Math.Min(i, 10);
                    _writer.Write(_indentChars, 0, num);
                    i -= num;
                }
            }
        }

        /// <summary>
        /// Writes the JSON value delimiter.
        /// </summary>
        protected override void WriteValueDelimiter()
        {
            _writer.Write(',');
        }

        /// <summary>
        /// Writes an indent space.
        /// </summary>
        protected override void WriteIndentSpace()
        {
            _writer.Write(' ');
        }

        private void WriteValueInternal(string value, JsonToken token)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Object" /> value.
        /// An error will raised if the value cannot be written as a single JSON token.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object" /> value to write.</param>
        public override void WriteValue(object value)
        {
            base.WriteValue(value);
        }

        /// <summary>
        /// Writes a null value.
        /// </summary>
        public override void WriteNull()
        {
            base.InternalWriteValue(JsonToken.Null);
            WriteValueInternal(JsonConvert.Null, JsonToken.Null);
        }

        /// <summary>
        /// Writes an undefined value.
        /// </summary>
        public override void WriteUndefined()
        {
            base.InternalWriteValue(JsonToken.Undefined);
            WriteValueInternal(JsonConvert.Undefined, JsonToken.Undefined);
        }

        /// <summary>
        /// Writes raw JSON.
        /// </summary>
        /// <param name="json">The raw JSON to write.</param>
        public override void WriteRaw(string json)
        {
            base.InternalWriteRaw();
            _writer.Write(json);
        }

        /// <summary>
        /// Writes a <see cref="T:System.String" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.String" /> value to write.</param>
        public override void WriteValue(string value)
        {
            base.InternalWriteValue(JsonToken.String);
            if (value == null)
            {
                WriteValueInternal(JsonConvert.Null, JsonToken.Null);
                return;
            }
            WriteEscapedString(value, true);
        }

        private void WriteEscapedString(string value, bool quote)
        {
            EnsureWriteBuffer();
            JavaScriptUtils.WriteEscapedJavaScriptString(_writer, value, _quoteChar, quote, _charEscapeFlags, base.StringEscapeHandling, _arrayPool, ref _writeBuffer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Int32" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Int32" /> value to write.</param>
        public override void WriteValue(int value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        /// <summary>
        /// Writes a <see cref="T:System.UInt32" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.UInt32" /> value to write.</param>
        //[CLSCompliant(false)]
        public override void WriteValue(uint value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue((long)((ulong)value));
        }

        /// <summary>
        /// Writes a <see cref="T:System.Int64" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Int64" /> value to write.</param>
        public override void WriteValue(long value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        /// <summary>
        /// Writes a <see cref="T:System.UInt64" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.UInt64" /> value to write.</param>
        //[CLSCompliant(false)]
        public override void WriteValue(ulong value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Single" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Single" /> value to write.</param>
        public override void WriteValue(float value)
        {
            base.InternalWriteValue(JsonToken.Float);
            WriteValueInternal(JsonConvert.ToString(value, base.FloatFormatHandling, QuoteChar, false), JsonToken.Float);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" /> value to write.</param>
        public override void WriteValue(float? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            base.InternalWriteValue(JsonToken.Float);
            WriteValueInternal(JsonConvert.ToString(value.GetValueOrDefault(), base.FloatFormatHandling, QuoteChar, true), JsonToken.Float);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Double" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Double" /> value to write.</param>
        public override void WriteValue(double value)
        {
            base.InternalWriteValue(JsonToken.Float);
            WriteValueInternal(JsonConvert.ToString(value, base.FloatFormatHandling, QuoteChar, false), JsonToken.Float);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" /> value to write.</param>
        public override void WriteValue(double? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            base.InternalWriteValue(JsonToken.Float);
            WriteValueInternal(JsonConvert.ToString(value.GetValueOrDefault(), base.FloatFormatHandling, QuoteChar, true), JsonToken.Float);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Boolean" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Boolean" /> value to write.</param>
        public override void WriteValue(bool value)
        {
            base.InternalWriteValue(JsonToken.Boolean);
            WriteValueInternal(JsonConvert.ToString(value), JsonToken.Boolean);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Int16" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Int16" /> value to write.</param>
        public override void WriteValue(short value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        /// <summary>
        /// Writes a <see cref="T:System.UInt16" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.UInt16" /> value to write.</param>
        //[CLSCompliant(false)]
        public override void WriteValue(ushort value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue((long)((ulong)value));
        }

        /// <summary>
        /// Writes a <see cref="T:System.Char" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Char" /> value to write.</param>
        public override void WriteValue(char value)
        {
            base.InternalWriteValue(JsonToken.String);
            WriteValueInternal(JsonConvert.ToString(value), JsonToken.String);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Byte" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Byte" /> value to write.</param>
        public override void WriteValue(byte value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue((long)((ulong)value));
        }

        /// <summary>
        /// Writes a <see cref="T:System.SByte" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.SByte" /> value to write.</param>
        //[CLSCompliant(false)]
        public override void WriteValue(sbyte value)
        {
            base.InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Decimal" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Decimal" /> value to write.</param>
        public override void WriteValue(decimal value)
        {
            base.InternalWriteValue(JsonToken.Float);
            WriteValueInternal(JsonConvert.ToString(value), JsonToken.Float);
        }

        /// <summary>
        /// Writes a <see cref="T:System.DateTime" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.DateTime" /> value to write.</param>
        public override void WriteValue(DateTime value)
        {
            base.InternalWriteValue(JsonToken.Date);
            value = DateTimeUtils.EnsureDateTime(value, base.DateTimeZoneHandling);
            if (string.IsNullOrEmpty(base.DateFormatString))
            {
                EnsureWriteBuffer();
                int num = 0;
                _writeBuffer[num++] = _quoteChar;
                num = DateTimeUtils.WriteDateTimeString(_writeBuffer, num, value, null, value.Kind, base.DateFormatHandling);
                _writeBuffer[num++] = _quoteChar;
                _writer.Write(_writeBuffer, 0, num);
                return;
            }
            _writer.Write(_quoteChar);
            _writer.Write(value.ToString(base.DateFormatString, base.Culture));
            _writer.Write(_quoteChar);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Byte" />[] value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Byte" />[] value to write.</param>
        public override void WriteValue(byte[] value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            base.InternalWriteValue(JsonToken.Bytes);
            _writer.Write(_quoteChar);
            Base64Encoder.Encode(value, 0, value.Length);
            Base64Encoder.Flush();
            _writer.Write(_quoteChar);
        }

        /// <summary>
        /// Writes a <see cref="T:System.DateTimeOffset" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.DateTimeOffset" /> value to write.</param>
        public override void WriteValue(DateTimeOffset value)
        {
            base.InternalWriteValue(JsonToken.Date);
            if (string.IsNullOrEmpty(base.DateFormatString))
            {
                EnsureWriteBuffer();
                int num = 0;
                _writeBuffer[num++] = _quoteChar;
                num = DateTimeUtils.WriteDateTimeString(_writeBuffer, num, (base.DateFormatHandling == DateFormatHandling.IsoDateFormat) ? value.DateTime : value.UtcDateTime, new TimeSpan?(value.Offset), DateTimeKind.Local, base.DateFormatHandling);
                _writeBuffer[num++] = _quoteChar;
                _writer.Write(_writeBuffer, 0, num);
                return;
            }
            _writer.Write(_quoteChar);
            _writer.Write(value.ToString(base.DateFormatString, base.Culture));
            _writer.Write(_quoteChar);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Guid" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Guid" /> value to write.</param>
        public override void WriteValue(Guid value)
        {
            base.InternalWriteValue(JsonToken.String);
            string value2 = value.ToString("D", CultureInfo.InvariantCulture);
            _writer.Write(_quoteChar);
            _writer.Write(value2);
            _writer.Write(_quoteChar);
        }

        /// <summary>
        /// Writes a <see cref="T:System.TimeSpan" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.TimeSpan" /> value to write.</param>
        public override void WriteValue(TimeSpan value)
        {
            base.InternalWriteValue(JsonToken.String);
            string value2 = value.ToString();
            _writer.Write(_quoteChar);
            _writer.Write(value2);
            _writer.Write(_quoteChar);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Uri" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Uri" /> value to write.</param>
        public override void WriteValue(Uri value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            base.InternalWriteValue(JsonToken.String);
            WriteEscapedString(value.OriginalString, true);
        }

        /// <summary>
        /// Writes a comment <c>/*...*/</c> containing the specified text. 
        /// </summary>
        /// <param name="text">Text to place inside the comment.</param>
        public override void WriteComment(string text)
        {
            base.InternalWriteComment();
            _writer.Write("/*");
            _writer.Write(text);
            _writer.Write("*/");
        }

        /// <summary>
        /// Writes the given white space.
        /// </summary>
        /// <param name="ws">The string of white space characters.</param>
        public override void WriteWhitespace(string ws)
        {
            base.InternalWriteWhitespace(ws);
            _writer.Write(ws);
        }

        private void EnsureWriteBuffer()
        {
            if (_writeBuffer == null)
            {
                _writeBuffer = BufferUtils.RentBuffer(_arrayPool, 35);
            }
        }

        private void WriteIntegerValue(long value)
        {
            if (value >= 0L && value <= 9L)
            {
                _writer.Write((char)(48L + value));
                return;
            }
            ulong uvalue = (ulong)((value < 0L) ? (-value) : value);
            if (value < 0L)
            {
                _writer.Write('-');
            }
            WriteIntegerValue(uvalue);
        }

        private void WriteIntegerValue(ulong uvalue)
        {
            if (uvalue <= 9UL)
            {
                _writer.Write((char)(48UL + uvalue));
                return;
            }
            EnsureWriteBuffer();
            int num = MathUtils.IntLength(uvalue);
            int num2 = 0;
            do
            {
                _writeBuffer[num - ++num2] = (char)(48UL + uvalue % 10UL);
                uvalue /= 10UL;
            }
            while (uvalue != 0UL);
            _writer.Write(_writeBuffer, 0, num2);
        }

        private readonly TextWriter _writer;

        private Base64Encoder _base64Encoder;

        private char _indentChar;

        private int _indentation;

        private char _quoteChar;

        private bool _quoteName;

        private bool[] _charEscapeFlags;

        private char[] _writeBuffer;

        private IArrayPool<char> _arrayPool;

        private char[] _indentChars;
    }
}
