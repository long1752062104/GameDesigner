#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using UnityEngine;

namespace LockStep.Client
{
    public class GameMgr : Actor
    {
        public GameMgr(GameSystem gameSystem) : base(gameSystem)
        {
        }

        public override void Start()
        {
            Debug.Log("开始");
            Enemy enemy = new Enemy(gameSystem);
            EventSystem.AddEvent(5f, ()=> {
                enemy.Destroy();
            });
        }
    }
}
#endif