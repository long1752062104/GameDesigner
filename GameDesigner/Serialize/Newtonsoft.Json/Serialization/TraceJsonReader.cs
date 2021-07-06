using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft_X.Json.Serialization
{
    internal class TraceJsonReader : JsonReader, IJsonLineInfo
    {
        public TraceJsonReader(JsonReader innerReader)
        {
            _innerReader = innerReader;
            _sw = new StringWriter(CultureInfo.InvariantCulture);
            _sw.Write("Deserialized JSON: " + Environment.NewLine);
            _textWriter = new JsonTextWriter(_sw);
            _textWriter.Formatting = Formatting.Indented;
        }

        public string GetDeserializedJsonMessage()
        {
            return _sw.ToString();
        }

        public override bool Read()
        {
            bool result = _innerReader.Read();
            _textWriter.WriteToken(_innerReader, false, false, true);
            return result;
        }

        public override int? ReadAsInt32()
        {
            int? result = _innerReader.ReadAsInt32();
            _textWriter.WriteToken(_innerReader, false, false, true);
            return result;
        }

        public override string ReadAsString()
        {
            string result = _innerReader.ReadAsString();
            _textWriter.WriteToken(_innerReader, false, false, true);
            return result;
        }

        public override byte[] ReadAsBytes()
        {
            byte[] result = _innerReader.ReadAsBytes();
            _textWriter.WriteToken(_innerReader, false, false, true);
            return result;
        }

        public override decimal? ReadAsDecimal()
        {
            decimal? result = _innerReader.ReadAsDecimal();
            _textWriter.WriteToken(_innerReader, false, false, true);
            return result;
        }

        public override double? ReadAsDouble()
        {
            double? result = _innerReader.ReadAsDouble();
            _textWriter.WriteToken(_innerReader, false, false, true);
            return result;
        }

        public override bool? ReadAsBoolean()
        {
            bool? result = _innerReader.ReadAsBoolean();
            _textWriter.WriteToken(_innerReader, false, false, true);
            return result;
        }

        public override DateTime? ReadAsDateTime()
        {
            DateTime? result = _innerReader.ReadAsDateTime();
            _textWriter.WriteToken(_innerReader, false, false, true);
            return result;
        }

        public override DateTimeOffset? ReadAsDateTimeOffset()
        {
            DateTimeOffset? result = _innerReader.ReadAsDateTimeOffset();
            _textWriter.WriteToken(_innerReader, false, false, true);
            return result;
        }

        public override int Depth
        {
            get
            {
                return _innerReader.Depth;
            }
        }

        public override string Path
        {
            get
            {
                return _innerReader.Path;
            }
        }

        public override char QuoteChar
        {
            get
            {
                return _innerReader.QuoteChar;
            }
            protected internal set
            {
                _innerReader.QuoteChar = value;
            }
        }

        public override JsonToken TokenType
        {
            get
            {
                return _innerReader.TokenType;
            }
        }

        public override object Value
        {
            get
            {
                return _innerReader.Value;
            }
        }

        public override Type ValueType
        {
            get
            {
                return _innerReader.ValueType;
            }
        }

        public override void Close()
        {
            _innerReader.Close();
        }

        bool IJsonLineInfo.HasLineInfo()
        {
            IJsonLineInfo jsonLineInfo = _innerReader as IJsonLineInfo;
            return jsonLineInfo != null && jsonLineInfo.HasLineInfo();
        }

        int IJsonLineInfo.LineNumber
        {
            get
            {
                IJsonLineInfo jsonLineInfo = _innerReader as IJsonLineInfo;
                if (jsonLineInfo == null)
                {
                    return 0;
                }
                return jsonLineInfo.LineNumber;
            }
        }

        int IJsonLineInfo.LinePosition
        {
            get
            {
                IJsonLineInfo jsonLineInfo = _innerReader as IJsonLineInfo;
                if (jsonLineInfo == null)
                {
                    return 0;
                }
                return jsonLineInfo.LinePosition;
            }
        }

        private readonly JsonReader _innerReader;

        private readonly JsonTextWriter _textWriter;

        private readonly StringWriter _sw;
    }
}
