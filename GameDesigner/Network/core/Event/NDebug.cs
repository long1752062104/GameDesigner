namespace Net.Event
{
    using global::System;
    using global::System.Reflection;
    using global::System.Threading;
    using Net.System;

    /// <summary>
    /// 消息输入输出处理类
    /// </summary>
    public static class NDebug
    {
        /// <summary>
        /// 输出调式消息
        /// </summary>
        public static event Action<string> LogHandle;
        /// <summary>
        /// 输出调式错误消息
        /// </summary>
        public static event Action<string> LogErrorHandle;
        /// <summary>
        /// 输出调式警告消息
        /// </summary>
        public static event Action<string> LogWarningHandle;
        /// <summary>
        /// 输出日志最多容纳条数
        /// </summary>
        public static int LogMax { get; set; } = 500;
        /// <summary>
        /// 输出错误日志最多容纳条数
        /// </summary>
        public static int LogErrorMax { get; set; } = 500;
        /// <summary>
        /// 输出警告日志最多容纳条数
        /// </summary>
        public static int LogWarningMax { get; set; } = 500;

        private static QueueSafe<object> logQueue = new QueueSafe<object>();
        private static QueueSafe<object> errorQueue = new QueueSafe<object>();
        private static QueueSafe<object> warningQueue = new QueueSafe<object>();
        private static Thread thread, thread1;

#if SERVICE
        static NDebug()
        {
            ToLogHandle();
            CheckEventsHandle();
        }

        private static void ToLogHandle()
        {
            thread = new Thread(() =>
            {
                while (thread != null)
                {
                    try
                    {
                        Thread.Sleep(1);
                        if (logQueue.TryDequeue(out object message))
                            LogHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Log] {message}");
                        if (errorQueue.TryDequeue(out message))
                            LogErrorHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Error] {message}");
                        if (warningQueue.TryDequeue(out message))
                            LogWarningHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Warning] {message}");
                        if (logQueue.Count >= LogMax)
                            logQueue = new QueueSafe<object>();
                        if (errorQueue.Count >= LogErrorMax)
                            errorQueue = new QueueSafe<object>();
                        if (warningQueue.Count >= LogWarningMax)
                            warningQueue = new QueueSafe<object>();
                    }
                    catch (Exception ex)
                    {
                        errorQueue.Enqueue(ex.Message);
                    }
                }
            }){ Name = "Log" };
            thread.Start();
        }

        //检测事件委托函数
        private static void CheckEventsHandle()
        {
            thread1 = new Thread(() =>
            {
                Type type = typeof(NDebug);
                EventInfo[] es = type.GetEvents(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
                while (thread1 != null)
                {
                    try
                    {
                        Thread.Sleep(1);
                        for (int i = 0; i < es.Length; i++)
                        {
                            FieldInfo f = type.GetField(es[i].Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
                            if (f == null)
                                continue;

                            object value = f.GetValue(null);
                            if (value == null)
                                continue;

                            Delegate dele = (Delegate)value;
                            Delegate[] ds = dele.GetInvocationList();
                            for (int a = 0; a < ds.Length; a++)
                            {
                                if (ds[a].Method == null)
                                {
                                    es[i].RemoveEventHandler(null, ds[a]);
                                    continue;
                                }
                                if (ds[a].Method.IsStatic)//静态方法不需要判断对象是否为空
                                    continue;
                                if (ds[a].Target == null)
                                {
                                    es[i].RemoveEventHandler(null, ds[a]);
                                    continue;
                                }
                                if (ds[a].Target.Equals(null) | ds[a].Method.Equals(null))
                                {
                                    es[i].RemoveEventHandler(null, ds[a]);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(ex);
                    }
                }
            }) { Name = "LogCheckEvent" };
            thread1.Start();
        }
#endif

        /// <summary>
        /// 输出调式消息
        /// </summary>
        /// <param name="message"></param>
        public static void Log(object message)
        {
#if SERVICE
            logQueue.Enqueue(message);
#else
            LogHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Log] {message}");
#endif
        }

        /// <summary>
        /// 输出错误消息
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(object message)
        {
#if SERVICE
            errorQueue.Enqueue(message);
#else
            LogErrorHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Error] {message}");
#endif
        }

        /// <summary>
        /// 输出警告消息
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(object message)
        {
#if SERVICE
            warningQueue.Enqueue(message);
#else
            LogWarningHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Warning] {message}");
#endif
        }

        public static void BindLogAll(Action<string> log)
        {
            LogHandle += log;
            LogWarningHandle += log;
            LogErrorHandle += log;
        }

        public static void RemoveLogAll(Action<string> log)
        {
            LogHandle -= log;
            LogWarningHandle -= log;
            LogErrorHandle -= log;
        }
    }
}