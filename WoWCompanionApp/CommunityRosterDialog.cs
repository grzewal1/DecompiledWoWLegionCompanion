using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CommunityRosterDialog : MonoBehaviour
	{
		public GameObject m_rosterItemPagePrefab;

		public GameObject m_contentPanel;

		public InputField m_searchInput;

		public Text m_headerText;

		public bool m_showOffline;

		public bool m_alphabeticalOrdering;

		private ReadOnlyCollection<CommunityMember> m_memberList;

		private Community m_community;

		public CommunityRosterDialog()
		{
		}

		private GameObject AddChildToObject(GameObject parentobj, GameObject prefabToCreate)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefabToCreate);
			gameObject.transform.SetParent(parentobj.transform);
			gameObject.transform.SetAsLastSibling();
			gameObject.transform.localScale = Vector3.one;
			return gameObject;
		}

		private void AddMemberListToPanel(List<CommunityMember> memberList)
		{
			this.ClearContentPanel();
			if (!this.m_showOffline)
			{
				memberList = this.FilterByPresence(memberList);
			}
			if (this.m_alphabeticalOrdering)
			{
				memberList = this.SortListAlphabetically(memberList);
			}
			GameObject obj = this.AddChildToObject(this.m_contentPanel, this.m_rosterItemPagePrefab);
			CommunityRosterPage component = obj.GetComponent<CommunityRosterPage>();
			foreach (CommunityMember communityMember in memberList)
			{
				if (component.AtCapacity())
				{
					obj = this.AddChildToObject(this.m_contentPanel, this.m_rosterItemPagePrefab);
					component = obj.GetComponent<CommunityRosterPage>();
				}
				component.AddMemberToRoster(communityMember);
			}
		}

		private void ClearContentPanel()
		{
			for (int i = this.m_contentPanel.transform.childCount - 1; i >= 0; i--)
			{
				UnityEngine.Object.Destroy(this.m_contentPanel.transform.GetChild(i).gameObject);
			}
			this.m_contentPanel.transform.DetachChildren();
		}

		private List<CommunityMember> FilterByPresence(List<CommunityMember> listToSort)
		{
			List<CommunityMember> communityMembers = new List<CommunityMember>(listToSort);
			foreach (CommunityMember communityMember in listToSort)
			{
				if (communityMember.Presence != ClubMemberPresence.Offline)
				{
					continue;
				}
				communityMembers.Remove(communityMember);
			}
			return communityMembers;
		}

		private void FilterRosterByString(string filterString)
		{
			if (string.IsNullOrEmpty(filterString))
			{
				this.RefreshRoster();
				return;
			}
			List<CommunityMember> communityMembers = new List<CommunityMember>(this.m_memberList);
			foreach (CommunityMember mMemberList in this.m_memberList)
			{
				if (mMemberList.Name.IndexOf(filterString, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					continue;
				}
				communityMembers.Remove(mMemberList);
			}
			this.AddMemberListToPanel(communityMembers);
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
			CommunityData.OnRosterRefresh -= new CommunityData.CommunityRefreshHandler(this.OnRosterRefresh);
			Club.OnClubRemoved -= new Club.ClubRemovedHandler(this.OnClubRemoved);
		}

		private void OnRosterRefresh(ulong clubID)
		{
			if (clubID == this.m_community.ClubId)
			{
				this.m_memberList = this.m_community.GetMemberList();
				this.RefreshRoster();
			}
		}

		private void RefreshRoster()
		{
			this.AddMemberListToPanel(new List<CommunityMember>(this.m_memberList));
		}

		public void SetRosterData(Community community)
		{
			this.m_community = community;
			this.m_memberList = this.m_community.GetMemberList();
			this.m_headerText.text = this.m_community.Name.ToUpper();
			this.RefreshRoster();
		}

		private List<CommunityMember> SortListAlphabetically(List<CommunityMember> listToSort)
		{
			return (
				from member in listToSort
				orderby member.Name
				select member).ToList<CommunityMember>();
		}

		public void Start()
		{
			this.m_searchInput.onValueChanged.AddListener((string input) => this.FilterRosterByString(input));
			CommunityData.OnRosterRefresh += new CommunityData.CommunityRefreshHandler(this.OnRosterRefresh);
			Club.OnClubRemoved += new Club.ClubRemovedHandler(this.OnClubRemoved);
		}

		public void ToggleAlphabetical()
		{
			this.m_alphabeticalOrdering = !this.m_alphabeticalOrdering;
			this.RefreshRoster();
		}

		public void ToggleShowOffline()
		{
			this.m_showOffline = !this.m_showOffline;
			this.RefreshRoster();
		}
	}
}