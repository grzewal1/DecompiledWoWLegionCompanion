using bgs;
using System;
using System.Collections.Generic;

namespace bgs.Wrapper.Impl
{
	internal class DefaultLogger : LoggerInterface
	{
		private static List<string> s_logEvents;

		static DefaultLogger()
		{
			DefaultLogger.s_logEvents = new List<string>();
		}

		public DefaultLogger()
		{
		}

		public void ClearLogEvents()
		{
			DefaultLogger.s_logEvents.Clear();
		}

		public List<string> GetLogEvents()
		{
			return DefaultLogger.s_logEvents;
		}

		public void Log(LogLevel logLevel, string str)
		{
			Console.WriteLine(str);
			DefaultLogger.s_logEvents.Add(str);
		}
	}
}