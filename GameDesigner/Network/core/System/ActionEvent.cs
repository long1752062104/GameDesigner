using System;
using System.Collections.Generic;

namespace Net.Share
{
    public class ActionEvent
    {
        public class Event
        {
            public float time;
            public Action act;
            public Action<object> act1;
            public object obj;
            public bool hasPars;
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
            actions.Add(new Event() { time = this.time + time, act = act, actionId = actionId });
            return actionId;
        }

        public int AddEvent(float time, Action<object> act, object obj)
        {
            actionId++;
            actions.Add(new Event() { time = this.time + time, act1 = act, obj = obj, hasPars = true, actionId = actionId });
            return actionId;
        }

        public int AddEvent(float time, int invokeNum, Action<object> act, object obj)
        {
            actionId++;
            actions.Add(new Event()
            {
                time = this.time + time,
                act1 = act,
                obj = obj,
                invokeNum = invokeNum,
                timeMax = time,
                hasPars = true,
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
                    if (!actions[i].hasPars)
                        actions[i].act();
                    else
                        actions[i].act1(actions[i].obj);
                    actions[i].invokeNum--;
                    if (actions[i].invokeNum <= 0)
                    {
                        actions.RemoveAt(i);
                        if (i > 0) i--;
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
