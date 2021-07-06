using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Newtonsoft_X.Json.Utilities
{
    internal static class StringUtils
    {
        public static string FormatWith(this string format, IFormatProvider provider, object arg0)
        {
            return format.FormatWith(provider, new object[]
            {
                arg0
            });
        }

        public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1)
        {
            return format.FormatWith(provider, new object[]
            {
                arg0,
                arg1
            });
        }

        public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1, object arg2)
        {
            return format.FormatWith(provider, new object[]
            {
                arg0,
                arg1,
                arg2
            });
        }

        public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1, object arg2, object arg3)
        {
            return format.FormatWith(provider, new object[]
            {
                arg0,
                arg1,
                arg2,
                arg3
            });
        }

        private static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            ValidationUtils.ArgumentNotNull(format, "format");
            return string.Format(provider, format, args);
        }

        /// <summary>
        /// Determines whether the string is all white space. Empty string will return <c>false</c>.
        /// </summary>
        /// <param name="s">The string to test whether it is all white space.</param>
        /// <returns>
        /// 	<c>true</c> if the string is all white space; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWhiteSpace(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (s.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsWhiteSpace(s[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static StringWriter CreateStringWriter(int capacity)
        {
            return new StringWriter(new StringBuilder(capacity), CultureInfo.InvariantCulture);
        }

        public static void ToCharAsUnicode(char c, char[] buffer)
        {
            buffer[0] = '\\';
            buffer[1] = 'u';
            buffer[2] = MathUtils.IntToHex(c >> 12 & '\u000f');
            buffer[3] = MathUtils.IntToHex(c >> 8 & '\u000f');
            buffer[4] = MathUtils.IntToHex(c >> 4 & '\u000f');
            buffer[5] = MathUtils.IntToHex(c & '\u000f');
        }

        public static TSource ForgivingCaseSensitiveFind<TSource>(this IEnumerable<TSource> source, Func<TSource, string> valueSelector, string testValue)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (valueSelector == null)
            {
                throw new ArgumentNullException("valueSelector");
            }
            IEnumerable<TSource> source2 = from s in source
                                           where string.Equals(valueSelector(s), testValue, StringComparison.OrdinalIgnoreCase)
                                           select s;
            if (source2.Count<TSource>() <= 1)
            {
                return source2.SingleOrDefault<TSource>();
            }
            return (from s in source
                    where string.Equals(valueSelector(s), testValue, StringComparison.Ordinal)
                    select s).SingleOrDefault<TSource>();
        }

        public static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }
            char[] array = s.ToCharArray();
            int num = 0;
            while (num < array.Length && (num != 1 || char.IsUpper(array[num])))
            {
                bool flag = num + 1 < array.Length;
                if (num > 0 && flag && !char.IsUpper(array[num + 1]))
                {
                    break;
                }
                char c = char.ToLower(array[num], CultureInfo.InvariantCulture);
                array[num] = c;
                num++;
            }
            return new string(array);
        }

        public static string ToSnakeCase(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            StringBuilder stringBuilder = new StringBuilder();
            StringUtils.SnakeCaseState snakeCaseState = StringUtils.SnakeCaseState.Start;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    if (snakeCaseState != StringUtils.SnakeCaseState.Start)
                    {
                        snakeCaseState = StringUtils.SnakeCaseState.NewWord;
                    }
                }
                else if (char.IsUpper(s[i]))
                {
                    switch (snakeCaseState)
                    {
                        case StringUtils.SnakeCaseState.Lower:
                        case StringUtils.SnakeCaseState.NewWord:
                            stringBuilder.Append('_');
                            break;
                        case StringUtils.SnakeCaseState.Upper:
                            {
                                bool flag = i + 1 < s.Length;
                                if (i > 0 && flag)
                                {
                                    char c = s[i + 1];
                                    if (!char.IsUpper(c) && c != '_')
                                    {
                                        stringBuilder.Append('_');
                                    }
                                }
                                break;
                            }
                    }
                    char value = char.ToLower(s[i], CultureInfo.InvariantCulture);
                    stringBuilder.Append(value);
                    snakeCaseState = StringUtils.SnakeCaseState.Upper;
                }
                else if (s[i] == '_')
                {
                    stringBuilder.Append('_');
                    snakeCaseState = StringUtils.SnakeCaseState.Start;
                }
                else
                {
                    if (snakeCaseState == StringUtils.SnakeCaseState.NewWord)
                    {
                        stringBuilder.Append('_');
                    }
                    stringBuilder.Append(s[i]);
                    snakeCaseState = StringUtils.SnakeCaseState.Lower;
                }
            }
            return stringBuilder.ToString();
        }

        public static bool IsHighSurrogate(char c)
        {
            return char.IsHighSurrogate(c);
        }

        public static bool IsLowSurrogate(char c)
        {
            return char.IsLowSurrogate(c);
        }

        public static bool StartsWith(this string source, char value)
        {
            return source.Length > 0 && source[0] == value;
        }

        public static bool EndsWith(this string source, char value)
        {
            return source.Length > 0 && source[source.Length - 1] == value;
        }

        public const string CarriageReturnLineFeed = "\r\n";

        public const string Empty = "";

        public const char CarriageReturn = '\r';

        public const char LineFeed = '\n';

        public const char Tab = '\t';

        internal enum SnakeCaseState
        {
            Start,
            Lower,
            Upper,
            NewWord
        }
    }
}
