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
			GameObject gameObject = this.m_scrollContent.AddAsChildObject(this.m_scrollSectionHeaderPrefab);
			string str = MobileClient.FormatString("%s'S COMMUNITIES", Singleton<CharacterData>.Instance.CharacterName.ToUpper());
			gameObject.GetComponentInChildren<Text>().text = str;
			CommunityData.Instance.ForEachCommunity((Community community) => this.m_scrollContent.AddAsChildObject(this.m_communityButtonPrefab).GetComponent<CommunityButton>().SetCommunity(community));
		}

		private void AddInvitationsToContent()
		{
			ReadOnlyCollection<CommunityPendingInvite> pendingInvites = CommunityData.Instance.GetPendingInvites();
			if (pendingInvites.Count > 0)
			{
				GameObject gameObject = this.m_scrollContent.AddAsChildObject(this.m_scrollSectionHeaderPrefab);
				gameObject.GetComponentInChildren<Text>().text = "PENDING INVITATIONS";
				GameObject gameObject1 = this.m_scrollContent.AddAsChildObject(this.m_inviteButtonPrefab);
				string str = (pendingInvites.Count <= 1 ? "1 INVITE" : "%s INVITATIONS");
				int count = pendingInvites.Count;
				str = MobileClient.FormatString(str, count.ToString());
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