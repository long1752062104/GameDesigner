using System;

namespace Newtonsoft_X.Json.Utilities
{
    internal class PropertyNameTable
    {
        public PropertyNameTable()
        {
            _entries = new Entry[_mask + 1];
        }

        public string Get(char[] key, int start, int length)
        {
            if (length == 0)
            {
                return string.Empty;
            }
            int num = length + HashCodeRandomizer;
            num += (num << 7 ^ key[start]);
            int num2 = start + length;
            for (int i = start + 1; i < num2; i++)
            {
                num += (num << 7 ^ key[i]);
            }
            num -= num >> 17;
            num -= num >> 11;
            num -= num >> 5;
            for (PropertyNameTable.Entry entry = _entries[num & _mask]; entry != null; entry = entry.Next)
            {
                if (entry.HashCode == num && PropertyNameTable.TextEquals(entry.Value, key, start, length))
                {
                    return entry.Value;
                }
            }
            return null;
        }

        public string Add(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            int length = key.Length;
            if (length == 0)
            {
                return string.Empty;
            }
            int num = length + PropertyNameTable.HashCodeRandomizer;
            for (int i = 0; i < key.Length; i++)
            {
                num += (num << 7 ^ key[i]);
            }
            num -= num >> 17;
            num -= num >> 11;
            num -= num >> 5;
            for (PropertyNameTable.Entry entry = _entries[num & _mask]; entry != null; entry = entry.Next)
            {
                if (entry.HashCode == num && entry.Value.Equals(key))
                {
                    return entry.Value;
                }
            }
            return AddEntry(key, num);
        }

        private string AddEntry(string str, int hashCode)
        {
            int num = hashCode & _mask;
            Entry entry = new Entry(str, hashCode, _entries[num]);
            _entries[num] = entry;
            int count = _count;
            _count = count + 1;
            if (count == _mask)
            {
                Grow();
            }
            return entry.Value;
        }

        private void Grow()
        {
            Entry[] entries = _entries;
            int newMask = (_mask * 2) + 1;
            Entry[] newEntries = new Entry[newMask + 1];

            for (int i = 0; i < entries.Length; i++)
            {
                Entry next;
                for (Entry entry = entries[i]; entry != null; entry = next)
                {
                    int index = entry.HashCode & newMask;
                    next = entry.Next;
                    entry.Next = newEntries[index];
                    newEntries[index] = entry;
                }
            }
            _entries = newEntries;
            _mask = newMask;
        }

        private static bool TextEquals(string str1, char[] str2, int str2Start, int str2Length)
        {
            if (str1.Length != str2Length)
            {
                return false;
            }
            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i] != str2[str2Start + i])
                {
                    return false;
                }
            }
            return true;
        }

        private static readonly int HashCodeRandomizer = Environment.TickCount;

        private int _count;

        private Entry[] _entries;

        private int _mask = 31;

        private class Entry
        {
            internal Entry(string value, int hashCode, Entry next)
            {
                Value = value;
                HashCode = hashCode;
                Next = next;
            }

            internal readonly string Value;

            internal readonly int HashCode;

            internal PropertyNameTable.Entry Next;
        }
    }
}
