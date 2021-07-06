using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Newtonsoft_X.Json.Linq
{
    /// <summary>
    /// Represents a value in JSON (string, integer, date, etc).
    /// </summary>
    public class JValue : JToken, IEquatable<JValue>, IFormattable, IComparable, IComparable<JValue>, IConvertible
    {
        internal JValue(object value, JTokenType type)
        {
            _value = value;
            _valueType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class from another <see cref="T:Newtonsoft.Json.Linq.JValue" /> object.
        /// </summary>
        /// <param name="other">A <see cref="T:Newtonsoft.Json.Linq.JValue" /> object to copy from.</param>
        public JValue(JValue other) : this(other.Value, other.Type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(long value) : this(value, JTokenType.Integer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(decimal value) : this(value, JTokenType.Float)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(char value) : this(value, JTokenType.String)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        //[CLSCompliant(false)]
        public JValue(ulong value) : this(value, JTokenType.Integer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(double value) : this(value, JTokenType.Float)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(float value) : this(value, JTokenType.Float)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(DateTime value) : this(value, JTokenType.Date)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(DateTimeOffset value) : this(value, JTokenType.Date)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(bool value) : this(value, JTokenType.Boolean)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(string value) : this(value, JTokenType.String)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(Guid value) : this(value, JTokenType.Guid)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(Uri value) : this(value, (value != null) ? JTokenType.Uri : JTokenType.Null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(TimeSpan value) : this(value, JTokenType.TimeSpan)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        public JValue(object value) : this(value, JValue.GetValueType(null, value))
        {
        }

        internal override bool DeepEquals(JToken node)
        {
            JValue jvalue = node as JValue;
            return jvalue != null && (jvalue == this || JValue.ValuesEquals(this, jvalue));
        }

        /// <summary>
        /// Gets a value indicating whether this token has child tokens.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this token has child values; otherwise, <c>false</c>.
        /// </value>
        public override bool HasValues
        {
            get
            {
                return false;
            }
        }

        internal static int Compare(JTokenType valueType, object objA, object objB)
        {
            if (objA == null && objB == null)
            {
                return 0;
            }
            if (objA != null && objB == null)
            {
                return 1;
            }
            if (objA == null && objB != null)
            {
                return -1;
            }
            switch (valueType)
            {
                case JTokenType.Comment:
                case JTokenType.String:
                case JTokenType.Raw:
                    {
                        string strA = Convert.ToString(objA, CultureInfo.InvariantCulture);
                        string strB = Convert.ToString(objB, CultureInfo.InvariantCulture);
                        return string.CompareOrdinal(strA, strB);
                    }
                case JTokenType.Integer:
                    if (objA is ulong || objB is ulong || objA is decimal || objB is decimal)
                    {
                        return Convert.ToDecimal(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToDecimal(objB, CultureInfo.InvariantCulture));
                    }
                    if (objA is float || objB is float || objA is double || objB is double)
                    {
                        return JValue.CompareFloat(objA, objB);
                    }
                    return Convert.ToInt64(objA, CultureInfo.InvariantCulture).CompareTo(Convert.ToInt64(objB, CultureInfo.InvariantCulture));
                case JTokenType.Float:
                    return JValue.CompareFloat(objA, objB);
                case JTokenType.Boolean:
                    {
                        bool flag = Convert.ToBoolean(objA, CultureInfo.InvariantCulture);
                        bool value = Convert.ToBoolean(objB, CultureInfo.InvariantCulture);
                        return flag.CompareTo(value);
                    }
                case JTokenType.Date:
                    {
                        if (objA is DateTime)
                        {
                            DateTime dateTime = (DateTime)objA;
                            DateTime value2;
                            if (objB is DateTimeOffset)
                            {
                                value2 = ((DateTimeOffset)objB).DateTime;
                            }
                            else
                            {
                                value2 = Convert.ToDateTime(objB, CultureInfo.InvariantCulture);
                            }
                            return dateTime.CompareTo(value2);
                        }
                        DateTimeOffset dateTimeOffset = (DateTimeOffset)objA;
                        DateTimeOffset other;
                        if (objB is DateTimeOffset)
                        {
                            other = (DateTimeOffset)objB;
                        }
                        else
                        {
                            other = new DateTimeOffset(Convert.ToDateTime(objB, CultureInfo.InvariantCulture));
                        }
                        return dateTimeOffset.CompareTo(other);
                    }
                case JTokenType.Bytes:
                    {
                        if (!(objB is byte[]))
                        {
                            throw new ArgumentException("Object must be of type byte[].");
                        }
                        byte[] array = objA as byte[];
                        byte[] array2 = objB as byte[];
                        if (array == null)
                        {
                            return -1;
                        }
                        if (array2 == null)
                        {
                            return 1;
                        }
                        return MiscellaneousUtils.ByteArrayCompare(array, array2);
                    }
                case JTokenType.Guid:
                    {
                        if (!(objB is Guid))
                        {
                            throw new ArgumentException("Object must be of type Guid.");
                        }
                        Guid guid = (Guid)objA;
                        Guid value3 = (Guid)objB;
                        return guid.CompareTo(value3);
                    }
                case JTokenType.Uri:
                    {
                        if (!(objB is Uri))
                        {
                            throw new ArgumentException("Object must be of type Uri.");
                        }
                        Uri uri = (Uri)objA;
                        Uri uri2 = (Uri)objB;
                        return Comparer<string>.Default.Compare(uri.ToString(), uri2.ToString());
                    }
                case JTokenType.TimeSpan:
                    {
                        if (!(objB is TimeSpan))
                        {
                            throw new ArgumentException("Object must be of type TimeSpan.");
                        }
                        TimeSpan timeSpan = (TimeSpan)objA;
                        TimeSpan value4 = (TimeSpan)objB;
                        return timeSpan.CompareTo(value4);
                    }
            }
            throw MiscellaneousUtils.CreateArgumentOutOfRangeException("valueType", valueType, "Unexpected value type: {0}".FormatWith(CultureInfo.InvariantCulture, valueType));
        }

        private static int CompareFloat(object objA, object objB)
        {
            double d = Convert.ToDouble(objA, CultureInfo.InvariantCulture);
            double num = Convert.ToDouble(objB, CultureInfo.InvariantCulture);
            if (MathUtils.ApproxEquals(d, num))
            {
                return 0;
            }
            return d.CompareTo(num);
        }

        internal override JToken CloneToken()
        {
            return new JValue(this);
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JValue" /> comment with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JValue" /> comment with the given value.</returns>
        public static JValue CreateComment(string value)
        {
            return new JValue(value, JTokenType.Comment);
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JValue" /> string with the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JValue" /> string with the given value.</returns>
        public static JValue CreateString(string value)
        {
            return new JValue(value, JTokenType.String);
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JValue" /> null value.
        /// </summary>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JValue" /> null value.</returns>
        public static JValue CreateNull()
        {
            return new JValue(null, JTokenType.Null);
        }

        /// <summary>
        /// Creates a <see cref="T:Newtonsoft.Json.Linq.JValue" /> undefined value.
        /// </summary>
        /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JValue" /> undefined value.</returns>
        public static JValue CreateUndefined()
        {
            return new JValue(null, JTokenType.Undefined);
        }

        private static JTokenType GetValueType(JTokenType? current, object value)
        {
            if (value == null)
            {
                return JTokenType.Null;
            }
            if (value == DBNull.Value)
            {
                return JTokenType.Null;
            }
            if (value is string)
            {
                return JValue.GetStringValueType(current);
            }
            if (value is long || value is int || value is short || value is sbyte || value is ulong || value is uint || value is ushort || value is byte)
            {
                return JTokenType.Integer;
            }
            if (value is Enum)
            {
                return JTokenType.Integer;
            }
            if (value is double || value is float || value is decimal)
            {
                return JTokenType.Float;
            }
            if (value is DateTime)
            {
                return JTokenType.Date;
            }
            if (value is DateTimeOffset)
            {
                return JTokenType.Date;
            }
            if (value is byte[])
            {
                return JTokenType.Bytes;
            }
            if (value is bool)
            {
                return JTokenType.Boolean;
            }
            if (value is Guid)
            {
                return JTokenType.Guid;
            }
            if (value is Uri)
            {
                return JTokenType.Uri;
            }
            if (value is TimeSpan)
            {
                return JTokenType.TimeSpan;
            }
            throw new ArgumentException("Could not determine JSON object type for type {0}.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
        }

        private static JTokenType GetStringValueType(JTokenType? current)
        {
            if (current == null)
            {
                return JTokenType.String;
            }
            JTokenType valueOrDefault = current.GetValueOrDefault();
            if (valueOrDefault == JTokenType.Comment || valueOrDefault == JTokenType.String || valueOrDefault == JTokenType.Raw)
            {
                return current.GetValueOrDefault();
            }
            return JTokenType.String;
        }

        /// <summary>
        /// Gets the node type for this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
        /// </summary>
        /// <value>The type.</value>
        public override JTokenType Type
        {
            get
            {
                return _valueType;
            }
        }

        /// <summary>
        /// Gets or sets the underlying token value.
        /// </summary>
        /// <value>The underlying token value.</value>
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                Type type = (_value != null) ? _value.GetType() : null;
                Type type2 = (value != null) ? value.GetType() : null;
                if (type != type2)
                {
                    _valueType = JValue.GetValueType(new JTokenType?(_valueType), value);
                }
                _value = value;
            }
        }

        /// <summary>
        /// Writes this token to a <see cref="T:Newtonsoft.Json.JsonWriter" />.
        /// </summary>
        /// <param name="writer">A <see cref="T:Newtonsoft.Json.JsonWriter" /> into which this method will write.</param>
        /// <param name="converters">A collection of <see cref="T:Newtonsoft.Json.JsonConverter" />s which will be used when writing the token.</param>
        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            if (converters != null && converters.Length != 0 && _value != null)
            {
                JsonConverter matchingConverter = JsonSerializer.GetMatchingConverter(converters, _value.GetType());
                if (matchingConverter != null && matchingConverter.CanWrite)
                {
                    matchingConverter.WriteJson(writer, _value, JsonSerializer.CreateDefault());
                    return;
                }
            }
            switch (_valueType)
            {
                case JTokenType.Comment:
                    writer.WriteComment((_value != null) ? _value.ToString() : null);
                    return;
                case JTokenType.Integer:
                    if (_value is int)
                    {
                        writer.WriteValue((int)_value);
                        return;
                    }
                    if (_value is long)
                    {
                        writer.WriteValue((long)_value);
                        return;
                    }
                    if (_value is ulong)
                    {
                        writer.WriteValue((ulong)_value);
                        return;
                    }
                    writer.WriteValue(Convert.ToInt64(_value, CultureInfo.InvariantCulture));
                    return;
                case JTokenType.Float:
                    if (_value is decimal)
                    {
                        writer.WriteValue((decimal)_value);
                        return;
                    }
                    if (_value is double)
                    {
                        writer.WriteValue((double)_value);
                        return;
                    }
                    if (_value is float)
                    {
                        writer.WriteValue((float)_value);
                        return;
                    }
                    writer.WriteValue(Convert.ToDouble(_value, CultureInfo.InvariantCulture));
                    return;
                case JTokenType.String:
                    writer.WriteValue((_value != null) ? _value.ToString() : null);
                    return;
                case JTokenType.Boolean:
                    writer.WriteValue(Convert.ToBoolean(_value, CultureInfo.InvariantCulture));
                    return;
                case JTokenType.Null:
                    writer.WriteNull();
                    return;
                case JTokenType.Undefined:
                    writer.WriteUndefined();
                    return;
                case JTokenType.Date:
                    if (_value is DateTimeOffset)
                    {
                        writer.WriteValue((DateTimeOffset)_value);
                        return;
                    }
                    writer.WriteValue(Convert.ToDateTime(_value, CultureInfo.InvariantCulture));
                    return;
                case JTokenType.Raw:
                    writer.WriteRawValue((_value != null) ? _value.ToString() : null);
                    return;
                case JTokenType.Bytes:
                    writer.WriteValue((byte[])_value);
                    return;
                case JTokenType.Guid:
                    writer.WriteValue((_value != null) ? ((Guid?)_value) : null);
                    return;
                case JTokenType.Uri:
                    writer.WriteValue((Uri)_value);
                    return;
                case JTokenType.TimeSpan:
                    writer.WriteValue((_value != null) ? ((TimeSpan?)_value) : null);
                    return;
                default:
                    throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", _valueType, "Unexpected token type.");
            }
        }

        internal override int GetDeepHashCode()
        {
            int num = (_value != null) ? _value.GetHashCode() : 0;
            int valueType = (int)_valueType;
            return valueType.GetHashCode() ^ num;
        }

        private static bool ValuesEquals(JValue v1, JValue v2)
        {
            return v1 == v2 || (v1._valueType == v2._valueType && JValue.Compare(v1._valueType, v1._value, v2._value) == 0);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(JValue other)
        {
            return other != null && JValue.ValuesEquals(this, other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            JValue jvalue = obj as JValue;
            if (jvalue != null)
            {
                return Equals(jvalue);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            if (_value == null)
            {
                return 0;
            }
            return _value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <remarks>
        /// <c>ToString()</c> returns a non-JSON string value for tokens with a type of <see cref="F:Newtonsoft.Json.Linq.JTokenType.String" />.
        /// If you want the JSON for all token types then you should use <see cref="M:Newtonsoft.Json.Linq.JValue.WriteTo(Newtonsoft.Json.JsonWriter,Newtonsoft.Json.JsonConverter[])" />.
        /// </remarks>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (_value == null)
            {
                return string.Empty;
            }
            return _value.ToString();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (_value == null)
            {
                return string.Empty;
            }
            IFormattable formattable = _value as IFormattable;
            if (formattable != null)
            {
                return formattable.ToString(format, formatProvider);
            }
            return _value.ToString();
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            object objB = (obj is JValue) ? ((JValue)obj).Value : obj;
            return JValue.Compare(_valueType, _value, objB);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance is less than <paramref name="obj" />.
        /// Zero
        /// This instance is equal to <paramref name="obj" />.
        /// Greater than zero
        /// This instance is greater than <paramref name="obj" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="obj" /> is not of the same type as this instance.
        /// </exception>
        public int CompareTo(JValue obj)
        {
            if (obj == null)
            {
                return 1;
            }
            return JValue.Compare(_valueType, _value, obj._value);
        }

        TypeCode IConvertible.GetTypeCode()
        {
            if (_value == null)
            {
                return TypeCode.Empty;
            }
            IConvertible convertible = _value as IConvertible;
            if (convertible == null)
            {
                return TypeCode.Object;
            }
            return convertible.GetTypeCode();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return (bool)this;
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return (char)this;
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return (sbyte)this;
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return (byte)this;
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return (short)this;
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return (ushort)this;
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return (int)this;
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return (uint)this;
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return (long)this;
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return (ulong)this;
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return (float)this;
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return (double)this;
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return (decimal)this;
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return (DateTime)this;
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return base.ToObject(conversionType);
        }

        private JTokenType _valueType;

        private object _value;
    }
}
