using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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

        /// <summary> 
        /// 加密字符串  
        /// </summary> 
        /// <param name="text">要加密的字符串</param> 
        /// <returns>加密后的字符串</returns> 
        public static string DESEncrypt(string encryptKey, string text)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(encryptKey);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(text);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary> 
        /// 解密字符串  
        /// </summary> 
        /// <param name="text">要解密的字符串</param> 
        /// <returns>解密后的字符串</returns>   
        public static string DESDecrypt(string encryptKey, string text)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(encryptKey);
            byte[] toEncryptArray = Convert.FromBase64String(text);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
