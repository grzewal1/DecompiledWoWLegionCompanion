using GarbageFreeStringBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowStatConstants;
using WowStaticData;

public class FollowerListItem : MonoBehaviour
{
	public int m_followerID;

	public Text followerIDText;

	public Text portraitErrorText;

	public Image followerPortrait;

	public Image followerPortraitFrame;

	public Text nameText;

	public Text m_levelText;

	public Text m_statusText;

	public GameObject selectedImage;

	public Image m_portraitQualityRing;

	public Image m_portraitQualityRing_TitleQuality;

	public Image m_levelBorder;

	public Image m_levelBorder_TitleQuality;

	public Image darkeningImage;

	public Image m_classIcon;

	public GameObject usefulAbilitiesGroup;

	public GameObject m_abilityDisplayPrefab;

	public GameObject missionMechanicCounterPrefab;

	public GameObject m_troopHeartContainer;

	public GameObject m_troopHeartPrefab;

	public GameObject m_troopEmptyHeartPrefab;

	public bool m_inParty;

	private bool m_availableForMission;

	private bool m_onMission;

	public long m_missionStartedTime;

	public long m_missionDurationInSeconds;

	[Header("XP Bar")]
	public GameObject m_progressBarObj;

	public Image m_progressBarFillImage;

	public Text m_xpAmountText;

	[Header("Detail Follower List Items Only")]
	public FollowerDetailView m_followerDetailView;

	public CanvasGroup m_followerDetailViewCanvasGroup;

	public LayoutElement m_followerDetailViewLayoutElement;

	public int m_followerDetailViewExtraHeight;

	public RectTransform m_listItemArea;

	public Image m_expandArrow;

	public GameObject m_selectionGlowRoot;

	public GameObject m_useArmamentsButton;

	public Text m_useArmamentsButtonText;

	public Image m_useArmamentsButtonUpArrowGreen;

	private bool m_isCombatAlly;

	private int m_itemLevel;

	private Duration m_missionTimeRemaining;

	private StringBuilder m_statusTextSB;

	private static string m_iLvlString;

	private static string m_inactiveString;

	private static string m_onMissionString;

	private static string m_fatiguedString;

	private static string m_inBuildingString;

	private static string m_inPartyString;

	private static string m_missionCompleteString;

	private static string m_combatAllyString;

	static FollowerListItem()
	{
	}

	public FollowerListItem()
	{
	}

	private void AddToParty(bool forceReplaceFirstSlot = false)
	{
		if (this.m_availableForMission)
		{
			MissionFollowerSlotGroup componentInChildren = base.gameObject.transform.parent.parent.parent.parent.gameObject.GetComponentInChildren<MissionFollowerSlotGroup>();
			if (componentInChildren != null && componentInChildren.gameObject.activeSelf)
			{
				this.m_inParty = componentInChildren.SetFollower(this.m_followerID, this.followerPortrait.overrideSprite, this.m_portraitQualityRing.color, forceReplaceFirstSlot);
				if (!this.m_inParty)
				{
					Main.instance.m_UISound.Play_DefaultNavClick();
					this.DeselectMe();
				}
				else if ((PersistentFollowerData.followerDictionary[this.m_followerID].Flags & 8) == 0)
				{
					Main.instance.m_UISound.Play_SlotChampion();
				}
				else
				{
					Main.instance.m_UISound.Play_SlotTroop();
				}
				this.SetAvailabilityStatus(PersistentFollowerData.followerDictionary[this.m_followerID]);
			}
		}
	}

	private void Awake()
	{
		if (PersistentFollowerData.followerDictionary.Count == 0)
		{
			return;
		}
		if (!AssetBundleManager.instance.IsInitialized())
		{
			AssetBundleManager.instance.InitializedAction += new Action(this.OnAssetBundleManagerInitialized);
		}
		else
		{
			this.OnAssetBundleManagerInitialized();
		}
		this.followerIDText.font = GeneralHelpers.LoadStandardFont();
		this.portraitErrorText.font = GeneralHelpers.LoadStandardFont();
		this.nameText.font = GeneralHelpers.LoadStandardFont();
		this.m_levelText.font = GeneralHelpers.LoadStandardFont();
		this.m_statusText.font = GeneralHelpers.LoadStandardFont();
		this.m_xpAmountText.font = GeneralHelpers.LoadStandardFont();
		if (this.m_useArmamentsButtonText != null)
		{
			this.m_useArmamentsButtonText.font = GeneralHelpers.LoadStandardFont();
		}
		this.m_missionTimeRemaining = new Duration(0, false);
		this.m_statusTextSB = new StringBuilder(16);
	}

