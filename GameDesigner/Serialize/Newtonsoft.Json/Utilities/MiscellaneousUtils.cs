using System;
using System.Globalization;

namespace Newtonsoft_X.Json.Utilities
{
    internal static class MiscellaneousUtils
    {
        public static bool ValueEquals(object objA, object objB)
        {
            if (objA == null && objB == null)
            {
                return true;
            }
            if (objA != null && objB == null)
            {
                return false;
            }
            if (objA == null && objB != null)
            {
                return false;
            }
            if (objA.GetType() == objB.GetType())
            {
                return objA.Equals(objB);
            }
            if (ConvertUtils.IsInteger(objA) && ConvertUtils.IsInteger(objB))
            {
                return Convert.ToDecimal(objA, CultureInfo.CurrentCulture).Equals(Convert.ToDecimal(objB, CultureInfo.CurrentCulture));
            }
            return (objA is double || objA is float || objA is decimal) && (objB is double || objB is float || objB is decimal) && MathUtils.ApproxEquals(Convert.ToDouble(objA, CultureInfo.CurrentCulture), Convert.ToDouble(objB, CultureInfo.CurrentCulture));
        }

        public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string paramName, object actualValue, string message)
        {
            string message2 = message + Environment.NewLine + "Actual value was {0}.".FormatWith(CultureInfo.InvariantCulture, actualValue);
            return new ArgumentOutOfRangeException(paramName, message2);
        }

        public static string ToString(object value)
        {
            if (value == null)
            {
                return "{null}";
            }
            if (!(value is string))
            {
                return value.ToString();
            }
            return "\"" + value.ToString() + "\"";
        }

        public static int ByteArrayCompare(byte[] a1, byte[] a2)
        {
            int num = a1.Length.CompareTo(a2.Length);
            if (num != 0)
            {
                return num;
            }
            for (int i = 0; i < a1.Length; i++)
            {
                int num2 = a1[i].CompareTo(a2[i]);
                if (num2 != 0)
                {
                    return num2;
                }
            }
            return 0;
        }

        public static string GetPrefix(string qualifiedName)
        {
            MiscellaneousUtils.GetQualifiedNameParts(qualifiedName, out string result, out string text);
            return result;
        }

        public static string GetLocalName(string qualifiedName)
        {
            MiscellaneousUtils.GetQualifiedNameParts(qualifiedName, out string text, out string result);
            return result;
        }

        public static void GetQualifiedNameParts(string qualifiedName, out string prefix, out string localName)
        {
            int num = qualifiedName.IndexOf(':');
            if (num == -1 || num == 0 || qualifiedName.Length - 1 == num)
            {
                prefix = null;
                localName = qualifiedName;
                return;
            }
            prefix = qualifiedName.Substring(0, num);
            localName = qualifiedName.Substring(num + 1);
        }

        internal static string FormatValueForPrint(object value)
        {
            if (value == null)
            {
                return "{null}";
            }
            if (value is string)
            {
                return "\"" + value + "\"";
            }
            return value.ToString();
        }
    }
}
