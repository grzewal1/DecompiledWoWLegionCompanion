using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class RewardInfoPopup : MonoBehaviour
	{
		public Text m_rewardName;

		public Text m_rewardDescription;

		public Text m_rewardQuantity;

		public Image m_rewardIcon;

		private int m_rewardID;

		private MissionRewardDisplay.RewardType m_rewardType;

		public bool m_muteEnableSFX;

		public bool m_disableScreenBlurEffect;

		public RewardInfoPopup()
		{
		}

		private void ItemStatsUpdated(int itemID, int itemContext, WrapperItemStats itemStats)
		{
			if (this.m_rewardType == MissionRewardDisplay.RewardType.item)
			{
				this.SetItem(this.m_rewardID, itemContext, this.m_rewardIcon.sprite);
			}
		}

		private void OnDisable()
		{
			if (!this.m_disableScreenBlurEffect)
			{
				Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
				Main.instance.m_canvasBlurManager.RemoveBlurRef_Level2Canvas();
			}
			if (this.m_rewardType == MissionRewardDisplay.RewardType.item)
			{
				ItemStatCache.instance.ItemStatCacheUpdateAction -= new Action<int, int, WrapperItemStats>(this.ItemStatsUpdated);
			}
			Main.instance.m_backButtonManager.PopBackAction();
		}

		public void OnEnable()
		{
			if (!this.m_muteEnableSFX)
			{
				Main.instance.m_UISound.Play_ShowGenericTooltip();
			}
			if (!this.m_disableScreenBlurEffect)
			{
				Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
				Main.instance.m_canvasBlurManager.AddBlurRef_Level2Canvas();
			}
			Main.instance.m_backButtonManager.PushBackAction(BackActionType.hideAllPopups, null);
		}

		public void SetCurrency(int currencyID, int quantity, Sprite iconSprite)
		{
			CurrencyTypesRec record = StaticDB.currencyTypesDB.GetRecord(currencyID);
			if (record != null)
			{
				this.m_rewardName.text = record.Name;
				this.m_rewardDescription.text = record.Description;
			}
			this.m_rewardQuantity.text = (quantity <= 1 ? string.Empty : string.Concat(string.Empty, quantity));
			this.m_rewardIcon.sprite = iconSprite;
		}

		public void SetFaction(int factionID, int quantity, Sprite iconSprite)
		{
			FactionRec record = StaticDB.factionDB.GetRecord(factionID);
			if (record != null)
			{
				this.m_rewardName.text = string.Concat(new object[] { StaticDB.GetString("REPUTATION_AWARD", null), "\n<color=#", GeneralHelpers.s_defaultColor, ">", record.Name, " +", quantity, "</color>" });
				this.m_rewardName.supportRichText = true;
				this.m_rewardDescription.text = string.Empty;
			}
			this.m_rewardQuantity.text = (quantity <= 1 ? string.Empty : string.Concat(string.Empty, quantity));
			this.m_rewardIcon.sprite = iconSprite;
		}

		public void SetFollowerXP(int quantity, Sprite iconSprite)
		{
			this.m_rewardName.text = StaticDB.GetString("EXPERIENCE", null);
			this.m_rewardDescription.text = StaticDB.GetString("EXPERIENCE_DESCRIPTION", null);
			this.m_rewardQuantity.text = (quantity <= 1 ? string.Empty : string.Concat(string.Empty, quantity));
			this.m_rewardIcon.sprite = iconSprite;
		}

		public void SetGold(int quantity, Sprite iconSprite)
		{
			this.m_rewardName.text = StaticDB.GetString("GOLD", null);
			this.m_rewardDescription.text = StaticDB.GetString("GOLD_DESCRIPTION", null);
			this.m_rewardQuantity.text = (quantity <= 1 ? string.Empty : string.Concat(string.Empty, quantity));
			this.m_rewardIcon.sprite = iconSprite;
		}

		public void SetItem(int itemID, int itemContext, Sprite iconSprite)
		{
			string str;
			string str1;
			this.m_rewardQuantity.text = string.Empty;
			this.m_rewardName.text = string.Empty;
			this.m_rewardDescription.text = string.Empty;
			this.m_rewardIcon.sprite = iconSprite;
			ItemRec record = StaticDB.itemDB.GetRecord(itemID);
			if (record == null)
			{
				this.m_rewardName.text = string.Concat("Unknown Item", itemID);
				this.m_rewardDescription.text = string.Empty;
			}
			else
			{
				WrapperItemStats? itemStats = ItemStatCache.instance.GetItemStats(itemID, itemContext);
				if (!itemStats.HasValue)
				{
					this.m_rewardName.text = string.Concat(GeneralHelpers.GetItemQualityColorTag(record.OverallQualityID), record.Display, "</color>");
				}
				else
				{
					Text mRewardName = this.m_rewardName;
					WrapperItemStats value = itemStats.Value;
					mRewardName.text = string.Concat(GeneralHelpers.GetItemQualityColorTag(value.Quality), record.Display, "</color>");
				}
				this.m_rewardName.supportRichText = true;
				if (record.ItemNameDescriptionID > 0)
				{
					ItemNameDescriptionRec itemNameDescriptionRec = StaticDB.itemNameDescriptionDB.GetRecord(record.ItemNameDescriptionID);
					if (itemNameDescriptionRec != null)
					{
						Text text = this.m_rewardName;
						str = text.text;
						text.text = string.Concat(new string[] { str, "\n<color=#", GeneralHelpers.GetColorFromInt(itemNameDescriptionRec.Color), "ff>", itemNameDescriptionRec.Description, "</color>" });
					}
				}
				if (record.ClassID == 2 || record.ClassID == 3 || record.ClassID == 4 || record.ClassID == 5 || record.ClassID == 6)
				{
					int itemLevel = record.ItemLevel;
					if (itemStats.HasValue)
					{
						itemLevel = itemStats.Value.ItemLevel;
					}
					Text mRewardName1 = this.m_rewardName;
					str = mRewardName1.text;
					mRewardName1.text = string.Concat(new string[] { str, "\n<color=#", GeneralHelpers.s_defaultColor, ">", StaticDB.GetString("ITEM_LEVEL", null), " ", itemLevel.ToString(), "</color>" });
				}
				if (record.Bonding > 0)
				{
					string empty = string.Empty;
					if ((record.Flags[0] & 134217728) != 0)
					{
						empty = ((record.Flags[1] & 131072) == 0 ? StaticDB.GetString("ITEM_BIND_TO_ACCOUNT", null) : StaticDB.GetString("ITEM_BIND_TO_BNETACCOUNT", null));
					}
					else if (record.Bonding == 1)
					{
						empty = StaticDB.GetString("ITEM_BIND_ON_PICKUP", null);
					}
					else if (record.Bonding == 4)
					{
						empty = StaticDB.GetString("ITEM_BIND_QUEST", null);
					}
					else if (record.Bonding == 2)
					{
						empty = StaticDB.GetString("ITEM_BIND_ON_EQUIP", null);
					}
					else if (record.Bonding == 3)
					{
						empty = StaticDB.GetString("ITEM_BIND_ON_USE", null);
					}
					if (empty != string.Empty)
					{
						Text text1 = this.m_rewardName;
						str = text1.text;
						text1.text = string.Concat(new string[] { str, "\n<color=#", GeneralHelpers.s_normalColor, ">", empty, "</color>" });
					}
				}
				ItemSubClassRec itemSubclass = StaticDB.GetItemSubclass(record.ClassID, record.SubclassID);
				if (itemSubclass != null && itemSubclass.DisplayName != null && itemSubclass.DisplayName != string.Empty && (itemSubclass.DisplayFlags & 1) == 0 && record.InventoryType != 16)
				{
					if (this.m_rewardDescription.text != string.Empty)
					{
						Text mRewardDescription = this.m_rewardDescription;
						mRewardDescription.text = string.Concat(mRewardDescription.text, "\n");
					}
					Text mRewardDescription1 = this.m_rewardDescription;
					str = mRewardDescription1.text;
					mRewardDescription1.text = string.Concat(new string[] { str, "<color=#", GeneralHelpers.s_normalColor, ">", itemSubclass.DisplayName, "</color>" });
				}
				string inventoryTypeString = GeneralHelpers.GetInventoryTypeString((INVENTORY_TYPE)record.InventoryType);
				if (inventoryTypeString != null && inventoryTypeString != string.Empty)
				{
					if (this.m_rewardDescription.text != string.Empty)
					{
						Text mRewardDescription2 = this.m_rewardDescription;
						mRewardDescription2.text = string.Concat(mRewardDescription2.text, "\n");
					}
					Text text2 = this.m_rewardDescription;
					str = text2.text;
					text2.text = string.Concat(new string[] { str, "<color=#", GeneralHelpers.s_normalColor, ">", inventoryTypeString, "</color>" });
				}
				if (itemStats.HasValue)
				{
					if (itemStats.Value.MinDamage != 0 || itemStats.Value.MaxDamage != 0)
					{
						if (this.m_rewardDescription.text != string.Empty)
						{
							Text mRewardDescription3 = this.m_rewardDescription;
							mRewardDescription3.text = string.Concat(mRewardDescription3.text, "\n");
						}
						if (itemStats.Value.MinDamage != itemStats.Value.MaxDamage)
						{
							Text text3 = this.m_rewardDescription;
							string str2 = text3.text;
							string str3 = itemStats.Value.MinDamage.ToString();
							WrapperItemStats wrapperItemStat = itemStats.Value;
							text3.text = string.Concat(str2, GeneralHelpers.TextOrderString(string.Concat(str3, " - ", wrapperItemStat.MaxDamage.ToString()), StaticDB.GetString("DAMAGE", null)));
						}
						else
						{
							Text mRewardDescription4 = this.m_rewardDescription;
							string str4 = mRewardDescription4.text;
							WrapperItemStats value1 = itemStats.Value;
							mRewardDescription4.text = string.Concat(str4, GeneralHelpers.TextOrderString(value1.MinDamage.ToString(), StaticDB.GetString("DAMAGE", null)));
						}
					}
					if (itemStats.Value.EffectiveArmor > 0)
					{
						if (this.m_rewardDescription.text != string.Empty)
						{
							Text text4 = this.m_rewardDescription;
							text4.text = string.Concat(text4.text, "\n");
						}
						Text mRewardDescription5 = this.m_rewardDescription;
						str = mRewardDescription5.text;
						string[] sNormalColor = new string[] { str, "<color=#", GeneralHelpers.s_normalColor, ">", null, null };
						WrapperItemStats wrapperItemStat1 = itemStats.Value;
						sNormalColor[4] = GeneralHelpers.TextOrderString(wrapperItemStat1.EffectiveArmor.ToString(), StaticDB.GetString("ARMOR", null));
						sNormalColor[5] = "</color>";
						mRewardDescription5.text = string.Concat(sNormalColor);
					}
					foreach (WrapperItemBonusStat bonusStat in itemStats.Value.BonusStats)
					{
						if (bonusStat.BonusAmount == 0)
						{
							continue;
						}
						if (this.m_rewardDescription.text != string.Empty)
						{
							Text text5 = this.m_rewardDescription;
							text5.text = string.Concat(text5.text, "\n");
						}
						Text mRewardDescription6 = this.m_rewardDescription;
						mRewardDescription6.text = string.Concat(mRewardDescription6.text, "<color=#", GeneralHelpers.GetMobileStatColorString(bonusStat.Color), ">");
						str1 = (bonusStat.BonusAmount <= 0 ? "-" : "+");
						Text text6 = this.m_rewardDescription;
						text6.text = string.Concat(text6.text, GeneralHelpers.TextOrderString(string.Concat(str1, bonusStat.BonusAmount.ToString()), GeneralHelpers.GetBonusStatString((BonusStatIndex)bonusStat.StatID)), "</color>");
					}
				}
				int requiredLevel = record.RequiredLevel;
				if (itemStats.HasValue)
				{
					requiredLevel = itemStats.Value.RequiredLevel;
				}
				if (requiredLevel > 1)
				{
					if (this.m_rewardDescription.text != string.Empty)
					{
						Text mRewardDescription7 = this.m_rewardDescription;
						mRewardDescription7.text = string.Concat(mRewardDescription7.text, "\n");
					}
					string mobileStatColorString = GeneralHelpers.s_normalColor;
					if (GarrisonStatus.CharacterLevel() < requiredLevel)
					{
						mobileStatColorString = GeneralHelpers.GetMobileStatColorString(WrapperStatColor.MOBILE_STAT_COLOR_ERROR);
					}
					Text text7 = this.m_rewardDescription;
					str = text7.text;
					text7.text = string.Concat(new object[] { str, "<color=#", mobileStatColorString, ">", StaticDB.GetString("ITEM_MIN_LEVEL", null), " ", requiredLevel, "</color>" });
				}
				string itemDescription = GeneralHelpers.GetItemDescription(record);
				if (itemDescription != null && itemDescription != string.Empty)
				{
					if (this.m_rewardDescription.text != string.Empty)
					{
						Text mRewardDescription8 = this.m_rewardDescription;
						mRewardDescription8.text = string.Concat(mRewardDescription8.text, "\n");
					}
					Text text8 = this.m_rewardDescription;
					text8.text = string.Concat(text8.text, itemDescription);
				}
				else if (!itemStats.HasValue)
				{
					if (this.m_rewardDescription.text != string.Empty)
					{
						Text mRewardDescription9 = this.m_rewardDescription;
						mRewardDescription9.text = string.Concat(mRewardDescription9.text, "\n");
					}
					Text text9 = this.m_rewardDescription;
					text9.text = string.Concat(text9.text, "...");
				}
			}
		}

		public void SetReward(MissionRewardDisplay.RewardType rewardType, int rewardID, int rewardQuantity, Sprite rewardSprite, int itemContext)
		{
			this.m_rewardType = rewardType;
			this.m_rewardID = rewardID;
			switch (rewardType)
			{
				case MissionRewardDisplay.RewardType.item:
				{
					ItemStatCache.instance.ItemStatCacheUpdateAction += new Action<int, int, WrapperItemStats>(this.ItemStatsUpdated);
					this.SetItem(rewardID, itemContext, rewardSprite);
					break;
				}
				case MissionRewardDisplay.RewardType.gold:
				{
					this.SetGold(rewardQuantity, rewardSprite);
					break;
				}
				case MissionRewardDisplay.RewardType.followerXP:
				{
					this.SetFollowerXP(rewardQuantity, rewardSprite);
					break;
				}
				case MissionRewardDisplay.RewardType.currency:
				{
					this.SetCurrency(rewardID, rewardQuantity, rewardSprite);
					break;
				}
				case MissionRewardDisplay.RewardType.faction:
				{
					this.SetFaction(rewardID, rewardQuantity, rewardSprite);
					break;
				}
			}
		}

		private void Start()
		{
			this.m_rewardName.font = GeneralHelpers.LoadStandardFont();
			this.m_rewardDescription.font = GeneralHelpers.LoadStandardFont();
			this.m_rewardQuantity.font = GeneralHelpers.LoadStandardFont();
		}
	}
}