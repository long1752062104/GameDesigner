using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft_X.Json.Utilities
{
    internal static class DateTimeUtils
    {
        public static TimeSpan GetUtcOffset(this DateTime d)
        {
            return TimeZone.CurrentTimeZone.GetUtcOffset(d);
        }

        internal static DateTime EnsureDateTime(DateTime value, DateTimeZoneHandling timeZone)
        {
            switch (timeZone)
            {
                case DateTimeZoneHandling.Local:
                    value = DateTimeUtils.SwitchToLocalTime(value);
                    break;
                case DateTimeZoneHandling.Utc:
                    value = DateTimeUtils.SwitchToUtcTime(value);
                    break;
                case DateTimeZoneHandling.Unspecified:
                    value = new DateTime(value.Ticks, DateTimeKind.Unspecified);
                    break;
                case DateTimeZoneHandling.RoundtripKind:
                    break;
                default:
                    throw new ArgumentException("Invalid date time handling value.");
            }
            return value;
        }

        private static DateTime SwitchToLocalTime(DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    return new DateTime(value.Ticks, DateTimeKind.Local);
                case DateTimeKind.Utc:
                    return value.ToLocalTime();
                case DateTimeKind.Local:
                    return value;
                default:
                    return value;
            }
        }

        private static DateTime SwitchToUtcTime(DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    return new DateTime(value.Ticks, DateTimeKind.Utc);
                case DateTimeKind.Utc:
                    return value;
                case DateTimeKind.Local:
                    return value.ToUniversalTime();
                default:
                    return value;
            }
        }

        private static long ToUniversalTicks(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.Ticks;
            }
            return DateTimeUtils.ToUniversalTicks(dateTime, dateTime.GetUtcOffset());
        }

        private static long ToUniversalTicks(DateTime dateTime, TimeSpan offset)
        {
            if (dateTime.Kind == DateTimeKind.Utc || dateTime == DateTime.MaxValue || dateTime == DateTime.MinValue)
            {
                return dateTime.Ticks;
            }
            long num = dateTime.Ticks - offset.Ticks;
            if (num > 3155378975999999999L)
            {
                return 3155378975999999999L;
            }
            if (num < 0L)
            {
                return 0L;
            }
            return num;
        }

        internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, TimeSpan offset)
        {
            return DateTimeUtils.UniversialTicksToJavaScriptTicks(DateTimeUtils.ToUniversalTicks(dateTime, offset));
        }

        internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime)
        {
            return DateTimeUtils.ConvertDateTimeToJavaScriptTicks(dateTime, true);
        }

        internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, bool convertToUtc)
        {
            return DateTimeUtils.UniversialTicksToJavaScriptTicks(convertToUtc ? DateTimeUtils.ToUniversalTicks(dateTime) : dateTime.Ticks);
        }

        private static long UniversialTicksToJavaScriptTicks(long universialTicks)
        {
            return (universialTicks - DateTimeUtils.InitialJavaScriptDateTicks) / 10000L;
        }

        internal static DateTime ConvertJavaScriptTicksToDateTime(long javaScriptTicks)
        {
            return new DateTime(javaScriptTicks * 10000L + DateTimeUtils.InitialJavaScriptDateTicks, DateTimeKind.Utc);
        }

        internal static bool TryParseDateTimeIso(StringReference text, DateTimeZoneHandling dateTimeZoneHandling, out DateTime dt)
        {
            DateTimeParser dateTimeParser = default(DateTimeParser);
            if (!dateTimeParser.Parse(text.Chars, text.StartIndex, text.Length))
            {
                dt = default(DateTime);
                return false;
            }
            DateTime dateTime = DateTimeUtils.CreateDateTime(dateTimeParser);
            switch (dateTimeParser.Zone)
            {
                case ParserTimeZone.Utc:
                    dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
                    break;
                case ParserTimeZone.LocalWestOfUtc:
                    {
                        TimeSpan timeSpan = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
                        long num = dateTime.Ticks + timeSpan.Ticks;
                        if (num <= DateTime.MaxValue.Ticks)
                        {
                            dateTime = new DateTime(num, DateTimeKind.Utc).ToLocalTime();
                        }
                        else
                        {
                            num += dateTime.GetUtcOffset().Ticks;
                            if (num > DateTime.MaxValue.Ticks)
                            {
                                num = DateTime.MaxValue.Ticks;
                            }
                            dateTime = new DateTime(num, DateTimeKind.Local);
                        }
                        break;
                    }
                case ParserTimeZone.LocalEastOfUtc:
                    {
                        TimeSpan timeSpan2 = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
                        long num = dateTime.Ticks - timeSpan2.Ticks;
                        if (num >= DateTime.MinValue.Ticks)
                        {
                            dateTime = new DateTime(num, DateTimeKind.Utc).ToLocalTime();
                        }
                        else
                        {
                            num += dateTime.GetUtcOffset().Ticks;
                            if (num < DateTime.MinValue.Ticks)
                            {
                                num = DateTime.MinValue.Ticks;
                            }
                            dateTime = new DateTime(num, DateTimeKind.Local);
                        }
                        break;
                    }
            }
            dt = DateTimeUtils.EnsureDateTime(dateTime, dateTimeZoneHandling);
            return true;
        }

        internal static bool TryParseDateTimeOffsetIso(StringReference text, out DateTimeOffset dt)
        {
            DateTimeParser dateTimeParser = default(DateTimeParser);
            if (!dateTimeParser.Parse(text.Chars, text.StartIndex, text.Length))
            {
                dt = default(DateTimeOffset);
                return false;
            }
            DateTime dateTime = DateTimeUtils.CreateDateTime(dateTimeParser);
            TimeSpan utcOffset;
            switch (dateTimeParser.Zone)
            {
                case ParserTimeZone.Utc:
                    utcOffset = new TimeSpan(0L);
                    break;
                case ParserTimeZone.LocalWestOfUtc:
                    utcOffset = new TimeSpan(-dateTimeParser.ZoneHour, -dateTimeParser.ZoneMinute, 0);
                    break;
                case ParserTimeZone.LocalEastOfUtc:
                    utcOffset = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
                    break;
                default:
                    utcOffset = dateTime.GetUtcOffset();
                    break;
            }
            long num = dateTime.Ticks - utcOffset.Ticks;
            if (num < 0L || num > 3155378975999999999L)
            {
                dt = default(DateTimeOffset);
                return false;
            }
            dt = new DateTimeOffset(dateTime, utcOffset);
            return true;
        }

        private static DateTime CreateDateTime(DateTimeParser dateTimeParser)
        {
            bool flag;
            if (dateTimeParser.Hour == 24)
            {
                flag = true;
                dateTimeParser.Hour = 0;
            }
            else
            {
                flag = false;
            }
            DateTime result = new DateTime(dateTimeParser.Year, dateTimeParser.Month, dateTimeParser.Day, dateTimeParser.Hour, dateTimeParser.Minute, dateTimeParser.Second);
            result = result.AddTicks(dateTimeParser.Fraction);
            if (flag)
            {
                result = result.AddDays(1.0);
            }
            return result;
        }

        internal static bool TryParseDateTime(StringReference s, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
        {
            if (s.Length > 0)
            {
                int startIndex = s.StartIndex;
                if (s[startIndex] == '/')
                {
                    if (s.Length >= 9 && s.StartsWith("/Date(") && s.EndsWith(")/") && DateTimeUtils.TryParseDateTimeMicrosoft(s, dateTimeZoneHandling, out dt))
                    {
                        return true;
                    }
                }
                else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[startIndex]) && s[startIndex + 10] == 'T' && DateTimeUtils.TryParseDateTimeIso(s, dateTimeZoneHandling, out dt))
                {
                    return true;
                }
                if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeExact(s.ToString(), dateTimeZoneHandling, dateFormatString, culture, out dt))
                {
                    return true;
                }
            }
            dt = default(DateTime);
            return false;
        }

        internal static bool TryParseDateTime(string s, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
        {
            if (s.Length > 0)
            {
                if (s[0] == '/')
                {
                    if (s.Length >= 9 && s.StartsWith("/Date(", StringComparison.Ordinal) && s.EndsWith(")/", StringComparison.Ordinal) && DateTimeUtils.TryParseDateTimeMicrosoft(new StringReference(s.ToCharArray(), 0, s.Length), dateTimeZoneHandling, out dt))
                    {
                        return true;
                    }
                }
                else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[0]) && s[10] == 'T' && DateTime.TryParseExact(s, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt))
                {
                    dt = DateTimeUtils.EnsureDateTime(dt, dateTimeZoneHandling);
                    return true;
                }
                if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeExact(s, dateTimeZoneHandling, dateFormatString, culture, out dt))
                {
                    return true;
                }
            }
            dt = default(DateTime);
            return false;
        }

        internal static bool TryParseDateTimeOffset(StringReference s, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
        {
            if (s.Length > 0)
            {
                int startIndex = s.StartIndex;
                if (s[startIndex] == '/')
                {
                    if (s.Length >= 9 && s.StartsWith("/Date(") && s.EndsWith(")/") && DateTimeUtils.TryParseDateTimeOffsetMicrosoft(s, out dt))
                    {
                        return true;
                    }
                }
                else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[startIndex]) && s[startIndex + 10] == 'T' && DateTimeUtils.TryParseDateTimeOffsetIso(s, out dt))
                {
                    return true;
                }
                if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeOffsetExact(s.ToString(), dateFormatString, culture, out dt))
                {
                    return true;
                }
            }
            dt = default(DateTimeOffset);
            return false;
        }

        internal static bool TryParseDateTimeOffset(string s, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
        {
            if (s.Length > 0)
            {
                if (s[0] == '/')
                {
                    if (s.Length >= 9 && s.StartsWith("/Date(", StringComparison.Ordinal) && s.EndsWith(")/", StringComparison.Ordinal) && DateTimeUtils.TryParseDateTimeOffsetMicrosoft(new StringReference(s.ToCharArray(), 0, s.Length), out dt))
                    {
                        return true;
                    }
                }
                else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[0]) && s[10] == 'T' && DateTimeOffset.TryParseExact(s, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt) && DateTimeUtils.TryParseDateTimeOffsetIso(new StringReference(s.ToCharArray(), 0, s.Length), out dt))
                {
                    return true;
                }
                if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeOffsetExact(s, dateFormatString, culture, out dt))
                {
                    return true;
                }
            }
            dt = default(DateTimeOffset);
            return false;
        }

        private static bool TryParseMicrosoftDate(StringReference text, out long ticks, out TimeSpan offset, out DateTimeKind kind)
        {
            kind = DateTimeKind.Utc;
            int num = text.IndexOf('+', 7, text.Length - 8);
            if (num == -1)
            {
                num = text.IndexOf('-', 7, text.Length - 8);
            }
            if (num != -1)
            {
                kind = DateTimeKind.Local;
                if (!DateTimeUtils.TryReadOffset(text, num + text.StartIndex, out offset))
                {
                    ticks = 0L;
                    return false;
                }
            }
            else
            {
                offset = TimeSpan.Zero;
                num = text.Length - 2;
            }
            return ConvertUtils.Int64TryParse(text.Chars, 6 + text.StartIndex, num - 6, out ticks) == ParseResult.Success;
        }

        private static bool TryParseDateTimeMicrosoft(StringReference text, DateTimeZoneHandling dateTimeZoneHandling, out DateTime dt)
        {
            if (!DateTimeUtils.TryParseMicrosoftDate(text, out long javaScriptTicks, out TimeSpan timeSpan, out DateTimeKind dateTimeKind))
            {
                dt = default(DateTime);
                return false;
            }
            DateTime dateTime = DateTimeUtils.ConvertJavaScriptTicksToDateTime(javaScriptTicks);
            if (dateTimeKind != DateTimeKind.Unspecified)
            {
                if (dateTimeKind != DateTimeKind.Local)
                {
                    dt = dateTime;
                }
                else
                {
                    dt = dateTime.ToLocalTime();
                }
            }
            else
            {
                dt = DateTime.SpecifyKind(dateTime.ToLocalTime(), DateTimeKind.Unspecified);
            }
            dt = DateTimeUtils.EnsureDateTime(dt, dateTimeZoneHandling);
            return true;
        }

        private static bool TryParseDateTimeExact(string text, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
        {
            if (DateTime.TryParseExact(text, dateFormatString, culture, DateTimeStyles.RoundtripKind, out DateTime dateTime))
            {
                dateTime = DateTimeUtils.EnsureDateTime(dateTime, dateTimeZoneHandling);
                dt = dateTime;
                return true;
            }
            dt = default(DateTime);
            return false;
        }

        private static bool TryParseDateTimeOffsetMicrosoft(StringReference text, out DateTimeOffset dt)
        {
            if (!DateTimeUtils.TryParseMicrosoftDate(text, out long javaScriptTicks, out TimeSpan timeSpan, out DateTimeKind dateTimeKind))
            {
                dt = default(DateTime);
                return false;
            }
            dt = new DateTimeOffset(DateTimeUtils.ConvertJavaScriptTicksToDateTime(javaScriptTicks).Add(timeSpan).Ticks, timeSpan);
            return true;
        }

        private static bool TryParseDateTimeOffsetExact(string text, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
        {
            if (DateTimeOffset.TryParseExact(text, dateFormatString, culture, DateTimeStyles.RoundtripKind, out DateTimeOffset dateTimeOffset))
            {
                dt = dateTimeOffset;
                return true;
            }
            dt = default(DateTimeOffset);
            return false;
        }

        private static bool TryReadOffset(StringReference offsetText, int startIndex, out TimeSpan offset)
        {
            bool flag = offsetText[startIndex] == '-';
            if (ConvertUtils.Int32TryParse(offsetText.Chars, startIndex + 1, 2, out int num) != ParseResult.Success)
            {
                offset = default(TimeSpan);
                return false;
            }
            int num2 = 0;
            if (offsetText.Length - startIndex > 5 && ConvertUtils.Int32TryParse(offsetText.Chars, startIndex + 3, 2, out num2) != ParseResult.Success)
            {
                offset = default(TimeSpan);
                return false;
            }
            offset = TimeSpan.FromHours(num) + TimeSpan.FromMinutes(num2);
            if (flag)
            {
                offset = offset.Negate();
            }
            return true;
        }

        internal static void WriteDateTimeString(TextWriter writer, DateTime value, DateFormatHandling format, string formatString, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(formatString))
            {
                char[] array = new char[64];
                int count = DateTimeUtils.WriteDateTimeString(array, 0, value, null, value.Kind, format);
                writer.Write(array, 0, count);
                return;
            }
            writer.Write(value.ToString(formatString, culture));
        }

        internal static int WriteDateTimeString(char[] chars, int start, DateTime value, TimeSpan? offset, DateTimeKind kind, DateFormatHandling format)
        {
            int num2;
            if (format == DateFormatHandling.MicrosoftDateFormat)
            {
                TimeSpan offset2 = offset ?? value.GetUtcOffset();
                long num = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(value, offset2);
                "\\/Date(".CopyTo(0, chars, start, 7);
                num2 = start + 7;
                string text = num.ToString(CultureInfo.InvariantCulture);
                text.CopyTo(0, chars, num2, text.Length);
                num2 += text.Length;
                if (kind != DateTimeKind.Unspecified)
                {
                    if (kind == DateTimeKind.Local)
                    {
                        num2 = DateTimeUtils.WriteDateTimeOffset(chars, num2, offset2, format);
                    }
                }
                else if (value != DateTime.MaxValue && value != DateTime.MinValue)
                {
                    num2 = DateTimeUtils.WriteDateTimeOffset(chars, num2, offset2, format);
                }
                ")\\/".CopyTo(0, chars, num2, 3);
                num2 += 3;
            }
            else
            {
                num2 = DateTimeUtils.WriteDefaultIsoDate(chars, start, value);
                if (kind != DateTimeKind.Utc)
                {
                    if (kind == DateTimeKind.Local)
                    {
                        num2 = DateTimeUtils.WriteDateTimeOffset(chars, num2, offset ?? value.GetUtcOffset(), format);
                    }
                }
                else
                {
                    chars[num2++] = 'Z';
                }
            }
            return num2;
        }

        internal static int WriteDefaultIsoDate(char[] chars, int start, DateTime dt)
        {
            int num = 19;
            DateTimeUtils.GetDateValues(dt, out int value, out int value2, out int value3);
            DateTimeUtils.CopyIntToCharArray(chars, start, value, 4);
            chars[start + 4] = '-';
            DateTimeUtils.CopyIntToCharArray(chars, start + 5, value2, 2);
            chars[start + 7] = '-';
            DateTimeUtils.CopyIntToCharArray(chars, start + 8, value3, 2);
            chars[start + 10] = 'T';
            DateTimeUtils.CopyIntToCharArray(chars, start + 11, dt.Hour, 2);
            chars[start + 13] = ':';
            DateTimeUtils.CopyIntToCharArray(chars, start + 14, dt.Minute, 2);
            chars[start + 16] = ':';
            DateTimeUtils.CopyIntToCharArray(chars, start + 17, dt.Second, 2);
            int num2 = (int)(dt.Ticks % 10000000L);
            if (num2 != 0)
            {
                int num3 = 7;
                while (num2 % 10 == 0)
                {
                    num3--;
                    num2 /= 10;
                }
                chars[start + 19] = '.';
                DateTimeUtils.CopyIntToCharArray(chars, start + 20, num2, num3);
                num += num3 + 1;
            }
            return start + num;
        }

        private static void CopyIntToCharArray(char[] chars, int start, int value, int digits)
        {
            while (digits-- != 0)
            {
                chars[start + digits] = (char)(value % 10 + 48);
                value /= 10;
            }
        }

        internal static int WriteDateTimeOffset(char[] chars, int start, TimeSpan offset, DateFormatHandling format)
        {
            chars[start++] = ((offset.Ticks >= 0L) ? '+' : '-');
            int value = Math.Abs(offset.Hours);
            DateTimeUtils.CopyIntToCharArray(chars, start, value, 2);
            start += 2;
            if (format == DateFormatHandling.IsoDateFormat)
            {
                chars[start++] = ':';
            }
            int value2 = Math.Abs(offset.Minutes);
            DateTimeUtils.CopyIntToCharArray(chars, start, value2, 2);
            start += 2;
            return start;
        }

        internal static void WriteDateTimeOffsetString(TextWriter writer, DateTimeOffset value, DateFormatHandling format, string formatString, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(formatString))
            {
                char[] array = new char[64];
                int count = DateTimeUtils.WriteDateTimeString(array, 0, (format == DateFormatHandling.IsoDateFormat) ? value.DateTime : value.UtcDateTime, new TimeSpan?(value.Offset), DateTimeKind.Local, format);
                writer.Write(array, 0, count);
                return;
            }
            writer.Write(value.ToString(formatString, culture));
        }

        private static void GetDateValues(DateTime td, out int year, out int month, out int day)
        {
            int i = (int)(td.Ticks / 864000000000L);
            int num = i / 146097;
            i -= num * 146097;
            int num2 = i / 36524;
            if (num2 == 4)
            {
                num2 = 3;
            }
            i -= num2 * 36524;
            int num3 = i / 1461;
            i -= num3 * 1461;
            int num4 = i / 365;
            if (num4 == 4)
            {
                num4 = 3;
            }
            year = num * 400 + num2 * 100 + num3 * 4 + num4 + 1;
            i -= num4 * 365;
            int[] array = (num4 == 3 && (num3 != 24 || num2 == 3)) ? DateTimeUtils.DaysToMonth366 : DateTimeUtils.DaysToMonth365;
            int num5 = i >> 6;
            while (i >= array[num5])
            {
                num5++;
            }
            month = num5;
            day = i - array[num5 - 1] + 1;
        }

        internal static readonly long InitialJavaScriptDateTicks = 621355968000000000L;

        private const string IsoDateFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";

        private const int DaysPer100Years = 36524;

        private const int DaysPer400Years = 146097;

        private const int DaysPer4Years = 1461;

        private const int DaysPerYear = 365;

        private const long TicksPerDay = 864000000000L;

        private static readonly int[] DaysToMonth365 = new int[]
        {
            0,
            31,
            59,
            90,
            120,
            151,
            181,
            212,
            243,
            273,
            304,
            334,
            365
        };

        private static readonly int[] DaysToMonth366 = new int[]
        {
            0,
            31,
            60,
            91,
            121,
            152,
            182,
            213,
            244,
            274,
            305,
            335,
            366
        };
    }
}
