using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class AllPanels : MonoBehaviour
	{
		public static AllPanels instance;

		public CreateNewLoginPanel createNewLoginPanel;

		public DownloadingPanel downloadingPanel;

		public FollowerListView followerListView_SlideIn_Bottom;

		public FollowerListView followerListView_SlideIn_Left;

		public AdventureMapPanel adventureMapPanel;

		public GameObject commonWoodBackground;

		public GameObject commonLegionWallpaper;

		public OrderHallMultiPanel m_orderHallMultiPanel;

		public RecentCharacterArea m_recentCharacterArea;

		public TroopsPanel m_troopsPanel;

		public CharacterViewPanel m_characterViewPanel;

		public CompanionMultiPanel m_companionMultiPanel;

		public AllPanels()
		{
		}

		private void Awake()
		{
			AllPanels.instance = this;
			if (!Singleton<AssetBundleManager>.instance.IsInitialized())
			{
				Singleton<AssetBundleManager>.instance.InitializedAction += new Action(this.OnAssetBundleManagerInitialized);
			}
			else
			{
				this.OnAssetBundleManagerInitialized();
			}
		}

		private void HideAllPanels(bool hideConnecting = true)
		{
			this.createNewLoginPanel.gameObject.SetActive(false);
			this.commonWoodBackground.SetActive(false);
			this.HideRecentCharacterPanel();
			if (hideConnecting)
			{
				this.commonLegionWallpaper.SetActive(false);
			}
			if (this.m_companionMultiPanel)
			{
				this.m_companionMultiPanel.gameObject.SetActive(false);
			}
			if (this.m_orderHallMultiPanel)
			{
				this.m_orderHallMultiPanel.gameObject.SetActive(false);
			}
		}

		private void HideRecentCharacterPanel()
		{
			if (AdventureMapPanel.instance != null)
			{
				AdventureMapPanel.instance.HideRecentCharacterPanel();
			}
		}

		public void OnAssetBundleManagerInitialized()
		{
		}

		public void ShowAdventureMap()
		{
			this.HideAllPanels(true);
			this.ShowOrderHallMultiPanel(true);
		}

		public bool ShowCompanionMultiPanel(bool show)
		{
			if (!this.m_companionMultiPanel)
			{
				return false;
			}
			this.m_companionMultiPanel.gameObject.SetActive(show);
			this.commonLegionWallpaper.SetActive(!show);
			return true;
		}

		public void ShowMissionList()
		{
			this.m_orderHallMultiPanel.ShowMissionListPanel();
		}

		public void ShowOrderHallMultiPanel(bool show)
		{
			if (this.m_orderHallMultiPanel)
			{
				this.m_orderHallMultiPanel.gameObject.SetActive(show);
				this.commonLegionWallpaper.SetActive(!show);
			}
		}
	}
}