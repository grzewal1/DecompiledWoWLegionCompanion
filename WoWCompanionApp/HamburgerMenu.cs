using System;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;

namespace WoWCompanionApp
{
	public class HamburgerMenu : MonoBehaviour
	{
		public SlidingPanel m_slidingPanel;

		public Text m_characterName;

		public HamburgerMenu()
		{
		}

		public void CloseMenu()
		{
			this.m_slidingPanel.SlideOut();
			Main.instance.m_backButtonManager.PopBackAction();
			base.GetComponent<Image>().enabled = false;
			Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
		}

		private void HidePanel()
		{
			base.gameObject.SetActive(false);
		}

		public void OnLogoutButtonClicked()
		{
			Singleton<Login>.Instance.ReturnToTitleScene();
		}

		public void OpenMenu()
		{
			base.gameObject.SetActive(true);
			this.m_slidingPanel.SlideIn();
			Main.instance.m_backButtonManager.PushBackAction(BackActionType.hideHamburgerMenu, null);
			base.GetComponent<Image>().enabled = true;
			Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
		}

		private void Start()
		{
			this.m_slidingPanel.m_closedAction += new Action(this.HidePanel);
			this.m_characterName.text = Singleton<CharacterData>.instance.CharacterName.ToUpper();
		}
	}
}