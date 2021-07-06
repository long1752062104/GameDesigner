using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Newtonsoft_X.Json.Utilities
{
    internal class CollectionWrapper<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IWrappedCollection, IList, ICollection
    {
        public CollectionWrapper(IList list)
        {
            ValidationUtils.ArgumentNotNull(list, "list");
            if (list is ICollection<T>)
            {
                _genericCollection = (ICollection<T>)list;
                return;
            }
            _list = list;
        }

        public CollectionWrapper(ICollection<T> list)
        {
            ValidationUtils.ArgumentNotNull(list, "list");
            _genericCollection = list;
        }

        public virtual void Add(T item)
        {
            if (_genericCollection != null)
            {
                _genericCollection.Add(item);
                return;
            }
            _list.Add(item);
        }

        public virtual void Clear()
        {
            if (_genericCollection != null)
            {
                _genericCollection.Clear();
                return;
            }
            _list.Clear();
        }

        public virtual bool Contains(T item)
        {
            if (_genericCollection != null)
            {
                return _genericCollection.Contains(item);
            }
            return _list.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            if (_genericCollection != null)
            {
                _genericCollection.CopyTo(array, arrayIndex);
                return;
            }
            _list.CopyTo(array, arrayIndex);
        }

        public virtual int Count
        {
            get
            {
                if (_genericCollection != null)
                {
                    return _genericCollection.Count;
                }
                return _list.Count;
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                if (_genericCollection != null)
                {
                    return _genericCollection.IsReadOnly;
                }
                return _list.IsReadOnly;
            }
        }

        public virtual bool Remove(T item)
        {
            if (_genericCollection != null)
            {
                return _genericCollection.Remove(item);
            }
            bool flag = _list.Contains(item);
            if (flag)
            {
                _list.Remove(item);
            }
            return flag;
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            if (_genericCollection != null)
            {
                return _genericCollection.GetEnumerator();
            }
            return _list.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_genericCollection != null)
            {
                return _genericCollection.GetEnumerator();
            }
            return _list.GetEnumerator();
        }

        int IList.Add(object value)
        {
            CollectionWrapper<T>.VerifyValueType(value);
            Add((T)value);
            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            return CollectionWrapper<T>.IsCompatibleObject(value) && Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            if (_genericCollection != null)
            {
                throw new InvalidOperationException("Wrapped ICollection<T> does not support IndexOf.");
            }
            if (CollectionWrapper<T>.IsCompatibleObject(value))
            {
                return _list.IndexOf((T)value);
            }
            return -1;
        }

        void IList.RemoveAt(int index)
        {
            if (_genericCollection != null)
            {
                throw new InvalidOperationException("Wrapped ICollection<T> does not support RemoveAt.");
            }
            _list.RemoveAt(index);
        }

        void IList.Insert(int index, object value)
        {
            if (_genericCollection != null)
            {
                throw new InvalidOperationException("Wrapped ICollection<T> does not support Insert.");
            }
            CollectionWrapper<T>.VerifyValueType(value);
            _list.Insert(index, (T)value);
        }

        bool IList.IsFixedSize
        {
            get
            {
                if (_genericCollection != null)
                {
                    return _genericCollection.IsReadOnly;
                }
                return _list.IsFixedSize;
            }
        }

        void IList.Remove(object value)
        {
            if (CollectionWrapper<T>.IsCompatibleObject(value))
            {
                Remove((T)value);
            }
        }

        object IList.this[int index]
        {
            get
            {
                if (_genericCollection != null)
                {
                    throw new InvalidOperationException("Wrapped ICollection<T> does not support indexer.");
                }
                return _list[index];
            }
            set
            {
                if (_genericCollection != null)
                {
                    throw new InvalidOperationException("Wrapped ICollection<T> does not support indexer.");
                }
                CollectionWrapper<T>.VerifyValueType(value);
                _list[index] = (T)value;
            }
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            CopyTo((T[])array, arrayIndex);
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
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

        private static void VerifyValueType(object value)
        {
            if (!CollectionWrapper<T>.IsCompatibleObject(value))
            {
                throw new ArgumentException("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.".FormatWith(CultureInfo.InvariantCulture, value, typeof(T)), "value");
            }
        }

        private static bool IsCompatibleObject(object value)
        {
            return value is T || (value == null && (!typeof(T).IsValueType() || ReflectionUtils.IsNullableType(typeof(T))));
        }

        public object UnderlyingCollection
        {
            get
            {
                if (_genericCollection != null)
                {
                    return _genericCollection;
                }
                return _list;
            }
        }

        private readonly IList _list;

        private readonly ICollection<T> _genericCollection;

        private object _syncRoot;
    }
}
