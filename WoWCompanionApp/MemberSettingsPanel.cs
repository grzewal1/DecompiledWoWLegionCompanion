using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class MemberSettingsPanel : MonoBehaviour
	{
		public Text m_characterNameText;

		public Text m_kickButtonText;

		public Toggle m_memberRole;

		public Toggle m_moderatorRole;

		public Toggle m_leaderRole;

		public Toggle m_ownerRole;

		public GameObject m_kickButton;

		private CommunityMember m_member;

		public MemberSettingsPanel()
		{
		}

		private void AssignRole(Toggle toggle, ClubRoleIdentifier newRole)
		{
			if (toggle.isOn)
			{
				this.m_member.AssignRole(newRole);
			}
		}

		private void Awake()
		{
			CommunityData.OnRosterRefresh += new CommunityData.CommunityRefreshHandler(this.RefreshPage);
			Club.OnClubRemoved += new Club.ClubRemovedHandler(this.OnClubRemoved);
			Club.OnClubMemberRemoved += new Club.ClubMemberRemovedHandler(this.OnMemberRemoved);
		}

		public void KickMember()
		{
			this.m_member.KickMember();
			base.GetComponent<BaseDialog>().CloseDialog();
		}

		private void OnClubRemoved(Club.ClubRemovedEvent clubRemovedEvent)
		{
			if (clubRemovedEvent.ClubID == this.m_member.ClubID)
			{
				base.GetComponent<BaseDialog>().CloseDialog();
			}
		}

		private void OnDestroy()
		{
			CommunityData.OnRosterRefresh -= new CommunityData.CommunityRefreshHandler(this.RefreshPage);
			Club.OnClubRemoved -= new Club.ClubRemovedHandler(this.OnClubRemoved);
			Club.OnClubMemberRemoved -= new Club.ClubMemberRemovedHandler(this.OnMemberRemoved);
		}

		private void OnMemberRemoved(Club.ClubMemberRemovedEvent memberRemovedEvent)
		{
			if (memberRemovedEvent.ClubID == this.m_member.ClubID && memberRemovedEvent.MemberID == this.m_member.MemberID)
			{
				base.GetComponent<BaseDialog>().CloseDialog();
			}
		}

		public void RefreshPage(ulong clubID)
		{
			if (this.m_member.ClubID == clubID)
			{
				this.SetRoleToggleState();
				this.SetValidAssignableRoles();
			}
		}

		private void ResetOptionInteractability()
		{
			this.m_kickButton.SetActive(false);
			this.m_memberRole.interactable = false;
			this.m_moderatorRole.interactable = false;
			this.m_leaderRole.interactable = false;
			this.m_ownerRole.interactable = false;
		}

		private void SetInteractabilityByRole(ClubRoleIdentifier role)
		{
			switch (role)
			{
				case ClubRoleIdentifier.Owner:
				{
					this.m_ownerRole.interactable = true;
					break;
				}
				case ClubRoleIdentifier.Leader:
				{
					this.m_leaderRole.interactable = true;
					break;
				}
				case ClubRoleIdentifier.Moderator:
				{
					this.m_moderatorRole.interactable = true;
					break;
				}
				default:
				{
					this.m_memberRole.interactable = true;
					break;
				}
			}
		}

		private void SetLeaderRole()
		{
			this.AssignRole(this.m_leaderRole, ClubRoleIdentifier.Leader);
		}

		public void SetMemberInfo(CommunityMember member)
		{
			this.m_characterNameText.text = member.Name.ToUpper();
			this.m_kickButtonText.text = MobileClient.FormatString(StaticDB.GetString("COMMUNITIES_KICK_MEMBER", "[PH] Kick %s"), this.m_characterNameText.text);
			this.m_member = member;
			this.SetRoleToggleState();
			this.SetValidAssignableRoles();
			this.m_memberRole.onValueChanged.AddListener((bool argument0) => this.SetMemberRole());
			this.m_moderatorRole.onValueChanged.AddListener((bool argument1) => this.SetModeratorRole());
			this.m_leaderRole.onValueChanged.AddListener((bool argument2) => this.SetLeaderRole());
			this.m_ownerRole.onValueChanged.AddListener((bool argument3) => this.SetOwnerRole());
		}

		private void SetMemberRole()
		{
			this.AssignRole(this.m_memberRole, ClubRoleIdentifier.Member);
		}

		private void SetModeratorRole()
		{
			this.AssignRole(this.m_moderatorRole, ClubRoleIdentifier.Moderator);
		}

		private void SetOwnerRole()
		{
			this.AssignRole(this.m_ownerRole, ClubRoleIdentifier.Owner);
		}

		private void SetRoleToggleState()
		{
			switch (this.m_member.Role)
			{
				case ClubRoleIdentifier.Owner:
				{
					this.m_ownerRole.isOn = true;
					break;
				}
				case ClubRoleIdentifier.Leader:
				{
					this.m_leaderRole.isOn = true;
					break;
				}
				case ClubRoleIdentifier.Moderator:
				{
					this.m_moderatorRole.isOn = true;
					break;
				}
				default:
				{
					this.m_memberRole.isOn = true;
					break;
				}
			}
		}

		private void SetValidAssignableRoles()
		{
			this.ResetOptionInteractability();
			foreach (ClubRoleIdentifier assignableRole in this.m_member.GetAssignableRoles())
			{
				this.SetInteractabilityByRole(assignableRole);
			}
			if (this.m_member.IsKickable())
			{
				this.m_kickButton.SetActive(true);
			}
		}
	}
}