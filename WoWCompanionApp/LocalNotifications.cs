using Assets.SimpleAndroidNotifications;
using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class LocalNotifications
	{
		public LocalNotifications()
		{
		}

		public static void ClearPending()
		{
			NotificationManager.CancelAll();
		}

		public static void RegisterForNotifications()
		{
		}

		public static void ScheduleMissionCompleteNotification(string missionName, int badgeNumber, long secondsFromNow)
		{
			if (secondsFromNow < (long)0)
			{
				return;
			}
			NotificationParams notificationParam = new NotificationParams()
			{
				Delay = TimeSpan.FromSeconds((double)secondsFromNow),
				Title = StaticDB.GetString("MISSION_COMPLETE2", null),
				Message = missionName,
				SmallIconColor = Color.black,
				SmallIcon = NotificationIcon.Mission,
				Sound = true,
				CustomSound = "ui_mission_complete_toast_n"
			};
			NotificationManager.SendCustom(notificationParam);
		}

		public static void ScheduleTalentResearchCompleteNotification(string talentName, int badgeNumber, long secondsFromNow)
		{
			if (secondsFromNow < (long)0)
			{
				return;
			}
			NotificationParams notificationParam = new NotificationParams()
			{
				Delay = TimeSpan.FromSeconds((double)secondsFromNow),
				Title = StaticDB.GetString("RESEARCH_COMPLETE", null),
				Message = talentName,
				SmallIconColor = Color.black,
				SmallIcon = NotificationIcon.Talent,
				Sound = true,
				CustomSound = "ui_orderhall_talent_ready_toast_n"
			};
			NotificationManager.SendCustom(notificationParam);
		}

		public static void ScheduleWorkOrderReadyNotification(string workOrderName, int badgeNumber, long secondsFromNow)
		{
			if (secondsFromNow < (long)0)
			{
				return;
			}
			NotificationParams notificationParam = new NotificationParams()
			{
				Delay = TimeSpan.FromSeconds((double)secondsFromNow),
				Title = StaticDB.GetString("READY_FOR_PICKUP", null),
				Message = workOrderName,
				SmallIconColor = Color.black,
				SmallIcon = NotificationIcon.WorkOrder,
				Sound = true,
				CustomSound = "ui_mission_troops_ready_toast_n"
			};
			NotificationManager.SendCustom(notificationParam);
		}
	}
}