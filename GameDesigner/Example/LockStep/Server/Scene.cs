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
            Operation[] opts = operations.GetRemoveRange(0, count);
            OperationList list1 = new OperationList(frame, opts);
            frameDatas.Add(list1);
            frame++;
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(512))
            {
                ProtoBuf.Serializer.Serialize(stream, list1);
                handle.Multicast(players, true, cmd, stream.ToArray(), false, false);
            }
        }
    }
}
