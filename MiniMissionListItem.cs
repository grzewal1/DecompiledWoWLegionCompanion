using GarbageFreeStringBuilder;
using System;
using System.Runtime.CompilerServices;
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

	public MiniMissionListItem()
	{
	}

	private void Awake()
	{
		this.m_missionName.font = GeneralHelpers.LoadFancyFont();
		this.m_missionLevel.font = GeneralHelpers.LoadStandardFont();
		this.m_missionTime.font = GeneralHelpers.LoadStandardFont();
		this.m_rareMissionLabel.font = GeneralHelpers.LoadFancyFont();
		this.m_statusText.font = GeneralHelpers.LoadStandardFont();
		this.m_rareMissionLabel.text = StaticDB.GetString("RARE", "Rare!");
		this.m_previewAbilityID = new int[3];
		this.m_previewCanCounterStatus = new FollowerCanCounterMechanic[3];
		this.m_missionOfferTimeRemaining = new Duration(0, false);
		this.m_missionOfferTimeSB = new StringBuilder(16);
	}

	public int GetMissionID()
	{
		return (this.m_mission != null ? this.m_mission.MissionRecID : 0);
	}

	private int GetUncounteredMissionDuration(JamGarrisonMobileMission mission)
	{
		if (mission == null)
		{
			return 0;
		}
		GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(mission.MissionRecID);
		if (record == null)
		{
			return 0;
		}
		float missionDuration = (float)record.MissionDuration;
		JamGarrisonEncounter[] encounter = mission.Encounter;
		for (int i = 0; i < (int)encounter.Length; i++)
		{
			int[] mechanicID = encounter[i].MechanicID;
			for (int j = 0; j < (int)mechanicID.Length; j++)
			{
				int num = mechanicID[j];
				GarrMechanicRec garrMechanicRec = StaticDB.garrMechanicDB.GetRecord(num);
				if (garrMechanicRec != null)
				{
					StaticDB.garrAbilityEffectDB.EnumRecordsByParentID(garrMechanicRec.GarrAbilityID, (GarrAbilityEffectRec garrAbilityEffectRec) => {
						if (garrAbilityEffectRec.AbilityAction == 17)
						{
							missionDuration *= garrAbilityEffectRec.ActionValueFlat;
						}
						return true;
					});
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
		if (this.m_mission.MissionState != 1)
		{
			if (this.m_mission.MissionState != 0)
			{
				return;
			}
			AdventureMapPanel.instance.SelectMissionFromList(this.m_mission.MissionRecID);
			return;
		}
		if (AdventureMapPanel.instance.ShowMissionResultAction != null)
		{
			AdventureMapPanel.instance.ShowMissionResultAction(this.m_mission.MissionRecID, 0, false);
		}
	}

	public void PlayClickSound()
	{
		Main.instance.m_UISound.Play_ButtonBlackClick();
	}

	public void SetMission(JamGarrisonMobileMission mission)
	{
		this.m_statusDarkener.gameObject.SetActive(false);
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
			this.m_missionTypeIcon.sprite = TextureAtlas.instance.GetAtlasSprite((int)garrMissionTypeRec.UiTextureAtlasMemberID);
		}
		bool flag = false;
		if (mission.MissionState == 1)
		{
			flag = true;
			this.m_statusDarkener.gameObject.SetActive(true);
			this.m_statusDarkener.color = new Color(0f, 0f, 0f, 0.3529412f);
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
		this.m_expirationText.gameObject.SetActive(flags);
		this.m_rareMissionLabel.gameObject.SetActive(flags);
		this.m_rareMissionHighlight.gameObject.SetActive(flags);
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
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
			}
		}
		MissionRewardDisplay.InitMissionRewards(this.m_missionRewardDisplayPrefab.gameObject, this.m_previewLootGroup.transform, mission.Reward);
	}

	private void Update()
	{
		if (this.m_mission.MissionState == 1)
		{
			long num = GarrisonStatus.CurrentTime() - this.m_mission.StartTime;
			long missionDuration = this.m_mission.MissionDuration - num;
			missionDuration = (missionDuration <= (long)0 ? (long)0 : missionDuration);
			Duration duration = new Duration((int)missionDuration, false);
			if (missionDuration <= (long)0)
			{
				this.m_statusText.text = string.Concat("<color=#00ff00ff>(", StaticDB.GetString("TAP_TO_COMPLETE", null), ")</color>");
			}
			else
			{
				this.m_statusText.text = string.Concat(duration.DurationString, " <color=#ff0000ff>(", StaticDB.GetString("IN_PROGRESS", null), ")</color>");
			}
		}
		long num1 = GarrisonStatus.CurrentTime() - this.m_mission.OfferTime;
		long offerDuration = this.m_mission.OfferDuration - num1;
		offerDuration = (offerDuration <= (long)0 ? (long)0 : offerDuration);
		if (offerDuration > (long)0)
		{
			if (this.m_expirationText.gameObject.activeSelf)
			{
				this.m_missionOfferTimeRemaining.FormatDurationString((int)offerDuration, false);
				this.m_missionOfferTimeSB.Length = 0;
				this.m_missionOfferTimeSB.ConcatFormat<string>("{0}", this.m_missionOfferTimeRemaining.DurationString);
				this.m_expirationText.text = this.m_missionOfferTimeSB.ToString();
			}
		}
		else if (this.m_mission.MissionState == 0 && this.m_mission.OfferDuration > (long)0)
		{
			AdventureMapPanel.instance.SelectMissionFromList(0);
			AllPopups.instance.m_missionDialog.m_missionDetailView.gameObject.SetActive(false);
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
	}

	public void UpdateMechanicPreview(bool missionInProgress, JamGarrisonMobileMission mission)
	{
		int num;
		int num1 = 0;
		if (!missionInProgress)
		{
			for (int i = 0; i < (int)mission.Encounter.Length; i++)
			{
				num = ((int)mission.Encounter[i].MechanicID.Length <= 0 ? 0 : mission.Encounter[i].MechanicID[0]);
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
						UnityEngine.Object.DestroyImmediate(componentsInChildren[j].gameObject);
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