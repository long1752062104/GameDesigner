using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Net.System
{
    internal sealed class StackSafeDebugView<T>
    {
        private readonly IProducerConsumerCollection<T> m_collection;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => m_collection.ToArray();
        
        public StackSafeDebugView(IProducerConsumerCollection<T> collection)
        {
            m_collection = collection ?? throw new ArgumentNullException("collection");
        }
    }

    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(StackSafeDebugView<>))]
    public class StackSafe<T> : IProducerConsumerCollection<T>, IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
    {
        internal class Node
        {
            internal readonly T m_value;

            internal Node m_next;

            internal Node(T value)
            {
                m_value = value;
                m_next = null;
            }
        }

        internal volatile Node m_head;

        public bool IsEmpty
        {
            
            get
            {
                return m_head == null;
            }
        }

        
        public int Count
        {
            
            get
            {
                int num = 0;
                for (Node node = m_head; node != null; node = node.m_next)
                {
                    num++;
                }
                return num;
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
                throw new NotSupportedException();
            }
        }

        
        public StackSafe()
        {
        }

        
        public StackSafe(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            InitializeFromCollection(collection);
        }

        private void InitializeFromCollection(IEnumerable<T> collection)
        {
            Node node = null;
            foreach (T item in collection)
            {
                Node node2 = new Node(item)
                {
                    m_next = node
                };
                node = node2;
            }

            m_head = node;
        }

        public void Clear()
        {
            m_head = null;
        }

        
        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            ((ICollection)ToList()).CopyTo(array, index);
        }

        
        public void CopyTo(T[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            ToList().CopyTo(array, index);
        }

        
        public void Push(T item)
        {
            Node node = new Node(item)
            {
                m_next = m_head
            };
            if (Interlocked.CompareExchange(ref m_head, node, node.m_next) != node.m_next)
            {
                PushCore(node, node);
            }
        }

        
        public void PushRange(T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            PushRange(items, 0, items.Length);
        }

        
        public void PushRange(T[] items, int startIndex, int count)
        {
            ValidatePushPopRangeInput(items, startIndex, count);
            if (count != 0)
            {
                Node node;
                Node node2 = node = new Node(items[startIndex]);
                for (int i = startIndex + 1; i < startIndex + count; i++)
                {
                    Node node3 = new Node(items[i])
                    {
                        m_next = node2
                    };
                    node2 = node3;
                }

                node.m_next = m_head;
                if (Interlocked.CompareExchange(ref m_head, node2, node.m_next) != node.m_next)
                {
                    PushCore(node2, node);
                }
            }
        }

        private void PushCore(Node head, Node tail)
        {
            SpinWait spinWait = default;
            do
            {
                spinWait.SpinOnce();
                tail.m_next = m_head;
            }
            while (Interlocked.CompareExchange(ref m_head, head, tail.m_next) != tail.m_next);
        }

        private void ValidatePushPopRangeInput(T[] items, int startIndex, int count)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "ConcurrentStack_PushPopRange_CountOutOfRange");
            }

            int num = items.Length;
            if (startIndex >= num || startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", "ConcurrentStack_PushPopRange_StartOutOfRange");
            }

            if (num - count < startIndex)
            {
                throw new ArgumentException("ConcurrentStack_PushPopRange_InvalidCount");
            }
        }

        
        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            Push(item);
            return true;
        }

        
        public bool TryPeek(out T result)
        {
            Node head = m_head;
            if (head == null)
            {
                result = default;
                return false;
            }

            result = head.m_value;
            return true;
        }

        
        public bool TryPop(out T result)
        {
            Node head = m_head;
            if (head == null)
            {
                result = default;
                return false;
            }

            if (Interlocked.CompareExchange(ref m_head, head.m_next, head) == head)
            {
                result = head.m_value;
                return true;
            }

            return TryPopCore(out result);
        }

        
        public int TryPopRange(T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            return TryPopRange(items, 0, items.Length);
        }

        
        public int TryPopRange(T[] items, int startIndex, int count)
        {
            ValidatePushPopRangeInput(items, startIndex, count);
            if (count == 0)
            {
                return 0;
            }

            int num = TryPopCore(count, out Node poppedHead);
            if (num > 0)
            {
                CopyRemovedItems(poppedHead, items, startIndex, num);
            }

            return num;
        }

        private bool TryPopCore(out T result)
        {
            if (TryPopCore(1, out Node poppedHead) == 1)
            {
                result = poppedHead.m_value;
                return true;
            }

            result = default;
            return false;
        }

        private int TryPopCore(int count, out Node poppedHead)
        {
            SpinWait spinWait = default;
            int num = 1;
            Random random = new Random(Environment.TickCount & int.MaxValue);
            Node head;
            int i;
            while (true)
            {
                head = m_head;
                if (head == null)
                {
                    poppedHead = null;
                    return 0;
                }

                Node node = head;
                for (i = 1; i < count; i++)
                {
                    if (node.m_next == null)
                    {
                        break;
                    }

                    node = node.m_next;
                }

                if (Interlocked.CompareExchange(ref m_head, node.m_next, head) == head)
                {
                    break;
                }

                for (int j = 0; j < num; j++)
                {
                    spinWait.SpinOnce();
                }

                num = spinWait.NextSpinWillYield ? random.Next(1, 8) : (num * 2);
            }
            poppedHead = head;
            return i;
        }

        private void CopyRemovedItems(Node head, T[] collection, int startIndex, int nodesCount)
        {
            Node node = head;
            for (int i = startIndex; i < startIndex + nodesCount; i++)
            {
                collection[i] = node.m_value;
                node = node.m_next;
            }
        }

        
        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            return TryPop(out item);
        }

        
        public T[] ToArray()
        {
            return ToList().ToArray();
        }

        private List<T> ToList()
        {
            List<T> list = new List<T>();
            for (Node node = m_head; node != null; node = node.m_next)
            {
                list.Add(node.m_value);
            }

            return list;
        }

        
        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumerator(m_head);
        }

        private IEnumerator<T> GetEnumerator(Node head)
        {
            for (Node current = head; current != null; current = current.m_next)
            {
                yield return current.m_value;
            }
        }

        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}