using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Newtonsoft_X.Json.Utilities
{
    internal class DictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IWrappedDictionary, IDictionary, ICollection
    {
        public DictionaryWrapper(IDictionary dictionary)
        {
            ValidationUtils.ArgumentNotNull(dictionary, "dictionary");
            _dictionary = dictionary;
        }

        public DictionaryWrapper(IDictionary<TKey, TValue> dictionary)
        {
            ValidationUtils.ArgumentNotNull(dictionary, "dictionary");
            _genericDictionary = dictionary;
        }

        public void Add(TKey key, TValue value)
        {
            if (_dictionary != null)
            {
                _dictionary.Add(key, value);
                return;
            }
            if (_genericDictionary != null)
            {
                _genericDictionary.Add(key, value);
                return;
            }
            throw new NotSupportedException();
        }

        public bool ContainsKey(TKey key)
        {
            if (_dictionary != null)
            {
                return _dictionary.Contains(key);
            }
            return _genericDictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary.Keys.Cast<TKey>().ToList<TKey>();
                }
                return _genericDictionary.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            if (_dictionary == null)
            {
                return _genericDictionary.Remove(key);
            }
            if (_dictionary.Contains(key))
            {
                _dictionary.Remove(key);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dictionary == null)
            {
                return _genericDictionary.TryGetValue(key, out value);
            }
            if (!_dictionary.Contains(key))
            {
                value = default(TValue);
                return false;
            }
            value = (TValue)_dictionary[key];
            return true;
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary.Values.Cast<TValue>().ToList<TValue>();
                }
                return _genericDictionary.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_dictionary != null)
                {
                    return (TValue)_dictionary[key];
                }
                return _genericDictionary[key];
            }
            set
            {
                if (_dictionary != null)
                {
                    _dictionary[key] = value;
                    return;
                }
                _genericDictionary[key] = value;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary != null)
            {
                ((IList)_dictionary).Add(item);
                return;
            }
            if (_genericDictionary != null)
            {
                _genericDictionary.Add(item);
            }
        }

        public void Clear()
        {
            if (_dictionary != null)
            {
                _dictionary.Clear();
                return;
            }
            _genericDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary != null)
            {
                return ((IList)_dictionary).Contains(item);
            }
            return _genericDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (_dictionary != null)
            {

                {
                    IDictionaryEnumerator enumerator = _dictionary.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        DictionaryEntry entry = enumerator.Entry;
                        array[arrayIndex++] = new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
                    }
                    return;
                }
            }
            _genericDictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary.Count;
                }
                return _genericDictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary.IsReadOnly;
                }
                return _genericDictionary.IsReadOnly;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary == null)
            {
                return _genericDictionary.Remove(item);
            }
            if (!_dictionary.Contains(item.Key))
            {
                return true;
            }
            if (object.Equals(_dictionary[item.Key], item.Value))
            {
                _dictionary.Remove(item.Key);
                return true;
            }
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (_dictionary != null)
            {
                return (from DictionaryEntry de in _dictionary
                        select new KeyValuePair<TKey, TValue>((TKey)de.Key, (TValue)de.Value)).GetEnumerator();
            }
            return _genericDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IDictionary.Add(object key, object value)
        {
            if (_dictionary != null)
            {
                _dictionary.Add(key, value);
                return;
            }
            _genericDictionary.Add((TKey)key, (TValue)value);
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary[key];
                }
                return _genericDictionary[(TKey)key];
            }
            set
            {
                if (_dictionary != null)
                {
                    _dictionary[key] = value;
                    return;
                }
                _genericDictionary[(TKey)key] = (TValue)value;
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            if (_dictionary != null)
            {
                return _dictionary.GetEnumerator();
            }
            return new DictionaryWrapper<TKey, TValue>.DictionaryEnumerator<TKey, TValue>(_genericDictionary.GetEnumerator());
        }

        bool IDictionary.Contains(object key)
        {
            if (_genericDictionary != null)
            {
                return _genericDictionary.ContainsKey((TKey)key);
            }
            return _dictionary.Contains(key);
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return _genericDictionary == null && _dictionary.IsFixedSize;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                if (_genericDictionary != null)
                {
                    return _genericDictionary.Keys.ToList<TKey>();
                }
                return _dictionary.Keys;
            }
        }

        public void Remove(object key)
        {
            if (_dictionary != null)
            {
                _dictionary.Remove(key);
                return;
            }
            _genericDictionary.Remove((TKey)key);
        }

        ICollection IDictionary.Values
        {
            get
            {
                if (_genericDictionary != null)
                {
                    return _genericDictionary.Values.ToList<TValue>();
                }
                return _dictionary.Values;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (_dictionary != null)
            {
                _dictionary.CopyTo(array, index);
                return;
            }
            _genericDictionary.CopyTo((KeyValuePair<TKey, TValue>[])array, index);
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return _dictionary != null && _dictionary.IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        public object UnderlyingDictionary
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary;
                }
                return _genericDictionary;
            }
        }

        private readonly IDictionary _dictionary;

        private readonly IDictionary<TKey, TValue> _genericDictionary;

        private object _syncRoot;

        private struct DictionaryEnumerator<TEnumeratorKey, TEnumeratorValue> : IDictionaryEnumerator, IEnumerator
        {
            public DictionaryEnumerator(IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
            {
                ValidationUtils.ArgumentNotNull(e, "e");
                _e = e;
            }

            public DictionaryEntry Entry
            {
                get
                {
                    return (DictionaryEntry)Current;
                }
            }

            public object Key
            {
                get
                {
                    return Entry.Key;
                }
            }

            public object Value
            {
                get
                {
                    return Entry.Value;
                }
            }

            public object Current
            {
                get
                {
                    KeyValuePair<TEnumeratorKey, TEnumeratorValue> keyValuePair = _e.Current;
                    object key = keyValuePair.Key;
                    keyValuePair = _e.Current;
                    return new DictionaryEntry(key, keyValuePair.Value);
                }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            public void Reset()
            {
                _e.Reset();
            }

            private readonly IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;
        }
    }
}
