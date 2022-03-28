using Net.Event;
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
        public int Offset { get; set; }
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
        /// <summary>
        /// 字符串记录的字节大小 1字节255个字符, 2字节65535个字符 3字节16777216字符 4字节4294967296
        /// </summary>
        public static byte StringRecordSize = 2;

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
            Offset = index;
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
            return $"byte[{(Buffer != null ? Buffer.Length : 0)}] index:{Offset} count:{Count} size:{Count - Offset}";
        }

        /// <summary>
        /// 复制分片数据
        /// </summary>
        /// <param name="recovery">复制数据后立即回收此分片?</param>
        /// <returns></returns>
        public byte[] ToArray(bool recovery = false, bool resetPos = false)
        {
            Flush(resetPos);
            byte[] array = new byte[Count];
            global::System.Buffer.BlockCopy(Buffer, Offset, array, 0, Count);
            if (recovery) BufferPool.Push(this);
            return array;
        }

        public unsafe void Write(void* ptr, int count)
        {
            fixed (void* ptr1 = &Buffer[Position])
            {
                global::System.Buffer.MemoryCopy(ptr, ptr1, count, count);
                Position += count;
            }
        }

        public void WriteValue<T>(T value)
        {
            switch (value)
            {
                case byte value1:
                    Write(value1);
                    break;
                case sbyte value1:
                    Write(value1);
                    break;
                case bool value1:
                    Write(value1);
                    break;
                case short value1:
                    Write(value1);
                    break;
                case ushort value1:
                    Write(value1);
                    break;
                case char value1:
                    Write(value1);
                    break;
                case int value1:
                    Write(value1);
                    break;
                case uint value1:
                    Write(value1);
                    break;
                case float value1:
                    Write(value1);
                    break;
                case long value1:
                    Write(value1);
                    break;
                case ulong value1:
                    Write(value1);
                    break;
                case double value1:
                    Write(value1);
                    break;
                case DateTime value1:
                    Write(value1);
                    break;
                case decimal value1:
                    Write(value1);
                    break;
                case string value1:
                    Write(value1);
                    break;
                case Enum value1:
                    Write(value1.GetHashCode());
                    break;
                default:
                    throw new Exception($"错误!基类不能序列化这个类:{value}");
            }
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
                    value = ReadByte();
                    break;
                case TypeCode.SByte:
                    value = ReadSByte();
                    break;
                case TypeCode.Boolean:
                    value = ReadBool();
                    break;
                case TypeCode.Int16:
                    value = ReadUInt16();
                    break;
                case TypeCode.UInt16:
                    value = ReadUInt16();
                    break;
                case TypeCode.Char:
                    value = ReadChar();
                    break;
                case TypeCode.Int32:
                    value = ReadInt32();
                    break;
                case TypeCode.UInt32:
                    value = ReadUInt32();
                    break;
                case TypeCode.Single:
                    value = ReadFloat();
                    break;
                case TypeCode.Int64:
                    value = ReadInt64();
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
            WriteArray(value: array);
        }
        public unsafe void WriteArray(object value)
        {
            switch (value)
            {
                case byte[] array1:
                    Write(array1.Length);
                    Write(array1);
                    break;
                case sbyte[] array1:
                    Write(array1.Length);
                    Write(array1);
                    break;
                case bool[] array1:
                    Write(array1);
                    break;
                case short[] array1:
                    Write(array1);
                    break;
                case ushort[] array1:
                    Write(array1);
                    break;
                case char[] array1:
                    Write(array1);
                    break;
                case int[] array1:
                    Write(array1);
                    break;
                case uint[] array1:
                    Write(array1);
                    break;
                case float[] array1:
                    Write(array1);
                    break;
                case long[] array1:
                    Write(array1);
                    break;
                case ulong[] array1:
                    Write(array1);
                    break;
                case double[] array1:
                    Write(array1);
                    break;
                case DateTime[] array1:
                    Write(array1);
                    break;
                case decimal[] array1:
                    Write(array1);
                    break;
                case string[] array1:
                    Write(array1);
                    break;
                default:
                    throw new Exception($"错误!基类不能序列化这个类:{value}");
            }
        }
        public unsafe List<T> ReadList<T>()
        {
            var array = ReadArray<T>();
            if (array == null)
                return new List<T>();
            return new List<T>(array);
        }
        public unsafe T[] ReadArray<T>()
        {
            object array;
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Byte:
                    array = ReadByteArray();
                    break;
                case TypeCode.SByte:
                    array = ReadSByteArray();
                    break;
                case TypeCode.Boolean:
                    array = ReadByteArray();
                    break;
                case TypeCode.Int16:
                    array = ReadInt16Array();
                    break;
                case TypeCode.UInt16:
                    array = ReadUInt16Array();
                    break;
                case TypeCode.Char:
                    array = ReadCharArray();
                    break;
                case TypeCode.Int32:
                    array = ReadInt32Array();
                    break;
                case TypeCode.UInt32:
                    array = ReadUInt32Array();
                    break;
                case TypeCode.Single:
                    array = ReadFloatArray();
                    break;
                case TypeCode.Int64:
                    array = ReadInt64Array();
                    break;
                case TypeCode.UInt64:
                    array = ReadUInt64Array();
                    break;
                case TypeCode.Double:
                    array = ReadDoubleArray();
                    break;
                case TypeCode.DateTime:
                    array = ReadDateTimeArray();
                    break;
                case TypeCode.Decimal:
                    array = ReadDecimalArray();
                    break;
                case TypeCode.String:
                    array = ReadStringArray();
                    break;
                default:
                    throw new Exception("错误!");
            }
            return array as T[];
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
        
        #region Write
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value)
        {
            Buffer[Position++] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSByte(sbyte value)
        {
            Buffer[Position++] = (byte)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte value)
        {
            WriteByte(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(sbyte value)
        {
            WriteSByte(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(bool value)
        {
            WriteByte((byte)(value ? 1 : 0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(short value)
        {
            Write((ushort)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(ushort value)
        {
            if (value == 0)
            {
                WriteByte(0);
                return;
            }
            fixed (byte* ptr = &Buffer[Position])
            {
                byte num = 0;
                while (value > 0)
                {
                    num++;
                    ptr[num] = (byte)(value >> 0);
                    value >>= 8;
                }
                ptr[0] = num;
                Position += num + 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(char value)
        {
            Write((ushort)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value)
        {
            Write((uint)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(uint value)
        {
            if (value == 0)
            {
                WriteByte(0);
                return;
            }
            fixed (byte* ptr = &Buffer[Position])
            {
                byte num = 0;
                while (value > 0)
                {
                    num++;
                    ptr[num] = (byte)(value >> 0);
                    value >>= 8;
                }
                ptr[0] = num;
                Position += num + 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(float value)
        {
            var ptr1 = *(uint*)&value;
            Write(ptr1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(long value)
        {
            Write((ulong)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(ulong value)
        {
            if (value == 0)
            {
                WriteByte(0);
                return;
            }
            fixed (byte* ptr = &Buffer[Position])
            {
                byte num = 0;
                while (value > 0)
                {
                    num++;
                    ptr[num] = (byte)(value >> 0);
                    value >>= 8;
                }
                ptr[0] = num;
                Position += num + 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(double value)
        {
            var ptr1 = *(ulong*)&value;
            Write(ptr1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(DateTime value)
        {
            Write((ulong)value.Ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(TimeSpan value)
        {
            Write((ulong)value.Ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(decimal value)
        {
            var ptr1 = &value;
            Write(ptr1, 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteByte(0);
                return;
            }
            fixed (char* ptr = value)
            {
                fixed (byte* ptr1 = &Buffer[Position])
                {
                    ptr1[0] = StringRecordSize;
                    int size = StringRecordSize + 1;
                    int count = Encoding.UTF8.GetBytes(ptr, value.Length, ptr1 + size, value.Length * 3);
                    Position += size + count;
                    for (int num = 1; num < size; num++)//必须重置StringRecordSize位,问题:从内存池取出之前已使用的脏数据后出现问题
                    {
                        ptr1[num] = (byte)(count >> 0);
                        count >>= 8;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] value)
        {
            global::System.Buffer.BlockCopy(value, 0, Buffer, Position, value.Length);
            Position += value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] value, int index, int count)
        {
            global::System.Buffer.BlockCopy(value, index, Buffer, Position, count);
            Position += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(sbyte[] value)
        {
            global::System.Buffer.BlockCopy(value, 0, Buffer, Position, value.Length);
            Position += value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(bool[] value)
        {
            Write(value.Length);
            fixed (bool* ptr = &value[0])
            {
                Write(ptr, value.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(short[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(ushort[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(char[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(int[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(uint[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(float[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(long[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(ulong[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(DateTime[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(TimeSpan[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(double[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(decimal[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(string[] value)
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(List<short> value)
        {
            Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(List<ushort> value)
        {
            Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(List<char> value)
        {
            Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(List<int> value)
        {
            Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(List<uint> value)
        {
            Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(List<float> value)
        {
            Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(List<long> value)
        {
            Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(List<ulong> value)
        {
            Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(List<DateTime> value)
        {
            Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(List<TimeSpan> value)
        {
            Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                Write(value[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(List<string> value)
        {
            Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                Write(value[i]);
            }
        }
        #endregion

        #region Read
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            return Buffer[Position++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte()
        {
            return (sbyte)Buffer[Position++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool()
        {
            return Buffer[Position++] == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe short ReadInt16()
        {
            return (short)ReadUInt32();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ushort ReadUInt16()
        {
            return (ushort)ReadUInt32();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ReadChar()
        {
            return (char)ReadUInt16();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int ReadInt32()
        {
            return (int)ReadUInt32();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe uint ReadUInt32()
        {
            byte num = Buffer[Position];
            Position++;//不安全代码中 i++和++i没区别
            if (num == 0)
                return 0;
            fixed (byte* ptr = &Buffer[Position])
            {
                Position += num;
                uint value = 0;
                if (BitConverter.IsLittleEndian)
                {
                    for (byte i = 0; i < num; i++)
                        value |= (uint)ptr[i] << (i * 8);
                    return value;
                }
                else
                {
                    for (byte i = num; i > 0; i++)
                        value |= (uint)ptr[i] << (i * 8);
                    return value;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float ReadFloat()
        {
            uint value = ReadUInt32();
            return *(float*)&value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe long ReadInt64()
        {
            return (long)ReadUInt64();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ulong ReadUInt64()
        {
            byte num = Buffer[Position];
            Position++;//不安全代码中 i++和++i没区别
            if (num == 0)
                return 0;
            fixed (byte* ptr = &Buffer[Position])
            {
                Position += num;
                ulong value = 0;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double ReadDouble()
        {
            ulong value = ReadUInt64();
            return *(double*)&value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime ReadDateTime()
        {
            return new DateTime((long)ReadUInt64());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimeSpan ReadTimeSpan()
        {
            return new TimeSpan((long)ReadUInt64());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe decimal ReadDecimal()
        {
            decimal value = default;
            void* ptr = &value;
            fixed (void* ptr1 = &Buffer[Position])
            {
                global::System.Buffer.MemoryCopy(ptr1, ptr, 16, 16);
            }
            Position += 16;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe string ReadString()
        {
            var count = ReadInt32();
            if (count == 0)
                return string.Empty;
            var value = global::System.Text.Encoding.UTF8.GetString(Buffer, Position, count);
            Position += count;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe byte[] ReadByteArray()
        {
            var count = ReadInt32();
            var value = new byte[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadByte();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe sbyte[] ReadSByteArray()
        {
            var count = ReadInt32();
            var value = new sbyte[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadSByte();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe short[] ReadInt16Array()
        {
            var count = ReadInt32();
            var value = new short[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadInt16();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ushort[] ReadUInt16Array()
        {
            var count = ReadInt32();
            var value = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadUInt16();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe char[] ReadCharArray()
        {
            var count = ReadInt32();
            var value = new char[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadChar();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int[] ReadInt32Array()
        {
            var count = ReadInt32();
            var value = new int[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadInt32();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe uint[] ReadUInt32Array()
        {
            var count = ReadInt32();
            var value = new uint[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadUInt32();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float[] ReadFloatArray()
        {
            var count = ReadInt32();
            var value = new float[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadFloat();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe long[] ReadInt64Array()
        {
            var count = ReadInt32();
            var value = new long[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadInt64();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ulong[] ReadUInt64Array()
        {
            var count = ReadInt32();
            var value = new ulong[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadUInt64();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double[] ReadDoubleArray()
        {
            var count = ReadInt32();
            var value = new double[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadDouble();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe DateTime[] ReadDateTimeArray()
        {
            var count = ReadInt32();
            var value = new DateTime[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadDateTime();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe TimeSpan[] ReadTimeSpanArray()
        {
            var count = ReadInt32();
            var value = new TimeSpan[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadTimeSpan();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe decimal[] ReadDecimalArray()
        {
            var count = ReadInt32();
            var value = new decimal[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadDecimal();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe string[] ReadStringArray()
        {
            var count = ReadInt32();
            var value = new string[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadString();
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<byte> ReadByteList()
        {
            var count = ReadInt32();
            var value = new List<byte>();
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadByte();
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<short> ReadInt16List()
        {
            var count = ReadInt32();
            var value = new List<short>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadInt16());
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<ushort> ReadUInt16List()
        {
            var count = ReadInt32();
            var value = new List<ushort>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadUInt16());
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<char> ReadCharList()
        {
            var count = ReadInt32();
            var value = new List<char>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadChar());
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<int> ReadInt32List()
        {
            var count = ReadInt32();
            var value = new List<int>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadInt32());
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<uint> ReadUInt32List()
        {
            var count = ReadInt32();
            var value = new List<uint>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadUInt32());
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<float> ReadFloatList()
        {
            var count = ReadInt32();
            var value = new List<float>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadFloat());
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<long> ReadInt64List()
        {
            var count = ReadInt32();
            var value = new List<long>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadInt64());
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<ulong> ReadUInt64List()
        {
            var count = ReadInt32();
            var value = new List<ulong>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadUInt64());
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<double> ReadDoubleList()
        {
            var count = ReadInt32();
            var value = new List<double>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadDouble());
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<DateTime> ReadDateTimeList()
        {
            var count = ReadInt32();
            var value = new List<DateTime>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadDateTime());
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<TimeSpan> ReadTimeSpanList()
        {
            var count = ReadInt32();
            var value = new List<TimeSpan>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadTimeSpan());
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe List<string> ReadStringList()
        {
            var count = ReadInt32();
            var value = new List<string>();
            for (int i = 0; i < count; i++)
            {
                value.Add(ReadString());
            }
            return value;
        }
        #endregion

        public void Flush(bool resetPos = true)
        {
            if (Position > Count)
                Count = Position;
            if (resetPos)
                Position = Offset;
        }
    }
}
