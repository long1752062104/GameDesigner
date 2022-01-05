#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Example
{
    using Net.Server;
    using Net.Share;
    using UnityEngine;

    public class ServerManager : MonoBehaviour
    {
        private Service server;
        public ushort port = 6666;
        public bool isEncrypt = false;//数据加密?

        // Start is called before the first frame update
        void Start()
        {
            server = new Service();//创建服务器对象
            server.AddAdapter(new Adapter.SerializeAdapter2() { isEncrypt = isEncrypt });
            server.Run(port);//启动
        }

        private void OnDestroy()
        {
            if (server == null)
                return;
            server.Close();
        }
    }

    public class Service : TcpServer<NetPlayer, NetScene<NetPlayer>>
    {
        [Rpc]
        void Test(string info)
        {
            Debug.Log(info);
        }
    }
}
#endif