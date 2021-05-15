using System;
using System.Collections.Generic;

namespace Net.Share
{
    public class AckQueue
    {
        public uint frame;
        public ushort index;

        public AckQueue()
        {
        }

        public AckQueue(uint frame, ushort index)
        {
            this.frame = frame;
            this.index = index;
        }
    }

    public class RevdAck
    {
        public DateTime time;

        public RevdAck(double seconds)
        {
            time = DateTime.Now.AddSeconds(seconds);
        }
    }

    public class RTBuffer
    {
        public DateTime time;
        public byte[] buffer;

        public RTBuffer(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public RTBuffer(DateTime time, byte[] buffer)
        {
            this.time = time;
            this.buffer = buffer;
        }
    }

    public class FrameList
    {
        public int frameLen;
        public HashSet<ushort> datas = new HashSet<ushort>();
        public long streamPos;
        internal int dataCount;
        internal DateTime time;
        internal uint frame;

        public int Count { get { return datas.Count; } }

        public FrameList(ushort entry)
        {
            frameLen = entry;
        }

        internal bool ContainsKey(ushort index)
        {
            return datas.Contains(index);
        }

        internal bool Add(ushort index)
        {
            return datas.Add(index);
        }
    }
}
