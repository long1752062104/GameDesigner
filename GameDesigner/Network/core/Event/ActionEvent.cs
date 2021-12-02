using Net.System;
using System;
using System.Threading;

namespace Net.Event
{
    public class ActionEvent
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

            public override string ToString()
            {
                return $"{name}";
            }
        }

        public ListSafe<Event> events = new ListSafe<Event>();
        private int eventId;
        private long time;

        public int AddEvent(float time, Action ptr)
        {
            var enentID = Interlocked.Increment(ref eventId);
            events.Add(new Event()
            {
                time = (long)(this.time + (time * 1000)),
                ptr1 = (o) => { ptr(); },
                eventId = enentID
            });
            return enentID;
        }

        public int AddEvent(float time, Action<object> ptr, object obj)
        {
            var enentID = Interlocked.Increment(ref eventId);
            events.Add(new Event()
            {
                time = (long)(this.time + (time * 1000)),
                ptr1 = ptr,
                obj = obj,
                eventId = enentID
            });
            return enentID;
        }

        public int AddEvent(float time, int invokeNum, Action<object> ptr, object obj)
        {
            var enentID = Interlocked.Increment(ref eventId);
            events.Add(new Event()
            {
                time = (long)(this.time + (time * 1000)),
                ptr1 = ptr,
                obj = obj,
                invokeNum = invokeNum,
                timeMax = (long)(time * 1000),
                eventId = enentID
            });
            return enentID;
        }

        public int AddEvent(float time, Func<bool> ptr)
        {
            return AddEvent("", time, ptr);
        }

        public int AddEvent(string name, float time, Func<bool> ptr)
        {
            var enentID = Interlocked.Increment(ref eventId);
            events.Add(new Event()
            {
                name = name,
                time = (long)(this.time + (time * 1000)),
                ptr2 = (o) => { return ptr(); },
                eventId = enentID,
                timeMax = (long)(time * 1000),
            });
            return enentID;
        }

        public int AddEvent(float time, Func<object, bool> func, object obj)
        {
            var enentID = Interlocked.Increment(ref eventId);
            events.Add(new Event()
            {
                time = (long)(this.time + (time * 1000)),
                obj = obj,
                invokeNum = 1,
                timeMax = (long)(time * 1000),
                eventId = enentID
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
                    events[i].ptr1?.Invoke(events[i].obj);
                    if (events[i].ptr2 != null)
                        if (events[i].ptr2(events[i].obj))
                            goto J;
                    if (--events[i].invokeNum <= 0)
                    {
                        events.RemoveAt(i);
                        if (i >= 0) i--;
                    }
                J: events[i].time = time + events[i].timeMax;
                }
            }
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