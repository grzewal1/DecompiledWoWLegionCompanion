using System;
using System.Collections.Generic;

namespace bgs
{
	public class Logger
	{
		public Logger()
		{
		}

		public void ClearLogEvents()
		{
			LogAdapter.ClearLogEvents();
		}

		public LogLevel GetDefaultLevel()
		{
			return LogLevel.Debug;
		}

		public List<string> GetLogEvents()
		{
			return LogAdapter.GetLogEvents();
		}

		public void Print(string format, params object[] args)
		{
			this.Print(this.GetDefaultLevel(), format, args);
		}

		public void Print(LogLevel level, string format, params object[] args)
		{
			string str;
			str = ((int)args.Length != 0 ? string.Format(format, args) : format);
			this.Print(level, str);
		}

		public void Print(LogLevel level, string message)
		{
			LogAdapter.Log(level, message);
		}
	}
}