using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowStatConstants;
using WowStaticData;

public class AdventureMapMissionSite : MonoBehaviour
{
	public Image m_errorImage;

	public CanvasGroup m_availableMissionGroup;

	public CanvasGroup m_inProgressMissionGroup;

	public CanvasGroup m_completeMissionGroup;

	public Image m_availableMissionTypeIcon;

	public Image m_inProgressMissionTypeIcon;

	public Text m_missingMissionTypeIconErrorText;

	public Text m_missionLevelText;

	public Text m_missionTimeRemainingText;

	public Image m_followerPortraitRingImage;

	public Image m_followerPortraitImage;

	public CanvasGroup m_missionSiteGroup;

	public RectTransform m_myRT;

	public int m_areaID;

	public Transform m_selectedEffectRoot;

	public Image m_selectionRing;

	public RectTransform m_zoomScaleRoot;

	public bool m_isStackablePreview;

	public GameObject[] m_stuffToHideInPreviewMode;

	private int m_garrMissionID;

	private int m_missionDurationInSeconds;

	private long m_missionStartedTime;

	private bool m_claimedMyLoot;

	private bool m_showedMyLoot;

	private bool m_isSupportMission;

	private bool m_autoCompletedSupportMission;

	public Text m_missionCompleteText;

	private UiAnimMgr.UiAnimHandle m_selectedEffectAnimHandle;

	private Duration m_missionTimeRemaining;

	public AdventureMapMissionSite()
	{
	}

	private void Awake()
	{
		this.m_selectionRing.gameObject.SetActive(false);
		this.m_missionCompleteText.text = StaticDB.GetString("MISSION_COMPLETE", null);
		this.m_isStackablePreview = false;
		this.m_missionTimeRemaining = new Duration(0, false);
	}

	public int GetGarrMissionID()
	{
		return this.m_garrMissionID;
	}

	public void HandleClaimMissionBonusResult(int garrMissionID, bool awardOvermax, int result)
	{
		if (garrMissionID == this.m_garrMissionID)
		{
			if (result != 0)
			{
				Debug.LogWarning(string.Concat("CLAIM MISSION FAILED! Result = ", (GARRISON_RESULT)result));
			}
			else
			{
				this.OnMissionStatusChanged(awardOvermax, true);
			}
		}
	}

	public void HandleCompleteMissionResult(int garrMissionID, bool missionSucceeded)
	{
		if (garrMissionID == this.m_garrMissionID)
		{
			this.OnMissionStatusChanged(false, missionSucceeded);
		}
	}

	public void HandleMissionChanged(int newMissionID)
	{
		if (this.m_isStackablePreview || this.m_garrMissionID == 0)
		{
			return;
		}
		if (this.m_selectedEffectAnimHandle != null)
		{
			UiAnimation anim = this.m_selectedEffectAnimHandle.GetAnim();
			if (anim != null)
			{
				anim.Stop(0.5f);
			}
		}
		if (newMissionID == this.m_garrMissionID)
		{
			this.m_selectedEffectAnimHandle = UiAnimMgr.instance.PlayAnim("MinimapLoopPulseAnim", this.m_selectedEffectRoot, Vector3.zero, 2.5f, 0f);
		}
		if (this.m_selectionRing != null)
		{
			this.m_selectionRing.gameObject.SetActive(newMissionID == this.m_garrMissionID);
		}
	}

	private void HandleZoomChanged(bool force)
	{
		this.m_zoomScaleRoot.sizeDelta = this.m_myRT.sizeDelta * AdventureMapPanel.instance.m_pinchZoomContentManager.m_zoomFactor;
	}

	public void JustZoomToMission()
	{
		Vector2 vector2;
		UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", base.transform, Vector3.zero, 3f, 0f);
		Main.instance.m_UISound.Play_SelectMission();
		if (StaticDB.garrMissionDB.GetRecord(this.m_garrMissionID) == null)
		{
			return;
		}
		AdventureMapPanel adventureMapPanel = AdventureMapPanel.instance;
		StackableMapIcon component = base.GetComponent<StackableMapIcon>();
		StackableMapIconContainer container = null;
		if (component != null)
		{
			container = component.GetContainer();
			AdventureMapPanel.instance.SetSelectedIconContainer(container);
		}
		vector2 = (container == null ? new Vector2(base.transform.position.x, base.transform.position.y) : new Vector2(container.transform.position.x, container.transform.position.y));
		adventureMapPanel.CenterAndZoom(vector2, null, true);
	}

