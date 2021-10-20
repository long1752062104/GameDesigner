#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using global::System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 界面面板单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="ITEM"></typeparam>
    public abstract class SingleCasePanel<T, ITEM> : SingleCase<T> where T : MonoBehaviour
    {
        public ITEM item;
        public Transform root;
        public List<ITEM> items = new List<ITEM>();
    }
}
#endif