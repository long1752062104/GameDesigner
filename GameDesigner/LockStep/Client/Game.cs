#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
using Net.Component;
using Net.Component.Client;
using Net.Share;
using System.Collections.Generic;
using System.Reflection;
using TrueSync;
using UnityEngine;

namespace LockStep.Client
{
    public class Game : SingleCase<Game>
    {
        private int frame;
        private readonly List<OperationList> snapshots=new List<OperationList>();
        private int logicFrame;
        public GameSystem gameSystem = new GameSystem();

        public GameObject @object;
        public GameObject enemyObj;
        public Net.Vector3 direction;

        // Use this for initialization
        void Start()
        {
            gameSystem.OnCreate += (opt) => 
            {
                Player actor = new Player(gameSystem)
                {
                    name = opt.name,
                    gameObject = Instantiate(@object)
                };
                actor.objectView = actor.gameObject.GetComponent<ObjectView>();
                actor.objectView.actor = actor;
                actor.transform = actor.gameObject.GetComponent<TSTransform>();
                actor.gameSystem.updates.Remove(actor);
                return actor;
            };
            ClientManager.Instance.client.OnOperationSync += OnOperationSync;

            GameMgr gameMgr = new GameMgr(gameSystem);
        }

        private void OnOperationSync(OperationList list)
        {
            if (frame != list.frame)
                return;
            frame++;
            snapshots.Add(list);
            ClientManager.AddOperation(new Operation(Command.Input, ClientManager.Identify, direction));
        }

        // Update is called once per frame
        void Update()
        {
            direction = new Net.Vector3(Input.GetAxis("Horizontal").ToFloat(100), 0f, Input.GetAxis("Vertical").ToFloat(100));
            int forLogic = frame - logicFrame;
            for (int i = 0; i < forLogic; i++)
            {
                if (logicFrame >= snapshots.Count - 2)
                    return;
                var list = snapshots[logicFrame];
                logicFrame++;
                gameSystem.Run(list);
            }
        }

        private void OnDestroy()
        {
            ClientManager.Instance.client.OnOperationSync -= OnOperationSync;
        }
    }
}
#endif