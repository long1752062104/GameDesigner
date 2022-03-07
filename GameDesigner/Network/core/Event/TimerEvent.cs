using Net.System;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Net.Event
{
    /// <summary>
    /// 时间计时器类
    /// </summary>
    public class TimerEvent
    {
        public class Event
        {
            public string name;
            public long time;
            public Action<object> ptr1;
            public Func<object, bool> ptr2;
            public object obj;
            public int invokeNum;
            internal long timeMax;
            internal int eventId;
            internal bool async;
            internal bool complete = true;

            public override string ToString()
            {
                return $"{name}";
            }
        }

        public ListSafe<Event> events = new ListSafe<Event>();
        private int eventId = 1000;
        private long time;

        /// <summary>
        /// 添加计时器事件, time时间后调用ptr
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns></returns>
        public int AddEvent(float time, Action ptr, bool isAsync = false)
        {
            var enentID = Interlocked.Increment(ref eventId);
            events.Add(new Event()
            {
                time = (long)(this.time + (time * 1000)),
                ptr1 = (o) => { ptr(); },
                eventId = enentID,
                async = isAsync,
            });
            return enentID;
        }

        /// <summary>
        /// 添加计时器事件, time时间后调用ptr
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <param name="obj"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns></returns>
        public int AddEvent(float time, Action<object> ptr, object obj, bool isAsync = false)
        {
            var enentID = Interlocked.Increment(ref eventId);
            events.Add(new Event()
            {
                time = (long)(this.time + (time * 1000)),
                ptr1 = ptr,
                obj = obj,
                eventId = enentID,
                async = isAsync,
            });
            return enentID;
        }

        /// <summary>
        /// 添加计时器事件, 当time时间到调用ptr, 总共调用invokeNum次数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="invokeNum">调用次数, -1则是无限循环</param>
        /// <param name="ptr"></param>
        /// <param name="obj"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns></returns>
        public int AddEvent(float time, int invokeNum, Action<object> ptr, object obj, bool isAsync = false)
        {
            var enentID = Interlocked.Increment(ref eventId);
            events.Add(new Event()
            {
                time = (long)(this.time + (time * 1000)),
                ptr1 = ptr,
                obj = obj,
                invokeNum = invokeNum,
                timeMax = (long)(time * 1000),
                eventId = enentID,
                async = isAsync,
            });
            return enentID;
        }

        /// <summary>
        /// 添加计时器事件, 当time时间到调用ptr, 当ptr返回true则time时间后再次调用ptr, 直到ptr返回false为止
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns></returns>
        public int AddEvent(float time, Func<bool> ptr, bool isAsync = false)
        {
            return AddEvent("", time, ptr, isAsync);
        }

        /// <summary>
        /// 添加计时器事件, 当time时间到调用ptr, 当ptr返回true则time时间后再次调用ptr, 直到ptr返回false为止
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns></returns>
        public int AddEvent(string name, float time, Func<bool> ptr, bool isAsync = false)
        {
            var enentID = Interlocked.Increment(ref eventId);
            events.Add(new Event()
            {
                name = name,
                time = (long)(this.time + (time * 1000)),
                ptr2 = (o) => { return ptr(); },
                eventId = enentID,
                timeMax = (long)(time * 1000),
                async = isAsync,
            });
            return enentID;
        }

        /// <summary>
        /// 添加计时事件, 当time时间到调用ptr, 当ptr返回true则time时间后再次调用ptr, 直到ptr返回false为止
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <param name="obj"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns></returns>
        public int AddEvent(float time, Func<object, bool> ptr, object obj, bool isAsync = false)
        {
            var enentID = Interlocked.Increment(ref eventId);
            events.Add(new Event()
            {
                time = (long)(this.time + (time * 1000)),
                ptr2 = ptr,
                obj = obj,
                invokeNum = 1,
                timeMax = (long)(time * 1000),
                eventId = enentID,
                async = isAsync,
            });
            return enentID;
        }

        public void UpdateEvent(int interval = 33)
        {
            time += interval;
            for (int i = 0; i < events.Count; i++)
            {
                if (time > events[i].time)
                {
                    if (events[i].ptr1 != null)
                    {
                        if (events[i].async)
                            WorkExecute1(events[i]);
                        else
                            events[i].ptr1(events[i].obj);
                    }
                    else if (events[i].ptr2 != null)
                    {
                        if (events[i].async)
                        {
                            WorkExecute2(events[i]);
                            continue;
                        }
                        if (events[i].ptr2(events[i].obj))
                            goto J;
                    }
                    if (events[i].invokeNum == -1)
                        goto J;
                    if (--events[i].invokeNum <= 0)
                    {
                        events.RemoveAt(i);
                        if (i >= 0) i--;
                        continue;//解决J:执行后索引超出异常
                    }
                J: events[i].time = time + events[i].timeMax;
                }
            }
        }

        private async void WorkExecute1(Event @event)
        {
            if (!@event.complete)
                return;
            @event.complete = false;
            await default(YieldAwaitable);
            @event.ptr1(@event.obj);
            if (--@event.invokeNum <= 0)
                events.Remove(@event);
            else
                @event.time = time + @event.timeMax;
            @event.complete = true;
        }

        private async void WorkExecute2(Event @event)
        {
            if (!@event.complete)
                return;
            @event.complete = false;
            await default(YieldAwaitable);
            if (@event.ptr2(@event.obj))
                @event.time = time + @event.timeMax;
            else
                events.Remove(@event);
            @event.complete = true;
        }

        public void RemoveEvent(int eventId)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].eventId == eventId)
                {
                    events.Remove(events[i]);
                    return;
                }
            }
        }
    }
}