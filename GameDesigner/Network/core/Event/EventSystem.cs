namespace Net.Event
{
    using Net.Config;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    /// <summary>
    /// 事件处理静态类, 此类可以用于计时调用事件
    /// </summary>
    public class EventSystem
    {
        private class TimeAct
        {
            public DateTime time;
            public Action action;
            public Action<object> action1;
            public Func<object, bool> action2;
            public object state;
            public int timeValue;
            public int actionId;
        }
        private static readonly List<TimeAct> timeActs = new List<TimeAct>();
        private static int actionId;

        static EventSystem()
        {
            GlobalConfig.ThreadPoolRun = true;
            Task.Run(() =>
            {
                while (GlobalConfig.ThreadPoolRun)
                {
                    Thread.Sleep(1);
                    for (int i = 0; i < timeActs.Count; i++)
                    {
                        try
                        {
                            if (DateTime.Now >= timeActs[i].time)
                            {
                                timeActs[i].action?.Invoke();
                                timeActs[i].action1?.Invoke(timeActs[i].state);
                                if (timeActs[i].action2 == null)
                                    timeActs.RemoveAt(i);
                                else
                                {
                                    if (timeActs[i].action2(timeActs[i].state))
                                        timeActs.RemoveAt(i);
                                    else
                                        timeActs[i].time = DateTime.Now.AddMilliseconds(timeActs[i].timeValue);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            NDebug.LogWarning("事件处理异常:" + ex);
                            timeActs.RemoveAt(i);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 延迟处理，millisecondsTimeout毫秒后执行action方法
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="action"></param>
        /// <returns>返回id标识</returns>
        public static int AddEvent(int millisecondsTimeout, Action action)
        {
            actionId++;
            timeActs.Add(new TimeAct()
            {
                time = DateTime.Now.AddMilliseconds(millisecondsTimeout),
                action = action,
                actionId = actionId
            });
            return actionId;
        }

        /// <summary>
        /// 添加计时器事件, 当系统时间大于或等于(time)时间后调用(action)事件
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        public static void AddEvent(DateTime time, Action action)
        {
            actionId++;
            timeActs.Add(new TimeAct()
            {
                time = time,
                action = action,
                actionId = actionId
            });
        }

        /// <summary>
        /// 添加计时器事件, 当系统时间大于或等于(time)时间后调用(action)事件
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <param name="obj"></param>
        public static void AddEvent(DateTime time, Action<object> action, object obj)
        {
            actionId++;
            timeActs.Add(new TimeAct()
            {
                time = time,
                action1 = action,
                state = obj,
                actionId = actionId
            });
        }

        /// <summary>
        /// 添加计时器事件, 当系统时间大于或等于(time)时间后调用(action)事件
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <param name="obj"></param>
        public static void AddEvent(int time, Action<object> action, object obj)
        {
            actionId++;
            timeActs.Add(new TimeAct()
            {
                time = DateTime.Now.AddMilliseconds(time),
                action1 = action,
                state = obj,
                actionId = actionId
            });
        }

        /// <summary>
        /// 添加计时器事件, 当系统时间大于或等于(time)时间后调用(action)事件, 当action返回true后事件结束, 则每time时间调用一次
        /// </summary>
        /// <param name="time">毫秒单位</param>
        /// <param name="action"></param>
        /// <param name="obj"></param>
        public static void AddEvent(int time, Func<object, bool> action, object obj)
        {
            actionId++;
            timeActs.Add(new TimeAct()
            {
                timeValue = time,
                time = DateTime.Now.AddMilliseconds(time),
                action2 = action,
                state = obj,
                actionId = actionId
            });
        }

        /// <summary>
        /// 添加计时器事件, 当系统时间大于或等于(time)时间后调用(action)事件, 当action返回true后事件结束, 则每time时间调用一次
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        /// <param name="obj"></param>
        public static void AddEvent(DateTime time, Func<object, bool> action, object obj)
        {
            actionId++;
            timeActs.Add(new TimeAct()
            {
                timeValue = time.Subtract(DateTime.Now).Milliseconds,
                time = time,
                action2 = action,
                state = obj,
                actionId = actionId
            });
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="actionId"></param>
        public static void RemoveEvent(int actionId)
        {
            lock (timeActs)
            {
                for (int i = 0; i < timeActs.Count; i++)
                {
                    if (timeActs[i].actionId == actionId)
                    {
                        timeActs.Remove(timeActs[i]);
                        return;
                    }
                }
            }
        }
    }
}