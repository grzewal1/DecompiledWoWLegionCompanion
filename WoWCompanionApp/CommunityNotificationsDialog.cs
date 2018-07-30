using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CommunityNotificationsDialog : MonoBehaviour
	{
		public Text m_headerCommunityText;

		public GameObject m_contentPane;

		public GameObject m_pushSettingPrefab;

		public GameObject m_sectionDividerPrefab;

		public GameObject m_channelNotificationSettingPrefab;

		private Community m_community;

		public CommunityNotificationsDialog()
		{
		}

		private void AddChannelSettingsToContent()
		{
			this.m_contentPane.AddAsChildObject(this.m_sectionDividerPrefab);
			foreach (CommunityStream allStream in this.m_community.GetAllStreams())
			{
				GameObject gameObject = this.m_contentPane.AddAsChildObject(this.m_channelNotificationSettingPrefab);
				gameObject.GetComponentInChildren<CommunityChannelNotificationButton>().SetChannel(allStream);
			}
		}

		private void AddPushSettingToContent()
		{
			this.m_contentPane.AddAsChildObject(this.m_pushSettingPrefab);
		}

		private void BuildContentPanel()
		{
			this.m_contentPane.DetachAllChildren();
			this.AddChannelSettingsToContent();
		}

		public void SetCommunity(Community community)
		{
			this.m_community = community;
			this.m_headerCommunityText.text = community.Name.ToUpper();
			this.BuildContentPanel();
		}
	}
}