	public void ContractArrowRotationComplete()
	{
		this.m_expandArrow.transform.localEulerAngles = Vector3.zero;
	}

	public void ContractDetailViewComplete()
	{
		this.m_followerDetailViewLayoutElement.minHeight = 0f;
		this.m_followerDetailViewCanvasGroup.alpha = 0f;
		this.m_followerDetailView.SetFollower(0);
	}

	public void DeselectMe()
	{
		if (this.selectedImage != null)
		{
			this.selectedImage.SetActive(false);
		}
	}

	private void DetailFollowerListItem_ManageFollowerDetailViewSize(int garrFollowerID)
	{
		if ((garrFollowerID != this.m_followerID ? false : this.m_followerDetailViewLayoutElement.minHeight == 0f))
		{
			if (this.m_followerDetailView.GetCurrentFollower() != this.m_followerID)
			{
				this.m_followerDetailView.SetFollower(this.m_followerID);
			}
			iTween.StopByName(base.gameObject, "FollowerDetailExpand");
			iTween.StopByName(base.gameObject, "FollowerDetailExpandArrow");
			iTween.StopByName(base.gameObject, "FollowerDetailContract");
			iTween.StopByName(base.gameObject, "FollowerDetailContractArrow");
			this.SelectMe();
			bool flag = false;
			float mFollowerDetailViewLayoutElement = 0f;
			FollowerListItem[] componentsInChildren = OrderHallFollowersPanel.instance.m_followerDetailListContent.GetComponentsInChildren<FollowerListItem>(true);
			int num = 0;
			while (num < (int)componentsInChildren.Length)
			{
				FollowerListItem followerListItem = componentsInChildren[num];
				if (followerListItem == this)
				{
					break;
				}
				else if (followerListItem.m_followerDetailViewLayoutElement.minHeight <= 0f)
				{
					num++;
				}
				else
				{
					mFollowerDetailViewLayoutElement = followerListItem.m_followerDetailViewLayoutElement.minHeight;
					flag = true;
					break;
				}
			}
			RectTransform component = this.m_followerDetailView.traitsAndAbilitiesRootObject.GetComponent<RectTransform>();
			OrderHallFollowersPanel orderHallFollowersPanel = OrderHallFollowersPanel.instance;
			Vector3 vector3 = base.transform.localPosition;
			orderHallFollowersPanel.ScrollListTo(-vector3.y - (!flag ? 0f : mFollowerDetailViewLayoutElement) - 56f);
			bool flag1 = true;
			JamGarrisonFollower item = PersistentFollowerData.followerDictionary[this.m_followerID];
			bool flags = (item.Flags & 8) != 0;
			GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(item.GarrFollowerID);
			if (flags || item.FollowerLevel < MissionDetailView.GarrisonFollower_GetMaxFollowerLevel((int)record.GarrFollowerTypeID))
			{
				flag1 = false;
			}
			this.m_useArmamentsButton.SetActive(flag1);
			GameObject gameObject = base.gameObject;
			object[] mFollowerDetailViewExtraHeight = new object[] { "name", "FollowerDetailExpand", "from", this.m_followerDetailViewLayoutElement.minHeight, "to", null, null, null, null, null, null, null, null, null };
			Rect rect = component.rect;
			mFollowerDetailViewExtraHeight[5] = rect.height + (float)this.m_followerDetailViewExtraHeight;
			mFollowerDetailViewExtraHeight[6] = "time";
			mFollowerDetailViewExtraHeight[7] = 0.25f;
			mFollowerDetailViewExtraHeight[8] = "easetype";
			mFollowerDetailViewExtraHeight[9] = iTween.EaseType.easeOutCubic;
			mFollowerDetailViewExtraHeight[10] = "onupdate";
			mFollowerDetailViewExtraHeight[11] = "SetDetailViewHeight";
			mFollowerDetailViewExtraHeight[12] = "oncomplete";
			mFollowerDetailViewExtraHeight[13] = "ExpandDetailViewComplete";
			iTween.ValueTo(gameObject, iTween.Hash(mFollowerDetailViewExtraHeight));
			iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "name", "FollowerDetailExpandArrow", "from", 0, "to", -90f, "time", 0.25f, "easetype", iTween.EaseType.easeOutCubic, "onupdate", "SetExpandArrowRotation", "oncomplete", "ExpandArrowRotationComplete" }));
			this.m_followerDetailViewCanvasGroup.alpha = 1f;
		}
		else if (this.m_followerDetailViewLayoutElement.minHeight > 0f)
		{
			iTween.StopByName(base.gameObject, "FollowerDetailExpand");
			iTween.StopByName(base.gameObject, "FollowerDetailExpandArrow");
			iTween.StopByName(base.gameObject, "FollowerDetailContract");
			iTween.StopByName(base.gameObject, "FollowerDetailContractArrow");
			this.DeselectMe();
			iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "name", "FollowerDetailContract", "from", this.m_followerDetailViewLayoutElement.minHeight, "to", 0f, "time", 0.25f, "easetype", iTween.EaseType.easeOutCubic, "onupdate", "SetDetailViewHeight", "oncomplete", "ContractDetailViewComplete" }));
			GameObject gameObject1 = base.gameObject;
			object[] objArray = new object[14];
			objArray[0] = "name";
			objArray[1] = "FollowerDetailContractArrow";
			objArray[2] = "from";
			Vector3 mExpandArrow = this.m_expandArrow.transform.localEulerAngles;
			objArray[3] = mExpandArrow.z;
			objArray[4] = "to";
			objArray[5] = 360f;
			objArray[6] = "time";
			objArray[7] = 0.25f;
			objArray[8] = "easetype";
			objArray[9] = iTween.EaseType.easeOutCubic;
			objArray[10] = "onupdate";
			objArray[11] = "SetExpandArrowRotation";
			objArray[12] = "oncomplete";
			objArray[13] = "ContractArrowRotationComplete";
			iTween.ValueTo(gameObject1, iTween.Hash(objArray));
		}
	}

	public void ExpandArrowRotationComplete()
	{
		this.m_expandArrow.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
	}

	public void ExpandDetailViewComplete()
	{
		RectTransform component = this.m_followerDetailView.traitsAndAbilitiesRootObject.GetComponent<RectTransform>();
		LayoutElement mFollowerDetailViewLayoutElement = this.m_followerDetailViewLayoutElement;
		Rect rect = component.rect;
		mFollowerDetailViewLayoutElement.minHeight = rect.height + (float)this.m_followerDetailViewExtraHeight;
	}

	private void FollowerDataChanged()
	{
		if (this.m_followerID > 0)
		{
			if (!PersistentFollowerData.followerDictionary.ContainsKey(this.m_followerID))
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
			else
			{
				this.SetAvailabilityStatus(PersistentFollowerData.followerDictionary[this.m_followerID]);
				if (this.m_followerDetailView != null)
				{
					this.m_followerDetailView.HandleFollowerDataChanged();
				}
			}
		}
	}

	private void HandleUseArmamentResult(int result, JamGarrisonFollower oldFollower, JamGarrisonFollower newFollower)
	{
		if (result != 0)
		{
			return;
		}
		if (newFollower.GarrFollowerID != this.m_followerID)
		{
			return;
		}
		this.SetAvailabilityStatus(newFollower);
		UiAnimMgr.instance.PlayAnim("FlameGlowPulse", this.followerPortraitFrame.transform, Vector3.zero, 1.5f, 0f);
		Main.instance.m_UISound.Play_UpgradeArmament();
	}

	public void OnAssetBundleManagerInitialized()
	{
		if (FollowerListItem.m_iLvlString == null)
		{
			FollowerListItem.m_iLvlString = StaticDB.GetString("ITEM_LEVEL_ABBREVIATION", null);
		}
		if (FollowerListItem.m_inactiveString == null)
		{
			FollowerListItem.m_inactiveString = StaticDB.GetString("INACTIVE", null);
		}
		if (FollowerListItem.m_onMissionString == null)
		{
			FollowerListItem.m_onMissionString = StaticDB.GetString("ON_MISSION", null);
		}
		if (FollowerListItem.m_fatiguedString == null)
		{
			FollowerListItem.m_fatiguedString = StaticDB.GetString("FATIGUED", null);
		}
		if (FollowerListItem.m_inBuildingString == null)
		{
			FollowerListItem.m_inBuildingString = StaticDB.GetString("IN_BUILDING", null);
		}
		if (FollowerListItem.m_inPartyString == null)
		{
			FollowerListItem.m_inPartyString = StaticDB.GetString("IN_PARTY", null);
		}
		if (FollowerListItem.m_missionCompleteString == null)
		{
			FollowerListItem.m_missionCompleteString = StaticDB.GetString("MISSION_COMPLETE", null);
		}
		if (FollowerListItem.m_combatAllyString == null)
		{
			FollowerListItem.m_combatAllyString = StaticDB.GetString("COMBAT_ALLY", null);
		}
	}

	private void OnDisable()
	{
		Main.instance.FollowerDataChangedAction -= new Action(this.FollowerDataChanged);
		Main.instance.UseArmamentResultAction -= new Action<int, JamGarrisonFollower, JamGarrisonFollower>(this.HandleUseArmamentResult);
		if (AdventureMapPanel.instance != null)
		{
			AdventureMapPanel.instance.OnMissionFollowerSlotChanged -= new Action<int, bool>(this.OnMissionFollowerSlotChanged);
			AdventureMapPanel.instance.DeselectAllFollowerListItemsAction -= new Action(this.DeselectMe);
		}
		if (this.m_followerDetailView != null && OrderHallFollowersPanel.instance != null)
		{
			OrderHallFollowersPanel.instance.FollowerDetailListItemSelectedAction -= new Action<int>(this.DetailFollowerListItem_ManageFollowerDetailViewSize);
		}
	}

	private void OnEnable()
	{
		if (Main.instance == null)
		{
			return;
		}
		if (this.m_followerID > 0)
		{
			if (!PersistentFollowerData.followerDictionary.ContainsKey(this.m_followerID))
			{
				base.transform.SetParent(Main.instance.transform);
				base.gameObject.SetActive(false);
				return;
			}
			this.SetAvailabilityStatus(PersistentFollowerData.followerDictionary[this.m_followerID]);
		}
		Main.instance.FollowerDataChangedAction += new Action(this.FollowerDataChanged);
		Main.instance.UseArmamentResultAction += new Action<int, JamGarrisonFollower, JamGarrisonFollower>(this.HandleUseArmamentResult);
		if (AdventureMapPanel.instance != null)
		{
			AdventureMapPanel.instance.OnMissionFollowerSlotChanged += new Action<int, bool>(this.OnMissionFollowerSlotChanged);
			AdventureMapPanel.instance.DeselectAllFollowerListItemsAction += new Action(this.DeselectMe);
		}
		if (this.m_followerDetailView != null && OrderHallFollowersPanel.instance != null)
		{
			OrderHallFollowersPanel.instance.FollowerDetailListItemSelectedAction += new Action<int>(this.DetailFollowerListItem_ManageFollowerDetailViewSize);
		}
	}

	private void OnMissionFollowerSlotChanged(int garrFollowerID, bool inParty)
	{
		if (this.m_followerID == garrFollowerID)
		{
			this.m_inParty = inParty;
			this.FollowerDataChanged();
		}
	}

	public void OnTapFollowerDetailListItem()
	{
		Main.instance.m_UISound.Play_ButtonBlackClick();
		AllPopups.instance.SetCurrentFollowerDetailView(this.m_followerDetailView);
		AllPopups.instance.HideChampionUpgradeDialogs();
		OrderHallFollowersPanel.instance.FollowerDetailListItemSelected(this.m_followerID);
	}

	public void RemoveFromParty(int garrFollowerID)
	{
		if (this.m_followerID == garrFollowerID)
		{
			this.m_inParty = false;
			this.SetAvailabilityStatus(PersistentFollowerData.followerDictionary[this.m_followerID]);
		}
	}

	public void RemoveFromParty()
	{
		this.m_inParty = false;
		this.SetAvailabilityStatus(PersistentFollowerData.followerDictionary[this.m_followerID]);
	}

	public void SelectAndAddToParty()
	{
		AdventureMapPanel.instance.DeselectAllFollowerListItems();
		this.SelectMe();
		this.AddToParty(false);
	}

	public void SelectAndInspect()
	{
		AdventureMapPanel.instance.DeselectAllFollowerListItems();
		this.SelectMe();
		AdventureMapPanel.instance.SetFollowerToInspect(this.m_followerID);
	}

	public void SelectAndReplaceExistingCombatAlly()
	{
		AdventureMapPanel.instance.DeselectAllFollowerListItems();
		this.SelectMe();
		this.AddToParty(true);
	}

	private void SelectMe()
	{
		if (this.selectedImage != null)
		{
			this.selectedImage.SetActive(true);
		}
	}

	public void SetAvailabilityStatus(JamGarrisonFollower follower)
	{
		GarrMissionRec record;
		this.m_isCombatAlly = false;
		if (follower.CurrentMissionID != 0)
		{
			JamGarrisonMobileMission item = (JamGarrisonMobileMission)PersistentMissionData.missionDictionary[follower.CurrentMissionID];
			this.m_missionStartedTime = item.StartTime;
			this.m_missionDurationInSeconds = item.MissionDuration;
		}
		this.m_itemLevel = (follower.ItemLevelWeapon + follower.ItemLevelArmor) / 2;
		bool flags = (follower.Flags & 4) != 0;
		bool flag = (follower.Flags & 2) != 0;
		this.m_onMission = follower.CurrentMissionID != 0;
		bool currentBuildingID = follower.CurrentBuildingID != 0;
		bool flags1 = (follower.Flags & 8) != 0;
		if (!this.m_onMission)
		{
			record = null;
		}
		else
		{
			record = StaticDB.garrMissionDB.GetRecord(follower.CurrentMissionID);
		}
		GarrMissionRec garrMissionRec = record;
		if (garrMissionRec != null && (garrMissionRec.Flags & 16) != 0)
		{
			this.m_isCombatAlly = true;
		}
		this.darkeningImage.gameObject.SetActive(true);
		this.darkeningImage.color = new Color(0f, 0f, 0.28f, 0.3f);
		this.m_statusText.color = Color.white;
		this.m_troopHeartContainer.SetActive(false);
		if (flags)
		{
			this.m_statusText.color = Color.red;
			if (follower.FollowerLevel != 110)
			{
				this.m_statusText.text = FollowerListItem.m_inactiveString;
			}
			else
			{
				this.m_statusText.text = string.Concat(new object[] { FollowerListItem.m_iLvlString, " ", this.m_itemLevel, " - ", FollowerListItem.m_inactiveString });
			}
			this.darkeningImage.color = new Color(0.28f, 0f, 0f, 0.196f);
		}
		else if (flag)
		{
			this.m_statusText.text = string.Concat(new object[] { FollowerListItem.m_iLvlString, " ", this.m_itemLevel, " - ", FollowerListItem.m_fatiguedString });
		}
		else if (this.m_isCombatAlly)
		{
			this.m_statusText.text = string.Concat(new object[] { FollowerListItem.m_iLvlString, " ", this.m_itemLevel, " - ", FollowerListItem.m_combatAllyString });
		}
		else if (this.m_onMission)
		{
			this.m_statusText.text = string.Concat(new object[] { FollowerListItem.m_iLvlString, " ", this.m_itemLevel, " - ", FollowerListItem.m_onMissionString });
		}
		else if (currentBuildingID)
		{
			this.m_statusText.text = string.Concat(new object[] { FollowerListItem.m_iLvlString, " ", this.m_itemLevel, " - ", FollowerListItem.m_inBuildingString });
		}
		else if (!this.m_inParty)
		{
			if (flags1 || follower.FollowerLevel != 110)
			{
				this.m_statusText.text = string.Empty;
			}
			else
			{
				this.m_statusText.text = string.Concat(FollowerListItem.m_iLvlString, " ", this.m_itemLevel);
			}
			this.darkeningImage.gameObject.SetActive(false);
			this.m_troopHeartContainer.SetActive(true);
		}
		else
		{
			this.m_statusText.text = FollowerListItem.m_inPartyString;
			this.darkeningImage.color = new Color(0.1f, 0.6f, 0.1f, 0.3f);
		}
		if (this.m_useArmamentsButtonText != null)
		{
			this.m_useArmamentsButtonText.text = string.Concat(FollowerListItem.m_iLvlString, " ", this.m_itemLevel);
		}
		this.m_availableForMission = (flags || flag || this.m_onMission ? 1 : (int)currentBuildingID) == 0;
	}

	public void SetDetailViewHeight(float value)
	{
		this.m_followerDetailViewLayoutElement.minHeight = value;
	}

	public void SetExpandArrowRotation(float zrot)
	{
		Vector3 mExpandArrow = this.m_expandArrow.transform.localEulerAngles;
		mExpandArrow.z = zrot;
		this.m_expandArrow.transform.localEulerAngles = mExpandArrow;
	}

	public void SetFollower(JamGarrisonFollower follower)
	{
		uint num;
		bool flag;
		bool flag1;
		this.m_followerID = follower.GarrFollowerID;
		this.followerIDText.text = string.Concat(new object[] { "ID:", follower.GarrFollowerID, " Q:", follower.Quality });
		this.m_inParty = false;
		this.SetAvailabilityStatus(follower);
		GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(follower.GarrFollowerID);
		if (record == null)
		{
			return;
		}
		if (record.GarrFollowerTypeID != 4)
		{
			return;
		}
		if (follower.Quality == 6 && record.TitleName != null && record.TitleName.Length > 0)
		{
			this.nameText.text = record.TitleName;
		}
		else if (record != null)
		{
			CreatureRec creatureRec = StaticDB.creatureDB.GetRecord((GarrisonStatus.Faction() != PVP_FACTION.HORDE ? record.AllianceCreatureID : record.HordeCreatureID));
			this.nameText.text = creatureRec.Name;
		}
		this.m_levelText.text = string.Concat(string.Empty, follower.FollowerLevel);
		int num1 = (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? record.AllianceIconFileDataID : record.HordeIconFileDataID);
		Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.PortraitIcons, num1);
		if (sprite == null)
		{
			this.portraitErrorText.text = string.Concat(string.Empty, num1);
			this.portraitErrorText.gameObject.SetActive(true);
		}
		else
		{
			this.followerPortrait.sprite = sprite;
			this.portraitErrorText.gameObject.SetActive(false);
		}
		GarrClassSpecRec garrClassSpecRec = StaticDB.garrClassSpecDB.GetRecord((GarrisonStatus.Faction() != PVP_FACTION.HORDE ? (int)record.AllianceGarrClassSpecID : (int)record.HordeGarrClassSpecID));
		Sprite atlasSprite = TextureAtlas.instance.GetAtlasSprite((int)garrClassSpecRec.UiTextureAtlasMemberID);
		if (atlasSprite != null)
		{
			this.m_classIcon.sprite = atlasSprite;
		}
		Transform[] componentsInChildren = this.m_troopHeartContainer.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			Transform transforms = componentsInChildren[i];
			if (transforms != this.m_troopHeartContainer.transform)
			{
				UnityEngine.Object.DestroyImmediate(transforms.gameObject);
			}
		}
		if ((follower.Flags & 8) == 0)
		{
			this.m_levelText.gameObject.SetActive(true);
			if (follower.Quality != 6)
			{
				this.m_portraitQualityRing.gameObject.SetActive(true);
				this.m_portraitQualityRing_TitleQuality.gameObject.SetActive(false);
				this.m_levelBorder.gameObject.SetActive(true);
				this.m_levelBorder_TitleQuality.gameObject.SetActive(false);
			}
			else
			{
				this.m_portraitQualityRing.gameObject.SetActive(false);
				this.m_portraitQualityRing_TitleQuality.gameObject.SetActive(true);
				this.m_levelBorder.gameObject.SetActive(false);
				this.m_levelBorder_TitleQuality.gameObject.SetActive(true);
			}
			Color qualityColor = GeneralHelpers.GetQualityColor(follower.Quality);
			this.m_portraitQualityRing.color = qualityColor;
			this.m_levelBorder.color = qualityColor;
			if (follower.Quality > 1)
			{
				this.nameText.color = qualityColor;
			}
			else
			{
				this.nameText.color = Color.white;
			}
			GeneralHelpers.GetXpCapInfo(follower.FollowerLevel, follower.Quality, out num, out flag, out flag1);
			if (!flag1)
			{
				this.m_progressBarObj.SetActive(true);
				float single = Mathf.Clamp01((float)follower.Xp / (float)((float)num));
				this.m_progressBarFillImage.fillAmount = single;
				this.m_xpAmountText.text = string.Concat(new object[] { string.Empty, follower.Xp, "/", num });
			}
			else
			{
				this.m_progressBarObj.SetActive(false);
			}
		}
		else
		{
			this.m_portraitQualityRing.color = Color.white;
			this.m_levelBorder.color = Color.white;
			this.nameText.color = Color.white;
			int j = 0;
			for (j = 0; j < follower.Durability; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_troopHeartPrefab);
				gameObject.transform.SetParent(this.m_troopHeartContainer.transform, false);
			}
			for (int k = j; k < record.Vitality; k++)
			{
				GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_troopEmptyHeartPrefab);
				gameObject1.transform.SetParent(this.m_troopHeartContainer.transform, false);
			}
			this.m_portraitQualityRing.gameObject.SetActive(false);
			this.m_portraitQualityRing_TitleQuality.gameObject.SetActive(false);
			this.m_levelBorder.gameObject.SetActive(false);
			this.m_levelBorder_TitleQuality.gameObject.SetActive(false);
			this.followerPortraitFrame.enabled = false;
			this.m_progressBarObj.SetActive(false);
			this.m_levelText.gameObject.SetActive(false);
		}
	}

	public void ShowArmamentDialog()
	{
		if (AllPopups.instance.GetCurrentFollowerDetailView() != this.m_followerDetailView)
		{
			return;
		}
		AllPopups.instance.ShowArmamentDialog(this.m_followerDetailView, true);
	}

	private void Update()
	{
		if (!this.m_onMission)
		{
			return;
		}
		if (this.m_isCombatAlly)
		{
			return;
		}
		long num = GarrisonStatus.CurrentTime() - this.m_missionStartedTime;
		long mMissionDurationInSeconds = this.m_missionDurationInSeconds - num;
		mMissionDurationInSeconds = (mMissionDurationInSeconds <= (long)0 ? (long)0 : mMissionDurationInSeconds);
		if (mMissionDurationInSeconds <= (long)0)
		{
			this.m_statusTextSB.Length = 0;
			this.m_statusTextSB.ConcatFormat<string, int, string, string>("{0} {1} - {2} - {3}", FollowerListItem.m_iLvlString, this.m_itemLevel, FollowerListItem.m_onMissionString, FollowerListItem.m_missionCompleteString);
			this.m_statusText.text = this.m_statusTextSB.ToString();
		}
		else
		{
			this.m_missionTimeRemaining.FormatDurationString((int)mMissionDurationInSeconds, false);
			this.m_statusTextSB.Length = 0;
			this.m_statusTextSB.ConcatFormat<string, int, string, string>("{0} {1} - {2} - {3}", FollowerListItem.m_iLvlString, this.m_itemLevel, FollowerListItem.m_onMissionString, this.m_missionTimeRemaining.DurationString);
			this.m_statusText.text = this.m_statusTextSB.ToString();
		}
	}

	public void UpdateUsefulAbilitiesDisplay(int currentGarrMissionID)
	{
		if (!PersistentFollowerData.followerDictionary.ContainsKey(this.m_followerID))
		{
			return;
		}
		AbilityDisplay[] componentsInChildren = this.usefulAbilitiesGroup.GetComponentsInChildren<AbilityDisplay>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
		List<int> nums = new List<int>();
		JamGarrisonMobileMission item = (JamGarrisonMobileMission)PersistentMissionData.missionDictionary[currentGarrMissionID];
		for (int j = 0; j < (int)item.Encounter.Length; j++)
		{
			int num = ((int)item.Encounter[j].MechanicID.Length <= 0 ? 0 : item.Encounter[j].MechanicID[0]);
			GarrMechanicRec record = StaticDB.garrMechanicDB.GetRecord(num);
			if (record != null)
			{
				int abilityToCounterMechanicType = MissionMechanic.GetAbilityToCounterMechanicType((int)record.GarrMechanicTypeID);
				if (!nums.Contains(abilityToCounterMechanicType))
				{
					nums.Add(abilityToCounterMechanicType);
				}
			}
			else
			{
				Debug.LogWarning(string.Concat(new object[] { "INVALID garrMechanic ID ", num, " in mission ", item.MissionRecID }));
			}
		}
		List<int> usefulBuffAbilitiesForFollower = MissionMechanic.GetUsefulBuffAbilitiesForFollower(this.m_followerID);
		List<int> list = nums.Union<int>(usefulBuffAbilitiesForFollower).ToList<int>();
		int[] abilityID = PersistentFollowerData.followerDictionary[this.m_followerID].AbilityID;
		for (int k = 0; k < (int)abilityID.Length; k++)
		{
			int num1 = abilityID[k];
			foreach (int num2 in list)
			{
				if (num1 != num2)
				{
					continue;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_abilityDisplayPrefab);
				gameObject.transform.SetParent(this.usefulAbilitiesGroup.transform, false);
				AbilityDisplay component = gameObject.GetComponent<AbilityDisplay>();
				component.SetAbility(num1, true, false, null);
				component.m_abilityNameText.gameObject.SetActive(false);
			}
		}
	}
}