using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class LoginButton : MonoBehaviour
	{
		public Text m_loginButtonNameText;

		public string m_mobileServerAddress;

		public int m_mobileServerPort;

		public string m_bnetAccount;

		public string m_characterID;

		public ulong m_virtualRealmAddress;

		public string m_wowAccount;

		public string m_token;

		public LoginButton()
		{
		}

		public void DeleteMe()
		{
			int num = 0;
			while (num < 10)
			{
				string str = SecurePlayerPrefs.GetString(string.Concat("DevAccount", num), Main.uniqueIdentifier);
				string str1 = SecurePlayerPrefs.GetString(string.Concat("DevToken", num), Main.uniqueIdentifier);
				if (str == null || !(str == this.m_loginButtonNameText.text) || str1 == null && !(str1 == this.m_token))
				{
					num++;
				}
				else
				{
					SecurePlayerPrefs.DeleteKey(string.Concat("DevAccount", num));
					SecurePlayerPrefs.DeleteKey(string.Concat("DevToken", num));
					break;
				}
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public void LogIn()
		{
			SecurePlayerPrefs.SetString("WebToken", this.m_token, Main.uniqueIdentifier);
			Singleton<Login>.instance.StartCachedLogin(false, false);
		}

		public void PlayClickSound()
		{
			Main.instance.m_UISound.Play_ButtonBlackClick();
		}

		private void Start()
		{
			this.m_loginButtonNameText.font = GeneralHelpers.LoadStandardFont();
		}

		private void Update()
		{
		}
	}
}