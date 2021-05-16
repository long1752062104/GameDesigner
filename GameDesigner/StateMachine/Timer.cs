using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameDesigner
{
    /// <summary>
    /// 计时器
    /// </summary>
	[Serializable]
    public class Timer
    {
        /// <summary>
        /// 当前时间
        /// </summary>
		public float Time;
        [SerializeField]
        private float timeMax;

        /// <summary>
        /// 结束时间
        /// </summary>
        public float EndTime
        {
            get
            {
                if (timeMax <= 0)
                {
                    timeMax = 1.0F;
                }
                return timeMax;
            }
            set { timeMax = value; }
        }

        /// <summary>
        /// 构造计时器
        /// </summary>
        /// <param name="timeMax">结束时间</param>
		public Timer(float timeMax)
        {
            Time = 0;
            this.timeMax = timeMax;
        }

        /// <summary>
        /// 构造计时器
        /// </summary>
        /// <param name="_time">当前时间</param>
        /// <param name="timeMax">结束时间</param>
		public Timer(float _time, float timeMax)
        {
            Time = _time;
            this.timeMax = timeMax;
        }

        private static Dictionary<int, Timer> timers = new Dictionary<int, Timer>();
        private static Dictionary<string, Timer> timer1s = new Dictionary<string, Timer>();

        /// <summary>
        /// 是否到达指定时间 key必须是独有的,否则将会造成共用计时器
        /// </summary>
        public static bool SpecifiedTime(int key, float timeMax)
        {
            if (!timers.ContainsKey(key))
            {
                timers.Add(key, new Timer(timeMax));
            }
            return timers[key].UpdateTime(timeMax, true);
        }

        /// <summary>
        /// 开始定时，定时键值不可一样，否则将会同用一个定时器
        /// </summary>
        public static bool StartTiming(string key, float time)
        {
            if (!timer1s.ContainsKey(key))
            {
                timer1s.Add(key, new Timer(time));
            }
            return timer1s[key].UpdateTime(time, true);
        }

        /// <summary>
        /// 获得时间是否到达时间的最大值 , 到达后m_Time归零
        /// </summary>
        public bool IsTimeOut
        {
            get { return UpdateTime(); }
        }

        /// <summary>
        /// 当时间到达时间的最大值后返回真 , 到达后m_Time不归零
        /// </summary>
        public bool OnTimeOut
        {
            get { return UpdateTime(false); }
        }

        /// <summary>
        /// 计时器是否到达最大值,到达则不再计时
        /// </summary>
        public bool IsOutTime
        {
            get
            {
                if (Time >= timeMax)
                    return true;
                Time += UnityEngine.Time.deltaTime;
                return false;
            }
        }

        /// <summary>
        /// 如果时间大于定义的时间则返回真,zero参数为真则时间大于定义的时间后归零
        /// </summary>
        public bool UpdateTime(bool zero = true)
        {
            if (Time >= timeMax)
            {
                Time = zero ? 0 : Time;
                return true;
            }
            Time += UnityEngine.Time.deltaTime;
            return false;
        }

        /// <summary>
        /// 如果时间大于定义的时间则返回真,zero参数为真则时间大于定义的时间后归零
        /// </summary>
        public bool UpdateTime(float timeMax, bool zero = true)
        {
            if (Time >= timeMax)
            {
                Time = zero ? 0 : Time;
                return true;
            }
            Time += UnityEngine.Time.deltaTime;
            return false;
        }
    }
}