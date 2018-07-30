using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleAndroidNotifications
{
	public class NotificationExample : MonoBehaviour
	{
		public UnityEngine.UI.Toggle Toggle;

		public NotificationExample()
		{
		}

		public void Awake()
		{
			this.Toggle.isOn = NotificationManager.GetNotificationCallback() != null;
		}

		public void CancelAll()
		{
			NotificationManager.CancelAll();
		}

		public void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				this.Toggle.isOn = NotificationManager.GetNotificationCallback() != null;
			}
		}

		public void OpenWiki()
		{
			Application.OpenURL("https://github.com/hippogamesunity/SimpleAndroidNotificationsPublic/wiki");
		}

		public void Rate()
		{
			Application.OpenURL("http://u3d.as/A6d");
		}

		public void ScheduleCustom()
		{
			NotificationParams notificationParam = new NotificationParams()
			{
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = TimeSpan.FromSeconds(5),
				Title = "Notification with callback",
				Message = "Open app and check the checkbox!",
				Ticker = "Notification with callback",
				Sound = true,
				Vibrate = true,
				Vibration = new int[] { 500, 500, 500, 500, 500, 500 },
				Light = true,
				LightOnMs = 1000,
				LightOffMs = 1000,
				LightColor = Color.red,
				SmallIcon = NotificationIcon.Sync,
				SmallIconColor = new Color(0f, 0.5f, 0f),
				LargeIcon = "app_icon",
				ExecuteMode = NotificationExecuteMode.Inexact,
				CallbackData = string.Concat("notification created at ", DateTime.Now)
			};
			NotificationManager.SendCustom(notificationParam);
		}

		public void ScheduleGrouped()
		{
			int notificationId = NotificationIdHandler.GetNotificationId();
			NotificationParams notificationParam = new NotificationParams()
			{
				Id = notificationId,
				GroupName = "Group",
				GroupSummary = "{0} new messages",
				Delay = TimeSpan.FromSeconds(5),
				Title = "Grouped notification",
				Message = string.Concat("Message ", notificationId),
				Ticker = "Please rate the asset on the Asset Store!"
			};
			NotificationManager.SendCustom(notificationParam);
		}

		public void ScheduleMultiline()
		{
			NotificationParams notificationParam = new NotificationParams()
			{
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = TimeSpan.FromSeconds(5),
				Title = "Multiline notification",
				Message = "Line#1\nLine#2\nLine#3\nLine#4",
				Ticker = "This is multiline message ticker!",
				Multiline = true
			};
			NotificationManager.SendCustom(notificationParam);
		}

		public void ScheduleNormal()
		{
			NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(5), "Notification", "Notification with app icon", new Color(0f, 0.6f, 1f), NotificationIcon.Message, false);
		}

		public void ScheduleRepeated()
		{
			NotificationParams notificationParam = new NotificationParams()
			{
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = TimeSpan.FromSeconds(5),
				Title = "Repeated notification",
				Message = "Please rate the asset on the Asset Store!",
				Ticker = "This is repeated message ticker!",
				Sound = true,
				Vibrate = true,
				Vibration = new int[] { 500, 500, 500, 500, 500, 500 },
				Light = true,
				LightOnMs = 1000,
				LightOffMs = 1000,
				LightColor = Color.magenta,
				SmallIcon = NotificationIcon.Skull,
				SmallIconColor = new Color(0f, 0.5f, 0f),
				LargeIcon = "app_icon",
				ExecuteMode = NotificationExecuteMode.Inexact,
				Repeat = true,
				RepeatInterval = TimeSpan.FromSeconds(30)
			};
			NotificationManager.SendCustom(notificationParam);
		}

		public void ScheduleSimple(int seconds)
		{
			NotificationManager.Send(TimeSpan.FromSeconds((double)seconds), "Simple notification", "Please rate the asset on the Asset Store!", new Color(1f, 0.3f, 0.15f), NotificationIcon.Bell, false);
		}

		public void ScheduleWithChannel()
		{
			NotificationParams notificationParam = new NotificationParams()
			{
				Id = NotificationIdHandler.GetNotificationId(),
				Delay = TimeSpan.FromSeconds(5),
				Title = "Notification with news channel",
				Message = "Check the channel in your app settings!",
				Ticker = "Notification with news channel",
				ChannelId = "com.company.app.news",
				ChannelName = "News"
			};
			NotificationManager.SendCustom(notificationParam);
		}
	}
}