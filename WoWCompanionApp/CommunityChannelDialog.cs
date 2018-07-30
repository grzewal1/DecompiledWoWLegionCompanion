using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CommunityChannelDialog : MonoBehaviour
	{
		public Text m_headerText;

		public GameObject m_channelSelectPrefab;

		public GameObject m_content;

		private Community m_community;

		private UnityAction<CommunityChannelButton> m_selectCallback;

		private UnityAction m_cleanupCallback;

		public CommunityChannelDialog()
		{
		}

		private void Awake()
		{
			CommunityData.OnChannelRefresh += new CommunityData.CommunityRefreshHandler(this.OnChannelRefresh);
		}

		private void BuildContentPane()
		{
			this.m_content.DetachAllChildren();
			foreach (CommunityStream allStream in this.m_community.GetAllStreams())
			{
				GameObject gameObject = this.m_content.AddAsChildObject(this.m_channelSelectPrefab);
				CommunityChannelButton component = gameObject.GetComponent<CommunityChannelButton>();
				component.SetCommunityInfo(this.m_community, allStream);
				component.GetComponentInChildren<Button>().onClick.AddListener(() => this.m_selectCallback(component));
				component.GetComponentInChildren<Button>().onClick.AddListener(this.m_cleanupCallback);
			}
		}

		public void InitializeContentPane(Community community, UnityAction<CommunityChannelButton> selectCallback, UnityAction cleanupCallback)
		{
			this.m_community = community;
			this.m_headerText.text = this.m_community.Name.ToUpper();
			this.m_selectCallback = selectCallback;
			this.m_cleanupCallback = cleanupCallback;
			this.BuildContentPane();
		}

		private void OnChannelRefresh(ulong communityID)
		{
			if (this.m_community.ClubId == communityID)
			{
				this.BuildContentPane();
			}
		}

		private void OnDestroy()
		{
			CommunityData.OnChannelRefresh -= new CommunityData.CommunityRefreshHandler(this.OnChannelRefresh);
		}
	}
}