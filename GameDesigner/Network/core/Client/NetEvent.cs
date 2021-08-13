using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Net.Client
{
    /// <summary>
    /// 网络事件
    /// </summary>
    public sealed class NetEvent : Event.EventSystem
    {
        /// <summary>
        /// unity同步线程
        /// </summary>
        public static SynchronizationContext Context;

        /// <summary>
        /// 添加unity主线程同步事件, action回调在unity主程序被调用
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="action"></param>
        public static void AddContextEvent(int millisecondsTimeout, Action action)
        {
            if (Context == null)
                Context = SynchronizationContext.Current;
            AddEvent(millisecondsTimeout, (state) =>
            {
                Context.Post(new SendOrPostCallback((state1) =>
                {
                    ((Action)state1)();
                }), state);
            }, action);
        }

        /// <summary>
        /// 添加unity主线程同步事件, action回调在unity主程序被调用
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="action"></param>
        public static void AddContextEvent(int millisecondsTimeout, Action<object> action, object state)
        {
            if (Context == null)
                Context = SynchronizationContext.Current;
            AddEvent(millisecondsTimeout, (state1) =>
            {
                Context.Post(new SendOrPostCallback((state2) =>
                {
                    action(state2);
                }), state1);
            }, state);
        }

        /// <summary>
        /// 添加携程事件
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="milliseconds"></param>
        /// <param name="updateAct"></param>
        /// <param name="endAct"></param>
        public static void AddCoroutineEvent(MonoBehaviour mono, int milliseconds, Action act)
        {
            mono.StartCoroutine(WaitHandle(milliseconds, () => { }, act));
        }

        /// <summary>
        /// 添加携程事件
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="milliseconds"></param>
        /// <param name="updateAct"></param>
        /// <param name="endAct"></param>
        public static void AddCoroutineEvent(MonoBehaviour mono, int milliseconds, Action updateAct, Action endAct)
        {
            mono.StartCoroutine(WaitHandle(milliseconds, updateAct, endAct));
        }

        private static IEnumerator WaitHandle(int milliseconds, Action updateAct, Action endAct)
        {
            DateTime time = DateTime.Now.AddMilliseconds(milliseconds);
            while (DateTime.Now < time)
            {
                updateAct();
                yield return null;
            }
            endAct();
        }

        /// <summary>
        /// 添加携程事件, 每time秒调用一次, 调用invokeNum次
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="time"></param>
        /// <param name="invokeNum"></param>
        /// <param name="updateAct"></param>
        /// <param name="obj"></param>
        public static void AddCoroutineEvent(MonoBehaviour mono, float time, int invokeNum, Action<object> updateAct, object obj)
        {
            mono.StartCoroutine(WaitHandle(time, invokeNum, updateAct, obj));
        }

        /// <summary>
        /// 添加携程事件, 每time秒调用一次, 调用invokeNum次
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="time"></param>
        /// <param name="invokeNum"></param>
        /// <param name="updateAct"></param>
        /// <param name="obj"></param>
        public static void AddCoroutineEvent<T>(MonoBehaviour mono, float time, int invokeNum, Action<T> updateAct, T obj)
        {
            mono.StartCoroutine(WaitHandle(time, invokeNum, updateAct, obj));
        }

        private static IEnumerator WaitHandle<T>(float time, int invokeNum, Action<T> updateAct, T obj)
        {
            while (invokeNum > 0)
            {
                yield return new WaitForSeconds(time);
                invokeNum--;
                updateAct(obj);
            }
        }

        /// <summary>
        /// 添加携程事件
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="milliseconds"></param>
        /// <param name="updateAct"></param>
        /// <param name="endAct"></param>
        public static void AddCoroutineEvent(MonoBehaviour mono, Func<bool> updateAct)
        {
            mono.StartCoroutine(WaitHandle(updateAct));
        }

        private static IEnumerator WaitHandle(Func<bool> updateAct)
        {
            while (true)
            {
                if (updateAct())
                    yield break;
                yield return null;
            }
        }

        /// <summary>
        /// 添加携程事件
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="milliseconds"></param>
        /// <param name="updateAct"></param>
        /// <param name="endAct"></param>
        public static void AddCoroutineEvent(MonoBehaviour mono, Func<object, bool> updateAct, object obj)
        {
            mono.StartCoroutine(WaitHandle(updateAct, obj));
        }

        /// <summary>
        /// 添加携程事件
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="milliseconds"></param>
        /// <param name="updateAct"></param>
        /// <param name="endAct"></param>
        public static void AddCoroutineEvent<T>(MonoBehaviour mono, Func<T, bool> updateAct, T obj)
        {
            mono.StartCoroutine(WaitHandle(updateAct, obj));
        }

        private static IEnumerator WaitHandle<T>(Func<T, bool> updateAct, T obj)
        {
            while (true)
            {
                if (updateAct(obj))
                    yield break;
                yield return null;
            }
        }
    }
}