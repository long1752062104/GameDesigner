using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Newtonsoft_X.Json.Bson
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to serialized BSON data.
    /// </summary>
    public class BsonReader : JsonReader
    {
        /// <summary>
        /// Gets or sets a value indicating whether binary data reading should be compatible with incorrect Json.NET 3.5 written binary.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if binary data reading will be compatible with incorrect Json.NET 3.5 written binary; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("JsonNet35BinaryCompatibility will be removed in a future version of Json.NET.")]
        public bool JsonNet35BinaryCompatibility
        {
            get
            {
                return _jsonNet35BinaryCompatibility;
            }
            set
            {
                _jsonNet35BinaryCompatibility = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the root object will be read as a JSON array.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the root object will be read as a JSON array; otherwise, <c>false</c>.
        /// </value>
        public bool ReadRootValueAsArray
        {
            get
            {
                return _readRootValueAsArray;
            }
            set
            {
                _readRootValueAsArray = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.DateTimeKind" /> used when reading <see cref="T:System.DateTime" /> values from BSON.
        /// </summary>
        /// <value>The <see cref="T:System.DateTimeKind" /> used when reading <see cref="T:System.DateTime" /> values from BSON.</value>
        public DateTimeKind DateTimeKindHandling
        {
            get
            {
                return _dateTimeKindHandling;
            }
            set
            {
                _dateTimeKindHandling = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Bson.BsonReader" /> class.
        /// </summary>
        /// <param name="stream">The <see cref="T:System.IO.Stream" /> containing the BSON data to read.</param>
        public BsonReader(Stream stream) : this(stream, false, DateTimeKind.Local)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Bson.BsonReader" /> class.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.IO.BinaryReader" /> containing the BSON data to read.</param>
        public BsonReader(BinaryReader reader) : this(reader, false, DateTimeKind.Local)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Bson.BsonReader" /> class.
        /// </summary>
        /// <param name="stream">The <see cref="T:System.IO.Stream" /> containing the BSON data to read.</param>
        /// <param name="readRootValueAsArray">if set to <c>true</c> the root object will be read as a JSON array.</param>
        /// <param name="dateTimeKindHandling">The <see cref="T:System.DateTimeKind" /> used when reading <see cref="T:System.DateTime" /> values from BSON.</param>
        public BsonReader(Stream stream, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
        {
            ValidationUtils.ArgumentNotNull(stream, "stream");
            _reader = new BinaryReader(stream);
            _stack = new List<BsonReader.ContainerContext>();
            _readRootValueAsArray = readRootValueAsArray;
            _dateTimeKindHandling = dateTimeKindHandling;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Bson.BsonReader" /> class.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.IO.BinaryReader" /> containing the BSON data to read.</param>
        /// <param name="readRootValueAsArray">if set to <c>true</c> the root object will be read as a JSON array.</param>
        /// <param name="dateTimeKindHandling">The <see cref="T:System.DateTimeKind" /> used when reading <see cref="T:System.DateTime" /> values from BSON.</param>
        public BsonReader(BinaryReader reader, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            _reader = reader;
            _stack = new List<BsonReader.ContainerContext>();
            _readRootValueAsArray = readRootValueAsArray;
            _dateTimeKindHandling = dateTimeKindHandling;
        }

        private string ReadElement()
        {
            _currentElementType = ReadType();
            return ReadString();
        }

        /// <summary>
        /// Reads the next JSON token from the underlying <see cref="T:System.IO.Stream" />.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the next token was read successfully; <c>false</c> if there are no more tokens to read.
        /// </returns>
        public override bool Read()
        {
            bool result;
            try
            {
                bool flag;
                switch (_bsonReaderState)
                {
                    case BsonReader.BsonReaderState.Normal:
                        flag = ReadNormal();
                        break;
                    case BsonReader.BsonReaderState.ReferenceStart:
                    case BsonReader.BsonReaderState.ReferenceRef:
                    case BsonReader.BsonReaderState.ReferenceId:
                        flag = ReadReference();
                        break;
                    case BsonReader.BsonReaderState.CodeWScopeStart:
                    case BsonReader.BsonReaderState.CodeWScopeCode:
                    case BsonReader.BsonReaderState.CodeWScopeScope:
                    case BsonReader.BsonReaderState.CodeWScopeScopeObject:
                    case BsonReader.BsonReaderState.CodeWScopeScopeEnd:
                        flag = ReadCodeWScope();
                        break;
                    default:
                        throw JsonReaderException.Create(this, "Unexpected state: {0}".FormatWith(CultureInfo.InvariantCulture, _bsonReaderState));
                }
                if (!flag)
                {
                    base.SetToken(JsonToken.None);
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            catch (EndOfStreamException)
            {
                base.SetToken(JsonToken.None);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Changes the reader's state to <see cref="F:Newtonsoft.Json.JsonReader.State.Closed" />.
        /// If <see cref="P:Newtonsoft.Json.JsonReader.CloseInput" /> is set to <c>true</c>, the underlying <see cref="T:System.IO.Stream" /> is also closed.
        /// </summary>
        public override void Close()
        {
            base.Close();
            if (base.CloseInput && _reader != null)
            {
                _reader.Close();
            }
        }

        private bool ReadCodeWScope()
        {
            switch (_bsonReaderState)
            {
                case BsonReader.BsonReaderState.CodeWScopeStart:
                    base.SetToken(JsonToken.PropertyName, "$code");
                    _bsonReaderState = BsonReader.BsonReaderState.CodeWScopeCode;
                    return true;
                case BsonReader.BsonReaderState.CodeWScopeCode:
                    ReadInt32();
                    base.SetToken(JsonToken.String, ReadLengthString());
                    _bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScope;
                    return true;
                case BsonReader.BsonReaderState.CodeWScopeScope:
                    {
                        if (base.CurrentState == JsonReader.State.PostValue)
                        {
                            base.SetToken(JsonToken.PropertyName, "$scope");
                            return true;
                        }
                        base.SetToken(JsonToken.StartObject);
                        _bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScopeObject;
                        BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(BsonType.Object);
                        PushContext(containerContext);
                        containerContext.Length = ReadInt32();
                        return true;
                    }
                case BsonReader.BsonReaderState.CodeWScopeScopeObject:
                    {
                        bool flag = ReadNormal();
                        if (flag && TokenType == JsonToken.EndObject)
                        {
                            _bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScopeEnd;
                        }
                        return flag;
                    }
                case BsonReader.BsonReaderState.CodeWScopeScopeEnd:
                    base.SetToken(JsonToken.EndObject);
                    _bsonReaderState = BsonReader.BsonReaderState.Normal;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool ReadReference()
        {
            JsonReader.State currentState = base.CurrentState;
            if (currentState != JsonReader.State.Property)
            {
                if (currentState == JsonReader.State.ObjectStart)
                {
                    base.SetToken(JsonToken.PropertyName, "$ref");
                    _bsonReaderState = BsonReader.BsonReaderState.ReferenceRef;
                    return true;
                }
                if (currentState != JsonReader.State.PostValue)
                {
                    throw JsonReaderException.Create(this, "Unexpected state when reading BSON reference: " + base.CurrentState);
                }
                if (_bsonReaderState == BsonReader.BsonReaderState.ReferenceRef)
                {
                    base.SetToken(JsonToken.PropertyName, "$id");
                    _bsonReaderState = BsonReader.BsonReaderState.ReferenceId;
                    return true;
                }
                if (_bsonReaderState == BsonReader.BsonReaderState.ReferenceId)
                {
                    base.SetToken(JsonToken.EndObject);
                    _bsonReaderState = BsonReader.BsonReaderState.Normal;
                    return true;
                }
                throw JsonReaderException.Create(this, "Unexpected state when reading BSON reference: " + _bsonReaderState);
            }
            else
            {
                if (_bsonReaderState == BsonReader.BsonReaderState.ReferenceRef)
                {
                    base.SetToken(JsonToken.String, ReadLengthString());
                    return true;
                }
                if (_bsonReaderState == BsonReader.BsonReaderState.ReferenceId)
                {
                    base.SetToken(JsonToken.Bytes, ReadBytes(12));
                    return true;
                }
                throw JsonReaderException.Create(this, "Unexpected state when reading BSON reference: " + _bsonReaderState);
            }
        }

        private bool ReadNormal()
        {
            switch (base.CurrentState)
            {
                case JsonReader.State.Start:
                    {
                        JsonToken token = (!_readRootValueAsArray) ? JsonToken.StartObject : JsonToken.StartArray;
                        BsonType type = (!_readRootValueAsArray) ? BsonType.Object : BsonType.Array;
                        base.SetToken(token);
                        BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(type);
                        PushContext(containerContext);
                        containerContext.Length = ReadInt32();
                        return true;
                    }
                case JsonReader.State.Complete:
                case JsonReader.State.Closed:
                    return false;
                case JsonReader.State.Property:
                    ReadType(_currentElementType);
                    return true;
                case JsonReader.State.ObjectStart:
                case JsonReader.State.ArrayStart:
                case JsonReader.State.PostValue:
                    {
                        BsonReader.ContainerContext currentContext = _currentContext;
                        if (currentContext == null)
                        {
                            return false;
                        }
                        int num = currentContext.Length - 1;
                        if (currentContext.Position < num)
                        {
                            if (currentContext.Type == BsonType.Array)
                            {
                                ReadElement();
                                ReadType(_currentElementType);
                                return true;
                            }
                            base.SetToken(JsonToken.PropertyName, ReadElement());
                            return true;
                        }
                        else
                        {
                            if (currentContext.Position != num)
                            {
                                throw JsonReaderException.Create(this, "Read past end of current container context.");
                            }
                            if (ReadByte() != 0)
                            {
                                throw JsonReaderException.Create(this, "Unexpected end of object byte value.");
                            }
                            PopContext();
                            if (_currentContext != null)
                            {
                                MovePosition(currentContext.Length);
                            }
                            JsonToken token2 = (currentContext.Type == BsonType.Object) ? JsonToken.EndObject : JsonToken.EndArray;
                            base.SetToken(token2);
                            return true;
                        }
                    }
                case JsonReader.State.ConstructorStart:
                case JsonReader.State.Constructor:
                case JsonReader.State.Error:
                case JsonReader.State.Finished:
                    return false;
            }
            throw new ArgumentOutOfRangeException();
        }

        private void PopContext()
        {
            _stack.RemoveAt(_stack.Count - 1);
            if (_stack.Count == 0)
            {
                _currentContext = null;
                return;
            }
            _currentContext = _stack[_stack.Count - 1];
        }

        private void PushContext(BsonReader.ContainerContext newContext)
        {
            _stack.Add(newContext);
            _currentContext = newContext;
        }

        private byte ReadByte()
        {
            MovePosition(1);
            return _reader.ReadByte();
        }

        private void ReadType(BsonType type)
        {
            switch (type)
            {
                case BsonType.Number:
                    {
                        double num = ReadDouble();
                        if (_floatParseHandling == FloatParseHandling.Decimal)
                        {
                            base.SetToken(JsonToken.Float, Convert.ToDecimal(num, CultureInfo.InvariantCulture));
                            return;
                        }
                        base.SetToken(JsonToken.Float, num);
                        return;
                    }
                case BsonType.String:
                case BsonType.Symbol:
                    base.SetToken(JsonToken.String, ReadLengthString());
                    return;
                case BsonType.Object:
                    {
                        base.SetToken(JsonToken.StartObject);
                        BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(BsonType.Object);
                        PushContext(containerContext);
                        containerContext.Length = ReadInt32();
                        return;
                    }
                case BsonType.Array:
                    {
                        base.SetToken(JsonToken.StartArray);
                        BsonReader.ContainerContext containerContext2 = new BsonReader.ContainerContext(BsonType.Array);
                        PushContext(containerContext2);
                        containerContext2.Length = ReadInt32();
                        return;
                    }
                case BsonType.Binary:
                    {
                        byte[] array = ReadBinary(out BsonBinaryType bsonBinaryType);
                        object value;
                        if (bsonBinaryType != BsonBinaryType.Uuid)
                            value = array;
                        else
                            value = new Guid(array);
                        base.SetToken(JsonToken.Bytes, value);
                        return;
                    }
                case BsonType.Undefined:
                    base.SetToken(JsonToken.Undefined);
                    return;
                case BsonType.Oid:
                    {
                        byte[] value2 = ReadBytes(12);
                        base.SetToken(JsonToken.Bytes, value2);
                        return;
                    }
                case BsonType.Boolean:
                    {
                        bool flag = Convert.ToBoolean(ReadByte());
                        base.SetToken(JsonToken.Boolean, flag);
                        return;
                    }
                case BsonType.Date:
                    {
                        DateTime dateTime = DateTimeUtils.ConvertJavaScriptTicksToDateTime(ReadInt64());
                        DateTimeKind dateTimeKindHandling = DateTimeKindHandling;
                        DateTime dateTime2;
                        if (dateTimeKindHandling != DateTimeKind.Unspecified)
                        {
                            if (dateTimeKindHandling != DateTimeKind.Local)
                            {
                                dateTime2 = dateTime;
                            }
                            else
                            {
                                dateTime2 = dateTime.ToLocalTime();
                            }
                        }
                        else
                        {
                            dateTime2 = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
                        }
                        base.SetToken(JsonToken.Date, dateTime2);
                        return;
                    }
                case BsonType.Null:
                    base.SetToken(JsonToken.Null);
                    return;
                case BsonType.Regex:
                    {
                        string str = ReadString();
                        string str2 = ReadString();
                        string value3 = "/" + str + "/" + str2;
                        base.SetToken(JsonToken.String, value3);
                        return;
                    }
                case BsonType.Reference:
                    base.SetToken(JsonToken.StartObject);
                    _bsonReaderState = BsonReader.BsonReaderState.ReferenceStart;
                    return;
                case BsonType.Code:
                    base.SetToken(JsonToken.String, ReadLengthString());
                    return;
                case BsonType.CodeWScope:
                    base.SetToken(JsonToken.StartObject);
                    _bsonReaderState = BsonReader.BsonReaderState.CodeWScopeStart;
                    return;
                case BsonType.Integer:
                    base.SetToken(JsonToken.Integer, (long)ReadInt32());
                    return;
                case BsonType.TimeStamp:
                case BsonType.Long:
                    base.SetToken(JsonToken.Integer, ReadInt64());
                    return;
                default:
                    throw new ArgumentOutOfRangeException("type", "Unexpected BsonType value: " + type);
            }
        }

        private byte[] ReadBinary(out BsonBinaryType binaryType)
        {
            int count = ReadInt32();
            binaryType = (BsonBinaryType)ReadByte();
            if (binaryType == BsonBinaryType.BinaryOld && !_jsonNet35BinaryCompatibility)
            {
                count = ReadInt32();
            }
            return ReadBytes(count);
        }

        private string ReadString()
        {
            EnsureBuffers();
            StringBuilder stringBuilder = null;
            int num = 0;
            int num2 = 0;
            int num4;
            for (; ; )
            {
                int num3 = num2;
                byte b;
                while (num3 < 128 && (b = _reader.ReadByte()) > 0)
                {
                    _byteBuffer[num3++] = b;
                }
                num4 = num3 - num2;
                num += num4;
                if (num3 < 128 && stringBuilder == null)
                {
                    break;
                }
                int lastFullCharStop = GetLastFullCharStop(num3 - 1);
                int chars = Encoding.UTF8.GetChars(_byteBuffer, 0, lastFullCharStop + 1, _charBuffer, 0);
                if (stringBuilder == null)
                {
                    stringBuilder = new StringBuilder(256);
                }
                stringBuilder.Append(_charBuffer, 0, chars);
                if (lastFullCharStop < num4 - 1)
                {
                    num2 = num4 - lastFullCharStop - 1;
                    Array.Copy(_byteBuffer, lastFullCharStop + 1, _byteBuffer, 0, num2);
                }
                else
                {
                    if (num3 < 128)
                    {
                        goto Block_6;
                    }
                    num2 = 0;
                }
            }
            int chars2 = Encoding.UTF8.GetChars(_byteBuffer, 0, num4, _charBuffer, 0);
            MovePosition(num + 1);
            return new string(_charBuffer, 0, chars2);
        Block_6:
            MovePosition(num + 1);
            return stringBuilder.ToString();
        }

        private string ReadLengthString()
        {
            int num = ReadInt32();
            MovePosition(num);
            string @string = GetString(num - 1);
            _reader.ReadByte();
            return @string;
        }

        private string GetString(int length)
        {
            if (length == 0)
            {
                return string.Empty;
            }
            EnsureBuffers();
            StringBuilder stringBuilder = null;
            int num = 0;
            int num2 = 0;
            int num3;
            for (; ; )
            {
                int count = (length - num > 128 - num2) ? (128 - num2) : (length - num);
                num3 = _reader.Read(_byteBuffer, num2, count);
                if (num3 == 0)
                {
                    break;
                }
                num += num3;
                num3 += num2;
                if (num3 == length)
                {
                    goto Block_4;
                }
                int lastFullCharStop = GetLastFullCharStop(num3 - 1);
                if (stringBuilder == null)
                {
                    stringBuilder = new StringBuilder(length);
                }
                int chars = Encoding.UTF8.GetChars(_byteBuffer, 0, lastFullCharStop + 1, _charBuffer, 0);
                stringBuilder.Append(_charBuffer, 0, chars);
                if (lastFullCharStop < num3 - 1)
                {
                    num2 = num3 - lastFullCharStop - 1;
                    Array.Copy(_byteBuffer, lastFullCharStop + 1, _byteBuffer, 0, num2);
                }
                else
                {
                    num2 = 0;
                }
                if (num >= length)
                {
                    goto Block_7;
                }
            }
            throw new EndOfStreamException("Unable to read beyond the end of the stream.");
        Block_4:
            int chars2 = Encoding.UTF8.GetChars(_byteBuffer, 0, num3, _charBuffer, 0);
            return new string(_charBuffer, 0, chars2);
        Block_7:
            return stringBuilder.ToString();
        }

        private int GetLastFullCharStop(int start)
        {
            int i = start;
            int num = 0;
            while (i >= 0)
            {
                num = BytesInSequence(_byteBuffer[i]);
                if (num == 0)
                {
                    i--;
                }
                else
                {
                    if (num != 1)
                    {
                        i--;
                        break;
                    }
                    break;
                }
            }
            if (num == start - i)
            {
                return start;
            }
            return i;
        }

        private int BytesInSequence(byte b)
        {
            if (b <= BsonReader.SeqRange1[1])
            {
                return 1;
            }
            if (b >= BsonReader.SeqRange2[0] && b <= BsonReader.SeqRange2[1])
            {
                return 2;
            }
            if (b >= BsonReader.SeqRange3[0] && b <= BsonReader.SeqRange3[1])
            {
                return 3;
            }
            if (b >= BsonReader.SeqRange4[0] && b <= BsonReader.SeqRange4[1])
            {
                return 4;
            }
            return 0;
        }

        private void EnsureBuffers()
        {
            if (_byteBuffer == null)
            {
                _byteBuffer = new byte[128];
            }
            if (_charBuffer == null)
            {
                int maxCharCount = Encoding.UTF8.GetMaxCharCount(128);
                _charBuffer = new char[maxCharCount];
            }
        }

        private double ReadDouble()
        {
            MovePosition(8);
            return _reader.ReadDouble();
        }

        private int ReadInt32()
        {
            MovePosition(4);
            return _reader.ReadInt32();
        }

        private long ReadInt64()
        {
            MovePosition(8);
            return _reader.ReadInt64();
        }

        private BsonType ReadType()
        {
            MovePosition(1);
            return (BsonType)_reader.ReadSByte();
        }

        private void MovePosition(int count)
        {
            _currentContext.Position += count;
        }

        private byte[] ReadBytes(int count)
        {
            MovePosition(count);
            return _reader.ReadBytes(count);
        }

        private const int MaxCharBytesSize = 128;

        private static readonly byte[] SeqRange1 = new byte[]
        {
            0,
            127
        };

        private static readonly byte[] SeqRange2 = new byte[]
        {
            194,
            223
        };

        private static readonly byte[] SeqRange3 = new byte[]
        {
            224,
            239
        };

        private static readonly byte[] SeqRange4 = new byte[]
        {
            240,
            244
        };

        private readonly BinaryReader _reader;

        private readonly List<BsonReader.ContainerContext> _stack;

        private byte[] _byteBuffer;

        private char[] _charBuffer;

        private BsonType _currentElementType;

        private BsonReader.BsonReaderState _bsonReaderState;

        private BsonReader.ContainerContext _currentContext;

        private bool _readRootValueAsArray;

        private bool _jsonNet35BinaryCompatibility;

        private DateTimeKind _dateTimeKindHandling;

        private enum BsonReaderState
        {
            Normal,
            ReferenceStart,
            ReferenceRef,
            ReferenceId,
            CodeWScopeStart,
            CodeWScopeCode,
            CodeWScopeScope,
            CodeWScopeScopeObject,
            CodeWScopeScopeEnd
        }

        private class ContainerContext
        {
            public ContainerContext(BsonType type)
            {
                Type = type;
            }

            public readonly BsonType Type;

            public int Length;

            public int Position;
        }
    }
}
