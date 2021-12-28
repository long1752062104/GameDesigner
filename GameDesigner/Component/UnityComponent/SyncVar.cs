using System;

namespace Net.UnityComponent
{
    /// <summary>
    /// 字段或属性同步
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SyncVar : Attribute
    {
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
        /// <param name="id">唯一id, 不能重复</param>
        public SyncVar(string hook)
        {
            this.hook = hook; 
            authorize = true;
        }
    }
}
