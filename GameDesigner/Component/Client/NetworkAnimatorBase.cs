#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Share;
    using UnityEngine;

    public abstract class NetworkAnimatorBase : MonoBehaviour
    {
        public NetworkTransformBase nt;
        protected Animator animator;
        protected AnimatorParameter[] parameters;
        protected float sendTime;
        public float rate = 30f;//网络帧率, 一秒30次
        internal int id;
        protected int nameHash = -1;
        protected int currLayer = -1;

        protected class AnimatorParameter
        {
            internal string name;
            internal AnimatorControllerParameterType type;
            internal float defaultFloat;
            internal int defaultInt;
            internal bool defaultBool;
        }

        public abstract void OnNetworkPlay(int stateNameHash, int layer, float normalizedTime);

        public abstract void SyncAnimatorParameter(Operation opt);
    }
}
#endif