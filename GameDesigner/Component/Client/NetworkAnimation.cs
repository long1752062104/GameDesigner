#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Share;
    using System.Collections.Generic;
    using UnityEngine;

    public class NetworkAnimation : MonoBehaviour
    {
        public NetworkTransformBase nt;
        private Animation anim;
        private string clipName = "";
        private List<AnimationClip> clips = new List<AnimationClip>();
        private float sendTime;
        public float rate = 30f;//网络帧率, 一秒30次
        private bool beinPlay;

        private void Awake()
        {
            anim = GetComponent<Animation>();
            foreach (AnimationState item in anim)
                clips.Add(item.clip);
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
                        index1 = index
                    });
                }
            }
            else if (!anim.isPlaying)
            {
                beinPlay = false;
            }
        }

        public void Play(int index)
        {
            Debug.Log(clips[index].name);
            anim.Play(clips[index].name);
        }
    }
}
#endif