namespace GameDesigner
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 音效管理
    /// </summary>
    public sealed class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private List<AudioSource> sources = new List<AudioSource>();
        [SerializeField]
        private List<AudioSource> destroyPlayingSources = new List<AudioSource>();
        private static AudioManager _instance = null;
        /// <summary>
        /// 音效实例
        /// </summary>
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                    if (_instance == null)
                    {
                        _instance = new GameObject("AudioManager").AddComponent<AudioManager>();
                        DontDestroyOnLoad(_instance);
                    }
                }
                return _instance;
            }
        }

        void Update()
        {
            for (int i = 0; i < destroyPlayingSources.Count; ++i)
            {
                if (!destroyPlayingSources[i].isPlaying)
                {
                    Destroy(destroyPlayingSources[i]);
                    destroyPlayingSources.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 播放音效剪辑
        /// 参数clip : 放入你要播放的音源
        /// </summary>
        public static void Play(AudioClip clip)
        {
            Play(clip, 1f);
        }

        /// <summary>
        /// 播放音效剪辑
        /// 参数clip : 放入你要播放的音源
        /// 参数volume : 声音大小调节
        /// </summary>
        public static AudioSource Play(AudioClip clip, float volume)
        {
            if (clip == null)
                return null;
            for (int i = 0; i < Instance.sources.Count; ++i)
            {
                if (!Instance.sources[i].isPlaying)//如果音效剪辑存在 并且 音效没有被播放 则可以执行播放音效
                {
                    Instance.sources[i].PlayOneShot(clip, volume);
                    return Instance.sources[i];
                }
            }
            AudioSource source = Instance.gameObject.AddComponent<AudioSource>();
            Instance.sources.Add(source);
            source.clip = clip;
            source.volume = volume;
            source.PlayOneShot(clip, volume);
            return source;
        }

        /// <summary>
        /// 当音效播放完成销毁AudioSource组件
        /// 参数clip : 放入你要播放的音源
        /// </summary>
        public static AudioSource OnPlayingDestroy(AudioClip clip)
        {
            return OnPlayingDestroy(clip, 1f); ;
        }

        /// <summary>
        /// 当音效播放完成销毁AudioSource组件
        /// 参数clip : 放入你要播放的音源
        /// 参数volume : 声音大小调节
        /// </summary>
        public static AudioSource OnPlayingDestroy(AudioClip clip, float volume)
        {
            AudioSource source = Instance.gameObject.AddComponent<AudioSource>();
            Instance.destroyPlayingSources.Add(source);
            source.volume = volume;
            source.clip = clip;
            source.PlayOneShot(clip);
            return source;
        }

        /// <summary>
        /// 当音效播放完成销毁AudioSource组件
        /// 参数clip : 放入你要播放的音源
        /// 参数source : 音频源组件
        /// </summary>
        public static void OnPlayingDestroy(AudioSource source, AudioClip clip)
        {
            Instance.destroyPlayingSources.Add(source);
            source.clip = clip;
            source.PlayOneShot(clip);
        }
    }
}