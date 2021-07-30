#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
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
                    foreach (var t in ts)
                    {
                        if (t.gameObject.scene.isLoaded) 
                        {
                            instance = t;
                            break;
                        }
                    }
                }
                return instance;
            }
            set { instance = value; }
        }
        /// <summary>
        /// 单例实例
        /// </summary>
        public static T I
        {
            get { return Instance; }
            set { instance = value; }
        }

        public static T Show()
        {
            I.gameObject.SetActive(true);
            return I;
        }

        public static void Hide()
        {
            I.gameObject.SetActive(false);
        }
    }
}
#endif