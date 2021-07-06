using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Newtonsoft_X.Json.Utilities
{
    internal static class JavaScriptUtils
    {
        static JavaScriptUtils()
        {
            IList<char> list = new List<char>
            {
                '\n',
                '\r',
                '\t',
                '\\',
                '\f',
                '\b'
            };
            for (int i = 0; i < 32; i++)
            {
                list.Add((char)i);
            }
            foreach (char c in list.Union(new char[]
            {
                '\''
            }))
            {
                JavaScriptUtils.SingleQuoteCharEscapeFlags[c] = true;
            }
            foreach (char c2 in list.Union(new char[]
            {
                '"'
            }))
            {
                JavaScriptUtils.DoubleQuoteCharEscapeFlags[c2] = true;
            }
            foreach (char c3 in list.Union(new char[]
            {
                '"',
                '\'',
                '<',
                '>',
                '&'
            }))
            {
                JavaScriptUtils.HtmlCharEscapeFlags[c3] = true;
            }
        }

        public static bool[] GetCharEscapeFlags(StringEscapeHandling stringEscapeHandling, char quoteChar)
        {
            if (stringEscapeHandling == StringEscapeHandling.EscapeHtml)
            {
                return JavaScriptUtils.HtmlCharEscapeFlags;
            }
            if (quoteChar == '"')
            {
                return JavaScriptUtils.DoubleQuoteCharEscapeFlags;
            }
            return JavaScriptUtils.SingleQuoteCharEscapeFlags;
        }

        public static bool ShouldEscapeJavaScriptString(string s, bool[] charEscapeFlags)
        {
            if (s == null)
            {
                return false;
            }
            foreach (char c in s)
            {
                if (c >= charEscapeFlags.Length || charEscapeFlags[c])
                {
                    return true;
                }
            }
            return false;
        }

        public static void WriteEscapedJavaScriptString(TextWriter writer, string s, char delimiter, bool appendDelimiters, bool[] charEscapeFlags, StringEscapeHandling stringEscapeHandling, IArrayPool<char> bufferPool, ref char[] writeBuffer)
        {
            if (appendDelimiters)
            {
                writer.Write(delimiter);
            }
            if (s != null)
            {
                int num = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    char c = s[i];
                    if (c >= charEscapeFlags.Length || charEscapeFlags[c])
                    {
                        string text;
                        if (c <= '\\')
                        {
                            switch (c)
                            {
                                case '\b':
                                    text = "\\b";
                                    break;
                                case '\t':
                                    text = "\\t";
                                    break;
                                case '\n':
                                    text = "\\n";
                                    break;
                                case '\v':
                                    goto IL_CB;
                                case '\f':
                                    text = "\\f";
                                    break;
                                case '\r':
                                    text = "\\r";
                                    break;
                                default:
                                    if (c != '\\')
                                    {
                                        goto IL_CB;
                                    }
                                    text = "\\\\";
                                    break;
                            }
                        }
                        else if (c != '\u0085')
                        {
                            if (c != '\u2028')
                            {
                                if (c != '\u2029')
                                {
                                    goto IL_CB;
                                }
                                text = "\\u2029";
                            }
                            else
                            {
                                text = "\\u2028";
                            }
                        }
                        else
                        {
                            text = "\\u0085";
                        }
                    IL_129:
                        if (text == null)
                        {
                            goto IL_1CC;
                        }
                        bool flag = string.Equals(text, "!");
                        if (i > num)
                        {
                            int num2 = i - num + (flag ? 6 : 0);
                            int num3 = flag ? 6 : 0;
                            if (writeBuffer == null || writeBuffer.Length < num2)
                            {
                                char[] array = BufferUtils.RentBuffer(bufferPool, num2);
                                if (flag)
                                {
                                    Array.Copy(writeBuffer, array, 6);
                                }
                                BufferUtils.ReturnBuffer(bufferPool, writeBuffer);
                                writeBuffer = array;
                            }
                            s.CopyTo(num, writeBuffer, num3, num2 - num3);
                            writer.Write(writeBuffer, num3, num2 - num3);
                        }
                        num = i + 1;
                        if (!flag)
                        {
                            writer.Write(text);
                            goto IL_1CC;
                        }
                        writer.Write(writeBuffer, 0, 6);
                        goto IL_1CC;
                    IL_CB:
                        if (c >= charEscapeFlags.Length && stringEscapeHandling != StringEscapeHandling.EscapeNonAscii)
                        {
                            text = null;
                            goto IL_129;
                        }
                        if (c == '\'' && stringEscapeHandling != StringEscapeHandling.EscapeHtml)
                        {
                            text = "\\'";
                            goto IL_129;
                        }
                        if (c == '"' && stringEscapeHandling != StringEscapeHandling.EscapeHtml)
                        {
                            text = "\\\"";
                            goto IL_129;
                        }
                        if (writeBuffer == null || writeBuffer.Length < 6)
                        {
                            writeBuffer = BufferUtils.EnsureBufferSize(bufferPool, 6, writeBuffer);
                        }
                        StringUtils.ToCharAsUnicode(c, writeBuffer);
                        text = "!";
                        goto IL_129;
                    }
                IL_1CC:;
                }
                if (num == 0)
                {
                    writer.Write(s);
                }
                else
                {
                    int num4 = s.Length - num;
                    if (writeBuffer == null || writeBuffer.Length < num4)
                    {
                        writeBuffer = BufferUtils.EnsureBufferSize(bufferPool, num4, writeBuffer);
                    }
                    s.CopyTo(num, writeBuffer, 0, num4);
                    writer.Write(writeBuffer, 0, num4);
                }
            }
            if (appendDelimiters)
            {
                writer.Write(delimiter);
            }
        }

        public static string ToEscapedJavaScriptString(string value, char delimiter, bool appendDelimiters, StringEscapeHandling stringEscapeHandling)
        {
            bool[] charEscapeFlags = JavaScriptUtils.GetCharEscapeFlags(stringEscapeHandling, delimiter);
            string result;
            using (StringWriter stringWriter = StringUtils.CreateStringWriter((value != null) ? value.Length : 16))
            {
                char[] array = null;
                JavaScriptUtils.WriteEscapedJavaScriptString(stringWriter, value, delimiter, appendDelimiters, charEscapeFlags, stringEscapeHandling, null, ref array);
                result = stringWriter.ToString();
            }
            return result;
        }

        internal static readonly bool[] SingleQuoteCharEscapeFlags = new bool[128];

        internal static readonly bool[] DoubleQuoteCharEscapeFlags = new bool[128];

        internal static readonly bool[] HtmlCharEscapeFlags = new bool[128];

        private const int UnicodeTextLength = 6;

        private const string EscapedUnicodeText = "!";
    }
}
