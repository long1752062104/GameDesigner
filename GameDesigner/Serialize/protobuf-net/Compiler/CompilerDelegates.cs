//#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || SERVICE
namespace ProtoBuf.Compiler
{
    internal delegate void ProtoSerializer(object value, ProtoWriter dest);
    internal delegate object ProtoDeserializer(object value, ProtoReader source);
}
//#endif