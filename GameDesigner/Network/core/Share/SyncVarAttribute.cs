using System;

namespace Net.Share
{
    /// <summary>
    /// 字段或属性同步
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SyncVar : Attribute
    {
        /// <summary>
        /// 唯一id, 如果与服务器NetPlayer变量同步, 就必须设置id
        /// 只与服务器的自身NetPlayer变量同步, 不做任何转发与其他客户端进行变量同步
        /// </summary>
        public ushort id;
        /// <summary>
        /// 值改变后调用的方法名
        /// </summary>
        public string hook;
        /// <summary>
        /// 允许所有客户端改变后同步此字段
        /// </summary>
        public bool authorize;

        /// <summary>
        /// 构造字段,属性同步特性
        /// </summary>
        public SyncVar()
        {
            authorize = true;
        }

        /// <summary>
        /// 构造字段,属性同步特性
        /// </summary>
        public SyncVar(string hook)
        {
            this.hook = hook;
            authorize = true;
        }
    }
}