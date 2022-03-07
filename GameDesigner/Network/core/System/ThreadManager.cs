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
        private static Thread MainThread;
        public static TimerEvent Event { get; private set; } = new TimerEvent();

        static ThreadManager()
        {
            Run();
        }

        private static void Run()
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
            });
            MainThread.IsBackground = true;
            MainThread.Priority = ThreadPriority.Highest;
            MainThread.Start();
        }

        public static void PingRun()
        {
            string str = MainThread.ThreadState.ToString();
            if (str.Contains("Abort") | str.Contains("Stop") | str.Contains("WaitSleepJoin"))
            {
                Run();
            }
        }

        public static int Invoke(Func<bool> ptr, bool isAsync = false) 
        {
            return Event.AddEvent(0, ptr, isAsync);
        }

        public static int Invoke(Action ptr, bool isAsync = false)
        {
            return Event.AddEvent(0, ptr, isAsync);
        }

        public static int Invoke(string name, Func<bool> ptr, bool isAsync = false)
        {
            return Event.AddEvent(name, 0, ptr, isAsync);
        }

        public static int Invoke(float time, Func<bool> ptr, bool isAsync = false)
        {
            return Event.AddEvent(time, ptr, isAsync);
        }

        public static int Invoke(string name, float time, Func<bool> ptr, bool isAsync = false)
        {
            return Event.AddEvent(name, time, ptr, isAsync);
        }
    }
}
