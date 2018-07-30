using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CommunitySettingsDialog : MonoBehaviour
	{
		public GameObject m_notificationSettingsPrefab;

		public GameObject m_communityRosterPrefab;

		public Text m_headerText;

		private Community m_community;

		private GameObject m_communityButton;

		private Action m_markReadCallback;

		public CommunitySettingsDialog()
		{
		}

		public void InitializeCommunitySettings(CommunityButton communityButton)
		{
			this.m_community = communityButton.m_community;
			this.m_headerText.text = this.m_community.Name;
			this.m_markReadCallback = new Action(communityButton.UpdateNotifications);
			this.m_community.RefreshMemberList();
			this.m_community.RefreshStreams();
		}

		public void LeaveCommunity()
		{
			this.m_community.LeaveClub();
		}

		public void MarkAllAsRead()
		{
			if (this.m_community != null)
			{
				this.m_community.MarkAllAsRead();
				this.m_markReadCallback();
			}
		}

		public void OpenNotificationSettingsDialog()
		{
			GameObject level2Canvas = Main.instance.AddChildToLevel2Canvas(this.m_notificationSettingsPrefab);
			level2Canvas.GetComponent<CommunityNotificationsDialog>().SetCommunity(this.m_community);
		}

		public void OpenRoleAndRanksMenu()
		{
			GameObject level2Canvas = Main.instance.AddChildToLevel2Canvas(this.m_communityRosterPrefab);
			level2Canvas.GetComponent<CommunityMemberSettingsDialog>().SetCommunityAndPopulateContent(this.m_community);
		}
	}
}