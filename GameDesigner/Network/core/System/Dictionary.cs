using Net.Share;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;

namespace Net.System
{
    internal sealed class Mscorlib_DictionaryDebugView<K, V>
    {
        private readonly IDictionary<K, V> dict;

        public Mscorlib_DictionaryDebugView(IDictionary<K, V> dictionary)
        {
            if (dictionary == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);

            dict = dictionary;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<K, V>[] Items
        {
            get
            {
                KeyValuePair<K, V>[] items = new KeyValuePair<K, V>[dict.Count];
                dict.CopyTo(items, 0);
                return items;
            }
        }
    }

    [DebuggerTypeProxy(typeof(Mscorlib_DictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    //[Serializable]//unity新版本会记录起来, 导致开始游戏后, 被unity初始化赋值 导致bug
    [ComVisible(false)]
    public class MyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, ISerializable, IDeserializationCallback
    {

        public MyDictionary() : this(1, null)
        {
        }


        public MyDictionary(int capacity) : this(capacity, null)
        {
        }


        public MyDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer)
        {
        }


        public MyDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
            }
            if (capacity > 0)
            {
                Initialize(capacity);
            }
            this.comparer = (comparer ?? EqualityComparer<TKey>.Default);
        }


        public MyDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null)
        {
        }


        public MyDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : this((dictionary != null) ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
            }
            foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
            {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        protected MyDictionary(SerializationInfo info, StreamingContext context)
        {
            HashHelpers.SerializationInfoTable.Add(this, info);
        }


        public IEqualityComparer<TKey> Comparer
        {

            get
            {
                return comparer;
            }
        }

        public int Count
        {

            get
            {
                return count - freeCount;
            }
        }


        public KeyCollection Keys
        {

            get
            {
                if (keys == null)
                {
                    keys = new KeyCollection(this);
                }
                return keys;
            }
        }


        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {

            get
            {
                if (keys == null)
                {
                    keys = new KeyCollection(this);
                }
                return keys;
            }
        }


        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {

            get
            {
                if (keys == null)
                {
                    keys = new KeyCollection(this);
                }
                return keys;
            }
        }


        public ValueCollection Values
        {

            get
            {
                if (values == null)
                {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }


        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {

            get
            {
                if (values == null)
                {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }


        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {

            get
            {
                if (values == null)
                {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }


        public TValue this[TKey key]
        {

            get
            {
                int num = FindEntry(key);
                if (num >= 0)
                {
                    return entries[num].value;
                }
                ThrowHelper.ThrowKeyNotFoundException();
                return default;
            }

            set
            {
                Insert(key, value, false);
            }
        }


        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }


        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int num = FindEntry(keyValuePair.Key);
            return num >= 0 && EqualityComparer<TValue>.Default.Equals(entries[num].value, keyValuePair.Value);
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int num = FindEntry(keyValuePair.Key);
            if (num >= 0 && EqualityComparer<TValue>.Default.Equals(entries[num].value, keyValuePair.Value))
            {
                Remove(keyValuePair.Key);
                return true;
            }
            return false;
        }


        public void Clear()
        {
            if (count > 0)
            {
                for (int i = 0; i < buckets.Length; i++)
                {
                    buckets[i] = -1;
                }
                Array.Clear(entries, 0, count);
                freeList = -1;
                count = 0;
                freeCount = 0;
                version++;
            }
        }


        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }


        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && entries[i].value == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
                for (int j = 0; j < count; j++)
                {
                    if (entries[j].hashCode >= 0 && @default.Equals(entries[j].value, value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }
            if (index < 0 || index > array.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (array.Length - index < Count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            int num = count;
            Entry[] array2 = entries;
            for (int i = 0; i < num; i++)
            {
                if (array2[i].hashCode >= 0)
                {
                    array[index++] = new KeyValuePair<TKey, TValue>(array2[i].key, array2[i].value);
                }
            }
        }


        public Enumerator GetEnumerator()
        {
            return new Enumerator(this, 2);
        }


        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }

        [SecurityCritical]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.info);
            }
            info.AddValue("Version", version);
            info.AddValue("Comparer", HashHelpers.GetEqualityComparerForSerialization(comparer), typeof(IEqualityComparer<TKey>));
            info.AddValue("HashSize", buckets.Length);
            KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[Count];
            CopyTo(array, 0);
            info.AddValue("KeyValuePairs", array, typeof(KeyValuePair<TKey, TValue>[]));
        }

        private int FindEntry(TKey key)
        {
            int num = key.GetHashCode() & int.MaxValue;
            for (int i = buckets[num % buckets.Length]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == num)
                {
                    return i;
                }
            }
            return -1;
        }

        private void Initialize(int capacity)
        {
            int prime = HashHelpers.GetPrime(capacity);
            buckets = new int[prime];
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = -1;
            }
            entries = new Entry[prime];
            freeList = -1;
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            int num = key.GetHashCode() & int.MaxValue;
            int num2 = num % buckets.Length;
            int num3 = 0;
            for (int i = buckets[num2]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == num && comparer.Equals(entries[i].key, key))
                {
                    if (add)
                        throw new Exception($"已经有{key}键存在!, 添加失败!");
                    entries[i].value = value;
                    version++;
                    return;
                }
                num3++;
            }
            int num4;
            if (freeCount > 0)
            {
                num4 = freeList;
                freeList = entries[num4].next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length)
                {
                    Resize();
                    num2 = num % buckets.Length;
                }
                num4 = count;
                count++;
            }
            entries[num4].hashCode = num;
            entries[num4].next = buckets[num2];
            entries[num4].key = key;
            entries[num4].value = value;
            buckets[num2] = num4;
            version++;
            if (num3 > 100 && HashHelpers.IsWellKnownEqualityComparer(comparer))
            {
                comparer = (IEqualityComparer<TKey>)HashHelpers.GetRandomizedEqualityComparer(comparer);
                Resize(entries.Length, true);
            }
        }

