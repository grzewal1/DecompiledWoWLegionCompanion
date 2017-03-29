using System;
using System.Collections;

public class MissionDataCache
{
	private static MissionDataCache s_instance;

	private Hashtable m_missionDataDictionary;

	private static MissionDataCache instance
	{
		get
		{
			if (MissionDataCache.s_instance == null)
			{
				MissionDataCache.s_instance = new MissionDataCache()
				{
					m_missionDataDictionary = new Hashtable()
				};
			}
			return MissionDataCache.s_instance;
		}
	}

	public static Hashtable missionDataDictionary
	{
		get
		{
			return MissionDataCache.instance.m_missionDataDictionary;
		}
	}

	static MissionDataCache()
	{
	}

	public MissionDataCache()
	{
	}

	public static void AddOrUpdateMissionData(int garrMissionID, int successChance)
	{
		if (MissionDataCache.instance.m_missionDataDictionary.ContainsKey(garrMissionID))
		{
			MissionDataCache.instance.m_missionDataDictionary.Remove(garrMissionID);
			MissionDataCache.instance.m_missionDataDictionary.Add(garrMissionID, successChance);
		}
		else
		{
			MissionDataCache.instance.m_missionDataDictionary.Add(garrMissionID, successChance);
		}
	}

	public static void ClearData()
	{
		MissionDataCache.instance.m_missionDataDictionary.Clear();
	}
}