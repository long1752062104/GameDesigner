using System;

namespace Newtonsoft_X.Json.Utilities
{
    internal static class MathUtils
    {
        public static int IntLength(ulong i)
        {
            if (i < 10000000000UL)
            {
                if (i < 10UL)
                {
                    return 1;
                }
                if (i < 100UL)
                {
                    return 2;
                }
                if (i < 1000UL)
                {
                    return 3;
                }
                if (i < 10000UL)
                {
                    return 4;
                }
                if (i < 100000UL)
                {
                    return 5;
                }
                if (i < 1000000UL)
                {
                    return 6;
                }
                if (i < 10000000UL)
                {
                    return 7;
                }
                if (i < 100000000UL)
                {
                    return 8;
                }
                if (i < 1000000000UL)
                {
                    return 9;
                }
                return 10;
            }
            else
            {
                if (i < 100000000000UL)
                {
                    return 11;
                }
                if (i < 1000000000000UL)
                {
                    return 12;
                }
                if (i < 10000000000000UL)
                {
                    return 13;
                }
                if (i < 100000000000000UL)
                {
                    return 14;
                }
                if (i < 1000000000000000UL)
                {
                    return 15;
                }
                if (i < 10000000000000000UL)
                {
                    return 16;
                }
                if (i < 100000000000000000UL)
                {
                    return 17;
                }
                if (i < 1000000000000000000UL)
                {
                    return 18;
                }
                if (i < 10000000000000000000UL)
                {
                    return 19;
                }
                return 20;
            }
        }

        public static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 48);
            }
            return (char)(n - 10 + 97);
        }

        public static int? Min(int? val1, int? val2)
        {
            if (val1 == null)
            {
                return val2;
            }
            if (val2 == null)
            {
                return val1;
            }
            return new int?(Math.Min(val1.GetValueOrDefault(), val2.GetValueOrDefault()));
        }

        public static int? Max(int? val1, int? val2)
        {
            if (val1 == null)
            {
                return val2;
            }
            if (val2 == null)
            {
                return val1;
            }
            return new int?(Math.Max(val1.GetValueOrDefault(), val2.GetValueOrDefault()));
        }

        public static double? Max(double? val1, double? val2)
        {
            if (val1 == null)
            {
                return val2;
            }
            if (val2 == null)
            {
                return val1;
            }
            return new double?(Math.Max(val1.GetValueOrDefault(), val2.GetValueOrDefault()));
        }

        public static bool ApproxEquals(double d1, double d2)
        {
            if (d1 == d2)
            {
                return true;
            }
            double num = (Math.Abs(d1) + Math.Abs(d2) + 10.0) * 2.220446049250313E-16;
            double num2 = d1 - d2;
            return -num < num2 && num > num2;
        }
    }
}
