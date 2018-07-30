using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CreateNewLoginPanel : MonoBehaviour
	{
		public InputField newLoginNameEditText;

		public InputField newLoginDataEditText;

		public GameObject loginListItemPrefab;

		public GameObject loginListContents;

		public CreateNewLoginPanel()
		{
		}

		public void SaveNewLogin()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.loginListItemPrefab);
			gameObject.transform.SetParent(this.loginListContents.transform, false);
			LoginButton component = gameObject.GetComponent<LoginButton>();
			component.m_loginButtonNameText.text = this.newLoginNameEditText.text;
			if (!this.newLoginDataEditText.text.StartsWith("http://localhost:0/?ST="))
			{
				component.m_token = this.newLoginDataEditText.text;
			}
			else
			{
				component.m_token = this.newLoginDataEditText.text.Substring(23);
			}
			bool flag = false;
			int num = 0;
			while (num < 10)
			{
				string str = SecurePlayerPrefs.GetString(string.Concat("DevAccount", num), Main.uniqueIdentifier);
				if (str == null || str != this.newLoginNameEditText.text)
				{
					num++;
				}
				else
				{
					SecurePlayerPrefs.SetString(string.Concat("DevToken", num), component.m_token, Main.uniqueIdentifier);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				int num1 = 0;
				while (num1 < 10)
				{
					string str1 = SecurePlayerPrefs.GetString(string.Concat("DevAccount", num1), Main.uniqueIdentifier);
					string str2 = SecurePlayerPrefs.GetString(string.Concat("DevToken", num1), Main.uniqueIdentifier);
					if ((str1 == null || str1 == string.Empty) && (str2 == null || str2 == string.Empty))
					{
						SecurePlayerPrefs.SetString(string.Concat("DevAccount", num1), this.newLoginNameEditText.text, Main.uniqueIdentifier);
						SecurePlayerPrefs.SetString(string.Concat("DevToken", num1), component.m_token, Main.uniqueIdentifier);
						break;
					}
					else
					{
						num1++;
					}
				}
			}
			Singleton<Login>.instance.LoginUI.ShowRealmListPanel();
		}
	}
}