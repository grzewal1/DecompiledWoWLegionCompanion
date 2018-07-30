using System;
using UnityEngine;
using UnityEngine.Events;

namespace WoWCompanionApp
{
	public class SocialPanel : MonoBehaviour
	{
		public GameObject m_communitiesChatPanel;

		public GameObject m_communitiesListPanel;

		public GameObject m_channelSelectDialogPrefab;

		public GameObject m_inviteLinkDialogPrefab;

		public GameObject m_pendingInvitationDialogPrefab;

		public GameObject m_noChannelsAvailableDialogPrefab;

		private const string LOOKUP_PREFIX = "DefaultChannel_";

		public SocialPanel()
		{
		}

		private void Awake()
		{
			CommunityData.Instance.RefreshCommunities();
			CommunityData.Instance.RefreshInvitations();
			CommunityData.OnCommunityRefresh += new CommunityData.RefreshHandler(this.RefreshScrollingContent);
			CommunityData.OnInviteRefresh += new CommunityData.RefreshHandler(this.RefreshScrollingContent);
		}

		public void CloseChatPanel()
		{
			this.m_communitiesChatPanel.SetActive(false);
			this.m_communitiesListPanel.SetActive(true);
			this.RefreshScrollingContent();
		}

		private void OnDestroy()
		{
			CommunityData.OnCommunityRefresh -= new CommunityData.RefreshHandler(this.RefreshScrollingContent);
			CommunityData.OnInviteRefresh -= new CommunityData.RefreshHandler(this.RefreshScrollingContent);
		}

		private void OnEnable()
		{
			this.RefreshScrollingContent();
		}

		public void OpenChannelSelect(CommunityButton button)
		{
			GameObject level2Canvas = Main.instance.AddChildToLevel2Canvas(this.m_channelSelectDialogPrefab);
			CommunityChannelDialog component = level2Canvas.GetComponent<CommunityChannelDialog>();
			component.InitializeContentPane(button.m_community, new UnityAction<CommunityChannelButton>(this.SelectChannelButton), new UnityAction(level2Canvas.GetComponent<BaseDialog>().CloseDialog));
		}

		private void OpenChatPanel(Community community, CommunityStream stream)
		{
			this.m_communitiesChatPanel.SetActive(true);
			this.m_communitiesChatPanel.GetComponent<CommunityChatPanel>().InitializeChatContent(community, stream);
			this.m_communitiesListPanel.SetActive(false);
		}

		public void OpenInviteLinkDialog()
		{
			Main.instance.AddChildToLevel2Canvas(this.m_inviteLinkDialogPrefab);
		}

		public void RefreshScrollingContent()
		{
			this.m_communitiesListPanel.GetComponent<CommunityListPanel>().RefreshPanelContent();
		}

		public void SelectChannelButton(CommunityChannelButton button)
		{
			ulong clubId = button.Community.ClubId;
			string str = string.Concat("DefaultChannel_", clubId.ToString());
			ulong streamId = button.StreamId;
			SecurePlayerPrefs.SetString(str, streamId.ToString(), Main.uniqueIdentifier);
			this.OpenChatPanel(button.Community, button.Stream);
		}

		public void SelectCommunityButton(CommunityButton button)
		{
			ulong num = (ulong)0;
			ulong clubId = button.m_community.ClubId;
			string str = string.Concat("DefaultChannel_", clubId.ToString());
			if (SecurePlayerPrefs.HasKey(str))
			{
				num = Convert.ToUInt64(SecurePlayerPrefs.GetString(str, Main.uniqueIdentifier));
			}
			CommunityStream defaultStream = button.m_community.GetDefaultStream(num);
			if (defaultStream != null)
			{
				ulong streamId = defaultStream.StreamId;
				SecurePlayerPrefs.SetString(str, streamId.ToString(), Main.uniqueIdentifier);
				this.OpenChatPanel(button.m_community, defaultStream);
			}
			else
			{
				Main.instance.AddChildToLevel2Canvas(this.m_noChannelsAvailableDialogPrefab);
			}
		}
	}
}