#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using global::System.Collections.Generic;
    using Net.Share;
    using UnityEngine;

    /// <summary>
    /// 网络同步动画机基础类
    /// </summary>
    public abstract class NetworkAnimationBase : MonoBehaviour
    {
        public NetworkTransformBase nt;
        protected Animation anim;
        protected string clipName = "";
        protected readonly List<AnimationClip> clips = new List<AnimationClip>();
        protected float sendTime;
        public float rate = 30f;//网络帧率, 一秒30次
        protected bool beinPlay;
        internal int id;

        public abstract void OnNetworkPlay(Operation opt);
    }
}
#endif