using System;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages.MobileClientJSON;
using WowStatConstants;
using WowStaticData;

public class AdventureMapWorldQuest : MonoBehaviour
{
	private const int WORLD_QUEST_TIME_LOW_MINUTES = 75;

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

	private int m_questID;

	private ITEM_QUALITY m_lootQuality;

	private long m_endTime;

	private int m_itemID;

	private int m_itemContext;

	public bool m_showLootIconInsteadOfMain;

	public int QuestID
	{
		get
		{
			return this.m_questID;
		}
	}

	private void OnEnable()
	{
		AdventureMapPanel expr_05 = AdventureMapPanel.instance;
		expr_05.TestIconSizeChanged = (Action<float>)Delegate.Combine(expr_05.TestIconSizeChanged, new Action<float>(this.OnTestIconSizeChanged));
		PinchZoomContentManager expr_30 = AdventureMapPanel.instance.m_pinchZoomContentManager;
		expr_30.ZoomFactorChanged = (Action)Delegate.Combine(expr_30.ZoomFactorChanged, new Action(this.HandleZoomChanged));
		this.m_showLootIconInsteadOfMain = true;
	}

	private void OnDisable()
	{
		AdventureMapPanel expr_05 = AdventureMapPanel.instance;
		expr_05.TestIconSizeChanged = (Action<float>)Delegate.Remove(expr_05.TestIconSizeChanged, new Action<float>(this.OnTestIconSizeChanged));
		PinchZoomContentManager expr_30 = AdventureMapPanel.instance.m_pinchZoomContentManager;
		expr_30.ZoomFactorChanged = (Action)Delegate.Remove(expr_30.ZoomFactorChanged, new Action(this.HandleZoomChanged));
	}

	private void ItemStatsUpdated(int itemID, int itemContext, MobileItemStats itemStats)
	{
		if (this.m_itemID == itemID && this.m_itemContext == itemContext)
		{
			ItemStatCache expr_1D = ItemStatCache.instance;
			expr_1D.ItemStatCacheUpdateAction = (Action<int, int, MobileItemStats>)Delegate.Remove(expr_1D.ItemStatCacheUpdateAction, new Action<int, int, MobileItemStats>(this.ItemStatsUpdated));
			this.ShowILVL();
		}
	}

	private void OnTestIconSizeChanged(float newScale)
	{
		base.get_transform().set_localScale(Vector3.get_one() * newScale);
	}

	private void HandleZoomChanged()
	{
		if (this.m_zoomScaleRoot != null)
		{
			this.m_zoomScaleRoot.get_transform().set_localScale(Vector3.get_one() * AdventureMapPanel.instance.m_pinchZoomContentManager.m_zoomFactor);
		}
	}

