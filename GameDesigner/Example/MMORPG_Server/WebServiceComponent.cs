#if SERVICE
using Net.Server;
using Net.Share;
using System;
using System.Collections.Generic;

namespace Net.Component.MMORPG_Server
{
    [Obsolete]
    public class WebServiceComponent : WebServer<PlayerComponent, SceneComponent>
    {
        protected override void OnAddPlayerToScene(PlayerComponent client)
        {
        }

        protected override bool OnWSUnClientRequest(PlayerComponent unClient, MessageModel model)
        {
            object[] pars = model.GetPars();
            switch (model.func)
            {
                case "Register":
                    Register(unClient, pars[0].ToString(), pars[1].ToString());
                    break;
                case "Login":
                    return Login(unClient, pars[0].ToString(), pars[1].ToString());
                default:
                    Send(unClient, "error", "尚未登录!");
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
                Send(unClient, "RegisterCallback", "账号已经存在!");
                return;
            }
            PlayerComponent p = new PlayerComponent();
            p.playerID = acc;
            p.data.account = acc;
            p.data.password = pwd;
            DBComponent.Instance.AddPlayer(p.data);
            DBComponent.Instance.Save(p.data);
            Send(unClient, "RegisterCallback", "注册成功！");
        }

        /// <summary>
        /// 当客户端发送的是登录方法的时候, 我们应该检查数据库是否有客户端所指定的账号, 如果有则判断 客户端输入的密码是否正确,
        /// 如果正确, 则提示登录成功, 并且返回玩家对象 , 返回的对象会添加到在线客户端字典列表里, 这是服务器内部管理的对象
        /// </summary>
        private bool Login(PlayerComponent unClient, string acc, string pwd)
        {
            if (!DBComponent.Instance.Contains(acc))
            {
                Send(unClient, "LoginCallback", false, "账号或密码错误!");
                return false;
            }
            PlayerData p = DBComponent.Instance[acc];
            if (p.password != pwd)
            {
                Send(unClient, "LoginCallback", false, "账号或密码错误!");
                return false;
            }
            Send(unClient, "LoginCallback", true, "登录成功!");
            return true;
        }

        /// <summary>
        /// 当开始调用 rpc标签的方法 时, 我们重写这个方法, 我们自己指定应该调用的方法, 这样会大大提高服务器效率
        /// </summary>
        /// <param name="client"></param>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        protected override void OnRpcExecute(PlayerComponent client, RPCModel model)
        {
            //即将反射调用rpc函数
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
                Send(player, "CreateRoomCallBack", false, "房间已经存在");
                return;
            }
            player.data.control = true;
            Send(player, "CreateRoomCallBack", true, "创建房间成功");
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
                Send(player, "JoinRoomCallBack", false, "加入房间失败, 房间不存在");
                return;
            }
            SceneComponent scene = Scenes[name];
            if (scene.state == NetState.InCombat)
            {
                Send(player, "JoinRoomCallBack", false, "此组队已经进入游戏!");
                return;
            }
            if (scene.SceneNumber >= scene.sceneCapacity)
            {
                Send(player, "JoinRoomCallBack", false, "房间人数已满!");
                return;
            }
            SceneComponent s = JoinScene(player, name, (scene1) =>
            {
                Multicast(scene.Players, "ExitRoom", player.data.account);
            });
            if (s == null)
            {
                Send(player, "JoinRoomCallBack", false, "加入房间失败, 房间不存在");
                return;
            }
            Send(player, "JoinRoomCallBack", true, "加入房间成功");
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
            Send(player, "SetRooms", array);
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
            Multicast(player.Scene.Players, "DismissRoom");
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
            Multicast(scene.Players, "StartGame");
        }

        void Ready(PlayerComponent player, bool isReady)
        {
            SceneComponent scene = player.Scene;
            if (scene == null)
            {
                Send(player, "error", "房间不存在!");
                return;
            }
            player.data.ready = isReady;
            List<JoinData> datas = new List<JoinData>();
            foreach (PlayerComponent p in scene.Players)
            {
                datas.Add(new JoinData() { name = p.playerID, ready = p.data.ready, teamTag = p.data.teamTag });
            }
            object array = datas.ToArray();
            Multicast(scene.Players, "ReadyCallback", array);
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
            Multicast(player.Scene.Players, "BackGame", player.playerID);
            if (scene.SceneNumber <= 1)
            {
                Scenes.TryRemove(player.sceneID, out SceneComponent _);
                player.Scene = null;
                player.sceneID = "";
                return;
            }
            ExitScene(player, false);
        }

        void ExitGame(PlayerComponent player)
        {
            BackGame(player);
            RemovePlayer(player);
        }
    }
}
#endif