using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowStatConstants;
using WowStaticData;

public class OrderHallFollowersPanel : MonoBehaviour
{
	public FollowerListItem m_followerDetailListItemPrefab;

	public GameObject m_followerDetailListContent;

	public FollowerListHeader m_followerListHeaderPrefab;

	public ScrollRect m_listScrollRect;

	public static OrderHallFollowersPanel instance;

	public Action<int> FollowerDetailListItemSelectedAction;

	private Vector2 m_multiPanelViewSizeDelta;

	private List<KeyValuePair<int, JamGarrisonFollower>> m_sortedFollowerList;

	private float m_scrollListToOffset;

	private FollowerListHeader m_championsHeader;

	private FollowerListHeader m_troopsHeader;

	private FollowerListHeader m_inactiveHeader;

	public OrderHallFollowersPanel()
	{
	}

	private void Awake()
	{
		OrderHallFollowersPanel.instance = this;
	}

	public void FollowerDetailListItemSelected(int garrFollowerID)
	{
		if (this.FollowerDetailListItemSelectedAction != null)
		{
			this.FollowerDetailListItemSelectedAction(garrFollowerID);
		}
	}

	private void InitFollowerList()
	{
		FollowerListItem[] componentsInChildren = this.m_followerDetailListContent.GetComponentsInChildren<FollowerListItem>(true);
		FollowerListItem[] followerListItemArray = componentsInChildren;
		for (int i = 0; i < (int)followerListItemArray.Length; i++)
		{
			FollowerListItem followerListItem = followerListItemArray[i];
			if (PersistentFollowerData.followerDictionary.ContainsKey(followerListItem.m_followerID))
			{
				JamGarrisonFollower item = PersistentFollowerData.followerDictionary[followerListItem.m_followerID];
				if ((item.Flags & 8) == 0 || item.Durability > 0)
				{
					followerListItem.SetFollower(item);
				}
				else
				{
					followerListItem.gameObject.SetActive(false);
					followerListItem.transform.SetParent(Main.instance.transform);
				}
			}
			else
			{
				followerListItem.gameObject.SetActive(false);
				followerListItem.transform.SetParent(Main.instance.transform);
			}
		}
		this.SortFollowerListData();
		if (this.m_championsHeader == null)
		{
			this.m_championsHeader = UnityEngine.Object.Instantiate<FollowerListHeader>(this.m_followerListHeaderPrefab);
		}
		this.m_championsHeader.transform.SetParent(this.m_followerDetailListContent.transform, false);
		this.m_championsHeader.m_title.font = GeneralHelpers.LoadStandardFont();
		this.m_championsHeader.m_title.text = string.Concat(StaticDB.GetString("CHAMPIONS", null), ": ");
		int numActiveChampions = GeneralHelpers.GetNumActiveChampions();
		int maxActiveFollowers = GarrisonStatus.GetMaxActiveFollowers();
		if (numActiveChampions > maxActiveFollowers)
		{
			this.m_championsHeader.m_count.text = string.Concat(new object[] { "<color=#ff0000ff>", numActiveChampions, "/", maxActiveFollowers, "</color>" });
		}
		else
		{
			this.m_championsHeader.m_count.text = string.Concat(new object[] { string.Empty, numActiveChampions, "/", maxActiveFollowers });
		}
		foreach (KeyValuePair<int, JamGarrisonFollower> mSortedFollowerList in this.m_sortedFollowerList)
		{
			bool flag = false;
			FollowerListItem[] followerListItemArray1 = componentsInChildren;
			int num = 0;
			while (num < (int)followerListItemArray1.Length)
			{
				if (followerListItemArray1[num].m_followerID != mSortedFollowerList.Value.GarrFollowerID)
				{
					num++;
				}
				else
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.InsertFollowerIntoListView(mSortedFollowerList.Value, FollowerCategory.ActiveChampion);
			}
		}
		int numTroops = GeneralHelpers.GetNumTroops();
		if (this.m_troopsHeader == null)
		{
			this.m_troopsHeader = UnityEngine.Object.Instantiate<FollowerListHeader>(this.m_followerListHeaderPrefab);
		}
		this.m_troopsHeader.transform.SetParent(this.m_followerDetailListContent.transform, false);
		this.m_troopsHeader.m_title.font = GeneralHelpers.LoadStandardFont();
		this.m_troopsHeader.m_title.text = string.Concat(StaticDB.GetString("TROOPS", null), ": ");
		this.m_troopsHeader.m_count.font = GeneralHelpers.LoadStandardFont();
		this.m_troopsHeader.m_count.text = string.Concat(string.Empty, numTroops);
		foreach (KeyValuePair<int, JamGarrisonFollower> keyValuePair in this.m_sortedFollowerList)
		{
			bool flag1 = false;
			FollowerListItem[] followerListItemArray2 = componentsInChildren;
			int num1 = 0;
			while (num1 < (int)followerListItemArray2.Length)
			{
				if (followerListItemArray2[num1].m_followerID != keyValuePair.Value.GarrFollowerID)
				{
					num1++;
				}
				else
				{
					flag1 = true;
					break;
				}
			}
			if (!flag1)
			{
				this.InsertFollowerIntoListView(keyValuePair.Value, FollowerCategory.Troop);
			}
		}
		int numInactiveChampions = GeneralHelpers.GetNumInactiveChampions();
		if (this.m_inactiveHeader == null)
		{
			this.m_inactiveHeader = UnityEngine.Object.Instantiate<FollowerListHeader>(this.m_followerListHeaderPrefab);
		}
		this.m_inactiveHeader.transform.SetParent(this.m_followerDetailListContent.transform, false);
		this.m_inactiveHeader.m_title.font = GeneralHelpers.LoadStandardFont();
		this.m_inactiveHeader.m_title.text = string.Concat(StaticDB.GetString("INACTIVE", null), ": ");
		this.m_inactiveHeader.m_count.font = GeneralHelpers.LoadStandardFont();
		this.m_inactiveHeader.m_count.text = string.Concat(string.Empty, numInactiveChampions);
		foreach (KeyValuePair<int, JamGarrisonFollower> mSortedFollowerList1 in this.m_sortedFollowerList)
		{
			bool flag2 = false;
			FollowerListItem[] followerListItemArray3 = componentsInChildren;
			int num2 = 0;
			while (num2 < (int)followerListItemArray3.Length)
			{
				if (followerListItemArray3[num2].m_followerID != mSortedFollowerList1.Value.GarrFollowerID)
				{
					num2++;
				}
				else
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				this.InsertFollowerIntoListView(mSortedFollowerList1.Value, FollowerCategory.InactiveChampion);
			}
		}
		this.SyncVisibleListOrderToSortedFollowerList();
		this.m_championsHeader.gameObject.SetActive(numActiveChampions > 0);
		this.m_troopsHeader.gameObject.SetActive(numTroops > 0);
		this.m_inactiveHeader.gameObject.SetActive(numInactiveChampions > 0);
		this.m_championsHeader.transform.SetSiblingIndex(0);
		this.m_troopsHeader.transform.SetSiblingIndex(numActiveChampions + 1);
		this.m_inactiveHeader.transform.SetSiblingIndex(numActiveChampions + numTroops + 2);
	}

