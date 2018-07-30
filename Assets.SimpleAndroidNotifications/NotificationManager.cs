using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.SimpleAndroidNotifications
{
	public static class NotificationManager
	{
		public const string FullClassName = "com.hippogames.simpleandroidnotifications.Controller";

		public const string MainActivityClassName = "com.blizzard.wowcompanion.CompanionNativeActivity";

		public static void Cancel(int id)
		{
			(new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller")).CallStatic("CancelNotification", new object[] { id });
			NotificationIdHandler.RemoveScheduledNotificaion(id);
		}

		public static void CancelAll()
		{
			(new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller")).CallStatic("CancelAllNotifications", new object[0]);
			NotificationIdHandler.RemoveAllScheduledNotificaions();
		}

		public static void CancelAllDisplayed()
		{
			(new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller")).CallStatic("CancelAllDisplayedNotifications", new object[0]);
			NotificationIdHandler.RemoveAllScheduledNotificaions();
		}

		private static int ColotToInt(Color color)
		{
			Color32 color32 = color;
			return color32.r * 65536 + color32.g * 256 + color32.b;
		}

		public static NotificationCallback GetNotificationCallback()
		{
			AndroidJavaObject @static = (new AndroidJavaClass("com.unity3d.player.UnityPlayer")).GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getIntent", new object[0]);
			if (!androidJavaObject.Call<bool>("hasExtra", new object[] { "Notification.Id" }))
			{
				return null;
			}
			AndroidJavaObject androidJavaObject1 = androidJavaObject.Call<AndroidJavaObject>("getExtras", new object[0]);
			NotificationCallback notificationCallback = new NotificationCallback()
			{
				Id = androidJavaObject1.Call<int>("getInt", new object[] { "Notification.Id" }),
				Data = androidJavaObject1.Call<string>("getString", new object[] { "Notification.CallbackData" })
			};
			return notificationCallback;
		}

		private static string GetSmallIconName(NotificationIcon icon)
		{
			return string.Concat("anp_", icon.ToString().ToLower());
		}

		public static int Send(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0, bool silent = false)
		{
			NotificationParams notificationParam = new NotificationParams()
			{
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = delay,
				Title = title,
				Message = message,
				Ticker = message,
				Sound = !silent,
				Vibrate = !silent,
				Light = true,
				SmallIcon = smallIcon,
				SmallIconColor = smallIconColor,
				LargeIcon = string.Empty,
				ExecuteMode = NotificationExecuteMode.Inexact
			};
			return NotificationManager.SendCustom(notificationParam);
		}

		public static int SendCustom(NotificationParams notificationParams)
		{
			NotificationParams notificationParam = notificationParams;
			long totalMilliseconds = (long)notificationParam.Delay.TotalMilliseconds;
			long num = (!notificationParam.Repeat ? (long)0 : (long)notificationParam.RepeatInterval.TotalMilliseconds);
			string str = string.Join(",", (
				from i in (IEnumerable<int>)notificationParam.Vibration
				select i.ToString()).ToArray<string>());
			(new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller")).CallStatic("SetNotification", new object[] { notificationParam.Id, notificationParam.GroupName ?? string.Empty, notificationParam.GroupSummary ?? string.Empty, notificationParam.ChannelId, notificationParam.ChannelName, totalMilliseconds, Convert.ToInt32(notificationParam.Repeat), num, notificationParam.Title, notificationParam.Message, notificationParam.Ticker, Convert.ToInt32(notificationParam.Multiline), Convert.ToInt32(notificationParam.Sound), notificationParam.CustomSound ?? string.Empty, Convert.ToInt32(notificationParam.Vibrate), str, Convert.ToInt32(notificationParam.Light), notificationParam.LightOnMs, notificationParam.LightOffMs, NotificationManager.ColotToInt(notificationParam.LightColor), notificationParam.LargeIcon ?? string.Empty, NotificationManager.GetSmallIconName(notificationParam.SmallIcon), NotificationManager.ColotToInt(notificationParam.SmallIconColor), (int)notificationParam.ExecuteMode, notificationParam.CallbackData, "com.blizzard.wowcompanion.CompanionNativeActivity" });
			NotificationIdHandler.AddScheduledNotificaion(notificationParam.Id);
			return notificationParams.Id;
		}

		public static int SendWithAppIcon(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0, bool silent = false)
		{
			NotificationParams notificationParam = new NotificationParams()
			{
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = delay,
				Title = title,
				Message = message,
				Ticker = message,
				Sound = !silent,
				Vibrate = !silent,
				Light = true,
				SmallIcon = smallIcon,
				SmallIconColor = smallIconColor,
				LargeIcon = "app_icon",
				ExecuteMode = NotificationExecuteMode.Inexact
			};
			return NotificationManager.SendCustom(notificationParam);
		}
	}
}