using System;
using System.Collections.Generic;
using TrueSync;

namespace LockStep
{
    public static class EventSystem
    {
        public class Evt
        {
            public FP time;
            public Action act;
            public Action<object> act1;
            public object obj;
            public bool hasPars;
            public int invokeNum;
            internal FP timeMax;
            internal int actionId;
        }

        public static List<Evt> actions = new List<Evt>();
        private static int actionId;

        internal static int AddEvent(FP time, Action act)
        {
            actionId++;
            actions.Add(new Evt() { time = LSTime.time + time, act = act, actionId = actionId });
            return actionId;
        }

        internal static int AddEvent(FP time, Action<object> act, object obj)
        {
            actionId++;
            actions.Add(new Evt() { time = LSTime.time + time, act1 = act, obj = obj, hasPars = true, actionId = actionId });
            return actionId;
        }

        internal static int AddEvent(FP time, int invokeNum, Action<object> act, object obj)
        {
            actionId++;
            actions.Add(new Evt()
            {
                time = LSTime.time + time,
                act1 = act,
                obj = obj,
                invokeNum = invokeNum,
                timeMax = time,
                hasPars = true,
                actionId = actionId
            });
            return actionId;
        }

        public static void UpdateEvent()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                if (LSTime.time > actions[i].time)
                {
                    if (!actions[i].hasPars)
                        actions[i].act();
                    else
                        actions[i].act1(actions[i].obj);
                    actions[i].invokeNum--;
                    if (actions[i].invokeNum <= 0)
                    {
                        actions.RemoveAt(i);
                        if (i > 0)
                            i--;
                    }
                    else
                    {
                        actions[i].time = LSTime.time + actions[i].timeMax;
                    }
                }
            }
        }

        public static void RemoveEvent(int actionId)
        {
            lock (actions)
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
}
