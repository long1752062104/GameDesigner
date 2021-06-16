using System.Collections.Generic;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 动画播放模式
    /// </summary>
	public enum AnimPlayMode
    {
        /// <summary>
        /// 随机播放动画
        /// </summary>
		Random,
        /// <summary>
        /// 顺序播放动画
        /// </summary>
		Sequence,
    }

    /// <summary>
    /// 音效模式
    /// </summary>
	public enum AudioMode
    {
        /// <summary>
        /// 进入状态播放音效
        /// </summary>
		EnterPlay,
        /// <summary>
        /// 动画事件播放音效
        /// </summary>
		AnimEvent,
        /// <summary>
        /// 退出状态播放音效
        /// </summary>
		ExitPlay
    }

    /// <summary>
    /// 技能生成模式
    /// </summary>
	public enum ActiveMode
    {
        /// <summary>
        /// 实例化
        /// </summary>
		Instantiate,
        /// <summary>
        /// 对象池
        /// </summary>
		ObjectPool
    }

    /// <summary>
    /// 技能物体设置模式
    /// </summary>
	public enum SpwanMode
    {
        /// <summary>
        /// 设置技能物体在自身位置
        /// </summary>
		localPosition,
        /// <summary>
        /// 设置技能物体在父对象位置和成为父对象的子物体
        /// </summary>
		SetParent,
        /// <summary>
        /// 设置技能物体在父对象位置
        /// </summary>
		SetInTargetPosition,
    }

    /// <summary>
    /// ARPG状态动作
    /// </summary>
	[System.Serializable]
    public sealed class StateAction
    {
        /// <summary>
        /// 动画剪辑名称
        /// </summary>
		public string clipName = "";
        /// <summary>
        /// 动画剪辑索引
        /// </summary>
		public int clipIndex = 0;
        /// <summary>
        /// 当前动画时间
        /// </summary>
		public float animTime = 0;
        /// <summary>
        /// 动画事件时间
        /// </summary>
		public float animEventTime = 50f;
        /// <summary>
        /// 动画结束时间
        /// </summary>
		public float animTimeMax = 100;
        /// <summary>
        /// 是否已到达事件时间或超过事件时间，到为true，没到为flase
        /// </summary>
		public bool eventEnter = false;
        /// <summary>
        /// 技能粒子物体
        /// </summary>
		public GameObject effectSpwan = null;
        /// <summary>
        /// 粒子物体生成模式
        /// </summary>
		public ActiveMode activeMode = ActiveMode.Instantiate;
        /// <summary>
        /// 粒子物体销毁或关闭时间
        /// </summary>
		public float spwanTime = 1f;
        /// <summary>
        /// 粒子物体对象池
        /// </summary>
		public List<GameObject> activeObjs = new List<GameObject>();
        /// <summary>
        /// 粒子位置设置
        /// </summary>
		public SpwanMode spwanmode = SpwanMode.localPosition;
        /// <summary>
        /// 作为粒子挂载的父对象 或 作为粒子生成在此parent对象的位置
        /// </summary>
		public Transform parent = null;
        /// <summary>
        /// 粒子出生位置
        /// </summary>
		public Vector3 effectPostion = new Vector3(0, 1.5f, 2f);
        /// <summary>
        /// 粒子角度
        /// </summary>
        public Vector3 effectEulerAngles;
        /// <summary>
        /// 是否播放音效
        /// </summary>
        public bool isPlayAudio = false;
        /// <summary>
        /// 音效触发模式
        /// </summary>
		public AudioMode audioModel = AudioMode.AnimEvent;
        /// <summary>
        /// 音效剪辑
        /// </summary>
		public List<AudioClip> audioClips = new List<AudioClip>();
        /// <summary>
        /// ARPG动作行为
        /// </summary>
		public List<ActionBehaviour> behaviours = new List<ActionBehaviour>();

#if UNITY_EDITOR || DEBUG
        /// <summary>
        /// 编辑器脚本是否展开 编辑器音效是否展开 查找行为组件
        /// </summary>
        public bool foldout = true, audiofoldout = false, findBehaviours = false;
        /// <summary>
        /// 删除音效索引 行为脚本菜单索引
        /// </summary>
		public int desAudioIndex = 0, behaviourMenuIndex = 0;
        /// <summary>
        /// 创建脚本名称
        /// </summary>
        public string createScriptName = "NewStateAction";

        public int acSize = 1;
#endif
        public StateMachine stateMachine;
       
        /// <summary>
        /// 动作是否完成?, 当动画播放结束后为True, 否则为false
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return animTime >= animTimeMax - 1;
            }
        }

        /// <summary>
        /// 构造状态动作
        /// </summary>
        public StateAction()
        {
            audioClips = new List<AudioClip> {
                null
            };
        }
    }
}