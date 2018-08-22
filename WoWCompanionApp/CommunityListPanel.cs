using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CommunityListPanel : MonoBehaviour
	{
		public GameObject m_scrollContent;

		public GameObject m_communityButtonPrefab;

		public GameObject m_inviteButtonPrefab;

		public GameObject m_scrollSectionHeaderPrefab;

		public GameObject m_pendingInvitesDialogPrefab;

		public CommunityListPanel()
		{
		}

		private void AddCommunitiesToContent()
		{
			if (CommunityData.Instance.HasGuild())
			{
				GameObject gameObject = this.m_scrollContent.AddAsChildObject(this.m_scrollSectionHeaderPrefab);
				string str = StaticDB.GetString("SOCIAL_CHARACTERS_GUILD", "%s'S GUILD");
				string str1 = MobileClient.FormatString(str, Singleton<CharacterData>.Instance.CharacterName.ToUpper());
				gameObject.GetComponentInChildren<Text>().text = str1;
				CommunityData.Instance.ForGuild((Community guild) => this.m_scrollContent.AddAsChildObject(this.m_communityButtonPrefab).GetComponent<CommunityButton>().SetCommunity(guild));
			}
			if (CommunityData.Instance.HasCommunities())
			{
				GameObject gameObject1 = this.m_scrollContent.AddAsChildObject(this.m_scrollSectionHeaderPrefab);
				string str2 = StaticDB.GetString("SOCIAL_CHARACTERS_COMMUNITIES", "%s'S COMMUNITIES");
				string str3 = MobileClient.FormatString(str2, Singleton<CharacterData>.Instance.CharacterName.ToUpper());
				gameObject1.GetComponentInChildren<Text>().text = str3;
				CommunityData.Instance.ForEachCommunity((Community community) => this.m_scrollContent.AddAsChildObject(this.m_communityButtonPrefab).GetComponent<CommunityButton>().SetCommunity(community));
			}
		}

		private void AddInvitationsToContent()
		{
			ReadOnlyCollection<CommunityPendingInvite> pendingInvites = CommunityData.Instance.GetPendingInvites();
			if (pendingInvites.Count > 0)
			{
				GameObject gameObject = this.m_scrollContent.AddAsChildObject(this.m_scrollSectionHeaderPrefab);
				gameObject.GetComponentInChildren<Text>().text = "PENDING INVITATIONS";
				GameObject gameObject1 = this.m_scrollContent.AddAsChildObject(this.m_inviteButtonPrefab);
				string str = StaticDB.GetString("COMMUNITIES_MULTIPLE_INVITATIONS", null);
				str = GeneralHelpers.QuantityRule(str, pendingInvites.Count);
				gameObject1.GetComponentInChildren<Text>().text = str;
				gameObject1.GetComponentInChildren<Button>().onClick.AddListener(new UnityAction(this.OpenPendingInvitesDialog));
			}
		}

		private void OpenPendingInvitesDialog()
		{
			Main.instance.AddChildToLevel2Canvas(this.m_pendingInvitesDialogPrefab);
		}

		public void RefreshPanelContent()
		{
			this.m_scrollContent.DetachAllChildren();
			this.AddInvitationsToContent();
			this.AddCommunitiesToContent();
		}
	}
}