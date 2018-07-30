using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WoWCompanionApp
{
	public class PersistentFollowerData
	{
		private static PersistentFollowerData s_instance;

		private Dictionary<int, WrapperGarrisonFollower> m_followerDictionary = new Dictionary<int, WrapperGarrisonFollower>();

		private Dictionary<int, WrapperGarrisonFollower> m_preMissionFollowerDictionary = new Dictionary<int, WrapperGarrisonFollower>();

		public static Dictionary<int, WrapperGarrisonFollower> followerDictionary
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
					PersistentFollowerData.s_instance = new PersistentFollowerData();
				}
				return PersistentFollowerData.s_instance;
			}
		}

		public static Dictionary<int, WrapperGarrisonFollower> preMissionFollowerDictionary
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

		public static void AddOrUpdateFollower(WrapperGarrisonFollower follower)
		{
			if (PersistentFollowerData.instance.m_followerDictionary.ContainsKey(follower.GarrFollowerID))
			{
				PersistentFollowerData.instance.m_followerDictionary.Remove(follower.GarrFollowerID);
			}
			PersistentFollowerData.instance.m_followerDictionary.Add(follower.GarrFollowerID, follower);
		}

		public static void CachePreMissionFollower(WrapperGarrisonFollower follower)
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

		public static void UpdateFollower(WrapperGarrisonMissionFollowerInfo followerInfo)
		{
			if (!PersistentFollowerData.instance.m_followerDictionary.Any<KeyValuePair<int, WrapperGarrisonFollower>>((KeyValuePair<int, WrapperGarrisonFollower> f) => f.Value.DbID == followerInfo.FollowerDBID))
			{
				Debug.LogError(string.Concat("Could not find follower with DB ID ", followerInfo.FollowerDBID));
				return;
			}
			KeyValuePair<int, WrapperGarrisonFollower> keyValuePair = PersistentFollowerData.instance.m_followerDictionary.First<KeyValuePair<int, WrapperGarrisonFollower>>((KeyValuePair<int, WrapperGarrisonFollower> f) => f.Value.DbID == followerInfo.FollowerDBID);
			WrapperGarrisonFollower value = keyValuePair.Value;
			value.CurrentMissionID = 0;
			PersistentFollowerData.instance.m_followerDictionary[value.GarrFollowerID] = value;
		}
	}
}