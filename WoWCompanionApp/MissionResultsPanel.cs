using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class MissionResultsPanel : MonoBehaviour
	{
		public GameObject m_darknessBG;

		public GameObject m_popupView;

		public GameObject missionFollowerSlotGroup;

		public GameObject enemyPortraitsGroup;

		public GameObject treasureChestHorde;

		public GameObject treasureChestAlliance;

		public GameObject missionEncounterPrefab;

		public Text missionNameText;

		public Text missionLocationText;

		public Text missioniLevelText;

		public Image missionTypeImage;

		public GameObject missionFollowerSlotPrefab;

		public Image m_scrollingEnvironment_Back;

		public Image m_scrollingEnvironment_Mid;

		public Image m_scrollingEnvironment_Fore;

		public GameObject m_lootGroupObj;

		public MissionRewardDisplay m_missionRewardDisplayPrefab;

		public GameObject m_mechanicEffectDisplayPrefab;

		public Text missionPercentChanceText;

		public GameObject m_missionChanceSpinner;

		public GameObject m_partyBuffGroup;

		public Text m_partyBuffsText;

		public GameObject m_lootBorderNormal;

		public GameObject m_lootBorderLitUp;

		public GameObject m_missionSuccessMessage;

		public GameObject m_missionFailMessage;

		public GameObject m_missionInProgressMessage;

		public Text m_missionTimeRemainingText;

		public float m_messageTimeToDelayEntrance;

		public float m_messageFadeInTime;

		public bool m_messagePunchScale;

		public float m_messagePunchScaleAmount;

		public float m_messagePunchScaleDuration;

		public float m_lootEffectInitialDelay;

		public float m_lootEffectDelay;

		public Text m_okButtonText;

		public Text m_inProgressText;

		public Text m_successText;

		public Text m_failureText;

		[Header("Bonus Loot")]
		public GameObject m_bonusLootDisplay;

		private int m_bonusLootChance;

		public Text m_bonusLootChanceText;

		public MissionRewardDisplay m_bonusMissionRewardDisplay;

		private Vector3 m_bonusLootInitialLocalPosition;

		public float m_bonusLootShakeInitialDelay;

		public float m_bonusLootShakeDuration;

		public float m_bonusLootShakeAmountX;

		public float m_bonusLootShakeAmountY;

		[Header("XP Display")]
		public GameObject m_followerExperienceDisplayArea;

		public GameObject m_followerExperienceDisplayPrefab;

		public float m_missionDetailsFadeOutDelay;

		public float m_experienceDisplayInitialEntranceDelay;

		public float m_experienceDisplayEntranceDelay;

		public AutoFadeOut m_missionResultsDisplayCanvasGroupAutoFadeOut;

		private MissionResultType m_currentResultType;

		private FancyEntrance m_fancyEntrance;

		private DateTime m_missionStartedTime;

		private TimeSpan m_missionDurationInSeconds;

		private int m_garrMissionID;

		private bool m_attemptedAutoComplete;

		private float m_timeUntilFadeOutMissionDetailsDisplay;

		private float m_timeUntilShowFollowerExperienceDisplays;

		[Header("Narrow Screen")]
		public GameObject m_EnemiesGroup;

		public GameObject m_FollowerSlotGroup;

		private bool m_mainCallbackInitialized;

		public MissionResultsPanel()
		{
		}

		private void Awake()
		{
			this.m_bonusLootInitialLocalPosition = this.m_bonusMissionRewardDisplay.m_rewardIcon.transform.localPosition;
			this.m_attemptedAutoComplete = false;
			this.m_darknessBG.SetActive(false);
			this.m_popupView.SetActive(false);
			if (this.m_partyBuffsText != null)
			{
				this.m_partyBuffsText.font = GeneralHelpers.LoadStandardFont();
				this.m_partyBuffsText.text = StaticDB.GetString("PARTY_BUFFS", null);
			}
			if (this.m_bonusLootChanceText != null)
			{
				this.m_bonusLootChanceText.font = GeneralHelpers.LoadStandardFont();
			}
		}

		public void HandleFollowerDataChanged()
		{
			WrapperGarrisonFollower wrapperGarrisonFollower;
			if (!this.m_popupView.activeSelf)
			{
				return;
			}
			FollowerExperienceDisplay[] componentsInChildren = this.m_followerExperienceDisplayArea.GetComponentsInChildren<FollowerExperienceDisplay>(true);
			int num = 0;
			FollowerExperienceDisplay[] followerExperienceDisplayArray = componentsInChildren;
			for (int i = 0; i < (int)followerExperienceDisplayArray.Length; i++)
			{
				FollowerExperienceDisplay followerExperienceDisplay = followerExperienceDisplayArray[i];
				if (PersistentFollowerData.preMissionFollowerDictionary.ContainsKey(followerExperienceDisplay.GetFollowerID()))
				{
					WrapperGarrisonFollower item = PersistentFollowerData.preMissionFollowerDictionary[followerExperienceDisplay.GetFollowerID()];
					wrapperGarrisonFollower = (!PersistentFollowerData.followerDictionary.ContainsKey(followerExperienceDisplay.GetFollowerID()) ? new WrapperGarrisonFollower()
					{
						GarrFollowerID = item.GarrFollowerID,
						Quality = item.Quality,
						Durability = 0
					} : PersistentFollowerData.followerDictionary[followerExperienceDisplay.GetFollowerID()]);
					followerExperienceDisplay.SetFollower(item, wrapperGarrisonFollower, (float)num * this.m_experienceDisplayEntranceDelay);
					num++;
				}
			}
		}

		public void HideMissionResults()
		{
			this.m_darknessBG.SetActive(false);
			this.m_popupView.SetActive(false);
		}

		private void InitFollowerExperienceDisplays()
		{
			int num = 0;
			foreach (WrapperGarrisonFollower value in PersistentFollowerData.preMissionFollowerDictionary.Values)
			{
				if (value.CurrentMissionID == this.m_garrMissionID)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_followerExperienceDisplayPrefab);
					FollowerExperienceDisplay component = gameObject.GetComponent<FollowerExperienceDisplay>();
					FancyEntrance fancyEntrance = gameObject.GetComponent<FancyEntrance>();
					float mExperienceDisplayEntranceDelay = (float)num * this.m_experienceDisplayEntranceDelay;
					fancyEntrance.m_timeToDelayEntrance = mExperienceDisplayEntranceDelay;
					fancyEntrance.Activate();
					component.SetFollower(value, value, mExperienceDisplayEntranceDelay);
					component.transform.SetParent(this.m_followerExperienceDisplayArea.transform, false);
					num++;
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

		private void OnBonusLootShakeComplete()
		{
			this.m_bonusMissionRewardDisplay.m_rewardIcon.transform.localPosition = this.m_bonusLootInitialLocalPosition;
		}

		private void OnBonusLootShakeUpdate(float val)
		{
			this.m_bonusMissionRewardDisplay.m_rewardIcon.transform.localPosition = new Vector3(this.m_bonusLootInitialLocalPosition.x + UnityEngine.Random.Range(-this.m_bonusLootShakeAmountX * val, this.m_bonusLootShakeAmountX * val), this.m_bonusLootInitialLocalPosition.y + UnityEngine.Random.Range(-this.m_bonusLootShakeAmountY * val, this.m_bonusLootShakeAmountY * val), this.m_bonusLootInitialLocalPosition.z);
		}

		private void OnDisable()
		{
			if (Main.instance != null && this.m_mainCallbackInitialized)
			{
				Main.instance.MissionSuccessChanceChangedAction -= new Action<int>(this.OnMissionSuccessChanceChanged);
				Main.instance.FollowerDataChangedAction -= new Action(this.HandleFollowerDataChanged);
				this.m_mainCallbackInitialized = false;
			}
			if (AdventureMapPanel.instance != null)
			{
				AdventureMapPanel.instance.ShowMissionResultAction -= new Action<int, int, bool>(this.ShowMissionResults);
			}
		}

		private void OnEnable()
		{
			this.m_missionSuccessMessage.SetActive(false);
			this.m_missionFailMessage.SetActive(false);
			this.m_missionInProgressMessage.SetActive(false);
			this.m_bonusMissionRewardDisplay.m_rewardIcon.transform.localPosition = this.m_bonusLootInitialLocalPosition;
			if (AdventureMapPanel.instance != null)
			{
				AdventureMapPanel.instance.ShowMissionResultAction += new Action<int, int, bool>(this.ShowMissionResults);
			}
			this.m_okButtonText.text = StaticDB.GetString("OK", null);
			this.m_inProgressText.text = StaticDB.GetString("IN_PROGRESS", null);
			this.m_successText.text = StaticDB.GetString("MISSION_SUCCESS", null);
			this.m_failureText.text = StaticDB.GetString("MISSION_FAILED", null);
		}

		private void OnMissionSuccessChanceChanged(int chance)
		{
			if (this.m_garrMissionID == 0)
			{
				return;
			}
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			this.m_bonusLootDisplay.SetActive(false);
			if (chance > -1000)
			{
				this.missionPercentChanceText.text = string.Concat(chance, "%");
				this.m_missionChanceSpinner.SetActive(false);
			}
			else
			{
				this.missionPercentChanceText.text = "%";
				this.m_missionChanceSpinner.SetActive(true);
			}
			this.m_lootBorderNormal.SetActive(chance < 100);
			this.m_lootBorderLitUp.SetActive(chance >= 100);
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(this.m_garrMissionID);
			if (record == null)
			{
				Debug.LogError(string.Concat("Invalid Mission ID:", this.m_garrMissionID));
				return;
			}
			if (StaticDB.rewardPackDB.GetRecord(record.OvermaxRewardPackID) == null)
			{
				return;
			}
			if (PersistentMissionData.missionDictionary.ContainsKey(this.m_garrMissionID))
			{
				WrapperGarrisonMission item = PersistentMissionData.missionDictionary[this.m_garrMissionID];
				if (record.OvermaxRewardPackID > 0 && item.OvermaxRewards.Count > 0)
				{
					this.m_bonusLootDisplay.SetActive(true);
					this.m_bonusLootChanceText.text = string.Concat("<color=#ff9600ff>", Mathf.Max(0, chance - 100), "%</color>");
					this.m_bonusLootChance = Mathf.Max(0, chance - 100);
				}
			}
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
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(this.m_garrMissionID);
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
					int[] buffsForCurrentMission = GeneralHelpers.GetBuffsForCurrentMission(num, this.m_garrMissionID, this.missionFollowerSlotGroup, adjustedMissionDuration);
					int[] numArray = buffsForCurrentMission;
					for (int k = 0; k < (int)numArray.Length; k++)
					{
						nums.Add(numArray[k]);
					}
				}
			}
			AllPopups.instance.ShowPartyBuffsPopup(nums.ToArray());
		}

		private void OnShowFailMessage()
		{
			Main.instance.m_UISound.Play_MissionFailure();
		}

		private void OnShowSuccessMessage()
		{
			Main.instance.m_UISound.Play_MissionSuccess();
		}

		private void RegisterMainScriptObjEvents()
		{
			if (Main.instance != null && !this.m_mainCallbackInitialized)
			{
				Main.instance.MissionSuccessChanceChangedAction += new Action<int>(this.OnMissionSuccessChanceChanged);
				Main.instance.FollowerDataChangedAction += new Action(this.HandleFollowerDataChanged);
				this.m_mainCallbackInitialized = true;
			}
		}

		public void ShowMissionResults(int garrMissionID, int missionResultType, bool awardOvermax)
		{
			int num;
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(garrMissionID);
			if (record == null)
			{
				return;
			}
			this.RegisterMainScriptObjEvents();
			this.m_missionResultsDisplayCanvasGroupAutoFadeOut.Reset();
			this.m_currentResultType = (MissionResultType)missionResultType;
			this.m_followerExperienceDisplayArea.SetActive(false);
			this.m_attemptedAutoComplete = false;
			this.m_garrMissionID = garrMissionID;
			this.m_darknessBG.SetActive(true);
			this.m_popupView.SetActive(true);
			this.m_bonusLootDisplay.SetActive(false);
			if (this.missionFollowerSlotGroup != null)
			{
				MissionFollowerSlot[] componentsInChildren = this.missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
				for (int i = 0; i < (int)componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] != null && componentsInChildren[i] != this.missionFollowerSlotGroup.transform)
					{
						componentsInChildren[i].gameObject.transform.SetParent(null);
						UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
					}
				}
			}
			MissionEncounter[] missionEncounterArray = this.enemyPortraitsGroup.GetComponentsInChildren<MissionEncounter>(true);
			for (int j = 0; j < (int)missionEncounterArray.Length; j++)
			{
				if (missionEncounterArray[j] != null && missionEncounterArray[j] != this.enemyPortraitsGroup.transform)
				{
					missionEncounterArray[j].gameObject.transform.SetParent(null);
					UnityEngine.Object.Destroy(missionEncounterArray[j].gameObject);
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
			this.m_missionStartedTime = item.StartTime;
			this.m_missionDurationInSeconds = item.MissionDuration;
			for (int k = 0; k < item.Encounters.Count; k++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.missionEncounterPrefab);
				gameObject.transform.SetParent(this.enemyPortraitsGroup.transform, false);
				MissionEncounter component = gameObject.GetComponent<MissionEncounter>();
				num = (item.Encounters[k].MechanicIDs.Count <= 0 ? 0 : item.Encounters[k].MechanicIDs[0]);
				component.SetEncounter(item.Encounters[k].EncounterID, num);
			}
			this.missionNameText.text = record.Name;
			this.missionLocationText.text = record.Location;
			this.missioniLevelText.text = string.Concat(StaticDB.GetString("ITEM_LEVEL_ABBREVIATION", null), " ", record.TargetItemLevel);
			GarrMissionTypeRec garrMissionTypeRec = StaticDB.garrMissionTypeDB.GetRecord((int)record.GarrMissionTypeID);
			this.missionTypeImage.overrideSprite = TextureAtlas.instance.GetAtlasSprite((int)garrMissionTypeRec.UiTextureAtlasMemberID);
			if (this.missionFollowerSlotGroup != null)
			{
				for (int l = 0; (long)l < (ulong)record.MaxFollowers; l++)
				{
					GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.missionFollowerSlotPrefab);
					gameObject1.transform.SetParent(this.missionFollowerSlotGroup.transform, false);
					gameObject1.GetComponent<MissionFollowerSlot>().m_enemyPortraitsGroup = this.enemyPortraitsGroup;
				}
			}
			if (record.UiTextureKitID <= 0)
			{
				Debug.LogWarning(string.Concat(new object[] { "DATA ERROR: Mission UITextureKit Not Set for mission ID:", record.ID, " - ", record.Name }));
				Debug.LogWarning("This means the scrolling background images will show the wrong location");
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
					Sprite sprite = TextureAtlas.instance.GetAtlasSprite(uITextureAtlasMemberID1);
					if (sprite != null)
					{
						this.m_scrollingEnvironment_Mid.enabled = true;
						this.m_scrollingEnvironment_Mid.sprite = sprite;
					}
				}
				int num1 = TextureAtlas.GetUITextureAtlasMemberID(string.Concat("_", uiTextureKitRec.KitPrefix, "-Fore"));
				if (num1 > 0)
				{
					Sprite atlasSprite1 = TextureAtlas.instance.GetAtlasSprite(num1);
					if (atlasSprite1 != null)
					{
						this.m_scrollingEnvironment_Fore.enabled = true;
						this.m_scrollingEnvironment_Fore.sprite = atlasSprite1;
					}
				}
			}
			if (this.m_lootGroupObj == null || this.m_missionRewardDisplayPrefab == null)
			{
				return;
			}
			MissionRewardDisplay[] missionRewardDisplayArray = this.m_lootGroupObj.GetComponentsInChildren<MissionRewardDisplay>(true);
			for (int m = 0; m < (int)missionRewardDisplayArray.Length; m++)
			{
				if (missionRewardDisplayArray[m] != null)
				{
					missionRewardDisplayArray[m].gameObject.transform.SetParent(null);
					UnityEngine.Object.Destroy(missionRewardDisplayArray[m].gameObject);
				}
			}
			if (missionResultType == 1)
			{
				PersistentFollowerData.ClearPreMissionFollowerData();
			}
			List<WrapperGarrisonFollower> wrapperGarrisonFollowers = new List<WrapperGarrisonFollower>();
			MissionFollowerSlot[] missionFollowerSlotArray = this.missionFollowerSlotGroup.GetComponentsInChildren<MissionFollowerSlot>(true);
			int num2 = 0;
			foreach (WrapperGarrisonFollower value in PersistentFollowerData.followerDictionary.Values)
			{
				if (value.CurrentMissionID != garrMissionID)
				{
					continue;
				}
				int num3 = num2;
				num2 = num3 + 1;
				missionFollowerSlotArray[num3].SetFollower(value.GarrFollowerID);
				if (missionResultType == 1)
				{
					PersistentFollowerData.CachePreMissionFollower(value);
				}
				wrapperGarrisonFollowers.Add(value);
			}
			this.UpdateMissionStatus(garrMissionID);
			MissionFollowerSlot[] missionFollowerSlotArray1 = missionFollowerSlotArray;
			for (int n = 0; n < (int)missionFollowerSlotArray1.Length; n++)
			{
				missionFollowerSlotArray1[n].InitHeartPanel();
			}
			MissionRewardDisplay.InitMissionRewards(this.m_missionRewardDisplayPrefab.gameObject, this.m_lootGroupObj.transform, item.Rewards);
			if (record.OvermaxRewardPackID > 0 && item.OvermaxRewards.Count > 0)
			{
				this.m_bonusLootDisplay.SetActive(true);
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
						CurrencyTypesRec currencyTypesRec = StaticDB.currencyTypesDB.GetRecord(wrapperGarrisonMissionReward.CurrencyType);
						int currencyQuantity = (int)((ulong)wrapperGarrisonMissionReward.CurrencyQuantity / (long)(((currencyTypesRec.Flags & 8) == 0 ? 1 : 100)));
						this.m_bonusMissionRewardDisplay.InitReward(MissionRewardDisplay.RewardType.currency, wrapperGarrisonMissionReward.CurrencyType, currencyQuantity, 0, 0);
					}
					else
					{
						this.m_bonusMissionRewardDisplay.InitReward(MissionRewardDisplay.RewardType.gold, 0, (int)(wrapperGarrisonMissionReward.CurrencyQuantity / 10000), 0, 0);
					}
				}
			}
			this.m_timeUntilFadeOutMissionDetailsDisplay = this.m_missionDetailsFadeOutDelay;
			this.m_timeUntilShowFollowerExperienceDisplays = this.m_experienceDisplayInitialEntranceDelay;
			if (missionResultType == 2)
			{
				this.InitFollowerExperienceDisplays();
				this.m_missionInProgressMessage.SetActive(false);
				this.m_missionSuccessMessage.SetActive(true);
				this.m_missionFailMessage.SetActive(false);
				if (this.m_fancyEntrance != null)
				{
					UnityEngine.Object.Destroy(this.m_fancyEntrance);
					iTween.Stop(this.m_missionSuccessMessage);
					this.m_missionSuccessMessage.transform.localScale = Vector3.one;
					iTween.Stop(this.m_missionFailMessage);
					this.m_missionFailMessage.transform.localScale = Vector3.one;
				}
				this.m_missionSuccessMessage.SetActive(false);
				this.m_fancyEntrance = this.m_missionSuccessMessage.AddComponent<FancyEntrance>();
				this.m_fancyEntrance.m_fadeInCanvasGroup = this.m_missionSuccessMessage.GetComponent<CanvasGroup>();
				this.m_fancyEntrance.m_fadeInTime = this.m_messageFadeInTime;
				this.m_fancyEntrance.m_punchScale = this.m_messagePunchScale;
				this.m_fancyEntrance.m_punchScaleAmount = this.m_messagePunchScaleAmount;
				this.m_fancyEntrance.m_punchScaleDuration = this.m_messagePunchScaleDuration;
				this.m_fancyEntrance.m_timeToDelayEntrance = this.m_messageTimeToDelayEntrance;
				this.m_fancyEntrance.m_activateOnEnable = true;
				this.m_fancyEntrance.m_objectToNotifyOnBegin = base.gameObject;
				this.m_fancyEntrance.m_notifyOnBeginCallbackName = "OnShowSuccessMessage";
				this.m_missionSuccessMessage.SetActive(true);
				MissionRewardDisplay[] componentsInChildren1 = this.m_lootGroupObj.GetComponentsInChildren<MissionRewardDisplay>(true);
				for (int o = 0; o < (int)componentsInChildren1.Length; o++)
				{
					componentsInChildren1[o].ShowResultSuccess(this.m_lootEffectInitialDelay + this.m_lootEffectDelay * (float)o);
				}
				if (this.m_bonusLootChance > 0)
				{
					iTween.ValueTo(base.gameObject, iTween.Hash(new object[] { "name", "ShakeIt", "from", 0.3f, "to", 1f, "delay", this.m_bonusLootShakeInitialDelay, "time", this.m_bonusLootShakeDuration, "onupdate", "OnBonusLootShakeUpdate", "oncomplete", "OnBonusLootShakeComplete" }));
				}
				if (!awardOvermax)
				{
					this.m_bonusMissionRewardDisplay.ShowResultFail(this.m_lootEffectInitialDelay + this.m_lootEffectDelay * (float)((int)componentsInChildren1.Length));
				}
				else
				{
					this.m_bonusMissionRewardDisplay.ShowResultSuccess(this.m_lootEffectInitialDelay + this.m_lootEffectDelay * (float)((int)componentsInChildren1.Length));
				}
			}
			if (missionResultType == 3)
			{
				this.InitFollowerExperienceDisplays();
				this.m_missionInProgressMessage.SetActive(false);
				this.m_missionSuccessMessage.SetActive(false);
				this.m_missionFailMessage.SetActive(true);
				if (this.m_fancyEntrance != null)
				{
					UnityEngine.Object.Destroy(this.m_fancyEntrance);
					iTween.Stop(this.m_missionSuccessMessage);
					this.m_missionSuccessMessage.transform.localScale = Vector3.one;
					iTween.Stop(this.m_missionFailMessage);
					this.m_missionFailMessage.transform.localScale = Vector3.one;
				}
				this.m_missionFailMessage.SetActive(false);
				this.m_fancyEntrance = this.m_missionFailMessage.AddComponent<FancyEntrance>();
				this.m_fancyEntrance.m_fadeInCanvasGroup = this.m_missionFailMessage.GetComponent<CanvasGroup>();
				this.m_fancyEntrance.m_fadeInTime = this.m_messageFadeInTime;
				this.m_fancyEntrance.m_punchScale = this.m_messagePunchScale;
				this.m_fancyEntrance.m_punchScaleAmount = this.m_messagePunchScaleAmount;
				this.m_fancyEntrance.m_punchScaleDuration = this.m_messagePunchScaleDuration;
				this.m_fancyEntrance.m_timeToDelayEntrance = this.m_messageTimeToDelayEntrance;
				this.m_fancyEntrance.m_activateOnEnable = true;
				this.m_fancyEntrance.m_objectToNotifyOnBegin = base.gameObject;
				this.m_fancyEntrance.m_notifyOnBeginCallbackName = "OnShowFailMessage";
				this.m_missionFailMessage.SetActive(true);
				MissionRewardDisplay[] missionRewardDisplayArray1 = this.m_lootGroupObj.GetComponentsInChildren<MissionRewardDisplay>(true);
				for (int p = 0; p < (int)missionRewardDisplayArray1.Length; p++)
				{
					missionRewardDisplayArray1[p].ShowResultFail(this.m_lootEffectInitialDelay);
				}
				this.m_bonusMissionRewardDisplay.ShowResultFail(this.m_lootEffectInitialDelay);
			}
			if (missionResultType == 0)
			{
				this.m_missionInProgressMessage.SetActive(true);
				this.m_missionSuccessMessage.SetActive(false);
				this.m_missionFailMessage.SetActive(false);
				this.m_bonusMissionRewardDisplay.ClearResults();
			}
			if (missionResultType == 1)
			{
				this.m_missionInProgressMessage.SetActive(false);
				this.m_missionSuccessMessage.SetActive(false);
				this.m_missionFailMessage.SetActive(false);
				FollowerExperienceDisplay[] followerExperienceDisplayArray = this.m_followerExperienceDisplayArea.GetComponentsInChildren<FollowerExperienceDisplay>(true);
				for (int q = 0; q < (int)followerExperienceDisplayArray.Length; q++)
				{
					FollowerExperienceDisplay followerExperienceDisplay = followerExperienceDisplayArray[q];
					followerExperienceDisplay.gameObject.transform.SetParent(null);
					UnityEngine.Object.Destroy(followerExperienceDisplay.gameObject);
				}
			}
			if (this.m_partyBuffGroup == null)
			{
				return;
			}
			AbilityDisplay[] abilityDisplayArray = this.m_partyBuffGroup.GetComponentsInChildren<AbilityDisplay>(true);
			for (int r = 0; r < (int)abilityDisplayArray.Length; r++)
			{
				AbilityDisplay abilityDisplay = abilityDisplayArray[r];
				abilityDisplay.gameObject.transform.SetParent(null);
				UnityEngine.Object.Destroy(abilityDisplay.gameObject);
			}
			int adjustedMissionDuration = GeneralHelpers.GetAdjustedMissionDuration(record, wrapperGarrisonFollowers, this.enemyPortraitsGroup);
			int length = 0;
			foreach (WrapperGarrisonFollower wrapperGarrisonFollower in PersistentFollowerData.followerDictionary.Values)
			{
				if (wrapperGarrisonFollower.CurrentMissionID == garrMissionID)
				{
					int[] buffsForCurrentMission = GeneralHelpers.GetBuffsForCurrentMission(wrapperGarrisonFollower.GarrFollowerID, garrMissionID, this.missionFollowerSlotGroup, adjustedMissionDuration);
					length += (int)buffsForCurrentMission.Length;
					int[] numArray = buffsForCurrentMission;
					for (int s = 0; s < (int)numArray.Length; s++)
					{
						int num4 = numArray[s];
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.m_mechanicEffectDisplayPrefab);
						gameObject2.transform.SetParent(this.m_partyBuffGroup.transform, false);
						gameObject2.GetComponent<AbilityDisplay>().SetAbility(num4, false, false, null);
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
			HorizontalLayoutGroup horizontalLayoutGroup = this.m_partyBuffGroup.GetComponent<HorizontalLayoutGroup>();
			if (horizontalLayoutGroup != null)
			{
				if (length <= 10 || !Main.instance.IsNarrowScreen())
				{
					horizontalLayoutGroup.spacing = 6f;
				}
				else
				{
					horizontalLayoutGroup.spacing = 3f;
				}
			}
			this.m_partyBuffGroup.SetActive(length > 0);
		}

		private void Start()
		{
			if (Main.instance.IsNarrowScreen())
			{
				this.NarrowScreenAdjust();
			}
		}

		private void Update()
		{
			this.UpdateMissionRemainingTimeDisplay();
			if (!this.m_followerExperienceDisplayArea.activeSelf && (this.m_currentResultType == MissionResultType.success || this.m_currentResultType == MissionResultType.failure))
			{
				this.m_timeUntilFadeOutMissionDetailsDisplay -= Time.deltaTime;
				if (this.m_timeUntilFadeOutMissionDetailsDisplay < 0f)
				{
					this.m_missionResultsDisplayCanvasGroupAutoFadeOut.EnableFadeOut();
				}
				this.m_timeUntilShowFollowerExperienceDisplays -= Time.deltaTime;
				if (this.m_timeUntilShowFollowerExperienceDisplays < 0f)
				{
					this.m_followerExperienceDisplayArea.SetActive(true);
				}
			}
		}

		private void UpdateMissionRemainingTimeDisplay()
		{
			if (!this.m_missionInProgressMessage.activeSelf)
			{
				return;
			}
			TimeSpan timeSpan = GarrisonStatus.CurrentTime() - this.m_missionStartedTime;
			TimeSpan mMissionDurationInSeconds = this.m_missionDurationInSeconds - timeSpan;
			bool flag = (mMissionDurationInSeconds.TotalSeconds >= 0 ? false : this.m_popupView.gameObject.activeSelf);
			mMissionDurationInSeconds = (mMissionDurationInSeconds.TotalSeconds <= 0 ? TimeSpan.Zero : mMissionDurationInSeconds);
			this.m_missionTimeRemainingText.text = mMissionDurationInSeconds.GetDurationString(false);
			if (flag && !this.m_attemptedAutoComplete)
			{
				if (AdventureMapPanel.instance.ShowMissionResultAction != null)
				{
					AdventureMapPanel.instance.ShowMissionResultAction(this.m_garrMissionID, 1, false);
				}
				Main.instance.CompleteMission(this.m_garrMissionID);
				this.m_attemptedAutoComplete = true;
			}
		}

		public void UpdateMissionStatus(int garrMissionID)
		{
			MissionMechanic[] componentsInChildren = this.enemyPortraitsGroup.GetComponentsInChildren<MissionMechanic>(true);
			if (componentsInChildren == null)
			{
				return;
			}
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetCountered(false, false, true);
			}
			AbilityDisplay[] abilityDisplayArray = this.enemyPortraitsGroup.GetComponentsInChildren<AbilityDisplay>(true);
			if (abilityDisplayArray == null)
			{
				return;
			}
			for (int j = 0; j < (int)abilityDisplayArray.Length; j++)
			{
				abilityDisplayArray[j].SetCountered(false, true);
			}
			MissionMechanicTypeCounter[] missionMechanicTypeCounterArray = base.gameObject.GetComponentsInChildren<MissionMechanicTypeCounter>(true);
			if (missionMechanicTypeCounterArray == null)
			{
				return;
			}
			for (int k = 0; k < (int)missionMechanicTypeCounterArray.Length; k++)
			{
				missionMechanicTypeCounterArray[k].usedIcon.gameObject.SetActive(false);
				int num = 0;
				while (num < (int)componentsInChildren.Length)
				{
					if (missionMechanicTypeCounterArray[k].countersMissionMechanicTypeID != componentsInChildren[num].m_missionMechanicTypeID || componentsInChildren[num].IsCountered())
					{
						num++;
					}
					else
					{
						componentsInChildren[num].SetCountered(true, false, false);
						if (num < (int)abilityDisplayArray.Length)
						{
							abilityDisplayArray[num].SetCountered(true, false);
						}
						break;
					}
				}
			}
			MissionFollowerSlot[] missionFollowerSlotArray = base.gameObject.GetComponentsInChildren<MissionFollowerSlot>(true);
			List<WrapperGarrisonFollower> wrapperGarrisonFollowers = new List<WrapperGarrisonFollower>();
			for (int l = 0; l < (int)missionFollowerSlotArray.Length; l++)
			{
				int currentGarrFollowerID = missionFollowerSlotArray[l].GetCurrentGarrFollowerID();
				if (PersistentFollowerData.followerDictionary.ContainsKey(currentGarrFollowerID))
				{
					wrapperGarrisonFollowers.Add(PersistentFollowerData.followerDictionary[currentGarrFollowerID]);
				}
			}
			int item = -1000;
			if (!MissionDataCache.missionDataDictionary.ContainsKey(this.m_garrMissionID))
			{
				LegionCompanionWrapper.EvaluateMission(garrMissionID, (
					from f in wrapperGarrisonFollowers
					select f.GarrFollowerID).ToList<int>());
			}
			else
			{
				item = MissionDataCache.missionDataDictionary[this.m_garrMissionID];
			}
			this.OnMissionSuccessChanceChanged(item);
		}
	}
}