	private void InsertFollowerIntoListView(JamGarrisonFollower follower, FollowerCategory followerCategory)
	{
		GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(follower.GarrFollowerID);
		if (record == null)
		{
			return;
		}
		if (record.GarrFollowerTypeID != 4)
		{
			return;
		}
		bool flags = (follower.Flags & 8) != 0;
		bool flag = !flags;
		FollowerStatus followerStatus = GeneralHelpers.GetFollowerStatus(follower);
		switch (followerCategory)
		{
			case FollowerCategory.ActiveChampion:
			{
				if (!flag || followerStatus == FollowerStatus.inactive)
				{
					return;
				}
				break;
			}
			case FollowerCategory.InactiveChampion:
			{
				if (!flag || followerStatus != FollowerStatus.inactive)
				{
					return;
				}
				break;
			}
			case FollowerCategory.Troop:
			{
				if (!flags || follower.Durability <= 0)
				{
					return;
				}
				break;
			}
			default:
			{
				return;
			}
		}
		FollowerListItem followerListItem = UnityEngine.Object.Instantiate<FollowerListItem>(this.m_followerDetailListItemPrefab);
		followerListItem.transform.SetParent(this.m_followerDetailListContent.transform, false);
		followerListItem.SetFollower(follower);
	}

	public void ScrollListTo(float offsetY)
	{
		this.m_scrollListToOffset = offsetY;
		this.m_listScrollRect.enabled = false;
		iTween.StopByName(base.gameObject, "ScrollListTo");
		GameObject gameObject = base.gameObject;
		object[] objArray = new object[14];
		objArray[0] = "name";
		objArray[1] = "ScrollListTo";
		objArray[2] = "from";
		Vector3 mFollowerDetailListContent = this.m_followerDetailListContent.transform.localPosition;
		objArray[3] = mFollowerDetailListContent.y;
		objArray[4] = "to";
		objArray[5] = offsetY;
		objArray[6] = "time";
		objArray[7] = 0.25f;
		objArray[8] = "easetype";
		objArray[9] = iTween.EaseType.easeOutCubic;
		objArray[10] = "onupdate";
		objArray[11] = "ScrollListTo_Update";
		objArray[12] = "oncomplete";
		objArray[13] = "ScrollListTo_Complete";
		iTween.ValueTo(gameObject, iTween.Hash(objArray));
	}

