using System;
using System.Runtime.CompilerServices;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class GarrisonStatus
	{
		private static DateTime s_serverConnectTime;

		private static DateTime s_clientConnectTime;

		private static int s_faction;

		private static int s_gold;

		private static int s_oil;

		private static int s_resources;

		private static int s_characterLevel;

		private static int s_characterClassID;

		private static string s_characterClassName;

		private static string s_characterName;

		private static int s_remainingFollowerActivations;

		private static int s_followerActivationGoldCost;

		private static int s_maxActiveFollowers;

		private readonly static string s_garrisonConfigFileName;

		private static GarrisonConfig s_garrisonConfig;

		public static int ArtifactKnowledgeLevel
		{
			get;
			set;
		}

		public static float ArtifactXpMultiplier
		{
			get;
			set;
		}

		public static GARR_FOLLOWER_TYPE GarrisonFollowerType
		{
			get
			{
				if (GarrisonStatus.s_garrisonConfig == null)
				{
					GarrisonStatus.LoadGarrisonConfig();
				}
				return GarrisonStatus.s_garrisonConfig.m_garrrisonFollowerType;
			}
		}

		public static GARR_TYPE GarrisonType
		{
			get
			{
				if (GarrisonStatus.s_garrisonConfig == null)
				{
					GarrisonStatus.LoadGarrisonConfig();
				}
				return GarrisonStatus.s_garrisonConfig.m_garrisonType;
			}
		}

		public static bool Initialized
		{
			get;
			set;
		}

		static GarrisonStatus()
		{
			GarrisonStatus.s_garrisonConfigFileName = "GarrisonConfig.asset";
		}

		public GarrisonStatus()
		{
		}

		public static int CharacterClassID()
		{
			return GarrisonStatus.s_characterClassID;
		}

		public static string CharacterClassName()
		{
			return GarrisonStatus.s_characterClassName;
		}

		public static int CharacterLevel()
		{
			return GarrisonStatus.s_characterLevel;
		}

		public static string CharacterName()
		{
			return GarrisonStatus.s_characterName ?? "Character Name";
		}

		public static void CheatFastForwardOneHour()
		{
			GarrisonStatus.s_serverConnectTime = GarrisonStatus.s_serverConnectTime.AddHours(1);
		}

		public static DateTime CurrentTime()
		{
			return GarrisonStatus.s_serverConnectTime + (DateTime.UtcNow - GarrisonStatus.s_clientConnectTime);
		}

		public static PVP_FACTION Faction()
		{
			return (PVP_FACTION)GarrisonStatus.s_faction;
		}

		public static int GetFollowerActivationGoldCost()
		{
			return GarrisonStatus.s_followerActivationGoldCost;
		}

		public static int GetMaxActiveFollowers()
		{
			return GarrisonStatus.s_maxActiveFollowers;
		}

		public static int GetRemainingFollowerActivations()
		{
			return GarrisonStatus.s_remainingFollowerActivations;
		}

		public static int Gold()
		{
			return GarrisonStatus.s_gold;
		}

		private static void LoadGarrisonConfig()
		{
			GarrisonStatus.s_garrisonConfig = AssetBundleManager.LoadAsset<GarrisonConfig>(AssetBundleManager.BundleName.ConfigBundleName, GarrisonStatus.s_garrisonConfigFileName);
		}

		public static int Oil()
		{
			return GarrisonStatus.s_oil;
		}

		public static int Resources()
		{
			return GarrisonStatus.s_resources;
		}

		public static void SetCharacterClass(int charClassID)
		{
			GarrisonStatus.s_characterClassID = charClassID;
			ChrClassesRec record = StaticDB.chrClassesDB.GetRecord(charClassID);
			if (record == null)
			{
				GarrisonStatus.s_characterClassName = string.Empty;
			}
			else
			{
				GarrisonStatus.s_characterClassName = record.Name;
			}
		}

		public static void SetCharacterLevel(int level)
		{
			GarrisonStatus.s_characterLevel = level;
		}

		public static void SetCharacterName(string name)
		{
			GarrisonStatus.s_characterName = name;
		}

		public static void SetCurrencies(int gold, int oil, int resources)
		{
			GarrisonStatus.s_gold = gold;
			GarrisonStatus.s_oil = oil;
			GarrisonStatus.s_resources = resources;
		}

		public static void SetFaction(int faction)
		{
			GarrisonStatus.s_faction = faction;
		}

		public static void SetFollowerActivationInfo(int remainingActivations, int activationGoldCost)
		{
			GarrisonStatus.s_remainingFollowerActivations = remainingActivations;
			GarrisonStatus.s_followerActivationGoldCost = activationGoldCost;
		}

		public static void SetGarrisonServerConnectTime(DateTime serverTime)
		{
			GarrisonStatus.s_serverConnectTime = serverTime;
			GarrisonStatus.s_clientConnectTime = DateTime.UtcNow;
		}

		public static void SetMaxActiveFollowers(int maxActiveFollowers)
		{
			GarrisonStatus.s_maxActiveFollowers = maxActiveFollowers;
		}
	}
}