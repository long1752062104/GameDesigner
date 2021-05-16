#if !NO_RUNTIME
using ProtoBuf.Meta;
namespace ProtoBuf.Serializers
{
    interface IProtoTypeSerializer : IProtoSerializer
    {
        bool HasCallbacks(TypeModel.CallbackType callbackType);
        bool CanCreateInstance();
#if !FEAT_IKVM
        object CreateInstance(ProtoReader source);
        void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context);
#endif
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || SERVICE
        void EmitCallback(Compiler.CompilerContext ctx, Compiler.Local valueFrom, TypeModel.CallbackType callbackType);
#endif
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || SERVICE
        void EmitCreateInstance(Compiler.CompilerContext ctx);
#endif
    }
}
#endif