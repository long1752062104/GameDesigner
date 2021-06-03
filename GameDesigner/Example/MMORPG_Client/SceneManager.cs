#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.MMORPG_Client
{
    using Net.Client;
    using Net.Component.Client;
    using Net.Share;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 场景管理组件, 这个组件负责 同步玩家操作, 玩家退出游戏移除物体对象, 怪物网络行为同步, 攻击同步等
    /// </summary>
    public class SceneManager : NetBehaviour
    {
        public PlayerComponent prefab;
        public EnemyComponent prefab1;
        public TransformComponent demo;
        public ComponentSync demo1;
        public Dictionary<string, PlayerComponent> players = new Dictionary<string, PlayerComponent>();
        public Dictionary<string, EnemyComponent> enemys = new Dictionary<string, EnemyComponent>();
        public Dictionary<int, TransformComponent> transforms = new Dictionary<int, TransformComponent>();
        public Dictionary<string, ComponentSync> componentSyncs = new Dictionary<string, ComponentSync>();

        void Start()
        {
            ClientManager.Instance.client.OnOperationSync += OnOperationSync;
            if (ClientManager.Instance.control)
            {
                InvokeRepeating("InstantiateEnemy", 3, 3);
            }
        }

        /// <summary>
        /// 当网络操作同步时调用
        /// </summary>
        /// <param name="list"></param>
        public void OnOperationSync(OperationList list)
        {
            foreach (var opt in list.operations)
            {
                switch (opt.cmd)
                {
                    case Command.CreatePlayer:
                        CreatePlayer(opt);
                        break;
                    case Command.Input:
                        InputOpt(opt);
                        break;
                    case Command.Movement:
                        Movement(opt);
                        break;
                    case Command.Attack:
                        AttackOpt(opt);
                        break;
                    case NetCmd.QuitGame:
                        PlayerQuit(opt);
                        break;
                    case Command.EnemySync:
                        EnemySync(opt);
                        break;
                    case Command.SwitchState:
                        SwitchState(opt);
                        break;
                    case Command.EnemySwitchState:
                        EnemySwitchState(opt);
                        break;
                    case Command.Transform:
                        TransformSync(opt);
                        break;
                    case Command.Destroy:
                        if (transforms.ContainsKey(opt.index))
                        {
                            if (!ClientManager.Instance.control)
                                Destroy(transforms[opt.index].gameObject);
                            transforms.Remove(opt.index);
                        }
                        if (string.IsNullOrEmpty(opt.name))
                            continue;
                        if (componentSyncs.ContainsKey(opt.name))
                        {
                            if (!ClientManager.Instance.control)
                                Destroy(componentSyncs[opt.name].gameObject);
                            componentSyncs.Remove(opt.name);
                        }
                        break;
                    case Command.PropertySync:
                        ComponentSync(opt);
                        break;
                }
            }
        }

        void ComponentSync(Operation opt)
        {
            var func = opt.GetData();
            if (!componentSyncs.ContainsKey(func.name))
            {
                var t = Instantiate(demo1, opt.position, opt.rotation);
                t.name = func.name;
                t.component = t.GetComponent(NetConvertOld.GetType(func[0].ToString()));
                t.propertySyncs = (List<PropertySync>)func[1];
                componentSyncs.Add(func.name, t);
            }
            if (ClientManager.Instance.control)
                return;
            var p = componentSyncs[func.name];
            p.SetPropertySync((List<PropertySync>)func[1]);
        }

        void TransformSync(Operation opt)
        {
            if (!transforms.TryGetValue(opt.index, out TransformComponent t))
            {
                t = Instantiate(demo, opt.position, opt.rotation);
                SyncMode mode = (SyncMode)opt.cmd1;
                if(mode == SyncMode.Control)
                    t.syncMode = SyncMode.SynchronizedAll;
                else
                    t.syncMode = SyncMode.Synchronized;
                t.identity = opt.index;
                transforms.Add(opt.index, t);
                TransformComponent.Identity++;
            }
            if (ClientManager.UID == opt.index1)
                return;
            t.sendTime = Time.time + t.interval;
            t.transform.position = opt.position;
            t.transform.rotation = opt.rotation;
            t.transform.localScale = opt.direction;
        }

        void EnemySwitchState(Operation opt)
        {
            if (!enemys.ContainsKey(opt.name))
                return;
            var p = enemys[opt.name];
            p.stateManager.StatusEntry(opt.index);
        }

        void SwitchState(Operation opt)
        {
            if (!players.ContainsKey(opt.name))
                return;
            var p = players[opt.name];
            p.stateManager.StatusEntry(opt.index);
        }

        void OnDestroy()
        {
            if (ClientManager.Instance == null)
                return;
            ClientManager.Instance.client.OnOperationSync -= OnOperationSync;
        }

        public void CreatePlayer(Operation opt)
        {
            if (players.ContainsKey(opt.name) | prefab == null)
                return;
            var p = Instantiate(prefab, opt.position, opt.rotation);
            p.name = opt.name;
            if (p.name == ClientBase.Instance.Identify)
            {
                var cam = FindObjectOfType<ARPGcamera>();
                if (cam == null)
                    cam = UnityEngine.Camera.main.gameObject.AddComponent<ARPGcamera>();
                cam.target = p.transform;
            }
            players.Add(opt.name, p);
        }

        public void InputOpt(Operation opt)
        {
            if (!players.ContainsKey(opt.name))
            {
                CreatePlayer(opt);
                return;
            }
            var p = players[opt.name];
            p.direction = opt.direction;
        }

        public void PlayerQuit(Operation opt)
        {
            if (!players.ContainsKey(opt.name))
                return;
            Destroy(players[opt.name].gameObject);
            players.Remove(opt.name);
        }

        internal void AttackOpt(Operation opt)
        {
            if (!players.ContainsKey(opt.name))
                return;
            players[opt.name].Attack(opt.index);
        }

        internal void Movement(Operation opt)
        {
            if (!players.ContainsKey(opt.name))
            {
                CreatePlayer(opt);
                return;
            }
            var p = players[opt.name];
            p.position = opt.position;
            p.rotation = opt.rotation;
            p.direction = opt.direction;
            if (!ClientManager.Instance.control)
                p.hp = opt.health;
            if (p.hp <= 0 & !p.isDead)
            {
                p.OnDead();
            }
        }

        private void InstantiateEnemy()
        {
            if (enemys.Count > 30 | prefab1 == null)
                return;
            var eny = Instantiate(prefab1);
            eny.transform.position = new UnityEngine.Vector3(RandomHelper.Range(-20, 20), 0, RandomHelper.Range(-20, 20));
            eny.transform.eulerAngles = new UnityEngine.Vector3(0, RandomHelper.Range(-360, 360), 0);
        JUMP: eny.name = RandomHelper.Range(100000, 999999).ToString();
            if (enemys.ContainsKey(eny.name))
                goto JUMP;
            enemys.Add(eny.name, eny);
        }

        public void CreateEnemy(Operation opt)
        {
            if (enemys.ContainsKey(opt.name))
                return;
            var p = Instantiate(prefab1, opt.position, opt.rotation);
            p.name = opt.name;
            enemys.Add(opt.name, p);
        }

        public void EnemySync(Operation opt)
        {
            if (!enemys.ContainsKey(opt.name))
            {
                CreateEnemy(opt);
                return;
            }
            var p = enemys[opt.name];
            p.position = opt.position;
            p.rotation = opt.rotation;
            if (!ClientManager.Instance.control)
                p.hp = opt.health;
            if (p.hp <= 0 & !p.isDead)
            {
                p.OnDead();
            }
        }
    }
}
#endif