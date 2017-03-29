using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.SimpleAndroidNotifications
{
	public static class NotificationIdHandler
	{
		private const string PlayerPrefsKey = "NotificationHelper.Scheduled";

		public static void AddScheduledNotificaion(int notificationId)
		{
			List<int> scheduledNotificaions = NotificationIdHandler.GetScheduledNotificaions();
			scheduledNotificaions.Add(notificationId);
			NotificationIdHandler.SetScheduledNotificaions(scheduledNotificaions);
		}

		public static int GetNotificationId()
		{
			int num;
			List<int> scheduledNotificaions = NotificationIdHandler.GetScheduledNotificaions();
			while (true)
			{
				num = UnityEngine.Random.Range(0, 2147483647);
				if (!scheduledNotificaions.Contains(num))
				{
					break;
				}
			}
			return num;
		}

		public static List<int> GetScheduledNotificaions()
		{
			return (!PlayerPrefs.HasKey("NotificationHelper.Scheduled") ? new List<int>() : (
				from i in PlayerPrefs.GetString("NotificationHelper.Scheduled").Split(new char[] { '|' })
				where i != string.Empty
				select int.Parse(i)).ToList<int>());
		}

		public static void RemoveScheduledNotificaion(int id)
		{
			List<int> scheduledNotificaions = NotificationIdHandler.GetScheduledNotificaions();
			scheduledNotificaions.RemoveAll((int i) => i == id);
			NotificationIdHandler.SetScheduledNotificaions(scheduledNotificaions);
		}

		public static void SetScheduledNotificaions(List<int> scheduledNotificaions)
		{
			PlayerPrefs.SetString("NotificationHelper.Scheduled", string.Join("|", (
				from i in scheduledNotificaions
				select i.ToString()).ToArray<string>()));
		}
	}
}