using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowStaticData;

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

	public void CancelCheatCompleteMission()
	{
		this.CompleteMissionCheatPopup.SetActive(false);
	}

	public void CheatCompleteMission()
	{
		if (this.m_currentCheatMissionID > 0)
		{
			Main.instance.ExpediteMissionCheat(this.m_currentCheatMissionID);
		}
		this.m_currentCheatMissionID = 0;
		this.CompleteMissionCheatPopup.SetActive(false);
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

	public void ShowCheatCompleteMission(int garrMissionID)
	{
		this.m_currentCheatMissionID = garrMissionID;
		this.CompleteMissionCheatPopup.SetActive(true);
		this.CompleteMissionCheatPopup.transform.SetAsLastSibling();
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
		IEnumerator enumerator = PersistentMissionData.missionDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamGarrisonMobileMission current = (JamGarrisonMobileMission)enumerator.Current;
				bool flag = false;
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(current.MissionRecID);
				if (record != null)
				{
					if (record.GarrFollowerTypeID == 4)
					{
						if (current.MissionState == 1)
						{
							long num = GarrisonStatus.CurrentTime() - current.StartTime;
							if (current.MissionDuration - num <= (long)0)
							{
								flag = true;
							}
						}
						if (current.MissionState == 2)
						{
							flag = true;
						}
						!flag;
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
	}
}