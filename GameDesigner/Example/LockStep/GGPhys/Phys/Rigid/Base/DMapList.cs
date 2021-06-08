using System.Collections.Generic;

namespace GGPhys.Rigid
{
    //双向链表结构节点
    public class DoubleLinkedListNode<T> where T : class, new()
    {
        //前一个节点
        public DoubleLinkedListNode<T> prev = null;
        //后一个节点
        public DoubleLinkedListNode<T> next = null;
        //当前节点
        public T t = null;
    }

    //双向链表结构
    public class DoubleLinedList<T> where T : class, new()
    {
        //表头
        public DoubleLinkedListNode<T> Head = null;
        //表尾
        public DoubleLinkedListNode<T> Tail = null;
        //双向链表结构类对象池
        protected ClassObjectPool<DoubleLinkedListNode<T>> m_DoubleLinkNodePool = new ClassObjectPool<DoubleLinkedListNode<T>>(1000);
        //个数
        protected int m_Count = 0;
        public int Count
        {
            get { return m_Count; }
        }

        /// <summary>
        /// 添加一个节点到头部
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DoubleLinkedListNode<T> AddToHeader(T t)
        {
            DoubleLinkedListNode<T> pList = m_DoubleLinkNodePool.Spawn(true);
            pList.next = null;
            pList.prev = null;
            pList.t = t;
            return AddToHeader(pList);
        }

        /// <summary>
        /// 添加一个节点到头部
        /// </summary>
        /// <param name="pNode"></param>
        /// <returns></returns>
        public DoubleLinkedListNode<T> AddToHeader(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null)
                return null;

            pNode.prev = null;
            if (Head == null)
            {
                Head = Tail = pNode;
            }
            else
            {
                pNode.next = Head;
                Head.prev = pNode;
                Head = pNode;
            }
            m_Count++;
            return Head;
        }

        /// <summary>
        /// 添加节点到尾部
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DoubleLinkedListNode<T> AddToTail(T t)
        {
            DoubleLinkedListNode<T> pList = m_DoubleLinkNodePool.Spawn(true);
            pList.next = null;
            pList.prev = null;
            pList.t = t;
            return AddToTail(pList);
        }

        /// <summary>
        /// 添加节点到尾部
        /// </summary>
        /// <param name="pNode"></param>
        /// <returns></returns>
        public DoubleLinkedListNode<T> AddToTail(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null)
                return null;

            pNode.next = null;
            if (Tail == null)
            {
                Head = Tail = pNode;
            }
            else
            {
                pNode.prev = Tail;
                Tail.next = pNode;
                Tail = pNode;
            }
            m_Count++;
            return Tail;
        }

        /// <summary>
        /// 移除某个节点
        /// </summary>
        /// <param name="pNode"></param>
        public void RemoveNode(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null)
                return;

            if (pNode == Head)
                Head = pNode.next;

            if (pNode == Tail)
                Tail = pNode.prev;

            if (pNode.prev != null)
                pNode.prev.next = pNode.next;

            if (pNode.next != null)
                pNode.next.prev = pNode.prev;

            pNode.next = pNode.prev = null;
            pNode.t = null;
            m_DoubleLinkNodePool.Recycle(pNode);
            m_Count--;
        }

        /// <summary>
        /// 把某个节点移动到头部
        /// </summary>
        /// <param name="pNode"></param>
        public void MoveToHead(DoubleLinkedListNode<T> pNode)
        {
            if (pNode == null || pNode == Head)
                return;

            if (pNode.prev == null && pNode.next == null)
                return;

            if (pNode == Tail)
                Tail = pNode.prev;

            if (pNode.prev != null)
                pNode.prev.next = pNode.next;

            if (pNode.next != null)
                pNode.next.prev = pNode.prev;

            pNode.prev = null;
            pNode.next = Head;
            Head.prev = pNode;
            Head = pNode;
            if (Tail == null)
            {
                Tail = Head;
            }
        }
    }

    public class DMapList<T> where T : class, new()
    {
        DoubleLinedList<T> m_DLink = new DoubleLinedList<T>();
        Dictionary<T, DoubleLinkedListNode<T>> m_FindMap = new Dictionary<T, DoubleLinkedListNode<T>>();

        ~DMapList()
        {
            Clear();
        }

        /// <summary>
        /// 情况列表
        /// </summary>
        public void Clear()
        {
            while (m_DLink.Tail != null)
            {
                Remove(m_DLink.Tail.t);
            }
        }

        /// <summary>
        /// 插入一个节点到表头
        /// </summary>
        /// <param name="t"></param>
        public void Insert(T t)
        {
            if (m_FindMap.TryGetValue(t, out DoubleLinkedListNode<T> node) && node != null)
            {
                return;
            }
            m_DLink.AddToHeader(t);
            m_FindMap.Add(t, m_DLink.Head);
        }

        /// <summary>
        /// 从表尾弹出一个结点
        /// </summary>
        public T Pop()
        {
            if (m_DLink.Tail != null)
            {
                T t = m_DLink.Tail.t;
                Remove(m_DLink.Tail.t);
                return t;
            }
            return null;
        }

        /// <summary>
        /// 删除某个节点
        /// </summary>
        /// <param name="t"></param>
        public void Remove(T t)
        {
            if (!m_FindMap.TryGetValue(t, out DoubleLinkedListNode<T> node) || node == null)
            {
                return;
            }
            m_DLink.RemoveNode(node);
            m_FindMap.Remove(t);
        }

        /// <summary>
        /// 获取到尾部节点
        /// </summary>
        /// <returns></returns>
        public T Back()
        {
            return m_DLink.Tail == null ? null : m_DLink.Tail.t;
        }

        /// <summary>
        /// 返回节点个数
        /// </summary>
        /// <returns></returns>
        public int Size()
        {
            return m_FindMap.Count;
        }

        /// <summary>
        /// 查找是否存在该节点
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Find(T t)
        {
            if (!m_FindMap.TryGetValue(t, out DoubleLinkedListNode<T> node) || node == null)
                return false;

            return true;
        }

        /// <summary>
        /// 刷新某个节点，把节点移动到头部
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Reflesh(T t)
        {
            if (!m_FindMap.TryGetValue(t, out DoubleLinkedListNode<T> node) || node == null)
                return false;

            m_DLink.MoveToHead(node);
            return true;
        }
    }
}

