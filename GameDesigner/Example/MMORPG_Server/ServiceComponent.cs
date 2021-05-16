#if SERVICE
namespace Net.Component.MMORPG_Server
{
    using Net.Client;
    using Net.Event;
    using Net.Server;
    using Net.Share;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 服务器组件, 可以分写或参考  (案例代码)
    /// </summary>
    public partial class ServiceComponent : UdpServer<PlayerComponent, SceneComponent>
    {
        /// <summary>
        /// 集群分布式服务器的客户端连接对象
        /// </summary>
        private UdpClient serverCluster;

        /// <summary>
        /// 当开始服务器的时候
        /// </summary>
        protected override void OnStarting()
        {
            Console.WriteLine("是否连接主服务器(网关服务器)! 连接[Y] 不连接[N]");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.Y)
            {
                Console.WriteLine("请输入主服务器IP和端口, 如: 127.0.0.1:6666");
                string ip_port_str = Console.ReadLine();
                string ip = ip_port_str.Split(':')[0];
                int prot = int.Parse(ip_port_str.Split(':')[1]);
                serverCluster = new UdpClient();//创建分布式服务器的客户端对象，用来连接主服务器，当前的这个服务器是分布式服务器的组成部分
                serverCluster.OnConnectedHandle += OnConnectedHandle;
                serverCluster.Connect(ip, prot).Wait();//连接主服务器，如果连接失败，这个分布式服务器将不允许启动
                if (!serverCluster.Connected)
                    throw new Exception("主服务器连接失败！无法开启分布式服务器");
                NDebug.Log($"[{AreaName} - {Name}] 分布式服务器启动成功!");
            }
            SetHeartTime(5, 1000);//我们设置心跳检测时间, 时间越小检测越快, 跳检测时间也不能太小, 太小会直接判断为离线状态
        }

        /// <summary>
        /// 当分布式客户端对象连接主服务器成功被调用
        /// </summary>
        void OnConnectedHandle()
        {
            serverCluster.SendRT("ServerCluster", "6a5c156a4d86f19b87d674a653c");//使用密码申请进入服务器
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
            //我们不允许登录成功加入主大厅场景, 直接不写代码就可以
            if (Scenes[MainSceneName].Players.Count > 0)
            {
                client.data.control = false;
                SendRT(client, "SetControl", false);
            }
            else
            {
                client.data.control = true;
                SendRT(client, "SetControl", true);
            }
            base.OnAddPlayerToScene(client);
        }

        protected override void OnOperationSync(PlayerComponent client, OperationList list)
        {
            if (client.Scene == null)
                return;
            foreach (Operation opt in list.operations)
                client.OnOperationSync(opt);
        }

        /// <summary>
        /// 当开始调用 rpc标签的方法 时, 我们重写这个方法, 我们自己指定应该调用的方法, 这样会大大提高服务器效率
        /// </summary>
        /// <param name="client"></param>
        /// <param name="model"></param>
        protected override void OnRpcExecute(PlayerComponent client, RPCModel model)
        {
            switch (model.func)
            {
                case "CreateRoom":
                    CreateRoom(client, model.pars[0].ToString(), (int)model.pars[1]);
                    break;
                case "JoinRoom":
                    JoinRoom(client, model.pars[0].ToString());
                    break;
                case "GetRooms":
                    GetRooms(client);
                    break;
                case "ExitRoom":
                    ExitRoom(client);
                    break;
                case "DismissRoom":
                    DismissRoom(client);
                    break;
                case "StartGame":
                    StartGame(client);
                    break;
                case "BackGame":
                    BackGame(client);
                    break;
                case "ExitGame":
                    ExitGame(client);
                    break;
                case "Ready":
                    Ready(client, (bool)model.pars[0]);
                    break;
                case "GetRoomData":
                    GetRoomData(client);
                    break;
                case "SelectTeam":
                    SelectTeam(client, (bool)model.pars[0]);
                    break;
                default:
                    base.OnRpcExecute(client, model);//反射调用rpc
                    break;
            }
        }

        /// <summary>
        /// 当我们接收到客户端 刚连接服务器后, 发送的第一个请求时, 我们应该处理登录或注册方法
        /// </summary>
        protected override bool OnUnClientRequest(PlayerComponent unClient, RPCModel model)
        {
            //return unClient;

            //我们解析了客户端数据, 如果得到的是 注册 或 登录 这样的函数才进行处理
            switch (model.func)
            {
                case "Register":
                    Register(unClient, model.pars[0].ToString(), model.pars[1].ToString());
                    break;
                case "Login":
                    return Login(unClient, model.pars[0].ToString(), model.pars[1].ToString());
                case "ServerCluster"://分布式服务器链接通道
                    if (model.pars[0].ToString() == "6a5c156a4d86f19b87d674a653c")//分布式服务器密码
                        return true;
                    break;
            }
            return false;
        }

