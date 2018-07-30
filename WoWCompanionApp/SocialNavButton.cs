using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class SocialNavButton : CompanionNavButton
	{
		public SocialNavButton()
		{
		}

		private void Awake()
		{
			Club.OnStreamViewMarkerUpdated += new Club.StreamViewMarkerUpdatedHandler(this.OnStreamViewMarkerUpdate);
		}

		private void OnDestroy()
		{
			Club.OnStreamViewMarkerUpdated -= new Club.StreamViewMarkerUpdatedHandler(this.OnStreamViewMarkerUpdate);
		}

		private void OnStreamViewMarkerUpdate(Club.StreamViewMarkerUpdatedEvent markerEvent)
		{
			this.UpdateNotificationState();
		}

		private void Start()
		{
			base.InitializeButtonState(true);
		}

		protected override void UpdateNotificationState()
		{
			this.m_notificationImage.SetActive(CommunityData.Instance.HasUnreadCommunityMessages(null));
		}
	}
}