	private void OnDisable()
	{
		AdventureMapPanel.instance.TestIconSizeChanged -= new Action<float>(this.OnTestIconSizeChanged);
		AdventureMapPanel.instance.m_pinchZoomContentManager.ZoomFactorChanged -= new Action<bool>(this.HandleZoomChanged);
		AdventureMapPanel.instance.MissionMapSelectionChangedAction -= new Action<int>(this.HandleMissionChanged);
	}

	private void OnEnable()
	{
		AdventureMapPanel.instance.TestIconSizeChanged += new Action<float>(this.OnTestIconSizeChanged);
		AdventureMapPanel.instance.m_pinchZoomContentManager.ZoomFactorChanged += new Action<bool>(this.HandleZoomChanged);
		AdventureMapPanel.instance.MissionMapSelectionChangedAction += new Action<int>(this.HandleMissionChanged);
	}

	public void OnMissionStatusChanged(bool awardOvermax, bool missionSucceeded)
	{
		JamGarrisonMobileMission item = (JamGarrisonMobileMission)PersistentMissionData.missionDictionary[this.m_garrMissionID];
		if (item.MissionState == 6 && !missionSucceeded)
		{
			Debug.Log(string.Concat("OnMissionStatusChanged() MISSION FAILED ", this.m_garrMissionID));
			this.m_claimedMyLoot = true;
			this.ShowMissionFailure();
			return;
		}
		if (this.m_claimedMyLoot)
		{
			if (!this.m_showedMyLoot)
			{
				this.ShowMissionSuccess(awardOvermax);
				this.m_showedMyLoot = true;
			}
			return;
		}
		if (item.MissionState == 2 || item.MissionState == 3)
		{
			Main.instance.ClaimMissionBonus(this.m_garrMissionID);
			this.m_claimedMyLoot = true;
		}
	}

	public void OnTapAvailableMission()
	{
		AdventureMapPanel.instance.SelectMissionFromMap(this.m_garrMissionID);
		this.JustZoomToMission();
	}

	public void OnTapCompletedMission()
	{
		UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", base.transform, Vector3.zero, 3f, 0f);
		Main.instance.m_UISound.Play_SelectMission();
		if (AdventureMapPanel.instance.ShowMissionResultAction != null)
		{
			AdventureMapPanel.instance.ShowMissionResultAction(this.m_garrMissionID, 1, false);
		}
		Main.instance.CompleteMission(this.m_garrMissionID);
	}

	private void OnTestIconSizeChanged(float newScale)
	{
		base.transform.localScale = Vector3.one * newScale;
	}

