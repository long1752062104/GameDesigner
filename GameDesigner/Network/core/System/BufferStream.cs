using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Net.System
{
    /// <summary>
    /// 共享文件流, 此类会瓜分一个文件流的段数据作为数据缓存位置
    /// </summary>
    public class BufferStream
    {
        public long position;
        public long Length;
        internal long offset;
        internal bool isDispose;

        public long Position => position;

        public void Write(byte[] buffer, int index, int count)
        {
            BufferStreamPool.Write(offset + position, buffer, index, count);
            position += count;
        }

        public void Read(byte[] buffer, int index, int count)
        {
            BufferStreamPool.Read(offset + position, buffer, index, count);
            position += count;
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            position = offset;
        }

        public void Close()
        {
            BufferStreamPool.Push(this);
        }

        ~BufferStream() 
        {
            BufferStreamPool.Push(this);
        }
    }

    public static class BufferStreamPool
    {
        private static readonly string filePath;
        private static readonly FileStream stream;
        private static long Pos;
        public static long Size = 1024 * 1024;
        private static readonly Stack<BufferStream> Stack = new Stack<BufferStream>();

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        static void Init()//有了这个方法就行, 在unity初始化就会进入此类的静态构造函数即可
        {
        }
#endif

        static BufferStreamPool()
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
#if UNITY_STANDALONE || UNITY_WSA
            var streamingAssetsPath = UnityEngine.Application.streamingAssetsPath;
            if (!Directory.Exists(streamingAssetsPath))
                Directory.CreateDirectory(streamingAssetsPath);
            filePath = streamingAssetsPath + "/BufferStreamPool.pool";
#else
            filePath = UnityEngine.Application.persistentDataPath + "/BufferStreamPool.pool";
#endif
#else
            filePath = AppDomain.CurrentDomain.BaseDirectory + "/BufferStreamPool.pool";
#endif
            if (File.Exists(filePath))
                File.Delete(filePath);
            stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        [NonSerialized]
        private static object _syncRoot;
        public static object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                return _syncRoot;
            }
        }

        public static BufferStream Take()
        {
            lock (SyncRoot)
            {
                BufferStream stream;
                if (Stack.Count == 0)
                {
                    stream = new BufferStream
                    {
                        offset = Pos,
                        Length = Size
                    };
                    Pos += Size;
                    return stream;
                }
                stream = Stack.Pop();
                stream.position = 0;
                stream.isDispose = false;
                return stream;
            }
        }

        public static void Push(BufferStream stream)
        {
            lock (SyncRoot) 
            {
                if (stream.isDispose)
                    return;
                stream.isDispose = true;
                Stack.Push(stream);
            }
        }

        public static void Write(long seek, byte[] buffer, int index, int count)
        {
            lock (SyncRoot)
            {
                stream.Seek(seek, SeekOrigin.Begin);
                stream.Write(buffer, index, count);
            }
        }

        public static int Read(long seek, byte[] buffer, int index, int count) 
        {
            lock (SyncRoot)
            {
                stream.Seek(seek, SeekOrigin.Begin);
                return stream.Read(buffer, index, count);
            }
        }
    }
}