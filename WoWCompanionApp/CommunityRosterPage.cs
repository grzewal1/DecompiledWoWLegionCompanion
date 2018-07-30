using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class CommunityRosterPage : MonoBehaviour
	{
		public CommunityRosterItem m_memberButtonPrefab;

		public GameObject m_contentPane;

		public int m_pageCapacity;

		public CommunityRosterPage()
		{
		}

		public void AddMemberToRoster(CommunityMember member)
		{
			this.m_contentPane.AddAsChildObject<CommunityRosterItem>(this.m_memberButtonPrefab).PopulateMemberInfo(member);
		}

		public bool AtCapacity()
		{
			return this.m_contentPane.transform.childCount == this.m_pageCapacity;
		}
	}
}