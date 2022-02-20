using Net.Client;
using Net.Server;
using Net.Share;
using Net.System;

namespace Net.Helper
{
    public class GatewayServer1<Client> : TcpServer<Client, NetScene<Client>> where Client : NetPlayer, new()
    {
        private TcpClient gameClient;

        protected override byte[] OnSerializeRpc(RPCModel model)
        {
            return null;//不处理任何数据, 直接发送到业务服务器
        }
    }
}
