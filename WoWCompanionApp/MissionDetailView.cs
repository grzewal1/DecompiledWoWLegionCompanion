using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class MissionDetailView : MonoBehaviour
	{
		[Header("Main Mission Display")]
		public Text missionNameText;

		public Text missionLocationText;

		public Image missionTypeImage;

		public Text missioniLevelText;

		public Text missionTypeText;

		public Text missionDescriptionText;

		public GameObject missionFollowerSlotGroup;

		public GameObject enemyPortraitsGroup;

		public GameObject missionFollowerSlotPrefab;

		public GameObject missionEncounterPrefab;

		public GameObject missionMechanicPrefab;

		public MissionMechanic missionEnvironmentMechanic;

		public GameObject treasureChestHorde;

		public GameObject treasureChestAlliance;

		public Text missionPercentChanceLabel;

		public Text missionPercentChanceText;

		public GameObject m_missionChanceSpinner;

		public Text missionCostText;

		public Image m_scrollingEnvironment_Back;

		public Image m_scrollingEnvironment_Mid;

		public Image m_scrollingEnvironment_Fore;

		public GameObject m_lootGroupObj;

		public MissionRewardDisplay m_missionRewardDisplayPrefab;

		public GameObject m_mechanicEffectDisplayPrefab;

		public Text m_resourcesDescText;

		public Image m_startMissionButton;

		public Text m_startMissionButtonText;

		public Text m_costDescText;

		public GameObject m_partyBuffGroup;

		public GameObject m_partyDebuffGroup;

		public GameObject m_lootBorderNormal;

		public GameObject m_lootBorderLitUp;

		public Image m_resourceIcon_MissionCost;

		[Header("Combat Ally")]
		public bool m_isCombatAlly;

		public GameObject m_combatAllyAvailableStuff;

		public GameObject m_assignCombatAllyStuff;

		public GameObject m_unassignCombatAllyStuff;

		public GameObject m_combatAllyNotAvailableStuff;

		public Text m_combatAllyThisIsWhatYouGetText;

		public SpellDisplay m_combatAllySupportSpellDisplay;

		[Header("Bonus Loot")]
		public GameObject m_bonusLootDisplay;

		public Text m_bonusLootChanceText;

		public MissionRewardDisplay m_bonusMissionRewardDisplay;

		[Header("Mission Preview")]
		public GameObject m_previewView;

		public Text m_previewMissionNameText;

		public Text m_previewMissionLocationText;

		public Image m_previewMissionTypeImage;

		public Text m_previewMissioniLevelText;

		public Text m_previewMissionTimeText;

		public GameObject m_previewMechanicsGroup;

		public GameObject m_previewMechanicEffectPrefab;

		public GameObject m_previewLootGroup;

		public Transform m_previewSlideUpHintEffectRoot;

		[Header("Misc")]
		public CanvasGroup m_topLevelDetailViewCanvasGroup;

		public MissionPanelSlider m_missionPanelSlider;

		public Shader m_grayscaleShader;

		public bool m_usedForMissionList;

		private int m_currentGarrMissionID;

		private static string m_timeText;

		private static string m_typeText;

		private static string m_iLevelText;

		public Text m_partyBuffsText;

		public Text m_partyDebuffsText;

		private CombatAllyMissionState m_combatAllyMissionState;

		private bool m_isOverMaxChampionSoftCap;

		private bool m_needMoreResources;

		private bool m_needAtLeastOneChampion;

		private int m_percentChance;

		public Action FollowerSlotsChangedAction;

		[Header("Notched Screen")]
		public GameObject m_DarkBG;

		public GameObject m_LevelBG;

		public GameObject m_MissionTypeImage;

		public GameObject m_MissionNameText;

		public GameObject m_MissionLocationText;

		public GameObject m_CloseButton;

		[Header("Narrow Screen")]
		public GameObject m_EnemiesGroup;

		public GameObject m_FollowerSlotGroup;

		private const int HidePartyBuffTextCount = 8;

		static MissionDetailView()
		{
		}

		public MissionDetailView()
		{
		}

		public void AssignCombatAlly()
		{
			this.StartMission();
		}

		public void Awake()
		{
			this.m_percentChance = 0;
			if (!Singleton<AssetBundleManager>.instance.IsInitialized())
			{
				Singleton<AssetBundleManager>.instance.InitializedAction += new Action(this.OnAssetBundleManagerInitialized);
			}
			else
			{
				this.OnAssetBundleManagerInitialized();
			}
			if (this.m_previewView != null)
			{
				this.m_previewView.SetActive(true);
			}
			Material material = new Material(this.m_grayscaleShader);
			this.m_startMissionButton.material = material;
			this.m_startMissionButton.material.SetFloat("_GrayscaleAmount", 0f);
		}

		private static float ComputeBiasValue(float par, float max, float bias)
		{
			if (bias < 0f)
			{
				return par + bias * par;
			}
			return par + bias * (max - par);
		}

		public static float ComputeFollowerBias(WrapperGarrisonFollower follower, int followerLevel, int followerItemLevel, int garrMissionID)
		{
			if ((follower.Flags & 8) != 0)
			{
				return 0f;
			}
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(garrMissionID);
			if (record == null)
			{
				return 0f;
			}
			float single = MissionDetailView.ComputeFollowerLevelBias(follower, followerLevel, record.TargetLevel);
			single += MissionDetailView.ComputeFollowerItemLevelBias(follower, followerItemLevel, (int)record.GarrFollowerTypeID, (int)record.TargetItemLevel);
			return Mathf.Clamp(single, -1f, 1f);
		}

		private static float ComputeFollowerItemLevelBias(WrapperGarrisonFollower follower, int followerItemLevel, int garrFollowerTypeID, int targetItemLevel)
		{
			int num;
			int num1;
			float single = 0f;
			if (follower.FollowerLevel == MissionDetailView.GarrisonFollower_GetMaxFollowerLevel(garrFollowerTypeID))
			{
				MissionDetailView.GarrisonFollower_GetFollowerBiasConstants(follower, out num, out num1);
				float single1 = (float)(followerItemLevel - (targetItemLevel <= 0 ? 600 : targetItemLevel));
				single = single + single1 / (float)num1;
			}
			return single;
		}

		private static float ComputeFollowerLevelBias(WrapperGarrisonFollower follower, int followerLevel, int targetLevel)
		{
			int num;
			int num1;
			MissionDetailView.GarrisonFollower_GetFollowerBiasConstants(follower, out num, out num1);
			return (float)(followerLevel - targetLevel) / (float)num;
		}

		private static void GarrisonFollower_GetFollowerBiasConstants(WrapperGarrisonFollower follower, out int out_levelRangeBias, out int out_itemLevelRangeBias)
		{
			out_levelRangeBias = 1;
			out_itemLevelRangeBias = 1;
			GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(follower.GarrFollowerID);
			if (record == null)
			{
				return;
			}
			GarrFollowerTypeRec garrFollowerTypeRec = StaticDB.garrFollowerTypeDB.GetRecord((int)record.GarrFollowerTypeID);
			if (garrFollowerTypeRec == null)
			{
				return;
			}
			out_levelRangeBias = (garrFollowerTypeRec.LevelRangeBias >= 1 ? (int)garrFollowerTypeRec.LevelRangeBias : 1);
			out_itemLevelRangeBias = (garrFollowerTypeRec.ItemLevelRangeBias >= 1 ? (int)garrFollowerTypeRec.ItemLevelRangeBias : 1);
		}

		public static int GarrisonFollower_GetMaxFollowerLevel(int garrFollowerTypeID)
		{
			return (int)StaticDB.garrFollowerLevelXPDB.GetRecordsWhere((GarrFollowerLevelXPRec rec) => (ulong)rec.GarrFollowerTypeID == (long)garrFollowerTypeID).Max<GarrFollowerLevelXPRec, uint>((GarrFollowerLevelXPRec rec) => rec.FollowerLevel);
		}

		public CombatAllyMissionState GetCombatAllyMissionState()
		{
			return this.m_combatAllyMissionState;
		}

		public int GetCurrentMissionID()
		{
			return this.m_currentGarrMissionID;
		}

		private int GetTrueMissionCost(GarrMissionRec garrMissionRec, List<int> followerBuffAbilityIDs)
		{
			float missionCost = (float)((float)garrMissionRec.MissionCost);
			if (this.enemyPortraitsGroup != null)
			{
				MissionMechanic[] componentsInChildren = this.enemyPortraitsGroup.GetComponentsInChildren<MissionMechanic>(true);
				for (int i = 0; i < (int)componentsInChildren.Length; i++)
				{
					MissionMechanic missionMechanic = componentsInChildren[i];
					if (!missionMechanic.IsCountered())
					{
						if (missionMechanic.AbilityID() != 0)
						{
							foreach (GarrAbilityEffectRec garrAbilityEffectRec in 
								from rec in StaticDB.garrAbilityEffectDB.GetRecordsByParentID(missionMechanic.AbilityID())
								where rec.AbilityAction == 39
								select rec)
							{
								missionCost *= garrAbilityEffectRec.ActionValueFlat;
							}
						}
					}
				}
			}
			foreach (int followerBuffAbilityID in followerBuffAbilityIDs)
			{
				foreach (GarrAbilityEffectRec garrAbilityEffectRec1 in 
					from rec in StaticDB.garrAbilityEffectDB.GetRecordsByParentID(followerBuffAbilityID)
					where rec.AbilityAction == 39
					select rec)
				{
					missionCost *= garrAbilityEffectRec1.ActionValueFlat;
				}
			}
			return (int)missionCost;
		}

		public void HandleMissionSelected(int garrMissionID)
		{
			if (garrMissionID <= 0)
			{
				return;
			}
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(garrMissionID);
			if (record == null)
			{
				return;
			}
			this.m_currentGarrMissionID = garrMissionID;
			if (this.missionFollowerSlotGroup != null)
			{
				MissionFollowerSlot[] componentsInChildren = this.missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
				for (int i = 0; i < (int)componentsInChildren.Length; i++)
				{
					componentsInChildren[i].ClearFollower();
				}
				RectTransform[] rectTransformArray = this.missionFollowerSlotGroup.GetComponentsInChildren<RectTransform>(true);
				for (int j = 0; j < (int)rectTransformArray.Length; j++)
				{
					if (rectTransformArray[j] != null && rectTransformArray[j] != this.missionFollowerSlotGroup.transform)
					{
						rectTransformArray[j].gameObject.transform.SetParent(null);
						UnityEngine.Object.Destroy(rectTransformArray[j].gameObject);
					}
				}
			}
			if (this.enemyPortraitsGroup != null)
			{
				RectTransform[] componentsInChildren1 = this.enemyPortraitsGroup.GetComponentsInChildren<RectTransform>(true);
				for (int k = 0; k < (int)componentsInChildren1.Length; k++)
				{
					if (componentsInChildren1[k] != null && componentsInChildren1[k] != this.enemyPortraitsGroup.transform)
					{
						componentsInChildren1[k].gameObject.transform.SetParent(null);
						UnityEngine.Object.Destroy(componentsInChildren1[k].gameObject);
					}
				}
			}
			if (this.m_previewMechanicsGroup != null)
			{
				AbilityDisplay[] abilityDisplayArray = this.m_previewMechanicsGroup.GetComponentsInChildren<AbilityDisplay>(true);
				for (int l = 0; l < (int)abilityDisplayArray.Length; l++)
				{
					if (abilityDisplayArray[l] != null)
					{
						abilityDisplayArray[l].gameObject.transform.SetParent(null);
						UnityEngine.Object.Destroy(abilityDisplayArray[l].gameObject);
					}
				}
			}
			if (this.treasureChestHorde != null && this.treasureChestAlliance != null)
			{
				if (GarrisonStatus.Faction() != PVP_FACTION.HORDE)
				{
					this.treasureChestHorde.SetActive(false);
					this.treasureChestAlliance.SetActive(true);
				}
				else
				{
					this.treasureChestHorde.SetActive(true);
					this.treasureChestAlliance.SetActive(false);
				}
			}
			WrapperGarrisonMission item = PersistentMissionData.missionDictionary[garrMissionID];
			if (this.enemyPortraitsGroup != null)
			{
				for (int m = 0; m < item.Encounters.Count; m++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.missionEncounterPrefab);
					gameObject.transform.SetParent(this.enemyPortraitsGroup.transform, false);
					MissionEncounter component = gameObject.GetComponent<MissionEncounter>();
					int num = (item.Encounters[m].MechanicIDs.Count <= 0 ? 0 : item.Encounters[m].MechanicIDs[0]);
					component.SetEncounter(item.Encounters[m].EncounterID, num);
					if (this.m_previewMechanicsGroup != null)
					{
						GarrMechanicRec garrMechanicRec = StaticDB.garrMechanicDB.GetRecord(num);
						if (garrMechanicRec != null && garrMechanicRec.GarrAbilityID != 0)
						{
							GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_previewMechanicEffectPrefab);
							gameObject1.transform.SetParent(this.m_previewMechanicsGroup.transform, false);
							AbilityDisplay abilityDisplay = gameObject1.GetComponent<AbilityDisplay>();
							abilityDisplay.SetAbility(garrMechanicRec.GarrAbilityID, false, false, null);
							this.SetupInputForPreviewSlider(abilityDisplay.m_mainButton);
							abilityDisplay.SetCanCounterStatus(GeneralHelpers.HasFollowerWhoCanCounter((int)garrMechanicRec.GarrMechanicTypeID));
						}
					}
				}
			}
			this.missionNameText.text = record.Name;
			if (this.m_previewMissionNameText != null)
			{
				this.m_previewMissionNameText.text = record.Name;
			}
			if (this.m_previewMissionLocationText != null)
			{
				this.m_previewMissionLocationText.text = record.Location;
			}
			if (this.missionDescriptionText != null)
			{
				this.missionDescriptionText.text = record.Description;
			}
			if (this.missioniLevelText != null)
			{
				if (record.TargetLevel >= 110)
				{
					this.missioniLevelText.text = string.Concat(new object[] { string.Empty, record.TargetLevel, "\n(", record.TargetItemLevel, ")" });
				}
				else
				{
					this.missioniLevelText.text = string.Concat(string.Empty, record.TargetLevel);
				}
			}
			if (this.m_previewMissioniLevelText != null)
			{
				this.m_previewMissioniLevelText.text = string.Concat(MissionDetailView.m_iLevelText, " ", record.TargetItemLevel);
			}
			if (this.missionTypeImage != null)
			{
				GarrMissionTypeRec garrMissionTypeRec = StaticDB.garrMissionTypeDB.GetRecord((int)record.GarrMissionTypeID);
				this.missionTypeImage.overrideSprite = TextureAtlas.instance.GetAtlasSprite((int)garrMissionTypeRec.UiTextureAtlasMemberID);
				if (this.m_previewMissionTypeImage != null)
				{
					this.m_previewMissionTypeImage.overrideSprite = TextureAtlas.instance.GetAtlasSprite((int)garrMissionTypeRec.UiTextureAtlasMemberID);
				}
			}
			if (this.missionEnvironmentMechanic != null)
			{
				this.missionEnvironmentMechanic.SetMechanicType((int)record.EnvGarrMechanicTypeID, 0, true);
				if (record.EnvGarrMechanicTypeID != 0)
				{
					GarrMechanicRec record1 = StaticDB.garrMechanicDB.GetRecord((int)record.EnvGarrMechanicTypeID);
					if (record1 != null && record1.GarrAbilityID != 0)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.m_previewMechanicEffectPrefab);
						gameObject2.transform.SetParent(this.m_previewMechanicsGroup.transform, false);
						AbilityDisplay component1 = gameObject2.GetComponent<AbilityDisplay>();
						component1.SetAbility(record1.GarrAbilityID, false, false, null);
						component1.SetCanCounterStatus(GeneralHelpers.HasFollowerWhoCanCounter((int)record1.GarrMechanicTypeID));
					}
				}
			}
			if (this.missionTypeText != null)
			{
				GarrMechanicTypeRec garrMechanicTypeRec = StaticDB.garrMechanicTypeDB.GetRecord((int)record.EnvGarrMechanicTypeID);
				if (garrMechanicTypeRec == null)
				{
					this.missionTypeText.gameObject.SetActive(false);
				}
				else
				{
					this.missionTypeText.gameObject.SetActive(true);
					this.missionTypeText.text = string.Concat(new string[] { "<color=#ffff00ff>", MissionDetailView.m_typeText, ": </color><color=#ffffffff>", garrMechanicTypeRec.Name, "</color>" });
				}
			}
			Sprite sprite = GeneralHelpers.LoadCurrencyIcon(1220);
			if (sprite != null)
			{
				this.m_resourceIcon_MissionCost.sprite = sprite;
			}
			if (this.missionFollowerSlotGroup != null)
			{
				for (int n = 0; (long)n < (ulong)record.MaxFollowers; n++)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.missionFollowerSlotPrefab);
					gameObject3.transform.SetParent(this.missionFollowerSlotGroup.transform, false);
					MissionFollowerSlot missionFollowerSlot = gameObject3.GetComponent<MissionFollowerSlot>();
					missionFollowerSlot.m_missionDetailView = this;
					missionFollowerSlot.m_enemyPortraitsGroup = this.enemyPortraitsGroup;
				}
			}
			if (this.m_isCombatAlly || record.UiTextureKitID <= 0)
			{
				(record.Flags & 16) != 0;
			}
			else
			{
				UiTextureKitRec uiTextureKitRec = StaticDB.uiTextureKitDB.GetRecord((int)record.UiTextureKitID);
				this.m_scrollingEnvironment_Back.enabled = false;
				this.m_scrollingEnvironment_Mid.enabled = false;
				this.m_scrollingEnvironment_Fore.enabled = false;
				int uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID(string.Concat("_", uiTextureKitRec.KitPrefix, "-Back"));
				if (uITextureAtlasMemberID > 0)
				{
					Sprite atlasSprite = TextureAtlas.instance.GetAtlasSprite(uITextureAtlasMemberID);
					if (atlasSprite != null)
					{
						this.m_scrollingEnvironment_Back.enabled = true;
						this.m_scrollingEnvironment_Back.sprite = atlasSprite;
					}
				}
				int uITextureAtlasMemberID1 = TextureAtlas.GetUITextureAtlasMemberID(string.Concat("_", uiTextureKitRec.KitPrefix, "-Mid"));
				if (uITextureAtlasMemberID1 > 0)
				{
					Sprite atlasSprite1 = TextureAtlas.instance.GetAtlasSprite(uITextureAtlasMemberID1);
					if (atlasSprite1 != null)
					{
						this.m_scrollingEnvironment_Mid.enabled = true;
						this.m_scrollingEnvironment_Mid.sprite = atlasSprite1;
					}
				}
				int num1 = TextureAtlas.GetUITextureAtlasMemberID(string.Concat("_", uiTextureKitRec.KitPrefix, "-Fore"));
				if (num1 > 0)
				{
					Sprite sprite1 = TextureAtlas.instance.GetAtlasSprite(num1);
					if (sprite1 != null)
					{
						this.m_scrollingEnvironment_Fore.enabled = true;
						this.m_scrollingEnvironment_Fore.sprite = sprite1;
					}
				}
			}
			this.UpdateMissionStatus();
			if (this.m_lootGroupObj == null || this.m_missionRewardDisplayPrefab == null)
			{
				return;
			}
			MissionRewardDisplay[] missionRewardDisplayArray = this.m_lootGroupObj.GetComponentsInChildren<MissionRewardDisplay>(true);
			for (int o = 0; o < (int)missionRewardDisplayArray.Length; o++)
			{
				if (missionRewardDisplayArray[o] != null)
				{
					UnityEngine.Object.Destroy(missionRewardDisplayArray[o].gameObject);
				}
			}
			if (this.m_previewLootGroup != null)
			{
				MissionRewardDisplay[] missionRewardDisplayArray1 = this.m_previewLootGroup.GetComponentsInChildren<MissionRewardDisplay>(true);
				for (int p = 0; p < (int)missionRewardDisplayArray1.Length; p++)
				{
					if (missionRewardDisplayArray1[p] != null)
					{
						UnityEngine.Object.Destroy(missionRewardDisplayArray1[p].gameObject);
					}
				}
			}
			if (!PersistentMissionData.missionDictionary.ContainsKey(this.m_currentGarrMissionID))
			{
				return;
			}
			MissionRewardDisplay.InitMissionRewards(this.m_missionRewardDisplayPrefab.gameObject, this.m_lootGroupObj.transform, item.Rewards);
			if (this.m_previewLootGroup != null)
			{
				MissionRewardDisplay.InitMissionRewards(this.m_missionRewardDisplayPrefab.gameObject, this.m_previewLootGroup.transform, item.Rewards);
			}
			this.InitBonusRewardDisplay(record, this.m_percentChance);
		}

		public void HideMissionDetailView()
		{
			Main.instance.m_UISound.Play_CloseButton();
			base.GetComponentInParent<MissionDialog>().gameObject.SetActive(false);
		}

		private void InitBonusRewardDisplay(GarrMissionRec garrMissionRec, int missionSuccessChance)
		{
			this.m_bonusLootDisplay.SetActive(false);
			if (garrMissionRec == null)
			{
				return;
			}
			if (StaticDB.rewardPackDB.GetRecord(garrMissionRec.OvermaxRewardPackID) == null)
			{
				return;
			}
			if (PersistentMissionData.missionDictionary.ContainsKey(garrMissionRec.ID))
			{
				WrapperGarrisonMission item = PersistentMissionData.missionDictionary[garrMissionRec.ID];
				if (garrMissionRec.OvermaxRewardPackID > 0 && item.OvermaxRewards.Count > 0)
				{
					this.m_bonusLootDisplay.SetActive(true);
					this.m_bonusLootChanceText.text = string.Concat(new object[] { "<color=#ffff00ff>", StaticDB.GetString("BONUS", "Bonus:"), " </color>\n<color=#ff8600ff>", Math.Max(0, missionSuccessChance - 100), "%</color>" });
					if (PersistentMissionData.missionDictionary.ContainsKey(this.m_currentGarrMissionID) && item.OvermaxRewards.Count > 0)
					{
						WrapperGarrisonMissionReward wrapperGarrisonMissionReward = item.OvermaxRewards[0];
						if (wrapperGarrisonMissionReward.ItemID > 0)
						{
							this.m_bonusMissionRewardDisplay.InitReward(MissionRewardDisplay.RewardType.item, wrapperGarrisonMissionReward.ItemID, (int)wrapperGarrisonMissionReward.ItemQuantity, 0, wrapperGarrisonMissionReward.ItemFileDataID);
						}
						else if (wrapperGarrisonMissionReward.FollowerXP > 0)
						{
							this.m_bonusMissionRewardDisplay.InitReward(MissionRewardDisplay.RewardType.followerXP, 0, (int)wrapperGarrisonMissionReward.FollowerXP, 0, 0);
						}
						else if (wrapperGarrisonMissionReward.CurrencyQuantity > 0)
						{
							if (wrapperGarrisonMissionReward.CurrencyType != 0)
							{
								CurrencyTypesRec record = StaticDB.currencyTypesDB.GetRecord(wrapperGarrisonMissionReward.CurrencyType);
								int currencyQuantity = (int)((ulong)wrapperGarrisonMissionReward.CurrencyQuantity / (long)(((record.Flags & 8) == 0 ? 1 : 100)));
								this.m_bonusMissionRewardDisplay.InitReward(MissionRewardDisplay.RewardType.currency, wrapperGarrisonMissionReward.CurrencyType, currencyQuantity, 0, 0);
							}
							else
							{
								this.m_bonusMissionRewardDisplay.InitReward(MissionRewardDisplay.RewardType.gold, 0, (int)(wrapperGarrisonMissionReward.CurrencyQuantity / 10000), 0, 0);
							}
						}
					}
				}
			}
		}

		public void NarrowScreenAdjust()
		{
			GridLayoutGroup component = this.m_EnemiesGroup.GetComponent<GridLayoutGroup>();
			if (component != null)
			{
				Vector2 vector2 = component.spacing;
				vector2.x = 40f;
				component.spacing = vector2;
			}
			component = this.m_FollowerSlotGroup.GetComponent<GridLayoutGroup>();
			if (component != null)
			{
				Vector2 vector21 = component.spacing;
				vector21.x = 40f;
				component.spacing = vector21;
			}
		}

		public void NotifyFollowerSlotsChanged()
		{
			if (this.FollowerSlotsChangedAction != null)
			{
				this.FollowerSlotsChangedAction();
			}
		}

		private void OnApplicationPause(bool paused)
		{
			this.HideMissionDetailView();
		}

		public void OnAssetBundleManagerInitialized()
		{
			this.m_currentGarrMissionID = 0;
			if (MissionDetailView.m_timeText == null)
			{
				MissionDetailView.m_timeText = StaticDB.GetString("TIME", null);
			}
			if (MissionDetailView.m_typeText == null)
			{
				MissionDetailView.m_typeText = StaticDB.GetString("TYPE", null);
			}
			MissionDetailView.m_iLevelText = StaticDB.GetString("ITEM_LEVEL_ABBREVIATION", null);
		}

		private void OnDisable()
		{
			if (!this.m_isCombatAlly)
			{
				if (!this.m_usedForMissionList)
				{
					AdventureMapPanel.instance.MissionSelectedFromMapAction -= new Action<int>(this.HandleMissionSelected);
				}
				else
				{
					AdventureMapPanel.instance.MissionSelectedFromListAction -= new Action<int>(this.HandleMissionSelected);
				}
				if (Main.instance != null)
				{
					Main.instance.MissionSuccessChanceChangedAction -= new Action<int>(this.OnMissionSuccessChanceChanged);
				}
			}
			Main.instance.m_backButtonManager.PopBackAction();
		}

		private void OnEnable()
		{
			if (!this.m_isCombatAlly)
			{
				if (!this.m_usedForMissionList)
				{
					AdventureMapPanel.instance.MissionSelectedFromMapAction += new Action<int>(this.HandleMissionSelected);
				}
				else
				{
					AdventureMapPanel.instance.MissionSelectedFromListAction += new Action<int>(this.HandleMissionSelected);
				}
				if (Main.instance != null)
				{
					Main.instance.MissionSuccessChanceChangedAction += new Action<int>(this.OnMissionSuccessChanceChanged);
				}
			}
			Main.instance.m_backButtonManager.PushBackAction(BackActionType.hideMissionDialog, null);
		}

		private void OnMissionSuccessChanceChanged(int newChance)
		{
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(this.m_currentGarrMissionID);
			if (record == null)
			{
				return;
			}
			if ((record.Flags & 16) != 0)
			{
				return;
			}
			this.m_missionChanceSpinner.SetActive(false);
			this.missionPercentChanceText.text = string.Concat(newChance, "%");
			this.m_lootBorderNormal.SetActive(newChance < 100);
			this.m_lootBorderLitUp.SetActive(newChance >= 100);
			if (newChance >= 0)
			{
				this.missionPercentChanceText.color = Color.green;
				this.missionPercentChanceLabel.color = Color.green;
			}
			else
			{
				this.missionPercentChanceText.color = Color.red;
				this.missionPercentChanceLabel.color = Color.red;
			}
			if (this.m_percentChance < 100 && newChance >= 100)
			{
				Main.instance.m_UISound.Play_100Percent();
			}
			else if (this.m_percentChance >= 200 || newChance < 200)
			{
				newChance <= this.m_percentChance;
			}
			else
			{
				Main.instance.m_UISound.Play_200Percent();
			}
			this.m_bonusLootChanceText.text = string.Concat("<color=#ff9600ff>", Math.Max(0, newChance - 100), "%</color>");
			this.m_percentChance = newChance;
		}

		public void OnPartyBuffSectionTapped()
		{
			List<int> nums = new List<int>();
			MissionFollowerSlot[] componentsInChildren = this.missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
			List<WrapperGarrisonFollower> wrapperGarrisonFollowers = new List<WrapperGarrisonFollower>();
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				int currentGarrFollowerID = componentsInChildren[i].GetCurrentGarrFollowerID();
				if (PersistentFollowerData.followerDictionary.ContainsKey(currentGarrFollowerID))
				{
					wrapperGarrisonFollowers.Add(PersistentFollowerData.followerDictionary[currentGarrFollowerID]);
				}
			}
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(this.m_currentGarrMissionID);
			if (record == null)
			{
				return;
			}
			int adjustedMissionDuration = GeneralHelpers.GetAdjustedMissionDuration(record, wrapperGarrisonFollowers, this.enemyPortraitsGroup);
			MissionFollowerSlot[] missionFollowerSlotArray = componentsInChildren;
			for (int j = 0; j < (int)missionFollowerSlotArray.Length; j++)
			{
				int num = missionFollowerSlotArray[j].GetCurrentGarrFollowerID();
				if (num != 0)
				{
					int[] buffsForCurrentMission = GeneralHelpers.GetBuffsForCurrentMission(num, this.m_currentGarrMissionID, this.missionFollowerSlotGroup, adjustedMissionDuration);
					int[] numArray = buffsForCurrentMission;
					for (int k = 0; k < (int)numArray.Length; k++)
					{
						nums.Add(numArray[k]);
					}
				}
			}
			AllPopups.instance.ShowPartyBuffsPopup(nums.ToArray());
		}

		public void SetCombatAllyMissionState(CombatAllyMissionState state)
		{
			this.m_combatAllyMissionState = state;
			switch (state)
			{
				case CombatAllyMissionState.notAvailable:
				{
					Text[] componentsInChildren = this.m_combatAllyNotAvailableStuff.GetComponentsInChildren<Text>(true);
					if (componentsInChildren[0] != null)
					{
						componentsInChildren[0].text = StaticDB.GetString("COMBAT_ALLY_UNAVAILABLE", null);
					}
					this.m_combatAllyNotAvailableStuff.SetActive(true);
					this.m_combatAllyAvailableStuff.SetActive(false);
					break;
				}
				case CombatAllyMissionState.available:
				{
					this.m_combatAllyNotAvailableStuff.SetActive(false);
					this.m_combatAllyAvailableStuff.SetActive(true);
					this.m_assignCombatAllyStuff.SetActive(true);
					this.m_unassignCombatAllyStuff.SetActive(false);
					break;
				}
				case CombatAllyMissionState.inProgress:
				{
					this.m_combatAllyNotAvailableStuff.SetActive(false);
					this.m_combatAllyAvailableStuff.SetActive(true);
					int garrFollowerID = 0;
					foreach (WrapperGarrisonFollower value in PersistentFollowerData.followerDictionary.Values)
					{
						if (value.CurrentMissionID != this.m_currentGarrMissionID)
						{
							continue;
						}
						garrFollowerID = value.GarrFollowerID;
						break;
					}
					MissionFollowerSlot[] missionFollowerSlotArray = base.gameObject.GetComponentsInChildren<MissionFollowerSlot>(true);
					missionFollowerSlotArray[0].SetFollower(garrFollowerID);
					this.m_assignCombatAllyStuff.SetActive(false);
					this.m_unassignCombatAllyStuff.SetActive(true);
					break;
				}
			}
		}

		private void SetupInputForPreviewSlider(Button button)
		{
			if (this.m_missionPanelSlider == null)
			{
				return;
			}
			EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry()
			{
				eventID = EventTriggerType.BeginDrag
			};
			entry.callback.AddListener((BaseEventData eventData) => this.m_missionPanelSlider.m_sliderPanel.OnBeginDrag(eventData));
			entry.callback.AddListener((BaseEventData eventData) => this.m_missionPanelSlider.StopTheBounce());
			entry.callback.AddListener((BaseEventData eventData) => button.enabled = false);
			eventTrigger.triggers.Add(entry);
			EventTrigger.Entry entry1 = new EventTrigger.Entry()
			{
				eventID = EventTriggerType.Drag
			};
			entry1.callback.AddListener((BaseEventData eventData) => this.m_missionPanelSlider.m_sliderPanel.OnDrag(eventData));
			eventTrigger.triggers.Add(entry1);
			EventTrigger.Entry entry2 = new EventTrigger.Entry()
			{
				eventID = EventTriggerType.EndDrag
			};
			entry2.callback.AddListener((BaseEventData eventData) => this.m_missionPanelSlider.m_sliderPanel.MissionPanelSlider_HandleAutopositioning_Bottom());
			entry2.callback.AddListener((BaseEventData eventData) => button.enabled = true);
			eventTrigger.triggers.Add(entry2);
		}

		private void Start()
		{
			if (Main.instance.IsNarrowScreen())
			{
				this.NarrowScreenAdjust();
			}
		}

		public void StartMission()
		{
			if ((this.m_isOverMaxChampionSoftCap || this.m_needMoreResources ? 1 : (int)this.m_needAtLeastOneChampion) != 0)
			{
				if (this.m_isOverMaxChampionSoftCap)
				{
					AllPopups.instance.ShowGenericPopupFull(StaticDB.GetString("TOO_MANY_CHAMPIONS", null));
				}
				else if (this.m_needAtLeastOneChampion)
				{
					AllPopups.instance.ShowGenericPopupFull(StaticDB.GetString("NEED_A_CHAMPION", null));
				}
				else if (this.m_needMoreResources)
				{
					AllPopups.instance.ShowGenericPopupFull(StaticDB.GetString("NEED_MORE_RESOURCES", null));
				}
				return;
			}
			Main.instance.m_UISound.Play_StartMission();
			IEnumerable<ulong> componentsInChildren = 
				from slot in (IEnumerable<MissionFollowerSlot>)base.gameObject.GetComponentsInChildren<MissionFollowerSlot>(true)
				where PersistentFollowerData.followerDictionary.ContainsKey(slot.GetCurrentGarrFollowerID())
				select PersistentFollowerData.followerDictionary[slot.GetCurrentGarrFollowerID()].DbID;
			Main.instance.StartMission(this.m_currentGarrMissionID, componentsInChildren);
			AdventureMapPanel.instance.SelectMissionFromMap(0);
			AdventureMapPanel.instance.SelectMissionFromList(0);
			AdventureMapPanel.instance.SetSelectedIconContainer(null);
			base.GetComponentInParent<MissionDialog>().gameObject.SetActive(false);
		}

		public void UnassignCombatAlly()
		{
			Main.instance.CompleteMission(this.m_currentGarrMissionID);
			this.SetCombatAllyMissionState(CombatAllyMissionState.available);
			MissionFollowerSlot[] componentsInChildren = base.gameObject.GetComponentsInChildren<MissionFollowerSlot>(true);
			componentsInChildren[0].SetFollower(0);
		}

		public void UpdateMissionStatus()
		{
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(this.m_currentGarrMissionID);
			if (record == null)
			{
				return;
			}
			if ((record.Flags & 16) != 0)
			{
				MissionFollowerSlot componentInChildren = base.gameObject.GetComponentInChildren<MissionFollowerSlot>(true);
				if (componentInChildren == null)
				{
					return;
				}
				if (!componentInChildren.IsOccupied())
				{
					if (this.missionDescriptionText != null)
					{
						this.missionDescriptionText.gameObject.SetActive(true);
					}
					this.m_combatAllyThisIsWhatYouGetText.gameObject.SetActive(false);
					this.m_combatAllySupportSpellDisplay.gameObject.SetActive(false);
				}
				else
				{
					if (this.missionDescriptionText != null)
					{
						this.missionDescriptionText.gameObject.SetActive(false);
					}
					this.m_combatAllyThisIsWhatYouGetText.gameObject.SetActive(true);
					this.m_combatAllySupportSpellDisplay.gameObject.SetActive(true);
					int currentGarrFollowerID = componentInChildren.GetCurrentGarrFollowerID();
					int zoneSupportSpellID = PersistentFollowerData.followerDictionary[currentGarrFollowerID].ZoneSupportSpellID;
					this.m_combatAllySupportSpellDisplay.SetSpell(zoneSupportSpellID);
				}
				return;
			}
			if (this.enemyPortraitsGroup != null)
			{
				MissionMechanic[] componentsInChildren = this.enemyPortraitsGroup.GetComponentsInChildren<MissionMechanic>(true);
				if (componentsInChildren == null)
				{
					return;
				}
				AbilityDisplay[] abilityDisplayArray = this.enemyPortraitsGroup.GetComponentsInChildren<AbilityDisplay>(true);
				if (abilityDisplayArray == null)
				{
					return;
				}
				MissionMechanicTypeCounter[] missionMechanicTypeCounterArray = base.gameObject.GetComponentsInChildren<MissionMechanicTypeCounter>(true);
				if (missionMechanicTypeCounterArray == null)
				{
					return;
				}
				int num = ((int)missionMechanicTypeCounterArray.Length <= 0 ? 1 : (int)missionMechanicTypeCounterArray.Length);
				bool[] flagArray = new bool[num];
				for (int i = 0; i < num; i++)
				{
					flagArray[i] = false;
				}
				for (int j = 0; j < (int)componentsInChildren.Length; j++)
				{
					bool flag = false;
					int num1 = 0;
					while (num1 < (int)missionMechanicTypeCounterArray.Length)
					{
						missionMechanicTypeCounterArray[num1].usedIcon.gameObject.SetActive(false);
						if (missionMechanicTypeCounterArray[num1].countersMissionMechanicTypeID != componentsInChildren[j].m_missionMechanicTypeID || flagArray[num1])
						{
							num1++;
						}
						else
						{
							flag = true;
							flagArray[num1] = true;
							break;
						}
					}
					componentsInChildren[j].SetCountered(flag, false, true);
					if (j < (int)abilityDisplayArray.Length)
					{
						abilityDisplayArray[j].SetCountered(flag, true);
					}
				}
			}
			MissionFollowerSlot[] missionFollowerSlotArray = base.gameObject.GetComponentsInChildren<MissionFollowerSlot>(true);
			List<WrapperGarrisonFollower> wrapperGarrisonFollowers = new List<WrapperGarrisonFollower>();
			for (int k = 0; k < (int)missionFollowerSlotArray.Length; k++)
			{
				int currentGarrFollowerID1 = missionFollowerSlotArray[k].GetCurrentGarrFollowerID();
				if (PersistentFollowerData.followerDictionary.ContainsKey(currentGarrFollowerID1))
				{
					wrapperGarrisonFollowers.Add(PersistentFollowerData.followerDictionary[currentGarrFollowerID1]);
				}
			}
			LegionCompanionWrapper.EvaluateMission(this.m_currentGarrMissionID, (
				from f in wrapperGarrisonFollowers
				select f.GarrFollowerID).ToList<int>());
			if (this.missionPercentChanceText != null)
			{
				this.missionPercentChanceText.text = "%";
				this.m_missionChanceSpinner.SetActive(true);
			}
			if (this.m_partyBuffGroup != null)
			{
				AbilityDisplay[] componentsInChildren1 = this.m_partyBuffGroup.GetComponentsInChildren<AbilityDisplay>(true);
				for (int l = 0; l < (int)componentsInChildren1.Length; l++)
				{
					UnityEngine.Object.Destroy(componentsInChildren1[l].gameObject);
				}
			}
			if (this.m_partyDebuffGroup != null)
			{
				AbilityDisplay[] abilityDisplayArray1 = this.m_partyDebuffGroup.GetComponentsInChildren<AbilityDisplay>(true);
				for (int m = 0; m < (int)abilityDisplayArray1.Length; m++)
				{
					UnityEngine.Object.Destroy(abilityDisplayArray1[m].gameObject);
				}
			}
			int adjustedMissionDuration = GeneralHelpers.GetAdjustedMissionDuration(record, wrapperGarrisonFollowers, this.enemyPortraitsGroup);
			List<int> nums = new List<int>();
			int length = 0;
			int num2 = 0;
			int num3 = 0;
			MissionFollowerSlot[] missionFollowerSlotArray1 = this.missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
			for (int n = 0; n < (int)missionFollowerSlotArray1.Length; n++)
			{
				int currentGarrFollowerID2 = missionFollowerSlotArray1[n].GetCurrentGarrFollowerID();
				if (currentGarrFollowerID2 != 0)
				{
					int[] buffsForCurrentMission = GeneralHelpers.GetBuffsForCurrentMission(currentGarrFollowerID2, this.m_currentGarrMissionID, this.missionFollowerSlotGroup, adjustedMissionDuration);
					length += (int)buffsForCurrentMission.Length;
					int[] numArray = buffsForCurrentMission;
					for (int o = 0; o < (int)numArray.Length; o++)
					{
						int num4 = numArray[o];
						nums.Add(num4);
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_mechanicEffectDisplayPrefab);
						gameObject.transform.SetParent(this.m_partyBuffGroup.transform, false);
						gameObject.GetComponent<AbilityDisplay>().SetAbility(num4, false, false, null);
					}
					if (PersistentFollowerData.followerDictionary.ContainsKey(currentGarrFollowerID2) && (PersistentFollowerData.followerDictionary[currentGarrFollowerID2].Flags & 8) == 0)
					{
						num3++;
					}
				}
			}
			if (length <= 8)
			{
				this.m_partyBuffsText.text = StaticDB.GetString("PARTY_BUFFS", null);
			}
			else
			{
				this.m_partyBuffsText.text = string.Empty;
			}
			if (this.m_partyBuffGroup != null)
			{
				HorizontalLayoutGroup component = this.m_partyBuffGroup.GetComponent<HorizontalLayoutGroup>();
				if (component != null)
				{
					if (length <= 10 || !Main.instance.IsNarrowScreen())
					{
						component.spacing = 6f;
					}
					else
					{
						component.spacing = 3f;
					}
				}
				this.m_partyBuffGroup.SetActive(length > 0);
			}
			if (this.m_partyDebuffGroup != null)
			{
				this.m_partyDebuffGroup.SetActive(num2 > 0);
			}
			int trueMissionCost = this.GetTrueMissionCost(record, nums);
			Text text = this.missionCostText;
			int num5 = GarrisonStatus.Resources();
			text.text = string.Concat(num5.ToString("N0"), " / ", trueMissionCost.ToString("N0"));
			int numActiveChampions = GeneralHelpers.GetNumActiveChampions();
			int maxActiveFollowers = GarrisonStatus.GetMaxActiveFollowers();
			this.m_isOverMaxChampionSoftCap = false;
			this.m_needMoreResources = false;
			this.m_needAtLeastOneChampion = false;
			if (numActiveChampions > maxActiveFollowers)
			{
				this.m_isOverMaxChampionSoftCap = true;
			}
			if (GarrisonStatus.Resources() < trueMissionCost)
			{
				this.m_needMoreResources = true;
			}
			if (num3 < 1)
			{
				this.m_needAtLeastOneChampion = true;
			}
			if (!this.m_needMoreResources)
			{
				this.missionCostText.color = Color.white;
			}
			else
			{
				this.missionCostText.color = Color.red;
			}
			if ((this.m_isOverMaxChampionSoftCap || this.m_needMoreResources ? 1 : (int)this.m_needAtLeastOneChampion) != 0)
			{
				this.m_startMissionButton.material.SetFloat("_GrayscaleAmount", 1f);
				this.m_startMissionButtonText.color = Color.gray;
				this.m_startMissionButtonText.GetComponent<Shadow>().enabled = false;
			}
			else
			{
				this.m_startMissionButton.material.SetFloat("_GrayscaleAmount", 0f);
				this.m_startMissionButtonText.color = new Color(1f, 0.8588f, 0f, 1f);
				this.m_startMissionButtonText.GetComponent<Shadow>().enabled = true;
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)adjustedMissionDuration);
			if (this.missionLocationText != null)
			{
				this.missionLocationText.text = string.Concat(new object[] { StaticDB.GetString("XP", "XP:"), " ", record.BaseFollowerXP, " (<color=#ff8600ff>", timeSpan.GetDurationString(false), "</color>)" });
			}
			if (this.m_previewMissionTimeText != null)
			{
				this.m_previewMissionTimeText.text = string.Concat(new string[] { "Hoopy <color=#ffff00ff>", MissionDetailView.m_timeText, ": </color><color=#ff8600ff>", timeSpan.GetDurationString(false), "</color>" });
			}
		}
	}
}