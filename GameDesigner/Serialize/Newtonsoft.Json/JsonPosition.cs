using Newtonsoft_X.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Newtonsoft_X.Json
{
    internal struct JsonPosition
    {
        public JsonPosition(JsonContainerType type)
        {
            Type = type;
            HasIndex = JsonPosition.TypeHasIndex(type);
            Position = -1;
            PropertyName = null;
        }

        internal int CalculateLength()
        {
            switch (Type)
            {
                case JsonContainerType.Object:
                    return PropertyName.Length + 5;
                case JsonContainerType.Array:
                case JsonContainerType.Constructor:
                    return MathUtils.IntLength((ulong)Position) + 2;
                default:
                    throw new ArgumentOutOfRangeException("Type");
            }
        }

        internal void WriteTo(StringBuilder sb)
        {
            switch (Type)
            {
                case JsonContainerType.Object:
                    {
                        string propertyName = PropertyName;
                        if (propertyName.IndexOfAny(JsonPosition.SpecialCharacters) != -1)
                        {
                            sb.Append("['");
                            sb.Append(propertyName);
                            sb.Append("']");
                            return;
                        }
                        if (sb.Length > 0)
                        {
                            sb.Append('.');
                        }
                        sb.Append(propertyName);
                        return;
                    }
                case JsonContainerType.Array:
                case JsonContainerType.Constructor:
                    sb.Append('[');
                    sb.Append(Position);
                    sb.Append(']');
                    return;
                default:
                    return;
            }
        }

        internal static bool TypeHasIndex(JsonContainerType type)
        {
            return type == JsonContainerType.Array || type == JsonContainerType.Constructor;
        }

        internal static string BuildPath(List<JsonPosition> positions, JsonPosition? currentPosition)
        {
            int num = 0;
            if (positions != null)
            {
                for (int i = 0; i < positions.Count; i++)
                {
                    num += positions[i].CalculateLength();
                }
            }
            if (currentPosition != null)
            {
                num += currentPosition.GetValueOrDefault().CalculateLength();
            }
            StringBuilder stringBuilder = new StringBuilder(num);
            if (positions != null)
            {
                foreach (JsonPosition jsonPosition in positions)
                {
                    jsonPosition.WriteTo(stringBuilder);
                }
            }
            if (currentPosition != null)
            {
                currentPosition.GetValueOrDefault().WriteTo(stringBuilder);
            }
            return stringBuilder.ToString();
        }

        internal static string FormatMessage(IJsonLineInfo lineInfo, string path, string message)
        {
            if (!message.EndsWith(Environment.NewLine, StringComparison.Ordinal))
            {
                message = message.Trim();
                if (!message.EndsWith('.'))
                {
                    message += ".";
                }
                message += " ";
            }
            message += "Path '{0}'".FormatWith(CultureInfo.InvariantCulture, path);
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                message += ", line {0}, position {1}".FormatWith(CultureInfo.InvariantCulture, lineInfo.LineNumber, lineInfo.LinePosition);
            }
            message += ".";
            return message;
        }

        private static readonly char[] SpecialCharacters = new char[]
        {
            '.',
            ' ',
            '[',
            ']',
            '(',
            ')'
        };

        internal JsonContainerType Type;

        internal int Position;

        internal string PropertyName;

        internal bool HasIndex;
    }
}
