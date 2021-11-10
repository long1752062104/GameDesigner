#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace AOIExample
{
    public class Command : Net.Component.Command
    {
        public const byte Show = 150;
        public const byte Hide = 151;
    }
}
#endif