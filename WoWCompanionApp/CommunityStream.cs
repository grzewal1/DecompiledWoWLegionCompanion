using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WoWCompanionApp
{
	public class CommunityStream
	{
		private ulong m_clubId;

		private readonly ClubStreamInfo m_streamInfo;

		private List<CommunityChatMessage> m_messages = new List<CommunityChatMessage>();

		private ClubStreamNotificationSetting m_notificationSetting;

		public ClubStreamNotificationSetting Filter
		{
			set
			{
				this.m_notificationSetting = value;
			}
		}

		public bool ForLeadersAndModerators
		{
			get
			{
				return this.m_streamInfo.leadersAndModeratorsOnly;
			}
		}

		public string Name
		{
			get
			{
				return this.m_streamInfo.name;
			}
		}

		public ulong StreamId
		{
			get
			{
				return this.m_streamInfo.streamId;
			}
		}

		public string Subject
		{
			get
			{
				return this.m_streamInfo.subject;
			}
		}

		public CommunityStream(ulong clubId, ClubStreamInfo streamInfo)
		{
			this.m_clubId = clubId;
			this.m_streamInfo = streamInfo;
		}

		public void AddMessage(string message)
		{
			Club.SendMessage(this.m_clubId, this.m_streamInfo.streamId, message);
		}

		public void ClearNotifications()
		{
			Club.AdvanceStreamViewMarker(this.m_clubId, this.StreamId);
		}

		public void DeleteMessage(CommunityChatMessage message)
		{
			Club.DestroyMessage(this.m_clubId, this.StreamId, message.MessageIdentifier);
		}

		public void EditMessage(CommunityChatMessage message, string newMessage)
		{
			Club.EditMessage(this.m_clubId, this.StreamId, message.MessageIdentifier, newMessage);
		}

		public void EditStream(string name, string subject, bool? modsOnly)
		{
			Club.EditStream(this.m_clubId, this.StreamId, name, subject, modsOnly);
		}

		public bool FocusStream()
		{
			return Club.FocusStream(this.m_clubId, this.StreamId);
		}

		public ReadOnlyCollection<CommunityChatMessage> GetMessages()
		{
			return this.m_messages.AsReadOnly();
		}

		public void HandleClubMessageHistoryEvent(Club.ClubMessageHistoryReceivedEvent historyEvent)
		{
			if (historyEvent.ClubID != this.m_clubId || historyEvent.StreamID != this.StreamId)
			{
				return;
			}
			ulong mClubId = this.m_clubId;
			ulong streamId = this.StreamId;
			ClubMessageIdentifier contiguousRange = historyEvent.ContiguousRange.oldestMessageId;
			ClubMessageRange clubMessageRange = historyEvent.ContiguousRange;
			List<ClubMessageInfo> messagesInRange = Club.GetMessagesInRange(mClubId, streamId, contiguousRange, clubMessageRange.newestMessageId);
			this.m_messages.Clear();
			foreach (ClubMessageInfo clubMessageInfo in messagesInRange)
			{
				this.m_messages.Add(new CommunityChatMessage(clubMessageInfo));
			}
		}

		public CommunityChatMessage HandleMessageAddedEvent(Club.ClubMessageAddedEvent messageEvent)
		{
			ClubMessageInfo? messageInfo = Club.GetMessageInfo(this.m_clubId, this.StreamId, messageEvent.MessageID);
			if (!messageInfo.HasValue)
			{
				return null;
			}
			CommunityChatMessage communityChatMessage = new CommunityChatMessage(messageInfo.Value);
			this.m_messages.Add(communityChatMessage);
			return communityChatMessage;
		}

		public bool HasUnreadMessages()
		{
			return Club.GetStreamViewMarker(this.m_clubId, this.StreamId).HasValue;
		}

		public bool IsSubscribed()
		{
			return Club.IsSubscribedToStream(this.m_clubId, this.StreamId);
		}

		public bool RequestMoreMessages()
		{
			ClubMessageIdentifier? nullable = null;
			return Club.RequestMoreMessagesBefore(this.m_clubId, this.StreamId, nullable, new uint?(50));
		}

		public void SetNotificationFilter(ClubStreamNotificationFilter filter)
		{
			this.m_notificationSetting.filter = filter;
			List<ClubStreamNotificationSetting> clubStreamNotificationSettings = new List<ClubStreamNotificationSetting>()
			{
				this.m_notificationSetting
			};
			Club.SetClubStreamNotificationSettings(this.m_clubId, clubStreamNotificationSettings);
		}

		public bool ShouldReceiveNotifications()
		{
			return this.m_notificationSetting.filter == ClubStreamNotificationFilter.All;
		}

		public void UnfocusStream()
		{
			Club.UnfocusStream(this.m_clubId, this.StreamId);
		}
	}
}