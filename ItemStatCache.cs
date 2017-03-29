using System;
using System.Collections;
using UnityEngine;
using WowJamMessages.MobileClientJSON;
using WowJamMessages.MobilePlayerJSON;

public class ItemStatCache : MonoBehaviour
{
	private static ItemStatCache s_instance;

	private Hashtable m_records;

	public Action<int, int, MobileItemStats> ItemStatCacheUpdateAction;

	public static ItemStatCache instance
	{
		get
		{
			return ItemStatCache.s_instance;
		}
	}

	static ItemStatCache()
	{
	}

	public ItemStatCache()
	{
	}

	public void AddMobileItemStats(int itemID, int itemContext, MobileItemStats stats)
	{
		if (this.m_records[itemID] != null)
		{
			this.m_records[itemID] = stats;
		}
		else
		{
			this.m_records.Add(itemID, stats);
		}
		if (this.ItemStatCacheUpdateAction != null)
		{
			this.ItemStatCacheUpdateAction(itemID, itemContext, stats);
		}
	}

	private void Awake()
	{
		this.m_records = new Hashtable();
		if (ItemStatCache.s_instance == null)
		{
			ItemStatCache.s_instance = this;
		}
	}

	public void ClearItemStats()
	{
		this.m_records.Clear();
	}

	public MobileItemStats GetItemStats(int itemID, int itemContext)
	{
		MobileItemStats item = (MobileItemStats)this.m_records[itemID];
		if (item != null)
		{
			return item;
		}
		MobilePlayerGetItemTooltipInfo mobilePlayerGetItemTooltipInfo = new MobilePlayerGetItemTooltipInfo()
		{
			ItemID = itemID,
			ItemContext = itemContext
		};
		Login.instance.SendToMobileServer(mobilePlayerGetItemTooltipInfo);
		return null;
	}
}