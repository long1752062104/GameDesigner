using Net.Config;

namespace Net.System
{
    /// <summary>
    /// 数据缓冲内存池
    /// </summary>
    public static class BufferPool
    {
        /// <summary>
        /// 数据缓冲池大小. 默认65536字节
        /// </summary>
        public static int Size { get; set; } = 65536;
        /// <summary>
        /// 当没有合理回收内存，导致内存泄漏被回收后提示
        /// </summary>
        public static bool Log { get; set; }

        private static readonly StackSafe<Segment>[] STACKS = new StackSafe<Segment>[37];
        private static readonly int[] TABLE = new int[] {
            256,512,1024,2048,4096,8192,16384,32768,65536,98304,131072,196608,262144,393216,524288,786432,1048576,
            1572864,2097152,3145728,4194304,6291456,8388608,12582912,16777216,25165824,33554432,50331648,67108864,
            100663296,134217728,201326592,268435456,402653184,536870912,805306368,1073741824
        };

        static BufferPool()
        {
            for (int i = 0; i < TABLE.Length; i++)
            {
                STACKS[i] = new StackSafe<Segment>();
            }
            GlobalConfig.ThreadPoolRun = true;
            ThreadManager.Invoke("BufferPool", 1f, ()=>
            {
                try
                {
#if UNITY_EDITOR
                    if (Client.ClientBase.Instance == null)
                        GlobalConfig.ThreadPoolRun = false;
#endif
                    for (int i = 0; i < STACKS.Length; i++)
                    {
                        var head = STACKS[i].m_head;
                        int index = 0;
                        while (head != null)
                        {
                            var seg = head.m_value;
                            if (seg != null)
                            {
                                if (seg.referenceCount == 0)
                                {
                                    int count = STACKS[i].Count - index;
                                    var segs = new Segment[count];
                                    STACKS[i].TryPopRange(segs, 0, count);
                                    foreach (var seg1 in segs)
                                        seg1?.Close();
                                    break;
                                }
                                seg.referenceCount = 0;
                            }
                            head = head.m_next;
                            index++;
                        }
                    }
                }
                catch { }
                return true;
            });
        }

        /// <summary>
        /// 从内存池取数据片
        /// </summary>
        /// <returns></returns>
        public static Segment Take()
        {
            return Take(Size);
        }

        /// <summary>
        /// 从内存池取数据片
        /// </summary>
        /// <param name="size">内存大小</param>
        /// <returns></returns>
        public static Segment Take(int size)
        {
            var tableInx = 0;
            for (int i = 0; i < TABLE.Length; i++)
            {
                if (size <= TABLE[i])
                {
                    size = TABLE[i];
                    tableInx = i;
                    goto J;
                }
            }
        J:  var stack = STACKS[tableInx];
            if (stack.TryPop(out Segment segment))
                goto J1;
            segment = new Segment(new byte[size], 0, size);
        J1: segment.isDespose = false;
            segment.Offset = 0;
            segment.Count = 0;
            segment.Position = 0;
            segment.referenceCount++;
            return segment;
        }

        /// <summary>
        /// 压入数据片, 等待复用
        /// </summary>
        /// <param name="segment"></param>
        public static void Push(Segment segment) 
        {
            if (segment.isDespose)
                return;
            segment.isDespose = true;
            for (int i = 0; i < TABLE.Length; i++)
            {
                if (segment.length == TABLE[i])
                {
                    STACKS[i].Push(segment);
                    return;
                }
            }
        }
    }

    public static class ObjectPool<T> where T : new()
    {
        private static readonly StackSafe<T> STACK = new StackSafe<T>();

        public static void Init(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                STACK.Push(new T());
            }
        }

        public static T Take()
        {
            if (STACK.TryPop(out T obj))
                return obj;
            return new T();
        }

        public static void Push(T obj)
        {
            STACK.Push(obj);
        }
    }
}
