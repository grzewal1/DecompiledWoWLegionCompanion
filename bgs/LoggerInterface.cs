using System;
using System.Collections.Generic;

namespace bgs
{
	public interface LoggerInterface
	{
		void ClearLogEvents();

		List<string> GetLogEvents();

		void Log(LogLevel logLevel, string str);
	}
}