namespace Net.Server
{
    using global::System.Collections;
    using global::System.Net;
    using global::System.Net.NetworkInformation;

    /// <summary>
    /// 网络端口检测类
    /// </summary>
    public static class NetPort
    {
        /// <summary>
        /// 获取第一个可用的端口号
        /// </summary>
        /// <param name="startPort">起始端口号</param>
        /// <param name="MaxPort">结束端口号</param>
        /// <returns></returns>
        public static int GetFirstAvailablePort(int startPort = 667, int MaxPort = 65535)
        {
            for (int i = startPort; i < MaxPort; i++)
            {
                if (PortIsAvailable(i)) return i;
            }
            return -1;
        }

        /// <summary>
        /// 获取操作系统已用的端口号
        /// </summary>
        /// <returns></returns>
        public static IList PortIsUsed()
        {
            //获取本地计算机的网络连接和通信统计数据的信息
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            //返回本地计算机上的所有Tcp监听程序
            IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();

            //返回本地计算机上的所有UDP监听程序
            IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();

            //返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息。
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            IList allPorts = new ArrayList();
            foreach (IPEndPoint ep in ipsTCP) allPorts.Add(ep.Port);
            foreach (IPEndPoint ep in ipsUDP) allPorts.Add(ep.Port);
            foreach (TcpConnectionInformation conn in tcpConnInfoArray) allPorts.Add(conn.LocalEndPoint.Port);

            return allPorts;
        }

        /// <summary>
        /// 检查指定端口是否已用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool PortIsAvailable(int port)
        {
            bool isAvailable = true;

            IList portUsed = PortIsUsed();

            foreach (int p in portUsed)
            {
                if (p == port)
                {
                    isAvailable = false; break;
                }
            }

            return isAvailable;
        }

        /// <summary>
        /// 获取本机ip地址
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry iPHostEntry = Dns.GetHostEntry(hostName);
            foreach (IPAddress ipAdd in iPHostEntry.AddressList)
                if (ipAdd.AddressFamily.ToString() == "InterNetwork")
                    return ipAdd.ToString();
            return "";
        }
    }
}
