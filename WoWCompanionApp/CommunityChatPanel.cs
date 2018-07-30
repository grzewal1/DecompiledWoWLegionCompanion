using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class CommunityChatPanel : MonoBehaviour
	{
		public GameObject m_chatContent;

		public GameObject m_chatObjectPrefab;

		public GameObject m_dateObjectPrefab;

		public GameObject m_notificationSettingsPrefab;

		public GameObject m_channelRosterPrefab;

		public GameObject m_channelSelectDialog;

		public GameObject m_otherCommunityNotification;

		public GameObject m_otherChannelNotification;

		public InputField m_chatInputText;

		public Text m_headerText;

		private string m_earliestDate;

		private string m_latestDate;

		private Community m_community;

		private CommunityStream m_focusedStream;

		private CommunityChatItem m_lastChatItem;

		private int m_childCountBeforeRefresh;

		private bool m_requestPending;

		private bool m_showingChannelSelect;

		private const int MINUTES_FOR_MINIMIZE = 5;

		private const string MON_DAY_YEAR_FORMAT = "MMM dd, yyyy";

		public CommunityChatPanel()
		{
		}

		private void AddChatMessage(CommunityChatMessage message)
		{
			string str = message.TimeStamp.ToString("MMM dd, yyyy");
			if (this.m_earliestDate == string.Empty)
			{
				this.m_earliestDate = str;
			}
			if (this.m_latestDate != str)
			{
				this.m_latestDate = str;
				GameObject upper = this.m_chatContent.AddAsChildObject(this.m_dateObjectPrefab);
				upper.GetComponentInChildren<Text>().text = str.ToUpper();
			}
			GameObject gameObject = this.m_chatContent.AddAsChildObject(this.m_chatObjectPrefab);
			CommunityChatItem component = gameObject.GetComponent<CommunityChatItem>();
			component.SetChatInfo(message);
			if (!this.ShouldMinimizeChatItem(message))
			{
				this.m_lastChatItem = component;
			}
			else
			{
				gameObject.GetComponent<CommunityChatItem>().MinimizeChatItem();
			}
		}

		private void Awake()
		{
			base.gameObject.transform.localScale = Vector3.one;
			RectTransform component = base.gameObject.GetComponent<RectTransform>();
			Vector2 vector2 = Vector2.zero;
			base.gameObject.GetComponent<RectTransform>().offsetMax = vector2;
			component.offsetMin = vector2;
			ScrollRect componentInChildren = base.GetComponentInChildren<ScrollRect>();
			componentInChildren.onValueChanged.AddListener((Vector2 argument0) => this.OnContentScroll());
			Club.OnClubStreamSubscribed += new Club.ClubStreamSubscribedHandler(this.OnStreamSubscribed);
			Club.OnClubMessageHistoryReceived += new Club.ClubMessageHistoryReceivedHandler(this.OnMessageHistoryReceived);
			Club.OnClubMessageAdded += new Club.ClubMessageAddedHandler(this.OnMessageAdded);
			Club.OnStreamViewMarkerUpdated += new Club.StreamViewMarkerUpdatedHandler(this.OnViewMarkerUpdated);
		}

		private void BuildMessageList()
		{
			foreach (CommunityChatMessage message in this.m_focusedStream.GetMessages())
			{
				this.AddChatMessage(message);
			}
			this.ScrollToBottom();
			if (this.m_childCountBeforeRefresh != 0)
			{
				this.SnapTo(this.m_chatContent.transform.GetChild(this.m_chatContent.transform.childCount - this.m_childCountBeforeRefresh).GetComponent<RectTransform>());
			}
		}

		public void CloseChatPanel()
		{
			base.gameObject.GetComponentInParent<SocialPanel>().CloseChatPanel();
		}

		private void GetPreloadedMessages()
		{
		}

		public void InitializeChatContent(Community community, CommunityStream stream)
		{
			if (this.m_focusedStream == null || this.m_focusedStream.StreamId != stream.StreamId)
			{
				this.ResetFocusedStream();
				this.ResetChatPanel();
				this.m_community = community;
				this.m_focusedStream = stream;
				this.m_headerText.text = this.m_focusedStream.Name.ToUpper();
				this.UpdateNotificationMarkers();
				this.m_focusedStream.FocusStream();
				if (this.m_focusedStream.IsSubscribed())
				{
					this.BuildMessageList();
				}
				this.m_channelSelectDialog.GetComponent<CommunityChannelDialog>().InitializeContentPane(this.m_community, new UnityAction<CommunityChannelButton>(base.GetComponentInParent<SocialPanel>().SelectChannelButton), () => this.m_channelSelectDialog.SetActive(false));
			}
			this.m_focusedStream.ClearNotifications();
		}

		private void OnContentScroll()
		{
			ScrollRect componentInChildren = base.GetComponentInChildren<ScrollRect>();
			if (!this.m_requestPending && componentInChildren.verticalNormalizedPosition == 1f && this.m_focusedStream != null)
			{
				if (!this.m_focusedStream.RequestMoreMessages())
				{
					this.m_requestPending = true;
				}
				else
				{
					this.GetPreloadedMessages();
				}
			}
		}

		private void OnDestroy()
		{
			Club.OnClubStreamSubscribed -= new Club.ClubStreamSubscribedHandler(this.OnStreamSubscribed);
			Club.OnClubMessageHistoryReceived -= new Club.ClubMessageHistoryReceivedHandler(this.OnMessageHistoryReceived);
			Club.OnClubMessageAdded -= new Club.ClubMessageAddedHandler(this.OnMessageAdded);
			Club.OnStreamViewMarkerUpdated -= new Club.StreamViewMarkerUpdatedHandler(this.OnViewMarkerUpdated);
		}

		private void OnDisable()
		{
			this.ResetFocusedStream();
		}

		private void OnMessageAdded(Club.ClubMessageAddedEvent messageEvent)
		{
			CommunityChatMessage communityChatMessage = null;
			if (this.m_focusedStream != null)
			{
				communityChatMessage = this.m_focusedStream.HandleMessageAddedEvent(messageEvent);
			}
			if (communityChatMessage == null)
			{
				CommunityData.Instance.HandleMessageAddedEvent(messageEvent);
			}
			else
			{
				this.AddChatMessage(communityChatMessage);
				this.ScrollToBottom();
			}
		}

		private void OnMessageHistoryReceived(Club.ClubMessageHistoryReceivedEvent historyEvent)
		{
			this.m_childCountBeforeRefresh = this.m_chatContent.transform.childCount;
			this.m_focusedStream.HandleClubMessageHistoryEvent(historyEvent);
			this.RebuildMessageList();
			this.m_requestPending = false;
		}

		private void OnStreamSubscribed(Club.ClubStreamSubscribedEvent subscribeEvent)
		{
			if (!this.m_focusedStream.RequestMoreMessages())
			{
				this.m_requestPending = true;
			}
			else
			{
				this.GetPreloadedMessages();
			}
		}

		private void OnViewMarkerUpdated(Club.StreamViewMarkerUpdatedEvent markerEvent)
		{
			this.UpdateNotificationMarkers();
		}

		public void OpenNotificationSettings()
		{
			Main.instance.AddChildToLevel2Canvas(this.m_notificationSettingsPrefab);
		}

		private void RebuildMessageList()
		{
			this.ResetChatPanel();
			this.BuildMessageList();
		}

		private void ResetChatPanel()
		{
			string empty = string.Empty;
			string str = empty;
			this.m_latestDate = empty;
			this.m_earliestDate = str;
			this.m_lastChatItem = null;
			this.m_chatContent.DetachAllChildren();
		}

		private void ResetFocusedStream()
		{
			if (this.m_focusedStream != null)
			{
				this.m_childCountBeforeRefresh = 0;
				this.m_requestPending = false;
				this.m_focusedStream.UnfocusStream();
				this.m_focusedStream = null;
			}
		}

		private void ScrollToBottom()
		{
			Canvas.ForceUpdateCanvases();
			base.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0f;
		}

		public void SendChatMessage()
		{
			if (this.m_chatInputText.text != string.Empty)
			{
				this.m_focusedStream.AddMessage(this.m_chatInputText.text);
			}
		}

		private bool ShouldMinimizeChatItem(CommunityChatMessage message)
		{
			if (this.m_lastChatItem == null)
			{
				return false;
			}
			TimeSpan timeStamp = message.TimeStamp - this.m_lastChatItem.TimeStamp;
			return (!(this.m_lastChatItem.GetAuthor() == message.Author) || timeStamp.Minutes >= 5 || timeStamp.Days != 0 ? false : timeStamp.Hours == 0);
		}

		public void ShowRoster()
		{
			GameObject level2Canvas = Main.instance.AddChildToLevel2Canvas(this.m_channelRosterPrefab);
			level2Canvas.GetComponent<CommunityRosterDialog>().SetRosterData(this.m_community);
		}

		public void SnapTo(RectTransform target)
		{
			RectTransform component = this.m_chatContent.GetComponent<RectTransform>();
			float componentInChildren = base.GetComponentInChildren<RectTransform>().rect.height;
			float single = component.rect.height;
			if (single < componentInChildren)
			{
				return;
			}
			Canvas.ForceUpdateCanvases();
			ScrollRect scrollRect = base.GetComponentInChildren<ScrollRect>();
			Vector2 vector2 = scrollRect.transform.InverseTransformPoint(component.position) - scrollRect.transform.InverseTransformPoint(target.position);
			if (vector2.y + componentInChildren > single)
			{
				vector2.y = single - componentInChildren + 60f;
			}
			component.anchoredPosition = vector2;
			component.offsetMin = new Vector2(40f, component.offsetMin.y);
			component.offsetMax = new Vector2(-40f, component.offsetMax.y);
		}

		private void Update()
		{
		}

		private void UpdateNotificationMarkers()
		{
			this.m_otherChannelNotification.SetActive(this.m_community.HasUnreadMessages(this.m_focusedStream));
			this.m_otherCommunityNotification.SetActive(CommunityData.Instance.HasUnreadCommunityMessages(this.m_community));
		}
	}
}