using Net.Server;
using Net.Share;

namespace Net.Example2
{
    /// <summary>
    /// 场景管理器, 状态同步, 帧同步 固定帧发送当前场景的所有玩家操作
    /// </summary>
    public class SceneComponent : NetScene<PlayerComponent>
    {
        public override void OnEnter(PlayerComponent client)
        {
            client.Scene = this;
        }

        /// <summary>
        /// 网络帧同步, 状态同步更新
        /// </summary>
        public override void Update(IServerSendHandle<PlayerComponent> handle, byte cmd = 18)
        {
            base.Update(handle, NetCmd.OperationSync);
        }
    }
}