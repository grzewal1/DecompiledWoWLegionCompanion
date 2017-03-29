using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.SimpleAndroidNotifications
{
	public static class NotificationManager
	{
		private const string FullClassName = "com.hippogames.simpleandroidnotifications.Controller";

		private const string MainActivityClassName = "com.blizzard.wowcompanion.CompanionNativeActivity";

		public static void Cancel(int id)
		{
			(new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller")).CallStatic("CancelNotification", new object[] { id });
			NotificationIdHandler.RemoveScheduledNotificaion(id);
		}

		public static void CancelAll()
		{
			(new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller")).CallStatic("CancelAllNotifications", new object[0]);
		}

		private static int ColotToInt(Color color)
		{
			Color32 color32 = color;
			return color32.r * 65536 + color32.g * 256 + color32.b;
		}

		private static string GetSmallIconName(NotificationIcon icon)
		{
			return string.Concat("anp_", icon.ToString().ToLower());
		}

		public static int Send(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0)
		{
			NotificationParams notificationParam = new NotificationParams()
			{
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = delay,
				Title = title,
				Message = message,
				Ticker = message,
				Sound = true,
				Vibrate = true,
				Light = true,
				SmallIcon = smallIcon,
				SmallIconColor = smallIconColor,
				LargeIcon = string.Empty,
				Mode = NotificationExecuteMode.Schedule
			};
			return NotificationManager.SendCustom(notificationParam);
		}

		public static int SendCustom(NotificationParams notificationParams)
		{
			NotificationParams notificationParam = notificationParams;
			long totalMilliseconds = (long)notificationParam.Delay.TotalMilliseconds;
			string str = string.Join(",", (
				from i in (IEnumerable<int>)notificationParam.Vibration
				select i.ToString()).ToArray<string>());
			(new AndroidJavaClass("com.hippogames.simpleandroidnotifications.Controller")).CallStatic("SetNotification", new object[] { notificationParam.Id, totalMilliseconds, notificationParam.Title, notificationParam.Message, notificationParam.Ticker, (!notificationParam.Sound ? 0 : 1), (!notificationParam.Vibrate ? 0 : 1), str, (!notificationParam.Light ? 0 : 1), notificationParam.LightOnMs, notificationParam.LightOffMs, NotificationManager.ColotToInt(notificationParam.LightColor), notificationParam.LargeIcon, NotificationManager.GetSmallIconName(notificationParam.SmallIcon), NotificationManager.ColotToInt(notificationParam.SmallIconColor), (int)notificationParam.Mode, "com.blizzard.wowcompanion.CompanionNativeActivity" });
			NotificationIdHandler.AddScheduledNotificaion(notificationParam.Id);
			return notificationParams.Id;
		}

		public static int SendWithAppIcon(TimeSpan delay, string title, string message, Color smallIconColor, NotificationIcon smallIcon = 0)
		{
			NotificationParams notificationParam = new NotificationParams()
			{
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = delay,
				Title = title,
				Message = message,
				Ticker = message,
				Sound = true,
				Vibrate = true,
				Light = true,
				SmallIcon = smallIcon,
				SmallIconColor = smallIconColor,
				LargeIcon = "app_icon",
				Mode = NotificationExecuteMode.Schedule
			};
			return NotificationManager.SendCustom(notificationParam);
		}
	}
}