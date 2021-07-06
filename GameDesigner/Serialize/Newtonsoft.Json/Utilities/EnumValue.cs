using System;

namespace Newtonsoft_X.Json.Utilities
{
    internal class EnumValue<T> where T : struct
    {
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public T Value
        {
            get
            {
                return _value;
            }
        }

        public EnumValue(string name, T value)
        {
            _name = name;
            _value = value;
        }

        private readonly string _name;

        private readonly T _value;
    }
}
