namespace Net.Client
{
    using Net.Share;
    using global::System;
    using global::System.Threading;
    using UnityEngine;

    /// <summary>
    /// 网络行为，此类负责网络增删Rpc远程过程调用函数，使用到网络通讯功能，需要继承此类
    /// </summary>
    public abstract class NetBehaviour : MonoBehaviour, ISendHandle
    {
        private ISendHandle handle;
        /// <summary>
        /// 客户端发送接口
        /// </summary>
        public ISendHandle Handle
        {
            get
            {
                if (handle == null)
                    handle = ClientBase.Instance;
                return handle;
            }
        }

        /// <summary>
        /// 添加远程过程调用函数的委托
        /// </summary>
        /// <param name="target">远程过程调用指定的对象</param>
        /// <param name="append">一个Rpc方法是否可以多次添加到Rpcs里面？</param>
        public static void AddRpc(object target, bool append = false)
        {
            AddRpc(ClientBase.Instance, target, append);
        }

        /// <summary>
        /// 添加远程过程调用函数的委托
        /// </summary>
        /// <param name="client">添加RPC到此客户端</param>
        /// <param name="target">远程过程调用指定的对象</param>
        /// <param name="append">一个Rpc方法是否可以多次添加到Rpcs里面？</param>
        public static void AddRpc(ClientBase client, object target, bool append = false)
        {
            if (client == null)
                return;
            client.AddRpcHandle(target, append);
        }

        /// <summary>
        /// 移除RPCFun函数
        /// </summary>
        /// <param name="target">将此对象的所有带有RPCFun特性的函数移除</param>
        public static void RemoveRpc(object target)
        {
            RemoveRpc(ClientBase.Instance, target);
        }

        /// <summary>
        /// 移除子客户端的RPCFun函数
        /// </summary>
        /// <param name="client">子客户端对象</param>
        /// <param name="target">将此对象的所有带有RPCFun特性的函数移除</param>
        public static void RemoveRpc(ClientBase client, object target)
        {
            client.RemoveRpc(target);
        }

        /// <summary>
        /// <para>当游戏开始初始化此对象时，搜索继承此类的方法中带有RPCFun或Rpc特性的方法，并添加到远程过程调用委托集合变量里</para>
        /// <para>如果重写此方法必须base.Awake();</para>
        /// </summary>
        protected virtual void Awake()
        {
            if (ClientBase.Instance == null)
                goto Jmp;
            if (!ClientBase.Instance.Connected)
                goto Jmp;
            InitRpc();
            return;
        Jmp: Net.Event.EventSystem.AddEvent(20, (state) =>
        {
            if (ClientBase.Instance == null)
                return false;
            if (!ClientBase.Instance.Connected)
                return false;
            InitRpc();
            return true;
        }, null);
        }

        private void InitRpc()
        {
            AddRpc(this);
            handle = ClientBase.Instance;
        }

        #region 发送网络数据接口处理程序
        public void CallRpc(string fun, params object[] pars)
        {
            Handle.CallRpc(fun, pars);
        }

        public void CallRpc(byte cmd, string func, params object[] pars)
        {
            Handle.CallRpc(cmd, func, pars);
        }

        public void Request(string func, params object[] pars)
        {
            Handle.Request(func, pars);
        }

        public void Request(byte cmd, string func, params object[] pars)
        {
            Handle.Request(cmd, func, pars);
        }

        public void Send(byte[] buffer)
        {
            Handle.Send(buffer);
        }

        public void Send(byte cmd, byte[] buffer)
        {
            Handle.Send(cmd, buffer);
        }

        public void Send(string func, params object[] pars)
        {
            Handle.Send(func, pars);
        }

        public void Send(byte cmd, string func, params object[] pars)
        {
            Handle.Send(cmd, func, pars);
        }

        public void SendRT(string func, params object[] pars)
        {
            Handle.SendRT(func, pars);
        }

        public void SendRT(byte cmd, string func, params object[] pars)
        {
            Handle.SendRT(cmd, func, pars);
        }

        public void SendRT(byte[] buffer)
        {
            Handle.SendRT(buffer);
        }

        public void SendRT(byte cmd, byte[] buffer)
        {
            Handle.SendRT(cmd, buffer);
        }

        public void Send(string func, string callbackFunc, Delegate callback, params object[] pars)
        {
            Handle.Send(func, callbackFunc, callback, pars);
        }

        public void Send(byte cmd, string func, string callbackFunc, Delegate callback, params object[] pars)
        {
            Handle.Send(cmd, func, callbackFunc, callback, pars);
        }

        public void Send(string func, string callbackFunc, Action callback, params object[] pars)
        {
            Handle.Send(func, callbackFunc, callback, pars);
        }

        public void Send(byte cmd, object obj)
        {
            Handle.Send(cmd, obj);
        }

        public void SendRT(byte cmd, object obj)
        {
            Handle.SendRT(cmd, obj);
        }

        public void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars)
        {
            Handle.Send(func, funcCB, callback, millisecondsDelay, pars);
        }

        public void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            Handle.Send(func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            Handle.Send(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void SendRT(string func, string funcCB, Delegate callback, params object[] pars)
        {
            Handle.SendRT(func, funcCB, callback, pars);
        }

        public void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars)
        {
            Handle.SendRT(func, funcCB, callback, millisecondsDelay, pars);
        }

        public void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            Handle.SendRT(func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            Handle.SendRT(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            Handle.SendRT(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, context, pars);
        }

        public void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            Handle.Send(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, context, pars);
        }
        #endregion
    }
}