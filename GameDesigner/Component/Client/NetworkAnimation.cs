#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Share;
    using UnityEngine;

    /// <summary>
    /// 网络动画同步组件, 主动调用播放方法来同步动画
    /// </summary>
    public class NetworkAnimation : NetworkAnimationBase
    {
        private void Awake()
        {
            anim = GetComponent<Animation>();
            foreach (AnimationState item in anim)
                clips.Add(item.clip);
            nt.animations.Add(this);
            id = nt.animations.Count - 1;
        }

        public override void OnNetworkPlay(Operation opt)
        {
            switch (opt.cmd1)
            {
                case 0:
                    anim.Play(clips[opt.index2].name);
                    break;
                case 1:
                    anim.Stop();
                    break;
                case 2:
                    anim.Stop(clips[opt.index2].name);
                    break;
                case 3:
                    anim.Rewind();
                    break;
                case 4:
                    anim.Rewind(clips[opt.index2].name);
                    break;
                case 5:
                    anim.Sample();
                    break;
                case 6:
                    anim.Play((PlayMode)opt.cmd2);
                    break;
                case 7:
                    anim.Play(clips[opt.index2].name, (PlayMode)opt.cmd2);
                    break;
                case 8:
                    anim.CrossFade(clips[opt.index2].name, opt.direction.x, (PlayMode)opt.cmd2);
                    break;
                case 9:
                    anim.Blend(clips[opt.index2].name, opt.direction.x, opt.direction.y);
                    break;
                case 10:
                    anim.CrossFadeQueued(clips[opt.index2].name, opt.direction.x, (QueueMode)opt.buffer[0], (PlayMode)opt.buffer[2]);
                    break;
                case 11:
                    anim.PlayQueued(clips[opt.index2].name, (QueueMode)opt.buffer[0], (PlayMode)opt.buffer[2]);
                    break;
            }
        }

        public void Stop()
        {
            ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
            {
                cmd1 = 1,
                index1 = id
            });
        }

        public void Stop(string name)
        {
            for (int index = 0; index < clips.Count; index++)
            {
                if (clips[index].name == name)
                {
                    ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
                    {
                        cmd1 = 2,
                        index1 = id,
                        index2 = index
                    });
                    break;
                }
            }
        }

        public void Rewind(string name)
        {
            for (int index = 0; index < clips.Count; index++)
            {
                if (clips[index].name == name)
                {
                    ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
                    {
                        cmd1 = 4,
                        index1 = id,
                        index2 = index
                    });
                    break;
                }
            }
        }

        public void Rewind()
        {
            ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
            {
                cmd1 = 3,
                index1 = id
            });
        }

        public void Sample()
        {
            ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
            {
                cmd1 = 5,
                index1 = id
            });
        }

        public void Play()
        {
            PlayMode mode = PlayMode.StopSameLayer;
            Play(mode);
        }

        public void Play(PlayMode mode)
        {
            ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
            {
                cmd1 = 6,
                cmd2 = (byte)mode,
                index1 = id
            });
        }

        public void Play(string animation)
        {
            PlayMode mode = PlayMode.StopSameLayer;
            Play(animation, mode);
        }

        public void Play(string animation, PlayMode mode)
        {
            for (int index = 0; index < clips.Count; index++)
            {
                if (clips[index].name == animation)
                {
                    ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
                    {
                        cmd1 = 7,
                        cmd2 = (byte)mode,
                        index1 = id,
                        index2 = index
                    });
                    break;
                }
            }
        }

        public void CrossFade(string animation, float fadeLength)
        {
            PlayMode mode = PlayMode.StopSameLayer;
            CrossFade(animation, fadeLength, mode);
        }

        public void CrossFade(string animation)
        {
            PlayMode mode = PlayMode.StopSameLayer;
            float fadeLength = 0.3f;
            CrossFade(animation, fadeLength, mode);
        }

        public void CrossFade(string animation, float fadeLength, PlayMode mode)
        {
            for (int index = 0; index < clips.Count; index++)
            {
                if (clips[index].name == animation)
                {
                    ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
                    {
                        cmd1 = 8,
                        cmd2 = (byte)mode,
                        index1 = id,
                        index2 = index,
                        direction = new Net.Vector3(fadeLength, 0, 0)
                    });
                    break;
                }
            }
        }

        public void Blend(string animation, float targetWeight)
        {
            float fadeLength = 0.3f;
            Blend(animation, targetWeight, fadeLength);
        }

        public void Blend(string animation)
        {
            float fadeLength = 0.3f;
            float targetWeight = 1f;
            Blend(animation, targetWeight, fadeLength);
        }

        public void Blend(string animation, float targetWeight, float fadeLength)
        {
            for (int index = 0; index < clips.Count; index++)
            {
                if (clips[index].name == animation)
                {
                    ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
                    {
                        cmd1 = 9,
                        index1 = id,
                        index2 = index,
                        direction = new Net.Vector3(targetWeight, fadeLength, 0)
                    });
                    break;
                }
            }
        }

        public void CrossFadeQueued(string animation, float fadeLength, QueueMode queue)
        {
            PlayMode mode = PlayMode.StopSameLayer;
            CrossFadeQueued(animation, fadeLength, queue, mode);
        }

        public void CrossFadeQueued(string animation, float fadeLength)
        {
            PlayMode mode = PlayMode.StopSameLayer;
            QueueMode queue = QueueMode.CompleteOthers;
            CrossFadeQueued(animation, fadeLength, queue, mode);
        }

        public void CrossFadeQueued(string animation)
        {
            PlayMode mode = PlayMode.StopSameLayer;
            QueueMode queue = QueueMode.CompleteOthers;
            float fadeLength = 0.3f;
            CrossFadeQueued(animation, fadeLength, queue, mode);
        }

        public void CrossFadeQueued(string animation, float fadeLength, QueueMode queue, PlayMode mode)
        {
            for (int index = 0; index < clips.Count; index++)
            {
                if (clips[index].name == animation)
                {
                    ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
                    {
                        cmd1 = 10,
                        index1 = id,
                        index2 = index,
                        direction = new Net.Vector3(fadeLength, 0, 0),
                        buffer = new byte[] { (byte)queue, (byte)mode }
                    });
                    break;
                }
            }
        }

        public void PlayQueued(string animation, QueueMode queue)
        {
            PlayMode mode = PlayMode.StopSameLayer;
            PlayQueued(animation, queue, mode);
        }

        public void PlayQueued(string animation)
        {
            PlayMode mode = PlayMode.StopSameLayer;
            QueueMode queue = QueueMode.CompleteOthers;
            PlayQueued(animation, queue, mode);
        }

        public void PlayQueued(string animation, QueueMode queue, PlayMode mode)
        {
            for (int index = 0; index < clips.Count; index++)
            {
                if (clips[index].name == animation)
                {
                    ClientManager.AddOperation(new Operation(Command.Animation, nt.identity)
                    {
                        cmd1 = 11,
                        index1 = id,
                        index2 = index,
                        buffer = new byte[] { (byte)queue, (byte)mode }
                    });
                    break;
                }
            }
        }
    }
}
#endif