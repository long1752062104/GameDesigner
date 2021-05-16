#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.MMORPG_Client
{
    using GameDesigner;
    using UnityEngine;

    /// <summary>
    /// 玩家组件, 监听玩家输入wsad或上下左右键后 发送到消息中心进行同步发送
    /// </summary>
    public class PlayerComponent : Actor
    {
        // Use this for initialization
        void Start()
        {
            stateManager = GetComponent<StateManager>();
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

        public void Attack(int stateID)
        {
            stateManager.StatusEntry(stateID);
        }

        public override void OnDead()
        {
            isDead = true;
            stateManager.StatusEntry(deadID);
        }
    }
}
#endif