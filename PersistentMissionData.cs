using System;
using System.Collections;
using WowJamMessages;
using WowStaticData;

public class PersistentMissionData
{
	private static PersistentMissionData s_instance;

	private Hashtable m_missionDictionary;

	private static PersistentMissionData instance
	{
		get
		{
			if (PersistentMissionData.s_instance == null)
			{
				PersistentMissionData.s_instance = new PersistentMissionData()
				{
					m_missionDictionary = new Hashtable()
				};
			}
			return PersistentMissionData.s_instance;
		}
	}

	public static Hashtable missionDictionary
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

	public static void AddMission(JamGarrisonMobileMission mission)
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

	public static int GetNumCompletedMissions(bool skipSupportMissions = false)
	{
		int num = 0;
		IEnumerator enumerator = PersistentMissionData.missionDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamGarrisonMobileMission current = (JamGarrisonMobileMission)enumerator.Current;
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(current.MissionRecID);
				if (record != null)
				{
					if (record.GarrFollowerTypeID == 4)
					{
						if (!skipSupportMissions || (record.Flags & 16) == 0)
						{
							long num1 = GarrisonStatus.CurrentTime() - current.StartTime;
							long missionDuration = current.MissionDuration - num1;
							if ((current.MissionState != 1 || missionDuration > (long)0) && current.MissionState != 2 && current.MissionState != 3)
							{
								continue;
							}
							num++;
						}
					}
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			IDisposable disposable1 = disposable;
			if (disposable != null)
			{
				disposable1.Dispose();
			}
		}
		return num;
	}

	public static void UpdateMission(JamGarrisonMobileMission mission)
	{
		PersistentMissionData.instance.m_missionDictionary.Remove(mission.MissionRecID);
		PersistentMissionData.AddMission(mission);
	}
}