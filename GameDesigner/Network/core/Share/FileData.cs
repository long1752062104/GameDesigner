using System.IO;

namespace Net.Share
{
    /// <summary>
    /// 文件传输数据
    /// </summary>
    public class FileData
    {
        /// <summary>
        /// 文件唯一标识
        /// </summary>
        public int ID;
        /// <summary>
        /// 传输的文件名称
        /// </summary>
        public string fileName;
        /// <summary>
        /// 每次发送的数据大小
        /// </summary>
        public int bufferSize;
        /// <summary>
        /// 文件总长度
        /// </summary>
        public long Length;
        /// <summary>
        /// 实际文件写入对象
        /// </summary>
        public FileStream fileStream;
    }
}
