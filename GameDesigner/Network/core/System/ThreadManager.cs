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
        public static ActionEvent Event { get; private set; } = new ActionEvent();

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
                        Event.UpdateEvent(0.002f);
                    }
                    catch (Exception ex)
                    {
                        NDebug.LogError("主线程异常:" + ex);
                    }
                }
            }) { IsBackground = true };
            MainThread.Start();
        }

        public static void Invoke(Func<bool> ptr) 
        {
            Event.AddEvent(0, ptr);
        }

        public static void Invoke(string name, Func<bool> ptr)
        {
            Event.AddEvent(name, 0, ptr);
        }

        public static void Invoke(float time, Func<bool> ptr)
        {
            Event.AddEvent(time, ptr);
        }

        public static void Invoke(string name, float time, Func<bool> ptr)
        {
            Event.AddEvent(name, time, ptr);
        }
    }
}
