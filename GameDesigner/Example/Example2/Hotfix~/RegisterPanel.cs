using MVC.View;
using Net.Client;
using Net.Component;
using Net.Share;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
	//热更新生成的脚本, 请看gitee的mvc模块使用介绍图示
	public class RegisterPanel
	{
		public static RegisterPanel Instance = new RegisterPanel();
		public GameObject panel;
		public InputField acc;
		public InputField email;
		public InputField pwd;
		public Button login;
		public Button Btn_Close;
		public Button register;

		public void Init(FieldCollection fc)
		{
			panel = fc.gameObject;
			acc = fc["acc"].target as InputField;
			email = fc["email"].target as InputField;
			pwd = fc["pwd"].target as InputField;
			login = fc["login"].target as Button;
			Btn_Close = fc["Btn_Close"].target as Button;
			register = fc["register"].target as Button;
			login.onClick.AddListener(() => {
				Hide();
			});
			Btn_Close.onClick.AddListener(() => {
				Hide();
			});
			register.onClick.AddListener(() => {
				if (acc.text.Length <= 0 | pwd.text.Length <= 0)
				{
					MsgPanel.Show("注册错误!");
					return;
				}
				ClientManager.Instance.SendRT("Register", acc.text, pwd.text);
			});
			ClientManager.Instance.client.Add_ILR_RpcHandle(this);//收集ilr的rpc方法
		}

		internal static void Show()
		{
			Instance.panel.SetActive(true);
		}

		internal static void Hide()
		{
			Instance.panel.SetActive(false);
		}

		[Rpc]
		void RegisterCallback(string info)
		{
			MsgPanel.Show(info);
		}
	}
}