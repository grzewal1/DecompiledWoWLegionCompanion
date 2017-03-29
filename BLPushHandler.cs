using System;
using UnityEngine;

public class BLPushHandler : MonoBehaviour
{
	public static BLPushManagerBuilder builder;

	public BLPushHandler()
	{
	}

	public void DidFailToLogout(string errorDescription)
	{
		Debug.Log(errorDescription);
		BLPushHandler.builder.didFailToLogoutDelegate(errorDescription);
	}

	public void DidFailToRegisterForRemoteNotificationsWithError(string errorDescription)
	{
		Debug.Log(errorDescription);
		BLPushHandler.builder.didFailToRegisterForRemoteNotificationsWithErrorDelegate(errorDescription);
	}

	public void DidReceiveDeeplinkURL(string url)
	{
		Debug.Log(string.Concat("unity ", url));
		BLPushHandler.builder.didReceiveDeeplinkURLDelegate(url);
	}

	public void DidReceiveRegistrationToken(string deviceToken)
	{
		Debug.Log(string.Concat("unity ", deviceToken));
		BLPushHandler.builder.didReceiveRegistrationTokenDelegate(deviceToken);
	}
}