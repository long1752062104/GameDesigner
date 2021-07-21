using Example2;
using MVC.View;
using Net.Component;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hotfix
{
    public class GameEntry
    {
        public static void Init()//热更新初始化入口--调用于主工程的GameInit组件
        {
            var pl = Object.FindObjectOfType<PanelLevel>();//得到面板层级组件

            var login = (GameObject)Object.Instantiate(Resources.Load("Panel/LoginPanel"), pl.levels[0]);//实例化登录界面
            LoginPanel.Instance.Init(login.GetComponent<FieldCollection>());//初始化登录界面输入框,按钮

            var register = (GameObject)Object.Instantiate(Resources.Load("Panel/RegisterPanel"), pl.levels[0]);//实例化注册界面
            RegisterPanel.Instance.Init(register.GetComponent<FieldCollection>());//初始化注册界面输入框,按钮
            RegisterPanel.Hide();//隐藏注册界面

            var msgPanel = (GameObject)Object.Instantiate(Resources.Load("Panel/MsgPanel"), pl.levels[2]);//实例化消息框, 在层级面板最前面
            MsgPanel.Instance.Init(msgPanel.GetComponent<FieldCollection>());//初始化消息框界面
            MsgPanel.Hide();//隐藏消息框

            GameEvent.OnPlayerDead += OnPlayerDead;
        }

        private static void OnPlayerDead()
        {
            MsgPanel.Show("死亡", "是否复活!", (r)=> {
                if (r)
                {
                    ClientManager.AddOperation(new Net.Share.Operation(Example2.Command.Resurrection, ClientManager.UID));
                }
                else 
                {

                }
            });
        }

        public static void Update()
        { 
            
        }
    }
}
