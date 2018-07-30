using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class BnLoginButton : MonoBehaviour
	{
		public Text m_loginButtonNameText;

		public Text m_numCharactersText;

		public Image m_realmStatusIcon;

		private ulong m_realmAddress;

		private string m_realmName;

		private string m_subRegion;

		private int m_characterCount;

		private bool m_enabled;

		private static Color s_disabledColor;

		private static Color s_enabledColor;

		private static Color s_enabledCharacterCountColor;

		static BnLoginButton()
		{
			BnLoginButton.s_disabledColor = new Color(0.3f, 0.3f, 0.3f);
			BnLoginButton.s_enabledColor = new Color(1f, 1f, 1f);
			BnLoginButton.s_enabledCharacterCountColor = new Color(1f, 0.86f, 0f);
		}

		public BnLoginButton()
		{
		}

		public ulong GetRealmAddress()
		{
			return this.m_realmAddress;
		}

		public void OnClick()
		{
			Singleton<Login>.instance.SendRealmJoinRequest(this.m_realmName, this.m_realmAddress, this.m_subRegion);
		}

		public void PlayClickSound()
		{
			Main.instance.m_UISound.Play_ButtonBlackClick();
		}

		public void SetCharacterCount(int characterCount)
		{
			this.m_characterCount = characterCount;
		}

		public void SetInfo(ulong realmAddress, string realmName, string subRegion, int characterCount, bool enabled)
		{
			this.m_realmAddress = realmAddress;
			this.m_realmName = realmName;
			this.m_subRegion = subRegion;
			this.m_characterCount = characterCount;
			this.m_enabled = enabled;
			this.UpdateOnlineStatus();
		}

		public void SetOnline(bool online)
		{
			this.m_enabled = online;
			this.UpdateOnlineStatus();
		}

		private void Start()
		{
			this.m_loginButtonNameText.font = GeneralHelpers.LoadStandardFont();
			this.m_numCharactersText.font = GeneralHelpers.LoadStandardFont();
		}

		private void UpdateOnlineStatus()
		{
			if (!this.m_enabled)
			{
				this.m_loginButtonNameText.text = string.Concat(this.m_realmName, " (", StaticDB.GetString("OFFLINE", null), ")");
				this.m_loginButtonNameText.color = BnLoginButton.s_disabledColor;
				this.m_numCharactersText.text = string.Concat(string.Empty, this.m_characterCount);
				this.m_numCharactersText.color = BnLoginButton.s_disabledColor;
			}
			else
			{
				this.m_loginButtonNameText.text = this.m_realmName;
				this.m_loginButtonNameText.color = BnLoginButton.s_enabledColor;
				this.m_numCharactersText.text = string.Concat(string.Empty, this.m_characterCount);
				this.m_numCharactersText.color = BnLoginButton.s_enabledCharacterCountColor;
			}
			base.GetComponent<Button>().interactable = this.m_enabled;
			if (!this.m_enabled)
			{
				this.m_realmStatusIcon.sprite = Resources.Load<Sprite>("NewLoginPanel/Realm_StatusRed");
			}
			else
			{
				this.m_realmStatusIcon.sprite = Resources.Load<Sprite>("NewLoginPanel/Realm_StatusGreen");
			}
		}
	}
}