using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WowJamMessages;
using WowStaticData;

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
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
			}
		}
		List<JamGarrisonMobileMission> list = PersistentMissionData.missionDictionary.Values.OfType<JamGarrisonMobileMission>().ToList<JamGarrisonMobileMission>();
		if (!this.isInProgressMissionList)
		{
			list.Sort(new MissionListView.MissionLevelComparer());
		}
		else
		{
			list.Sort(new MissionListView.MissionTimeComparer());
		}
		foreach (JamGarrisonMobileMission jamGarrisonMobileMission in list)
		{
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(jamGarrisonMobileMission.MissionRecID);
			if (record != null)
			{
				if (record.GarrFollowerTypeID == 4)
				{
					if (this.isInProgressMissionList)
					{
						if (jamGarrisonMobileMission.MissionState == 0)
						{
							continue;
						}
						else if (jamGarrisonMobileMission.MissionState == 1)
						{
							long num = GarrisonStatus.CurrentTime() - jamGarrisonMobileMission.StartTime;
							if (jamGarrisonMobileMission.MissionDuration - num <= (long)0)
							{
								continue;
							}
						}
					}
					if (this.isInProgressMissionList || jamGarrisonMobileMission.MissionState == 0)
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

	private class MissionLevelComparer : IComparer<JamGarrisonMobileMission>
	{
		public MissionLevelComparer()
		{
		}

		public int Compare(JamGarrisonMobileMission m1, JamGarrisonMobileMission m2)
		{
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(m1.MissionRecID);
			GarrMissionRec garrMissionRec = StaticDB.garrMissionDB.GetRecord(m2.MissionRecID);
			if (record == null || garrMissionRec == null)
			{
				return 0;
			}
			return garrMissionRec.TargetLevel - record.TargetLevel;
		}
	}

	public class MissionTimeComparer : IComparer<JamGarrisonMobileMission>
	{
		public MissionTimeComparer()
		{
		}

		public int Compare(JamGarrisonMobileMission m1, JamGarrisonMobileMission m2)
		{
			long num = GarrisonStatus.CurrentTime() - m1.StartTime;
			long missionDuration = m1.MissionDuration - num;
			long num1 = GarrisonStatus.CurrentTime() - m2.StartTime;
			return (int)(missionDuration - (m2.MissionDuration - num1));
		}
	}
}