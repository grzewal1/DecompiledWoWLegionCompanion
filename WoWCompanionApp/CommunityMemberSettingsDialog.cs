using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CommunityMemberSettingsDialog : MonoBehaviour
	{
		public Text m_communityName;

		public GameObject m_memberButtonPrefab;

		public GameObject m_contentPanel;

		private Community m_community;

		public CommunityMemberSettingsDialog()
		{
		}

		private void Awake()
		{
			CommunityData.OnRosterRefresh += new CommunityData.CommunityRefreshHandler(this.OnRefresh);
			Club.OnClubRemoved += new Club.ClubRemovedHandler(this.OnClubRemoved);
		}

		private void OnClubRemoved(Club.ClubRemovedEvent clubRemovedEvent)
		{
			if (clubRemovedEvent.ClubID == this.m_community.ClubId)
			{
				base.GetComponent<BaseDialog>().CloseDialog();
			}
		}

		private void OnDestroy()
		{
			CommunityData.OnRosterRefresh -= new CommunityData.CommunityRefreshHandler(this.OnRefresh);
			Club.OnClubRemoved -= new Club.ClubRemovedHandler(this.OnClubRemoved);
		}

		private void OnRefresh(ulong clubID)
		{
			if (this.m_community.ClubId == clubID)
			{
				this.m_contentPanel.DetachAllChildren();
				this.PopulateMemberList();
			}
		}

		private void PopulateMemberList()
		{
			foreach (CommunityMember memberList in this.m_community.GetMemberList())
			{
				GameObject gameObject = this.m_contentPanel.AddAsChildObject(this.m_memberButtonPrefab);
				gameObject.GetComponent<CommunityMemberButton>().PopulateMemberInfo(memberList);
			}
		}

		public void SetCommunityAndPopulateContent(Community community)
		{
			this.m_community = community;
			this.m_communityName.text = this.m_community.Name;
			this.m_community.RefreshMemberList();
			this.m_contentPanel.DetachAllChildren();
			this.PopulateMemberList();
		}
	}
}