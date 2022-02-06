using Net;
using Net.Component;
using Net.Server;
using Net.Share;
using System;
using System.Data;
using System.Threading;

namespace Example2
{
    /// <summary>
    /// 服务器玩家组件, 这个组件处理 玩家移动操作, 后面增加攻击操作, 碰撞操作....
    /// </summary>
    public class Player : WebPlayer, ISendHandle
    {
        public UserinfoData data;
        internal NTransform transform = new NTransform();
        internal bool isDead;
        internal Scene scene;

        public override void OnEnter()
        {
            data.Health = 100;
            isDead = false;
            scene = Scene as Scene;
        }

        public override void OnUpdate()
        {
            scene.AddOperation(new Operation(Command.PlayerState, UserID) {
               index1 = (int)data.Health
            });
        }

        internal void BeAttacked(int damage)
        {
            if (isDead)
                return;
            data.Health -= damage;
            if (data.Health <= 0)
            {
                isDead = true;
                data.Health = 0;
            }
        }

        public void Resurrection()
        {
            data.Health = data.HealthMax;
            isDead = false;
        }

        #region 扩展网络请求
        public void Send(byte[] buffer)
        {
            Service.Instance.Send(this, buffer);
        }

        public void Send(byte cmd, byte[] buffer)
        {
            Service.Instance.Send(this, cmd, buffer);
        }

        public void Send(string func, params object[] pars)
        {
            Service.Instance.Send(this, func, pars);
        }

        public void Send(byte cmd, string func, params object[] pars)
        {
            Service.Instance.Send(this, cmd, func, pars);
        }

        public void CallRpc(string func, params object[] pars)
        {
            Service.Instance.Send(this, func, pars);
        }

        public void CallRpc(byte cmd, string func, params object[] pars)
        {
            Service.Instance.Send(this, cmd, func, pars);
        }

        public void Request(string func, params object[] pars)
        {
            Service.Instance.Send(this, func, pars);
        }

        public void Request(byte cmd, string func, params object[] pars)
        {
            Service.Instance.Send(this, cmd, func, pars);
        }

        public void SendRT(string func, params object[] pars)
        {
            Service.Instance.SendRT(this, func, pars);
        }

        public void SendRT(byte cmd, string func, params object[] pars)
        {
            Service.Instance.SendRT(this, cmd, func, pars);
        }

        public void SendRT(byte[] buffer)
        {
            Service.Instance.SendRT(this, buffer);
        }

        public void SendRT(byte cmd, byte[] buffer)
        {
            Service.Instance.SendRT(this, cmd, buffer);
        }

        public void Send(string func, string callbackFunc, Action callback, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void Send(byte cmd, string func, string callbackFunc, Action callback, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void Send(string func, string callbackFunc, Action<object[]> callback, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void Send(byte cmd, string func, string callbackFunc, Action<object[]> callback, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void Send(string func, string callbackFunc, Delegate callback, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void Send(byte cmd, string func, string callbackFunc, Delegate callback, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void Send(byte cmd, object obj)
        {
            Service.Instance.Send(this, cmd, obj);
        }

        public void SendRT(byte cmd, object obj)
        {
            Service.Instance.SendRT(this, cmd, obj);
        }

        public void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void SendRT(string func, string funcCB, Delegate callback, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            throw new NotImplementedException();
        }

        public void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}