	public void SetMission(int garrMissionID)
	{
		base.gameObject.name = string.Concat("AdvMapMissionSite ", garrMissionID);
		if (!PersistentMissionData.missionDictionary.ContainsKey(garrMissionID))
		{
			return;
		}
		this.m_garrMissionID = garrMissionID;
		GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(garrMissionID);
		if (record == null || !PersistentMissionData.missionDictionary.ContainsKey(garrMissionID))
		{
			return;
		}
		this.m_areaID = record.AreaID;
		this.m_isSupportMission = false;
		if ((record.Flags & 16) != 0)
		{
			this.m_isSupportMission = true;
			this.m_missionTimeRemainingText.text = "Fortified";
		}
		GarrMissionTypeRec garrMissionTypeRec = StaticDB.garrMissionTypeDB.GetRecord((int)record.GarrMissionTypeID);
		if (garrMissionTypeRec.UiTextureAtlasMemberID > 0)
		{
			Sprite atlasSprite = TextureAtlas.instance.GetAtlasSprite((int)garrMissionTypeRec.UiTextureAtlasMemberID);
			if (atlasSprite != null)
			{
				this.m_availableMissionTypeIcon.sprite = atlasSprite;
				this.m_inProgressMissionTypeIcon.sprite = atlasSprite;
			}
		}
		JamGarrisonMobileMission item = (JamGarrisonMobileMission)PersistentMissionData.missionDictionary[garrMissionID];
		if (item.MissionState == 1 || item.MissionState == 2)
		{
			this.m_missionDurationInSeconds = (int)item.MissionDuration;
		}
		else
		{
			this.m_missionDurationInSeconds = record.MissionDuration;
		}
		this.m_missionStartedTime = item.StartTime;
		this.m_availableMissionGroup.gameObject.SetActive(item.MissionState == 0);
		this.m_inProgressMissionGroup.gameObject.SetActive(item.MissionState == 1);
		this.m_completeMissionGroup.gameObject.SetActive((item.MissionState == 2 ? true : item.MissionState == 3));
		if (item.MissionState == 1)
		{
			foreach (KeyValuePair<int, JamGarrisonFollower> keyValuePair in PersistentFollowerData.followerDictionary)
			{
				if (keyValuePair.Value.CurrentMissionID != garrMissionID)
				{
					continue;
				}
				GarrFollowerRec garrFollowerRec = StaticDB.garrFollowerDB.GetRecord(keyValuePair.Value.GarrFollowerID);
				if (garrFollowerRec != null)
				{
					Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.PortraitIcons, (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? garrFollowerRec.AllianceIconFileDataID : garrFollowerRec.HordeIconFileDataID));
					if (sprite != null)
					{
						this.m_followerPortraitImage.sprite = sprite;
					}
					this.m_followerPortraitRingImage.GetComponent<Image>().enabled = true;
					break;
				}
			}
		}
		this.m_missionLevelText.text = string.Concat(string.Empty, record.TargetLevel, (record.TargetLevel != 110 ? string.Empty : string.Concat(" (", record.TargetItemLevel, ")")));
		this.UpdateMissionRemainingTimeDisplay();
	}

	public void SetPreviewMode(bool isPreview)
	{
		this.m_isStackablePreview = isPreview;
		GameObject[] mStuffToHideInPreviewMode = this.m_stuffToHideInPreviewMode;
		for (int i = 0; i < (int)mStuffToHideInPreviewMode.Length; i++)
		{
			mStuffToHideInPreviewMode[i].SetActive(!isPreview);
		}
	}

	public bool ShouldShowCompletedMission()
	{
		JamGarrisonMobileMission item = (JamGarrisonMobileMission)PersistentMissionData.missionDictionary[this.m_garrMissionID];
		if (item.MissionState == 2 || item.MissionState == 3)
		{
			return true;
		}
		if (item.MissionState == 1 && GarrisonStatus.CurrentTime() - this.m_missionStartedTime >= (long)this.m_missionDurationInSeconds)
		{
			return true;
		}
		return false;
	}

	public void ShowInProgressMissionDetails()
	{
		Main.instance.m_UISound.Play_SelectWorldQuest();
		if (AdventureMapPanel.instance.ShowMissionResultAction != null)
		{
			AdventureMapPanel.instance.ShowMissionResultAction(this.m_garrMissionID, 0, false);
		}
	}

	private void ShowMissionFailure()
	{
		if (AdventureMapPanel.instance.ShowMissionResultAction != null)
		{
			AdventureMapPanel.instance.ShowMissionResultAction(this.m_garrMissionID, 3, false);
		}
		StackableMapIcon component = base.gameObject.GetComponent<StackableMapIcon>();
		GameObject gameObject = base.gameObject;
		if (component != null)
		{
			component.RemoveFromContainer();
		}
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	private void ShowMissionSuccess(bool awardOvermax)
	{
		if (AdventureMapPanel.instance.ShowMissionResultAction != null)
		{
			AdventureMapPanel.instance.ShowMissionResultAction(this.m_garrMissionID, 2, awardOvermax);
		}
		StackableMapIcon component = base.gameObject.GetComponent<StackableMapIcon>();
		GameObject gameObject = base.gameObject;
		if (component != null)
		{
			component.RemoveFromContainer();
		}
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	private void Update()
	{
		this.UpdateMissionRemainingTimeDisplay();
	}

	private void UpdateMissionRemainingTimeDisplay()
	{
		if (!this.m_inProgressMissionGroup.gameObject.activeSelf)
		{
			return;
		}
		if (this.m_missionSiteGroup != null && this.m_missionSiteGroup.alpha < 0.1f)
		{
			return;
		}
		long num = GarrisonStatus.CurrentTime() - this.m_missionStartedTime;
		long mMissionDurationInSeconds = (long)this.m_missionDurationInSeconds - num;
		mMissionDurationInSeconds = (mMissionDurationInSeconds <= (long)0 ? (long)0 : mMissionDurationInSeconds);
		if (!this.m_isSupportMission)
		{
			this.m_missionTimeRemaining.FormatDurationString((int)mMissionDurationInSeconds, false);
			this.m_missionTimeRemainingText.text = this.m_missionTimeRemaining.DurationString;
		}
		if (mMissionDurationInSeconds == 0)
		{
			if (!this.m_isSupportMission)
			{
				this.m_availableMissionGroup.gameObject.SetActive(false);
				this.m_inProgressMissionGroup.gameObject.SetActive(false);
				this.m_completeMissionGroup.gameObject.SetActive(true);
			}
			else if (!this.m_autoCompletedSupportMission)
			{
				if (AdventureMapPanel.instance.ShowMissionResultAction != null)
				{
					AdventureMapPanel.instance.ShowMissionResultAction(this.m_garrMissionID, 1, false);
				}
				Main.instance.CompleteMission(this.m_garrMissionID);
				this.m_autoCompletedSupportMission = true;
			}
		}
	}
}