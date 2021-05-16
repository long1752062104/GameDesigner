using Net.Event;
using Net.Share;

namespace Net.Adapter
{
    /// <summary>
    /// 网络事件监听适配器
    /// </summary>
    public class ListenerAdapter : INetworkEvtAdapter
    {
        public void OnBlockConnection()
        {
            NDebug.LogError("服务器爆满了!");
        }

        public void OnCloseConnect()
        {
            NDebug.Log("客户端关闭了连接");
        }

        public void OnConnected()
        {
            NDebug.Log("成功连接服务器");
        }

        public void OnConnectFailed()
        {
            NDebug.LogError("连接服务器失败");
        }

        public void OnConnectLost()
        {
            NDebug.LogError("连接中断了");
        }

        public void OnDisconnect()
        {
            NDebug.Log("主动断开连接!");
        }

        public void OnReconnect()
        {
            NDebug.Log("重连服务器成功!");
        }

        public void OnTryToConnect()
        {
            NDebug.Log("尝试重新连接服务器!");
        }
    }
}
