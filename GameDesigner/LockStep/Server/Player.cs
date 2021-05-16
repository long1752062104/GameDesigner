using Net.Server;

namespace LockStep.Server
{
    public class Player : NetPlayer
    {
        internal bool readyBattle;
        private Scene scene;
        public Scene Scene { get { return scene; } set { scene = value; } }

        public override void OnExit()
        {
            scene = null;
        }
    }
}
