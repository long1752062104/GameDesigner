#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Example2
{
    using Net.Component;
    using Net.Share;
    using Net.System;

    public class SceneManager : Net.Component.SceneManager
    {
        public AIMonster[] monsters;
        internal MyDictionary<int, AIMonster> monsterDics = new MyDictionary<int, AIMonster>();

        public override void OnCrateTransform(Operation opt, NetworkTransformBase t)
        {
            var p = t.GetComponent<Player>();
            if (p != null)
            {
                p.id = opt.index;
                GameManager.I.players.Add(p);
            }
        }

        public override void OnDestroyTransform(NetworkTransformBase t)
        {
            var p = t.GetComponent<Player>();
            if (p != null) 
            {
                Destroy(p.headBloodBar.gameObject);
                GameManager.I.players.Remove(p);
            }
        }

        public AIMonster CheckMonster(Operation opt)
        {
            if (!monsterDics.TryGetValue(opt.index, out AIMonster monster))
            {
                var mid = opt.buffer[0];
                var monster1 = monsters[mid];
                monster = Instantiate(monster1, opt.position, opt.rotation);
                monster.id = opt.index;
                monsterDics.Add(opt.index, monster);
                GameManager.I.enemys.Add(monster);
            }
            return monster;
        }

        public override void OnOperationOther(Operation opt)
        {
            switch (opt.cmd)
            {
                case Command.Fire:
                    {
                        if (transforms.TryGetValue(opt.index, out NetworkTransformBase t))
                        {
                            var p = t.GetComponent<Player>();
                            p.Fire();
                        }
                    }
                    break;
                case Command.AIMonster:
                    var monster = CheckMonster(opt);
                    monster.state = opt.cmd1;
                    monster.state1 = opt.cmd2;
                    monster.health = opt.index1;
                    monster.StatusEntry();
                    monster.transform.position = opt.position;
                    monster.transform.rotation = opt.rotation;
                    if (monster.health != monster.preHealth) 
                    {
                        monster.BeAttacked(null, monster.health - monster.preHealth);
                        monster.preHealth = monster.health;
                    }
                    break;
                case Command.EnemySync:
                    var monster1 = CheckMonster(opt);
                    monster1.state = opt.cmd1;
                    monster1.state1 = opt.cmd2;
                    monster1.health = opt.index1;
                    monster1.targetID = opt.index2;
                    if (monster1.targetID != ClientManager.UID)
                    {
                        monster1.transform.position = opt.position;
                        monster1.transform.rotation = opt.rotation;
                    }
                    if (monster1.health != monster1.preHealth)
                    {
                        monster1.BeAttacked(null, monster1.health - monster1.preHealth);
                        monster1.preHealth = monster1.health;
                    }
                    break;
                case Command.EnemySwitchState:
                    var monster2 = CheckMonster(opt);
                    monster2.state = opt.cmd1;
                    monster2.state1 = opt.cmd2;
                    monster2.StatusEntry();
                    break;
                case Command.PlayerState:
                    {
                        if (transforms.TryGetValue(opt.index, out NetworkTransformBase t))
                        {
                            var p = t.GetComponent<Player>();
                            p.health = opt.index1;
                            if (p.health != p.preHealth)
                            {
                                p.BeAttacked(null, p.health - p.preHealth);
                                p.preHealth = p.health;
                            }
                            p.Check();
                        }
                    }
                    break;
                case Command.Resurrection:
                    {
                        if (transforms.TryGetValue(opt.index, out NetworkTransformBase t))
                        {
                            var p = t.GetComponent<Player>();
                            p.Resurrection();
                        }
                    }
                    break;
            }
        }
    }
}
#endif