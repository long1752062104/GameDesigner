using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Net.System
{
    public ref struct Segment2
    {
        public byte[] buffer;
        /// <summary>
        /// 片的开始位置
        /// </summary>
        public int offset;
        /// <summary>
        /// 片的长度
        /// </summary>
        public int count;
        /// <summary>
        /// 读写位置
        /// </summary>
        public int position;
        /// <summary>
        /// 获取总长度
        /// </summary>
        public int length;

        public Segment2(byte[] buffer)
        {
            offset = 0;
            count = 0;
            position = 0;
            length = buffer.Length;
            this.buffer = buffer;
        }

        #region Write
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value)
        {
            buffer[position++] = value;
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
            fixed (byte* ptr = &buffer[position])
            {
                byte num = 0;
                while (value > 0)
                {
                    num++;
                    ptr[num] = (byte)(value >> 0);
                    value >>= 8;
                }
                ptr[0] = num;
                position += num + 1;
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
            fixed (byte* ptr = &buffer[position])
            {
                byte num = 0;
                while (value > 0)
                {
                    num++;
                    ptr[num] = (byte)(value >> 0);
                    value >>= 8;
                }
                ptr[0] = num;
                position += num + 1;
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
            fixed (byte* ptr = &buffer[position])
            {
                byte num = 0;
                while (value > 0)
                {
                    num++;
                    ptr[num] = (byte)(value >> 0);
                    value >>= 8;
                }
                ptr[0] = num;
                position += num + 1;
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
        public unsafe void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteByte(0);
                return;
            }
            fixed (char* ptr = value)
            {
                fixed (byte* ptr1 = &buffer[position])
                {
                    ptr1[0] = 4;
                    int count = Encoding.UTF8.GetBytes(ptr, value.Length, ptr1 + 5, value.Length * 3);
                    position += 5 + count;
                    byte num = 0;
                    while (count > 0)
                    {
                        num++;
                        ptr1[num] = (byte)(count >> 0);
                        count >>= 8;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] value)
        {
            global::System.Buffer.BlockCopy(value, 0, buffer, position, value.Length);
            position += value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte[] value, int index, int count)
        {
            global::System.Buffer.BlockCopy(value, index, buffer, position, count);
            position += count - index;
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
            return buffer[position++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool()
        {
            return buffer[position++] == 1;
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
            byte num = buffer[position];
            position++;//不安全代码中 i++和++i没区别
            if (num == 0)
                return 0;
            fixed (byte* ptr = &buffer[position])
            {
                position += num;
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
            byte num = buffer[position];
            position++;//不安全代码中 i++和++i没区别
            if (num == 0)
                return 0;
            fixed (byte* ptr = &buffer[position])
            {
                position += num;
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
        public unsafe string ReadString()
        {
            var count = ReadInt32();
            if(count == 0)
                return string.Empty;
            var value = global::System.Text.Encoding.UTF8.GetString(buffer, position, count);
            position += count;
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
            if (position > count)
                count = position;
            if (resetPos)
                position = offset;
        }

        public byte[] ToArray()
        {
            Flush();
            byte[] array = new byte[count];
            global::System.Buffer.BlockCopy(buffer, offset, array, 0, count);
            return array;
        }
    }
}
