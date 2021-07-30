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
            ClientManager.Instance.client.OnConnectedHandle += Connected;
        }

        private void Connected()
        {
            NetworkTransformBase.Identity = (ClientManager.UID - 10000) * 5000;//避免唯一标识碰撞
            InvokeRepeating("Ins", 0.1f, 0.1f);
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