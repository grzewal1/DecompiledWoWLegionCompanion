using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStaticData;

namespace WoWCompanionApp
{
	public class MiniMissionListPanel : MonoBehaviour
	{
		public MiniMissionListItem m_miniMissionListItemPrefab;

		public GameObject m_availableMissionListScrollView;

		public GameObject m_availableMission_listContents;

		public GameObject m_inProgressMissionListScrollView;

		public GameObject m_inProgressMission_listContents;

		public Button m_availableMissionsTabButton;

		public Text m_availableMissionsTabLabel;

		public Image m_availableMissionNotSelectedImage;

		public Button m_inProgressMissionsTabButton;

		public Text m_inProgressMissionsTabLabel;

		public Image m_inProgressMissionNotSelectedImage;

		public Text m_noMissionsAvailableLabel;

		public Text m_noMissionsInProgressLabel;

		public CombatAllyListItem m_combatAllyListItem;

		private Vector2 m_multiPanelViewSizeDelta;

		public GameObject m_missionStartedEffectObjPrefab;

		public OrderHallNavButton m_missionListOrderHallNavButton;

		private GameObject m_currentMissionStartedEffectObj;

		public MiniMissionListPanel()
		{
		}

		private void Awake()
		{
			this.m_noMissionsAvailableLabel.font = GeneralHelpers.LoadStandardFont();
			this.m_noMissionsAvailableLabel.text = StaticDB.GetString("NO_MISSIONS_AVAILABLE", "No missions are currently available.");
			this.m_noMissionsInProgressLabel.font = GeneralHelpers.LoadStandardFont();
			this.m_noMissionsInProgressLabel.text = StaticDB.GetString("NO_MISSIONS_IN_PROGRESS", "No missions are currently in progress.");
		}

		private void HandleGarrisonDataResetFinished()
		{
			this.InitMissionList();
		}

		private void HandleMissionAdded(int garrMissionID, int result)
		{
			this.InitMissionList();
		}

