#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.MMORPG_Client
{
    using GameDesigner;
    using Net.Component;
    using Net.Component.Client;
    using Net.Share;
    using UnityEngine;

    public class MoveState : StateBehaviour
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

        /// <summary>
        /// 摄像机的Transform通过摇杆输出的方向
        /// </summary>
        public static Vector3 Transform3Dir(Transform t, Vector3 dir)
        {
            //注意：1、摄像机的Y轴角度和摇杆的坐标系是反方向的  2、角度和弧度
            var f = Mathf.Deg2Rad * (-t.rotation.eulerAngles.y);
            //方向标准化
            dir.Normalize();
            //旋转角度
            var ret = new Vector3(dir.x * Mathf.Cos(f) - dir.z * Mathf.Sin(f), 0, dir.x * Mathf.Sin(f) + dir.z * Mathf.Cos(f));
            return ret;
        }

        public override void OnUpdate(State currentState, State nextState)
        {
            if (ClientManager.Instance.client.Identify == player.name)
            {//如果是本地标识,则可以发送键盘操作
                var dir1 = Transform3Dir(Camera.main.transform, moveDirection);
                ClientManager.AddOperation(new Operation(Command.Input, player.name, dir1));
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    ClientManager.AddOperation(new Operation() { name = player.name, cmd = Command.Attack, index = 2 });
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ClientManager.AddOperation(new Operation() { name = player.name, cmd = Command.Attack, index = 3 });
                }
                if (moveDirection == Vector3.zero)
                {
                    ClientManager.AddOperation(new Operation(Command.SwitchState, player.name, 0));
                }
            }

            if (player.direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.direction, Vector3.up), 0.5f);
                if (ClientManager.Instance.control)
                    transform.Translate(0, 0, player.moveSpeed * Time.deltaTime);
            }

            if (ClientManager.Instance.control)
                ClientManager.AddOperation(new Operation(Command.Movement, player.name, player.direction, transform.position, transform.rotation) { health = player.hp });
            else
                transform.position = Vector3.Lerp(transform.position, player.position, 0.5f);
        }
    }
}
#endif