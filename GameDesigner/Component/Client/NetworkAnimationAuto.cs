#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Share;
    using UnityEngine;

    /// <summary>
    /// 网络同步动画组件, 自动检测动画改变即发送动画同步
    /// </summary>
    public class NetworkAnimationAuto : NetworkAnimationBase
    {
        private void Awake()
        {
            anim = GetComponent<Animation>();
            foreach (AnimationState item in anim)
                clips.Add(item.clip);
            nt.animations.Add(this);
            id = nt.animations.Count - 1;
        }

        void Update()
        {
            if (nt.mode == SyncMode.Synchronized)
                return;
            if (Time.time < sendTime)
                return;
            sendTime = Time.time + (1f / rate);
            if (anim.isPlaying)
            {
                if (!anim.IsPlaying(clipName) | !beinPlay)
                {
                    int index = 0;
                    for (int i = 0; i < clips.Count; i++)
                    {
                        if (anim.IsPlaying(clips[i].name))
                        {
                            clipName = clips[i].name;
                            index = i;
                            beinPlay = true;
                            break;
                        }
                    }
                    ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
                    {
                        index1 = id,
                        index2 = index
                    });
                }
            }
            else if (!anim.isPlaying)
            {
                beinPlay = false;
            }
        }

        public override void OnNetworkPlay(Operation opt)
        {
            anim.Play(clips[opt.index2].name);
        }
    }
}
#endif