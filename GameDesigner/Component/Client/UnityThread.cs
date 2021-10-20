#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using global::System;
    using global::System.Threading;

    /// <summary>
    /// 提供对unity主线程的访问
    /// </summary>
    public class UnityThread : SingleCase<UnityThread>
    {
        /// <summary>
        /// 在多线程调用unity主线程的上下文对象
        /// </summary>
        public static SynchronizationContext Context { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            Context = SynchronizationContext.Current;
        }

        public static void Call(Action action)
        {
            Context.Post((act) =>
            {
                ((Action)act)();
            }, action);
        }

        public static void Call(SendOrPostCallback action, object par)
        {
            Context.Post(action, par);
        }
    }
}
#endif