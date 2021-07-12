namespace Example2
{
    using Net.Event;
    using Net.Server;
    using Net.Share;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// 服务器组件,  (案例代码)
    /// </summary>
    public class ServiceComponent : TcpServer<PlayerComponent, SceneComponent>
    {
        /// <summary>
        /// 当开始服务器的时候
        /// </summary>
        protected override void OnStarting()
        {
            DBComponent.Instance.Load().Wait();
            NDebug.Log("数据库加载完成!");
            SetHeartTime(5, 300);//我们设置心跳检测时间, 时间越小检测越快, 跳检测时间也不能太小, 太小会直接判断为离线状态
        }

        protected override void OnStartupCompleted()
        {
            base.OnStartupCompleted();
            RemoveScene(MainSceneName, false);
#if !UNITY_EDITOR
            var path = AppDomain.CurrentDomain.BaseDirectory + "/Data/";
#else
            var path = UnityEngine.Application.dataPath + "/GameDesigner/Example/ExampleServer~/bin/Debug/Data/";
#endif
            var files = Directory.GetFiles(path, "*.sceneData");
            foreach (var flie in files)
            {
                var sceneData = SceneData.ReadData(flie);
                var scene = CreateScene(sceneData.name);
                scene.sceneData = sceneData;
                scene.Init();
                NDebug.Log("创建地图:" + scene.Name);
            }
            MainSceneName = "Battle";//指定你的主战斗场景名称, 根据unity的主战斗场景名称设置
            NDebug.Log("主地图名称:Battle");
        }

        /// <summary>
        /// 当添加默认场景的时候
        /// </summary>
        /// <returns></returns>
        protected override KeyValuePair<string, SceneComponent> OnAddDefaultScene()
        {
            //我们创建了一个名为 "主场景" 的一个场景对象, 并且可以容纳1000的场景
            return new KeyValuePair<string, SceneComponent>("主场景", new SceneComponent() { sceneCapacity = 1000 });
        }

        /// <summary>
        /// 当客户端登录成功时要添加到主场景时
        /// </summary>
        /// <param name="client"></param>
        protected override void OnAddPlayerToScene(PlayerComponent client)
        {
            base.OnAddPlayerToScene(client);//如果不允许登录成功加入主大厅场景, 注释这行代码即可
        }

        protected override void OnOperationSync(PlayerComponent client, OperationList list)
        {
            base.OnOperationSync(client, list);//当操作同步处理, 帧同步或状态同步通用
        }

        /// <summary>
        /// 当开始调用 rpc标签的方法 时, 我们重写这个方法, 我们自己指定应该调用的方法, 这样会大大提高服务器效率
        /// </summary>
        /// <param name="client"></param>
        /// <param name="model"></param>
        protected override void OnRpcExecute(PlayerComponent client, RPCModel model)
        {
            base.OnRpcExecute(client, model);//反射调用rpc
        }

        /// <summary>
        /// 当我们接收到客户端 刚连接服务器后, 发送的第一个请求时, 我们应该处理登录或注册方法
        /// </summary>
        protected override bool OnUnClientRequest(PlayerComponent unClient, RPCModel model)
        {
            //我们解析了客户端数据, 如果得到的是 注册 或 登录 这样的函数才进行处理
            switch (model.func)
            {
                case "Register":
                    Register(unClient, model.pars[0].ToString(), model.pars[1].ToString());
                    break;
                case "Login":
                    return Login(unClient, model.pars[0].ToString(), model.pars[1].ToString());
            }
            return false;
        }

        /// <summary>
        /// 当客户端注册的时候, 我们应该检查数据库 账号是否存在, 如果不存在, 则可以注册新的账号, 反则应该提示客户端注册失败
        /// </summary>
        private void Register(PlayerComponent unClient, string acc, string pwd)
        {
            if (DBComponent.Instance.HasAccout(acc))
            {
                SendRT(unClient, "RegisterCallback", "账号已经存在!");
                return;
            }
            PlayerComponent p = new PlayerComponent();
            p.playerID = acc;
            p.data.account = acc;
            p.data.password = pwd;
            DBComponent.Instance.AddPlayerAndSave(p.data);
            SendRT(unClient, "RegisterCallback", "注册成功！");
        }

        /// <summary>
        /// 当客户端发送的是登录方法的时候, 我们应该检查数据库是否有客户端所指定的账号, 如果有则判断 客户端输入的密码是否正确,
        /// 如果正确, 则提示登录成功, 并且返回玩家对象 , 返回的对象会添加到在线客户端字典列表里, 这是服务器内部管理的对象
        /// </summary>
        [Rpc(NetCmd.SafeCall)]
        private bool Login(PlayerComponent unClient, string acc, string pwd)
        {
            if (!DBComponent.Instance.HasAccout(acc))
            {
                SendRT(unClient, "LoginCallback", false, "账号或密码错误!");
                return false;
            }
            PlayerData data = DBComponent.Instance[acc];
            if (data.password != pwd)
            {
                SendRT(unClient, "LoginCallback", false, "账号或密码错误!");
                return false;
            }
            if (IsOnline(acc, out PlayerComponent player))
            {
                SendRT(player, "BackLogin", "你的账号在其他地方被登录!");//在客户端热更新工程的MsgPanel类找到
                SignOut(player);
            }
            unClient.playerID = acc;
            unClient.data = data;
            SendRT(unClient, "LoginCallback", true, "登录成功!");
            return true;
        }

        protected override void OnRemoveClient(PlayerComponent client)
        {
            base.OnRemoveClient(client);//当客户端断开连接处理
        }
    }
}