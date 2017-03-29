using System;
using System.Collections;
using UnityEngine;
using WowJamMessages;
using WowStatConstants;
using WowStaticData;

public class CombatAllyMissionPanel : MonoBehaviour
{
	public MissionDetailView m_missionDetailView;

	public SliderPanel m_sliderPanel;

	public CombatAllyMissionPanel()
	{
	}

	public void Hide()
	{
		this.m_sliderPanel.HideSliderPanel();
	}

	public void Show()
	{
		int missionRecID = 0;
		CombatAllyMissionState combatAllyMissionState = CombatAllyMissionState.notAvailable;
		IEnumerator enumerator = PersistentMissionData.missionDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamGarrisonMobileMission current = (JamGarrisonMobileMission)enumerator.Current;
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(current.MissionRecID);
				if (record != null)
				{
					if ((record.Flags & 16) == 0)
					{
						continue;
					}
					missionRecID = current.MissionRecID;
					combatAllyMissionState = (current.MissionState != 1 ? CombatAllyMissionState.available : CombatAllyMissionState.inProgress);
					break;
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
		if (missionRecID > 0)
		{
			this.m_missionDetailView.HandleMissionSelected(missionRecID);
		}
		this.m_missionDetailView.SetCombatAllyMissionState(combatAllyMissionState);
		this.m_sliderPanel.MaximizeSliderPanel();
	}

	private void Update()
	{
	}
}