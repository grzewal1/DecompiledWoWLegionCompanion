using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CommunityButton : MonoBehaviour
	{
		public Text m_communityName;

		public Community m_community;

		public GameObject m_communitySettingsDialogPrefab;

		public GameObject m_notificationImage;

		public CommunityButton()
		{
		}

		private void Awake()
		{
			Club.OnStreamViewMarkerUpdated += new Club.StreamViewMarkerUpdatedHandler(this.OnViewMarkerUpdated);
		}

		public ReadOnlyCollection<CommunityStream> GetStreamList()
		{
			return this.m_community.GetAllStreams();
		}

		private void OnDestroy()
		{
			Club.OnStreamViewMarkerUpdated -= new Club.StreamViewMarkerUpdatedHandler(this.OnViewMarkerUpdated);
		}

		private void OnViewMarkerUpdated(Club.StreamViewMarkerUpdatedEvent markerEvent)
		{
			if (markerEvent.ClubID == this.m_community.ClubId)
			{
				this.UpdateNotifications();
			}
		}

		public void OpenChannelSelect()
		{
			this.m_community.PopulateCommunityInfo();
			base.gameObject.GetComponentInParent<SocialPanel>().OpenChannelSelect(this);
		}

		public void OpenCommunitySettings()
		{
			GameObject level2Canvas = Main.instance.AddChildToLevel2Canvas(this.m_communitySettingsDialogPrefab);
			level2Canvas.GetComponent<CommunitySettingsDialog>().InitializeCommunitySettings(this);
		}

		public void SelectCommunity()
		{
			this.m_community.PopulateCommunityInfo();
			base.gameObject.GetComponentInParent<SocialPanel>().SelectCommunityButton(this);
		}

		public void SetCommunity(Community community)
		{
			this.m_community = community;
			this.m_communityName.text = community.Name.ToUpper();
			this.UpdateNotifications();
		}

		public void UpdateNotifications()
		{
			this.m_notificationImage.SetActive(this.m_community.HasUnreadMessages(null));
		}
	}
}