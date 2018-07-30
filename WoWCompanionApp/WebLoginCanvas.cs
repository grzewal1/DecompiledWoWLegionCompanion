using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class WebLoginCanvas : MonoBehaviour
	{
		public WebLoginCanvas()
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void WebViewBackButtonPressed(string empty)
		{
			Debug.Log(string.Concat("--------------------------- WebViewBackButtonPressed: ", empty, " -------------------------------"));
		}

		public void WebViewDidFinishLoad(string pageState)
		{
			Debug.Log(string.Concat(new object[] { "--------------------------- WebViewDidFinishLoad: ", pageState, ", login state: ", Singleton<Login>.instance.GetLoginState(), " -------------------------------" }));
			if (pageState.Contains("STATE_AUTHENTICATOR"))
			{
				Debug.Log("WebViewDidFinishLoad: no action for authenticator state.");
			}
			else if (!Singleton<Login>.instance.IsWebAuthState())
			{
				Singleton<Login>.instance.CancelWebAuth();
				Debug.Log(string.Concat("WebViewDidFinishLoad: Did not show web auth view because not in web auth login state.", Singleton<Login>.instance.GetLoginState()));
			}
			else
			{
				Singleton<Login>.instance.ShowWebAuthView();
			}
		}

		public void WebViewReceivedToken(string token)
		{
			Debug.Log(string.Concat("--------------------------- WebViewReceivedToken: ", token, " -------------------------------"));
		}
	}
}