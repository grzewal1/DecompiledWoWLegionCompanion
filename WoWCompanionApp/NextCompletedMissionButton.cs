using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;

namespace WoWCompanionApp
{
	public class NextCompletedMissionButton : MonoBehaviour
	{
		public GameObject m_treasureChestAlliance;

		public GameObject m_treasureChestHorde;

		public Text m_numReadyTroopsText;

		public GameObject m_theActualButton;

		public MiniMissionListPanel m_miniMissionListPanel;

		public float amount = 0.1f;

		public float duration = 1.2f;

		public float delay = 1.8f;

		public int m_numReadyTroops;

		private UiAnimMgr.UiAnimHandle m_glowHandle;

		private UiAnimMgr.UiAnimHandle m_glowLoopHandle;

		public NextCompletedMissionButton()
		{
		}

		private void ClearEffects()
		{
			iTween.StopByName(this.m_theActualButton, "RecruitWobble");
			iTween.StopByName(this.m_theActualButton, "RecruitWobbleL");
			iTween.StopByName(this.m_theActualButton, "RecruitButtonSwing");
			this.m_theActualButton.transform.localScale = Vector3.one;
			this.m_theActualButton.transform.localRotation = Quaternion.identity;
			if (this.m_glowHandle != null)
			{
				UiAnimation anim = this.m_glowHandle.GetAnim();
				if (anim != null)
				{
					anim.Stop(0.5f);
				}
			}
			if (this.m_glowLoopHandle != null)
			{
				UiAnimation uiAnimation = this.m_glowLoopHandle.GetAnim();
				if (uiAnimation != null)
				{
					uiAnimation.Stop(0.5f);
				}
			}
		}

		private void OnEnable()
		{
			this.m_theActualButton.SetActive(false);
			this.m_numReadyTroops = 0;
			this.m_numReadyTroopsText.text = string.Empty;
			if (this.m_treasureChestHorde != null && this.m_treasureChestAlliance != null)
			{
				if (GarrisonStatus.Faction() != PVP_FACTION.HORDE)
				{
					this.m_treasureChestHorde.SetActive(false);
					this.m_treasureChestAlliance.SetActive(true);
				}
				else
				{
					this.m_treasureChestHorde.SetActive(true);
					this.m_treasureChestAlliance.SetActive(false);
				}
			}
		}

		private void Update()
		{
			int numCompletedMissions = PersistentMissionData.GetNumCompletedMissions(true);
			if (numCompletedMissions != this.m_numReadyTroops)
			{
				this.m_theActualButton.SetActive(numCompletedMissions > 0);
				if (numCompletedMissions == 0)
				{
					this.ClearEffects();
				}
				if (numCompletedMissions > this.m_numReadyTroops)
				{
					this.ClearEffects();
					this.m_glowHandle = UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", this.m_theActualButton.transform, Vector3.zero, 3f, 0f);
					this.m_glowLoopHandle = UiAnimMgr.instance.PlayAnim("MinimapLoopPulseAnim", this.m_theActualButton.transform, Vector3.zero, 3f, 0f);
					iTween.PunchScale(this.m_theActualButton, iTween.Hash(new object[] { "name", "RecruitWobble", "x", this.amount, "y", this.amount, "time", this.duration, "delay", 0.1f, "looptype", iTween.LoopType.none }));
					iTween.PunchScale(this.m_theActualButton, iTween.Hash(new object[] { "name", "RecruitWobbleL", "x", this.amount, "y", this.amount, "time", this.duration, "delay", this.delay, "looptype", iTween.LoopType.loop }));
					iTween.PunchRotation(this.m_theActualButton, iTween.Hash(new object[] { "name", "RecruitButtonSwing", "z", -30f, "time", 2f }));
					Main.instance.m_UISound.Play_LootReady();
				}
				this.m_numReadyTroops = numCompletedMissions;
				this.m_numReadyTroopsText.text = string.Concat(string.Empty, (this.m_numReadyTroops <= 0 ? string.Empty : string.Concat(string.Empty, this.m_numReadyTroops)));
			}
		}

		public void ZoomToNextCompleteMission()
		{
			this.m_miniMissionListPanel.InitMissionList();
			MiniMissionListItem[] componentsInChildren = this.m_miniMissionListPanel.m_inProgressMission_listContents.GetComponentsInChildren<MiniMissionListItem>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				MiniMissionListItem miniMissionListItem = componentsInChildren[i];
				if (PersistentMissionData.missionDictionary.ContainsKey(miniMissionListItem.GetMissionID()))
				{
					WrapperGarrisonMission item = PersistentMissionData.missionDictionary[miniMissionListItem.GetMissionID()];
					if (item.MissionState == 1)
					{
						TimeSpan timeSpan = GarrisonStatus.CurrentTime() - item.StartTime;
						if ((item.MissionDuration - timeSpan).TotalSeconds <= 0)
						{
							AdventureMapPanel.instance.ShowMissionResultAction(item.MissionRecID, 0, false);
							break;
						}
					}
				}
			}
		}
	}
}