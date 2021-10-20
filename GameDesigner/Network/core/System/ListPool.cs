using Net.Component;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;

namespace Net.System
{
    internal sealed class Mscorlib_CollectionDebugView<T>
    {
        public Mscorlib_CollectionDebugView(ICollection<T> collection)
        {
            if (collection == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            }
            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[collection.Count];
                collection.CopyTo(array, 0);
                return array;
            }
        }

        private readonly ICollection<T> collection;
    }

    /// <summary>
    /// 线程安全的List类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerTypeProxy(typeof(Mscorlib_CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class ListPool<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        public ListPool()
        {
            _items = _emptyArray;
        }


        public ListPool(int capacity)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (capacity == 0)
            {
                _items = _emptyArray;
                return;
            }
            _items = new T[capacity];
        }


        public ListPool(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            }
            if (!(collection is ICollection<T> collection2))
            {
                _size = 0;
                _items = _emptyArray;
                foreach (T item in collection)
                {
                    Add(item);
                }
                return;
            }
            int count = collection2.Count;
            if (count == 0)
            {
                _items = _emptyArray;
                return;
            }
            _items = new T[count];
            collection2.CopyTo(_items, 0);
            _size = count;
        }


        public int Capacity
        {

            get
            {
                return _items.Length;
            }

            set
            {
                if (value < _size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
                }
                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        T[] array = new T[value];
                        if (_size > 0)
                        {
                            Array.Copy(_items, 0, array, 0, _size);
                        }
                        _items = array;
                        return;
                    }
                    _items = _emptyArray;
                }
            }
        }


        public int Count
        {

            get
            {
                return _size;
            }
        }


        bool IList.IsFixedSize
        {

            get
            {
                return false;
            }
        }


        bool ICollection<T>.IsReadOnly
        {

            get
            {
                return false;
            }
        }


        bool IList.IsReadOnly
        {

            get
            {
                return false;
            }
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
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }


        public T this[int index]
        {

            get
            {
                if (index >= _size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException();
                }
                return _items[index];
            }

            set
            {
                if (index >= _size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException();
                }
                _items[index] = value;
                _version++;
            }
        }

        private static bool IsCompatibleObject(object value)
        {
            return value is T || (value == null && default(T) == null);
        }


        object IList.this[int index]
        {

            get
            {
                return this[index];
            }

            set
            {
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);
                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
                }
            }
        }


        public void Add(T item)
        {
            lock (this)
            {
                write = true;
                SpinWait spinLocal = new SpinWait();
                while (read)
                {
                    spinLocal.SpinOnce();
                }
                if (_size == _items.Length)
                {
                    EnsureCapacity(_size + 1);
                }
                _items[_size] = item;
                _size++;
                _version++;
                write = false;
            }
        }

        int IList.Add(object item)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);
            try
            {
                Add((T)item);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof(T));
            }
            return Count - 1;
        }


        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(_size, collection);
        }


        public ReadOnlyCollection<T> AsReadOnly()
        {
            return new ReadOnlyCollection<T>(this);
        }


        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }
            return Array.BinarySearch(_items, index, count, item, comparer);
        }


        public int BinarySearch(T item)
        {
            return BinarySearch(0, Count, item, null);
        }


        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return BinarySearch(0, Count, item, comparer);
        }


        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size);
                _size = 0;
            }
            _version++;
        }


        public bool Contains(T item)
        {
            if (item == null)
            {
                for (int i = 0; i < _size; i++)
                {
                    if (_items[i] == null)
                    {
                        return true;
                    }
                }
                return false;
            }
            EqualityComparer<T> @default = EqualityComparer<T>.Default;
            for (int j = 0; j < _size; j++)
            {
                if (@default.Equals(_items[j], item))
                {
                    return true;
                }
            }
            return false;
        }


        bool IList.Contains(object item)
        {
            return IsCompatibleObject(item) && Contains((T)item);
        }

        public ListPool<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            if (converter == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.converter);
            }
            ListPool<TOutput> list = new ListPool<TOutput>(_size);
            for (int i = 0; i < _size; i++)
            {
                list._items[i] = converter(_items[i]);
            }
            list._size = _size;
            return list;
        }


        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }


        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array != null && array.Rank != 1)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }
            try
            {
                Array.Copy(_items, 0, array, arrayIndex, _size);
            }
            catch (ArrayTypeMismatchException)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
            }
        }


        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }
            Array.Copy(_items, index, array, arrayIndex, count);
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int num = (_items.Length == 0) ? 4 : (_items.Length * 2);
                if (num > 2146435071)
                {
                    num = 2146435071;
                }
                if (num < min)
                {
                    num = min;
                }
                Capacity = num;
            }
        }


        public bool Exists(Predicate<T> match)
        {
            return FindIndex(match) != -1;
        }


        public T Find(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            for (int i = 0; i < _size; i++)
            {
                if (match(_items[i]))
                {
                    return _items[i];
                }
            }
            return default;
        }


        public List<T> FindAll(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            List<T> list = new List<T>();
            for (int i = 0; i < _size; i++)
            {
                if (match(_items[i]))
                {
                    list.Add(_items[i]);
                }
            }
            return list;
        }


        public int FindIndex(Predicate<T> match)
        {
            return FindIndex(0, _size, match);
        }


        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return FindIndex(startIndex, _size - startIndex, match);
        }


        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            if (startIndex > _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
            }
            if (count < 0 || startIndex > _size - count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
            }
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            int num = startIndex + count;
            for (int i = startIndex; i < num; i++)
            {
                if (match(_items[i]))
                {
                    return i;
                }
            }
            return -1;
        }


        public T FindLast(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            for (int i = _size - 1; i >= 0; i--)
            {
                if (match(_items[i]))
                {
                    return _items[i];
                }
            }
            return default;
        }


        public int FindLastIndex(Predicate<T> match)
        {
            return FindLastIndex(_size - 1, _size, match);
        }


        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return FindLastIndex(startIndex, startIndex + 1, match);
        }


        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            if (_size == 0)
            {
                if (startIndex != -1)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
                }
            }
            else if (startIndex >= _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
            }
            if (count < 0 || startIndex - count + 1 < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
            }
            int num = startIndex - count;
            for (int i = startIndex; i > num; i--)
            {
                if (match(_items[i]))
                {
                    return i;
                }
            }
            return -1;
        }


        public void ForEach(Action<T> action)
        {
            if (action == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            int version = _version;
            int num = 0;
            while (num < _size && (version == _version))
            {
                action(_items[num]);
                num++;
            }
            if (version != _version)
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
            }
        }


        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }


        public T[] GetRange(int index, int count)
        {
            lock (this) 
            {
                read = true;
                SpinWait spinLocal = new SpinWait();
                while (write)
                {
                    spinLocal.SpinOnce();
                }
                if (index < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (count < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (_size - index < count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
                }
                T[] list = new T[count];
                Array.Copy(_items, index, list, 0, count);
                read = false;
                return list;
            }
        }

        private volatile bool read, write;

        /// <summary>
        /// 获取列表对象, 并移除列表, 如果在多线程下, 多线程并行下, 是可以获取到对象, 但是会出现长度不是所指定的长度, 所以获取后要判断一下长度
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public T[] GetRemoveRange(int index, int count) 
        {
            lock (this)
            {
                read = true;
                SpinWait spinLocal = new SpinWait();
                while (write)
                {
                    spinLocal.SpinOnce();
                }
                //获取
                if (index < 0 | count < 0)
                    return null;
                if (_size - index < count)
                    count = _size - index;
                T[] list = new T[count];
                Array.Copy(_items, index, list, 0, count);
                //移除
                _size -= count;
                if (index < _size)
                    Array.Copy(_items, index + count, _items, index, _size - index);
                //Array.Clear(_items, _size, count);
                _version++;
                read = false;
                return list;
            }
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _size);
        }


        int IList.IndexOf(object item)
        {
            if (IsCompatibleObject(item))
            {
                return IndexOf((T)item);
            }
            return -1;
        }


        public int IndexOf(T item, int index)
        {
            if (index > _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
            }
            return Array.IndexOf(_items, item, index, _size - index);
        }


        public int IndexOf(T item, int index, int count)
        {
            if (index > _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
            }
            if (count < 0 || index > _size - count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
            }
            return Array.IndexOf(_items, item, index, count);
        }


        public void Insert(int index, T item)
        {
            if (index > _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_ListInsert);
            }
            if (_size == _items.Length)
            {
                EnsureCapacity(_size + 1);
            }
            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }
            _items[index] = item;
            _size++;
            _version++;
        }


        void IList.Insert(int index, object item)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);
            try
            {
                Insert(index, (T)item);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof(T));
            }
        }


        public void InsertRange(int index, IEnumerable<T> collection)
        {
            lock (this) 
            {
                write = true;
                SpinWait spinLocal = new SpinWait();
                while (read)
                {
                    spinLocal.SpinOnce();
                }
                if (collection == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
                }
                if (index > _size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
                }
                if (collection is ICollection<T> collection2)
                {
                    int count = collection2.Count;
                    if (count > 0)
                    {
                        EnsureCapacity(_size + count);
                        if (index < _size)
                        {
                            Array.Copy(_items, index, _items, index + count, _size - index);
                        }
                        if (this == collection2)
                        {
                            Array.Copy(_items, 0, _items, index, index);
                            Array.Copy(_items, index + count, _items, index * 2, _size - index);
                        }
                        else
                        {
                            T[] array = new T[count];
                            collection2.CopyTo(array, 0);
                            array.CopyTo(_items, index);
                        }
                        _size += count;
                    }
                }
                else
                {
                    foreach (T item in collection)
                    {
                        Insert(index++, item);
                    }
                }
                _version++;
                write = false;
            }
        }

        public int LastIndexOf(T item)
        {
            if (_size == 0)
            {
                return -1;
            }
            return LastIndexOf(item, _size - 1, _size);
        }


        public int LastIndexOf(T item, int index)
        {
            if (index >= _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
            }
            return LastIndexOf(item, index, index + 1);
        }


        public int LastIndexOf(T item, int index, int count)
        {
            if (Count != 0 && index < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (Count != 0 && count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (_size == 0)
            {
                return -1;
            }
            if (index >= _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
            }
            if (count > index + 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
            }
            return Array.LastIndexOf(_items, item, index, count);
        }


        public bool Remove(T item)
        {
            lock (this) 
            {
                write = true;
                SpinWait spinLocal = new SpinWait();
                while (read)
                {
                    spinLocal.SpinOnce();
                }
                if (_size == _items.Length)
                {
                    EnsureCapacity(_size + 1);
                }
                int index = IndexOf(item);
                if (index >= 0)
                {
                    //RemoveAt(index);
                    _size--;
                    if (index < _size)
                    {
                        Array.Copy(_items, index + 1, _items, index, _size - index);
                    }
                    _items[_size] = default;
                    _version++;

                    write = false;
                    return true;
                }
                write = false;
                return false;
            }
        }


        void IList.Remove(object item)
        {
            if (IsCompatibleObject(item))
            {
                Remove((T)item);
            }
        }


        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            int num = 0;
            while (num < _size && !match(_items[num]))
            {
                num++;
            }
            if (num >= _size)
            {
                return 0;
            }
            int i = num + 1;
            while (i < _size)
            {
                while (i < _size && match(_items[i]))
                {
                    i++;
                }
                if (i < _size)
                {
                    _items[num++] = _items[i++];
                }
            }
            Array.Clear(_items, num, _size - num);
            int result = _size - num;
            _size = num;
            _version++;
            return result;
        }


        public void RemoveAt(int index)
        {
            lock (this)
            {
                write = true;
                SpinWait spinLocal = new SpinWait();
                while (read)
                {
                    spinLocal.SpinOnce();
                }
                if (index >= _size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException();
                }
                _size--;
                if (index < _size)
                {
                    Array.Copy(_items, index + 1, _items, index, _size - index);
                }
                _items[_size] = default;
                _version++;
                write = false;
            }
        }


        public void RemoveRange(int index, int count)
        {
            lock (this) 
            {
                read = true;
                SpinWait spinLocal = new SpinWait();
                while (write)
                {
                    spinLocal.SpinOnce();
                }
                if (index < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (count < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (_size - index < count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
                }
                if (count > 0)
                {
                    _size -= count;
                    if (index < _size)
                    {
                        Array.Copy(_items, index + count, _items, index, _size - index);
                    }
                    Array.Clear(_items, _size, count);
                    _version++;
                }
                read = false;
            }
        }


        public void Reverse()
        {
            Reverse(0, Count);
        }


        public void Reverse(int index, int count)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }
            Array.Reverse(_items, index, count);
            _version++;
        }


        public void Sort()
        {
            Sort(0, Count, null);
        }


        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }


        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }
            Array.Sort(_items, index, count, comparer);
            _version++;
        }

        internal sealed class FunctorComparer<T1> : IComparer<T1>
        {
            public FunctorComparer(Comparison<T1> comparison)
            {
                this.comparison = comparison;
            }

            public int Compare(T1 x, T1 y)
            {
                return comparison(x, y);
            }

            private readonly Comparison<T1> comparison;
        }

        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            if (_size > 0)
            {
                IComparer<T> comparer = new FunctorComparer<T>(comparison);
                Array.Sort(_items, 0, _size, comparer);
            }
        }


        public T[] ToArray()
        {
            T[] array = new T[_size];
            Array.Copy(_items, 0, array, 0, _size);
            return array;
        }


        public void TrimExcess()
        {
            int num = (int)(_items.Length * 0.9);
            if (_size < num)
            {
                Capacity = _size;
            }
        }


        public bool TrueForAll(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            for (int i = 0; i < _size; i++)
            {
                if (!match(_items[i]))
                {
                    return false;
                }
            }
            return true;
        }

        internal static IList<T> Synchronized(ListPool<T> list)
        {
            return new SynchronizedList(list);
        }

        private T[] _items;

        private int _size;

        private int _version;

        [NonSerialized]
        private object _syncRoot;

        private static readonly T[] _emptyArray = new T[0];

        [Serializable]
        internal class SynchronizedList : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
        {
            internal SynchronizedList(ListPool<T> list)
            {
                _list = list;
                _root = ((ICollection)list).SyncRoot;
            }

            public int Count
            {
                get
                {
                    object root = _root;
                    int count;
                    lock (root)
                    {
                        count = _list.Count;
                    }
                    return count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return ((ICollection<T>)_list).IsReadOnly;
                }
            }

            public void Add(T item)
            {
                object root = _root;
                lock (root)
                {
                    _list.Add(item);
                }
            }

            public void Clear()
            {
                object root = _root;
                lock (root)
                {
                    _list.Clear();
                }
            }

            public bool Contains(T item)
            {
                object root = _root;
                bool result;
                lock (root)
                {
                    result = _list.Contains(item);
                }
                return result;
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                object root = _root;
                lock (root)
                {
                    _list.CopyTo(array, arrayIndex);
                }
            }

            public bool Remove(T item)
            {
                object root = _root;
                bool result;
                lock (root)
                {
                    result = _list.Remove(item);
                }
                return result;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                object root = _root;
                IEnumerator result;
                lock (root)
                {
                    result = _list.GetEnumerator();
                }
                return result;
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                object root = _root;
                IEnumerator<T> enumerator;
                lock (root)
                {
                    enumerator = ((IEnumerable<T>)_list).GetEnumerator();
                }
                return enumerator;
            }

            public T this[int index]
            {
                get
                {
                    object root = _root;
                    T result;
                    lock (root)
                    {
                        result = _list[index];
                    }
                    return result;
                }
                set
                {
                    object root = _root;
                    lock (root)
                    {
                        _list[index] = value;
                    }
                }
            }

            public int IndexOf(T item)
            {
                object root = _root;
                int result;
                lock (root)
                {
                    result = _list.IndexOf(item);
                }
                return result;
            }

            public void Insert(int index, T item)
            {
                object root = _root;
                lock (root)
                {
                    _list.Insert(index, item);
                }
            }

            public void RemoveAt(int index)
            {
                object root = _root;
                lock (root)
                {
                    _list.RemoveAt(index);
                }
            }

            private readonly ListPool<T> _list;

            private readonly object _root;
        }


        [Serializable]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            internal Enumerator(ListPool<T> list)
            {
                this.list = list;
                index = 0;
                version = list._version;
                current = default;
            }


            public void Dispose()
            {
            }


            public bool MoveNext()
            {
                ListPool<T> list = this.list;
                if (version == list._version && index < list._size)
                {
                    current = list._items[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (version != list._version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                index = list._size + 1;
                current = default;
                return false;
            }


            public T Current
            {

                get
                {
                    return current;
                }
            }


            object IEnumerator.Current
            {

                get
                {
                    if (index == 0 || index == list._size + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return Current;
                }
            }


            void IEnumerator.Reset()
            {
                if (version != list._version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                index = 0;
                current = default;
            }

            private readonly ListPool<T> list;

            private int index;

            private readonly int version;

            private T current;
        }
    }
}