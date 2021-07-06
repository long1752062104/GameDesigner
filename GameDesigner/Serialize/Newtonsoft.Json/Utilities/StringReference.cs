using System;

namespace Newtonsoft_X.Json.Utilities
{
    internal struct StringReference
    {
        public char this[int i]
        {
            get
            {
                return _chars[i];
            }
        }

        public char[] Chars
        {
            get
            {
                return _chars;
            }
        }

        public int StartIndex
        {
            get
            {
                return _startIndex;
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }

        public StringReference(char[] chars, int startIndex, int length)
        {
            _chars = chars;
            _startIndex = startIndex;
            _length = length;
        }

        public override string ToString()
        {
            return new string(_chars, _startIndex, _length);
        }

        private readonly char[] _chars;

        private readonly int _startIndex;

        private readonly int _length;
    }
}
