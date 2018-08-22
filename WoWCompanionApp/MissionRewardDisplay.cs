using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class MissionRewardDisplay : MonoBehaviour
	{
		public bool m_isExpandedDisplay;

		public Image m_rewardIcon;

		public Text m_rewardQuantityText;

		public Button m_mainButton;

		public Text m_rewardName;

		[Header("Loot result effects")]
		private bool m_enableLootEffect_success;

		private bool m_enableLootEffect_fail;

		public string m_uiAnimation_success;

		public string m_uiAnimation_fail;

		public float m_animScale_success;

		public float m_animScale_fail;

		public Image m_greenCheck;

		public Image m_redX;

		public Image m_qualityBorder;

		public Transform m_greenCheckEffectRootTransform;

		public Transform m_redFailXEffectRootTransform;

		public Transform m_glowEffectRootTransform;

		[Header("Consume this item")]
		public Transform m_readyToUseGlowEffectRootTransform;

		public GameObject m_collectingSpinner;

		public GameObject m_useItemMessageBaseObj;

		public Text m_useItemMessage;

		[Header("Error reporting")]
		public Text m_iconErrorText;

		private float timeRemainingUntilLootEffect;

		private int m_rewardQuantity;

		private MissionRewardDisplay.RewardType m_rewardType;

		private int m_itemContext;

		private int m_rewardID;

		private UiAnimMgr.UiAnimHandle m_effectHandle;

		private UiAnimMgr.UiAnimHandle m_glowEffectHandle;

		private UiAnimMgr.UiAnimHandle m_akReadyToConsumeEffectHandle;

		public MissionRewardDisplay()
		{
		}

		private void Awake()
		{
			this.ClearResults();
			if (this.m_collectingSpinner != null)
			{
				this.m_collectingSpinner.SetActive(false);
			}
			if (this.m_useItemMessage != null)
			{
				this.m_useItemMessage.font = GeneralHelpers.LoadStandardFont();
				this.m_useItemMessage.text = StaticDB.GetString("USE", "Use");
			}
		}

		public void ClearResults()
		{
			this.m_enableLootEffect_success = false;
			this.m_enableLootEffect_fail = false;
			if (this.m_greenCheck != null)
			{
				this.m_greenCheck.gameObject.SetActive(false);
			}
			if (this.m_redX != null)
			{
				this.m_redX.gameObject.SetActive(false);
			}
			if (this.m_effectHandle != null)
			{
				UiAnimation anim = this.m_effectHandle.GetAnim();
				if (anim != null)
				{
					anim.Stop(0f);
				}
			}
			if (this.m_glowEffectHandle != null)
			{
				UiAnimation uiAnimation = this.m_glowEffectHandle.GetAnim();
				if (uiAnimation != null)
				{
					uiAnimation.Stop(0f);
				}
			}
			if (this.m_akReadyToConsumeEffectHandle != null)
			{
				UiAnimation anim1 = this.m_akReadyToConsumeEffectHandle.GetAnim();
				if (anim1 != null)
				{
					anim1.Stop(0f);
				}
			}
		}

		public void ConsumeThisItem()
		{
			Main.instance.m_UISound.Play_ArtifactClick();
			this.m_mainButton.enabled = false;
			if (this.m_collectingSpinner != null)
			{
				this.m_collectingSpinner.SetActive(true);
			}
			if (this.m_useItemMessageBaseObj != null)
			{
				this.m_useItemMessageBaseObj.SetActive(false);
			}
			this.StopAKReadyToUseAnim();
		}

		private int GetFactionRewardQuantity(int index)
		{
			switch (index)
			{
				case 0:
				{
					return 0;
				}
				case 1:
				{
					return 10;
				}
				case 2:
				{
					return 25;
				}
				case 3:
				{
					return 75;
				}
				case 4:
				{
					return 150;
				}
				case 5:
				{
					return 250;
				}
				case 6:
				{
					return 350;
				}
				case 7:
				{
					return 500;
				}
				case 8:
				{
					return 1000;
				}
				default:
				{
					return 0;
				}
			}
		}

		public static void InitMissionRewards(GameObject prefab, Transform parent, IEnumerable<WrapperGarrisonMissionReward> rewards)
		{
			foreach (WrapperGarrisonMissionReward reward in rewards)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
				gameObject.SetActive(true);
				gameObject.transform.SetParent(parent, false);
				MissionRewardDisplay component = gameObject.GetComponent<MissionRewardDisplay>();
				if (reward.ItemID > 0)
				{
					component.InitReward(MissionRewardDisplay.RewardType.item, reward.ItemID, (int)reward.ItemQuantity, 0, reward.ItemFileDataID);
				}
				else if (reward.FollowerXP <= 0)
				{
					if (reward.CurrencyQuantity <= 0)
					{
						continue;
					}
					if (reward.CurrencyType != 0)
					{
						CurrencyTypesRec record = StaticDB.currencyTypesDB.GetRecord(reward.CurrencyType);
						if (record == null)
						{
							Debug.LogError(string.Concat("Unknown CurrencyType ID ", reward.CurrencyType));
						}
						else
						{
							int currencyQuantity = (int)((ulong)reward.CurrencyQuantity / (long)(((record.Flags & 8) == 0 ? 1 : 100)));
							component.InitReward(MissionRewardDisplay.RewardType.currency, reward.CurrencyType, currencyQuantity, 0, 0);
						}
					}
					else
					{
						component.InitReward(MissionRewardDisplay.RewardType.gold, 0, (int)(reward.CurrencyQuantity / 10000), 0, 0);
					}
				}
				else
				{
					component.InitReward(MissionRewardDisplay.RewardType.followerXP, 0, (int)reward.FollowerXP, 0, 0);
				}
			}
		}

		public void InitReward(MissionRewardDisplay.RewardType rewardType, int rewardID, int rewardQuantity, int itemContext, int iconFileDataID = 0)
		{
			if (rewardType == MissionRewardDisplay.RewardType.faction)
			{
				return;
			}
			this.ClearResults();
			this.m_rewardType = rewardType;
			this.m_rewardID = rewardID;
			this.m_rewardQuantity = rewardQuantity;
			this.m_itemContext = itemContext;
			if (this.m_iconErrorText != null)
			{
				this.m_iconErrorText.gameObject.SetActive(false);
			}
			switch (this.m_rewardType)
			{
				case MissionRewardDisplay.RewardType.item:
				{
					Sprite sprite = null;
					ItemRec record = StaticDB.itemDB.GetRecord(this.m_rewardID);
					if (iconFileDataID > 0)
					{
						sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, iconFileDataID);
					}
					else if (record != null)
					{
						sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, record.IconFileDataID);
					}
					if (sprite != null)
					{
						this.m_rewardIcon.sprite = sprite;
					}
					else if (this.m_iconErrorText != null)
					{
						this.m_iconErrorText.gameObject.SetActive(true);
						this.m_iconErrorText.text = string.Concat(string.Empty, iconFileDataID);
					}
					if (this.m_rewardName != null)
					{
						if (record == null)
						{
							this.m_rewardName.text = string.Concat("Unknown Item ", this.m_rewardID);
						}
						else
						{
							this.m_rewardName.text = record.Display;
							this.m_rewardName.color = GeneralHelpers.GetQualityColor(record.OverallQualityID);
						}
					}
					if (this.m_isExpandedDisplay)
					{
						this.m_rewardQuantityText.text = (this.m_rewardQuantity <= 1 ? string.Empty : this.m_rewardQuantity.ToString("N0"));
					}
					break;
				}
				case MissionRewardDisplay.RewardType.gold:
				{
					this.m_rewardIcon.sprite = Resources.Load<Sprite>("MiscIcons/INV_Misc_Coin_01");
					if (this.m_isExpandedDisplay)
					{
						this.m_rewardQuantityText.text = string.Empty;
						this.m_rewardName.text = (this.m_rewardQuantity <= 1 ? string.Empty : this.m_rewardQuantity.ToString("N0"));
					}
					break;
				}
				case MissionRewardDisplay.RewardType.followerXP:
				{
					this.m_rewardIcon.sprite = GeneralHelpers.GetLocalizedFollowerXpIcon();
					this.m_rewardQuantityText.text = string.Empty;
					if (this.m_rewardName != null && this.m_isExpandedDisplay)
					{
						this.m_rewardName.text = (this.m_rewardQuantity <= 1 ? string.Empty : string.Concat(this.m_rewardQuantity.ToString("N0"), " ", StaticDB.GetString("XP2", "XP")));
					}
					break;
				}
				case MissionRewardDisplay.RewardType.currency:
				{
					Sprite sprite1 = CurrencyContainerDB.LoadCurrencyContainerIcon(this.m_rewardID, this.m_rewardQuantity);
					if (sprite1 == null)
					{
						this.m_iconErrorText.gameObject.SetActive(true);
						this.m_iconErrorText.text = string.Concat("c ", this.m_rewardID);
					}
					else
					{
						this.m_rewardIcon.sprite = sprite1;
					}
					if (this.m_isExpandedDisplay)
					{
						CurrencyTypesRec currencyTypesRec = StaticDB.currencyTypesDB.GetRecord(rewardID);
						if (currencyTypesRec == null)
						{
							this.m_rewardName.text = string.Empty;
							this.m_rewardQuantityText.text = (this.m_rewardQuantity <= 1 ? string.Empty : this.m_rewardQuantity.ToString("N0"));
						}
						else
						{
							CurrencyContainerRec currencyContainerRec = CurrencyContainerDB.CheckAndGetValidCurrencyContainer(this.m_rewardID, this.m_rewardQuantity);
							if (currencyContainerRec == null)
							{
								this.m_rewardName.text = currencyTypesRec.Name;
								this.m_rewardQuantityText.text = (this.m_rewardQuantity <= 1 ? string.Empty : this.m_rewardQuantity.ToString("N0"));
							}
							else
							{
								this.m_rewardName.text = currencyContainerRec.ContainerName;
								this.m_rewardName.color = GeneralHelpers.GetQualityColor(currencyContainerRec.ContainerQuality);
								this.m_rewardQuantityText.text = string.Empty;
							}
						}
					}
					else if (StaticDB.currencyTypesDB.GetRecord(rewardID) != null)
					{
						CurrencyContainerRec currencyContainerRec1 = CurrencyContainerDB.CheckAndGetValidCurrencyContainer(this.m_rewardID, this.m_rewardQuantity);
						if (currencyContainerRec1 != null && currencyContainerRec1.ContainerQuality > 0 && this.m_qualityBorder != null)
						{
							this.m_qualityBorder.color = GeneralHelpers.GetQualityColor(currencyContainerRec1.ContainerQuality);
						}
					}
					break;
				}
			}
			if (!this.m_isExpandedDisplay)
			{
				if (CurrencyContainerDB.CheckAndGetValidCurrencyContainer(this.m_rewardID, rewardQuantity) == null)
				{
					this.m_rewardQuantityText.text = (this.m_rewardQuantity <= 1 ? string.Empty : this.m_rewardQuantity.ToString("N0"));
				}
				else
				{
					this.m_rewardQuantityText.text = string.Empty;
				}
			}
		}

		public static void InitWorldQuestRewards(WrapperWorldQuest worldQuest, GameObject prefab, Transform parent)
		{
			if (worldQuest.Items != null)
			{
				foreach (WrapperWorldQuestReward item in worldQuest.Items)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
					gameObject.transform.SetParent(parent, false);
					MissionRewardDisplay component = gameObject.GetComponent<MissionRewardDisplay>();
					component.InitReward(MissionRewardDisplay.RewardType.item, item.RecordID, item.Quantity, item.ItemContext, item.FileDataID);
				}
			}
			if (worldQuest.Money > 0)
			{
				GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(prefab);
				gameObject1.transform.SetParent(parent, false);
				MissionRewardDisplay missionRewardDisplay = gameObject1.GetComponent<MissionRewardDisplay>();
				missionRewardDisplay.InitReward(MissionRewardDisplay.RewardType.gold, 0, worldQuest.Money / 10000, 0, 0);
			}
			if (worldQuest.Experience > 0)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(prefab);
				gameObject2.transform.SetParent(parent, false);
				MissionRewardDisplay component1 = gameObject2.GetComponent<MissionRewardDisplay>();
				component1.InitReward(MissionRewardDisplay.RewardType.followerXP, 0, worldQuest.Experience, 0, 0);
			}
			foreach (WrapperWorldQuestReward currency in worldQuest.Currencies)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(prefab);
				gameObject3.transform.SetParent(parent, false);
				MissionRewardDisplay missionRewardDisplay1 = gameObject3.GetComponent<MissionRewardDisplay>();
				CurrencyTypesRec record = StaticDB.currencyTypesDB.GetRecord(currency.RecordID);
				if (record == null)
				{
					Debug.LogWarning(string.Concat(new object[] { "WORLD QUEST ", worldQuest.QuestID, " has bogus currency reward (id ", currency.RecordID, ")" }));
				}
				else
				{
					int quantity = currency.Quantity / ((record.Flags & 8) == 0 ? 1 : 100);
					missionRewardDisplay1.InitReward(MissionRewardDisplay.RewardType.currency, currency.RecordID, quantity, 0, 0);
				}
			}
		}

		public void PlayAKReadyToUseAnim()
		{
			this.m_akReadyToConsumeEffectHandle = UiAnimMgr.instance.PlayAnim("ItemReadyToUseGlowLoop", base.transform, Vector3.zero, 1.2f, 0f);
		}

		public void ShowResultFail(float delay)
		{
			this.m_enableLootEffect_fail = true;
			this.timeRemainingUntilLootEffect = delay;
		}

		public void ShowResultSuccess(float delay)
		{
			this.m_enableLootEffect_success = true;
			this.timeRemainingUntilLootEffect = delay;
		}

		public void ShowRewardTooltip()
		{
			Main.instance.allPopups.ShowRewardTooltip(this.m_rewardType, this.m_rewardID, this.m_rewardQuantity, this.m_rewardIcon, this.m_itemContext);
		}

		public void StopAKReadyToUseAnim()
		{
			if (this.m_akReadyToConsumeEffectHandle != null)
			{
				UiAnimation anim = this.m_akReadyToConsumeEffectHandle.GetAnim();
				if (anim != null)
				{
					anim.Stop(0f);
				}
				this.m_akReadyToConsumeEffectHandle = null;
			}
		}

		private void Update()
		{
			if (this.timeRemainingUntilLootEffect <= 0f)
			{
				return;
			}
			this.timeRemainingUntilLootEffect -= Time.deltaTime;
			if (this.timeRemainingUntilLootEffect <= 0f)
			{
				if (this.m_enableLootEffect_success)
				{
					this.m_greenCheck.gameObject.SetActive(true);
					if (this.m_glowEffectRootTransform != null)
					{
						this.m_glowEffectHandle = UiAnimMgr.instance.PlayAnim("GarrisonMissionRewardsEffectTemplate", this.m_glowEffectRootTransform, Vector3.zero, 1f, 0f);
					}
					this.m_effectHandle = UiAnimMgr.instance.PlayAnim(this.m_uiAnimation_success, this.m_greenCheckEffectRootTransform, Vector3.zero, this.m_animScale_success, 0f);
					Main.instance.m_UISound.Play_GreenCheck();
				}
				if (this.m_enableLootEffect_fail)
				{
					this.m_redX.gameObject.SetActive(true);
					this.m_effectHandle = UiAnimMgr.instance.PlayAnim(this.m_uiAnimation_fail, this.m_redFailXEffectRootTransform, Vector3.zero, this.m_animScale_fail, 0f);
					Main.instance.m_UISound.Play_RedFailX();
				}
			}
		}

		public enum RewardType
		{
			item,
			gold,
			followerXP,
			currency,
			faction
		}
	}
}