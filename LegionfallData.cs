using System;
using System.Collections;
using WowJamMessages.MobileClientJSON;

public class LegionfallData
{
	private static LegionfallData s_instance;

	private int m_legionfallWarResources;

	private bool m_hasAccess;

	private Hashtable m_legionfallDictionary;

	private JamMobileAreaPOI m_currentInvasionPOI;

	private long m_invasionExpirationTime;

	private static LegionfallData instance
	{
		get
		{
			if (LegionfallData.s_instance == null)
			{
				LegionfallData.s_instance = new LegionfallData()
				{
					m_legionfallDictionary = new Hashtable(),
					m_legionfallWarResources = 0
				};
			}
			return LegionfallData.s_instance;
		}
	}

	public static Hashtable legionfallDictionary
	{
		get
		{
			return LegionfallData.instance.m_legionfallDictionary;
		}
	}

	static LegionfallData()
	{
	}

	public LegionfallData()
	{
	}

	public static void AddOrUpdateLegionfallBuilding(MobileContribution contribution)
	{
		if (LegionfallData.instance.m_legionfallDictionary.ContainsKey(contribution.ContributionID))
		{
			LegionfallData.instance.m_legionfallDictionary.Remove(contribution.ContributionID);
		}
		LegionfallData.MobileContributionData mobileContributionDatum = new LegionfallData.MobileContributionData()
		{
			contribution = contribution,
			underAttackExpireTime = (long)0
		};
		LegionfallData.instance.m_legionfallDictionary.Add(contribution.ContributionID, mobileContributionDatum);
	}

	public static void ClearData()
	{
		LegionfallData.instance.m_hasAccess = false;
		LegionfallData.instance.m_legionfallWarResources = 0;
		LegionfallData.instance.m_legionfallDictionary.Clear();
	}

	public static long GetCurrentInvasionExpirationTime()
	{
		return LegionfallData.instance.m_invasionExpirationTime;
	}

	public static JamMobileAreaPOI GetCurrentInvasionPOI()
	{
		return LegionfallData.instance.m_currentInvasionPOI;
	}

	public static bool HasAccess()
	{
		return LegionfallData.instance.m_hasAccess;
	}

	public static void SetCurrentInvasionExpirationTime(int secondsUntilExpiration)
	{
		LegionfallData.instance.m_invasionExpirationTime = GarrisonStatus.CurrentTime() + (long)secondsUntilExpiration;
	}

	public static void SetCurrentInvasionPOI(JamMobileAreaPOI poi)
	{
		LegionfallData.instance.m_currentInvasionPOI = poi;
	}

	public static void SetHasAccess(bool hasAccess)
	{
		LegionfallData.instance.m_hasAccess = hasAccess;
	}

	public static void SetLegionfallWarResources(int legionfallWarResources)
	{
		LegionfallData.instance.m_legionfallWarResources = legionfallWarResources;
	}

	public static int WarResources()
	{
		return LegionfallData.instance.m_legionfallWarResources;
	}

	public struct MobileContributionData
	{
		public MobileContribution contribution;

		public long underAttackExpireTime;
	}
}