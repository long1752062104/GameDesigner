using Net.Server;
using Net.AOI;
using Net.Share;
using System.Collections.Generic;
using Net.Component;
using Net.Event;

namespace AOIExample
{
    internal class Scene:NetScene<Client>
    {
        internal GridManager gridManager = new GridManager();

        public Scene() 
        {
            gridManager.Init(-500, -500, 20, 20, 50, 50);
        }
        public override void OnEnter(Client client)
        {
            gridManager.Insert(client);
        }
        public override void OnExit(Client client)
        {
            AddOperation(new Operation(Command.Destroy, client.UserID));
            gridManager.Remove(client);
        }
        public override void OnOperationSync(Client client, OperationList list)
        {
            foreach (var item in list.operations)
            {
                switch (item.cmd) 
                {
                    case Command.Transform:
                        client.Position = item.position;
                        client.operations.Add(item);
                        break;
                    default:
                        client.operations.Add(item);
                        break;
                }
            }
        }
        public override void Update(IServerSendHandle<Client> handle, byte cmd = 19)
        {
            var players = Clients;
            int playerCount = players.Count;
            if (playerCount <= 0)
                return;
            frame++;
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (player == null)
                    continue;
                player.OnUpdate();
                List<Operation> opts = new List<Operation>();
                var grid = gridManager.TryGetGrid(players[i]);
                if (grid == null)
                    continue;
                var bodies = grid.GetGridBodiesAll();
                foreach (var item in bodies)
                {
                    var player1 = item as Client;
                    var opts1 = player1.operations.GetRange(0, player1.operations.Count);
                    player1.getLen = opts1.Length;
                    opts.AddRange(opts1);
                }
                if (opts.Count == 0)
                    continue;
                OperationList list = ObjectPool<OperationList>.Take();
                list.frame = frame;
                list.operations = opts.ToArray();
                var buffer = onSerializeOptHandle(list);
                handle.Send(player, cmd, buffer, false, false);
                ObjectPool<OperationList>.Push(list);
            }
            int count = operations.Count;//不管aoi, 整个场景的同步在这里, 如玩家退出操作
            if (count > 0)
            {
                while (count > Split)
                {
                    OnPacket(handle, cmd, Split);
                    count -= Split;
                }
                if (count > 0)
                    OnPacket(handle, cmd, count);
            }
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == null)
                    continue;
                players[i].operations.RemoveRange(0, players[i].getLen);
            }
            gridManager.UpdateHandler();
            Event.UpdateEvent();
        }
    }
}