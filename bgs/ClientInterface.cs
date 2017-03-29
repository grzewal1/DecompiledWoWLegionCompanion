using System;

namespace bgs
{
	public interface ClientInterface
	{
		string GetAuroraVersionName();

		string GetBasePersistentDataPath();

		bool GetDisableConnectionMetering();

		string GetLocaleName();

		constants.MobileEnv GetMobileEnvironment();

		string GetPlatformName();

		constants.RuntimeEnvironment GetRuntimeEnvironment();

		string GetTemporaryCachePath();

		IUrlDownloader GetUrlDownloader();

		string GetUserAgent();

		string GetVersion();

		bool IsVersionInt();
	}
}