using bgs;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class MyClientInterface : ClientInterface
{
	public string m_temporaryCachePath = Application.persistentDataPath;

	public MyClientInterface()
	{
	}

	public string GetAuroraVersionName()
	{
		return "0";
	}

	public string GetBasePersistentDataPath()
	{
		return this.m_temporaryCachePath;
	}

	public bool GetDisableConnectionMetering()
	{
		return true;
	}

	public string GetLocaleName()
	{
		return Main.instance.GetLocale();
	}

	public constants.MobileEnv GetMobileEnvironment()
	{
		int num;
		string lower = Login.m_portal.ToLower();
		if (lower != null)
		{
			if (MyClientInterface.<>f__switch$map5 == null)
			{
				Dictionary<string, int> strs = new Dictionary<string, int>(7)
				{
					{ "us", 0 },
					{ "eu", 0 },
					{ "kr", 0 },
					{ "cn", 0 },
					{ "tw", 0 },
					{ "beta", 0 },
					{ "test", 0 }
				};
				MyClientInterface.<>f__switch$map5 = strs;
			}
			if (MyClientInterface.<>f__switch$map5.TryGetValue(lower, out num))
			{
				if (num == 0)
				{
					return constants.MobileEnv.PRODUCTION;
				}
			}
		}
		return constants.MobileEnv.DEVELOPMENT;
	}

	public string GetPlatformName()
	{
		return "And";
	}

	public constants.RuntimeEnvironment GetRuntimeEnvironment()
	{
		return constants.RuntimeEnvironment.Mono;
	}

	public string GetTemporaryCachePath()
	{
		return this.m_temporaryCachePath;
	}

	public IUrlDownloader GetUrlDownloader()
	{
		return Login.instance.m_urlDownloader;
	}

	public string GetUserAgent()
	{
		return null;
	}

	public string GetVersion()
	{
		return string.Empty;
	}

	public bool IsVersionInt()
	{
		return true;
	}

	private void SetCachePath(string cachePath)
	{
		this.m_temporaryCachePath = cachePath;
	}
}