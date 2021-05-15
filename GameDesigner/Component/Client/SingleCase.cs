#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
namespace Net.Component.Client
{
    using UnityEngine;

    /// <summary>
    /// MonoBehaviour 单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingleCase<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// 由于第一次判断实例是否为空的时候, 如果直接使用Instance会进行查找类型, 单例被被赋值, 所以有必要的时候这个静态字段要用到
        /// </summary>
        protected static T instance;
        /// <summary>
        /// 单例实例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    var ts = Resources.FindObjectsOfTypeAll<T>();
                    if (ts.Length > 0)
                        instance = ts[0];
                }
                return instance;
            }
            set { instance = value; }
        }

        public static void Show()
        {
            Instance.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            Instance.gameObject.SetActive(false);
        }
    }
}
#endif