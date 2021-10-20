namespace Net.Share
{
    using global::System;

    /// <summary>
    /// 事件处理结构
    /// </summary>
    public struct EventHandle
    {
        /// <summary>
        /// 事件对象
        /// </summary>
        public object obj;
        /// <summary>
        /// 事件委托
        /// </summary>
        public Action<object> action;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="action"></param>
        /// <param name="obj"></param>
        public EventHandle(Action<object> action, object obj)
        {
            this.action = action;
            this.obj = obj;
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        public void Invoke()
        {
            action(obj);
        }
    }
}
