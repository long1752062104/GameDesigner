#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using Net.Share;
using UnityEngine;
using UnityEngine.UI;

public class UITest : SingleCase<UITest>
{
    public InputField cri;
    public Button cr, jr, er, rb, eb;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        cr.onClick.AddListener(()=> {
            ClientManager.Instance.SendRT("CreateRoom", cri.text);
        });
        jr.onClick.AddListener(() => {
            ClientManager.Instance.SendRT("JoinRoom", cri.text);
        });
        er.onClick.AddListener(() => {
            ClientManager.Instance.SendRT("ExitRoom");
        });
        rb.onClick.AddListener(() => {
            ClientManager.Instance.SendRT("StartBattle");
        });
        eb.onClick.AddListener(() => {
            ClientManager.Instance.SendRT("ExitBattle");
        });
        ClientManager.Instance.client.AddRpcHandle(this);
        ClientManager.Instance.client.OnPingCallback += (delay) => {
            text.text = "网络延迟:" + delay;
        };
    }

    [Rpc]
    void CreateRoomCallback(string str) 
    {
        Debug.Log(str);
    }

    [Rpc]
    void JoinRoomCallback(string str)
    {
        Debug.Log(str);
    }

    [Rpc]
    void ExitRoomCallback(string str)
    {
        Debug.Log(str);
    }

    [Rpc]
    void StartGameSync()
    {
        Debug.Log("开始帧同步!");
    }
}
#endif