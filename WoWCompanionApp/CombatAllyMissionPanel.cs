using System;
using System.Collections.Generic;
using UnityEngine;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
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
			foreach (WrapperGarrisonMission value in PersistentMissionData.missionDictionary.Values)
			{
				GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(value.MissionRecID);
				if (record != null)
				{
					if ((record.Flags & 16) == 0)
					{
						continue;
					}
					missionRecID = value.MissionRecID;
					combatAllyMissionState = (value.MissionState != 1 ? CombatAllyMissionState.available : CombatAllyMissionState.inProgress);
					break;
				}
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
}