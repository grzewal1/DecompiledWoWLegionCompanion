using System;
using System.Collections.Generic;
using WowStaticData;

namespace WoWCompanionApp
{
	public class PersistentMissionData
	{
		private static PersistentMissionData s_instance;

		private Dictionary<int, WrapperGarrisonMission> m_missionDictionary = new Dictionary<int, WrapperGarrisonMission>();

		private static PersistentMissionData instance
		{
			get
			{
				if (PersistentMissionData.s_instance == null)
				{
					PersistentMissionData.s_instance = new PersistentMissionData();
				}
				return PersistentMissionData.s_instance;
			}
		}

		public static IDictionary<int, WrapperGarrisonMission> missionDictionary
		{
			get
			{
				return PersistentMissionData.instance.m_missionDictionary;
			}
		}

		static PersistentMissionData()
		{
		}

		public PersistentMissionData()
		{
		}

		public static void AddMission(WrapperGarrisonMission mission)
		{
			if (!PersistentMissionData.instance.m_missionDictionary.ContainsKey(mission.MissionRecID))
			{
				PersistentMissionData.instance.m_missionDictionary.Add(mission.MissionRecID, mission);
			}
		}

		public static void ClearData()
		{
			PersistentMissionData.instance.m_missionDictionary.Clear();
		}

		public static void GetAvailableAndProgressCounts(ref int numAvailable, ref int numInProgress)
		{
			foreach (WrapperGarrisonMission value in PersistentMissionData.missionDictionary.Values)
			{
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(value.MissionRecID);
				if (record != null && record.GarrFollowerTypeID == (uint)GarrisonStatus.GarrisonFollowerType && (record.Flags & 16) == 0)
				{
					if (value.MissionState == 1 || value.MissionState == 2 || value.MissionState == 3)
					{
						numInProgress++;
					}
					else
					{
						numAvailable++;
					}
				}
			}
		}

		public static int GetNumCompletedMissions(bool skipSupportMissions = false)
		{
			int num = 0;
			foreach (WrapperGarrisonMission value in PersistentMissionData.missionDictionary.Values)
			{
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(value.MissionRecID);
				if (record != null)
				{
					if (record.GarrFollowerTypeID == (uint)GarrisonStatus.GarrisonFollowerType)
					{
						if (!skipSupportMissions || (record.Flags & 16) == 0)
						{
							TimeSpan timeSpan = GarrisonStatus.CurrentTime() - value.StartTime;
							TimeSpan missionDuration = value.MissionDuration - timeSpan;
							if ((value.MissionState != 1 || missionDuration.TotalSeconds > 0) && value.MissionState != 2 && value.MissionState != 3)
							{
								continue;
							}
							num++;
						}
					}
				}
			}
			return num;
		}

		public static void UpdateMission(WrapperGarrisonMission mission)
		{
			PersistentMissionData.instance.m_missionDictionary.Remove(mission.MissionRecID);
			PersistentMissionData.AddMission(mission);
		}
	}
}