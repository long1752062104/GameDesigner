#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || SERVICE
using GGPhysUnity;
using Net.Component;
using Net.Share;
using System;
using System.Collections.Generic;

namespace LockStep.Client
{
    [Serializable]
    public class GameSystem
    {
        public List<Actor> starts = new List<Actor>();
        public List<Actor> fixedUpdates = new List<Actor>();
        public List<Actor> updates = new List<Actor>();
        public List<Actor> lateUpdates = new List<Actor>();
        public List<Actor> guis = new List<Actor>();
        public List<Actor> destroys = new List<Actor>();
        public List<Actor> playersView = new List<Actor>();//unity视图观察
        public Dictionary<string, Actor> players = new Dictionary<string, Actor>();

        public Func<Operation, Actor> OnCreate;

        public void Run(OperationList list)
        {
            LSTime.time += LSTime.deltaTime;//最先执行的时间,逻辑时间
            for (int i = 0; i < list.operations.Length; i++)
            {
                Operation opt = list.operations[i];
                switch (opt.cmd)
                {
                    case Command.Input:
                        if (!players.TryGetValue(opt.name, out Actor actor))
                        {
                            actor = OnCreate(opt);
                            actor.name = opt.name;
                            players.Add(opt.name, actor);
                            playersView.Add(actor);
                        }
                        actor.Update(opt);
                        break;
                    case NetCmd.QuitGame:
                        if (players.TryGetValue(opt.name, out Actor actor1))
                        {
                            actor1.Destroy();
                        }
                        break;
                }
            }
            for (int i = 0; i < starts.Count; i++)
            {
                starts[i].Start();
            }
            for (int i = 0; i < fixedUpdates.Count; i++)
            {
                fixedUpdates[i].FixedUpdate();
            }
            for (int i = 0; i < updates.Count; i++)
            {
                updates[i].Update(null);
            }
            for (int i = 0; i < lateUpdates.Count; i++)
            {
                lateUpdates[i].LateUpdate();
            }
            for (int i = 0; i < guis.Count; i++)
            {
                guis[i].OnGUI();
            }
            RigidPhysicsEngine.Instance.RunPhysics(0.02f);
            EventSystem.UpdateEvent();//事件帧同步更新
            for (int i = 0; i < destroys.Count; i++)
            {
                destroys[i].OnDisable();
                destroys[i].OnDestroy();
                UnityEngine.Object.DestroyImmediate(destroys[i].gameObject, true);//当快进时候，视图层update没有被更新，所以使用Destroy的话，对象就没有被销毁，当玩家吃肉升级后， 怪物锁定的之前的玩家对象还存在！
                fixedUpdates.Remove(destroys[i]);
                updates.Remove(destroys[i]);
                lateUpdates.Remove(destroys[i]);
                guis.Remove(destroys[i]);
            }
            starts.Clear();
            destroys.Clear();
        }
    }
}
#endif