#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || SERVICE
using Net.Share;
using TrueSync;

namespace LockStep.Client
{
    public class Player : Actor
    {
        public ObjectView objectView;
        public FP moveSpeed = 5f;

        public Player(GameSystem gameSystem) : base(gameSystem)
        {
        }

        public override void Update(Operation opt)
        {
            Net.Vector3 dir = opt.direction;
            transform.position += new TSVector3(dir.x, dir.y, dir.z) * moveSpeed * LSTime.deltaTime;
        }

        public override void OnGUI()
        {
        }
    }
}
#endif