using GarbageFreeStringBuilder;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowStatConstants;
using WowStaticData;

public class MiniMissionListItem : MonoBehaviour
{
	public Text m_missionName;

	public Image m_missionTypeIcon;

	public Image m_rareMissionHighlight;

	public Image m_missionTypeBG;

	public Image m_missionLocation;

	public Image m_statusDarkener;

	public Text m_missionLevel;

	public Text m_missionTime;

	public Text m_rareMissionLabel;

	public Text m_statusText;

	public Text m_expirationText;

	public MissionRewardDisplay m_missionRewardDisplayPrefab;

	public GameObject m_previewLootGroup;

	public GameObject m_previewMechanicEffectPrefab;

	public GameObject m_previewMechanicsGroup;

	private JamGarrisonMobileMission m_mission;

	private int[] m_previewAbilityID;

	private FollowerCanCounterMechanic[] m_previewCanCounterStatus;

	private Duration m_missionOfferTimeRemaining;

	private StringBuilder m_missionOfferTimeSB;

	private void Awake()
	{
		this.m_missionName.set_font(GeneralHelpers.LoadFancyFont());
		this.m_missionLevel.set_font(GeneralHelpers.LoadStandardFont());
		this.m_missionTime.set_font(GeneralHelpers.LoadStandardFont());
		this.m_rareMissionLabel.set_font(GeneralHelpers.LoadFancyFont());
		this.m_statusText.set_font(GeneralHelpers.LoadStandardFont());
		this.m_rareMissionLabel.set_text(StaticDB.GetString("RARE", "Rare!"));
		this.m_previewAbilityID = new int[3];
		this.m_previewCanCounterStatus = new FollowerCanCounterMechanic[3];
		this.m_missionOfferTimeRemaining = new Duration(0, false);
		this.m_missionOfferTimeSB = new StringBuilder(16);
	}

