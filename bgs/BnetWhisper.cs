using System;

namespace bgs
{
	public class BnetWhisper
	{
		private BnetGameAccountId m_speakerId;

		private BnetGameAccountId m_receiverId;

		private string m_message;

		private ulong m_timestampMicrosec;

		private BnetErrorInfo m_errorInfo;

		public BnetWhisper()
		{
		}

		public BnetErrorInfo GetErrorInfo()
		{
			return this.m_errorInfo;
		}

		public string GetMessage()
		{
			return this.m_message;
		}

		public BnetGameAccountId GetReceiverId()
		{
			return this.m_receiverId;
		}

		public BnetGameAccountId GetSpeakerId()
		{
			return this.m_speakerId;
		}

		public ulong GetTimestampMicrosec()
		{
			return this.m_timestampMicrosec;
		}

		public void SetErrorInfo(BnetErrorInfo errorInfo)
		{
			this.m_errorInfo = errorInfo;
		}

		public void SetMessage(string message)
		{
			this.m_message = message;
		}

		public void SetReceiverId(BnetGameAccountId id)
		{
			this.m_receiverId = id;
		}

		public void SetSpeakerId(BnetGameAccountId id)
		{
			this.m_speakerId = id;
		}

		public void SetTimestampMicrosec(ulong microsec)
		{
			this.m_timestampMicrosec = microsec;
		}

		public void SetTimestampMilliseconds(double milliseconds)
		{
			this.m_timestampMicrosec = (ulong)(milliseconds * 1000);
		}
	}
}