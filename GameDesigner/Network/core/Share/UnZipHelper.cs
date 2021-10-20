namespace Net.Share
{
    using global::System.IO;
    using global::System.IO.Compression;

    /// <summary>
    /// 压缩数据传输
    /// </summary>
    public static class UnZipHelper
    {
        #region 返回压缩后的字节数组
        /// <summary>
        /// 返回压缩后的字节数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(4096))
            {
                using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress))
                {
                    gzip.Write(buffer, 0, buffer.Length);
                }
                buffer = ms.ToArray();
                return buffer;
            }
        }
        #endregion

        #region 返回解压后的字节数组
        /// <summary>
        /// 返回解压后的字节数组
        /// </summary>
        /// <param name="data">原始字节数组</param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data, int index, int count)
        {
            using (MemoryStream stream = new MemoryStream(data, index, count))
            {
                using (MemoryStream stream1 = new MemoryStream(4096))
                {
                    using (GZipStream decompress = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        decompress.CopyTo(stream1);
                        byte[] result = stream1.ToArray();
                        return result;
                    }
                }
            }
        }

        #endregion
    }
}