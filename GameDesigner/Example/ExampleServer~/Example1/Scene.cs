using Net.Component;
using Net.Server;
using Net.Share;

namespace Example1
{
    internal class Scene:NetScene<Client>
    {
        public override void OnExit(Client client)
        {
            AddOperation(new Operation(Command.OnPlayerExit) { uid = client.UserID });
        }
    }
}