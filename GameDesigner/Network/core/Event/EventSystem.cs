namespace Net.Event
{
    using global::System;
    using Net.System;

    /// <summary>
    /// 事件处理静态类, 此类可以用于计时调用事件
    /// </summary>
    public class EventSystem
    {
        /// <summary>
        /// 延迟处理，millisecondsTimeout毫秒后执行action方法
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="action"></param>
        /// <returns>返回id标识</returns>
        public static int AddEvent(int millisecondsTimeout, Action action)
        {
            return ThreadManager.Event.AddEvent(millisecondsTimeout * 0.001f, action);
        }

        /// <summary>
        /// 添加计时器事件, 当系统时间大于或等于(time)时间后调用(action)事件
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="action"></param>
        /// <param name="obj"></param>
        public static int AddEvent(int millisecondsTimeout, Action<object> action, object obj)
        {
            return ThreadManager.Event.AddEvent(millisecondsTimeout * 0.001f, action, obj);
        }

        /// <summary>
        /// 添加计时器事件, 当系统时间大于或等于(time)时间后调用(action)事件, 当action返回true后事件结束, 则每time时间调用一次
        /// </summary>
        /// <param name="time">毫秒单位</param>
        /// <param name="action"></param>
        /// <param name="obj"></param>
        public static int AddEvent(int time, Func<object, bool> action, object obj)
        {
            return ThreadManager.Event.AddEvent(time * 0.001f, action, obj);
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="actionId"></param>
        public static void RemoveEvent(int actionId)
        {
            ThreadManager.Event.RemoveEvent(actionId);
        }
    }
}