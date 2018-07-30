using System;
using UnityEngine;
using WowStatConstants;

namespace WoWCompanionApp
{
	public class MissionPanelSlider : MonoBehaviour
	{
		public MissionDetailView m_missionDetailView;

		public FollowerDetailView m_followerDetailView;

		public bool m_isVertical;

		public bool m_disablePreview;

		public bool m_usedForMissionList;

		public SliderPanel m_sliderPanel;

		private int m_garrFollowerID;

		public MissionPanelSlider()
		{
		}

		public void HandleMissionChanged(int garrMissionID)
		{
			if (garrMissionID <= 0)
			{
				this.m_sliderPanel.HideSliderPanel();
			}
			else if (!this.m_disablePreview)
			{
				this.m_sliderPanel.ShowSliderPanel();
			}
			else
			{
				this.m_sliderPanel.MaximizeSliderPanel();
			}
			iTween.StopByName(base.gameObject, "bounce");
			if (!this.m_disablePreview)
			{
				iTween.PunchPosition(base.gameObject, iTween.Hash(new object[] { "name", "bounce", "y", 16, "time", 2.2, "delay", 4, "looptype", "loop" }));
			}
		}

		private void HandleSliderPanelFinishMinimize()
		{
			AdventureMapPanel.instance.SelectMissionFromMap(0);
		}

		private void OnDisable()
		{
			AdventureMapPanel.instance.OnZoomOutMap -= new Action(this.OnZoomOutMap);
			if (!this.m_usedForMissionList)
			{
				AdventureMapPanel.instance.MissionMapSelectionChangedAction -= new Action<int>(this.HandleMissionChanged);
			}
			else
			{
				AdventureMapPanel.instance.MissionSelectedFromListAction -= new Action<int>(this.HandleMissionChanged);
			}
			AdventureMapPanel.instance.OnShowMissionRewardPanel -= new Action<bool>(this.OnShowMissionRewardPanel);
			this.m_sliderPanel.SliderPanelMaximizedAction -= new Action(this.OnSliderPanelMaximized);
			this.m_sliderPanel.SliderPanelBeginMinimizeAction -= new Action(this.RevealMap);
			this.m_sliderPanel.SliderPanelBeginDragAction -= new Action(this.RevealMap);
			this.m_sliderPanel.SliderPanelBeginShrinkToPreviewPositionAction -= new Action(this.RevealMap);
			this.m_sliderPanel.SliderPanelFinishMinimizeAction -= new Action(this.HandleSliderPanelFinishMinimize);
			Main.instance.m_backButtonManager.PopBackAction();
		}

		private void OnEnable()
		{
			this.m_sliderPanel = base.GetComponent<SliderPanel>();
			this.m_sliderPanel.m_masterCanvasGroup.alpha = 0f;
			AdventureMapPanel.instance.OnZoomOutMap += new Action(this.OnZoomOutMap);
			if (!this.m_usedForMissionList)
			{
				AdventureMapPanel.instance.MissionMapSelectionChangedAction += new Action<int>(this.HandleMissionChanged);
			}
			else
			{
				AdventureMapPanel.instance.MissionSelectedFromListAction += new Action<int>(this.HandleMissionChanged);
			}
			AdventureMapPanel.instance.OnShowMissionRewardPanel += new Action<bool>(this.OnShowMissionRewardPanel);
			this.m_sliderPanel.SliderPanelMaximizedAction += new Action(this.OnSliderPanelMaximized);
			this.m_sliderPanel.SliderPanelBeginMinimizeAction += new Action(this.RevealMap);
			this.m_sliderPanel.SliderPanelBeginDragAction += new Action(this.RevealMap);
			this.m_sliderPanel.SliderPanelBeginShrinkToPreviewPositionAction += new Action(this.RevealMap);
			this.m_sliderPanel.SliderPanelFinishMinimizeAction += new Action(this.HandleSliderPanelFinishMinimize);
			Main.instance.m_backButtonManager.PushBackAction(BackActionType.hideSliderPanel, this.m_sliderPanel.gameObject);
		}

		private void OnFollowerDetailViewSliderPanelMaximized()
		{
			this.m_missionDetailView.m_topLevelDetailViewCanvasGroup.alpha = 0f;
		}

		private void OnShowMissionRewardPanel(bool show)
		{
			if (show)
			{
				this.m_sliderPanel.HideSliderPanel();
			}
		}

		private void OnSliderPanelMaximized()
		{
		}

		public void OnZoomOutMap()
		{
			this.m_sliderPanel.HideSliderPanel();
		}

		public void PlayMinimizeSound()
		{
			Main.instance.m_UISound.Play_DefaultNavClick();
		}

		private void RevealMap()
		{
		}

		private void RevealMissionDetails()
		{
			this.m_missionDetailView.m_topLevelDetailViewCanvasGroup.alpha = 1f;
		}

		public void StopTheBounce()
		{
			iTween.StopByName(base.gameObject, "bounce");
		}
	}
}