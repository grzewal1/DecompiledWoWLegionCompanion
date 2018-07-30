using System;
using System.Collections.Generic;
using UnityEngine;
using WowStaticData;

namespace WoWCompanionApp
{
	public class RewardPanelSlider : MonoBehaviour
	{
		public SliderPanel m_sliderPanel;

		public GameObject m_rewardIconArea;

		public bool m_isVertical;

		public RewardPanelSlider()
		{
		}

		private void Awake()
		{
			this.ClearRewardIcons();
			this.m_sliderPanel = base.GetComponent<SliderPanel>();
			AdventureMapPanel.instance.OnZoomOutMap += new Action(this.OnZoomOutMap);
			AdventureMapPanel.instance.MissionMapSelectionChangedAction += new Action<int>(this.HandleMissionChanged);
			AdventureMapPanel.instance.OnAddMissionLootToRewardPanel += new Action<int>(this.OnAddMissionLootToRewardPanel);
			AdventureMapPanel.instance.OnShowMissionRewardPanel += new Action<bool>(this.OnShowMissionRewardPanel);
		}

		public void ClearRewardIcons()
		{
			MissionRewardDisplay[] componentsInChildren = this.m_rewardIconArea.GetComponentsInChildren<MissionRewardDisplay>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
		}

		private void HandleMissionChanged(int garrMissionID)
		{
			this.m_sliderPanel.HideSliderPanel();
		}

		public void OnAddMissionLootToRewardPanel(int garrMissionID)
		{
			WrapperGarrisonMission item = PersistentMissionData.missionDictionary[garrMissionID];
			MissionRewardDisplay.InitMissionRewards(AdventureMapPanel.instance.m_missionRewardResultsDisplayPrefab, this.m_rewardIconArea.transform, item.Rewards);
			if (item.MissionState != 3)
			{
				return;
			}
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(garrMissionID);
			if (record == null)
			{
				return;
			}
			if (StaticDB.rewardPackDB.GetRecord(record.OvermaxRewardPackID) == null)
			{
				return;
			}
			if (item.OvermaxRewards.Count > 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(AdventureMapPanel.instance.m_missionRewardResultsDisplayPrefab);
				gameObject.transform.SetParent(this.m_rewardIconArea.transform, false);
				MissionRewardDisplay component = gameObject.GetComponent<MissionRewardDisplay>();
				int itemID = item.OvermaxRewards[0].ItemID;
				uint itemQuantity = item.OvermaxRewards[0].ItemQuantity;
				WrapperGarrisonMissionReward wrapperGarrisonMissionReward = item.OvermaxRewards[0];
				component.InitReward(MissionRewardDisplay.RewardType.item, itemID, (int)itemQuantity, 0, wrapperGarrisonMissionReward.ItemFileDataID);
			}
		}

		public void OnShowMissionRewardPanel(bool show)
		{
			if (!show)
			{
				this.m_sliderPanel.HideSliderPanel();
			}
			else
			{
				this.m_sliderPanel.ShowSliderPanel();
			}
		}

		public void OnShowMissionRewardSlider(bool show)
		{
			if (!show)
			{
				this.m_sliderPanel.HideSliderPanel();
			}
			else
			{
				this.m_sliderPanel.ShowSliderPanel();
			}
		}

		private void OnZoomOutMap()
		{
			this.m_sliderPanel.HideSliderPanel();
		}
	}
}