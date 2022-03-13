#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Client;
    using Net.Event;
    using Net.Share;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Threading;
    using UnityEngine;
    using global::System.Net;
    using global::System.Threading.Tasks;

    public enum TransportProtocol
    {
        Gudp, Tcp, Udx, Kcp, Web, Enet
    }

    public class ClientManager : SingleCase<ClientManager>, ISendHandle
    {
        private bool mainInstance;
        private ClientBase _client;
        public TransportProtocol protocol = TransportProtocol.Gudp;
        public string ip = "127.0.0.1";
        public int port = 6666;
        public bool throwException;
        public bool debugRpc = true;
        public int frameRate = 60;
        public bool authorize;
        public bool startConnect = true;
        public bool md5CRC;
        public List<RPCMethod> rpcs = new List<RPCMethod>();

        public ClientBase client
        {
            get
            {
                if (_client == null)
                {
                    switch (protocol)
                    {
                        case TransportProtocol.Gudp:
                            _client = new UdpClient(true);
                            break;
                        case TransportProtocol.Tcp:
                            _client = new TcpClient(true);
                            break;
                        case TransportProtocol.Enet:
                            _client = new ENetClient(true);
                            break;
                        case TransportProtocol.Kcp:
                            _client = new KcpClient(true);
                            break;
                        case TransportProtocol.Udx:   
                            _client = new UdxClient(true);
                            break;
#if UNITY_STANDALONE_WIN || UNITY_WSA
                        case TransportProtocol.Web:
                            _client = new Client.WebClient(true);
                            break;
#endif
                    }
                    _client.host = ip;
                    _client.port = port;
                    _client.ThrowException = throwException;
                    _client.LogRpc = debugRpc;
                    _client.MD5CRC = md5CRC;
                }
                return _client;
            }
            set { _client = value; }
        }

        /// <summary>
        /// 客户端唯一标识
        /// </summary>
        public static string Identify { get { return Instance.client.Identify; } }
        /// <summary>
        /// 客户端唯一标识
        /// </summary>
        public static int UID { get { return Instance.client.UID; } }

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            mainInstance = true;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = frameRate;
            Application.runInBackground = true;
        }

        // Use this for initialization
        void Start()
        {
            NDebug.BindLogAll(Debug.Log);
            if (startConnect)
                Connect();
        }

        public Task<bool> Connect()
        {
            _client = client;
            var ips = Dns.GetHostAddresses(ip);
            if (ips.Length > 0)
                _client.host = ips[UnityEngine.Random.Range(0, ips.Length)].ToString();
            else
                _client.host = ip;
            _client.port = port;
            _client.AddRpcHandle(this);
            return _client.Connect(result =>
            {
                if (result)
                {
                    _client.Send(new byte[1]);//发送一个字节:调用服务器的OnUnClientRequest方法, 如果不需要账号登录, 则会直接允许进入服务器
                }
            });
        }

        // Update is called once per frame
        void Update()
        {
            _client.NetworkEventUpdate();
            rpcs = _client.RPCs;
        }

        void OnDestroy()
        {
            if (mainInstance)
                _client.Close();
        }

        /// <summary>
        /// 发起场景同步操作, 在同一个场景的所有客户端都会收到该操作参数operation
        /// </summary>
        /// <param name="operation"></param>
        public static void AddOperation(Operation operation)
        {
            Instance.client.AddOperation(operation);
        }

        public static void AddRpcHandler(object target)
        {
            I.client.AddRpcHandle(target);
        }

        /// <summary>
        /// 判断name是否是本地唯一id(本机玩家标识)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static bool IsLocal(string name)
        {
            if (Instance == null)
                return false;
            return instance._client.Identify == name;
        }

        /// <summary>
        /// 判断uid是否是本地唯一id(本机玩家标识)
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        internal static bool IsLocal(int uid)
        {
            if (Instance == null)
                return false;
            return instance._client.UID == uid;
        }

        [rpc]
        void Offline(string info)
        {
            MessageBox.Show("登录提示", info, (r) =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            });
        }

#region 发送接口实现
        public void Send(byte[] buffer)
        {
            ((ISendHandle)_client).Send(buffer);
        }

        public void Send(byte cmd, byte[] buffer)
        {
            ((ISendHandle)_client).Send(cmd, buffer);
        }

        public void Send(byte cmd, object obj)
        {
            ((ISendHandle)_client).Send(cmd, obj);
        }

        public void Send(string func, params object[] pars)
        {
            ((ISendHandle)_client).Send(func, pars);
        }

        public void Send(byte cmd, string func, params object[] pars)
        {
            ((ISendHandle)_client).Send(cmd, func, pars);
        }

        public void CallRpc(string func, params object[] pars)
        {
            ((ISendHandle)_client).CallRpc(func, pars);
        }

        public void CallRpc(byte cmd, string func, params object[] pars)
        {
            ((ISendHandle)_client).CallRpc(cmd, func, pars);
        }

        public void Request(string func, params object[] pars)
        {
            ((ISendHandle)_client).Request(func, pars);
        }

        public void Request(byte cmd, string func, params object[] pars)
        {
            ((ISendHandle)_client).Request(cmd, func, pars);
        }

        public void SendRT(string func, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(func, pars);
        }

        public void SendRT(byte cmd, string func, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(cmd, func, pars);
        }

        public void SendRT(byte[] buffer)
        {
            ((ISendHandle)_client).SendRT(buffer);
        }

        public void SendRT(byte cmd, byte[] buffer)
        {
            ((ISendHandle)_client).SendRT(cmd, buffer);
        }

        public void SendRT(byte cmd, object obj)
        {
            ((ISendHandle)_client).SendRT(cmd, obj);
        }

        public void Send(string func, string funcCB, Delegate callback, params object[] pars)
        {
            ((ISendHandle)_client).Send(func, funcCB, callback, pars);
        }

        public void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars)
        {
            ((ISendHandle)_client).Send(func, funcCB, callback, millisecondsDelay, pars);
        }

        public void Send(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            ((ISendHandle)_client).Send(func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            ((ISendHandle)_client).Send(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void SendRT(string func, string funcCB, Delegate callback, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(func, funcCB, callback, pars);
        }

        public void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(func, funcCB, callback, millisecondsDelay, pars);
        }

        public void SendRT(string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, pars);
        }

        public void Send(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            ((ISendHandle)_client).Send(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, context, pars);
        }

        public void SendRT(byte cmd, string func, string funcCB, Delegate callback, int millisecondsDelay, Action outTimeAct, SynchronizationContext context, params object[] pars)
        {
            ((ISendHandle)_client).SendRT(cmd, func, funcCB, callback, millisecondsDelay, outTimeAct, context, pars);
        }
        #endregion
    }
}
#endif