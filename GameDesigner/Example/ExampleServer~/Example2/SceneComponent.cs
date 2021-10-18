using ECS;
using Net;
using Net.Component;
using Net.Server;
using Net.Share;

namespace Example2
{
    /// <summary>
    /// 场景管理器, 状态同步, 帧同步 固定帧发送当前场景的所有玩家操作
    /// </summary>
    public class SceneComponent : NetScene<PlayerComponent>
    {
        internal SceneData sceneData = new SceneData();
        internal readonly MyDictionary<int, AIMonster> monsters = new MyDictionary<int, AIMonster>();
        internal GSystem ecsSystem = new GSystem();

        public void Init()
        {
            int id = 1;
            foreach (var item in sceneData.monsterPoints)
            {
                RoamingPath1 roamingPath = item.roamingPath;
                for (int i = 0; i < item.monsterIDs.Length; i++)
                {
                    var point = roamingPath.waypointsList[RandomHelper.Range(0, roamingPath.waypointsList.Count)];
                    var monster1 = ecsSystem.Create<Entity>().AddComponent<AIMonster>();
                    monster1.transform = new NTransform();
                    monster1.transform.position = point;
                    monster1.transform.rotation = Quaternion.identity;
                    monster1.roamingPath = roamingPath;
                    monster1.scene = this;
                    monster1.id = id++;
                    monster1.mid = item.monsterIDs[i];
                    monsters.Add(monster1.id, monster1);
                }
            }
        }

        public override void OnEnter(PlayerComponent client)
        {
            client.Scene = this;
        }

        /// <summary>
        /// 网络帧同步, 状态同步更新
        /// </summary>
        public override void Update(IServerSendHandle<PlayerComponent> handle, byte cmd = 19)
        {
            var players = Clients;
            int playerCount = players.Count;
            if (playerCount <= 0)
                return;
            for (int i = 0; i < players.Count; i++)
                players[i].OnUpdate();
            ecsSystem.Update();
            frame++;
            int count = operations.Count;
            if (count > 0)
            {
                while (count > Split)
                {
                    OnPacket(handle, cmd, Split);
                    count -= Split;
                }
                if (count > 0)
                    OnPacket(handle, cmd, count);
            }
            Event.UpdateEvent();
        }

        public override void OnOperationSync(PlayerComponent client, OperationList list)
        {
            for (int i = 0; i < list.operations.Length; i++)
            {
                var opt = list.operations[i];
                switch (opt.cmd)
                {
                    case Command.Attack:
                        if (monsters.TryGetValue(opt.index, out AIMonster monster))
                        {
                            monster.targetID = client.UserID;
                            monster.OnDamage(opt.index1);
                            if (monster.isDeath)
                                monster.PatrolCall();
                        }
                        break;
                    case Command.AttackPlayer:
                        var players = Clients;
                        for (int n = 0; n < players.Count; n++)
                        {
                            if (players[n].UserID == opt.index) 
                            {
                                players[n].BeAttacked(opt.index1);
                                break;
                            }
                        }
                        break;
                    case Command.EnemySync:
                        if (monsters.TryGetValue(opt.index, out AIMonster monster1))
                        {
                            monster1.transform.position = opt.position;
                            monster1.transform.rotation = opt.rotation;
                        }
                        break;
                    case Command.EnemySwitchState:
                        if (monsters.TryGetValue(opt.index, out AIMonster monster3))
                        {
                            if (!monster3.isDeath)
                            {
                                monster3.state = opt.cmd1;
                                monster3.state1 = opt.cmd2;
                                if (monster3.state == 0)
                                    monster3.targetID = 0;
                                AddOperation(opt);
                            }
                            else monster3.PatrolCall();
                        }
                        break;
                    case Command.AIAttack:
                        client.BeAttacked(opt.index);
                        break;
                    case Command.Resurrection:
                        client.Resurrection();
                        AddOperation(opt);
                        break;
                    default:
                        AddOperation(opt);
                        break;
                }
            }
        }

        public override void OnExit(PlayerComponent client)
        {
            foreach (var monster in monsters.Values)
            {
                if (monster.targetID == client.UserID)
                {
                    monster.targetID = 0;
                    monster.state = 0;
                    monster.state1 = 0;
                }
            }
            AddOperation(new Operation(Command.Destroy) { index = client.UserID });
        }
    }
}