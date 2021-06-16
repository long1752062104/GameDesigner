#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Client;
    using Net.Share;
    using UnityEngine.UI;

    public class Register : NetBehaviour
    {
        public InputField acc, pwd;
        public Button regBtn;

        // Start is called before the first frame update
        void Start()
        {
            regBtn.onClick.AddListener(() =>
            {
                if (acc.text.Length <= 0 | pwd.text.Length <= 0)
                {
                    MessageBox.Show("注册错误!");
                    return;
                }
                Send("Register", acc.text, pwd.text);
            });
        }

        [Rpc]
        void RegisterCallback(string info)
        {
            MessageBox.Show(info);
        }
    }
}
#endif
