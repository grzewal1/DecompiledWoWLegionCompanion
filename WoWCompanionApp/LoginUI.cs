using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowJamMessages.JSONRealmList;

namespace WoWCompanionApp
{
	public class LoginUI : MonoBehaviour
	{
		public ConnectingPanel m_connectingPanel;

		public Button m_connectingPanelCancelButton;

		public RealmListPanel m_realmListPanel;

		public CharacterListPanel m_characterListPanel;

		public CharacterListPanel m_characterListPopup;

		public DownloadingPanel m_downloadingPanel;

		public CreateNewLoginPanel m_createNewLoginPanel;

		public TitlePanel m_titlePanel;

		public WebAuthPanel m_webAuthPanel;

		public RecentCharacterArea m_recentCharacterArea;

		public GenericPopup m_genericPopup;

		public GameObject m_commonLegionWallpaper;

		private bool shouldShowConnectingPanel;

		public WoWCompanionApp.RealmListView RealmListView
		{
			get
			{
				return this.m_realmListPanel.m_realmListView;
			}
		}

		public LoginUI()
		{
		}

		public void AddCharacterButton(JamJSONCharacterEntry charData, string subRegion, string realmName, bool online)
		{
			if (this.m_characterListPanel != null)
			{
				this.m_characterListPanel.m_characterListView.AddCharacterButton(charData, subRegion, realmName, online);
			}
			if (this.m_characterListPopup != null)
			{
				this.m_characterListPopup.m_characterListView.AddCharacterButton(charData, subRegion, realmName, online);
			}
		}

		public void CancelRegionIndex()
		{
			this.m_titlePanel.CancelRegionIndex();
		}

		public void ClearCharacterList()
		{
			if (this.m_characterListPanel != null)
			{
				this.m_characterListPanel.m_characterListView.ClearList();
			}
		}

		public void CloseCharacterListDialog()
		{
			Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
			Main.instance.m_backButtonManager.PopBackAction();
			this.m_characterListPopup.gameObject.SetActive(false);
		}

		private void HideAllPanels(bool hideConnecting = true)
		{
			if (this.m_titlePanel != null)
			{
				this.m_titlePanel.gameObject.SetActive(false);
			}
			if (this.m_realmListPanel != null)
			{
				this.m_realmListPanel.gameObject.SetActive(false);
			}
			if (this.m_createNewLoginPanel != null)
			{
				this.m_createNewLoginPanel.gameObject.SetActive(false);
			}
			if (this.m_downloadingPanel != null)
			{
				this.m_downloadingPanel.gameObject.SetActive(false);
			}
			if (this.m_characterListPanel != null)
			{
				this.m_characterListPanel.gameObject.SetActive(false);
			}
			if (this.m_webAuthPanel != null)
			{
				this.m_webAuthPanel.gameObject.SetActive(false);
			}
			if (hideConnecting)
			{
				if (this.m_connectingPanel != null)
				{
					this.m_connectingPanel.gameObject.SetActive(false);
				}
				if (this.m_commonLegionWallpaper != null)
				{
					this.m_commonLegionWallpaper.SetActive(false);
				}
				this.shouldShowConnectingPanel = false;
			}
		}

		public void HideAllPopups()
		{
			if (this.m_characterListPopup && this.m_characterListPopup.gameObject.activeInHierarchy)
			{
				this.CloseCharacterListDialog();
			}
			this.m_genericPopup.gameObject.SetActive(false);
		}

		public void HideConnectingPanel()
		{
			this.m_connectingPanel.gameObject.SetActive(false);
			this.shouldShowConnectingPanel = false;
		}

		public void HidePanelsForWebAuth()
		{
			this.HideAllPanels(false);
		}

		public void HideWebAuthPanel()
		{
			this.m_webAuthPanel.gameObject.SetActive(false);
		}

		public bool IsGenericPopupShowing()
		{
			return this.m_genericPopup.gameObject.activeSelf;
		}

		public bool IsShowingCharacterListPanel()
		{
			return (this.m_characterListPanel == null ? true : this.m_characterListPanel.gameObject.activeSelf);
		}

		public bool IsShowingRealmListPanel()
		{
			return this.m_realmListPanel.gameObject.activeSelf;
		}

		public void OnAccountSelectButtonClicked()
		{
			Singleton<Login>.Instance.OnClickTitleConnect();
		}

		public void OnClickCharacterSelectCancel()
		{
			Singleton<Login>.instance.OnClickCharacterSelectCancel();
		}

		public void OnClickConnectingCancel()
		{
			Singleton<Login>.Instance.OnClickConnectingCancel();
		}

		public void OnLoginButtonClicked()
		{
			Singleton<Login>.Instance.OnClickTitleResume();
		}

