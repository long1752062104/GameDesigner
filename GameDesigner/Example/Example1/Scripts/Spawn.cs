#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Example
{
    using Net.Component;
    using UnityEngine;

    public class Spawn : MonoBehaviour
    {
        public int num = 1000;
        public int currnum;
        public GameObject @object;

        // Start is called before the first frame update
        void Start()
        {
            if (ClientManager.Instance.client.Connected)
                Connected();
            else
                ClientManager.Instance.client.OnConnectedHandle += Connected;
        }

        private void Connected()
        {
            //服务器的记录uid从10000开始,所以这里uid-10000=0(transform同步组件唯一id), 这里 * 5000是每个客户端都可以实例化5000个transform同步组件
            //并且保证唯一id都是正确的,如果一个客户端实例化超过5000个, 就会和uid=10001的玩家transform同步物体唯一id碰撞, 会出现弹跳问题
            //NetworkTransformBase.Identity = (ClientManager.UID - 10000) * 5000;//避免唯一标识碰撞
            InvokeRepeating(nameof(Ins), 0.1f, 0.1f);
        }

        void Ins()
        {
            if (currnum >= num)
                return;
            currnum++;
            Instantiate(@object, transform.position, transform.rotation);
            transform.Rotate(transform.forward, 30f);
        }
    }
}
#endif