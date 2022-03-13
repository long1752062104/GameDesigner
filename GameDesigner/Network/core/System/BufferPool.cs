using Net.Config;
using Net.Event;
using Net.Serialize;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Net.System
{
    /// <summary>
    /// 内存数据片段
    /// </summary>
    public class Segment : IDisposable
    {
        /// <summary>
        /// 总内存
        /// </summary>
        public byte[] Buffer { get; internal set; }
        /// <summary>
        /// 片的开始位置
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 片的长度
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 读写位置
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// 获取总长度
        /// </summary>
        public int Length { get { return length; } }

        internal bool isDespose;
        internal int length;
        internal bool isRecovery;
        internal int referenceCount;
        private const byte BYTE = 1, SHORT = 2, INT24 = 3, INT32 = 4, INT40 = 5, INT48 = 6, INT56 = 7, LONG = 8;
        private const long Int24 = 16777216, Int40 = 1099511627776, Int48 = 281474976710656, Int56 = 72057594037927936;

        /// <summary>
        /// 获取或设置总内存位置索引
        /// </summary>
        /// <param name="index">内存位置索引</param>
        /// <returns></returns>
        public byte this[int index] { get { return Buffer[index]; } set { Buffer[index] = value; } }

        /// <summary>
        /// 构造内存分片
        /// </summary>
        /// <param name="buffer"></param>
        public Segment(byte[] buffer) : this(buffer, 0, buffer.Length)
        {
        }

        /// <summary>
        /// 构造内存分片
        /// </summary>
        /// <param name="buffer"></param>
        public Segment(byte[] buffer, bool isRecovery) : this(buffer, 0, buffer.Length, isRecovery)
        {
        }

        /// <summary>
        /// 构造内存分片
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public Segment(byte[] buffer, int index, int count, bool isRecovery = true)
        {
            Buffer = buffer;
            Index = index;
            Count = count;
            length = buffer.Length;
            Position = index;
            isDespose = !isRecovery;//如果不回收，则已经释放状态，不允许压入数组池
            this.isRecovery = isRecovery;
        }

        public static implicit operator Segment(byte[] buffer)
        {
            return new Segment(buffer);
        }

        public static implicit operator byte[](Segment segment)
        {
            return segment.Buffer;
        }

        ~Segment()
        {
            if (isRecovery && BufferPool.Log)
                NDebug.LogError("片段内存泄漏!请检查代码正确Push内存池!");
            Dispose();
        }

        public void Dispose()
        {
            if (!isRecovery)
                return;
            BufferPool.Push(this);
        }

        public void Close() 
        {
            isRecovery = false;
            isDespose = true;
            Buffer = null;
        }

        public override string ToString()
        {
            return $"byte[{(Buffer != null ? Buffer.Length : 0)}] index:{Index} count:{Count} size:{Count-Index}";
        }

        /// <summary>
        /// 复制分片数据
        /// </summary>
        /// <param name="recovery">复制数据后立即回收此分片?</param>
        /// <returns></returns>
        public byte[] ToArray(bool recovery = false)
        {
            if (Position > Count) Count = Position;
            byte[] array = new byte[Count];
            global::System.Buffer.BlockCopy(Buffer, Index, array, 0, Count);
            if (recovery) BufferPool.Push(this);
            return array;
        }

        public void WriteByte(byte value) 
        {
            Buffer[Position++] = value;
            if (Position > Count) Count = Position;
        }

        public byte ReadByte()
        {
            return Buffer[Position++];
        }

        public bool ReadBoolean() { return Buffer[Position++] == 1; }

        public void Write(byte[] buffer, int index, int count)
        {
            global::System.Buffer.BlockCopy(buffer, index, Buffer, Position, count);
            Position += count;
            if (Position > Count) Count = Position;
        }

        public unsafe void Write(byte* buffer, int index, int count)
        {
            fixed (void* ptr = &Buffer[Position]) 
            {
                void* ptr1 = buffer + index;
                global::System.Buffer.MemoryCopy(ptr1, ptr, count, count);
                Position += count;
                if (Position > Count) Count = Position;
            }
        }
        public unsafe void Write(void* ptr, int count)
        {
            fixed (void* ptr1 = &Buffer[Position])
            {
                global::System.Buffer.MemoryCopy(ptr, ptr1, count, count);
                Position += count;
                if (Position > Count) Count = Position;
            }
        }
        public int WriteValue<T>(T value)
        {
            switch (value) 
            {
                case byte value1:
                    WriteByte(value1);
                    return 1;
                case sbyte value1:
                    WriteByte((byte)value1);
                    return 1;
                case bool value1:
                    WriteByte((byte)(value1 ? 1 : 0));
                    return 1;
                case short value1:
                    return WriteValue((ulong)(ushort)value1);
                case ushort value1:
                    return WriteValue((ulong)value1);
                case char value1:
                    return WriteValue((ulong)value1);
                case int value1:
                    return WriteValue((ulong)(uint)value1);//为什么转uint? 因为-1xx时数据非常庞大
                case uint value1:
                    return WriteValue((ulong)value1);
                case float value1:
                    return WriteValue(value1);
                case long value1:
                    return WriteValue((ulong)value1);
                case ulong value1:
                    return WriteValue(value1);
                case double value1:
                    return WriteValue(value1);
                case DateTime value1:
                    return WriteValue(value1);
                case decimal value1:
                    return WriteValue(value1);
                case string value1:
                    return WriteValue(value1);
                case Enum value1:
                    return WriteValue((ulong)(uint)value1.GetHashCode());
                default:
                    throw new Exception($"错误!基类不能序列化这个类:{value}");
            }
        }
        public int WriteValue(int value) => WriteValue((ulong)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int WriteValue(ulong value)
        {
            if (value == 0)
            {
                Buffer[Position++] = 0;
                if (Position > Count) Count = Position;
                return 0;
            }
            byte num = value < byte.MaxValue ? BYTE : value < ushort.MaxValue ? SHORT : value < Int24 ? INT24 : 
            value < int.MaxValue ? INT32 : value < Int40 ? INT40 : value < Int48 ? INT48 : value < Int56 ? INT56 : LONG;
            void* ptr = &value;
            Buffer[Position++] = num;
            Write(ptr, num);
            if (Position > Count) Count = Position;
            return num;
        }

        public T ReadValue<T>()
        {
            return (T)ReadValue(typeof(T));
        }

        public object ReadValue(Type type) 
        {
            object value = null;
            switch (Type.GetTypeCode(type)) 
            {
                case TypeCode.Byte:
                    value = Buffer[Position++];
                    break;
                case TypeCode.SByte:
                    value = (sbyte)Buffer[Position++];
                    break;
                case TypeCode.Boolean:
                    value = Buffer[Position++] == 1;
                    break;
                case TypeCode.Int16:
                    value = (short)ReadUInt64();
                    break;
                case TypeCode.UInt16:
                    value = (ushort)ReadUInt64();
                    break;
                case TypeCode.Char:
                    value = (char)ReadUInt64();
                    break;
                case TypeCode.Int32:
                    value = (int)ReadUInt64();
                    break;
                case TypeCode.UInt32:
                    value = (uint)ReadUInt64();
                    break;
                case TypeCode.Single:
                    value = ReadFloat();
                    break;
                case TypeCode.Int64:
                    value = (long)ReadUInt64();
                    break;
                case TypeCode.UInt64:
                    value = ReadUInt64();
                    break;
                case TypeCode.Double:
                    value = ReadDouble();
                    break;
                case TypeCode.DateTime:
                    value = ReadDateTime();
                    break;
                case TypeCode.Decimal:
                    value = ReadDecimal();
                    break;
                case TypeCode.String:
                    value = ReadString();
                    break;
            }
            return value;
        }

        public unsafe byte[] Read(int count)
        {
            byte[] buffer = new byte[count];
            fixed (byte* ptr = &Buffer[Position])
            {
                for (int i = 0; i < count; i++)
                {
                    buffer[i] = ptr[i];
                }
            }
            Position += count;
            return buffer;
        }

        public unsafe int WriteValue(float value)
        {
            byte record = 0;
            byte* ptr = (byte*)&value;
            var pos = Position++;
            for (byte i = 0; i < 4; i++)
            {
                if (ptr[i] != 0)
                {
                    NetConvertBase.SetBit(ref record, i + 1, true);
                    Buffer[Position++] = ptr[i];
                }
            }
            Buffer[pos] = record;
            if (Position > Count) Count = Position;
            return 4;
        }

        public unsafe float ReadFloat() 
        {
            byte record = Buffer[Position++];
            float value = 0f;
            byte* ptr = (byte*)&value;
            for (byte i = 1; i < 5; i++)
            {
                if(NetConvertBase.GetBit(record, i))
                    ptr[i - 1] = Buffer[Position++];
            }
            int num;
            if (BitConverter.IsLittleEndian)
                num = * ptr | (ptr[1] << 8) | (ptr[2] << 16) | (ptr[3] << 24);
            else
                num = (*ptr << 24) | (ptr[1] << 16) | (ptr[2] << 8) | ptr[3];
            return *(float*)&num;
        }

        public unsafe int WriteValue(double value)
        {
            byte record = 0;
            byte* ptr = (byte*)&value;
            var pos = Position++;
            for (byte i = 0; i < 8; i++)
            {
                if (ptr[i] != 0)
                {
                    NetConvertBase.SetBit(ref record, i + 1, true);
                    Buffer[Position++] = ptr[i];
                }
            }
            Buffer[pos] = record;
            if (Position > Count) Count = Position;
            return 8;
        }

        public unsafe double ReadDouble()
        {
            byte record = Buffer[Position++];
            double value = 0d;
            byte* ptr = (byte*)&value;
            for (byte i = 1; i < 9; i++)
            {
                if (NetConvertBase.GetBit(record, i))
                    ptr[i - 1] = Buffer[Position++];
            }
            long value1;
            if (BitConverter.IsLittleEndian)
            {
                int num = *ptr | (ptr[1] << 8) | (ptr[2] << 16) | (ptr[3] << 24);
                int num2 = ptr[4] | (ptr[5] << 8) | (ptr[6] << 16) | (ptr[7] << 24);
                value1 = (uint)num | ((long)num2 << 32);
            }
            else 
            {
                int num3 = (*ptr << 24) | (ptr[1] << 16) | (ptr[2] << 8) | ptr[3];
                int num4 = (ptr[4] << 24) | (ptr[5] << 16) | (ptr[6] << 8) | ptr[7];
                value1 = (uint)num4 | ((long)num3 << 32);
            }
            return *(double*)&value1;
        }

        public unsafe int WriteValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteValue(0ul);
                return 0;
            }
            fixed (char* ptr = value)
            {
                var charCount = value.Length * 3;
                byte num = charCount < byte.MaxValue ? BYTE : charCount < ushort.MaxValue ? SHORT : charCount < Int24 ? INT24 : INT32;
                Buffer[Position++] = num;
                fixed (byte* ptr2 = &Buffer[Position])
                {
                    Position += num;
                    fixed (byte* ptr1 = &Buffer[Position])
                    {
                        int count = Encoding.UTF8.GetBytes(ptr, value.Length, ptr1, charCount);
                        byte* ptr3 = (byte*)&count;
                        for (int i = 0; i < num; i++)
                        {
                            ptr2[i] = ptr3[i];
                        }
                        Position += count;
                        if (Position > Count) Count = Position;
                        return num;
                    }
                }
            }
        }

        public unsafe string ReadString() 
        {
            int count = (int)ReadUInt64();
            if (count == 0)
                return string.Empty;
            fixed (byte* ptr = &Buffer[Position]) 
            {
                var value = Encoding.UTF8.GetString(ptr, count);
                Position += count;
                return value;
            }
        }

        public unsafe int WriteValue(decimal value) 
        {
            if (value == 0)
            {
                Buffer[Position++] = 0;
                return 0;
            }
            byte num = 16;
            void* ptr = &value;
            Buffer[Position++] = num;
            Write(ptr, num);
            //if (Position > Count) Count = Position;
            return num;
        }

        public unsafe decimal ReadDecimal()
        {
            int num = Buffer[Position++];
            if (num == 0)
                return 0m;
            decimal value = default;
            void* ptr = &value;
            fixed (void* ptr1 = &Buffer[Position]) 
            {
                global::System.Buffer.MemoryCopy(ptr1, ptr, 16, 16);
            }
            Position += 16;
            return value;
        }

        public unsafe int WriteValue(DateTime value)
        {
            if (value == default)
            {
                Buffer[Position++] = 0;
                return 0;
            }
            byte num = 8;
            void* ptr = &value;
            Buffer[Position++] = num;
            Write(ptr, num);
            //if (Position > Count) Count = Position;
            return num;
        }

        public unsafe DateTime ReadDateTime()
        {
            int num = Buffer[Position++];
            if (num == 0)
                return default;
            DateTime value = default;
            void* ptr = &value;
            fixed (void* ptr1 = &Buffer[Position])
            {
                global::System.Buffer.MemoryCopy(ptr1, ptr, 8, 8);
            }
            Position += 8;
            return value;
        }
        public unsafe void WriteList<T>(List<T> array) 
        {
            WriteArray(array.ToArray());
        }
        public unsafe void WriteList(object value)
        {
            var array = value.GetType().GetMethod("ToArray").Invoke(value, null);
            WriteArray(array);
        }
        public void WriteArray<T>(T[] array)
        {
            WriteArray(value:array);
        }
        public unsafe void WriteArray(object value)
        {
            if (!(value is Array array))
                throw new Exception($"错误!{value}类转换数组失败!");
            int count = array.Length;
            WriteValue(count);
            if (count == 0)
                return;
            switch (value)
            {
                case byte[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 1); };
                    break;
                case sbyte[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 1); };
                    break;
                case bool[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 1); };
                    break;
                case short[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 2); };
                    break;
                case ushort[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 2); };
                    break;
                case char[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 2); };
                    break;
                case int[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 4); };
                    break;
                case uint[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 4); };
                    break;
                case float[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 4); };
                    break;
                case long[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 8); };
                    break;
                case ulong[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 8); };
                    break;
                case double[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 8); };
                    break;
                case DateTime[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 8); };
                    break;
                case decimal[] array1:
                    fixed (void* ptr1 = &array1[0]) {  WriteValue(ptr1, count, 16); };
                    break;
                case string[] array1:
                    foreach (var str in array1)
                        WriteValue(str);
                    return;
                default:
                    throw new Exception($"错误!基类不能序列化这个类:{value}");
            }
        }
        private unsafe void WriteValue(void* ptr, int count, int num) 
        {
            fixed (void* ptr2 = &Buffer[Position])
            {
                var count1 = count * num;
                if (Position + count1 > length)
                    throw new Exception($"错误!写入的数组大于总内存段!");
                global::System.Buffer.MemoryCopy(ptr, ptr2, count1, count1);
                Position += count1;
                if (Position > Count) Count = Position;
            }
        }
        public unsafe List<T> ReadList<T>()
        {
            var array = ReadArray<T>();
            if(array == null)
                return new List<T>();
            return new List<T>(array);
        }
        public unsafe T[] ReadArray<T>()
        {
            var count = ReadValue<int>();
            if (count == 0)
                return null;
            T[] array = new T[count];
            switch (array) 
            {
                case byte[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 1); };
                    break;
                case sbyte[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 1); }; 
                    break;
                case bool[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 1); }; 
                    break;
                case short[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 2); }; 
                    break;
                case ushort[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 2); }; 
                    break;
                case char[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 2); }; 
                    break;
                case int[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 4); }; 
                    break;
                case uint[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 4); }; 
                    break;
                case float[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 4); }; 
                    break;
                case long[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 8); }; 
                    break;
                case ulong[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 8); }; 
                    break;
                case double[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 8); }; 
                    break;
                case DateTime[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 8); }; 
                    break;
                case decimal[] array1:
                    fixed (void* ptr1 = &array1[0]) { ReadArray(ptr1, count, 16); };
                    break;
                case string[] array1:
                    for (int i = 0; i < array1.Length; i++)
                        array1[i] = ReadValue<string>();
                    return array1 as T[];
                default:
                    throw new Exception("错误!");
            }
            return array;
        }
        private unsafe void ReadArray(void* ptr, int count, int num) 
        {
            fixed (void* ptr1 = &Buffer[Position])
            {
                var count1 = count * num;
                global::System.Buffer.MemoryCopy(ptr1, ptr, count1, count1);
                Position += count * num;
                Count = Position;
            }
        }
        public void SetLength(int length)
        {
            if (Position > length)
                Position = length;
            Count = length;
        }

        public void SetPositionLength(int length)
        {
            Position = length;
            Count = length;
        }
        public void WriteInt32(int value)
        {
            WriteUInt64((ulong)value);
        }
        public void WriteUInt32(uint value)
        {
            WriteUInt64(value);
        }
        public void WriteUInt64(ulong value)
        {
            if (value == 0)
            {
                WriteByte(0);
                return;
            }
            byte num = default;
            Position++;
            while (value > 0)
            {
                Buffer[Position++] = (byte)(value >> 0);
                value >>= 8;
                num++;
            }
            Buffer[Position - (num + 1)] = num;
        }
        public unsafe int ReadInt32()
        {
            return (int)ReadUInt64();
        }
        public unsafe uint ReadUInt32()
        {
            return (uint)ReadUInt64();
        }
        public unsafe ulong ReadUInt64()
        {
            byte num = Buffer[Position++];
            fixed (byte* ptr = &Buffer[Position])
            {
                Position += num;
                //if (Position > Count) Count = Position;
                ulong value = 0ul;
                if (BitConverter.IsLittleEndian)
                {
                    for (byte i = 0; i < num; i++)
                        value |= (ulong)ptr[i] << (i * 8);
                    return value;
                }
                else 
                {
                    for (byte i = num; i > 0; i++)
                        value |= (ulong)ptr[i] << (i * 8);
                    return value;
                }
            }
        }
    }

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
            segment.Index = 0;
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
