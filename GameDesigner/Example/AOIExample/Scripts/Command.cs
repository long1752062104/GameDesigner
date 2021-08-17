#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace AOIExample
{
    public class Command : Net.Component.Command
    {
        public const byte Show = 45;
        public const byte Hide = 46;
    }
}
#endif