using System;
using UnityEngine;

public class GameClient : MonoBehaviour
{
	public GameObject pushManager;

	public static GameClient instance;

	public GameClient()
	{
	}

	public void DidReceiveDeeplinkURLDelegateHandler(string url)
	{
		Debug.Log(string.Concat("DidReceiveDeeplinkURLDelegateHandler: url ", url));
	}

	public void DidReceiveRegistrationTokenHandler(string deviceToken)
	{
		Debug.Log(string.Concat("DidReceiveRegistrationTokenHandler: device token ", deviceToken));
	}

	public void RegisterPushManager(string token, string locale)
	{
		BLPushManagerBuilder empty = ScriptableObject.CreateInstance<BLPushManagerBuilder>();
		if (Login.m_portal.ToLower() != "wow-dev")
		{
			empty.isDebug = false;
			empty.applicationName = "wowcompanion";
		}
		else
		{
			empty.isDebug = true;
			empty.applicationName = "test.wowcompanion";
		}
		empty.shouldRegisterwithBPNS = true;
		empty.region = "US";
		empty.locale = locale;
		empty.authToken = token;
		empty.authRegion = "US";
		empty.appAccountID = string.Empty;
		empty.senderId = "952133414280";
		empty.didReceiveRegistrationTokenDelegate = new DidReceiveRegistrationTokenDelegate(this.DidReceiveRegistrationTokenHandler);
		empty.didReceiveDeeplinkURLDelegate = new DidReceiveDeeplinkURLDelegate(this.DidReceiveDeeplinkURLDelegateHandler);
		BLPushManager.instance.InitWithBuilder(empty);
		BLPushManager.instance.RegisterForPushNotifications();
	}

	private void Start()
	{
		if (BLPushManager.instance == null)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.pushManager);
		}
		GameClient.instance = this;
	}
}