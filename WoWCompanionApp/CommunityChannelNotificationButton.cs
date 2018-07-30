using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CommunityChannelNotificationButton : MonoBehaviour
	{
		public Text m_channelName;

		public GameObject m_leaderModeratorImage;

		public Toggle m_allButton;

		public Toggle m_nothingButton;

		private CommunityStream m_stream;

		public CommunityChannelNotificationButton()
		{
		}

		public void SetChannel(CommunityStream stream)
		{
			this.m_stream = stream;
			this.m_channelName.text = this.m_stream.Name.ToUpper();
			this.m_leaderModeratorImage.SetActive(this.m_stream.ForLeadersAndModerators);
			this.UpdateToggleState();
			this.m_allButton.onValueChanged.AddListener((bool argument0) => this.TurnOnNotifications());
			this.m_nothingButton.onValueChanged.AddListener((bool argument1) => this.TurnOffNotifications());
		}

		private void TurnOffNotifications()
		{
			if (this.m_nothingButton.isOn)
			{
				this.m_stream.SetNotificationFilter(ClubStreamNotificationFilter.None);
			}
		}

		private void TurnOnNotifications()
		{
			if (this.m_allButton.isOn)
			{
				this.m_stream.SetNotificationFilter(ClubStreamNotificationFilter.All);
			}
		}

		private void UpdateToggleState()
		{
			if (!this.m_stream.ShouldReceiveNotifications())
			{
				this.m_nothingButton.isOn = true;
			}
			else
			{
				this.m_allButton.isOn = true;
			}
		}
	}
}