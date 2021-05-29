using Net.Server;
using System.Collections.Concurrent;

namespace Net.Share
{
    /// <summary>
    /// 用户对接基类服务器
    /// </summary>
    public interface IServerHandle<Player, Scene> :
        IServerSendHandle<Player>,
        INetworkSceneHandle<Player, Scene>,
        IServerEventHandle<Player>
        where Player : NetPlayer where Scene : NetScene<Player>
    {
        /// <summary>
        /// 服务器场景，每个key都处于一个场景或房间，关卡，value是场景对象
        /// </summary>
        ConcurrentDictionary<string, Scene> Scenes { get; set; }
        /// <summary>
        /// 服务器是否处于运行状态, 如果服务器套接字已经被释放则返回False, 否则返回True. 当调用Close方法后将改变状态
        /// </summary>
        bool IsRunServer { get; set; }
    }
}