	private void ShowILVL()
	{
		ItemRec record = StaticDB.itemDB.GetRecord(this.m_itemID);
		if (record == null)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Invalid Item ID ",
				this.m_itemID,
				" from Quest ID ",
				this.m_questID,
				". Ignoring for showing iLevel on map."
			}));
			return;
		}
		if (AdventureMapPanel.instance.IsFilterEnabled(MapFilterType.Gear) && (record.ClassID == 2 || record.ClassID == 3 || record.ClassID == 4 || record.ClassID == 6))
		{
			MobileItemStats itemStats = ItemStatCache.instance.GetItemStats(this.m_itemID, this.m_itemContext);
			if (itemStats != null)
			{
				this.m_quantityArea.get_gameObject().SetActive(true);
				this.m_quantity.set_text(StaticDB.GetString("ILVL", null) + " " + itemStats.ItemLevel);
			}
			else
			{
				ItemStatCache expr_FF = ItemStatCache.instance;
				expr_FF.ItemStatCacheUpdateAction = (Action<int, int, MobileItemStats>)Delegate.Combine(expr_FF.ItemStatCacheUpdateAction, new Action<int, int, MobileItemStats>(this.ItemStatsUpdated));
			}
		}
	}

	public void SetQuestID(int questID)
	{
		this.m_questID = questID;
		base.get_gameObject().set_name("WorldQuest " + this.m_questID);
		MobileWorldQuest mobileWorldQuest = (MobileWorldQuest)WorldQuestData.worldQuestDictionary.get_Item(this.m_questID);
		if (mobileWorldQuest == null || mobileWorldQuest.Item == null)
		{
			return;
		}
		this.m_quantityArea.get_gameObject().SetActive(false);
		bool flag = false;
		MobileWorldQuestReward[] item = mobileWorldQuest.Item;
		for (int i = 0; i < item.Length; i++)
		{
			MobileWorldQuestReward mobileWorldQuestReward = item[i];
			ItemRec record = StaticDB.itemDB.GetRecord(mobileWorldQuestReward.RecordID);
			if (record == null)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Invalid Item ID ",
					mobileWorldQuestReward.RecordID,
					" from Quest ID ",
					this.m_questID,
					". Ignoring for loot quality check."
				}));
			}
			else
			{
				flag = true;
				if (record.OverallQualityID > (int)this.m_lootQuality)
				{
					this.m_lootQuality = (ITEM_QUALITY)record.OverallQualityID;
				}
				if (this.m_showLootIconInsteadOfMain)
				{
					bool isArtifactXP = false;
					int quantity = 0;
					StaticDB.itemEffectDB.EnumRecordsByParentID(mobileWorldQuestReward.RecordID, delegate(ItemEffectRec itemEffectRec)
					{
						StaticDB.spellEffectDB.EnumRecordsByParentID(itemEffectRec.SpellID, delegate(SpellEffectRec spellEffectRec)
						{
							if (spellEffectRec.Effect == 240)
							{
								isArtifactXP = true;
								quantity = GeneralHelpers.ApplyArtifactXPMultiplier(spellEffectRec.EffectBasePoints, GarrisonStatus.ArtifactXpMultiplier);
								return false;
							}
							return true;
						});
						return !isArtifactXP;
					});
					if (isArtifactXP)
					{
						this.m_main.set_sprite(Resources.Load<Sprite>("WorldMap/INV_Artifact_XP02"));
						if (AdventureMapPanel.instance.IsFilterEnabled(MapFilterType.ArtifactPower))
						{
							this.m_quantityArea.get_gameObject().SetActive(true);
							this.m_quantity.set_text(quantity.ToString());
						}
					}
					else
					{
						this.m_main.set_sprite(GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, mobileWorldQuestReward.FileDataID));
						this.m_itemID = mobileWorldQuestReward.RecordID;
						this.m_itemContext = mobileWorldQuestReward.ItemContext;
						this.ShowILVL();
					}
				}
			}
		}
		if (!flag && this.m_showLootIconInsteadOfMain)
		{
			if (mobileWorldQuest.Currency.GetLength(0) > 0)
			{
				MobileWorldQuestReward[] currency = mobileWorldQuest.Currency;
				for (int j = 0; j < currency.Length; j++)
				{
					MobileWorldQuestReward mobileWorldQuestReward2 = currency[j];
					CurrencyTypesRec record2 = StaticDB.currencyTypesDB.GetRecord(mobileWorldQuestReward2.RecordID);
					if (record2 != null)
					{
						this.m_main.set_sprite(GeneralHelpers.LoadCurrencyIcon(mobileWorldQuestReward2.RecordID));
					}
					if (AdventureMapPanel.instance.IsFilterEnabled(MapFilterType.OrderResources))
					{
						this.m_quantityArea.get_gameObject().SetActive(true);
						this.m_quantity.set_text(mobileWorldQuestReward2.Quantity.ToString());
					}
				}
			}
			else if (mobileWorldQuest.Money > 0)
			{
				this.m_main.set_sprite(Resources.Load<Sprite>("MiscIcons/INV_Misc_Coin_01"));
				if (AdventureMapPanel.instance.IsFilterEnabled(MapFilterType.Gold))
				{
					this.m_quantityArea.get_gameObject().SetActive(true);
					this.m_quantity.set_text(string.Empty + mobileWorldQuest.Money / 100 / 100);
				}
			}
			else if (mobileWorldQuest.Experience > 0)
			{
				this.m_main.set_sprite(GeneralHelpers.GetLocalizedFollowerXpIcon());
			}
		}
		this.m_endTime = (long)(mobileWorldQuest.EndTime - 900);
		int areaID = 0;
		WorldMapAreaRec record3 = StaticDB.worldMapAreaDB.GetRecord(mobileWorldQuest.WorldMapAreaID);
		if (record3 != null)
		{
			areaID = record3.AreaID;
		}
		this.m_areaID = areaID;
		QuestInfoRec record4 = StaticDB.questInfoDB.GetRecord(mobileWorldQuest.QuestInfoID);
		if (record4 == null)
		{
			return;
		}
		bool active = (record4.Modifiers & 2) != 0;
		this.m_dragonFrame.get_gameObject().SetActive(active);
		bool flag2 = (record4.Modifiers & 1) != 0;
		if (flag2 && record4.Type != 3)
		{
			this.m_background.set_sprite(Resources.Load<Sprite>("NewWorldQuest/Mobile-RareQuest"));
		}
		bool flag3 = (record4.Modifiers & 4) != 0;
		if (flag3 && record4.Type != 3)
		{
			this.m_background.set_sprite(Resources.Load<Sprite>("NewWorldQuest/Mobile-EpicQuest"));
		}
		int uITextureAtlasMemberID;
		string text;
		switch (record4.Type)
		{
		case 1:
		{
			int profession = record4.Profession;
			switch (profession)
			{
			case 182:
				uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-herbalism");
				text = "Mobile-Herbalism";
				goto IL_683;
			case 183:
			case 184:
				IL_46E:
				if (profession == 164)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-blacksmithing");
					text = "Mobile-Blacksmithing";
					goto IL_683;
				}
				if (profession == 165)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-leatherworking");
					text = "Mobile-Leatherworking";
					goto IL_683;
				}
				if (profession == 129)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-firstaid");
					text = "Mobile-FirstAid";
					goto IL_683;
				}
				if (profession == 171)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-alchemy");
					text = "Mobile-Alchemy";
					goto IL_683;
				}
				if (profession == 197)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-tailoring");
					text = "Mobile-Tailoring";
					goto IL_683;
				}
				if (profession == 202)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-engineering");
					text = "Mobile-Engineering";
					goto IL_683;
				}
				if (profession == 333)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-enchanting");
					text = "Mobile-Enchanting";
					goto IL_683;
				}
				if (profession == 356)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-fishing");
					text = "Mobile-Fishing";
					goto IL_683;
				}
				if (profession == 393)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-skinning");
					text = "Mobile-Skinning";
					goto IL_683;
				}
				if (profession == 755)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-jewelcrafting");
					text = "Mobile-Jewelcrafting";
					goto IL_683;
				}
				if (profession == 773)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-inscription");
					text = "Mobile-Inscription";
					goto IL_683;
				}
				if (profession != 794)
				{
					uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-questmarker-questbang");
					text = "Mobile-QuestExclamationIcon";
					goto IL_683;
				}
				uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-archaeology");
				text = "Mobile-Archaeology";
				goto IL_683;
			case 185:
				uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-cooking");
				text = "Mobile-Cooking";
				goto IL_683;
			case 186:
				uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-mining");
				text = "Mobile-Mining";
				goto IL_683;
			}
			goto IL_46E;
			IL_683:
			goto IL_6D0;
		}
		case 3:
			uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-pvp-ffa");
			text = "Mobile-PVP";
			goto IL_6D0;
		case 4:
			uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-icon-petbattle");
			text = "Mobile-Pets";
			goto IL_6D0;
		}
		uITextureAtlasMemberID = TextureAtlas.GetUITextureAtlasMemberID("worldquest-questmarker-questbang");
		text = "Mobile-QuestExclamationIcon";
		IL_6D0:
		if (!this.m_showLootIconInsteadOfMain)
		{
			if (text != null)
			{
				this.m_main.set_sprite(Resources.Load<Sprite>("NewWorldQuest/" + text));
			}
			else if (uITextureAtlasMemberID > 0)
			{
				this.m_main.set_sprite(TextureAtlas.instance.GetAtlasSprite(uITextureAtlasMemberID));
				this.m_main.SetNativeSize();
			}
		}
	}

	public void OnClick()
	{
		Main.instance.m_UISound.Play_SelectWorldQuest();
		UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", base.get_transform(), Vector3.get_zero(), 2f, 0f);
		AllPopups.instance.ShowWorldQuestTooltip(this.m_questID);
		StackableMapIcon component = base.GetComponent<StackableMapIcon>();
		if (component != null)
		{
			StackableMapIconContainer container = component.GetContainer();
			AdventureMapPanel.instance.SetSelectedIconContainer(container);
		}
	}

	private void Awake()
	{
		this.m_errorImage.get_gameObject().SetActive(false);
		this.m_dragonFrame.get_gameObject().SetActive(false);
		this.m_highlight.get_gameObject().SetActive(false);
		this.m_expiringSoon.get_gameObject().SetActive(false);
	}

	private void Update()
	{
		long num = this.m_endTime - GarrisonStatus.CurrentTime();
		bool active = num < 4500L;
		this.m_expiringSoon.get_gameObject().SetActive(active);
		if (num <= 0L)
		{
			Object.DestroyImmediate(base.get_gameObject());
			return;
		}
	}
}
