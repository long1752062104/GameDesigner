
using Net.Event;
using Net.Share;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Client
{
    [Obsolete("弃用")]
    public class RawClient : ClientBase
    {
        protected override Task ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            try
            {
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);//创建套接字
                this.localPort = localPort;
                if (localPort != -1)
                    Client.Bind(new IPEndPoint(IPAddress.Any, localPort));
                Client.Connect(host, port);
                bool isDone = false;
                Task.Run(() =>
                {
                    while (!isDone)
                    {
                        if (Client != null)
                            SendByteData(new byte[] { 6, 0, 0, 0, 0, 0x2d, 74, NetCmd.Connect, 0, 0, 0, 0 }, false);
                        Thread.Sleep(200);
                    }
                });
                return Task.Run(() =>
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        Client.ReceiveTimeout = 5000;
                        int count = Client.Receive(buffer);
                        Client.ReceiveTimeout = 0;
                        isDone = true;
                        if (buffer[7] == NetCmd.BlockConnection)
                        {
                            InvokeContext((arg) => {
                                networkState = NetworkState.BlockConnection;
                                StateHandle();
                            });
                            throw new Exception();
                        }
                        if (buffer[7] == NetCmd.ExceededNumber)
                        {
                            InvokeContext((arg) => {
                                networkState = NetworkState.ExceededNumber;
                                StateHandle();
                            });
                            throw new Exception();
                        }
                        Connected = true;
                        StartupThread();
                        InvokeContext((arg) => {
                            networkState = NetworkState.Connected;
                            result(true);
                        });
                    }
                    catch (Exception)
                    {
                        isDone = true;
                        Client?.Close();
                        Client = null;
                        InvokeContext((arg) => {
                            networkState = NetworkState.ConnectFailed;
                            result(false);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                NDebug.LogError("连接错误:" + ex.ToString());
                networkState = NetworkState.ConnectFailed;
                result(false);
                return null;
            }
        }

        protected override void SendByteData(byte[] buffer, bool reliable)
        {


            sendCount += buffer.Length;
            sendAmount++;
            if (buffer.Length <= 65507)
                Client.SendTo(buffer, 0, buffer.Length, SocketFlags.None, Client.RemoteEndPoint);
            else
                NDebug.LogError("数据过大, 请使用SendRT发送...");
        }
    }
}
