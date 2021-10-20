using System;
using System.Collections.Generic;

namespace Net.Event
{
    public class ActionEvent
    {
        public class Event
        {
            public float time;
            public Action<object> action1;
            public Func<object,bool> action2;
            public object obj;
            public int invokeNum;
            internal float timeMax;
            internal int actionId;
        }

        public List<Event> actions = new List<Event>();
        private int actionId;
        private float time;

        public int AddEvent(float time, Action act)
        {
            actionId++;
            actions.Add(new Event() { time = this.time + time, action1 = (o)=> { act(); }, actionId = actionId });
            return actionId;
        }

        public int AddEvent(float time, Action<object> act, object obj)
        {
            actionId++;
            actions.Add(new Event() { time = this.time + time, action1 = act, obj = obj, actionId = actionId });
            return actionId;
        }

        public int AddEvent(float time, int invokeNum, Action<object> act, object obj)
        {
            actionId++;
            actions.Add(new Event()
            {
                time = this.time + time,
                action1 = act,
                obj = obj,
                invokeNum = invokeNum,
                timeMax = time,
                actionId = actionId
            });
            return actionId;
        }

        public int AddEvent(float time, Func<bool> act)
        {
            actionId++;
            actions.Add(new Event() { time = this.time + time, action2 = (o) => { return act(); }, actionId = actionId });
            return actionId;
        }

        public int AddEvent(float time, Func<object, bool> func, object obj)
        {
            actionId++;
            actions.Add(new Event()
            {
                time = this.time + time,
                action2 = func,
                obj = obj,
                invokeNum = 1,
                timeMax = time,
                actionId = actionId
            });
            return actionId;
        }

        public void UpdateEvent()
        {
            time += 0.033f;
            for (int i = 0; i < actions.Count; i++)
            {
                if (time > actions[i].time)
                {
                    actions[i].action1?.Invoke(actions[i].obj);
                    if (actions[i].action2 != null)
                        if (actions[i].action2(actions[i].obj))
                            continue;
                    actions[i].invokeNum--;
                    if (actions[i].invokeNum <= 0)
                    {
                        actions.RemoveAt(i);
                        if (i >= 0) i--;
                    }
                    else
                    {
                        actions[i].time = time + actions[i].timeMax;
                    }
                }
            }
        }

        public void RemoveEvent(int actionId)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].actionId == actionId)
                {
                    actions.Remove(actions[i]);
                    return;
                }
            }
        }
    }
}
