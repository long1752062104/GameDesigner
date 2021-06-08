#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component.Client
{
    using Net.Share;
    using System;
    using UnityEngine.UI;

    public class MessageBox : SingleCase<MessageBox>
    {
        public Text tips, info;
        public Button confirm, cancel;
        public Action<bool> action;

        // Use this for initialization
        void Init()
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
            FindObjectOfType<ClientManager>().client.AddRpcHandle(this);
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
            instance.tips.text = tips;
            instance.info.text = info;
            instance.action = action;
            instance.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            instance.gameObject.SetActive(false);
        }

        [rpc]
        void ShowInfo(string msg)
        {
            Show(msg);
        }
    }
}
#endif