        /// <summary>
        /// 当客户端注册的时候, 我们应该检查数据库 账号是否存在, 如果不存在, 则可以注册新的账号, 反则应该提示客户端注册失败
        /// </summary>
        private void Register(PlayerComponent unClient, string acc, string pwd)
        {
            if (DBComponent.Instance.Contains(acc))
            {
                SendRT(unClient, "RegisterCallback", "账号已经存在!");
                return;
            }
            PlayerComponent p = new PlayerComponent();
            p.playerID = acc;
            p.data.account = acc;
            p.data.password = pwd;
            DBComponent.Instance.AddPlayer(p.data);
            DBComponent.Instance.Save(p.data);
            SendRT(unClient, "RegisterCallback", "注册成功！");
        }

        /// <summary>
        /// 当客户端发送的是登录方法的时候, 我们应该检查数据库是否有客户端所指定的账号, 如果有则判断 客户端输入的密码是否正确,
        /// 如果正确, 则提示登录成功, 并且返回玩家对象 , 返回的对象会添加到在线客户端字典列表里, 这是服务器内部管理的对象
        /// </summary>
        private bool Login(PlayerComponent unClient, string acc, string pwd)
        {
            if (!DBComponent.Instance.Contains(acc))
            {
                SendRT(unClient, "LoginCallback", false, "账号或密码错误!");
                return false;
            }
            PlayerData p = DBComponent.Instance[acc];
            if (p.password != pwd)
            {
                SendRT(unClient, "LoginCallback", false, "账号或密码错误!");
                return false;
            }
            SendRT(unClient, "LoginCallback", true, "登录成功!");
            return true;
        }

        /// <summary>
        /// 当客户端请求的是创建房间, 那么我们应该判断 客户端想要创建的房间名称是否已存在, 如果存在, 我们应该给客户端提示房间已存在
        /// 如果房间不存在, 我们可以创建出一个新的房间, 然后加入到所创建的房间里面, 并且给客户端发送一个创建房间成功的消息提示
        /// </summary>

        //[Rpc(NetCmd.SafeCall)]
        void CreateRoom(PlayerComponent player, string name, int num)
        {
            SceneComponent scene = new SceneComponent();
            scene.Name = name;
            scene.sceneCapacity = num;
            SceneComponent s = CreateScene(player, name, scene);
            if (s == null)
            {
                SendRT(player, "CreateRoomCallBack", false, "房间已经存在");
                return;
            }
            player.data.control = true;
            SendRT(player, "CreateRoomCallBack", true, "创建房间成功");
        }

        /// <summary>
        /// 当客户端请求的是加入房间, 那么我们检查一下, 所要加入的房间人数是否已经满, 或者房间已经进入战斗状态了, 如果是则提示进入失败
        /// 反则加入场景, 在加入场景的时候, 我们也会判断客户端的代理, 如果代理不存在, 我们可以让这个客户端进行代理
        /// </summary>

        //[Rpc(NetCmd.SafeCall)]
        void JoinRoom(PlayerComponent player, string name)
        {
            if (!Scenes.ContainsKey(name))
            {
                SendRT(player, "JoinRoomCallBack", false, "加入房间失败, 房间不存在");
                return;
            }
            SceneComponent scene = Scenes[name];
            if (scene.state == NetState.InCombat)
            {
                SendRT(player, "JoinRoomCallBack", false, "此组队已经进入游戏!");
                return;
            }
            if (scene.SceneNumber >= scene.sceneCapacity)
            {
                SendRT(player, "JoinRoomCallBack", false, "房间人数已满!");
                return;
            }
            SceneComponent s = JoinScene(player, name, (scene1) =>
            {
                Multicast(scene.Players, true, "ExitRoom", player.data.account);
            });
            if (s == null)
            {
                SendRT(player, "JoinRoomCallBack", false, "加入房间失败, 房间不存在");
                return;
            }
            SendRT(player, "JoinRoomCallBack", true, "加入房间成功");
            //List<string> ps = new List<string>();
            //foreach (var p in s.Players) {
            //    ps.Add(p.account);
            //}
            //object array = ps;
            //Multicast(s.Players, "PlayerJoinRoom", array);
            GetRoomData(player);
        }

        /// <summary>
        /// 当客户端请求的是获得房间信息, 那么我们应该发送所有房间的信息给客户端, 然后客户端显示出来
        /// </summary>

