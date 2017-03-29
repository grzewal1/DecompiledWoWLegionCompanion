using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using WowJamMessages;
using WowStatConstants;
using WowStaticData;

public class FollowerListView : MonoBehaviour
{
	private List<KeyValuePair<int, JamGarrisonFollower>> m_sortedFollowerList;

	public GameObject m_followerListViewContents;

	public GameObject m_followerListItemPrefab;

	public MissionDetailView m_missionDetailView;

	public bool m_isCombatAllyList;

	public bool m_usedForMissionList;

	public FollowerListView()
	{
	}

	public void HandleMissionChanged(int garrMissionID)
	{
		if (garrMissionID == 0)
		{
			return;
		}
		this.SortFollowerListData();
		this.SyncVisibleListOrderToSortedFollowerList();
		this.UpdateUsefulAbilitiesDisplay();
		this.RemoveAllFromParty();
		this.UpdateAllAvailabilityStatus();
	}

	public void InitFollowerList()
	{
		FollowerListItem[] componentsInChildren = this.m_followerListViewContents.GetComponentsInChildren<FollowerListItem>(true);
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
		Transform mFollowerListViewContents = this.m_followerListViewContents.transform;
		float single = this.m_followerListViewContents.transform.localPosition.x;
		Vector3 vector3 = this.m_followerListViewContents.transform.localPosition;
		mFollowerListViewContents.localPosition = new Vector3(single, 0f, vector3.z);
		this.SortFollowerListData();
		componentsInChildren = this.m_followerListViewContents.GetComponentsInChildren<FollowerListItem>(true);
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
				if ((mSortedFollowerList.Value.Flags & 8) == 0 || mSortedFollowerList.Value.Durability > 0)
				{
					this.InsertFollowerIntoListView(mSortedFollowerList.Value);
				}
			}
		}
	}

	private FollowerListItem InsertFollowerIntoListView(JamGarrisonFollower follower)
	{
		GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(follower.GarrFollowerID);
		if (record == null)
		{
			return null;
		}
		if (record.GarrFollowerTypeID != 4)
		{
			return null;
		}
		if (this.m_isCombatAllyList)
		{
			bool flags = (follower.Flags & 8) != 0;
			FollowerStatus followerStatus = GeneralHelpers.GetFollowerStatus(follower);
			if (flags || follower.ZoneSupportSpellID <= 0 || followerStatus == FollowerStatus.inactive || followerStatus == FollowerStatus.fatigued || followerStatus == FollowerStatus.inBuilding)
			{
				return null;
			}
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_followerListItemPrefab);
		gameObject.transform.SetParent(this.m_followerListViewContents.transform, false);
		FollowerListItem component = gameObject.GetComponent<FollowerListItem>();
		component.SetFollower(follower);
		return component;
	}

	private void OnDisable()
	{
		if (this.m_usedForMissionList)
		{
			if (AdventureMapPanel.instance != null)
			{
				AdventureMapPanel.instance.MissionSelectedFromListAction -= new Action<int>(this.HandleMissionChanged);
			}
		}
		else if (AdventureMapPanel.instance != null)
		{
			AdventureMapPanel.instance.MissionMapSelectionChangedAction -= new Action<int>(this.HandleMissionChanged);
		}
	}

	private void OnEnable()
	{
		if (this.m_usedForMissionList)
		{
			if (AdventureMapPanel.instance != null)
			{
				AdventureMapPanel.instance.MissionSelectedFromListAction += new Action<int>(this.HandleMissionChanged);
			}
		}
		else if (AdventureMapPanel.instance != null)
		{
			AdventureMapPanel.instance.MissionMapSelectionChangedAction += new Action<int>(this.HandleMissionChanged);
		}
	}

	public void RemoveAllFromParty()
	{
		FollowerListItem[] componentsInChildren = this.m_followerListViewContents.GetComponentsInChildren<FollowerListItem>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RemoveFromParty();
		}
	}

	private void SortFollowerListData()
	{
		this.m_sortedFollowerList = PersistentFollowerData.followerDictionary.ToList<KeyValuePair<int, JamGarrisonFollower>>();
		FollowerListView.FollowerComparer followerComparer = new FollowerListView.FollowerComparer()
		{
			m_missionDetailViewForComparer = this.m_missionDetailView
		};
		this.m_sortedFollowerList.Sort(followerComparer);
	}

	private void Start()
	{
		this.InitFollowerList();
		Main.instance.GarrisonDataResetFinishedAction += new Action(this.InitFollowerList);
	}

	private void SyncVisibleListOrderToSortedFollowerList()
	{
		FollowerListItem[] componentsInChildren = this.m_followerListViewContents.GetComponentsInChildren<FollowerListItem>(true);
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

	private void UpdateAllAvailabilityStatus()
	{
		FollowerListItem[] componentsInChildren = this.m_followerListViewContents.GetComponentsInChildren<FollowerListItem>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetAvailabilityStatus(PersistentFollowerData.followerDictionary[componentsInChildren[i].m_followerID]);
		}
	}

	public void UpdateUsefulAbilitiesDisplay()
	{
		FollowerListItem[] componentsInChildren = this.m_followerListViewContents.GetComponentsInChildren<FollowerListItem>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			componentsInChildren[i].UpdateUsefulAbilitiesDisplay((!this.m_usedForMissionList ? AdventureMapPanel.instance.GetCurrentMapMission() : AdventureMapPanel.instance.GetCurrentListMission()));
		}
	}

	private class FollowerComparer : IComparer<KeyValuePair<int, JamGarrisonFollower>>
	{
		public MissionDetailView m_missionDetailViewForComparer;

		public FollowerComparer()
		{
		}

		public int Compare(KeyValuePair<int, JamGarrisonFollower> follower1, KeyValuePair<int, JamGarrisonFollower> follower2)
		{
			JamGarrisonFollower value = follower1.Value;
			JamGarrisonFollower jamGarrisonFollower = follower2.Value;
			FollowerStatus followerStatus = GeneralHelpers.GetFollowerStatus(value);
			FollowerStatus followerStatu = GeneralHelpers.GetFollowerStatus(jamGarrisonFollower);
			if (followerStatus != followerStatu)
			{
				return (int)followerStatus - (int)followerStatu;
			}
			bool flag = this.HasUsefulAbility(value);
			if (flag != this.HasUsefulAbility(jamGarrisonFollower))
			{
				return (!flag ? 1 : -1);
			}
			int itemLevelArmor = (value.ItemLevelArmor + value.ItemLevelWeapon) / 2;
			int num = (jamGarrisonFollower.ItemLevelArmor + jamGarrisonFollower.ItemLevelWeapon) / 2;
			if (num != itemLevelArmor)
			{
				return num - itemLevelArmor;
			}
			if (value.Quality != jamGarrisonFollower.Quality)
			{
				return jamGarrisonFollower.Quality - value.Quality;
			}
			bool flags = (value.Flags & 8) != 0;
			bool flags1 = (jamGarrisonFollower.Flags & 8) != 0;
			if (flags == flags1)
			{
				return 0;
			}
			return (!flags1 ? 1 : -1);
		}

		private bool HasUsefulAbility(JamGarrisonFollower follower)
		{
			bool flag1 = false;
			if (this.m_missionDetailViewForComparer == null)
			{
				return false;
			}
			MissionMechanic[] componentsInChildren = this.m_missionDetailViewForComparer.gameObject.GetComponentsInChildren<MissionMechanic>(true);
			if (componentsInChildren == null)
			{
				return false;
			}
			for (int i = 0; i < (int)follower.AbilityID.Length; i++)
			{
				GarrAbilityRec garrAbilityRec = StaticDB.garrAbilityDB.GetRecord(follower.AbilityID[i]);
				if (garrAbilityRec != null)
				{
					if ((garrAbilityRec.Flags & 1) == 0)
					{
						StaticDB.garrAbilityEffectDB.EnumRecordsByParentID(garrAbilityRec.ID, (GarrAbilityEffectRec garrAbilityEffectRec) => {
							if (garrAbilityEffectRec.GarrMechanicTypeID == 0)
							{
								return true;
							}
							if (garrAbilityEffectRec.AbilityAction != 0)
							{
								return true;
							}
							GarrMechanicTypeRec record = StaticDB.garrMechanicTypeDB.GetRecord((int)garrAbilityEffectRec.GarrMechanicTypeID);
							if (record == null)
							{
								return true;
							}
							bool flag = false;
							int num = 0;
							while (num < (int)componentsInChildren.Length)
							{
								if (componentsInChildren[num].m_missionMechanicTypeID != record.ID)
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
								return true;
							}
							flag1 = true;
							return false;
						});
						if (flag1)
						{
							break;
						}
					}
				}
			}
			return flag1;
		}
	}
}