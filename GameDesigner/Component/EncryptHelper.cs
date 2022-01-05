using System;

namespace Net.Helper
{
    /// <summary>
    /// 加密解密帮助类
    /// </summary>
    public class EncryptHelper
    {
        /// <summary>
        /// 随机数形式加密法
        /// </summary>
        /// <param name="password"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] ToEncrypt(int password, byte[] buffer)
        {
            return ToEncrypt(password, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 随机数形式加密法
        /// </summary>
        /// <param name="password"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] ToEncrypt(int password, byte[] buffer, int index, int count)
        {
            if (password < 10000000)
                throw new Exception("密码值不能小于10000000");
            Random random = new Random(password);
            for (int i = index; i < index + count; i++)
            {
                buffer[i] += (byte)random.Next(0, 255);
            }
            return buffer;
        }

        /// <summary>
        /// 随机数形式解密法
        /// </summary>
        /// <param name="password"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] ToDecrypt(int password, byte[] buffer)
        {
            return ToDecrypt(password, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 随机数形式解密法
        /// </summary>
        /// <param name="password"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] ToDecrypt(int password, byte[] buffer, int index, int count)
        {
            if (password < 10000000)
                throw new Exception("密码值不能小于10000000");
            Random random = new Random(password);
            for (int i = index; i < index + count; i++)
            {
                buffer[i] -= (byte)random.Next(0, 255);
            }
            return buffer;
        }
    }
}
