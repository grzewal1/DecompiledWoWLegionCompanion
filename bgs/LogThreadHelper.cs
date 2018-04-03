using System;
using System.Collections.Generic;
using System.Threading;

namespace bgs
{
	public class LogThreadHelper
	{
		private BattleNetLogSource m_logSource;

		private List<LogThreadHelper.LogEntry> m_queuedLogs = new List<LogThreadHelper.LogEntry>();

		public LogThreadHelper(string name)
		{
			this.m_logSource = new BattleNetLogSource(name);
		}

		public void LogDebug(string message)
		{
			this.LogMessage(message, LogLevel.Debug);
		}

		public void LogDebug(string format, params object[] args)
		{
			this.LogMessage(string.Format(format, args), LogLevel.Debug);
		}

		public void LogError(string message)
		{
			this.LogMessage(message, LogLevel.Error);
		}

		public void LogError(string format, params object[] args)
		{
			this.LogMessage(string.Format(format, args), LogLevel.Error);
		}

		public void LogInfo(string message)
		{
			this.LogMessage(message, LogLevel.Info);
		}

		public void LogInfo(string format, params object[] args)
		{
			this.LogMessage(string.Format(format, args), LogLevel.Info);
		}

		private void LogMessage(string message, LogLevel level)
		{
			object mQueuedLogs = this.m_queuedLogs;
			Monitor.Enter(mQueuedLogs);
			try
			{
				List<LogThreadHelper.LogEntry> logEntries = this.m_queuedLogs;
				LogThreadHelper.LogEntry logEntry = new LogThreadHelper.LogEntry()
				{
					Message = message,
					Level = level
				};
				logEntries.Add(logEntry);
			}
			finally
			{
				Monitor.Exit(mQueuedLogs);
			}
		}

		public void LogWarning(string message)
		{
			this.LogMessage(message, LogLevel.Warning);
		}

		public void LogWarning(string format, params object[] args)
		{
			this.LogMessage(string.Format(format, args), LogLevel.Warning);
		}

		public void Process()
		{
			object mQueuedLogs = this.m_queuedLogs;
			Monitor.Enter(mQueuedLogs);
			try
			{
				foreach (LogThreadHelper.LogEntry mQueuedLog in this.m_queuedLogs)
				{
					switch (mQueuedLog.Level)
					{
						case LogLevel.Debug:
						{
							this.m_logSource.LogDebug(mQueuedLog.Message);
							continue;
						}
						case LogLevel.Info:
						{
							this.m_logSource.LogInfo(mQueuedLog.Message);
							continue;
						}
						case LogLevel.Warning:
						{
							this.m_logSource.LogWarning(mQueuedLog.Message);
							continue;
						}
						case LogLevel.Error:
						{
							this.m_logSource.LogError(mQueuedLog.Message);
							continue;
						}
						default:
						{
							goto case LogLevel.Debug;
						}
					}
				}
				this.m_queuedLogs.Clear();
			}
			finally
			{
				Monitor.Exit(mQueuedLogs);
			}
		}

		private class LogEntry
		{
			public string Message;

			public LogLevel Level;

			public LogEntry()
			{
			}
		}
	}
}