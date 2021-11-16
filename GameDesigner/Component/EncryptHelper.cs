using System;

namespace Net.Component
{
    /// <summary>
    /// 加密解密帮助类
    /// </summary>
    public class EncryptHelper
    {
        /// <summary>
        /// 随机数形式加密法
        /// </summary>
        /// <param name="Password"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] ToEncrypt(int Password, byte[] buffer)
        {
            if (Password < 10000000)
                throw new Exception("密码值不能小于10000000");
            Random random = new Random(Password);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] += (byte)random.Next(0, 255);
            }
            return buffer;
        }

        /// <summary>
        /// 随机数形式解密法
        /// </summary>
        /// <param name="Password"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] ToDecrypt(int Password, byte[] buffer)
        {
            if (Password < 10000000)
                throw new Exception("密码值不能小于10000000");
            Random random = new Random(Password);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] -= (byte)random.Next(0, 255);
            }
            return buffer;
        }
    }
}
