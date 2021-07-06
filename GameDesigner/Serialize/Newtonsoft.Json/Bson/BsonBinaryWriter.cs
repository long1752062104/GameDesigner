using Newtonsoft_X.Json.Utilities;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Newtonsoft_X.Json.Bson
{
    internal class BsonBinaryWriter
    {
        public DateTimeKind DateTimeKindHandling { get; set; }

        public BsonBinaryWriter(BinaryWriter writer)
        {
            DateTimeKindHandling = DateTimeKind.Utc;
            _writer = writer;
        }

        public void Flush()
        {
            _writer.Flush();
        }

        public void Close()
        {
            _writer.Close();
        }

        public void WriteToken(BsonToken t)
        {
            CalculateSize(t);
            WriteTokenInternal(t);
        }

        private void WriteTokenInternal(BsonToken t)
        {
            switch (t.Type)
            {
                case BsonType.Number:
                    {
                        BsonValue bsonValue = (BsonValue)t;
                        _writer.Write(Convert.ToDouble(bsonValue.Value, CultureInfo.InvariantCulture));
                        return;
                    }
                case BsonType.String:
                    {
                        BsonString bsonString = (BsonString)t;
                        WriteString((string)bsonString.Value, bsonString.ByteCount, new int?(bsonString.CalculatedSize - 4));
                        return;
                    }
                case BsonType.Object:
                    {
                        BsonObject bsonObject = (BsonObject)t;
                        _writer.Write(bsonObject.CalculatedSize);
                        foreach (BsonProperty bsonProperty in bsonObject)
                        {
                            _writer.Write((sbyte)bsonProperty.Value.Type);
                            WriteString((string)bsonProperty.Name.Value, bsonProperty.Name.ByteCount, null);
                            WriteTokenInternal(bsonProperty.Value);
                        }
                        _writer.Write(0);
                        return;
                    }
                case BsonType.Array:
                    {
                        BsonArray bsonArray = (BsonArray)t;
                        _writer.Write(bsonArray.CalculatedSize);
                        ulong num = 0UL;
                        foreach (BsonToken bsonToken in bsonArray)
                        {
                            _writer.Write((sbyte)bsonToken.Type);
                            WriteString(num.ToString(CultureInfo.InvariantCulture), MathUtils.IntLength(num), null);
                            WriteTokenInternal(bsonToken);
                            num += 1UL;
                        }
                        _writer.Write(0);
                        return;
                    }
                case BsonType.Binary:
                    {
                        BsonBinary bsonBinary = (BsonBinary)t;
                        byte[] array = (byte[])bsonBinary.Value;
                        _writer.Write(array.Length);
                        _writer.Write((byte)bsonBinary.BinaryType);
                        _writer.Write(array);
                        return;
                    }
                case BsonType.Undefined:
                case BsonType.Null:
                    return;
                case BsonType.Oid:
                    {
                        byte[] buffer = (byte[])((BsonValue)t).Value;
                        _writer.Write(buffer);
                        return;
                    }
                case BsonType.Boolean:
                    {
                        BsonValue bsonValue2 = (BsonValue)t;
                        _writer.Write((bool)bsonValue2.Value);
                        return;
                    }
                case BsonType.Date:
                    {
                        BsonValue bsonValue3 = (BsonValue)t;
                        long value;
                        if (bsonValue3.Value is DateTime)
                        {
                            DateTime dateTime = (DateTime)bsonValue3.Value;
                            if (DateTimeKindHandling == DateTimeKind.Utc)
                            {
                                dateTime = dateTime.ToUniversalTime();
                            }
                            else if (DateTimeKindHandling == DateTimeKind.Local)
                            {
                                dateTime = dateTime.ToLocalTime();
                            }
                            value = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(dateTime, false);
                        }
                        else
                        {
                            DateTimeOffset dateTimeOffset = (DateTimeOffset)bsonValue3.Value;
                            value = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(dateTimeOffset.UtcDateTime, dateTimeOffset.Offset);
                        }
                        _writer.Write(value);
                        return;
                    }
                case BsonType.Regex:
                    {
                        BsonRegex bsonRegex = (BsonRegex)t;
                        WriteString((string)bsonRegex.Pattern.Value, bsonRegex.Pattern.ByteCount, null);
                        WriteString((string)bsonRegex.Options.Value, bsonRegex.Options.ByteCount, null);
                        return;
                    }
                case BsonType.Integer:
                    {
                        BsonValue bsonValue4 = (BsonValue)t;
                        _writer.Write(Convert.ToInt32(bsonValue4.Value, CultureInfo.InvariantCulture));
                        return;
                    }
                case BsonType.Long:
                    {
                        BsonValue bsonValue5 = (BsonValue)t;
                        _writer.Write(Convert.ToInt64(bsonValue5.Value, CultureInfo.InvariantCulture));
                        return;
                    }
            }
            throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(CultureInfo.InvariantCulture, t.Type));
        }

        private void WriteString(string s, int byteCount, int? calculatedlengthPrefix)
        {
            if (calculatedlengthPrefix != null)
            {
                _writer.Write(calculatedlengthPrefix.GetValueOrDefault());
            }
            WriteUtf8Bytes(s, byteCount);
            _writer.Write(0);
        }

        public void WriteUtf8Bytes(string s, int byteCount)
        {
            if (s != null)
            {
                if (_largeByteBuffer == null)
                {
                    _largeByteBuffer = new byte[256];
                }
                if (byteCount <= 256)
                {
                    BsonBinaryWriter.Encoding.GetBytes(s, 0, s.Length, _largeByteBuffer, 0);
                    _writer.Write(_largeByteBuffer, 0, byteCount);
                    return;
                }
                byte[] bytes = BsonBinaryWriter.Encoding.GetBytes(s);
                _writer.Write(bytes);
            }
        }

        private int CalculateSize(int stringByteCount)
        {
            return stringByteCount + 1;
        }

        private int CalculateSizeWithLength(int stringByteCount, bool includeSize)
        {
            return (includeSize ? 5 : 1) + stringByteCount;
        }

        private int CalculateSize(BsonToken t)
        {
            switch (t.Type)
            {
                case BsonType.Number:
                    return 8;
                case BsonType.String:
                    {
                        BsonString bsonString = (BsonString)t;
                        string text = (string)bsonString.Value;
                        bsonString.ByteCount = ((text != null) ? BsonBinaryWriter.Encoding.GetByteCount(text) : 0);
                        bsonString.CalculatedSize = CalculateSizeWithLength(bsonString.ByteCount, bsonString.IncludeLength);
                        return bsonString.CalculatedSize;
                    }
                case BsonType.Object:
                    {
                        BsonObject bsonObject = (BsonObject)t;
                        int num = 4;
                        foreach (BsonProperty bsonProperty in bsonObject)
                        {
                            int num2 = 1;
                            num2 += CalculateSize(bsonProperty.Name);
                            num2 += CalculateSize(bsonProperty.Value);
                            num += num2;
                        }
                        num++;
                        bsonObject.CalculatedSize = num;
                        return num;
                    }
                case BsonType.Array:
                    {
                        BsonArray bsonArray = (BsonArray)t;
                        int num3 = 4;
                        ulong num4 = 0UL;
                        foreach (BsonToken t2 in bsonArray)
                        {
                            num3++;
                            num3 += CalculateSize(MathUtils.IntLength(num4));
                            num3 += CalculateSize(t2);
                            num4 += 1UL;
                        }
                        num3++;
                        bsonArray.CalculatedSize = num3;
                        return bsonArray.CalculatedSize;
                    }
                case BsonType.Binary:
                    {
                        BsonBinary bsonBinary = (BsonBinary)t;
                        byte[] array = (byte[])bsonBinary.Value;
                        bsonBinary.CalculatedSize = 5 + array.Length;
                        return bsonBinary.CalculatedSize;
                    }
                case BsonType.Undefined:
                case BsonType.Null:
                    return 0;
                case BsonType.Oid:
                    return 12;
                case BsonType.Boolean:
                    return 1;
                case BsonType.Date:
                    return 8;
                case BsonType.Regex:
                    {
                        BsonRegex bsonRegex = (BsonRegex)t;
                        int num5 = 0;
                        num5 += CalculateSize(bsonRegex.Pattern);
                        num5 += CalculateSize(bsonRegex.Options);
                        bsonRegex.CalculatedSize = num5;
                        return bsonRegex.CalculatedSize;
                    }
                case BsonType.Integer:
                    return 4;
                case BsonType.Long:
                    return 8;
            }
            throw new ArgumentOutOfRangeException("t", "Unexpected token when writing BSON: {0}".FormatWith(CultureInfo.InvariantCulture, t.Type));
        }

        private static readonly Encoding Encoding = new UTF8Encoding(false);

        private readonly BinaryWriter _writer;

        private byte[] _largeByteBuffer;
    }
}
