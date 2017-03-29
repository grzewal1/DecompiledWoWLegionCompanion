using Assets.SimpleAndroidNotifications;
using System;
using UnityEngine;

public class LocalNotifications
{
	public LocalNotifications()
	{
	}

	public static void ClearPending()
	{
		NotificationManager.CancelAll();
	}

	public static void ScheduleMissionCompleteNotification(string missionName, int badgeNumber, long secondsFromNow)
	{
		if (secondsFromNow < (long)0)
		{
			return;
		}
		NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds((double)secondsFromNow), StaticDB.GetString("MISSION_COMPLETE2", null), missionName, Color.black, NotificationIcon.Mission);
	}

	public static void ScheduleTalentResearchCompleteNotification(string talentName, int badgeNumber, long secondsFromNow)
	{
		if (secondsFromNow < (long)0)
		{
			return;
		}
		NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds((double)secondsFromNow), StaticDB.GetString("RESEARCH_COMPLETE", null), talentName, Color.black, NotificationIcon.Talent);
	}

	public static void ScheduleWorkOrderReadyNotification(string workOrderName, int badgeNumber, long secondsFromNow)
	{
		if (secondsFromNow < (long)0)
		{
			return;
		}
		NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds((double)secondsFromNow), StaticDB.GetString("READY_FOR_PICKUP", null), workOrderName, Color.black, NotificationIcon.WorkOrder);
	}
}