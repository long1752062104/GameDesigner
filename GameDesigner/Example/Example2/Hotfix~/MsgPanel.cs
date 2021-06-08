using MVC.View;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
	public class MsgPanel
	{
		public static MsgPanel Instance = new MsgPanel();
		public GameObject panel;
		public Text TextContent;
		public Text Title;
		public Button Btn_No;
		public Button Btn_Yes;
		public Button Btn_ClosePopup2;

		public Action<bool> action;

		public void Init(FieldCollection fc)
		{
			panel = fc.gameObject;
			TextContent = fc["TextContent"].target as Text;
			Title = fc["Title"].target as Text;
			Btn_No = fc["Btn_No"].target as Button;
			Btn_Yes = fc["Btn_Yes"].target as Button;
			Btn_ClosePopup2 = fc["Btn_ClosePopup2"].target as Button;
			Btn_No.onClick.AddListener(() => {
				action?.Invoke(false);
				action = null;
				Hide();
			});
			Btn_Yes.onClick.AddListener(() => {
				action?.Invoke(true);
				action = null;
				Hide();
			});
			Btn_ClosePopup2.onClick.AddListener(() => {
				action = null;
				Hide();
			});
		}

		public static void Show(string info)
		{
			Show("ÏûÏ¢", info, null);
		}

		public static void Show(string tips, string info)
		{
			Show(tips, info, null);
		}

		public static void Show(string tips, string info, Action<bool> action)
		{
			Instance.Title.text = tips;
			Instance.TextContent.text = info;
			Instance.action = action;
			Instance.panel.SetActive(true);
		}

		internal static void Hide()
		{
			Instance.panel.SetActive(false);
		}
	}
}