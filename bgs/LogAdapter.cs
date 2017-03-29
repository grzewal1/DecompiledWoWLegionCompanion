using bgs.Wrapper.Impl;
using System;
using System.Collections.Generic;

namespace bgs
{
	public class LogAdapter
	{
		private static LoggerInterface s_impl;

		static LogAdapter()
		{
			LogAdapter.s_impl = new DefaultLogger();
		}

		public LogAdapter()
		{
		}

		public static void ClearLogEvents()
		{
			LogAdapter.s_impl.ClearLogEvents();
		}

		public static List<string> GetLogEvents()
		{
			return LogAdapter.s_impl.GetLogEvents();
		}

		public static void Log(LogLevel logLevel, string str)
		{
			LogAdapter.s_impl.Log(logLevel, str);
		}

		public static void SetLogger<T>(T outputter)
		where T : LoggerInterface, new()
		{
			LogAdapter.s_impl = outputter;
		}
	}
}