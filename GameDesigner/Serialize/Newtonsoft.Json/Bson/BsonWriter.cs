using Newtonsoft_X.Json.Utilities;
using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft_X.Json.Bson
{
    /// <summary>
    /// Represents a writer that provides a fast, non-cached, forward-only way of generating BSON data.
    /// </summary>
    public class BsonWriter : JsonWriter
    {
        /// <summary>
        /// Gets or sets the <see cref="T:System.DateTimeKind" /> used when writing <see cref="T:System.DateTime" /> values to BSON.
        /// When set to <see cref="F:System.DateTimeKind.Unspecified" /> no conversion will occur.
        /// </summary>
        /// <value>The <see cref="T:System.DateTimeKind" /> used when writing <see cref="T:System.DateTime" /> values to BSON.</value>
        public DateTimeKind DateTimeKindHandling
        {
            get
            {
                return _writer.DateTimeKindHandling;
            }
            set
            {
                _writer.DateTimeKindHandling = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Bson.BsonWriter" /> class.
        /// </summary>
        /// <param name="stream">The <see cref="T:System.IO.Stream" /> to write to.</param>
        public BsonWriter(Stream stream)
        {
            ValidationUtils.ArgumentNotNull(stream, "stream");
            _writer = new BsonBinaryWriter(new BinaryWriter(stream));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Bson.BsonWriter" /> class.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.IO.BinaryWriter" /> to write to.</param>
        public BsonWriter(BinaryWriter writer)
        {
            ValidationUtils.ArgumentNotNull(writer, "writer");
            _writer = new BsonBinaryWriter(writer);
        }

        /// <summary>
        /// Flushes whatever is in the buffer to the underlying <see cref="T:System.IO.Stream" /> and also flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            _writer.Flush();
        }

        /// <summary>
        /// Writes the end.
        /// </summary>
        /// <param name="token">The token.</param>
        protected override void WriteEnd(JsonToken token)
        {
            base.WriteEnd(token);
            RemoveParent();
            if (base.Top == 0)
            {
                _writer.WriteToken(_root);
            }
        }

        /// <summary>
        /// Writes a comment <c>/*...*/</c> containing the specified text.
        /// </summary>
        /// <param name="text">Text to place inside the comment.</param>
        public override void WriteComment(string text)
        {
            throw JsonWriterException.Create(this, "Cannot write JSON comment as BSON.", null);
        }

        /// <summary>
        /// Writes the start of a constructor with the given name.
        /// </summary>
        /// <param name="name">The name of the constructor.</param>
        public override void WriteStartConstructor(string name)
        {
            throw JsonWriterException.Create(this, "Cannot write JSON constructor as BSON.", null);
        }

        /// <summary>
        /// Writes raw JSON.
        /// </summary>
        /// <param name="json">The raw JSON to write.</param>
        public override void WriteRaw(string json)
        {
            throw JsonWriterException.Create(this, "Cannot write raw JSON as BSON.", null);
        }

        /// <summary>
        /// Writes raw JSON where a value is expected and updates the writer's state.
        /// </summary>
        /// <param name="json">The raw JSON to write.</param>
        public override void WriteRawValue(string json)
        {
            throw JsonWriterException.Create(this, "Cannot write raw JSON as BSON.", null);
        }

        /// <summary>
        /// Writes the beginning of a JSON array.
        /// </summary>
        public override void WriteStartArray()
        {
            base.WriteStartArray();
            AddParent(new BsonArray());
        }

        /// <summary>
        /// Writes the beginning of a JSON object.
        /// </summary>
        public override void WriteStartObject()
        {
            base.WriteStartObject();
            AddParent(new BsonObject());
        }

        /// <summary>
        /// Writes the property name of a name/value pair on a JSON object.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        public override void WritePropertyName(string name)
        {
            base.WritePropertyName(name);
            _propertyName = name;
        }

        /// <summary>
        /// Closes this writer.
        /// If <see cref="P:Newtonsoft.Json.JsonWriter.CloseOutput" /> is set to <c>true</c>, the underlying <see cref="T:System.IO.Stream" /> is also closed.
        /// If <see cref="P:Newtonsoft.Json.JsonWriter.AutoCompleteOnClose" /> is set to <c>true</c>, the JSON is auto-completed.
        /// </summary>
        public override void Close()
        {
            base.Close();
            if (base.CloseOutput && _writer != null)
            {
                _writer.Close();
            }
        }

        private void AddParent(BsonToken container)
        {
            AddToken(container);
            _parent = container;
        }

        private void RemoveParent()
        {
            _parent = _parent.Parent;
        }

        private void AddValue(object value, BsonType type)
        {
            AddToken(new BsonValue(value, type));
        }

        internal void AddToken(BsonToken token)
        {
            if (_parent != null)
            {
                if (_parent is BsonObject)
                {
                    ((BsonObject)_parent).Add(_propertyName, token);
                    _propertyName = null;
                    return;
                }
                ((BsonArray)_parent).Add(token);
                return;
            }
            else
            {
                if (token.Type != BsonType.Object && token.Type != BsonType.Array)
                {
                    throw JsonWriterException.Create(this, "Error writing {0} value. BSON must start with an Object or Array.".FormatWith(CultureInfo.InvariantCulture, token.Type), null);
                }
                _parent = token;
                _root = token;
                return;
            }
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
            base.WriteNull();
            AddValue(null, BsonType.Null);
        }

        /// <summary>
        /// Writes an undefined value.
        /// </summary>
        public override void WriteUndefined()
        {
            base.WriteUndefined();
            AddValue(null, BsonType.Undefined);
        }

        /// <summary>
        /// Writes a <see cref="T:System.String" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.String" /> value to write.</param>
        public override void WriteValue(string value)
        {
            base.WriteValue(value);
            if (value == null)
            {
                AddValue(null, BsonType.Null);
                return;
            }
            AddToken(new BsonString(value, true));
        }

        /// <summary>
        /// Writes a <see cref="T:System.Int32" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Int32" /> value to write.</param>
        public override void WriteValue(int value)
        {
            base.WriteValue(value);
            AddValue(value, BsonType.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.UInt32" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.UInt32" /> value to write.</param>
        //[CLSCompliant(false)]
        public override void WriteValue(uint value)
        {
            if (value > 2147483647U)
            {
                throw JsonWriterException.Create(this, "Value is too large to fit in a signed 32 bit integer. BSON does not support unsigned values.", null);
            }
            base.WriteValue(value);
            AddValue(value, BsonType.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Int64" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Int64" /> value to write.</param>
        public override void WriteValue(long value)
        {
            base.WriteValue(value);
            AddValue(value, BsonType.Long);
        }

        /// <summary>
        /// Writes a <see cref="T:System.UInt64" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.UInt64" /> value to write.</param>
        //[CLSCompliant(false)]
        public override void WriteValue(ulong value)
        {
            if (value > 9223372036854775807UL)
            {
                throw JsonWriterException.Create(this, "Value is too large to fit in a signed 64 bit integer. BSON does not support unsigned values.", null);
            }
            base.WriteValue(value);
            AddValue(value, BsonType.Long);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Single" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Single" /> value to write.</param>
        public override void WriteValue(float value)
        {
            base.WriteValue(value);
            AddValue(value, BsonType.Number);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Double" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Double" /> value to write.</param>
        public override void WriteValue(double value)
        {
            base.WriteValue(value);
            AddValue(value, BsonType.Number);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Boolean" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Boolean" /> value to write.</param>
        public override void WriteValue(bool value)
        {
            base.WriteValue(value);
            AddValue(value, BsonType.Boolean);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Int16" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Int16" /> value to write.</param>
        public override void WriteValue(short value)
        {
            base.WriteValue(value);
            AddValue(value, BsonType.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.UInt16" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.UInt16" /> value to write.</param>
        //[CLSCompliant(false)]
        public override void WriteValue(ushort value)
        {
            base.WriteValue(value);
            AddValue(value, BsonType.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Char" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Char" /> value to write.</param>
        public override void WriteValue(char value)
        {
            base.WriteValue(value);
            string value2 = value.ToString(CultureInfo.InvariantCulture);
            AddToken(new BsonString(value2, true));
        }

        /// <summary>
        /// Writes a <see cref="T:System.Byte" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Byte" /> value to write.</param>
        public override void WriteValue(byte value)
        {
            base.WriteValue(value);
            AddValue(value, BsonType.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.SByte" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.SByte" /> value to write.</param>
        //[CLSCompliant(false)]
        public override void WriteValue(sbyte value)
        {
            base.WriteValue(value);
            AddValue(value, BsonType.Integer);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Decimal" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Decimal" /> value to write.</param>
        public override void WriteValue(decimal value)
        {
            base.WriteValue(value);
            AddValue(value, BsonType.Number);
        }

        /// <summary>
        /// Writes a <see cref="T:System.DateTime" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.DateTime" /> value to write.</param>
        public override void WriteValue(DateTime value)
        {
            base.WriteValue(value);
            value = DateTimeUtils.EnsureDateTime(value, base.DateTimeZoneHandling);
            AddValue(value, BsonType.Date);
        }

        /// <summary>
        /// Writes a <see cref="T:System.DateTimeOffset" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.DateTimeOffset" /> value to write.</param>
        public override void WriteValue(DateTimeOffset value)
        {
            base.WriteValue(value);
            AddValue(value, BsonType.Date);
        }

        /// <summary>
        /// Writes a <see cref="T:System.Byte" />[] value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Byte" />[] value to write.</param>
        public override void WriteValue(byte[] value)
        {
            base.WriteValue(value);
            AddToken(new BsonBinary(value, BsonBinaryType.Binary));
        }

        /// <summary>
        /// Writes a <see cref="T:System.Guid" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Guid" /> value to write.</param>
        public override void WriteValue(Guid value)
        {
            base.WriteValue(value);
            AddToken(new BsonBinary(value.ToByteArray(), BsonBinaryType.Uuid));
        }

        /// <summary>
        /// Writes a <see cref="T:System.TimeSpan" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.TimeSpan" /> value to write.</param>
        public override void WriteValue(TimeSpan value)
        {
            base.WriteValue(value);
            AddToken(new BsonString(value.ToString(), true));
        }

        /// <summary>
        /// Writes a <see cref="T:System.Uri" /> value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Uri" /> value to write.</param>
        public override void WriteValue(Uri value)
        {
            base.WriteValue(value);
            AddToken(new BsonString(value.ToString(), true));
        }

        /// <summary>
        /// Writes a <see cref="T:System.Byte" />[] value that represents a BSON object id.
        /// </summary>
        /// <param name="value">The Object ID value to write.</param>
        public void WriteObjectId(byte[] value)
        {
            ValidationUtils.ArgumentNotNull(value, "value");
            if (value.Length != 12)
            {
                throw JsonWriterException.Create(this, "An object id must be 12 bytes", null);
            }
            base.UpdateScopeWithFinishedValue();
            base.AutoComplete(JsonToken.Undefined);
            AddValue(value, BsonType.Oid);
        }

        /// <summary>
        /// Writes a BSON regex.
        /// </summary>
        /// <param name="pattern">The regex pattern.</param>
        /// <param name="options">The regex options.</param>
        public void WriteRegex(string pattern, string options)
        {
            ValidationUtils.ArgumentNotNull(pattern, "pattern");
            base.UpdateScopeWithFinishedValue();
            base.AutoComplete(JsonToken.Undefined);
            AddToken(new BsonRegex(pattern, options));
        }

        private readonly BsonBinaryWriter _writer;

        private BsonToken _root;

        private BsonToken _parent;

        private string _propertyName;
    }
}
