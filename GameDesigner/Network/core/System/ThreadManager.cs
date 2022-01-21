using Net.Config;
using Net.Event;
using System;
using System.Threading;

namespace Net.System
{
    /// <summary>
    /// 主线程管理中心
    /// </summary>
    public static class ThreadManager
    {
        private static readonly Thread MainThread;
        public static TimerEvent Event { get; private set; } = new TimerEvent();

        static ThreadManager()
        {
            GlobalConfig.ThreadPoolRun = true;
            MainThread = new Thread(() =>
            {
                while (GlobalConfig.ThreadPoolRun)
                {
                    try
                    {
                        Thread.Sleep(1);
                        Event.UpdateEvent(2);
                    }
                    catch (Exception ex)
                    {
                        NDebug.LogError("主线程异常:" + ex);
                    }
                }
            }) { IsBackground = true };
            MainThread.Priority = ThreadPriority.Highest;
            MainThread.Start();
        }

        public static int Invoke(Func<bool> ptr) 
        {
            return Event.AddEvent(0, ptr);
        }

        public static int Invoke(Action ptr)
        {
            return Event.AddEvent(0, ptr);
        }

        public static int Invoke(string name, Func<bool> ptr)
        {
            return Event.AddEvent(name, 0, ptr);
        }

        public static int Invoke(float time, Func<bool> ptr)
        {
            return Event.AddEvent(time, ptr);
        }

        public static int Invoke(string name, float time, Func<bool> ptr)
        {
            return Event.AddEvent(name, time, ptr);
        }
    }
}
