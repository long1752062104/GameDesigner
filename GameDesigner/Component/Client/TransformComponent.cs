#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    /// <summary>
    /// 网络Transform同步组件, 没有同步子物体, 如果想同步子物体, 请使用<see cref="NetworkTransform"/>组件
    /// </summary>
    public class TransformComponent : NetworkTransformBase
    {
    }
}
#endif