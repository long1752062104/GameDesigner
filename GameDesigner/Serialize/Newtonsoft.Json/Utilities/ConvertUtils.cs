using Newtonsoft_X.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Newtonsoft_X.Json.Utilities
{
    internal static class ConvertUtils
    {
        public static PrimitiveTypeCode GetTypeCode(Type t)
        {
            return ConvertUtils.GetTypeCode(t, out bool flag);
        }

        public static PrimitiveTypeCode GetTypeCode(Type t, out bool isEnum)
        {
            if (ConvertUtils.TypeCodeMap.TryGetValue(t, out PrimitiveTypeCode result))
            {
                isEnum = false;
                return result;
            }
            if (t.IsEnum())
            {
                isEnum = true;
                return ConvertUtils.GetTypeCode(Enum.GetUnderlyingType(t));
            }
            if (ReflectionUtils.IsNullableType(t))
            {
                Type underlyingType = Nullable.GetUnderlyingType(t);
                if (underlyingType.IsEnum())
                {
                    Type t2 = typeof(Nullable<>).MakeGenericType(new Type[]
                    {
                        Enum.GetUnderlyingType(underlyingType)
                    });
                    isEnum = true;
                    return ConvertUtils.GetTypeCode(t2);
                }
            }
            isEnum = false;
            return PrimitiveTypeCode.Object;
        }

        public static TypeInformation GetTypeInformation(IConvertible convertable)
        {
            return ConvertUtils.PrimitiveTypeCodes[(int)convertable.GetTypeCode()];
        }

        public static bool IsConvertible(Type t)
        {
            return typeof(IConvertible).IsAssignableFrom(t);
        }

        public static TimeSpan ParseTimeSpan(string input)
        {
            return TimeSpan.Parse(input);
        }

        private static Func<object, object> CreateCastConverter(ConvertUtils.TypeConvertKey t)
        {
            MethodInfo method = t.TargetType.GetMethod("op_Implicit", new Type[]
            {
                t.InitialType
            });
            if (method == null)
            {
                method = t.TargetType.GetMethod("op_Explicit", new Type[]
                {
                    t.InitialType
                });
            }
            if (method == null)
            {
                return null;
            }
            MethodCall<object, object> call = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
            return (object o) => call(null, new object[]
            {
                o
            });
        }

        public static object Convert(object initialValue, CultureInfo culture, Type targetType)
        {
            switch (ConvertUtils.TryConvertInternal(initialValue, culture, targetType, out object result))
            {
                case ConvertUtils.ConvertResult.Success:
                    return result;
                case ConvertUtils.ConvertResult.CannotConvertNull:
                    throw new Exception("Can not convert null {0} into non-nullable {1}.".FormatWith(CultureInfo.InvariantCulture, initialValue.GetType(), targetType));
                case ConvertUtils.ConvertResult.NotInstantiableType:
                    throw new ArgumentException("Target type {0} is not a value type or a non-abstract class.".FormatWith(CultureInfo.InvariantCulture, targetType), "targetType");
                case ConvertUtils.ConvertResult.NoValidConversion:
                    throw new InvalidOperationException("Can not convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, initialValue.GetType(), targetType));
                default:
                    throw new InvalidOperationException("Unexpected conversion result.");
            }
        }

        private static bool TryConvert(object initialValue, CultureInfo culture, Type targetType, out object value)
        {
            bool result;
            try
            {
                if (ConvertUtils.TryConvertInternal(initialValue, culture, targetType, out value) == ConvertUtils.ConvertResult.Success)
                {
                    result = true;
                }
                else
                {
                    value = null;
                    result = false;
                }
            }
            catch
            {
                value = null;
                result = false;
            }
            return result;
        }

        private static ConvertUtils.ConvertResult TryConvertInternal(object initialValue, CultureInfo culture, Type targetType, out object value)
        {
            if (initialValue == null)
            {
                throw new ArgumentNullException("initialValue");
            }
            if (ReflectionUtils.IsNullableType(targetType))
            {
                targetType = Nullable.GetUnderlyingType(targetType);
            }
            Type type = initialValue.GetType();
            if (targetType == type)
            {
                value = initialValue;
                return ConvertUtils.ConvertResult.Success;
            }
            if (ConvertUtils.IsConvertible(initialValue.GetType()) && ConvertUtils.IsConvertible(targetType))
            {
                if (targetType.IsEnum())
                {
                    if (initialValue is string)
                    {
                        value = Enum.Parse(targetType, initialValue.ToString(), true);
                        return ConvertUtils.ConvertResult.Success;
                    }
                    if (ConvertUtils.IsInteger(initialValue))
                    {
                        value = Enum.ToObject(targetType, initialValue);
                        return ConvertUtils.ConvertResult.Success;
                    }
                }
                value = System.Convert.ChangeType(initialValue, targetType, culture);
                return ConvertUtils.ConvertResult.Success;
            }
            if (initialValue is DateTime && targetType == typeof(DateTimeOffset))
            {
                value = new DateTimeOffset((DateTime)initialValue);
                return ConvertUtils.ConvertResult.Success;
            }
            if (initialValue is byte[] && targetType == typeof(Guid))
            {
                value = new Guid((byte[])initialValue);
                return ConvertUtils.ConvertResult.Success;
            }
            if (initialValue is Guid && targetType == typeof(byte[]))
            {
                value = ((Guid)initialValue).ToByteArray();
                return ConvertUtils.ConvertResult.Success;
            }
            string text = initialValue as string;
            if (text != null)
            {
                if (targetType == typeof(Guid))
                {
                    value = new Guid(text);
                    return ConvertUtils.ConvertResult.Success;
                }
                if (targetType == typeof(Uri))
                {
                    value = new Uri(text, UriKind.RelativeOrAbsolute);
                    return ConvertUtils.ConvertResult.Success;
                }
                if (targetType == typeof(TimeSpan))
                {
                    value = ConvertUtils.ParseTimeSpan(text);
                    return ConvertUtils.ConvertResult.Success;
                }
                if (targetType == typeof(byte[]))
                {
                    value = System.Convert.FromBase64String(text);
                    return ConvertUtils.ConvertResult.Success;
                }
                if (targetType == typeof(Version))
                {
                    if (ConvertUtils.VersionTryParse(text, out Version version))
                    {
                        value = version;
                        return ConvertUtils.ConvertResult.Success;
                    }
                    value = null;
                    return ConvertUtils.ConvertResult.NoValidConversion;
                }
                else if (typeof(Type).IsAssignableFrom(targetType))
                {
                    value = Type.GetType(text, true);
                    return ConvertUtils.ConvertResult.Success;
                }
            }
            TypeConverter converter = ConvertUtils.GetConverter(type);
            if (converter != null && converter.CanConvertTo(targetType))
            {
                value = converter.ConvertTo(null, culture, initialValue, targetType);
                return ConvertUtils.ConvertResult.Success;
            }
            TypeConverter converter2 = ConvertUtils.GetConverter(targetType);
            if (converter2 != null && converter2.CanConvertFrom(type))
            {
                value = converter2.ConvertFrom(null, culture, initialValue);
                return ConvertUtils.ConvertResult.Success;
            }
            if (initialValue == DBNull.Value)
            {
                if (ReflectionUtils.IsNullable(targetType))
                {
                    value = ConvertUtils.EnsureTypeAssignable(null, type, targetType);
                    return ConvertUtils.ConvertResult.Success;
                }
                value = null;
                return ConvertUtils.ConvertResult.CannotConvertNull;
            }
            else
            {
                if (targetType.IsInterface() || targetType.IsGenericTypeDefinition() || targetType.IsAbstract())
                {
                    value = null;
                    return ConvertUtils.ConvertResult.NotInstantiableType;
                }
                value = null;
                return ConvertUtils.ConvertResult.NoValidConversion;
            }
        }

        /// <summary>
        /// Converts the value to the specified type. If the value is unable to be converted, the
        /// value is checked whether it assignable to the specified type.
        /// </summary>
        /// <param name="initialValue">The value to convert.</param>
        /// <param name="culture">The culture to use when converting.</param>
        /// <param name="targetType">The type to convert or cast the value to.</param>
        /// <returns>
        /// The converted type. If conversion was unsuccessful, the initial value
        /// is returned if assignable to the target type.
        /// </returns>
        public static object ConvertOrCast(object initialValue, CultureInfo culture, Type targetType)
        {
            if (targetType == typeof(object))
            {
                return initialValue;
            }
            if (initialValue == null && ReflectionUtils.IsNullable(targetType))
            {
                return null;
            }
            if (ConvertUtils.TryConvert(initialValue, culture, targetType, out object result))
            {
                return result;
            }
            return ConvertUtils.EnsureTypeAssignable(initialValue, ReflectionUtils.GetObjectType(initialValue), targetType);
        }

        private static object EnsureTypeAssignable(object value, Type initialType, Type targetType)
        {
            Type type = (value != null) ? value.GetType() : null;
            if (value != null)
            {
                if (targetType.IsAssignableFrom(type))
                {
                    return value;
                }
                Func<object, object> func = ConvertUtils.CastConverters.Get(new ConvertUtils.TypeConvertKey(type, targetType));
                if (func != null)
                {
                    return func(value);
                }
            }
            else if (ReflectionUtils.IsNullable(targetType))
            {
                return null;
            }
            throw new ArgumentException("Could not cast or convert from {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, (initialType != null) ? initialType.ToString() : "{null}", targetType));
        }

        internal static TypeConverter GetConverter(Type t)
        {
            return JsonTypeReflector.GetTypeConverter(t);
        }

        public static bool VersionTryParse(string input, out Version result)
        {
            bool result2;
            try
            {
                result = new Version(input);
                result2 = true;
            }
            catch
            {
                result = null;
                result2 = false;
            }
            return result2;
        }

        public static bool IsInteger(object value)
        {
            switch (ConvertUtils.GetTypeCode(value.GetType()))
            {
                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.UInt32:
                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                    return true;
            }
            return false;
        }

        public static ParseResult Int32TryParse(char[] chars, int start, int length, out int value)
        {
            value = 0;
            if (length == 0)
            {
                return ParseResult.Invalid;
            }
            bool flag = chars[start] == '-';
            if (flag)
            {
                if (length == 1)
                {
                    return ParseResult.Invalid;
                }
                start++;
                length--;
            }
            int num = start + length;
            if (length > 10 || (length == 10 && chars[start] - '0' > '\u0002'))
            {
                for (int i = start; i < num; i++)
                {
                    int num2 = chars[i] - '0';
                    if (num2 < 0 || num2 > 9)
                    {
                        return ParseResult.Invalid;
                    }
                }
                return ParseResult.Overflow;
            }
            for (int j = start; j < num; j++)
            {
                int num3 = chars[j] - '0';
                if (num3 < 0 || num3 > 9)
                {
                    return ParseResult.Invalid;
                }
                int num4 = 10 * value - num3;
                if (num4 > value)
                {
                    for (j++; j < num; j++)
                    {
                        num3 = chars[j] - '0';
                        if (num3 < 0 || num3 > 9)
                        {
                            return ParseResult.Invalid;
                        }
                    }
                    return ParseResult.Overflow;
                }
                value = num4;
            }
            if (!flag)
            {
                if (value == -2147483648)
                {
                    return ParseResult.Overflow;
                }
                value = -value;
            }
            return ParseResult.Success;
        }

        public static ParseResult Int64TryParse(char[] chars, int start, int length, out long value)
        {
            value = 0L;
            if (length == 0)
            {
                return ParseResult.Invalid;
            }
            bool flag = chars[start] == '-';
            if (flag)
            {
                if (length == 1)
                {
                    return ParseResult.Invalid;
                }
                start++;
                length--;
            }
            int num = start + length;
            if (length > 19)
            {
                for (int i = start; i < num; i++)
                {
                    int num2 = chars[i] - '0';
                    if (num2 < 0 || num2 > 9)
                    {
                        return ParseResult.Invalid;
                    }
                }
                return ParseResult.Overflow;
            }
            for (int j = start; j < num; j++)
            {
                int num3 = chars[j] - '0';
                if (num3 < 0 || num3 > 9)
                {
                    return ParseResult.Invalid;
                }
                long num4 = 10L * value - num3;
                if (num4 > value)
                {
                    for (j++; j < num; j++)
                    {
                        num3 = chars[j] - '0';
                        if (num3 < 0 || num3 > 9)
                        {
                            return ParseResult.Invalid;
                        }
                    }
                    return ParseResult.Overflow;
                }
                value = num4;
            }
            if (!flag)
            {
                if (value == -9223372036854775808L)
                {
                    return ParseResult.Overflow;
                }
                value = -value;
            }
            return ParseResult.Success;
        }

        public static bool TryConvertGuid(string s, out Guid g)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (new Regex("^[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}$").Match(s).Success)
            {
                g = new Guid(s);
                return true;
            }
            g = Guid.Empty;
            return false;
        }

        public static int HexTextToInt(char[] text, int start, int end)
        {
            int num = 0;
            for (int i = start; i < end; i++)
            {
                num += ConvertUtils.HexCharToInt(text[i]) << (end - 1 - i) * 4;
            }
            return num;
        }

        private static int HexCharToInt(char ch)
        {
            if (ch <= '9' && ch >= '0')
            {
                return ch - '0';
            }
            if (ch <= 'F' && ch >= 'A')
            {
                return ch - '7';
            }
            if (ch <= 'f' && ch >= 'a')
            {
                return ch - 'W';
            }
            throw new FormatException("Invalid hex character: " + ch.ToString());
        }

        private static readonly Dictionary<Type, PrimitiveTypeCode> TypeCodeMap = new Dictionary<Type, PrimitiveTypeCode>
        {
            {
                typeof(char),
                PrimitiveTypeCode.Char
            },
            {
                typeof(char?),
                PrimitiveTypeCode.CharNullable
            },
            {
                typeof(bool),
                PrimitiveTypeCode.Boolean
            },
            {
                typeof(bool?),
                PrimitiveTypeCode.BooleanNullable
            },
            {
                typeof(sbyte),
                PrimitiveTypeCode.SByte
            },
            {
                typeof(sbyte?),
                PrimitiveTypeCode.SByteNullable
            },
            {
                typeof(short),
                PrimitiveTypeCode.Int16
            },
            {
                typeof(short?),
                PrimitiveTypeCode.Int16Nullable
            },
            {
                typeof(ushort),
                PrimitiveTypeCode.UInt16
            },
            {
                typeof(ushort?),
                PrimitiveTypeCode.UInt16Nullable
            },
            {
                typeof(int),
                PrimitiveTypeCode.Int32
            },
            {
                typeof(int?),
                PrimitiveTypeCode.Int32Nullable
            },
            {
                typeof(byte),
                PrimitiveTypeCode.Byte
            },
            {
                typeof(byte?),
                PrimitiveTypeCode.ByteNullable
            },
            {
                typeof(uint),
                PrimitiveTypeCode.UInt32
            },
            {
                typeof(uint?),
                PrimitiveTypeCode.UInt32Nullable
            },
            {
                typeof(long),
                PrimitiveTypeCode.Int64
            },
            {
                typeof(long?),
                PrimitiveTypeCode.Int64Nullable
            },
            {
                typeof(ulong),
                PrimitiveTypeCode.UInt64
            },
            {
                typeof(ulong?),
                PrimitiveTypeCode.UInt64Nullable
            },
            {
                typeof(float),
                PrimitiveTypeCode.Single
            },
            {
                typeof(float?),
                PrimitiveTypeCode.SingleNullable
            },
            {
                typeof(double),
                PrimitiveTypeCode.Double
            },
            {
                typeof(double?),
                PrimitiveTypeCode.DoubleNullable
            },
            {
                typeof(DateTime),
                PrimitiveTypeCode.DateTime
            },
            {
                typeof(DateTime?),
                PrimitiveTypeCode.DateTimeNullable
            },
            {
                typeof(DateTimeOffset),
                PrimitiveTypeCode.DateTimeOffset
            },
            {
                typeof(DateTimeOffset?),
                PrimitiveTypeCode.DateTimeOffsetNullable
            },
            {
                typeof(decimal),
                PrimitiveTypeCode.Decimal
            },
            {
                typeof(decimal?),
                PrimitiveTypeCode.DecimalNullable
            },
            {
                typeof(Guid),
                PrimitiveTypeCode.Guid
            },
            {
                typeof(Guid?),
                PrimitiveTypeCode.GuidNullable
            },
            {
                typeof(TimeSpan),
                PrimitiveTypeCode.TimeSpan
            },
            {
                typeof(TimeSpan?),
                PrimitiveTypeCode.TimeSpanNullable
            },
            {
                typeof(Uri),
                PrimitiveTypeCode.Uri
            },
            {
                typeof(string),
                PrimitiveTypeCode.String
            },
            {
                typeof(byte[]),
                PrimitiveTypeCode.Bytes
            },
            {
                typeof(DBNull),
                PrimitiveTypeCode.DBNull
            }
        };

        private static readonly TypeInformation[] PrimitiveTypeCodes = new TypeInformation[]
        {
            new TypeInformation
            {
                Type = typeof(object),
                TypeCode = PrimitiveTypeCode.Empty
            },
            new TypeInformation
            {
                Type = typeof(object),
                TypeCode = PrimitiveTypeCode.Object
            },
            new TypeInformation
            {
                Type = typeof(object),
                TypeCode = PrimitiveTypeCode.DBNull
            },
            new TypeInformation
            {
                Type = typeof(bool),
                TypeCode = PrimitiveTypeCode.Boolean
            },
            new TypeInformation
            {
                Type = typeof(char),
                TypeCode = PrimitiveTypeCode.Char
            },
            new TypeInformation
            {
                Type = typeof(sbyte),
                TypeCode = PrimitiveTypeCode.SByte
            },
            new TypeInformation
            {
                Type = typeof(byte),
                TypeCode = PrimitiveTypeCode.Byte
            },
            new TypeInformation
            {
                Type = typeof(short),
                TypeCode = PrimitiveTypeCode.Int16
            },
            new TypeInformation
            {
                Type = typeof(ushort),
                TypeCode = PrimitiveTypeCode.UInt16
            },
            new TypeInformation
            {
                Type = typeof(int),
                TypeCode = PrimitiveTypeCode.Int32
            },
            new TypeInformation
            {
                Type = typeof(uint),
                TypeCode = PrimitiveTypeCode.UInt32
            },
            new TypeInformation
            {
                Type = typeof(long),
                TypeCode = PrimitiveTypeCode.Int64
            },
            new TypeInformation
            {
                Type = typeof(ulong),
                TypeCode = PrimitiveTypeCode.UInt64
            },
            new TypeInformation
            {
                Type = typeof(float),
                TypeCode = PrimitiveTypeCode.Single
            },
            new TypeInformation
            {
                Type = typeof(double),
                TypeCode = PrimitiveTypeCode.Double
            },
            new TypeInformation
            {
                Type = typeof(decimal),
                TypeCode = PrimitiveTypeCode.Decimal
            },
            new TypeInformation
            {
                Type = typeof(DateTime),
                TypeCode = PrimitiveTypeCode.DateTime
            },
            new TypeInformation
            {
                Type = typeof(object),
                TypeCode = PrimitiveTypeCode.Empty
            },
            new TypeInformation
            {
                Type = typeof(string),
                TypeCode = PrimitiveTypeCode.String
            }
        };

        private static readonly ThreadSafeStore<ConvertUtils.TypeConvertKey, Func<object, object>> CastConverters = new ThreadSafeStore<ConvertUtils.TypeConvertKey, Func<object, object>>(new Func<ConvertUtils.TypeConvertKey, Func<object, object>>(ConvertUtils.CreateCastConverter));

        internal struct TypeConvertKey : IEquatable<ConvertUtils.TypeConvertKey>
        {
            public Type InitialType
            {
                get
                {
                    return _initialType;
                }
            }

            public Type TargetType
            {
                get
                {
                    return _targetType;
                }
            }

            public TypeConvertKey(Type initialType, Type targetType)
            {
                _initialType = initialType;
                _targetType = targetType;
            }

            public override int GetHashCode()
            {
                return _initialType.GetHashCode() ^ _targetType.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is ConvertUtils.TypeConvertKey && Equals((ConvertUtils.TypeConvertKey)obj);
            }

            public bool Equals(ConvertUtils.TypeConvertKey other)
            {
                return _initialType == other._initialType && _targetType == other._targetType;
            }

            private readonly Type _initialType;

            private readonly Type _targetType;
        }

        internal enum ConvertResult
        {
            Success,
            CannotConvertNull,
            NotInstantiableType,
            NoValidConversion
        }
    }
}
