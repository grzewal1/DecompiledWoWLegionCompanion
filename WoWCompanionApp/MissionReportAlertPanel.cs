using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;

namespace WoWCompanionApp
{
	public class MissionReportAlertPanel : MonoBehaviour
	{
		public GameObject missionReportView;

		public GameObject allianceCommander;

		public GameObject hordeCommander;

		public Text completedMissionsText;

		public GameObject missionResultsView;

		public GameObject missionRewardsIconArea;

		public GameObject missionListItemPrefab;

		public GameObject missionRewardResultsDisplayPrefab;

		public GameObject completedMissionListContents;

		public GameObject okButton;

		public Canvas mainCanvas;

		private Dictionary<int, bool> _requestedMissionCollection;

		public MissionReportAlertPanel()
		{
		}

		private void Awake()
		{
		}

		private void CollectFirstCompletedMission()
		{
			MissionListItem[] componentsInChildren = this.completedMissionListContents.GetComponentsInChildren<MissionListItem>(true);
			int num = 0;
			while (num < (int)componentsInChildren.Length)
			{
				WrapperGarrisonMission item = PersistentMissionData.missionDictionary[componentsInChildren[num].garrMissionID];
				if (this.GetRequestedMissionCollectionDictionary().ContainsKey(componentsInChildren[num].garrMissionID) || !PersistentMissionData.missionDictionary.ContainsKey(componentsInChildren[num].garrMissionID) || item.MissionState != 2)
				{
					num++;
				}
				else
				{
					this.GetRequestedMissionCollectionDictionary().Add(componentsInChildren[num].garrMissionID, true);
					Main.instance.ClaimMissionBonus(componentsInChildren[num].garrMissionID);
					break;
				}
			}
		}

		public void CompleteAllMissions()
		{
			Main.instance.CompleteAllMissions();
			this.missionReportView.SetActive(false);
			this.missionResultsView.SetActive(true);
			this.PopulateCompletedMissionList();
			this.CollectFirstCompletedMission();
		}

		private Dictionary<int, bool> GetRequestedMissionCollectionDictionary()
		{
			if (this._requestedMissionCollection == null)
			{
				this._requestedMissionCollection = new Dictionary<int, bool>();
			}
			return this._requestedMissionCollection;
		}

		private bool MissionIsOnCompletedMissionList(int garrMissionID)
		{
			MissionListItem[] componentsInChildren = this.completedMissionListContents.GetComponentsInChildren<MissionListItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].garrMissionID == garrMissionID)
				{
					return true;
				}
			}
			return false;
		}

		private void OnDisable()
		{
			this.mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		}

		private void OnEnable()
		{
			this.GetRequestedMissionCollectionDictionary().Clear();
			this.okButton.SetActive(false);
			this.mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
			if (GarrisonStatus.Faction() != PVP_FACTION.HORDE)
			{
				this.hordeCommander.SetActive(false);
				this.allianceCommander.SetActive(true);
			}
			else
			{
				this.hordeCommander.SetActive(true);
				this.allianceCommander.SetActive(false);
			}
			this.completedMissionsText.text = string.Concat(string.Empty, PersistentMissionData.GetNumCompletedMissions(false), " Completed Missions");
			MissionListItem[] componentsInChildren = this.completedMissionListContents.GetComponentsInChildren<MissionListItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
			MissionRewardDisplay[] missionRewardDisplayArray = this.missionRewardsIconArea.GetComponentsInChildren<MissionRewardDisplay>(true);
			for (int j = 0; j < (int)missionRewardDisplayArray.Length; j++)
			{
				UnityEngine.Object.Destroy(missionRewardDisplayArray[j].gameObject);
			}
			this.missionReportView.SetActive(true);
			this.missionResultsView.SetActive(false);
		}

		public void OnMissionStatusChanged()
		{
			this.PopulateCompletedMissionList();
			this.CollectFirstCompletedMission();
			int num = 0;
			MissionListItem[] componentsInChildren = this.completedMissionListContents.GetComponentsInChildren<MissionListItem>(true);
			MissionRewardDisplay[] missionRewardDisplayArray = this.missionRewardsIconArea.transform.GetComponentsInChildren<MissionRewardDisplay>(true);
			for (int i = 0; i < (int)missionRewardDisplayArray.Length; i++)
			{
				UnityEngine.Object.Destroy(missionRewardDisplayArray[i].gameObject);
			}
			for (int j = 0; j < (int)componentsInChildren.Length; j++)
			{
				WrapperGarrisonMission item = PersistentMissionData.missionDictionary[componentsInChildren[j].garrMissionID];
				if (!PersistentMissionData.missionDictionary.ContainsKey(componentsInChildren[j].garrMissionID) || item.MissionState != 6)
				{
					num++;
				}
				else
				{
					componentsInChildren[j].inProgressDarkener.SetActive(true);
					componentsInChildren[j].missionResultsText.gameObject.SetActive(true);
					if (!this.GetRequestedMissionCollectionDictionary().ContainsKey(componentsInChildren[j].garrMissionID))
					{
						componentsInChildren[j].missionResultsText.text = "<color=#ff0000ff>FAILED</color>";
					}
					else
					{
						componentsInChildren[j].missionResultsText.text = "<color=#00ff00ff>SUCCEEDED!</color>";
						MissionRewardDisplay[] componentsInChildren1 = componentsInChildren[j].missionRewardGroup.GetComponentsInChildren<MissionRewardDisplay>(true);
						for (int k = 0; k < (int)componentsInChildren1.Length; k++)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.missionRewardResultsDisplayPrefab);
							gameObject.transform.SetParent(this.missionRewardsIconArea.transform, false);
						}
					}
				}
				if (num == 0)
				{
					this.okButton.SetActive(true);
				}
			}
		}

		private void PopulateCompletedMissionList()
		{
			foreach (WrapperGarrisonMission value in PersistentMissionData.missionDictionary.Values)
			{
				if (value.MissionState != 2 && value.MissionState != 6 || this.MissionIsOnCompletedMissionList(value.MissionRecID))
				{
					continue;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.missionListItemPrefab);
				gameObject.transform.SetParent(this.completedMissionListContents.transform, false);
				MissionListItem component = gameObject.GetComponent<MissionListItem>();
				component.Init(value.MissionRecID);
				component.isResultsItem = true;
			}
		}

		public void ShowMissionListAndRefreshData()
		{
			Debug.Log("Request Data Refresh");
			Main.instance.MobileRequestData();
			Main.instance.allPanels.ShowAdventureMap();
		}

		private void Update()
		{
			this.completedMissionsText.text = string.Concat(string.Empty, PersistentMissionData.GetNumCompletedMissions(false), " Completed Missions");
		}
	}
}