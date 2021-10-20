#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using global::System;
    using global::System.Reflection;
    using UnityEngine;

    public class UIPanelInit : MonoBehaviour
    {
        void Awake()
        {
            var monos = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            foreach (var mono in monos)
            {
                var method = mono.GetType().GetMethod("Init", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null)
                {
                    try
                    {
                        if (method.GetParameters().Length == 0)
                            method.Invoke(mono, null);
                    }
                    catch (Exception ex) { Debug.Log("错误物体名称:" + mono.name + " : " + ex); }
                }
            }
        }
    }
}
#endif