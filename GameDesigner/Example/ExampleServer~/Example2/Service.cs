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
    public class Service : TcpServer<Player, Scene>
    {
        /// <summary>
        /// 当开始服务器的时候
        /// </summary>
        protected override void OnStarting()
        {
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
        protected override KeyValuePair<string, Scene> OnAddDefaultScene()
        {
            //我们创建了一个名为 "主场景" 的一个场景对象, 并且可以容纳1000的场景
            return new KeyValuePair<string, Scene>("主场景", new Scene() { sceneCapacity = 1000 });
        }

        /// <summary>
        /// 当客户端登录成功时要添加到主场景时
        /// </summary>
        /// <param name="client"></param>
        protected override void OnAddPlayerToScene(Player client)
        {
            base.OnAddPlayerToScene(client);//如果不允许登录成功加入主大厅场景, 注释这行代码即可
        }

        protected override void OnOperationSync(Player client, OperationList list)
        {
            base.OnOperationSync(client, list);//当操作同步处理, 帧同步或状态同步通用
        }

        /// <summary>
        /// 当开始调用 rpc标签的方法 时, 我们重写这个方法, 我们自己指定应该调用的方法, 这样会大大提高服务器效率
        /// </summary>
        /// <param name="client"></param>
        /// <param name="model"></param>
        protected override void OnRpcExecute(Player client, RPCModel model)
        {
            base.OnRpcExecute(client, model);//反射调用rpc
        }

        /// <summary>
        /// 当我们接收到客户端 刚连接服务器后, 发送的第一个请求时, 我们应该处理登录或注册方法
        /// </summary>
        protected override bool OnUnClientRequest(Player unClient, RPCModel model)
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
        private void Register(Player unClient, string acc, string pwd)
        {
            Example2DB.I.Invoke(()=>
            {
                if (Example2DB.I.UserinfoDatas.TryGetValue(acc, out var data))
                {
                    SendRT(unClient, "RegisterCallback", "账号已经存在!");
                    return;
                }
                data = new UserinfoData();
                int id = Example2DB.I.UserinfoTable.Rows.Count + 1;
                object buffer = new byte[] { 1,2,3, (byte)id };
                data.Row = Example2DB.I.AddUserinfoNewRow(id, acc, pwd, null, null, null, 100, 100, buffer);
                data.Init(data.Row);
                Example2DB.I.UserinfoDatas.TryAdd(acc, data);
                SendRT(unClient, "RegisterCallback", "注册成功！");
            });
        }

        /// <summary>
        /// 当客户端发送的是登录方法的时候, 我们应该检查数据库是否有客户端所指定的账号, 如果有则判断 客户端输入的密码是否正确,
        /// 如果正确, 则提示登录成功, 并且返回玩家对象 , 返回的对象会添加到在线客户端字典列表里, 这是服务器内部管理的对象
        /// </summary>
        [Rpc(NetCmd.SafeCall)]
        private bool Login(Player unClient, string acc, string pwd)
        {
            if (!Example2DB.I.UserinfoDatas.TryGetValue(acc, out var data))
            {
                SendRT(unClient, "LoginCallback", false, "账号或密码错误!");
                return false;
            }
            if (data.Password != pwd)
            {
                SendRT(unClient, "LoginCallback", false, "账号或密码错误!");
                return false;
            }
            if (IsOnline(acc, out Player player))
            {
                SendRT(player, "BackLogin", "你的账号在其他地方被登录!");//在客户端热更新工程的MsgPanel类找到
                SignOut(player);
            }
            unClient.PlayerID = acc;
            unClient.data = data;
            SendRT(unClient, "LoginCallback", true, "登录成功!");
            return true;
        }

        /// <summary>
        /// 客户端主动发起退出登录请求
        /// </summary>
        /// <param name="client"></param>
        [Rpc(NetCmd.SafeCall)]
        private void LogOut(Player client) 
        {
            SendRT(client, "LogOut");//在客户端热更新工程的MsgPanel类找到
            SignOut(client);
        }

        protected override void OnRemoveClient(Player client)
        {
            base.OnRemoveClient(client);//当客户端断开连接处理
        }
    }
}