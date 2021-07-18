using Net.Server;
using Net.Share;
using System.Collections.Generic;

namespace LockStep.Server
{
    public class Scene : NetScene<Player>
    {
        public bool battle;
        public List<OperationList> frameDatas = new List<OperationList>();
        internal bool check;
        internal int actionId;

        public override void OnEnter(Player client)
        {
            client.Scene = this;
        }

        //帧同步核心功能
        public override void Update(IServerSendHandle<Player> handle, byte cmd = 19)
        {
            if (!battle)
                return;
            List<Player> players = GetPlayers();
            if (players.Count <= 0)
            {
                frame = 0;
                frameDatas.Clear();
                battle = false;
                return;
            }
            int count = operations.Count;
            OnPacket(handle, cmd, count);
            frame++;
        }

        public override void OnExit(Player client)
        {
            AddOperation(new Operation(NetCmd.QuitGame, client.UserID));
        }
    }
}
