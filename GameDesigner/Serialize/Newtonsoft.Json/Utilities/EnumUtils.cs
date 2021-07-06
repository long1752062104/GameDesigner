using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Newtonsoft_X.Json.Utilities
{
    internal static class EnumUtils
    {
        private static BidirectionalDictionary<string, string> InitializeEnumType(Type type)
        {
            BidirectionalDictionary<string, string> bidirectionalDictionary = new BidirectionalDictionary<string, string>(StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                string name = fieldInfo.Name;
                string text = (from EnumMemberAttribute a in fieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), true)
                               select a.Value).SingleOrDefault<string>() ?? fieldInfo.Name;
                if (bidirectionalDictionary.TryGetBySecond(text, out string text2))
                {
                    throw new InvalidOperationException("Enum name '{0}' already exists on enum '{1}'.".FormatWith(CultureInfo.InvariantCulture, text, type.Name));
                }
                bidirectionalDictionary.Set(name, text);
            }
            return bidirectionalDictionary;
        }

        public static IList<T> GetFlagsValues<T>(T value) where T : struct
        {
            Type typeFromHandle = typeof(T);
            if (!typeFromHandle.IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException("Enum type {0} is not a set of flags.".FormatWith(CultureInfo.InvariantCulture, typeFromHandle));
            }
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());
            ulong num = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            IList<EnumValue<ulong>> namesAndValues = EnumUtils.GetNamesAndValues<T>();
            IList<T> list = new List<T>();
            foreach (EnumValue<ulong> enumValue in namesAndValues)
            {
                if ((num & enumValue.Value) == enumValue.Value && enumValue.Value != 0UL)
                {
                    list.Add((T)Convert.ChangeType(enumValue.Value, underlyingType, CultureInfo.CurrentCulture));
                }
            }
            if (list.Count == 0)
            {
                if (namesAndValues.SingleOrDefault((EnumValue<ulong> v) => v.Value == 0UL) != null)
                {
                    list.Add(default(T));
                }
            }
            return list;
        }

        public static IList<EnumValue<ulong>> GetNamesAndValues<T>() where T : struct
        {
            return EnumUtils.GetNamesAndValues<ulong>(typeof(T));
        }

        public static IList<EnumValue<TUnderlyingType>> GetNamesAndValues<TUnderlyingType>(Type enumType) where TUnderlyingType : struct
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }
            if (!enumType.IsEnum())
            {
                throw new ArgumentException("Type {0} is not an Enum.".FormatWith(CultureInfo.InvariantCulture, enumType), "enumType");
            }
            IList<object> values = EnumUtils.GetValues(enumType);
            IList<string> names = EnumUtils.GetNames(enumType);
            IList<EnumValue<TUnderlyingType>> list = new List<EnumValue<TUnderlyingType>>();
            for (int i = 0; i < values.Count; i++)
            {
                try
                {
                    list.Add(new EnumValue<TUnderlyingType>(names[i], (TUnderlyingType)Convert.ChangeType(values[i], typeof(TUnderlyingType), CultureInfo.CurrentCulture)));
                }
                catch (OverflowException innerException)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Value from enum with the underlying type of {0} cannot be added to dictionary with a value type of {1}. Value was too large: {2}", new object[]
                    {
                        Enum.GetUnderlyingType(enumType),
                        typeof(TUnderlyingType),
                        Convert.ToUInt64(values[i], CultureInfo.InvariantCulture)
                    }), innerException);
                }
            }
            return list;
        }

        public static IList<object> GetValues(Type enumType)
        {
            if (!enumType.IsEnum())
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum.");
            }
            List<object> list = new List<object>();
            foreach (FieldInfo fieldInfo in from f in enumType.GetFields()
                                            where f.IsLiteral
                                            select f)
            {
                object value = fieldInfo.GetValue(enumType);
                list.Add(value);
            }
            return list;
        }

        public static IList<string> GetNames(Type enumType)
        {
            if (!enumType.IsEnum())
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum.");
            }
            List<string> list = new List<string>();
            foreach (FieldInfo fieldInfo in from f in enumType.GetFields()
                                            where f.IsLiteral
                                            select f)
            {
                list.Add(fieldInfo.Name);
            }
            return list;
        }

        public static object ParseEnumName(string enumText, bool isNullable, Type t)
        {
            if (enumText == string.Empty && isNullable)
            {
                return null;
            }
            BidirectionalDictionary<string, string> map = EnumUtils.EnumMemberNamesPerType.Get(t);
            string value;
            if (enumText.IndexOf(',') != -1)
            {
                string[] array = enumText.Split(new char[]
                {
                    ','
                });
                for (int i = 0; i < array.Length; i++)
                {
                    string enumText2 = array[i].Trim();
                    array[i] = EnumUtils.ResolvedEnumName(map, enumText2);
                }
                value = string.Join(", ", array);
            }
            else
            {
                value = EnumUtils.ResolvedEnumName(map, enumText);
            }
            return Enum.Parse(t, value, true);
        }

        public static string ToEnumName(Type enumType, string enumText, bool camelCaseText)
        {
            BidirectionalDictionary<string, string> bidirectionalDictionary = EnumUtils.EnumMemberNamesPerType.Get(enumType);
            string[] array = enumText.Split(new char[]
            {
                ','
            });
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i].Trim();
                bidirectionalDictionary.TryGetByFirst(text, out string text2);
                text2 = (text2 ?? text);
                if (camelCaseText)
                {
                    text2 = StringUtils.ToCamelCase(text2);
                }
                array[i] = text2;
            }
            return string.Join(", ", array);
        }

        private static string ResolvedEnumName(BidirectionalDictionary<string, string> map, string enumText)
        {
            map.TryGetBySecond(enumText, out string text);
            text = (text ?? enumText);
            return text;
        }

        private static readonly ThreadSafeStore<Type, BidirectionalDictionary<string, string>> EnumMemberNamesPerType = new ThreadSafeStore<Type, BidirectionalDictionary<string, string>>(new Func<Type, BidirectionalDictionary<string, string>>(EnumUtils.InitializeEnumType));
    }
}
