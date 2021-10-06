using System;

namespace Net.Share
{
    /// <summary>
    /// 变量或属性同步特性
    /// 服务器的同步特性只能用<see cref="Server.NetPlayer.AddRpcHandle(object)"/>方法来收集
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VarSync : Attribute
    {
        /// <summary>
        /// 变量或属性的唯一标识
        /// </summary>
        public ushort id;
        /// <summary>
        /// 你的字段,属性不能主动同步, 只能被对方同步
        /// </summary>
        public bool passive;

        /// <summary>
        /// 构造字段,属性同步特性
        /// 服务器的同步特性只能用<see cref="Server.NetPlayer.AddRpcHandle(object)"/>方法来收集
        /// </summary>
        public VarSync()
        {
        }

        /// <summary>
        /// 构造字段,属性同步特性
        /// 服务器的同步特性只能用<see cref="Server.NetPlayer.AddRpcHandle(object)"/>方法来收集
        /// </summary>
        /// <param name="id">唯一id, 不能重复</param>
        public VarSync(ushort id)
        {
            this.id = id;
        }
    }
}