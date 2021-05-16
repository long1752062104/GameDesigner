#if SERVICE
using Net.Server;
using Net.Share;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Net.Component.MMORPG_Server
{
    public class PlayerData : ISerializableData
    {
        public string account { get { return UIDKey; } set { UIDKey = value; } }
        public string password;
        public float moveSpeed = 5f;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 direction;
        public float health = 1000;
        public bool control;
        public string name;
        public bool teamTag;//true:A退伍 false:B退伍
        public bool ready;//准备?

        public string UIDKey { get; set; }
    }

    /// <summary>
    /// 服务器玩家组件, 这个组件处理 玩家移动操作, 后面增加攻击操作, 碰撞操作....
    /// </summary>
    public class PlayerComponent : WebPlayer, ISendHandle
    {
        public PlayerData data = new PlayerData();
        public new SceneComponent Scene;

        /// <summary>
        /// 客户端发送的网络数据, 玩家实体操作执行方法
        /// </summary>
        /// <param name="list"></param>
        public void ExecuteOpts(OperationList list)
        {
            foreach (Operation opt in list.operations)
            {
                OnOperationSync(opt);
            }
        }

        /// <summary>
        /// 当客户端上传操作
        /// </summary>
        /// <param name="opt"></param>
        public virtual void OnOperationSync(Operation opt)
        {
            switch (opt.cmd)
            {
                case Command.Input:
                    InputOpt(opt.direction);
                    break;
                case Command.Movement:
                    Movement(opt);
                    break;
                case Command.SyncHealth:
                    data.health = opt.health;
                    break;
                case NetCmd.QuitGame:
                    ExitSceneOrGameExit();
                    break;
                default:
                    if (Scene != null)
                        Scene.AddOperation(opt);
                    break;
            }
        }

        public virtual void Create()
        {
            List<Operation> opts = new List<Operation>();
            foreach (PlayerComponent p in Scene.Players)
            {
                opts.Add(new Operation(Command.CreatePlayer, p.playerID, p.data.direction, p.data.position, p.data.rotation));
            }
            Scene.AddOperations(opts);
        }

        public virtual void InputOpt(Vector3 direction)
        {
            data.direction = direction;
        }

        public virtual void Movement(Operation opt)
        {
            data.position = opt.position;
            data.rotation = opt.rotation;
            data.health = opt.health;
        }

        /// <summary>
        /// 服务器的玩家 每一帧更新. 处理物体移动
        /// </summary>
        /// <returns></returns>
        public override void OnUpdate()
        {
            Scene.AddOperation(new Operation(Command.Movement, playerID, data.direction, data.position, data.rotation) { health = data.health });
        }

        public virtual void ExitSceneOrGameExit()
        {
            Scene.AddOperation(new Operation()
            {
                cmd = NetCmd.QuitGame,
                name = playerID
            });
            ServiceComponent.Instance.RemovePlayer(this);
        }

        public override void OnRemoveClient()
        {
            if (Scene != null)
            {
                Scene.AddOperation(new Operation() { cmd = NetCmd.QuitGame, name = playerID });
                List<PlayerComponent> ps = Scene.GetPlayers();
                if (ps.Count > 0 & data.control)
                {
                    ServiceComponent.Instance.SendRT(ps[0], "SetControl", true);
                    ps[0].data.control = true;
                }
                data.control = false;
            }
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
#endif