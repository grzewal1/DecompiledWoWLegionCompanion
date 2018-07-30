using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStaticData;

namespace WoWCompanionApp
{
	public class MissionListPanel : MonoBehaviour
	{
		public Text characterNameText;

		public Text characterLevelAndClassText;

		public Image availableMissionsButtonHighlight;

		public Text availableMissionsButtonText;

		public MissionListView availableMissionListView;

		public GameObject availableMissionListContents;

		public Image inProgressMissionsButtonHighlight;

		public Text inProgressMissionsButtonText;

		public MissionListView inProgressMissionListView;

		public GameObject inProgressMissionListContents;

		public GameObject CompleteMissionCheatPopup;

		private int m_currentCheatMissionID;

		public MissionListPanel()
		{
		}

		public void OnUIRefresh()
		{
			this.availableMissionListView.OnUIRefresh();
			this.inProgressMissionListView.OnUIRefresh();
			MissionListItem[] componentsInChildren = this.availableMissionListContents.GetComponentsInChildren<MissionListItem>(true);
			MissionListItem[] missionListItemArray = this.inProgressMissionListContents.GetComponentsInChildren<MissionListItem>(true);
			this.availableMissionsButtonText.text = string.Concat("Available - ", (int)componentsInChildren.Length);
			this.inProgressMissionsButtonText.text = string.Concat("In Progress - ", (int)missionListItemArray.Length);
		}

		public void SetCharacterLevelAndClass(int level, int charClassID)
		{
		}

		public void SetCharacterName(string name)
		{
			this.characterNameText.text = name;
		}

		public void ShowAvailableMissionList()
		{
			this.availableMissionListView.gameObject.SetActive(true);
			this.inProgressMissionListView.gameObject.SetActive(false);
			this.availableMissionsButtonHighlight.gameObject.SetActive(true);
			this.inProgressMissionsButtonHighlight.gameObject.SetActive(false);
		}

		public void ShowInProgressMissionList()
		{
			this.availableMissionListView.gameObject.SetActive(false);
			this.inProgressMissionListView.gameObject.SetActive(true);
			this.availableMissionsButtonHighlight.gameObject.SetActive(false);
			this.inProgressMissionsButtonHighlight.gameObject.SetActive(true);
		}

		private void Start()
		{
			this.m_currentCheatMissionID = 0;
		}

		private void Update()
		{
			foreach (WrapperGarrisonMission value in PersistentMissionData.missionDictionary.Values)
			{
				bool flag = false;
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(value.MissionRecID);
				if (record != null)
				{
					if (record.GarrFollowerTypeID == (uint)GarrisonStatus.GarrisonFollowerType)
					{
						if (value.MissionState == 1)
						{
							TimeSpan timeSpan = GarrisonStatus.CurrentTime() - value.StartTime;
							if ((value.MissionDuration - timeSpan).TotalSeconds <= 0)
							{
								flag = true;
							}
						}
						if (value.MissionState == 2)
						{
							flag = true;
						}
						!flag;
					}
				}
			}
		}
	}
}