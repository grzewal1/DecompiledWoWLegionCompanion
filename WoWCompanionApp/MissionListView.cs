using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using WowStaticData;

namespace WoWCompanionApp
{
	public class MissionListView : MonoBehaviour
	{
		private PersistentMissionData missionData;

		public GameObject missionListViewContents;

		public GameObject missionListItemPrefab;

		public GameObject collectLootListItemPrefab;

		public bool isInProgressMissionList;

		public GameObject missionRewardDisplayPrefab;

		public MissionListView()
		{
		}

		private void InitMissionList()
		{
			RectTransform[] componentsInChildren = this.missionListViewContents.GetComponentsInChildren<RectTransform>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] != null && componentsInChildren[i] != this.missionListViewContents.transform)
				{
					UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
				}
			}
			List<WrapperGarrisonMission> list = PersistentMissionData.missionDictionary.Values.ToList<WrapperGarrisonMission>();
			list = (!this.isInProgressMissionList ? (
				from mission in list
				orderby StaticDB.garrMissionDB.GetRecord(mission.MissionRecID).TargetLevel
				select mission).ToList<WrapperGarrisonMission>() : (
				from mission in list
				orderby mission.StartTime + mission.MissionDuration
				select mission).ToList<WrapperGarrisonMission>());
			foreach (WrapperGarrisonMission wrapperGarrisonMission in list)
			{
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(wrapperGarrisonMission.MissionRecID);
				if (record != null)
				{
					if (record.GarrFollowerTypeID == (uint)GarrisonStatus.GarrisonFollowerType)
					{
						if (this.isInProgressMissionList)
						{
							if (wrapperGarrisonMission.MissionState == 0)
							{
								continue;
							}
							else if (wrapperGarrisonMission.MissionState == 1)
							{
								TimeSpan timeSpan = GarrisonStatus.CurrentTime() - wrapperGarrisonMission.StartTime;
								if ((wrapperGarrisonMission.MissionDuration - timeSpan).TotalSeconds <= 0)
								{
									continue;
								}
							}
						}
						if (this.isInProgressMissionList || wrapperGarrisonMission.MissionState == 0)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.missionListItemPrefab);
							gameObject.transform.SetParent(this.missionListViewContents.transform, false);
							gameObject.GetComponent<MissionListItem>().Init(record.ID);
						}
					}
				}
			}
		}

		public void OnUIRefresh()
		{
			this.InitMissionList();
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}