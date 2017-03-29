using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class BLPushManager : MonoBehaviour
{
	public static BLPushManager instance;

	public BLPushHandler pushHandler;

	private AndroidJavaObject currentContext;

	public BLPushManager()
	{
	}

	private void Awake()
	{
		if (BLPushManager.instance == null)
		{
			BLPushManager.instance = this;
		}
		else if (BLPushManager.instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		base.name = "BLPushManager";
	}

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void InitializePushManager();

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void InitializePushManagerBuilderUnity();

	public void InitWithBuilder(BLPushManagerBuilder builder)
	{
		BLPushHandler.builder = builder;
		if (Application.platform != RuntimePlatform.Android)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				BLPushManager.InitializePushManagerBuilderUnity();
				BLPushManager.SetApplicationName(builder.applicationName);
				BLPushManager.SetRegion(builder.region);
				BLPushManager.SetLocale(builder.locale);
				BLPushManager.SetAuthRegion(builder.authRegion);
				BLPushManager.SetAuthToken(builder.authToken);
				BLPushManager.SetAppAccountId(builder.appAccountID);
				BLPushManager.SetShouldRegisterWithBPNS(builder.shouldRegisterwithBPNS);
				BLPushManager.SetIsDebug(builder.isDebug);
				BLPushManager.SetUnityPushEventHandler();
				BLPushManager.InitializePushManager();
			}
		}
	}

	public void RegisterForPushNotifications()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			this.RegisterForPushNotificationsAndroid();
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			BLPushManager.RegisterForPushNotificationsIOS();
		}
	}

	public void RegisterForPushNotificationsAndroid()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			this.currentContext = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		}
		using (AndroidJavaClass androidJavaClass1 = new AndroidJavaClass("com.blizzard.pushlibrary.BlizzardPush"))
		{
			if (androidJavaClass1 != null)
			{
				BLPushManagerBuilder bLPushManagerBuilder = BLPushHandler.builder;
				androidJavaClass1.CallStatic("initialize", new object[] { this.currentContext, bLPushManagerBuilder.applicationName, bLPushManagerBuilder.senderId, bLPushManagerBuilder.region, bLPushManagerBuilder.locale, bLPushManagerBuilder.authRegion, bLPushManagerBuilder.authToken, bLPushManagerBuilder.appAccountID });
			}
		}
	}

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void RegisterForPushNotificationsIOS();

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void SetAppAccountId(string appAccountId);

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void SetApplicationName(string applicationName);

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void SetAuthRegion(string authRegion);

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void SetAuthToken(string authToken);

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void SetIsDebug(bool isDebug);

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void SetLocale(string locale);

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void SetRegion(string region);

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void SetShouldRegisterWithBPNS(bool shouldRegisterWithBPNS);

	[DllImport("__Internal", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern void SetUnityPushEventHandler();
}