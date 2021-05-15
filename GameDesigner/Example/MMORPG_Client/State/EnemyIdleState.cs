#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.MMORPG_Client
{
    using GameDesigner;
    using Net.Component;
    using Net.Component.Client;
    using Net.Share;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class EnemyIdleState : StateBehaviour
    {
        public EnemyComponent enemy;
        private float time;

        private void Start()
        {
            enemy = transform.GetComponent<EnemyComponent>();
        }

        public override void OnEnter(State currentState, State nextState)
        {
            time = Time.time + Random.Range(1, 3);
        }

        public override void OnUpdate(State currentState, State nextState)
        {
            if (enemy.hp <= 0)
            {
                EnterState(enemy.deadID);
                return;
            }
            if (!ClientManager.Instance.control)
                return;

            if (enemy.player != null)
            {
                if (enemy.player.isDead)
                {
                    enemy.player = null;
                    return;
                }
                var dis = Vector3.Distance(enemy.player.transform.position, enemy.transform.position);
                if (dis < enemy.attackR)
                {
                    enemy.transform.LookAt(enemy.player.position, Vector3.up);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    ClientManager.AddOperation(new Operation(Command.EnemySwitchState, enemy.name, Random.Range(2, 3)));
                }
                else if (dis < enemy.pursuit)
                {
                    ClientManager.AddOperation(new Operation(Command.EnemySwitchState, enemy.name, 1));
                }
                else
                    enemy.player = null;
            }
            else if (Time.time > time)
            {
                ClientManager.AddOperation(new Operation(Command.EnemySwitchState, enemy.name, 1));
            }
        }
    }
}
#endif