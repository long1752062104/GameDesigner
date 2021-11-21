using Net.System;
using System;

namespace Net.Event
{
    public class ActionEvent
    {
        public class Event
        {
            public string name;
            public float time;
            public Action<object> ptr1;
            public Func<object, bool> ptr2;
            public object obj;
            public int invokeNum;
            internal float timeMax;
            internal int eventId;

            public override string ToString()
            {
                return $"{name}";
            }
        }

        public ListSafe<Event> events = new ListSafe<Event>();
        private int eventId;
        private float time;

        public int AddEvent(float time, Action ptr)
        {
            eventId++;
            events.Add(new Event()
            {
                time = this.time + time,
                ptr1 = (o) => { ptr(); },
                eventId = eventId
            });
            return eventId;
        }

        public int AddEvent(float time, Action<object> ptr, object obj)
        {
            eventId++;
            events.Add(new Event()
            {
                time = this.time + time,
                ptr1 = ptr,
                obj = obj,
                eventId = eventId
            });
            return eventId;
        }

        public int AddEvent(float time, int invokeNum, Action<object> ptr, object obj)
        {
            eventId++;
            events.Add(new Event()
            {
                time = this.time + time,
                ptr1 = ptr,
                obj = obj,
                invokeNum = invokeNum,
                timeMax = time,
                eventId = eventId
            });
            return eventId;
        }

        public int AddEvent(float time, Func<bool> ptr)
        {
            return AddEvent("", time, ptr);
        }

        public int AddEvent(string name, float time, Func<bool> ptr)
        {
            eventId++;
            events.Add(new Event()
            {
                name = name,
                time = this.time + time,
                ptr2 = (o) => { return ptr(); },
                eventId = eventId,
                timeMax = time,
            });
            return eventId;
        }

        public int AddEvent(float time, Func<object, bool> func, object obj)
        {
            eventId++;
            events.Add(new Event()
            {
                time = this.time + time,
                ptr2 = func,
                obj = obj,
                invokeNum = 1,
                timeMax = time,
                eventId = eventId
            });
            return eventId;
        }

        public void UpdateEvent(float interval = 0.033f)
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

        public void RemoveEvent(int ptrionId)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].eventId == ptrionId)
                {
                    events.Remove(events[i]);
                    return;
                }
            }
        }
    }
}