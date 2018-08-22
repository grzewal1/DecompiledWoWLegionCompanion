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
	public class AdventureMapWorldQuest : MonoBehaviour
	{
		public Image m_errorImage;

		public Image m_dragonFrame;

		public Image m_background;

		public Image m_main;

		public Image m_highlight;

		public Image m_expiringSoon;

		public int m_areaID;

		public GameObject m_zoomScaleRoot;

		public GameObject m_quantityArea;

		public Text m_quantity;

		public Image m_normalGlow;

		public Image m_legionAssaultGlow;

		private int m_questID;

		private ITEM_QUALITY m_lootQuality;

		private const int WORLD_QUEST_TIME_LOW_MINUTES = 75;

		private DateTime m_endTime;

		private int m_itemID;

		private int m_itemContext;

		private Color WORLD_QUEST_GLOW_COLOR_DEFAULT = new Color(255f, 210f, 0f);

		public bool m_showLootIconInsteadOfMain;

		public int QuestID
		{
			get
			{
				return this.m_questID;
			}
		}

		public AdventureMapWorldQuest()
		{
		}

		private void Awake()
		{
			this.m_errorImage.gameObject.SetActive(false);
			this.m_dragonFrame.gameObject.SetActive(false);
			this.m_highlight.gameObject.SetActive(false);
			this.m_expiringSoon.gameObject.SetActive(false);
		}

		public void ForceUpdate()
		{
			this.Update();
		}

		private void HandleZoomChanged(bool force)
		{
			if (this.m_zoomScaleRoot != null)
			{
				this.m_zoomScaleRoot.transform.localScale = Vector3.one * AdventureMapPanel.instance.m_pinchZoomContentManager.m_zoomFactor;
			}
		}

		private void ItemStatsUpdated(int itemID, int itemContext, WrapperItemStats itemStats)
		{
			if (this.m_itemID == itemID && this.m_itemContext == itemContext)
			{
				ItemStatCache.instance.ItemStatCacheUpdateAction -= new Action<int, int, WrapperItemStats>(this.ItemStatsUpdated);
				this.ShowILVL();
			}
		}

		public void OnClick()
		{
			Main.instance.m_UISound.Play_SelectWorldQuest();
			UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", base.transform, Vector3.zero, 2f, 0f);
			AllPopups.instance.ShowWorldQuestTooltip(this.m_questID);
			StackableMapIcon component = base.GetComponent<StackableMapIcon>();
			StackableMapIconContainer container = null;
			if (component != null)
			{
				container = component.GetContainer();
				AdventureMapPanel.instance.SetSelectedIconContainer(container);
			}
		}

		private void OnDisable()
		{
			AdventureMapPanel.instance.TestIconSizeChanged -= new Action<float>(this.OnTestIconSizeChanged);
			AdventureMapPanel.instance.m_pinchZoomContentManager.ZoomFactorChanged -= new Action<bool>(this.HandleZoomChanged);
		}

		private void OnEnable()
		{
			AdventureMapPanel.instance.TestIconSizeChanged += new Action<float>(this.OnTestIconSizeChanged);
			AdventureMapPanel.instance.m_pinchZoomContentManager.ZoomFactorChanged += new Action<bool>(this.HandleZoomChanged);
			this.m_showLootIconInsteadOfMain = true;
		}

		private void OnTestIconSizeChanged(float newScale)
		{
			base.transform.localScale = Vector3.one * newScale;
		}

		public void SetQuestID(int questID)
		{
			Color color;
			this.m_questID = questID;
			base.gameObject.name = string.Concat("WorldQuest ", this.m_questID);
			if (!WorldQuestData.WorldQuestDictionary.ContainsKey(this.m_questID))
			{
				return;
			}
			WrapperWorldQuest item = WorldQuestData.WorldQuestDictionary[this.m_questID];
			if (item.Items == null)
			{
				return;
			}
			this.m_quantityArea.gameObject.SetActive(false);
			bool flag = false;
			foreach (WrapperWorldQuestReward wrapperWorldQuestReward in item.Items)
			{
				ItemRec record = StaticDB.itemDB.GetRecord(wrapperWorldQuestReward.RecordID);
				if (record != null)
				{
					flag = true;
					if (record.OverallQualityID > (int)this.m_lootQuality)
					{
						this.m_lootQuality = (ITEM_QUALITY)record.OverallQualityID;
					}
					if (!this.m_showLootIconInsteadOfMain)
					{
						continue;
					}
					StaticDB.itemEffectDB.GetRecordsByParentID(wrapperWorldQuestReward.RecordID).SelectMany<ItemEffectRec, SpellEffectRec>((ItemEffectRec itemEffectRec) => StaticDB.spellEffectDB.GetRecordsByParentID(itemEffectRec.SpellID)).FirstOrDefault<SpellEffectRec>((SpellEffectRec spellEffectRec) => spellEffectRec.Effect == 240);
					this.m_main.sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, wrapperWorldQuestReward.FileDataID);
					this.m_itemID = wrapperWorldQuestReward.RecordID;
					this.m_itemContext = wrapperWorldQuestReward.ItemContext;
					this.ShowILVL();
				}
				else
				{
					Debug.LogWarning(string.Concat(new object[] { "Invalid Item ID ", wrapperWorldQuestReward.RecordID, " from Quest ID ", this.m_questID, ". Ignoring for loot quality check." }));
				}
			}
			if (!flag && this.m_showLootIconInsteadOfMain)
			{
				if (item.Currencies.Count > 0)
				{
					foreach (WrapperWorldQuestReward currency in item.Currencies)
					{
						if (StaticDB.currencyTypesDB.GetRecord(currency.RecordID) != null)
						{
							this.m_main.sprite = CurrencyContainerDB.LoadCurrencyContainerIcon(currency.RecordID, currency.Quantity);
							CurrencyContainerRec currencyContainerRec = CurrencyContainerDB.CheckAndGetValidCurrencyContainer(currency.RecordID, currency.Quantity);
							if (currencyContainerRec != null)
							{
								this.m_lootQuality = (ITEM_QUALITY)currencyContainerRec.ContainerQuality;
							}
						}
						if (!AdventureMapPanel.instance.IsFilterEnabled(MapFilterType.OrderResources))
						{
							continue;
						}
						this.m_quantityArea.gameObject.SetActive(true);
						this.m_quantity.text = currency.Quantity.ToString();
					}
				}
				else if (item.Money > 0)
				{
					this.m_main.sprite = Resources.Load<Sprite>("MiscIcons/INV_Misc_Coin_01");
					if (AdventureMapPanel.instance.IsFilterEnabled(MapFilterType.Gold))
					{
						this.m_quantityArea.gameObject.SetActive(true);
						this.m_quantity.text = string.Concat(string.Empty, item.Money / 100 / 100);
					}
				}
				else if (item.Experience > 0)
				{
					this.m_main.sprite = GeneralHelpers.GetLocalizedFollowerXpIcon();
				}
			}
			this.m_endTime = item.EndTime;
			int areaID = 0;
			WorldMapAreaRec worldMapAreaRec = StaticDB.worldMapAreaDB.GetRecord(item.WorldMapAreaID);
			if (worldMapAreaRec != null)
			{
				areaID = worldMapAreaRec.AreaID;
			}
			this.m_areaID = areaID;
			QuestInfoRec questInfoRec = StaticDB.questInfoDB.GetRecord(item.QuestInfoID);
			if (questInfoRec == null)
			{
				return;
			}
			bool modifiers = (questInfoRec.Modifiers & 2) != 0;
			this.m_dragonFrame.gameObject.SetActive(modifiers);
			bool type = questInfoRec.Type == 7;
			this.m_normalGlow.gameObject.SetActive(!type);
			if (this.m_lootQuality < ITEM_QUALITY.STANDARD)
			{
				this.m_normalGlow.color = this.WORLD_QUEST_GLOW_COLOR_DEFAULT;
			}
			if (this.m_lootQuality > ITEM_QUALITY.STANDARD && ColorUtility.TryParseHtmlString(string.Concat("#", GeneralHelpers.GetItemQualityColor((int)this.m_lootQuality)), out color))
			{
				this.m_normalGlow.color = color;
			}
			this.m_legionAssaultGlow.gameObject.SetActive(type);
			if ((questInfoRec.Modifiers & 1) != 0 && questInfoRec.Type != 3)
			{
				this.m_background.sprite = Resources.Load<Sprite>("NewWorldQuest/Mobile-RareQuest");
			}
			if ((questInfoRec.Modifiers & 4) != 0 && questInfoRec.Type != 3)
			{
				this.m_background.sprite = Resources.Load<Sprite>("NewWorldQuest/Mobile-EpicQuest");
			}
			int uITextureAtlasMemberID = 0;
			string str = null;
			switch (questInfoRec.Type)
			{
				case 1:
				{
					int profession = questInfoRec.Profession;
					switch (profession)
					{
						case 182:
						{
							uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-herbalism");
							str = "Mobile-Herbalism";
							break;
						}
						case 185:
						{
							uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-cooking");
							str = "Mobile-Cooking";
							break;
						}
						case 186:
						{
							uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-mining");
							str = "Mobile-Mining";
							break;
						}
						default:
						{
							if (profession == 164)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-blacksmithing");
								str = "Mobile-Blacksmithing";
								break;
							}
							else if (profession == 165)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-leatherworking");
								str = "Mobile-Leatherworking";
								break;
							}
							else if (profession == 129)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-firstaid");
								str = "Mobile-FirstAid";
								break;
							}
							else if (profession == 171)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-alchemy");
								str = "Mobile-Alchemy";
								break;
							}
							else if (profession == 197)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-tailoring");
								str = "Mobile-Tailoring";
								break;
							}
							else if (profession == 202)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-engineering");
								str = "Mobile-Engineering";
								break;
							}
							else if (profession == 333)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-enchanting");
								str = "Mobile-Enchanting";
								break;
							}
							else if (profession == 356)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-fishing");
								str = "Mobile-Fishing";
								break;
							}
							else if (profession == 393)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-skinning");
								str = "Mobile-Skinning";
								break;
							}
							else if (profession == 755)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-jewelcrafting");
								str = "Mobile-Jewelcrafting";
								break;
							}
							else if (profession == 773)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-inscription");
								str = "Mobile-Inscription";
								break;
							}
							else if (profession == 794)
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-archaeology");
								str = "Mobile-Archaeology";
								break;
							}
							else
							{
								uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-questmarker-questbang");
								str = "Mobile-QuestExclamationIcon";
								break;
							}
						}
					}
					break;
				}
				case 2:
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-questmarker-questbang");
					str = "Mobile-QuestExclamationIcon";
					break;
				}
				case 3:
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-pvp-ffa");
					str = "Mobile-PVP";
					break;
				}
				case 4:
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-petbattle");
					str = "Mobile-Pets";
					break;
				}
				default:
				{
					goto case 2;
				}
			}
			if (!this.m_showLootIconInsteadOfMain)
			{
				if (str != null)
				{
					this.m_main.sprite = Resources.Load<Sprite>(string.Concat("NewWorldQuest/", str));
				}
				else if (uITextureAtlasMemberID > 0)
				{
					this.m_main.sprite = TextureAtlas.instance.GetAtlasSprite(uITextureAtlasMemberID);
					this.m_main.SetNativeSize();
				}
			}
		}

		private void ShowILVL()
		{
			ItemRec record = StaticDB.itemDB.GetRecord(this.m_itemID);
			if (record == null)
			{
				Debug.LogWarning(string.Concat(new object[] { "Invalid Item ID ", this.m_itemID, " from Quest ID ", this.m_questID, ". Ignoring for showing iLevel on map." }));
				return;
			}
			if (AdventureMapPanel.instance.IsFilterEnabled(MapFilterType.Gear) && (record.ClassID == 2 || record.ClassID == 3 || record.ClassID == 4 || record.ClassID == 6))
			{
				WrapperItemStats? itemStats = ItemStatCache.instance.GetItemStats(this.m_itemID, this.m_itemContext);
				if (!itemStats.HasValue)
				{
					ItemStatCache.instance.ItemStatCacheUpdateAction += new Action<int, int, WrapperItemStats>(this.ItemStatsUpdated);
				}
				else
				{
					this.m_quantityArea.gameObject.SetActive(true);
					Text mQuantity = this.m_quantity;
					string str = StaticDB.GetString("ILVL", null);
					WrapperItemStats value = itemStats.Value;
					mQuantity.text = string.Concat(str, " ", value.ItemLevel);
				}
			}
		}

		private void Update()
		{
			TimeSpan mEndTime = this.m_endTime - GarrisonStatus.CurrentTime();
			this.m_expiringSoon.gameObject.SetActive(mEndTime.TotalSeconds < 4500);
			if (mEndTime.TotalSeconds <= 0)
			{
				StackableMapIcon component = base.gameObject.GetComponent<StackableMapIcon>();
				GameObject gameObject = base.gameObject;
				if (component != null)
				{
					component.RemoveFromContainer();
				}
				if (gameObject != null)
				{
					UnityEngine.Object.Destroy(gameObject);
					return;
				}
			}
		}
	}
}