#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace MVC.View
{
    using UnityEngine;

    public abstract class PanelBase<T> where T : PanelBase<T>, new()
    {
        public static T Instance;
        public GameObject panel;

        public abstract void Init(FieldCollection fc);

        public static void Show() { Instance.panel.SetActive(true); }

        public static void Hide() { Instance.panel.SetActive(false); }
    }
}
#endif