		public void ReturnToTitleScene()
		{
			Singleton<Login>.Instance.ReturnToCharacterList = true;
			Singleton<Login>.Instance.ReturnToTitleScene();
		}

		public void SetConnectingPanelCancelButtonEnabled(bool enabled)
		{
			if (this.m_connectingPanelCancelButton != null)
			{
				this.m_connectingPanelCancelButton.gameObject.SetActive(enabled);
			}
		}

		public void SetRecentCharacter(int index, RecentCharacter recentChar)
		{
			if (this.m_recentCharacterArea != null)
			{
				this.m_recentCharacterArea.SetRecentCharacter(index, recentChar);
			}
		}

		public void SetRegionIndex()
		{
			this.m_titlePanel.SetRegionIndex();
		}

		public void ShowCharacterListPanel()
		{
			this.m_connectingPanel.Hide();
			this.m_realmListPanel.gameObject.SetActive(false);
			this.m_characterListPanel.gameObject.SetActive(true);
			this.ShowLegionBackground();
		}

		public void ShowConnectingPanel()
		{
			if (this.m_connectingPanel.gameObject.activeSelf)
			{
				return;
			}
			this.HideAllPanels(true);
			this.ShowLegionBackground();
			if (!StaticDB.StringsAvailable())
			{
				this.shouldShowConnectingPanel = true;
				return;
			}
			this.m_connectingPanel.gameObject.SetActive(true);
			this.shouldShowConnectingPanel = false;
		}

		public void ShowCreateNewLoginPanel()
		{
		}

		public void ShowDownloadingPanel(bool show)
		{
			if (show == this.m_downloadingPanel.gameObject.activeSelf)
			{
				return;
			}
			if (show)
			{
				this.HideAllPanels(true);
				this.ShowLegionBackground();
			}
			this.m_downloadingPanel.gameObject.SetActive(show);
		}

		public void ShowGenericPopup(string headerText, string descriptionText)
		{
			this.HideAllPopups();
			this.m_genericPopup.SetText(headerText, descriptionText);
			this.m_genericPopup.gameObject.SetActive(true);
		}

		public void ShowGenericPopupFull(string fullText)
		{
			this.HideAllPopups();
			this.m_genericPopup.SetFullText(fullText);
			this.m_genericPopup.gameObject.SetActive(true);
		}

		private void ShowLegionBackground()
		{
			if (this.m_commonLegionWallpaper != null)
			{
				this.m_commonLegionWallpaper.SetActive(true);
			}
		}

		public void ShowLogoutConfirmationPopup(bool goToWebAuth)
		{
			Singleton<DialogFactory>.Instance.CreateOKCancelDialog((!goToWebAuth ? "LOG_OUT" : "ACCOUNT_SELECTION"), "ARE_YOU_SURE", () => Singleton<Login>.instance.OnLogoutConfirmed(goToWebAuth), new Action(Singleton<Login>.instance.OnLogoutCancel));
		}

		public void ShowRealmListPanel()
		{
			this.HideAllPanels(true);
			this.m_realmListPanel.gameObject.SetActive(true);
			this.ShowLegionBackground();
		}

		public void ShowRegionConfirmationPopup(int index)
		{
			Singleton<DialogFactory>.Instance.CreateOKCancelDialog("RESTART_REQUIRED", "ARE_YOU_SURE", new Action(Singleton<Login>.instance.SetRegionIndex), new Action(Singleton<Login>.instance.CancelRegionIndex));
		}

		public void ShowTitlePanel()
		{
			this.HideAllPanels(true);
			if (this.m_titlePanel != null)
			{
				this.m_titlePanel.gameObject.SetActive(true);
			}
			this.ShowLegionBackground();
			if (this.m_titlePanel != null)
			{
				this.m_titlePanel.UpdateResumeButtonVisiblity();
			}
		}

		public void ShowWebAuthPanel()
		{
			this.HideAllPanels(true);
			this.m_commonLegionWallpaper.SetActive(true);
			this.m_webAuthPanel.gameObject.SetActive(true);
		}

		public void SortCharacterList()
		{
			if (this.m_characterListPanel != null)
			{
				this.m_characterListPanel.m_characterListView.SortCharacterList();
			}
		}

		private void Start()
		{
			if (this.m_connectingPanel != null)
			{
				this.m_connectingPanelCancelButton = this.m_connectingPanel.GetComponentInChildren<Button>();
			}
		}

		private void Update()
		{
			if (this.shouldShowConnectingPanel && StaticDB.StringsAvailable())
			{
				this.ShowConnectingPanel();
			}
			this.SetConnectingPanelCancelButtonEnabled(Singleton<Login>.Instance.CanCancelNow());
		}
	}
}