		public void InitMissionList()
		{
			this.m_combatAllyListItem.gameObject.SetActive(false);
			MiniMissionListItem[] componentsInChildren = this.m_availableMission_listContents.GetComponentsInChildren<MiniMissionListItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				MiniMissionListItem miniMissionListItem = componentsInChildren[i];
				bool flag = true;
				if (PersistentMissionData.missionDictionary.ContainsKey(miniMissionListItem.GetMissionID()))
				{
					WrapperGarrisonMission item = PersistentMissionData.missionDictionary[miniMissionListItem.GetMissionID()];
					if (item.MissionState == 0)
					{
						flag = false;
						miniMissionListItem.UpdateMechanicPreview(false, item);
					}
				}
				if (flag)
				{
					miniMissionListItem.gameObject.transform.SetParent(null);
					UnityEngine.Object.Destroy(miniMissionListItem.gameObject);
				}
			}
			MiniMissionListItem[] miniMissionListItemArray = this.m_inProgressMission_listContents.GetComponentsInChildren<MiniMissionListItem>(true);
			for (int j = 0; j < (int)miniMissionListItemArray.Length; j++)
			{
				MiniMissionListItem miniMissionListItem1 = miniMissionListItemArray[j];
				bool flag1 = true;
				if (PersistentMissionData.missionDictionary.ContainsKey(miniMissionListItem1.GetMissionID()) && PersistentMissionData.missionDictionary[miniMissionListItem1.GetMissionID()].MissionState != 0)
				{
					flag1 = false;
				}
				if (flag1)
				{
					miniMissionListItem1.gameObject.transform.SetParent(null);
					UnityEngine.Object.Destroy(miniMissionListItem1.gameObject);
				}
			}
			MiniMissionListItem[] componentsInChildren1 = this.m_availableMission_listContents.GetComponentsInChildren<MiniMissionListItem>(true);
			MiniMissionListItem[] miniMissionListItemArray1 = this.m_inProgressMission_listContents.GetComponentsInChildren<MiniMissionListItem>(true);
			foreach (WrapperGarrisonMission value in PersistentMissionData.missionDictionary.Values)
			{
				bool flag2 = false;
				MiniMissionListItem[] miniMissionListItemArray2 = componentsInChildren1;
				int num = 0;
				while (num < (int)miniMissionListItemArray2.Length)
				{
					if (miniMissionListItemArray2[num].GetMissionID() != value.MissionRecID)
					{
						num++;
					}
					else
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					MiniMissionListItem[] miniMissionListItemArray3 = miniMissionListItemArray1;
					int num1 = 0;
					while (num1 < (int)miniMissionListItemArray3.Length)
					{
						if (miniMissionListItemArray3[num1].GetMissionID() != value.MissionRecID)
						{
							num1++;
						}
						else
						{
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(value.MissionRecID);
					if (record == null)
					{
						Debug.LogWarning(string.Concat("Mission Not Found: ID ", value.MissionRecID));
					}
					else if (record.GarrFollowerTypeID == (uint)GarrisonStatus.GarrisonFollowerType)
					{
						if ((record.Flags & 16) == 0)
						{
							MiniMissionListItem miniMissionListItem2 = UnityEngine.Object.Instantiate<MiniMissionListItem>(this.m_miniMissionListItemPrefab);
							if (value.MissionState != 0)
							{
								miniMissionListItem2.transform.SetParent(this.m_inProgressMission_listContents.transform, false);
								this.ShowMissionStartedAnim();
							}
							else
							{
								miniMissionListItem2.transform.SetParent(this.m_availableMission_listContents.transform, false);
							}
							miniMissionListItem2.SetMission(value);
						}
						else
						{
							this.m_combatAllyListItem.gameObject.SetActive(true);
							this.m_combatAllyListItem.UpdateVisuals();
						}
					}
				}
			}
			int num2 = 0;
			int num3 = 0;
			PersistentMissionData.GetAvailableAndProgressCounts(ref num3, ref num2);
			this.m_availableMissionsTabLabel.text = string.Concat(StaticDB.GetString("AVAILABLE", null), " - ", num3);
			this.m_inProgressMissionsTabLabel.text = string.Concat(StaticDB.GetString("IN_PROGRESS", null), " - ", num2);
			this.m_noMissionsAvailableLabel.gameObject.SetActive(num3 == 0);
			this.m_noMissionsInProgressLabel.gameObject.SetActive(num2 == 0);
		}

		private void OnDisable()
		{
			Main.instance.GarrisonDataResetFinishedAction -= new Action(this.HandleGarrisonDataResetFinished);
			Main.instance.MissionAddedAction -= new Action<int, int>(this.HandleMissionAdded);
		}

		public void OnEnable()
		{
			Main.instance.GarrisonDataResetFinishedAction += new Action(this.HandleGarrisonDataResetFinished);
			Main.instance.MissionAddedAction += new Action<int, int>(this.HandleMissionAdded);
			this.InitMissionList();
			this.ShowAvailableMissionList();
			Singleton<DialogFactory>.Instance.CloseMissionDialog();
		}

		public void ShowAvailableMissionList()
		{
			this.m_availableMissionListScrollView.SetActive(true);
			this.m_inProgressMissionListScrollView.SetActive(false);
			this.m_availableMissionNotSelectedImage.gameObject.SetActive(false);
			this.m_inProgressMissionNotSelectedImage.gameObject.SetActive(true);
		}

		public void ShowInProgressMissionList()
		{
			this.m_availableMissionListScrollView.SetActive(false);
			this.m_inProgressMissionListScrollView.SetActive(true);
			this.m_availableMissionNotSelectedImage.gameObject.SetActive(true);
			this.m_inProgressMissionNotSelectedImage.gameObject.SetActive(false);
		}

		private void ShowMissionStartedAnim()
		{
			if (!this.m_missionListOrderHallNavButton.IsSelected())
			{
				return;
			}
			this.m_currentMissionStartedEffectObj = UnityEngine.Object.Instantiate<GameObject>(this.m_missionStartedEffectObjPrefab);
			this.m_currentMissionStartedEffectObj.transform.SetParent(this.m_inProgressMissionsTabButton.transform, false);
			this.m_currentMissionStartedEffectObj.transform.localPosition = Vector3.zero;
		}
	}
}