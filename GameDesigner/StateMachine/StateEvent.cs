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
            internal int id;
            internal float time;
            internal Action action;
            public EventQueue(float time, Action action)
            {
                this.time = time;
                this.action = action;
            }
        }
        private static int ID = 0;
        private static readonly List<EventQueue> events = new List<EventQueue>();
        
        public static int AddEvent(float time, Action action)
        {
            if (instance == null)
            {
                instance = new GameObject("Event").AddComponent<StateEvent>();
                DontDestroyOnLoad(instance);
            }
            int id = ID++;
            events.Add(new EventQueue(Time.time + time, action) { id = id });
            return id;
        }

        public static void RemoveEvent(int id) 
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].id == id)
                {
                    events.RemoveAt(i);
                    break;
                }
            }
        }

        public static void RemoveAllEvent()
        {
            events.Clear();
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
