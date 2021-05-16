using Net.Client;
using Net.Server;
using System;

namespace Net.Helper
{
    /// <summary>
    /// 分散控制系统（分布式子服务器）案例代码 需要开发者扩展
    /// <code>服务器先时连接网关服务器，委托网关服务器存ip和端口，当网关服务器有客户端连接后，会指派客户端连接的子服务器</code>
    /// </summary>
    /// <typeparam name="Player"></typeparam>
    /// <typeparam name="Scene"></typeparam>
    public class DCSHelper<Player,Scene> : UdpServer<Player,Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
        public UdpClient client;

        public override void Start(ushort port = 6665)
        {
            client?.Close();
            client = new UdpClient();
            client.Connect("127.0.0.1", 6666).Wait();//此处要写为你的网关服务器ip和端口
            if (!client.Connected)
                throw new Exception("网关服务器尚未开启，连接失败!");
            client.SendRT("UpdateArea", Name, NetPort.GetIP(), port, onlineNumber);//要让网关服务器记录一下
            base.Start(port);
        }

        protected override void OnRemoveClient(Player client)//当玩家退出游戏后更新网关服务器
        {
            this.client.SendRT("UpdateArea", Name, NetPort.GetIP(), Port, onlineNumber);
        }
    }
}