	public void SetMission(JamGarrisonMobileMission mission)
	{
		this.m_statusDarkener.get_gameObject().SetActive(false);
		this.m_statusText.get_gameObject().SetActive(false);
		this.m_mission = mission;
		GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(mission.MissionRecID);
		if (record == null)
		{
			return;
		}
		if (this.m_missionTypeIcon != null)
		{
			GarrMissionTypeRec record2 = StaticDB.garrMissionTypeDB.GetRecord((int)record.GarrMissionTypeID);
			this.m_missionTypeIcon.set_sprite(TextureAtlas.instance.GetAtlasSprite((int)record2.UiTextureAtlasMemberID));
		}
		bool flag = false;
		if (mission.MissionState == 1)
		{
			flag = true;
			this.m_statusDarkener.get_gameObject().SetActive(true);
			this.m_statusDarkener.set_color(new Color(0f, 0f, 0f, 0.3529412f));
			this.m_statusText.get_gameObject().SetActive(true);
			this.m_missionTime.get_gameObject().SetActive(false);
		}
		this.m_previewMechanicsGroup.SetActive(!flag);
		Duration duration = new Duration(record.MissionDuration, false);
		string text;
		if (duration.DurationValue >= 28800)
		{
			text = "<color=#ff8600ff>" + duration.DurationString + "</color>";
		}
		else
		{
			text = "<color=#BEBEBEFF>" + duration.DurationString + "</color>";
		}
		this.m_missionTime.set_text("(" + text + ")");
		this.m_missionName.set_text(record.Name);
		if (this.m_missionLevel != null)
		{
			if (record.TargetLevel < 110)
			{
				this.m_missionLevel.set_text(string.Empty + record.TargetLevel);
			}
			else
			{
				this.m_missionLevel.set_text(string.Concat(new object[]
				{
					string.Empty,
					record.TargetLevel,
					"\n(",
					record.TargetItemLevel,
					")"
				}));
			}
		}
		bool flag2 = (record.Flags & 1u) != 0u;
		this.m_expirationText.get_gameObject().SetActive(flag2);
		this.m_rareMissionLabel.get_gameObject().SetActive(flag2);
		this.m_rareMissionHighlight.get_gameObject().SetActive(flag2);
		if (flag2)
		{
			this.m_missionTypeBG.set_color(new Color(0f, 0f, 1f, 0.24f));
		}
		else
		{
			this.m_missionTypeBG.set_color(new Color(0f, 0f, 0f, 0.478f));
		}
		this.m_missionLocation.set_enabled(false);
		UiTextureKitRec record3 = StaticDB.uiTextureKitDB.GetRecord((int)record.UiTextureKitID);
		if (record3 != null)
		{
			int uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID(record3.KitPrefix + "-List");
			if (uITextureAtlasMemberID > 0)
			{
				Sprite atlasSprite = TextureAtlas.instance.GetAtlasSprite(uITextureAtlasMemberID);
				if (atlasSprite != null)
				{
					this.m_missionLocation.set_enabled(true);
					this.m_missionLocation.set_sprite(atlasSprite);
				}
			}
		}
		this.UpdateMechanicPreview(flag, mission);
		MissionRewardDisplay[] componentsInChildren = this.m_previewLootGroup.GetComponentsInChildren<MissionRewardDisplay>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] != null)
			{
				Object.DestroyImmediate(componentsInChildren[i].get_gameObject());
			}
		}
		MissionRewardDisplay.InitMissionRewards(this.m_missionRewardDisplayPrefab.get_gameObject(), this.m_previewLootGroup.get_transform(), mission.Reward);
	}

	public void UpdateMechanicPreview(bool missionInProgress, JamGarrisonMobileMission mission)
	{
		int num = 0;
		if (!missionInProgress)
		{
			for (int i = 0; i < mission.Encounter.Length; i++)
			{
				int id = (mission.Encounter[i].MechanicID.Length <= 0) ? 0 : mission.Encounter[i].MechanicID[0];
				GarrMechanicRec record = StaticDB.garrMechanicDB.GetRecord(id);
				if (record != null && record.GarrAbilityID != 0)
				{
					this.m_previewAbilityID[num] = record.GarrAbilityID;
					this.m_previewCanCounterStatus[num] = GeneralHelpers.HasFollowerWhoCanCounter((int)record.GarrMechanicTypeID);
					num++;
				}
			}
			bool flag = true;
			AbilityDisplay[] componentsInChildren = this.m_previewMechanicsGroup.GetComponentsInChildren<AbilityDisplay>(true);
			if (num != componentsInChildren.Length)
			{
				flag = false;
			}
			if (flag)
			{
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					if (componentsInChildren[j] == null)
					{
						flag = false;
						break;
					}
					if (componentsInChildren[j].GetAbilityID() != this.m_previewAbilityID[j])
					{
						flag = false;
						break;
					}
					if (componentsInChildren[j].GetCanCounterStatus() != this.m_previewCanCounterStatus[j])
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag)
			{
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					if (componentsInChildren[k] != null)
					{
						Object.DestroyImmediate(componentsInChildren[k].get_gameObject());
					}
				}
				for (int l = 0; l < num; l++)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.m_previewMechanicEffectPrefab);
					gameObject.get_transform().SetParent(this.m_previewMechanicsGroup.get_transform(), false);
					AbilityDisplay component = gameObject.GetComponent<AbilityDisplay>();
					component.SetAbility(this.m_previewAbilityID[l], false, false, null);
					component.SetCanCounterStatus(this.m_previewCanCounterStatus[l]);
				}
			}
		}
	}

	public void OnTap()
	{
		this.PlayClickSound();
		AllPopups.instance.HideAllPopups();
		if (this.m_mission.MissionState == 1)
		{
			if (AdventureMapPanel.instance.ShowMissionResultAction != null)
			{
				AdventureMapPanel.instance.ShowMissionResultAction.Invoke(this.m_mission.MissionRecID, 0, false);
			}
			return;
		}
		if (this.m_mission.MissionState == 0)
		{
			AdventureMapPanel.instance.SelectMissionFromList(this.m_mission.MissionRecID);
			return;
		}
	}

	private void Update()
	{
		if (this.m_mission.MissionState == 1)
		{
			long num = GarrisonStatus.CurrentTime() - this.m_mission.StartTime;
			long num2 = this.m_mission.MissionDuration - num;
			num2 = ((num2 <= 0L) ? 0L : num2);
			Duration duration = new Duration((int)num2, false);
			if (num2 > 0L)
			{
				this.m_statusText.set_text(duration.DurationString + " <color=#ff0000ff>(" + StaticDB.GetString("IN_PROGRESS", null) + ")</color>");
			}
			else
			{
				this.m_statusText.set_text("<color=#00ff00ff>(" + StaticDB.GetString("TAP_TO_COMPLETE", null) + ")</color>");
			}
		}
		long num3 = GarrisonStatus.CurrentTime() - this.m_mission.OfferTime;
		long num4 = this.m_mission.OfferDuration - num3;
		num4 = ((num4 <= 0L) ? 0L : num4);
		if (num4 > 0L)
		{
			if (this.m_expirationText.get_gameObject().get_activeSelf())
			{
				this.m_missionOfferTimeRemaining.FormatDurationString((int)num4, false);
				this.m_missionOfferTimeSB.set_Length(0);
				this.m_missionOfferTimeSB.ConcatFormat("{0}", this.m_missionOfferTimeRemaining.DurationString);
				this.m_expirationText.set_text(this.m_missionOfferTimeSB.ToString());
			}
		}
		else if (this.m_mission.MissionState == 0 && this.m_mission.OfferDuration > 0L)
		{
			AdventureMapPanel.instance.SelectMissionFromList(0);
			AllPopups.instance.m_missionDialog.m_missionDetailView.get_gameObject().SetActive(false);
			Object.DestroyImmediate(base.get_gameObject());
			return;
		}
	}

	public void PlayClickSound()
	{
		Main.instance.m_UISound.Play_ButtonBlackClick();
	}

	public int GetMissionID()
	{
		return (this.m_mission != null) ? this.m_mission.MissionRecID : 0;
	}
}
