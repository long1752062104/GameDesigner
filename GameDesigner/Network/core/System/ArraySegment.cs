using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.System
{
    /// <summary>
    /// 内存字节数组片
    /// </summary>
    public struct ArraySegment
    {
        private byte[] array;
        private int position;
        private int count;
        public int Position { get{ return position; } set { position = value; } }
        public int Length => count;

        public ArraySegment(byte[] array, int position, int count)
        {
            this.array = array;
            this.position = position;
            this.count = count;
        }

        public byte this[int index] 
        {
            get 
            {
                return array[position + index];
            }
            set 
            {
                array[position + index] = value;
            }
        }

        public static implicit operator ArraySegment(byte[] buffer)
        {
            return new ArraySegment(buffer, 0, buffer.Length);
        }

        public static implicit operator byte[](ArraySegment segment)
        {
            return segment.array;
        }
    }
}
