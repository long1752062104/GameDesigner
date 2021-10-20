using Net.Server;
using Net.Share;
using Net.System;
using System;
using System.Linq;

namespace Net.Helper
{
    /// <summary>
    /// 网关服务器(主服务器), 负责分配客户端连接最优的游戏服务器, 负载均衡
    /// <code>第一步:客户端连接网关服务器，然后网关服务器将集群游戏服务器的ip，端口发给客户端</code>
    /// <code>第二步:客户端自动连接到指定的游戏服务器上，请查看NetCmd.SwitchPort指令说明</code>
    /// </summary>
    public class GatewayServer<SubServer> : UdpServer<SubServer, NetScene<SubServer>> where SubServer : NetPlayer, new()
    {
        /// <summary>
        /// 分布式(集群)游戏服务器列表
        /// </summary>
        public MyDictionary<SubServer, Constituency> serverArea = new MyDictionary<SubServer, Constituency>();
        /// <summary>
        /// 当分布服务器更新时调用
        /// </summary>
        public Action<Constituency> OnAreaUpdate;

        protected override bool OnUnClientRequest(SubServer unClient, RPCModel model)
        {
            OnRpcExecute(unClient, model);
            return true;
        }

        protected override void OnRpcExecute(SubServer client, RPCModel model)
        {
            switch (model.func)//主服务器时, 客户端只能访问GetArea, AutoSelectArea方法
            {
                case "GetArea"://客户端输入帐号密码登录游戏 后 应该获取服务器区域， 选择想要进入的服务器区
                    object obj = serverArea.Values.ToList();
                    Send(client, "SetArea", obj);
                    break;
                case "UpdateArea"://当集群服务器的客户端退出游戏后会调用这个方法更新服务器的数据
                    if (!serverArea.TryGetValue(client, out Constituency area))
                        serverArea.Add(client, area = new Constituency());
                    area.name = model.pars[0].ToString();
                    area.ip = model.pars[1].ToString();
                    area.port = (ushort)model.pars[2];
                    area.online = (int)model.pars[3];
                    area.status = area.online <= 500 ? "顺畅" : area.online <= 1000 ? "拥挤" : "爆满";
                    OnAreaUpdate?.Invoke(area);
                    break;
                case "AutoSelectArea"://自动分配游戏服务器
                    var entries = serverArea.entries;
                    for (int i = 0; i < serverArea.count; i++)
                    {
                        if (entries[i].hashCode >= 0)
                        {
                            Constituency area1 = entries[i].value;
                            if (area1 == null)
                                continue;
                            if (area1.online < 800)
                            {
                                SendRT(client, NetCmd.SwitchPort, string.Empty, area1.ip, area1.port);
                                area1.online++;
                                return;
                            }
                        }
                    }
                    SendRT(client, NetCmd.BlockConnection, new byte[1]);
                    break;
            }
        }
    }
}
