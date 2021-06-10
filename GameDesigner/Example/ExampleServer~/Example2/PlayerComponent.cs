using Net.Server;
using Net.Share;
using System;
using System.Threading;

namespace Net.Example2
{
    public class PlayerData : ISerializableData
    {
        public string UIDKey { get; set; }
        public string account { get { return UIDKey; } set { UIDKey = value; } }
        public string password;
        public float moveSpeed = 5f;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 direction;
        public float health = 1000;
        public string name;
        public bool teamTag;//true:A退伍 false:B退伍
        public bool ready;//准备?
    }

    /// <summary>
    /// 服务器玩家组件, 这个组件处理 玩家移动操作, 后面增加攻击操作, 碰撞操作....
    /// </summary>
    public class PlayerComponent : WebPlayer, ISendHandle
    {
        public PlayerData data = new PlayerData();
        public new SceneComponent Scene;

        #region 扩展网络请求
        public void Send(byte[] buffer)
        {
            ServiceComponent.Instance.Send(this, buffer);
        }

        public void Send(byte cmd, byte[] buffer)
        {
            ServiceComponent.Instance.Send(this, cmd, buffer);
        }

        public void Send(string func, params object[] pars)
        {
            ServiceComponent.Instance.Send(this, func, pars);
        }

        public void Send(byte cmd, string func, params object[] pars)
        {
            ServiceComponent.Instance.Send(this, cmd, func, pars);
        }

        public void CallRpc(string func, params object[] pars)
        {
            ServiceComponent.Instance.Send(this, func, pars);
        }

        public void CallRpc(byte cmd, string func, params object[] pars)
        {
            ServiceComponent.Instance.Send(this, cmd, func, pars);
        }

        public void Request(string func, params object[] pars)
        {
            ServiceComponent.Instance.Send(this, func, pars);
        }

        public void Request(byte cmd, string func, params object[] pars)
        {
            ServiceComponent.Instance.Send(this, cmd, func, pars);
        }

        public void SendRT(string func, params object[] pars)
        {
            ServiceComponent.Instance.SendRT(this, func, pars);
        }

        public void SendRT(byte cmd, string func, params object[] pars)
        {
            ServiceComponent.Instance.SendRT(this, cmd, func, pars);
        }

        public void SendRT(byte[] buffer)
        {
            ServiceComponent.Instance.SendRT(this, buffer);
        }

        public void SendRT(byte cmd, byte[] buffer)
        {
            ServiceComponent.Instance.SendRT(this, cmd, buffer);
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
            ServiceComponent.Instance.Send(this, cmd, obj);
        }

        public void SendRT(byte cmd, object obj)
        {
            ServiceComponent.Instance.SendRT(this, cmd, obj);
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