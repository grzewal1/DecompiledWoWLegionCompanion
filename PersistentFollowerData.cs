using System;
using System.Collections.Generic;
using WowJamMessages;

public class PersistentFollowerData
{
	private static PersistentFollowerData s_instance;

	private Dictionary<int, JamGarrisonFollower> m_followerDictionary;

	private Dictionary<int, JamGarrisonFollower> m_preMissionFollowerDictionary;

	public static Dictionary<int, JamGarrisonFollower> followerDictionary
	{
		get
		{
			return PersistentFollowerData.instance.m_followerDictionary;
		}
	}

	private static PersistentFollowerData instance
	{
		get
		{
			if (PersistentFollowerData.s_instance == null)
			{
				PersistentFollowerData.s_instance = new PersistentFollowerData()
				{
					m_followerDictionary = new Dictionary<int, JamGarrisonFollower>(),
					m_preMissionFollowerDictionary = new Dictionary<int, JamGarrisonFollower>()
				};
			}
			return PersistentFollowerData.s_instance;
		}
	}

	public static Dictionary<int, JamGarrisonFollower> preMissionFollowerDictionary
	{
		get
		{
			return PersistentFollowerData.instance.m_preMissionFollowerDictionary;
		}
	}

	static PersistentFollowerData()
	{
	}

	public PersistentFollowerData()
	{
	}

	public static void AddOrUpdateFollower(JamGarrisonFollower follower)
	{
		if (PersistentFollowerData.instance.m_followerDictionary.ContainsKey(follower.GarrFollowerID))
		{
			PersistentFollowerData.instance.m_followerDictionary.Remove(follower.GarrFollowerID);
		}
		PersistentFollowerData.instance.m_followerDictionary.Add(follower.GarrFollowerID, follower);
	}

	public static void CachePreMissionFollower(JamGarrisonFollower follower)
	{
		if (PersistentFollowerData.instance.m_preMissionFollowerDictionary.ContainsKey(follower.GarrFollowerID))
		{
			PersistentFollowerData.instance.m_preMissionFollowerDictionary.Remove(follower.GarrFollowerID);
		}
		PersistentFollowerData.instance.m_preMissionFollowerDictionary.Add(follower.GarrFollowerID, follower);
	}

	public static void ClearData()
	{
		PersistentFollowerData.instance.m_followerDictionary.Clear();
	}

	public static void ClearPreMissionFollowerData()
	{
		PersistentFollowerData.instance.m_preMissionFollowerDictionary.Clear();
	}

	public static void UpdateFollower(JamGarrisonMissionFollowerInfo followerInfo)
	{
		foreach (KeyValuePair<int, JamGarrisonFollower> mFollowerDictionary in PersistentFollowerData.instance.m_followerDictionary)
		{
			if (mFollowerDictionary.Value.DbID != followerInfo.FollowerDBID)
			{
				continue;
			}
			mFollowerDictionary.Value.CurrentMissionID = 0;
			break;
		}
	}
}