using System;
using System.Collections.Generic;
using System.Threading;

namespace Newtonsoft_X.Json.Utilities
{
    internal class ThreadSafeStore<TKey, TValue>
    {
        public ThreadSafeStore(Func<TKey, TValue> creator)
        {
            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }
            _creator = creator;
            _store = new Dictionary<TKey, TValue>();
        }

        public TValue Get(TKey key)
        {
            if (!_store.TryGetValue(key, out TValue result))
            {
                return AddValue(key);
            }
            return result;
        }

        private TValue AddValue(TKey key)
        {
            TValue tvalue = _creator(key);
            object @lock = _lock;
            TValue result2;
            lock (@lock)
            {
                if (_store == null)
                {
                    _store = new Dictionary<TKey, TValue>();
                    _store[key] = tvalue;
                }
                else
                {
                    if (_store.TryGetValue(key, out TValue result))
                    {
                        return result;
                    }
                    Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(_store);
                    dictionary[key] = tvalue;
                    Thread.MemoryBarrier();
                    _store = dictionary;
                }
                result2 = tvalue;
            }
            return result2;
        }

        private readonly object _lock = new object();

        private Dictionary<TKey, TValue> _store;

        private readonly Func<TKey, TValue> _creator;
    }
}
