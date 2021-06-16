using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameDesigner
{
    public class StateEvent : MonoBehaviour
    {
        private static StateEvent instance;

        internal class EventQueue
        {
            internal float time;
            internal Action action;

            public EventQueue(float time, Action action)
            {
                this.time = time;
                this.action = action;
            }
        }
        private static List<EventQueue> events = new List<EventQueue>();

        public static void AddEvent(float time, Action action)
        {
            if (instance == null)
                instance = new GameObject("Event").AddComponent<StateEvent>();
            events.Add(new EventQueue(Time.time + time, action));
        }

        void Update()
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (Time.time >= events[i].time) 
                {
                    events[i].action();
                    events.RemoveAt(i);
                    if(i > 0) i--;
                }
            }
        }
    }
}
