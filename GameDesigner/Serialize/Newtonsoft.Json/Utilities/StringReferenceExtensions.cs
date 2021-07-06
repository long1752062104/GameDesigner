using System;

namespace Newtonsoft_X.Json.Utilities
{
    internal static class StringReferenceExtensions
    {
        public static int IndexOf(this StringReference s, char c, int startIndex, int length)
        {
            int num = Array.IndexOf<char>(s.Chars, c, s.StartIndex + startIndex, length);
            if (num == -1)
            {
                return -1;
            }
            return num - s.StartIndex;
        }

        public static bool StartsWith(this StringReference s, string text)
        {
            if (text.Length > s.Length)
            {
                return false;
            }
            char[] chars = s.Chars;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != chars[i + s.StartIndex])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool EndsWith(this StringReference s, string text)
        {
            if (text.Length > s.Length)
            {
                return false;
            }
            char[] chars = s.Chars;
            int num = s.StartIndex + s.Length - text.Length;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != chars[i + num])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
