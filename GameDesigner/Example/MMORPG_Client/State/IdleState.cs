#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.MMORPG_Client
{
    using GameDesigner;
    using Net.Component;
    using Net.Component.Client;
    using Net.Share;
    using UnityEngine;

    public class IdleState : StateBehaviour
    {
        public PlayerComponent player;

        private void Start()
        {
            player = transform.GetComponent<PlayerComponent>();
        }

        public static Vector3 moveDirection
        {
            get { return new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")); }
        }

        public override void OnUpdate(State currentState, State nextState)
        {
            if (ClientManager.Instance.client.Identify == player.name)//如果是本地标识,则可以发送键盘操作
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    ClientManager.AddOperation(new Operation() { name = player.name, cmd = Command.Attack, index = 2 });
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ClientManager.AddOperation(new Operation() { name = player.name, cmd = Command.Attack, index = 3 });
                }

                if (moveDirection != Vector3.zero)
                {
                    ClientManager.AddOperation(new Operation(Command.SwitchState, player.name, 1));
                }
            }
        }
    }
}
#endif