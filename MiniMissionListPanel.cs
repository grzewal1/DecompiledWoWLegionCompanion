using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowStaticData;

public class MiniMissionListPanel : MonoBehaviour
{
	public GameObject m_miniMissionListItemPrefab;

	public GameObject m_availableMissionListScrollView;

	public GameObject m_availableMission_listContents;

	public GameObject m_inProgressMissionListScrollView;

	public GameObject m_inProgressMission_listContents;

	public Button m_availableMissionsTabButton;

	public Text m_availableMissionsTabLabel;

	public Image m_availableMissionsTabSelectedImage;

	public Button m_inProgressMissionsTabButton;

	public Text m_inProgressMissionsTabLabel;

	public Image m_inProgressMissionsTabSelectedImage;

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
		this.m_availableMissionsTabLabel.font = GeneralHelpers.LoadFancyFont();
		this.m_inProgressMissionsTabLabel.font = GeneralHelpers.LoadFancyFont();
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
				JamGarrisonMobileMission item = (JamGarrisonMobileMission)PersistentMissionData.missionDictionary[miniMissionListItem.GetMissionID()];
				if (item.MissionState == 0)
				{
					flag = false;
					miniMissionListItem.UpdateMechanicPreview(false, item);
				}
			}
			if (flag)
			{
				UnityEngine.Object.DestroyImmediate(miniMissionListItem.gameObject);
			}
		}
		MiniMissionListItem[] miniMissionListItemArray = this.m_inProgressMission_listContents.GetComponentsInChildren<MiniMissionListItem>(true);
		for (int j = 0; j < (int)miniMissionListItemArray.Length; j++)
		{
			MiniMissionListItem miniMissionListItem1 = miniMissionListItemArray[j];
			bool flag1 = true;
			if (PersistentMissionData.missionDictionary.ContainsKey(miniMissionListItem1.GetMissionID()) && ((JamGarrisonMobileMission)PersistentMissionData.missionDictionary[miniMissionListItem1.GetMissionID()]).MissionState != 0)
			{
				flag1 = false;
			}
			if (flag1)
			{
				UnityEngine.Object.DestroyImmediate(miniMissionListItem1.gameObject);
			}
		}
		MiniMissionListItem[] componentsInChildren1 = this.m_availableMission_listContents.GetComponentsInChildren<MiniMissionListItem>(true);
		MiniMissionListItem[] miniMissionListItemArray1 = this.m_inProgressMission_listContents.GetComponentsInChildren<MiniMissionListItem>(true);
		IEnumerator enumerator = PersistentMissionData.missionDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamGarrisonMobileMission current = (JamGarrisonMobileMission)enumerator.Current;
				bool flag2 = false;
				MiniMissionListItem[] miniMissionListItemArray2 = componentsInChildren1;
				int num = 0;
				while (num < (int)miniMissionListItemArray2.Length)
				{
					if (miniMissionListItemArray2[num].GetMissionID() != current.MissionRecID)
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
						if (miniMissionListItemArray3[num1].GetMissionID() != current.MissionRecID)
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
					GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(current.MissionRecID);
					if (record == null)
					{
						Debug.LogWarning(string.Concat("Mission Not Found: ID ", current.MissionRecID));
					}
					else if (record.GarrFollowerTypeID == 4)
					{
						if ((record.Flags & 16) == 0)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_miniMissionListItemPrefab);
							if (current.MissionState != 0)
							{
								gameObject.transform.SetParent(this.m_inProgressMission_listContents.transform, false);
								this.ShowMissionStartedAnim();
							}
							else
							{
								gameObject.transform.SetParent(this.m_availableMission_listContents.transform, false);
							}
							gameObject.GetComponent<MiniMissionListItem>().SetMission(current);
						}
						else
						{
							this.m_combatAllyListItem.gameObject.SetActive(true);
						}
					}
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable == null)
			{
			}
			disposable.Dispose();
		}
		componentsInChildren1 = this.m_availableMission_listContents.GetComponentsInChildren<MiniMissionListItem>(true);
		miniMissionListItemArray1 = this.m_inProgressMission_listContents.GetComponentsInChildren<MiniMissionListItem>(true);
		int length = (int)componentsInChildren1.Length;
		int length1 = (int)miniMissionListItemArray1.Length;
		this.m_availableMissionsTabLabel.text = string.Concat(StaticDB.GetString("AVAILABLE", null), " - ", length);
		this.m_inProgressMissionsTabLabel.text = string.Concat(StaticDB.GetString("IN_PROGRESS", null), " - ", length1);
		this.m_noMissionsAvailableLabel.gameObject.SetActive(length == 0);
		this.m_noMissionsInProgressLabel.gameObject.SetActive(length1 == 0);
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
	}

	public void ShowAvailableMissionList()
	{
		this.m_availableMissionListScrollView.SetActive(true);
		this.m_inProgressMissionListScrollView.SetActive(false);
		this.m_availableMissionsTabSelectedImage.gameObject.SetActive(true);
		this.m_inProgressMissionsTabSelectedImage.gameObject.SetActive(false);
	}

	public void ShowInProgressMissionList()
	{
		this.m_availableMissionListScrollView.SetActive(false);
		this.m_inProgressMissionListScrollView.SetActive(true);
		this.m_availableMissionsTabSelectedImage.gameObject.SetActive(false);
		this.m_inProgressMissionsTabSelectedImage.gameObject.SetActive(true);
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

	private void Update()
	{
	}
}