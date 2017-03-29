using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace bgs
{
	public class BattleNetLogSource
	{
		private string m_sourceName;

		public BattleNetLogSource(string sourceName)
		{
			this.m_sourceName = sourceName;
		}

		private string FormatStacktrace(StackFrame sf, bool fullPath = false)
		{
			if (sf == null)
			{
				return string.Empty;
			}
			string str = (!fullPath ? Path.GetFileName(sf.GetFileName()) : sf.GetFileName());
			return string.Format(" ({2} at {0}:{1})", str, sf.GetFileLineNumber(), sf.GetMethod());
		}

		public void LogDebug(string message)
		{
			this.LogMessage(LogLevel.Debug, message, true);
		}

		public void LogDebug(string format, params object[] args)
		{
			this.LogMessage(LogLevel.Debug, string.Format(format, args), true);
		}

		public void LogDebugStackTrace(string message, int maxFrames, int skipFrames = 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat(message, "\n"));
			int num = 1 + skipFrames;
			while (num < maxFrames)
			{
				StackTrace stackTrace = new StackTrace(new StackFrame(num, true));
				if (stackTrace != null)
				{
					StackFrame frame = stackTrace.GetFrame(0);
					if (frame == null)
					{
						break;
					}
					else if (frame.GetMethod() == null || frame.GetMethod().ToString().StartsWith("<"))
					{
						break;
					}
					else
					{
						stringBuilder.Append(string.Format("File \"{0}\", line {1} -- {2}\n", Path.GetFileName(frame.GetFileName()), frame.GetFileLineNumber(), frame.GetMethod()));
						num++;
					}
				}
				else
				{
					break;
				}
			}
			this.LogMessage(LogLevel.Debug, stringBuilder.ToString().TrimEnd(new char[0]), false);
		}

		public void LogError(string message)
		{
			this.LogMessage(LogLevel.Error, message, true);
		}

		public void LogError(string format, params object[] args)
		{
			this.LogMessage(LogLevel.Error, string.Format(format, args), true);
		}

		public void LogInfo(string message)
		{
			this.LogMessage(LogLevel.Info, message, true);
		}

		public void LogInfo(string format, params object[] args)
		{
			this.LogMessage(LogLevel.Info, string.Format(format, args), true);
		}

		private void LogMessage(LogLevel logLevel, string message, bool includeFilename = true)
		{
			StackTrace stackTrace = new StackTrace(new StackFrame(2, true));
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			stringBuilder.Append(this.m_sourceName);
			stringBuilder.Append("] ");
			stringBuilder.Append(message);
			if (stackTrace != null && includeFilename)
			{
				StackFrame frame = stackTrace.GetFrame(0);
				stringBuilder.Append(this.FormatStacktrace(frame, false));
			}
			Log.BattleNet.Print(logLevel, stringBuilder.ToString());
		}

		public void LogWarning(string message)
		{
			this.LogMessage(LogLevel.Warning, message, true);
		}

		public void LogWarning(string format, params object[] args)
		{
			this.LogMessage(LogLevel.Warning, string.Format(format, args), true);
		}
	}
}