        //[Rpc(NetCmd.SafeCall)]
        void GetRooms(PlayerComponent player)
        {
            List<RoomData> data = new List<RoomData>();
            foreach (KeyValuePair<string, SceneComponent> scene in Scenes)
            {
                data.Add(new RoomData()
                {
                    name = scene.Key,
                    num = scene.Value.sceneCapacity,
                    currNum = scene.Value.SceneNumber,
                    state = scene.Value.state
                });
            }
            object array = data;
            SendRT(player, "SetRooms", array);
        }

        /// <summary>
        /// 当客户端请求退出房间, 那么我们可以把这个客户端从房间里面移除掉
        /// </summary>

        //[Rpc(NetCmd.SafeCall)]
        void ExitRoom(PlayerComponent player)
        {
            BackGame(player);
        }

        /// <summary>
        /// 当客户端请求解散房间, 那么我们发送解散指令数据给在这个房间内的所有客户端, 让他们同步解散房间, 并且服务器也把这个房间给移除掉
        /// </summary>

        //[Rpc(NetCmd.SafeCall)]
        void DismissRoom(PlayerComponent player)
        {
            SceneComponent scene = player.Scene;
            if (scene == null)
                return;
            Multicast(player.Scene.Players, true, "DismissRoom");
            Scenes.TryRemove(player.sceneID, out SceneComponent _);
        }

        /// <summary>
        /// 当客户端请求 开始游戏的时候, 也就是进入战斗时候, 我们发送 开始游戏 给其他客户端 也同步进入战斗场景
        /// </summary>
        /// <param name="player"></param>

        //[Rpc(NetCmd.SafeCall)]
        void StartGame(PlayerComponent player)
        {
            SceneComponent scene = player.Scene;
            if (scene == null)
                return;
            scene.state = NetState.InCombat;
            Multicast(scene.Players, true, "StartGame");
        }

        void Ready(PlayerComponent player, bool isReady)
        {
            SceneComponent scene = player.Scene;
            if (scene == null)
                return;
            player.data.ready = isReady;
            List<JoinData> datas = new List<JoinData>();
            foreach (PlayerComponent p in scene.Players)
            {
                datas.Add(new JoinData() { name = p.playerID, ready = p.data.ready, teamTag = p.data.teamTag });
            }
            object array = datas;
            Multicast(scene.Players, true, "ReadyCallback", array);
            if (scene.Players.Count < scene.sceneCapacity)
            {
                return;
            }
            int num = 0;
            int a = 0;
            int b = 0;
            foreach (PlayerComponent p in scene.Players)
            {
                if (p.data.ready)
                    num++;
                if (p.data.teamTag)
                    a++;
                else
                    b++;
            }
            if (a <= 0 | b <= 0)
                return;
            Multicast(scene.Players, true, "StartGame");
        }

        void GetRoomData(PlayerComponent player)
        {
            SceneComponent scene = player.Scene;
            if (scene == null)
                return;
            List<JoinData> datas = new List<JoinData>();
            foreach (PlayerComponent p in scene.Players)
            {
                datas.Add(new JoinData() { name = p.playerID, ready = p.data.ready, teamTag = p.data.teamTag });
            }
            object array = datas;
            Multicast(scene.Players, true, "ReadyCallback", array);
        }

        void SelectTeam(PlayerComponent player, bool teamTag)
        {
            player.data.teamTag = teamTag;
            GetRoomData(player);
        }

        /// <summary>
        /// 当客户端在战斗场景的时候, 发送请求退出场景时, 我们应该通知创建内的其他玩家, 移除掉这个玩家的游戏对象,
        /// 然后这个客户端退出战斗场景
        /// </summary>

        //[Rpc(NetCmd.SafeCall)]
        void BackGame(PlayerComponent player)
        {
            player.data.ready = false;
            player.data.teamTag = true;
            SceneComponent scene = player.Scene;
            if (scene == null)
                return;
            Multicast(player.Scene.Players, true, "BackGame", player.playerID);
            if (scene.SceneNumber <= 1)
            {
                Scenes.TryRemove(player.sceneID, out SceneComponent _);
                player.Scene = null;
                player.sceneID = "";
                return;
            }
            ExitScene(player, false);
            if (player.data.control)
            {
                List<PlayerComponent> ps = scene.GetPlayers();
                if (ps.Count > 0)
                    SendRT(ps[0], "SetControl", true);
                player.data.control = false;
            }
        }

        void ExitGame(PlayerComponent player)
        {
            BackGame(player);
            RemovePlayer(player);
        }

        protected override void OnRemoveClient(PlayerComponent client)
        {
            if (serverCluster != null)
                serverCluster.SendRT("UpdateArea", Name, OnlinePlayers);//当集群子服务器的客户端退出游戏后， 要告诉主服务器， 让主服务器更新这个子服务器的当前在线人数
        }
    }
}
#endif