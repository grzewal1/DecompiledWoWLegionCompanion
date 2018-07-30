using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class WebAuth
	{
		private string m_url;

		private Rect m_window;

		private bool m_show;

		private string m_callbackGameObject;

		private static bool m_isNewCreatedAccount;

		private static AndroidJavaClass m_androidWebLoginActivity;

		static WebAuth()
		{
			WebAuth.m_isNewCreatedAccount = false;
			WebAuth.m_androidWebLoginActivity = new AndroidJavaClass("com.blizzard.wowcompanion.WebLoginActivity");
		}

		public WebAuth(string url, float x, float y, float width, float height, string gameObjectName)
		{
			this.m_url = url;
			this.m_window = new Rect(x, y, width, height);
			this.m_callbackGameObject = gameObjectName;
			WebAuth.m_isNewCreatedAccount = false;
		}

		public static void ClearBrowserCache()
		{
			WebAuth.ClearURLCache();
		}

		public static void ClearLoginData()
		{
			WebAuth.DeleteStoredToken();
			WebAuth.DeleteCookies();
			WebAuth.ClearBrowserCache();
		}

		private static void ClearURLCache()
		{
			WebAuth.m_androidWebLoginActivity.CallStatic("ClearURLCache", new object[0]);
		}

		private static void ClearWebViewCookies()
		{
			WebAuth.m_androidWebLoginActivity.CallStatic("ClearWebViewCookies", new object[0]);
		}

		public void Close()
		{
			WebAuth.CloseWebView();
		}

		private static void CloseWebView()
		{
			WebAuth.m_androidWebLoginActivity.CallStatic("Destroy", new object[0]);
		}

		public static void DeleteCookies()
		{
			WebAuth.ClearWebViewCookies();
		}

		private static void DeleteStoredLoginToken()
		{
			WebAuth.m_androidWebLoginActivity.CallStatic("DeleteStoredLoginToken", new object[0]);
		}

		public static void DeleteStoredToken()
		{
			WebAuth.DeleteStoredLoginToken();
		}

		public WebAuth.Error GetError()
		{
			return WebAuth.Error.InternalError;
		}

		public static bool GetIsNewCreatedAccount()
		{
			return WebAuth.m_isNewCreatedAccount;
		}

		public string GetLoginCode()
		{
			Debug.Log("GetLoginCode");
			return WebAuth.GetWebViewLoginCode();
		}

		public WebAuth.Status GetStatus()
		{
			int webViewStatus = WebAuth.GetWebViewStatus();
			if (webViewStatus == 6)
			{
				Debug.Log("------------ Web View status no instance ------------");
			}
			if (webViewStatus >= 0 && webViewStatus < 7)
			{
				return (WebAuth.Status)webViewStatus;
			}
			return WebAuth.Status.Error;
		}

		private static string GetStoredLoginToken()
		{
			return WebAuth.m_androidWebLoginActivity.CallStatic<string>("GetStoredLoginToken", new object[0]);
		}

		public static string GetStoredToken()
		{
			return WebAuth.GetStoredLoginToken();
		}

		private static string GetWebViewLoginCode()
		{
			if (!WebAuth.IsWebLoginActivityReady())
			{
				return string.Empty;
			}
			AndroidJavaObject androidJavaObject = WebAuth.m_androidWebLoginActivity.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			return androidJavaObject.Call<string>("GetWebViewLoginCode", new object[0]);
		}

		private static int GetWebViewStatus()
		{
			return WebAuth.m_androidWebLoginActivity.CallStatic<int>("GetWebViewStatus", new object[0]);
		}

		private static void GoBack()
		{
			if (!WebAuth.IsWebLoginActivityReady())
			{
				return;
			}
			AndroidJavaObject androidJavaObject = WebAuth.m_androidWebLoginActivity.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			androidJavaObject.Call("GoBack", new object[0]);
		}

		public static void GoBackWebPage()
		{
			WebAuth.GoBack();
		}

		public void Hide()
		{
			if (!this.m_show)
			{
				return;
			}
			this.m_show = false;
			WebAuth.ShowWebView(false);
		}

		public bool IsShown()
		{
			return this.m_show;
		}

		private static bool IsWebLoginActivityReady()
		{
			return WebAuth.m_androidWebLoginActivity.CallStatic<bool>("IsWebLoginActivityReady", new object[0]);
		}

		public void Load()
		{
			Debug.Log("Load");
			WebAuth.LoadWebView(this.m_url, this.m_window.x, this.m_window.y, this.m_window.width, this.m_window.height, Main.uniqueIdentifier, this.m_callbackGameObject);
		}

		private static void LoadWebView(string url, float x, float y, float width, float height, string deviceUniqueIdentifier, string gameObjectName)
		{
			WebAuth.m_androidWebLoginActivity.CallStatic("Init", new object[] { url, x, y, width, height, Main.uniqueIdentifier, gameObjectName });
		}

		private static void SetBreakingNews(string localized_title, string body, string gameObjectName)
		{
			if (!WebAuth.IsWebLoginActivityReady())
			{
				return;
			}
			AndroidJavaObject androidJavaObject = WebAuth.m_androidWebLoginActivity.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			androidJavaObject.Call("SetBreakingNews", new object[] { localized_title, body, gameObjectName });
		}

		public void SetCountryCodeCookie(string countryCode, string domain)
		{
			WebAuth.SetWebViewCountryCodeCookie(countryCode, domain);
		}

		public static void SetIsNewCreatedAccount(bool isNewCreatedAccount)
		{
			WebAuth.m_isNewCreatedAccount = isNewCreatedAccount;
		}

		private static void SetRegionSelectVisualState(bool isVisible)
		{
			if (!WebAuth.IsWebLoginActivityReady())
			{
				return;
			}
			AndroidJavaObject androidJavaObject = WebAuth.m_androidWebLoginActivity.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			androidJavaObject.Call("SetRegionSelectVisualState", new object[] { isVisible });
		}

		private static bool SetStoredLoginToken(string str)
		{
			return WebAuth.m_androidWebLoginActivity.CallStatic<bool>("SetStoredLoginToken", new object[] { str });
		}

		public static bool SetStoredToken(string str)
		{
			return WebAuth.SetStoredLoginToken(str);
		}

		private static void SetWebViewCountryCodeCookie(string countryCode, string domain)
		{
			WebAuth.m_androidWebLoginActivity.CallStatic("SetWebViewCountryCodeCookie", new object[] { countryCode, domain });
		}

		public void Show()
		{
			if (this.m_show)
			{
				return;
			}
			this.m_show = true;
			WebAuth.ShowWebView(true);
		}

		private static void ShowWebView(bool show)
		{
			AndroidJavaObject androidJavaObject = WebAuth.m_androidWebLoginActivity.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			androidJavaObject.Call("ShowWebView", new object[] { show });
		}

		public static void UpdateBreakingNews(string title, string body, string gameObjectName)
		{
			WebAuth.SetBreakingNews(title, body, gameObjectName);
		}

		public static void UpdateRegionSelectVisualState(bool isVisible)
		{
			WebAuth.SetRegionSelectVisualState(isVisible);
		}

		public enum Error
		{
			InternalError
		}

		public enum Status
		{
			Closed,
			Loading,
			ReadyToDisplay,
			Processing,
			Success,
			Error,
			NoInstance,
			MAX
		}
	}
}