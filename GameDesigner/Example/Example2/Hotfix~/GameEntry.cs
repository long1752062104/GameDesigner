using Example2;
using MVC.View;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hotfix
{
    public class GameEntry
    {
        public static void Init()
        {
            var pl = Object.FindObjectOfType<PanelLevel>();

            var login = (GameObject)Object.Instantiate(Resources.Load("Panel/LoginPanel"), pl.levels[0]);
            LoginPanel.Instance.Init(login.GetComponent<FieldCollection>());

            var register = (GameObject)Object.Instantiate(Resources.Load("Panel/RegisterPanel"), pl.levels[0]);
            RegisterPanel.Instance.Init(register.GetComponent<FieldCollection>());
            RegisterPanel.Hide();

            var msgPanel = (GameObject)Object.Instantiate(Resources.Load("Panel/MsgPanel"), pl.levels[2]);
            MsgPanel.Instance.Init(msgPanel.GetComponent<FieldCollection>());
            MsgPanel.Hide();
        }

        public static void Update()
        { 
            
        }
    }
}
