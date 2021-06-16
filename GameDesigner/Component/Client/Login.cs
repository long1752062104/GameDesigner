#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Net.Component
{
    using Net.Client;
    using Net.Share;
    using UnityEngine.UI;

    public class Login : NetBehaviour
    {
        public InputField acc, pwd;
        public Button logBtn;

        // Start is called before the first frame update
        void Start()
        {
            logBtn.onClick.AddListener(() =>
            {
                if (acc.text.Length <= 0 | pwd.text.Length <= 0)
                {
                    MessageBox.Show("登录错误!");
                    return;
                }
                Send("Login", acc.text, pwd.text);
            });
        }

        [Rpc]
        void LoginCallback(bool result, string info)
        {
            if (result)
            {
                gameObject.SetActive(false);
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            }
            else MessageBox.Show(info);
        }
    }
}
#endif