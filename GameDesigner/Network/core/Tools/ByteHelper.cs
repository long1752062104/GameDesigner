using System;

namespace Net.Share
{
    public static class ByteHelper
    {
        /// <summary>
        /// 转换方法
        /// </summary>
        /// <param name="size">字节值</param>
        /// <returns></returns>
        public static string HumanReadableFilesize(double size)
        {
            string[] units = new string[] { "B", "K", "M", "G", "T", "P" };
            double mod = 1024.0;
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            return Math.Round(size, 2) + units[i];
        }
    }
}
