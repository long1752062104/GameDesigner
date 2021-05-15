using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ECS
{
    public interface IIndex
    {
        int Index { get; set; }
    }

    internal class HashSetDebugView<T> where T : IIndex
    {
        private readonly ArrayPool<T> set;

        public HashSetDebugView(ArrayPool<T> set)
        {
            this.set = set ?? throw new ArgumentNullException("set");
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                return set.ToArray();
            }
        }
    }

    [DebuggerTypeProxy(typeof(HashSetDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ArrayPool<T> where T : IIndex
    {
        internal T[] items = new T[256];
        internal bool[] buckets = new bool[256];
        private int index;
        private int count;

        public T this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        public int Count { get { return count; } }

        public void Add(T item)
        {
            lock (this)
            {
                if (item.Index != -1)
                {
                    if (!buckets[item.Index])
                    {
                        items[item.Index] = item;
                        buckets[item.Index] = true;
                        return;
                    }
                }
                items[index] = item;
                buckets[index] = true;
                item.Index = index;
                index++;
                if (index >= items.Length)
                {
                    T[] newItems = new T[items.Length * 2];
                    bool[] newItems1 = new bool[items.Length * 2];
                    Array.Copy(items, 0, newItems, 0, items.Length);
                    Array.Copy(buckets, 0, newItems1, 0, buckets.Length);
                    items = newItems;
                    buckets = newItems1;
                }
                count = index;
            }
        }

        public void Remove(T item)
        {
            lock (this)
            {
                items[item.Index] = default;
                buckets[item.Index] = false;
            }
        }

        public T[] ToArray()
        {
            List<T> items1 = new List<T>();
            for (int i = 0; i < items.Length; i++)
            {
                if (buckets[i])
                    items1.Add(items[i]);
            }
            return items1.ToArray();
        }
    }
}
