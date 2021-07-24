using Net.Client;
using Net.Component;
using Net.Share;
using UnityEngine;
using UnityEngine.UI;

public class InjectUI : NetBehaviour
{
    public Button btn, btn1, btn2, btn3, btn4;

    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(() => {
            LocalSync("玩家ID:" + ClientManager.UID + " 发送的自身同步!");
        });
        btn1.onClick.AddListener(()=> {
            SceneSync("玩家ID:" + ClientManager.UID + " 发送的场景同步! 请开两个unity才会看到效果");
        });
        btn2.onClick.AddListener(() => {
            ServerCallBack("玩家ID:" + ClientManager.UID + " 请求的服务器回调!");//此方法需要开发者在服务器调用SendRT(client, "ServerCallBack", "服务器回调!");
        });
        btn3.onClick.AddListener(() => {
            BugParType(gameObject, gameObject, transform);
        });
        btn4.onClick.AddListener(() => {
            BugParNull(null);
        });
    }

    [Rpc(cmd = NetCmd.Local)]
    void LocalSync(string msg)
    {
        Debug.Log(msg);
    }

    [Rpc(cmd = NetCmd.Scene)]
    void SceneSync(string msg) 
    {
        Debug.Log(msg);
    }

    [Rpc]
    void ServerCallBack(string msg)//此方法需要服务器回调, 大概服务器代码 SendRT(client, "ServerCallBack", "服务器回调!");
    {
        Debug.Log(msg);
    }

    [Rpc]
    void BugParType(GameObject obj, Object obj1, Transform tran)//不允许发送Unity的类型, 但允许unity的大部分结构类型. 如:Vector2,3,4,Rect,Quaternion
    {
        Debug.Log("错误参数,不会调用!");
    }

    [Rpc]
    void BugParNull(RoomData data)
    {
        Debug.Log(data);
    }
}