        public virtual void OnDeserialization(object sender)
        {
            HashHelpers.SerializationInfoTable.TryGetValue(this, out SerializationInfo serializationInfo);
            if (serializationInfo == null)
            {
                return;
            }
            int @int = serializationInfo.GetInt32("Version");
            int int2 = serializationInfo.GetInt32("HashSize");
            comparer = (IEqualityComparer<TKey>)serializationInfo.GetValue("Comparer", typeof(IEqualityComparer<TKey>));
            if (int2 != 0)
            {
                buckets = new int[int2];
                for (int i = 0; i < buckets.Length; i++)
                {
                    buckets[i] = -1;
                }
                entries = new Entry[int2];
                freeList = -1;
                KeyValuePair<TKey, TValue>[] array = (KeyValuePair<TKey, TValue>[])serializationInfo.GetValue("KeyValuePairs", typeof(KeyValuePair<TKey, TValue>[]));
                if (array == null)
                {
                    ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_MissingKeys);
                }
                for (int j = 0; j < array.Length; j++)
                {
                    if (array[j].Key == null)
                    {
                        ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_NullKey);
                    }
                    Insert(array[j].Key, array[j].Value, true);
                }
            }
            else
            {
                buckets = null;
            }
            version = @int;
            HashHelpers.SerializationInfoTable.Remove(this);
        }

        private void Resize()
        {
            Resize(HashHelpers.ExpandPrime(count), false);
        }

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            int[] array = new int[newSize];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = -1;
            }
            Entry[] array2 = new Entry[newSize];
            Array.Copy(entries, 0, array2, 0, count);
            if (forceNewHashCodes)
            {
                for (int j = 0; j < count; j++)
                {
                    if (array2[j].hashCode != -1)
                    {
                        array2[j].hashCode = (comparer.GetHashCode(array2[j].key) & int.MaxValue);
                    }
                }
            }
            for (int k = 0; k < count; k++)
            {
                if (array2[k].hashCode >= 0)
                {
                    int num = array2[k].hashCode % newSize;
                    array2[k].next = array[num];
                    array[num] = k;
                }
            }
            buckets = array;
            entries = array2;
        }


        public bool Remove(TKey key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            int num = comparer.GetHashCode(key) & int.MaxValue;
            int num2 = num % buckets.Length;
            int num3 = -1;
            for (int i = buckets[num2]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == num && comparer.Equals(entries[i].key, key))
                {
                    if (num3 < 0)
                    {
                        buckets[num2] = entries[i].next;
                    }
                    else
                    {
                        entries[num3].next = entries[i].next;
                    }
                    entries[i].hashCode = -1;
                    entries[i].next = freeList;
                    entries[i].key = default;
                    entries[i].value = default;
                    freeList = i;
                    freeCount++;
                    version++;
                    return true;
                }
                num3 = i;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int num = FindEntry(key);
            if (num >= 0)
            {
                value = entries[num].value;
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value, out int index)
        {
            index = FindEntry(key);
            if (index >= 0)
            {
                value = entries[index].value;
                return true;
            }
            value = default;
            return false;
        }

        internal TValue GetValueOrDefault(TKey key)
        {
            int num = FindEntry(key);
            if (num >= 0)
            {
                return entries[num].value;
            }
            return default;
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {

            get
            {
                return false;
            }
        }


        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            CopyTo(array, index);
        }


        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }
            if (array.Rank != 1)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }
            if (array.GetLowerBound(0) != 0)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
            }
            if (index < 0 || index > array.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (array.Length - index < Count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            if (array is KeyValuePair<TKey, TValue>[] array2)
            {
                CopyTo(array2, index);
                return;
            }
            if (array is DictionaryEntry[])
            {
                DictionaryEntry[] array3 = array as DictionaryEntry[];
                Entry[] array4 = entries;
                for (int i = 0; i < count; i++)
                {
                    if (array4[i].hashCode >= 0)
                    {
                        array3[index++] = new DictionaryEntry(array4[i].key, array4[i].value);
                    }
                }
                return;
            }
            object[] array5 = array as object[];
            if (array5 == null)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
            }
            try
            {
                int num = count;
                Entry[] array6 = entries;
                for (int j = 0; j < num; j++)
                {
                    if (array6[j].hashCode >= 0)
                    {
                        array5[index++] = new KeyValuePair<TKey, TValue>(array6[j].key, array6[j].value);
                    }
                }
            }
            catch (ArrayTypeMismatchException)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, 2);
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


        bool IDictionary.IsFixedSize
        {

            get
            {
                return false;
            }
        }


        bool IDictionary.IsReadOnly
        {

            get
            {
                return false;
            }
        }


        ICollection IDictionary.Keys
        {

            get
            {
                return Keys;
            }
        }


        ICollection IDictionary.Values
        {

            get
            {
                return Values;
            }
        }


        object IDictionary.this[object key]
        {

            get
            {
                if (IsCompatibleKey(key))
                {
                    int num = FindEntry((TKey)key);
                    if (num >= 0)
                    {
                        return entries[num].value;
                    }
                }
                return null;
            }

            set
            {
                if (key == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
                }
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);
                try
                {
                    TKey key2 = (TKey)key;
                    try
                    {
                        this[key2] = (TValue)value;
                    }
                    catch (InvalidCastException)
                    {
                        ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                    }
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
                }
            }
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            return key is TKey;
        }


        void IDictionary.Add(object key, object value)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);
            try
            {
                TKey key2 = (TKey)key;
                try
                {
                    Add(key2, (TValue)value);
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                }
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
            }
        }


        bool IDictionary.Contains(object key)
        {
            return IsCompatibleKey(key) && ContainsKey((TKey)key);
        }


        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this, 1);
        }


        void IDictionary.Remove(object key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key);
            }
        }

        private int[] buckets;

        public Entry[] entries;//unity的editor用到

        public int count;

        private int version;

        private int freeList;

        private int freeCount;

        private IEqualityComparer<TKey> comparer;

        private KeyCollection keys;

        private ValueCollection values;

        private object _syncRoot;

        public struct Entry
        {
            public int hashCode;

            public int next;

            public TKey key;

            public TValue value;
        }


        [Serializable]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator, IDictionaryEnumerator
        {
            internal Enumerator(MyDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                this.dictionary = dictionary;
                version = dictionary.version;
                index = 0;
                this.getEnumeratorRetType = getEnumeratorRetType;
                current = default;
            }


            public bool MoveNext()
            {
                if (version != dictionary.version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                while (index < dictionary.count)
                {
                    if (dictionary.entries[index].hashCode >= 0)
                    {
                        current = new KeyValuePair<TKey, TValue>(dictionary.entries[index].key, dictionary.entries[index].value);
                        index++;
                        return true;
                    }
                    index++;
                }
                index = dictionary.count + 1;
                current = default;
                return false;
            }


            public KeyValuePair<TKey, TValue> Current
            {

                get
                {
                    return current;
                }
            }


            public void Dispose()
            {
            }


            object IEnumerator.Current
            {

                get
                {
                    if (index == 0 || index == dictionary.count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    if (getEnumeratorRetType == 1)
                    {
                        return new DictionaryEntry(current.Key, current.Value);
                    }
                    return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                }
            }


            void IEnumerator.Reset()
            {
                if (version != dictionary.version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                index = 0;
                current = default;
            }


            DictionaryEntry IDictionaryEnumerator.Entry
            {

                get
                {
                    if (index == 0 || index == dictionary.count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return new DictionaryEntry(current.Key, current.Value);
                }
            }


            object IDictionaryEnumerator.Key
            {

                get
                {
                    if (index == 0 || index == dictionary.count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return current.Key;
                }
            }


            object IDictionaryEnumerator.Value
            {

                get
                {
                    if (index == 0 || index == dictionary.count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return current.Value;
                }
            }

            private readonly MyDictionary<TKey, TValue> dictionary;

            private readonly int version;

            private int index;

            private KeyValuePair<TKey, TValue> current;

            private readonly int getEnumeratorRetType;

            internal const int DictEntry = 1;

            internal const int KeyValuePair = 2;
        }


        [DebuggerDisplay("Count = {Count}")]

        [Serializable]
        public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable, ICollection, IReadOnlyCollection<TKey>
        {

            public KeyCollection(MyDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                }
                this.dictionary = dictionary;
            }


            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }


            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (index < 0 || index > array.Length)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0)
                    {
                        array[index++] = entries[i].key;
                    }
                }
            }


            public int Count
            {

                get
                {
                    return dictionary.Count;
                }
            }


            bool ICollection<TKey>.IsReadOnly
            {

                get
                {
                    return true;
                }
            }


            void ICollection<TKey>.Add(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }


            void ICollection<TKey>.Clear()
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }


            bool ICollection<TKey>.Contains(TKey item)
            {
                return dictionary.ContainsKey(item);
            }


            bool ICollection<TKey>.Remove(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
                return false;
            }


            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }


            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }


            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (array.Rank != 1)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }
                if (array.GetLowerBound(0) != 0)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }
                if (index < 0 || index > array.Length)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                if (array is TKey[] array2)
                {
                    CopyTo(array2, index);
                    return;
                }
                object[] array3 = array as object[];
                if (array3 == null)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                }
                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (entries[i].hashCode >= 0)
                        {
                            array3[index++] = entries[i].key;
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
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
                    return ((ICollection)dictionary).SyncRoot;
                }
            }

            private readonly MyDictionary<TKey, TValue> dictionary;


            [Serializable]
            public struct Enumerator : IEnumerator<TKey>, IDisposable, IEnumerator
            {
                internal Enumerator(MyDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentKey = default;
                }


                public void Dispose()
                {
                }


                public bool MoveNext()
                {
                    if (version != dictionary.version)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    while (index < dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentKey = dictionary.entries[index].key;
                            index++;
                            return true;
                        }
                        index++;
                    }
                    index = dictionary.count + 1;
                    currentKey = default;
                    return false;
                }


                public TKey Current
                {

                    get
                    {
                        return currentKey;
                    }
                }


                object IEnumerator.Current
                {

                    get
                    {
                        if (index == 0 || index == dictionary.count + 1)
                        {
                            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                        }
                        return currentKey;
                    }
                }


                void IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    index = 0;
                    currentKey = default;
                }

                private readonly MyDictionary<TKey, TValue> dictionary;

                private int index;

                private readonly int version;

                private TKey currentKey;
            }
        }


        [DebuggerDisplay("Count = {Count}")]

        [Serializable]
        public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, ICollection, IReadOnlyCollection<TValue>
        {

            public ValueCollection(MyDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                }
                this.dictionary = dictionary;
            }


            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }


            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (index < 0 || index > array.Length)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0)
                    {
                        array[index++] = entries[i].value;
                    }
                }
            }


            public int Count
            {

                get
                {
                    return dictionary.Count;
                }
            }


            bool ICollection<TValue>.IsReadOnly
            {

                get
                {
                    return true;
                }
            }


            void ICollection<TValue>.Add(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }


            bool ICollection<TValue>.Remove(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
                return false;
            }


            void ICollection<TValue>.Clear()
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }


            bool ICollection<TValue>.Contains(TValue item)
            {
                return dictionary.ContainsValue(item);
            }


            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }


            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }


            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (array.Rank != 1)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }
                if (array.GetLowerBound(0) != 0)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }
                if (index < 0 || index > array.Length)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (array.Length - index < dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                if (array is TValue[] array2)
                {
                    CopyTo(array2, index);
                    return;
                }
                object[] array3 = array as object[];
                if (array3 == null)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
                }
                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (entries[i].hashCode >= 0)
                        {
                            array3[index++] = entries[i].value;
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
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
                    return ((ICollection)dictionary).SyncRoot;
                }
            }

            private readonly MyDictionary<TKey, TValue> dictionary;


            [Serializable]
            public struct Enumerator : IEnumerator<TValue>, IDisposable, IEnumerator
            {
                internal Enumerator(MyDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentValue = default;
                }


                public void Dispose()
                {
                }


                public bool MoveNext()
                {
                    if (version != dictionary.version)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    while (index < dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentValue = dictionary.entries[index].value;
                            index++;
                            return true;
                        }
                        index++;
                    }
                    index = dictionary.count + 1;
                    currentValue = default;
                    return false;
                }


                public TValue Current
                {

                    get
                    {
                        return currentValue;
                    }
                }


                object IEnumerator.Current
                {

                    get
                    {
                        if (index == 0 || index == dictionary.count + 1)
                        {
                            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                        }
                        return currentValue;
                    }
                }


                void IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                    }
                    index = 0;
                    currentValue = default;
                }

                private readonly MyDictionary<TKey, TValue> dictionary;

                private int index;

                private readonly int version;

                private TValue currentValue;
            }
        }
    }
}