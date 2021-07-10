#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Example2
{
    public class Command : Net.Component.Command
    {
        public const byte Fire = 46;
        public const byte AIMonster = 47;
        public const byte SetTarget = 48;
    }
}
#endif