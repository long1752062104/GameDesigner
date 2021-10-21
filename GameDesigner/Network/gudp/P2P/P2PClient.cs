using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Net.Client
{
    /// <summary>
    /// NAT穿透最详细介绍: https://www.cnblogs.com/ssyfj/p/14791064.html#三：p2p网络中的nat穿透问题---nat的原理和类型
    /// </summary>
    [Obsolete("未完工!", true)]
    public class P2PClient : UdpClient
    {
        public Task Connect(IPEndPoint remotePoint)
        {
            var remotePointStr = remotePoint.ToString().Split(':');
            var ip = remotePointStr[0];
            var port = int.Parse(remotePointStr[1]);
            return Connect(ip, port);
        }

        protected override Task ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//创建套接字
            this.localPort = localPort;
            if (localPort != -1)
                Client.Bind(new IPEndPoint(IPAddress.Any, localPort));
            Client.Connect(host, port);
            Connected = true;
            StartupThread();
            InvokeContext((arg) => {
                networkState = Share.NetworkState.ConnectFailed;
                result(true);
            });
            return Task.Delay(1);
        }
    }
}
