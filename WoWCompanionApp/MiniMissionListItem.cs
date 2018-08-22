using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class MiniMissionListItem : MonoBehaviour
	{
		public Text m_missionName;

		public Image m_missionTypeIcon;

		public Image m_missionTypeBG;

		public Image m_missionLocation;

		public Sprite m_starSprite;

		public Sprite m_swordSprite;

		public Sprite m_bootSprite;

		public Sprite m_stealthSprite;

		public Sprite m_scrollSprite;

		public Text m_missionLevel;

		public Text m_missionTime;

		public Text m_rareMissionLabel;

		public Text m_statusText;

		public MissionRewardDisplay m_missionRewardDisplayPrefab;

		public GameObject m_previewLootGroup;

		public GameObject m_previewMechanicEffectPrefab;

		public GameObject m_previewMechanicsGroup;

		private WrapperGarrisonMission m_mission;

		private int m_starIconId = 7746;

		private int m_swordIconID = 7747;

		private int m_bootIconId = 7748;

		private int m_stealthIconId = 7749;

		private int m_scrollIconId = 8046;

		private int[] m_previewAbilityID;

		private FollowerCanCounterMechanic[] m_previewCanCounterStatus;

		public MiniMissionListItem()
		{
		}

		private void Awake()
		{
			this.m_missionName.font = GeneralHelpers.LoadFancyFont();
			this.m_missionLevel.font = GeneralHelpers.LoadStandardFont();
			this.m_missionTime.font = GeneralHelpers.LoadStandardFont();
			this.m_statusText.font = GeneralHelpers.LoadStandardFont();
			this.m_previewAbilityID = new int[3];
			this.m_previewCanCounterStatus = new FollowerCanCounterMechanic[3];
		}

		public int GetMissionID()
		{
			return this.m_mission.MissionRecID;
		}

		private int GetUncounteredMissionDuration(WrapperGarrisonMission mission)
		{
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(mission.MissionRecID);
			if (record == null)
			{
				return 0;
			}
			float missionDuration = (float)record.MissionDuration;
			foreach (WrapperGarrisonEncounter encounter in mission.Encounters)
			{
				foreach (int mechanicID in encounter.MechanicIDs)
				{
					GarrMechanicRec garrMechanicRec = StaticDB.garrMechanicDB.GetRecord(mechanicID);
					if (garrMechanicRec != null)
					{
						foreach (GarrAbilityEffectRec garrAbilityEffectRec in 
							from rec in StaticDB.garrAbilityEffectDB.GetRecordsByParentID(garrMechanicRec.GarrAbilityID)
							where rec.AbilityAction == 17
							select rec)
						{
							missionDuration *= garrAbilityEffectRec.ActionValueFlat;
						}
					}
				}
			}
			missionDuration *= GeneralHelpers.GetMissionDurationTalentMultiplier();
			return (int)missionDuration;
		}

		public void OnTap()
		{
			this.PlayClickSound();
			AllPopups.instance.HideAllPopups();
			if (this.m_mission.MissionState == 1)
			{
				if (AdventureMapPanel.instance.ShowMissionResultAction != null)
				{
					AdventureMapPanel.instance.ShowMissionResultAction(this.m_mission.MissionRecID, 0, false);
				}
				return;
			}
			if (this.m_mission.MissionState != 0)
			{
				return;
			}
			Singleton<DialogFactory>.Instance.CreateMissionDialog(this.m_mission.MissionRecID);
		}

		public void PlayClickSound()
		{
			Main.instance.m_UISound.Play_ButtonBlackClick();
		}

		public void SetMission(WrapperGarrisonMission mission)
		{
			this.m_statusText.gameObject.SetActive(false);
			this.m_mission = mission;
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(mission.MissionRecID);
			if (record == null)
			{
				return;
			}
			if (this.m_missionTypeIcon != null)
			{
				GarrMissionTypeRec garrMissionTypeRec = StaticDB.garrMissionTypeDB.GetRecord((int)record.GarrMissionTypeID);
				if ((ulong)garrMissionTypeRec.UiTextureAtlasMemberID == (long)this.m_starIconId)
				{
					this.m_missionTypeIcon.sprite = this.m_starSprite;
				}
				if ((ulong)garrMissionTypeRec.UiTextureAtlasMemberID == (long)this.m_swordIconID)
				{
					this.m_missionTypeIcon.sprite = this.m_swordSprite;
				}
				if ((ulong)garrMissionTypeRec.UiTextureAtlasMemberID == (long)this.m_bootIconId)
				{
					this.m_missionTypeIcon.sprite = this.m_bootSprite;
				}
				if ((ulong)garrMissionTypeRec.UiTextureAtlasMemberID == (long)this.m_stealthIconId)
				{
					this.m_missionTypeIcon.sprite = this.m_stealthSprite;
				}
				if ((ulong)garrMissionTypeRec.UiTextureAtlasMemberID == (long)this.m_scrollIconId)
				{
					this.m_missionTypeIcon.sprite = this.m_scrollSprite;
				}
			}
			bool flag = false;
			if (mission.MissionState == 1)
			{
				flag = true;
				this.m_statusText.gameObject.SetActive(true);
				this.m_missionTime.gameObject.SetActive(false);
			}
			this.m_previewMechanicsGroup.SetActive(!flag);
			Duration duration = new Duration(this.GetUncounteredMissionDuration(mission), false);
			string str = "123 Min";
			str = (duration.DurationValue < 28800 ? string.Concat("<color=#BEBEBEFF>", duration.DurationString, "</color>") : string.Concat("<color=#ff8600ff>", duration.DurationString, "</color>"));
			this.m_missionTime.text = string.Concat("(", str, ")");
			this.m_missionName.text = record.Name;
			if (this.m_missionLevel != null)
			{
				if (record.TargetLevel >= 110)
				{
					this.m_missionLevel.text = string.Concat(new object[] { string.Empty, record.TargetLevel, "\n(", record.TargetItemLevel, ")" });
				}
				else
				{
					this.m_missionLevel.text = string.Concat(string.Empty, record.TargetLevel);
				}
			}
			bool flags = (record.Flags & 1) != 0;
			this.m_rareMissionLabel.gameObject.SetActive(flags);
			if (!flags)
			{
				this.m_missionTypeBG.color = new Color(0f, 0f, 0f, 0.478f);
			}
			else
			{
				this.m_missionTypeBG.color = new Color(0f, 0f, 1f, 0.24f);
			}
			this.m_missionLocation.enabled = false;
			UiTextureKitRec uiTextureKitRec = StaticDB.uiTextureKitDB.GetRecord((int)record.UiTextureKitID);
			if (uiTextureKitRec != null)
			{
				int uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID(string.Concat(uiTextureKitRec.KitPrefix, "-List"));
				if (uITextureAtlasMemberID > 0)
				{
					Sprite atlasSprite = TextureAtlas.instance.GetAtlasSprite(uITextureAtlasMemberID);
					if (atlasSprite != null)
					{
						this.m_missionLocation.enabled = true;
						this.m_missionLocation.sprite = atlasSprite;
					}
				}
			}
			this.UpdateMechanicPreview(flag, mission);
			MissionRewardDisplay[] componentsInChildren = this.m_previewLootGroup.GetComponentsInChildren<MissionRewardDisplay>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] != null)
				{
					UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
				}
			}
			MissionRewardDisplay.InitMissionRewards(this.m_missionRewardDisplayPrefab.gameObject, this.m_previewLootGroup.transform, mission.Rewards);
		}

		private void Update()
		{
			TimeSpan missionDuration;
			if (this.m_mission.MissionState == 1)
			{
				TimeSpan timeSpan = GarrisonStatus.CurrentTime() - this.m_mission.StartTime;
				missionDuration = this.m_mission.MissionDuration - timeSpan;
				if (missionDuration.TotalSeconds <= 0)
				{
					this.m_statusText.text = string.Concat("<color=#00ff00ff>(", StaticDB.GetString("TAP_TO_COMPLETE", null), ")</color>");
				}
				else
				{
					this.m_statusText.text = string.Concat(missionDuration.GetDurationString(false), " <color=#ff0000ff>(", StaticDB.GetString("IN_PROGRESS", null), ")</color>");
				}
			}
			TimeSpan timeSpan1 = GarrisonStatus.CurrentTime() - this.m_mission.OfferTime;
			missionDuration = this.m_mission.OfferDuration - timeSpan1;
			missionDuration = (missionDuration.TotalSeconds <= 0 ? TimeSpan.Zero : missionDuration);
			if (missionDuration.TotalSeconds > 0)
			{
				missionDuration.GetDurationString(false);
			}
			else if (this.m_mission.MissionState == 0 && this.m_mission.OfferDuration.TotalSeconds > 0)
			{
				AdventureMapPanel.instance.SelectMissionFromList(0);
				Singleton<DialogFactory>.Instance.CloseMissionDialog();
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
		}

		public void UpdateMechanicPreview(bool missionInProgress, WrapperGarrisonMission mission)
		{
			int num;
			int num1 = 0;
			if (!missionInProgress)
			{
				for (int i = 0; i < mission.Encounters.Count; i++)
				{
					num = (mission.Encounters[i].MechanicIDs.Count <= 0 ? 0 : mission.Encounters[i].MechanicIDs[0]);
					GarrMechanicRec record = StaticDB.garrMechanicDB.GetRecord(num);
					if (record != null && record.GarrAbilityID != 0)
					{
						this.m_previewAbilityID[num1] = record.GarrAbilityID;
						this.m_previewCanCounterStatus[num1] = GeneralHelpers.HasFollowerWhoCanCounter((int)record.GarrMechanicTypeID);
						num1++;
					}
				}
				bool flag = true;
				AbilityDisplay[] componentsInChildren = this.m_previewMechanicsGroup.GetComponentsInChildren<AbilityDisplay>(true);
				if (num1 != (int)componentsInChildren.Length)
				{
					flag = false;
				}
				if (flag)
				{
					int num2 = 0;
					while (num2 < (int)componentsInChildren.Length)
					{
						if (componentsInChildren[num2] == null)
						{
							flag = false;
							break;
						}
						else if (componentsInChildren[num2].GetAbilityID() != this.m_previewAbilityID[num2])
						{
							flag = false;
							break;
						}
						else if (componentsInChildren[num2].GetCanCounterStatus() == this.m_previewCanCounterStatus[num2])
						{
							num2++;
						}
						else
						{
							flag = false;
							break;
						}
					}
				}
				if (!flag)
				{
					for (int j = 0; j < (int)componentsInChildren.Length; j++)
					{
						if (componentsInChildren[j] != null)
						{
							UnityEngine.Object.Destroy(componentsInChildren[j].gameObject);
						}
					}
					for (int k = 0; k < num1; k++)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_previewMechanicEffectPrefab);
						gameObject.transform.SetParent(this.m_previewMechanicsGroup.transform, false);
						AbilityDisplay component = gameObject.GetComponent<AbilityDisplay>();
						component.SetAbility(this.m_previewAbilityID[k], false, false, null);
						component.SetCanCounterStatus(this.m_previewCanCounterStatus[k]);
					}
				}
			}
		}
	}
}