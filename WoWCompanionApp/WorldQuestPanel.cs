using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class WorldQuestPanel : MonoBehaviour
	{
		public SliderPanel m_sliderPanel;

		public Text m_worldQuestNameText;

		public Text m_worldQuestDescriptionText;

		public Text m_worldQuestTimeText;

		public MissionRewardDisplay m_missionRewardDisplayPrefab;

		public GameObject m_lootGroupObj;

		private int m_questID;

		public int QuestID
		{
			get
			{
				return this.m_questID;
			}
			set
			{
				this.m_questID = value;
				MissionRewardDisplay[] componentsInChildren = this.m_lootGroupObj.GetComponentsInChildren<MissionRewardDisplay>(true);
				for (int i = 0; i < (int)componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] != null)
					{
						UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
					}
				}
				if (this.m_questID > 0 && WorldQuestData.WorldQuestDictionary.ContainsKey(this.m_questID))
				{
					WrapperWorldQuest item = WorldQuestData.WorldQuestDictionary[this.m_questID];
					this.m_worldQuestNameText.text = item.QuestTitle;
					this.m_worldQuestDescriptionText.text = item.QuestTitle;
					TimeSpan endTime = item.EndTime - GarrisonStatus.CurrentTime();
					if (endTime.TotalSeconds < 0)
					{
						endTime = TimeSpan.Zero;
					}
					this.m_worldQuestTimeText.text = endTime.GetDurationString(false);
					MissionRewardDisplay.InitWorldQuestRewards(item, this.m_missionRewardDisplayPrefab.gameObject, this.m_lootGroupObj.transform);
				}
			}
		}

		public WorldQuestPanel()
		{
		}

		private void Awake()
		{
			this.m_sliderPanel = base.GetComponent<SliderPanel>();
			AdventureMapPanel.instance.OnZoomOutMap += new Action(this.OnZoomOutMap);
			AdventureMapPanel.instance.MissionMapSelectionChangedAction += new Action<int>(this.HandleMissionChanged);
			AdventureMapPanel.instance.OnShowMissionRewardPanel += new Action<bool>(this.OnShowMissionRewardPanel);
			AdventureMapPanel.instance.WorldQuestChangedAction += new Action<int>(this.HandleWorldQuestChanged);
		}

		public void HandleMissionChanged(int garrMissionID)
		{
			if (garrMissionID != 0)
			{
				this.m_sliderPanel.HideSliderPanel();
			}
		}

		private void HandleWorldQuestChanged(int worldQuestID)
		{
			this.QuestID = worldQuestID;
			if (this.QuestID == 0)
			{
				this.m_sliderPanel.HideSliderPanel();
			}
			else
			{
				this.m_sliderPanel.ShowSliderPanel();
			}
		}

		private void OnShowMissionRewardPanel(bool show)
		{
			this.m_sliderPanel.HideSliderPanel();
		}

		public void OnZoomOutMap()
		{
			this.m_sliderPanel.HideSliderPanel();
		}
	}
}