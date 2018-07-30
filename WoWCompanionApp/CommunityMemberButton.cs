using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CommunityMemberButton : MonoBehaviour
	{
		public GameObject m_memberSettingsPrefab;

		public Text m_characterName;

		public Button m_rankButton;

		public GameObject m_buttonFrame;

		public LocalizedText m_buttonText;

		public Image m_classImage;

		public GameObject m_moderatorImage;

		public GameObject m_leaderImage;

		private CommunityMember m_memberInfo;

		public CommunityMemberButton()
		{
		}

		public void OpenMemberSettings()
		{
			GameObject level3Canvas = Main.instance.AddChildToLevel3Canvas(this.m_memberSettingsPrefab);
			level3Canvas.GetComponent<MemberSettingsPanel>().SetMemberInfo(this.m_memberInfo);
		}

		public void PopulateMemberInfo(CommunityMember member)
		{
			this.m_memberInfo = member;
			this.m_characterName.text = this.m_memberInfo.Name;
			this.m_classImage.sprite = GeneralHelpers.LoadClassIcon((int)this.m_memberInfo.Class);
			this.m_buttonText.SetNewStringKey(this.m_memberInfo.ConvertRoleToString());
			this.SetButtonState(this.m_memberInfo.GetAssignableRoles().Count > 0);
			this.SetRoleIconVisibility();
		}

		private void SetButtonState(bool state)
		{
			this.m_buttonFrame.SetActive(state);
			this.m_rankButton.interactable = state;
		}

		private void SetRoleIconVisibility()
		{
			ClubRoleIdentifier role = this.m_memberInfo.Role;
			this.m_moderatorImage.SetActive(role == ClubRoleIdentifier.Moderator);
			this.m_leaderImage.SetActive((role == ClubRoleIdentifier.Leader ? true : role == ClubRoleIdentifier.Owner));
		}
	}
}