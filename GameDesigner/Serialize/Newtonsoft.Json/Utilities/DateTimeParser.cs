using System;

namespace Newtonsoft_X.Json.Utilities
{
    internal struct DateTimeParser
    {
        public bool Parse(char[] text, int startIndex, int length)
        {
            _text = text;
            _end = startIndex + length;
            return ParseDate(startIndex) && ParseChar(DateTimeParser.Lzyyyy_MM_dd + startIndex, 'T') && ParseTimeAndZoneAndWhitespace(DateTimeParser.Lzyyyy_MM_ddT + startIndex);
        }

        private bool ParseDate(int start)
        {
            return Parse4Digit(start, out Year) && 1 <= Year && ParseChar(start + DateTimeParser.Lzyyyy, '-') && Parse2Digit(start + DateTimeParser.Lzyyyy_, out Month) && 1 <= Month && Month <= 12 && ParseChar(start + DateTimeParser.Lzyyyy_MM, '-') && Parse2Digit(start + DateTimeParser.Lzyyyy_MM_, out Day) && 1 <= Day && Day <= DateTime.DaysInMonth(Year, Month);
        }

        private bool ParseTimeAndZoneAndWhitespace(int start)
        {
            return ParseTime(ref start) && ParseZone(start);
        }

        private bool ParseTime(ref int start)
        {
            if (!Parse2Digit(start, out Hour) || Hour > 24 || !ParseChar(start + DateTimeParser.LzHH, ':') || !Parse2Digit(start + DateTimeParser.LzHH_, out Minute) || Minute >= 60 || !ParseChar(start + DateTimeParser.LzHH_mm, ':') || !Parse2Digit(start + DateTimeParser.LzHH_mm_, out Second) || Second >= 60 || (Hour == 24 && (Minute != 0 || Second != 0)))
            {
                return false;
            }
            start += DateTimeParser.LzHH_mm_ss;
            if (ParseChar(start, '.'))
            {
                Fraction = 0;
                int num = 0;
                for (; ; )
                {
                    int num2 = start + 1;
                    start = num2;
                    if (num2 >= _end || num >= 7)
                    {
                        break;
                    }
                    int num3 = _text[start] - '0';
                    if (num3 < 0 || num3 > 9)
                    {
                        break;
                    }
                    Fraction = Fraction * 10 + num3;
                    num++;
                }
                if (num < 7)
                {
                    if (num == 0)
                    {
                        return false;
                    }
                    Fraction *= DateTimeParser.Power10[7 - num];
                }
                if (Hour == 24 && Fraction != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ParseZone(int start)
        {
            if (start < _end)
            {
                char c = _text[start];
                if (c == 'Z' || c == 'z')
                {
                    Zone = ParserTimeZone.Utc;
                    start++;
                }
                else
                {
                    if (start + 2 < _end && Parse2Digit(start + DateTimeParser.Lz_, out ZoneHour) && ZoneHour <= 99)
                    {
                        if (c != '+')
                        {
                            if (c == '-')
                            {
                                Zone = ParserTimeZone.LocalWestOfUtc;
                                start += DateTimeParser.Lz_zz;
                            }
                        }
                        else
                        {
                            Zone = ParserTimeZone.LocalEastOfUtc;
                            start += DateTimeParser.Lz_zz;
                        }
                    }
                    if (start < _end)
                    {
                        if (ParseChar(start, ':'))
                        {
                            start++;
                            if (start + 1 < _end && Parse2Digit(start, out ZoneMinute) && ZoneMinute <= 99)
                            {
                                start += 2;
                            }
                        }
                        else if (start + 1 < _end && Parse2Digit(start, out ZoneMinute) && ZoneMinute <= 99)
                        {
                            start += 2;
                        }
                    }
                }
            }
            return start == _end;
        }

        private bool Parse4Digit(int start, out int num)
        {
            if (start + 3 < _end)
            {
                int num2 = _text[start] - '0';
                int num3 = _text[start + 1] - '0';
                int num4 = _text[start + 2] - '0';
                int num5 = _text[start + 3] - '0';
                if (0 <= num2 && num2 < 10 && 0 <= num3 && num3 < 10 && 0 <= num4 && num4 < 10 && 0 <= num5 && num5 < 10)
                {
                    num = ((num2 * 10 + num3) * 10 + num4) * 10 + num5;
                    return true;
                }
            }
            num = 0;
            return false;
        }

        private bool Parse2Digit(int start, out int num)
        {
            if (start + 1 < _end)
            {
                int num2 = _text[start] - '0';
                int num3 = _text[start + 1] - '0';
                if (0 <= num2 && num2 < 10 && 0 <= num3 && num3 < 10)
                {
                    num = num2 * 10 + num3;
                    return true;
                }
            }
            num = 0;
            return false;
        }

        private bool ParseChar(int start, char ch)
        {
            return start < _end && _text[start] == ch;
        }

        public int Year;

        public int Month;

        public int Day;

        public int Hour;

        public int Minute;

        public int Second;

        public int Fraction;

        public int ZoneHour;

        public int ZoneMinute;

        public ParserTimeZone Zone;

        private char[] _text;

        private int _end;

        private static readonly int[] Power10 = new int[]
        {
            -1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000
        };

        private static readonly int Lzyyyy = "yyyy".Length;

        private static readonly int Lzyyyy_ = "yyyy-".Length;

        private static readonly int Lzyyyy_MM = "yyyy-MM".Length;

        private static readonly int Lzyyyy_MM_ = "yyyy-MM-".Length;

        private static readonly int Lzyyyy_MM_dd = "yyyy-MM-dd".Length;

        private static readonly int Lzyyyy_MM_ddT = "yyyy-MM-ddT".Length;

        private static readonly int LzHH = "HH".Length;

        private static readonly int LzHH_ = "HH:".Length;

        private static readonly int LzHH_mm = "HH:mm".Length;

        private static readonly int LzHH_mm_ = "HH:mm:".Length;

        private static readonly int LzHH_mm_ss = "HH:mm:ss".Length;

        private static readonly int Lz_ = "-".Length;

        private static readonly int Lz_zz = "-zz".Length;

        private const short MaxFractionDigits = 7;
    }
}
