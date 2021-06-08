#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component.Client;
using Net.Share;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace LockStep.Client
{
    public class DataProcess : SingleCase<DataProcess>, INetworkHandle
    {
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            InitSDRpc();
        }

        public void InitSDRpc()
        {
            var cm = GetComponent<ClientManager>();
            cm.client.OnSerializeRPC = SerializeRpc;
            cm.client.OnDeserializeRPC = DeserializeRpc;
            cm.client.BindNetworkHandle(this);
            NetConvertFast.Init();
        }

        private FuncData DeserializeRpc(byte[] arg, int index, int count)
        {
            return NetConvertFast.Deserialize(arg, index, count);
        }

        private byte[] SerializeRpc(RPCModel arg)
        {
            return NetConvertFast.Serialize(arg);
        }

        public void OnBlockConnection()
        {
        }

        public void OnCloseConnect()
        {
        }

        public void OnConnected()
        {
        }

        public void OnConnectFailed()
        {
            MessageBox.Show("网络错误!", "连接服务器失败!", (r) =>
            {
                Destroy(Instance.gameObject);
                SceneManager.LoadSceneAsync(0);
            });
        }

        public void OnConnectLost()
        {
            MessageBox.Show("网络错误!", "连接中断...", (r) =>
            {
                Destroy(Instance.gameObject);
                SceneManager.LoadSceneAsync(0);
            });
        }

        public void OnDisconnect()
        {
        }

        public void OnReconnect()
        {
        }

        public void OnTryToConnect()
        {
        }
    }
}
#endif