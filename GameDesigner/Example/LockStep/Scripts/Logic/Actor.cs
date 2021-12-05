#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using System;
using TrueSync;

namespace LockStep.Client
{
    [Serializable]
    public class Actor : ECS.Component
    {
        public string name;
        public UnityEngine.GameObject gameObject;
        public TSTransform transform;
    }
}
#endif