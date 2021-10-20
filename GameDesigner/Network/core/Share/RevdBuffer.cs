namespace Net.Share
{
    using global::System;
    using global::System.Reflection;
    using global::System.Runtime.InteropServices;

    /// <summary>
    /// 网络接收缓存结构体
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RevdBuffer
    {
        /// <summary>
        /// 函数和参数的名称
        /// </summary>
        public string name;
        /// <summary>
        /// 存储封包反序列化出来的对象
        /// </summary>
        public object target;
        /// <summary>
        /// 存储反序列化的函数
        /// </summary>
        public MethodInfo method;
        /// <summary>
        /// 存储反序列化参数
        /// </summary>
        public object[] pars;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="target">远程调用对象</param>
        /// <param name="method">远程调用方法</param>
        /// <param name="pars">远程调用参数</param>
        public RevdBuffer(object target, MethodInfo method, params object[] pars)
        {
            name = method.ToString();
            this.target = target;
            this.method = method;
            this.pars = pars;
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <returns></returns>
        public void Invoke()
        {
            if (method == null)
                return;
            method.Invoke(target, pars);
        }

        public override string ToString()
        {
            return $"{target}->{name}";
        }
    }
}

