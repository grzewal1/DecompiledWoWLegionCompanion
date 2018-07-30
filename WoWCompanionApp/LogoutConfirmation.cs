using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;

namespace WoWCompanionApp
{
	public class LogoutConfirmation : MonoBehaviour
	{
		public Text m_logoutText;

		public Text m_sureText;

		public Text m_okayText;

		public Text m_cancelText;

		public bool GoToWebAuth
		{
			get;
			set;
		}

		public LogoutConfirmation()
		{
		}

		public void OnClickCancel()
		{
			Singleton<Login>.instance.OnLogoutCancel();
		}

		public void OnClickOkay()
		{
			Singleton<Login>.instance.OnLogoutConfirmed(this.GoToWebAuth);
		}

		private void OnDisable()
		{
			Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
			Main.instance.m_backButtonManager.PopBackAction();
		}

		private void OnEnable()
		{
			if (!this.GoToWebAuth)
			{
				this.m_logoutText.text = StaticDB.GetString("LOG_OUT", null);
			}
			else
			{
				this.m_logoutText.text = StaticDB.GetString("ACCOUNT_SELECTION", null);
			}
			this.m_sureText.text = StaticDB.GetString("ARE_YOU_SURE", null);
			this.m_okayText.text = StaticDB.GetString("OK", null);
			this.m_cancelText.text = StaticDB.GetString("CANCEL", null);
			Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
			Main.instance.m_backButtonManager.PushBackAction(BackActionType.hideAllPopups, null);
		}
	}
}