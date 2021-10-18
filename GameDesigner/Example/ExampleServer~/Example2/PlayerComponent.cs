using Net;
using Net.Component;
using Net.Server;
using Net.Share;
using System;
using System.Data;
using System.Threading;

namespace Example2
{
    public class PlayerData : ISerializableData
    {
        public string UIDKey { get; set; }
        public string account { get => UIDKey; set => UIDKey = value; }
        public long StreamPosition { get; set; }
        public DataRow Row { get; set; }

        public string password;
        public float moveSpeed = 5f;
        public Vector3 position;
        public Quaternion rotation;
        public float health = 100;
        public float healthMax = 100;
    }

    /// <summary>
    /// 服务器玩家组件, 这个组件处理 玩家移动操作, 后面增加攻击操作, 碰撞操作....
    /// </summary>
    public class PlayerComponent : WebPlayer, ISendHandle
    {
        public PlayerData data = new PlayerData();
        internal NTransform transform = new NTransform();
        internal bool isDead;
        internal SceneComponent scene;

        public override void OnEnter()
        {
            data.health = 100;
            isDead = false;
            scene = Scene as SceneComponent;
        }

        public override void OnUpdate()
        {
            scene.AddOperation(new Operation(Command.PlayerState, UserID) {
               index1 = (int)data.health
            });
        }

        internal void BeAttacked(int damage)
        {
            if (isDead)
                return;
            data.health -= damage;
            if (data.health <= 0)
            {
                isDead = true;
                data.health = 0;
            }
        }

        public void Resurrection()
        {
            data.health = data.healthMax;
            isDead = false;
        }

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