	private void ScrollListTo_Complete()
	{
		Vector3 mFollowerDetailListContent = this.m_followerDetailListContent.transform.localPosition;
		mFollowerDetailListContent.y = this.m_scrollListToOffset;
		this.m_followerDetailListContent.transform.localPosition = mFollowerDetailListContent;
		this.m_listScrollRect.enabled = true;
	}

	private void ScrollListTo_Update(float offsetY)
	{
		Vector3 mFollowerDetailListContent = this.m_followerDetailListContent.transform.localPosition;
		mFollowerDetailListContent.y = offsetY;
		this.m_followerDetailListContent.transform.localPosition = mFollowerDetailListContent;
	}

	private void SortFollowerListData()
	{
		this.m_sortedFollowerList = PersistentFollowerData.followerDictionary.ToList<KeyValuePair<int, JamGarrisonFollower>>();
		OrderHallFollowersPanel.FollowerComparer followerComparer = new OrderHallFollowersPanel.FollowerComparer();
		this.m_sortedFollowerList.Sort(followerComparer);
	}

	private void Start()
	{
		this.InitFollowerList();
		Main.instance.GarrisonDataResetFinishedAction += new Action(this.InitFollowerList);
		Main.instance.FollowerDataChangedAction += new Action(this.InitFollowerList);
	}

	private void SyncVisibleListOrderToSortedFollowerList()
	{
		FollowerListItem[] componentsInChildren = this.m_followerDetailListContent.GetComponentsInChildren<FollowerListItem>(true);
		for (int i = 0; i < this.m_sortedFollowerList.Count; i++)
		{
			FollowerListItem[] followerListItemArray = componentsInChildren;
			int num = 0;
			while (num < (int)followerListItemArray.Length)
			{
				FollowerListItem followerListItem = followerListItemArray[num];
				if (followerListItem.m_followerID != this.m_sortedFollowerList[i].Value.GarrFollowerID)
				{
					num++;
				}
				else
				{
					followerListItem.transform.SetSiblingIndex(i);
					break;
				}
			}
		}
	}

	private void Update()
	{
	}

	private class FollowerComparer : IComparer<KeyValuePair<int, JamGarrisonFollower>>
	{
		public FollowerComparer()
		{
		}

		public int Compare(KeyValuePair<int, JamGarrisonFollower> follower1, KeyValuePair<int, JamGarrisonFollower> follower2)
		{
			JamGarrisonFollower value = follower1.Value;
			JamGarrisonFollower jamGarrisonFollower = follower2.Value;
			FollowerStatus followerStatus = GeneralHelpers.GetFollowerStatus(value);
			FollowerStatus followerStatu = GeneralHelpers.GetFollowerStatus(jamGarrisonFollower);
			bool flags = (value.Flags & 8) != 0;
			bool flag = (jamGarrisonFollower.Flags & 8) != 0;
			bool flag1 = (flags ? false : followerStatus != FollowerStatus.inactive);
			if (flag1 != (flag ? false : followerStatu != FollowerStatus.inactive))
			{
				return (!flag1 ? 1 : -1);
			}
			if (followerStatus != followerStatu)
			{
				return (int)followerStatus - (int)followerStatu;
			}
			int itemLevelArmor = (value.ItemLevelArmor + value.ItemLevelWeapon) / 2;
			int num = (jamGarrisonFollower.ItemLevelArmor + jamGarrisonFollower.ItemLevelWeapon) / 2;
			if (num != itemLevelArmor)
			{
				return num - itemLevelArmor;
			}
			if (value.Quality == jamGarrisonFollower.Quality)
			{
				return 0;
			}
			return jamGarrisonFollower.Quality - value.Quality;
		}
	}
}