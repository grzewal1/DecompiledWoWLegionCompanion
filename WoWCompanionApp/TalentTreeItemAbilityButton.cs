using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class TalentTreeItemAbilityButton : MonoBehaviour
	{
		public Image m_abilityIcon;

		public Image m_greenFrameGlow;

		public Image m_yellowFrameGlow;

		public Image m_researchProgressBG;

		public Image m_researchProgressFill;

		public Text m_missingIconText;

		public Shader m_grayscaleShader;

		private string m_whyCantResearch;

		private WrapperGarrisonTalent m_talent;

		private int m_garrTalentID;

		private GarrTalentRec m_garrTalentRec;

		private Color m_inactiveColor;

		private bool m_canResearch;

		private TalentVisualState m_visualState;

		private bool m_requestedUpdate;

		private bool m_shouldShowCheckAnim;

		private bool m_playedShowCheckAnim;

		private bool m_playedTalentToast;

		private TalentTreeItemAbilityButton m_sameTierButton;

		public TalentTreeItemAbilityButton()
		{
		}

		private void Awake()
		{
			this.m_playedTalentToast = false;
			this.m_shouldShowCheckAnim = false;
			this.m_playedShowCheckAnim = false;
			this.m_inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);
			this.m_canResearch = false;
			this.m_greenFrameGlow.gameObject.SetActive(false);
			this.m_yellowFrameGlow.gameObject.SetActive(false);
			this.m_researchProgressBG.gameObject.SetActive(false);
			this.m_visualState = TalentVisualState.cannotResearch;
		}

		public bool CanResearch()
		{
			return this.m_canResearch;
		}

		public bool CanRespec()
		{
			return this.m_visualState == TalentVisualState.canRespec;
		}

		public TimeSpan GetRemainingResearchTime()
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)this.m_garrTalentRec.ResearchDurationSecs) - (GarrisonStatus.CurrentTime() - this.m_talent.StartTime);
			return timeSpan;
		}

		public TimeSpan GetRemainingRespecTime()
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)this.m_garrTalentRec.RespecDurationSecs) - (GarrisonStatus.CurrentTime() - this.m_talent.StartTime);
			return timeSpan;
		}

		public TalentTreeItemAbilityButton GetSameTierButton()
		{
			return this.m_sameTierButton;
		}

		public int GetTalentID()
		{
			return this.m_garrTalentID;
		}

		public string GetWhyCantResearch()
		{
			return this.m_whyCantResearch;
		}

		public void HandleCanResearchGarrisonTalentResult(int garrTalentID, int result, string whyCantResearch)
		{
			if (this.m_garrTalentID != garrTalentID)
			{
				return;
			}
			if (PersistentTalentData.talentDictionary.ContainsKey(garrTalentID))
			{
				this.m_talent = PersistentTalentData.talentDictionary[garrTalentID];
			}
			this.m_garrTalentRec = StaticDB.garrTalentDB.GetRecord(garrTalentID);
			this.m_canResearch = false;
			if (result != 0)
			{
				this.m_whyCantResearch = whyCantResearch;
			}
			else
			{
				this.m_canResearch = true;
			}
		}

		public void HandleGarrisonDataResetFinished()
		{
			this.m_requestedUpdate = false;
		}

		public void HandleTap()
		{
			AllPopups.instance.ShowTalentTooltip(this);
		}

		public bool IsOwned()
		{
			return (this.m_talent.Flags & 1) != 0;
		}

		public bool IsReadyToShowGreenCheckAnim()
		{
			return this.m_shouldShowCheckAnim;
		}

		public bool IsResearching()
		{
			return (this.IsOwned() ? false : this.m_talent.StartTime > DateTime.MinValue);
		}

		public bool IsRespec()
		{
			return (this.m_talent.Flags & 2) != 0;
		}

		public bool IsSet()
		{
			return this.m_garrTalentRec != null;
		}

		public void SetTalent(int garrTalentID, TalentTreeItemAbilityButton sameTierButton)
		{
			this.m_garrTalentID = garrTalentID;
			this.m_sameTierButton = sameTierButton;
			if (PersistentTalentData.talentDictionary.ContainsKey(garrTalentID))
			{
				this.m_talent = PersistentTalentData.talentDictionary[garrTalentID];
			}
			this.m_garrTalentRec = StaticDB.garrTalentDB.GetRecord(garrTalentID);
			Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, this.m_garrTalentRec.IconFileDataID);
			if (sprite == null)
			{
				this.m_missingIconText.gameObject.SetActive(true);
				this.m_missingIconText.text = string.Concat(string.Empty, this.m_garrTalentRec.IconFileDataID);
			}
			else
			{
				this.m_missingIconText.gameObject.SetActive(false);
				this.m_abilityIcon.sprite = sprite;
			}
			Material material = new Material(this.m_grayscaleShader);
			this.m_abilityIcon.material = material;
			this.m_abilityIcon.material.SetFloat("_GrayscaleAmount", 1f);
		}

		public void SetVisualState(TalentVisualState visualState)
		{
			this.m_visualState = visualState;
			switch (visualState)
			{
				case TalentVisualState.canResearch:
				{
					this.m_greenFrameGlow.gameObject.SetActive(true);
					this.m_yellowFrameGlow.gameObject.SetActive(false);
					this.m_researchProgressBG.gameObject.SetActive(false);
					this.m_abilityIcon.color = Color.white;
					this.m_abilityIcon.material.SetFloat("_GrayscaleAmount", 0f);
					break;
				}
				case TalentVisualState.canRespec:
				{
					this.m_greenFrameGlow.gameObject.SetActive(false);
					this.m_yellowFrameGlow.gameObject.SetActive(false);
					this.m_researchProgressBG.gameObject.SetActive(false);
					this.m_abilityIcon.color = this.m_inactiveColor;
					this.m_abilityIcon.material.SetFloat("_GrayscaleAmount", 0f);
					break;
				}
				case TalentVisualState.cannotResearch:
				{
					this.m_greenFrameGlow.gameObject.SetActive(false);
					this.m_yellowFrameGlow.gameObject.SetActive(false);
					this.m_researchProgressBG.gameObject.SetActive(false);
					this.m_abilityIcon.color = Color.white;
					this.m_abilityIcon.material.SetFloat("_GrayscaleAmount", 1f);
					break;
				}
				case TalentVisualState.researching:
				{
					this.m_greenFrameGlow.gameObject.SetActive(false);
					this.m_yellowFrameGlow.gameObject.SetActive(true);
					this.m_researchProgressBG.gameObject.SetActive(true);
					this.m_abilityIcon.color = this.m_inactiveColor;
					this.m_abilityIcon.material.SetFloat("_GrayscaleAmount", 1f);
					break;
				}
				case TalentVisualState.owned:
				{
					this.m_greenFrameGlow.gameObject.SetActive(false);
					this.m_yellowFrameGlow.gameObject.SetActive(true);
					this.m_researchProgressBG.gameObject.SetActive(false);
					this.m_abilityIcon.color = Color.white;
					this.m_abilityIcon.material.SetFloat("_GrayscaleAmount", 0f);
					break;
				}
			}
		}

		public void StartResearch()
		{
			if (this.m_canResearch)
			{
				LegionCompanionWrapper.ResearchGarrisonTalent(this.m_garrTalentRec.ID);
			}
		}

		private void Update()
		{
			float totalSeconds;
			if (this.IsResearching())
			{
				this.m_researchProgressBG.gameObject.SetActive(true);
				if (!this.IsRespec())
				{
					TimeSpan remainingResearchTime = this.GetRemainingResearchTime();
					totalSeconds = (float)remainingResearchTime.TotalSeconds / (float)this.m_garrTalentRec.ResearchDurationSecs;
				}
				else
				{
					TimeSpan remainingRespecTime = this.GetRemainingRespecTime();
					totalSeconds = (float)remainingRespecTime.TotalSeconds / (float)this.m_garrTalentRec.RespecDurationSecs;
				}
				float single = 1f - totalSeconds;
				this.m_researchProgressFill.fillAmount = single;
				this.SetVisualState(TalentVisualState.researching);
				if (!this.m_requestedUpdate && single >= 1f)
				{
					this.m_requestedUpdate = true;
					LegionCompanionWrapper.RequestGarrisonData(3);
					this.m_shouldShowCheckAnim = true;
					if (!AllPanels.instance.m_orderHallMultiPanel.m_talentNavButton.IsSelected() && !this.m_playedTalentToast)
					{
						Main.instance.m_UISound.Play_TalentReadyToast();
						this.m_playedTalentToast = true;
					}
				}
			}
			if (this.m_shouldShowCheckAnim && !this.m_playedShowCheckAnim && AllPanels.instance.m_orderHallMultiPanel.m_talentNavButton.IsSelected())
			{
				UiAnimMgr.instance.PlayAnim("TalentDoneAnim", base.transform, Vector3.zero, 1f, 0f);
				Main.instance.m_UISound.Play_TalentReadyCheck();
				this.m_shouldShowCheckAnim = false;
				this.m_playedShowCheckAnim = true;
			}
		}
	}
}