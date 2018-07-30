using bnet.protocol;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class BnGameAccountButton : MonoBehaviour
	{
		public Text m_buttonText;

		private EntityId m_gameAccount;

		private string m_accountName;

		public BnGameAccountButton()
		{
		}

		public void OnClick()
		{
			Singleton<Login>.instance.SelectGameAccount(this.m_gameAccount);
		}

		public void PlayClickSound()
		{
			Main.instance.m_UISound.Play_ButtonBlackClick();
		}

		public void SetInfo(EntityId gameAccount, string accountName, bool isBanned, bool isSuspended)
		{
			this.m_gameAccount = gameAccount;
			this.m_accountName = accountName;
			this.m_buttonText.text = this.m_accountName;
		}
	}
}