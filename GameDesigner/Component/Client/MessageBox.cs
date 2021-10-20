#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using global::System;
    using UnityEngine.UI;

    public class MessageBox : SingleCase<MessageBox>
    {
        public Text tips, info;
        public Button confirm, cancel;
        public Action<bool> action;

        // Use this for initialization
        void Start()
        {
            instance = this;
            confirm.onClick.AddListener(() =>
            {
                action?.Invoke(true);
                action = null;
                Hide();
            });
            cancel.onClick.AddListener(() =>
            {
                action?.Invoke(false);
                action = null;
                Hide();
            });
        }

        public static void Show(string info)
        {
            Show("消息", info, null);
        }

        public static void Show(string tips, string info)
        {
            Show(tips, info, null);
        }

        public static void Show(string tips, string info, Action<bool> action)
        {
            I.tips.text = tips;
            I.info.text = info;
            I.action = action;
            I.gameObject.SetActive(true);
        }
    }
}
#endif