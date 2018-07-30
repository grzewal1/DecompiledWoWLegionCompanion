using System;

namespace WoWCompanionApp
{
	public class CommunityChatMessage
	{
		private readonly ClubMessageInfo m_messageInfo;

		private readonly static DateTime BASE_EPOCH;

		public string Author
		{
			get
			{
				return this.m_messageInfo.author.name;
			}
		}

		public string Message
		{
			get
			{
				return this.m_messageInfo.content;
			}
		}

		public ClubMessageIdentifier MessageIdentifier
		{
			get
			{
				return this.m_messageInfo.messageId;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				DateTime bASEEPOCH = CommunityChatMessage.BASE_EPOCH;
				ClubMessageInfo mMessageInfo = this.m_messageInfo;
				DateTime dateTime = bASEEPOCH.AddTicks(Convert.ToInt64(mMessageInfo.messageId.epoch) * (long)10);
				return dateTime.ToLocalTime();
			}
		}

		static CommunityChatMessage()
		{
			CommunityChatMessage.BASE_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		}

		public CommunityChatMessage(ClubMessageInfo messageInfo)
		{
			this.m_messageInfo = messageInfo;
		}

		public bool PostedByModerator()
		{
			if (!this.m_messageInfo.author.role.HasValue)
			{
				return false;
			}
			return this.m_messageInfo.author.role.Value != ClubRoleIdentifier.Member;
		}
	}
}