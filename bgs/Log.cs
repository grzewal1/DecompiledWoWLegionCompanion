using System;

namespace bgs
{
	public class Log
	{
		public static Logger BattleNet;

		public static Logger Party;

		private static Log s_instance;

		static Log()
		{
			Log.BattleNet = new Logger();
			Log.Party = new Logger();
		}

		public Log()
		{
		}

		public static Log Get()
		{
			if (Log.s_instance == null)
			{
				Log.s_instance = new Log();
				Log.s_instance.Initialize();
			}
			return Log.s_instance;
		}

		private void Initialize()
		{
		}
	}
}