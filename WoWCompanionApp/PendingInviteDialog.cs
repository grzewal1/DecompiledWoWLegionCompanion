using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace WoWCompanionApp
{
	public class PendingInviteDialog : MonoBehaviour
	{
		public GameObject m_inviteButtonPrefab;

		public GameObject m_contentPane;

		public PendingInviteDialog()
		{
		}

		private void Awake()
		{
			CommunityData.OnInviteRefresh += new CommunityData.RefreshHandler(this.RefreshInviteList);
			this.RefreshInviteList();
		}

		private void OnDestroy()
		{
			CommunityData.OnInviteRefresh -= new CommunityData.RefreshHandler(this.RefreshInviteList);
		}

		private void RefreshInviteList()
		{
			this.m_contentPane.DetachAllChildren();
			ReadOnlyCollection<CommunityPendingInvite> pendingInvites = CommunityData.Instance.GetPendingInvites();
			if (pendingInvites.Count == 0)
			{
				base.GetComponent<BaseDialog>().CloseDialog();
				return;
			}
			foreach (CommunityPendingInvite pendingInvite in pendingInvites)
			{
				GameObject gameObject = this.m_contentPane.AddAsChildObject(this.m_inviteButtonPrefab);
				gameObject.GetComponent<CommunityInviteButton>().SetInviteForButton(pendingInvite);
			}
		}
	}
}