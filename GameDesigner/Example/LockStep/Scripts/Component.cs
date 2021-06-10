#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || SERVICE
using TrueSync;

namespace LockStep.Client
{
    public class Component : Actor
    {
        public Component(GameSystem gameSystem) : base(gameSystem) { }
    }
}
#endif