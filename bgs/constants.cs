using System;
using System.ComponentModel;

namespace bgs
{
	public class constants
	{
		public const ushort RouteToAnyUtil = 0;

		public const float ResubsribeAttemptDelaySeconds = 120f;

		public constants()
		{
		}

		public enum BnetRegion
		{
			REGION_UNINITIALIZED = -1,
			REGION_UNKNOWN = 0,
			REGION_US = 1,
			REGION_EU = 2,
			REGION_KR = 3,
			REGION_TW = 4,
			REGION_CN = 5,
			REGION_LIVE_VERIFICATION = 40,
			REGION_PTR_LOC = 41,
			REGION_MSCHWEITZER_BN11 = 52,
			REGION_MSCHWEITZER_BN12 = 53,
			REGION_DEV = 60,
			REGION_PTR = 98
		}

		public enum BNetState
		{
			BATTLE_NET_UNKNOWN,
			BATTLE_NET_LOGGING_IN,
			BATTLE_NET_TIMEOUT,
			BATTLE_NET_LOGIN_FAILED,
			BATTLE_NET_LOGGED_IN
		}

		public enum MobileEnv
		{
			[Description("Development")]
			DEVELOPMENT,
			[Description("Production")]
			PRODUCTION
		}

		public enum RuntimeEnvironment
		{
			Mono,
			MSDotNet
		}
	}
}