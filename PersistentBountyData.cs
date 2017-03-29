using System;
using System.Collections;
using WowJamMessages.MobileClientJSON;

public class PersistentBountyData
{
	private static PersistentBountyData s_instance;

	private Hashtable m_bountyDictionary;

	private Hashtable m_bountiesByWorldQuestDictionary;

	private bool m_bountiesAreVisible;

	public static Hashtable bountiesByWorldQuestDictionary
	{
		get
		{
			return PersistentBountyData.instance.m_bountiesByWorldQuestDictionary;
		}
	}

	public static Hashtable bountyDictionary
	{
		get
		{
			return PersistentBountyData.instance.m_bountyDictionary;
		}
	}

	private static PersistentBountyData instance
	{
		get
		{
			if (PersistentBountyData.s_instance == null)
			{
				PersistentBountyData.s_instance = new PersistentBountyData()
				{
					m_bountyDictionary = new Hashtable(),
					m_bountiesByWorldQuestDictionary = new Hashtable(),
					m_bountiesAreVisible = false
				};
			}
			return PersistentBountyData.s_instance;
		}
	}

	static PersistentBountyData()
	{
	}

	public PersistentBountyData()
	{
	}

	public static void AddOrUpdateBountiesByWorldQuest(MobileBountiesByWorldQuest bountiesByWorldQuest)
	{
		if (PersistentBountyData.instance.m_bountiesByWorldQuestDictionary.ContainsKey(bountiesByWorldQuest.QuestID))
		{
			PersistentBountyData.instance.m_bountiesByWorldQuestDictionary.Remove(bountiesByWorldQuest.QuestID);
		}
		PersistentBountyData.instance.m_bountiesByWorldQuestDictionary.Add(bountiesByWorldQuest.QuestID, bountiesByWorldQuest);
	}

	public static void AddOrUpdateBounty(MobileWorldQuestBounty bounty)
	{
		if (PersistentBountyData.instance.m_bountyDictionary.ContainsKey(bounty.QuestID))
		{
			PersistentBountyData.instance.m_bountyDictionary.Remove(bounty.QuestID);
		}
		PersistentBountyData.instance.m_bountyDictionary.Add(bounty.QuestID, bounty);
	}

	public static bool BountiesAreVisible()
	{
		return PersistentBountyData.s_instance.m_bountiesAreVisible;
	}

	public static void ClearData()
	{
		PersistentBountyData.instance.m_bountyDictionary.Clear();
		PersistentBountyData.instance.m_bountiesByWorldQuestDictionary.Clear();
	}

	public static void SetBountiesVisible(bool visible)
	{
		PersistentBountyData.s_instance.m_bountiesAreVisible = visible;
	}
}