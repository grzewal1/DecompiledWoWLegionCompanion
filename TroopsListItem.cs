using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowJamMessages.MobileClientJSON;
using WowJamMessages.MobilePlayerJSON;
using WowStatConstants;
using WowStaticData;

public class TroopsListItem : MonoBehaviour
{
	public GameObject m_troopSpecificArea;

	public GameObject m_itemSpecificArea;

	public LayoutElement m_rightStackLayoutElement;

	public Text m_akHintText;

	public Image m_troopSnapshotImage;

	public GameObject m_troopHeartContainer;

	public GameObject m_troopHeartPrefab;

	public Text m_troopName;

	public GameObject m_traitsAndAbilitiesRootObject;

	public GameObject m_abilityDisplayPrefab;

	public GameObject m_troopSlotsRootObject;

	public CanvasGroup m_troopSlotsCanvasGroup;

	public GameObject m_troopSlotPrefab;

	public Image m_troopResourceIcon;

	public Text m_troopResourceCostText;

	public Image m_itemResourceIcon;

	public Text m_itemResourceCostText;

	public Button m_recruitTroopsButton;

	public Text m_recruitButtonText;

	public Text m_itemName;

	public MissionRewardDisplay m_itemDisplay;

	public GameObject m_lootDisplayArea;

	public GameObject m_lootItemArea;

	public MissionRewardDisplay m_rewardDisplayPrefab;

	public Text m_youReceivedLoot;

	public Text m_artifactKnowledgeLevelIncreasedLabel;

	public MissionRewardDisplay m_artifactResearchNotesDisplayPrefab;

	private bool m_isTroop;

	private int m_shipmentCost;

	private GarrFollowerRec m_followerRec;

	private CharShipmentRec m_charShipmentRec;

	private bool m_isArtifactResearch;

	private int m_akLevelBefore;

	private bool m_akResearchDisabled;

	public TroopsListItem()
	{
	}

	public void AddInventoryItems()
	{
		if (this.m_isArtifactResearch)
		{
			ItemRec record = StaticDB.itemDB.GetRecord(this.m_charShipmentRec.DummyItemID);
			if (record == null)
			{
				return;
			}
			MissionRewardDisplay[] componentsInChildren = this.m_lootItemArea.GetComponentsInChildren<MissionRewardDisplay>();
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
			}
			for (int j = 0; j < ArtifactKnowledgeData.s_artifactKnowledgeInfo.ItemsInBags; j++)
			{
				MissionRewardDisplay missionRewardDisplay = UnityEngine.Object.Instantiate<MissionRewardDisplay>(this.m_artifactResearchNotesDisplayPrefab);
				missionRewardDisplay.transform.SetParent(this.m_lootItemArea.transform, false);
				missionRewardDisplay.InitReward(MissionRewardDisplay.RewardType.item, record.ID, 1, 0, record.IconFileDataID);
				UiAnimMgr.instance.PlayAnim("ItemReadyToUseGlowLoop", missionRewardDisplay.transform, Vector3.zero, 1.2f, 0f);
			}
			this.m_lootDisplayArea.SetActive(ArtifactKnowledgeData.s_artifactKnowledgeInfo.ItemsInBags > 0);
		}
	}

	private void Awake()
	{
		this.ClearAndHideLootArea();
		this.m_akLevelBefore = 0;
		if (ArtifactKnowledgeData.s_artifactKnowledgeInfo != null)
		{
			this.m_akLevelBefore = ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel;
		}
	}

	public void ClearAndHideLootArea()
	{
		MissionRewardDisplay[] componentsInChildren = this.m_lootItemArea.GetComponentsInChildren<MissionRewardDisplay>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyObject(componentsInChildren[i].gameObject);
		}
		this.m_lootDisplayArea.SetActive(false);
	}

	public int GetCharShipmentTypeID()
	{
		if (this.m_charShipmentRec == null)
		{
			return 0;
		}
		return this.m_charShipmentRec.ID;
	}

	private string GetCurrentArtifactPowerText()
	{
		if (ArtifactKnowledgeData.s_artifactKnowledgeInfo == null)
		{
			return string.Empty;
		}
		return string.Concat(new object[] { " <color=#ffffffff>(", StaticDB.GetString("LVL", "Lvl"), " ", ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel, "/", ArtifactKnowledgeData.s_artifactKnowledgeInfo.MaxLevel, ")</color>" });
	}

	private int GetMaxTroops(int garrClassSpecID)
	{
		GarrClassSpecRec record = StaticDB.garrClassSpecDB.GetRecord(garrClassSpecID);
		int followerClassLimit = 0;
		if (record != null)
		{
			followerClassLimit = (int)record.FollowerClassLimit;
		}
		IEnumerator enumerator = PersistentTalentData.talentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamGarrisonTalent current = (JamGarrisonTalent)enumerator.Current;
				if ((current.Flags & 1) == 0)
				{
					continue;
				}
				GarrTalentRec garrTalentRec = StaticDB.garrTalentDB.GetRecord(current.GarrTalentID);
				if (garrTalentRec == null)
				{
					continue;
				}
				StaticDB.garrAbilityEffectDB.EnumRecordsByParentID((int)garrTalentRec.GarrAbilityID, (GarrAbilityEffectRec effectRec) => {
					if (effectRec.AbilityAction == 34 && (ulong)effectRec.ActionRecordID == (long)garrClassSpecID)
					{
						followerClassLimit += (int)effectRec.ActionValueFlat;
					}
					return true;
				});
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			IDisposable disposable1 = disposable;
			if (disposable != null)
			{
				disposable1.Dispose();
			}
		}
		return followerClassLimit;
	}

	private void HandleArtifactKnowledgeInfoAboutToChange()
	{
		this.m_akLevelBefore = ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel;
	}

	private void HandleArtifactKnowledgeInfoChanged()
	{
		if (!this.m_isArtifactResearch)
		{
			return;
		}
		ItemRec record = StaticDB.itemDB.GetRecord(this.m_charShipmentRec.DummyItemID);
		this.m_itemName.text = string.Concat(record.Display, this.GetCurrentArtifactPowerText());
		this.ClearAndHideLootArea();
		this.AddInventoryItems();
		this.UpdateAKStatus();
		this.UpdateItemSlots();
		this.UpdateRecruitButtonState();
		this.AddInventoryItems();
		if (ArtifactKnowledgeData.s_artifactKnowledgeInfo == null || this.m_akLevelBefore == ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel)
		{
			this.m_artifactKnowledgeLevelIncreasedLabel.gameObject.SetActive(false);
		}
		else
		{
			this.m_artifactKnowledgeLevelIncreasedLabel.gameObject.SetActive(true);
			this.m_artifactKnowledgeLevelIncreasedLabel.text = string.Concat(new object[] { StaticDB.GetString("ARTIFACT_KNOWLEDGE_INCREASED_TO", "Artifact Knowledge Increased to"), " ", StaticDB.GetString("LVL", "LvL"), " ", ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel });
		}
	}

	public void HandleFollowerDataChanged()
	{
		if (this.m_charShipmentRec == null)
		{
			return;
		}
		if (!this.m_isTroop)
		{
			this.UpdateItemSlots();
		}
		else
		{
			this.UpdateTroopSlots();
		}
		this.UpdateRecruitButtonState();
	}

	private void HandleShipmentAdded(int charShipmentID, ulong shipmentDBID)
	{
		if (charShipmentID == this.m_charShipmentRec.ID)
		{
			TroopSlot[] componentsInChildren = this.m_troopSlotsRootObject.GetComponentsInChildren<TroopSlot>(true);
			TroopSlot[] troopSlotArray = componentsInChildren;
			for (int i = 0; i < (int)troopSlotArray.Length; i++)
			{
				if (troopSlotArray[i].GetDBID() == shipmentDBID)
				{
					return;
				}
			}
			TroopSlot[] troopSlotArray1 = componentsInChildren;
			for (int j = 0; j < (int)troopSlotArray1.Length; j++)
			{
				TroopSlot troopSlot = troopSlotArray1[j];
				if (troopSlot.IsPendingCreate())
				{
					troopSlot.SetCharShipment(charShipmentID, shipmentDBID, 0, true, 0);
					this.UpdateAKStatus();
					this.UpdateRecruitButtonState();
					return;
				}
			}
			TroopSlot[] troopSlotArray2 = componentsInChildren;
			for (int k = 0; k < (int)troopSlotArray2.Length; k++)
			{
				TroopSlot troopSlot1 = troopSlotArray2[k];
				if (troopSlot1.IsEmpty())
				{
					troopSlot1.SetCharShipment(charShipmentID, shipmentDBID, 0, true, 0);
					this.UpdateAKStatus();
					this.UpdateRecruitButtonState();
					return;
				}
			}
		}
	}

	public void HandleShipmentItemPushed(MobileClientShipmentItem item)
	{
		if (!this.m_itemResourceCostText.gameObject.activeSelf)
		{
			return;
		}
		if (!this.m_lootDisplayArea.activeSelf)
		{
			this.m_lootDisplayArea.SetActive(true);
		}
		int charShipmentTypeID = this.GetCharShipmentTypeID();
		MissionRewardDisplay missionRewardDisplay = null;
		if (this.m_isArtifactResearch)
		{
			this.HandleArtifactKnowledgeInfoAboutToChange();
			MobilePlayerRequestArtifactKnowledgeInfo mobilePlayerRequestArtifactKnowledgeInfo = new MobilePlayerRequestArtifactKnowledgeInfo();
			Login.instance.SendToMobileServer(mobilePlayerRequestArtifactKnowledgeInfo);
		}
		else if (charShipmentTypeID < 372 || charShipmentTypeID > 383)
		{
			missionRewardDisplay = UnityEngine.Object.Instantiate<MissionRewardDisplay>(this.m_rewardDisplayPrefab);
			missionRewardDisplay.transform.SetParent(this.m_lootItemArea.transform, false);
			missionRewardDisplay.InitReward(MissionRewardDisplay.RewardType.item, item.ItemID, item.Count, item.Context, item.IconFileDataID);
		}
		else
		{
			missionRewardDisplay = UnityEngine.Object.Instantiate<MissionRewardDisplay>(this.m_rewardDisplayPrefab);
			missionRewardDisplay.transform.SetParent(this.m_lootItemArea.transform, false);
			missionRewardDisplay.InitReward(MissionRewardDisplay.RewardType.currency, item.ItemID, item.Count, 0, 0);
		}
		if (missionRewardDisplay != null)
		{
			UiAnimMgr.instance.PlayAnim("MinimapPulseAnim", missionRewardDisplay.transform, Vector3.zero, 1.5f, 0f);
		}
	}

	private void OnDisable()
	{
		Main.instance.ShipmentAddedAction -= new Action<int, ulong>(this.HandleShipmentAdded);
		Main.instance.ArtifactKnowledgeInfoChangedAction -= new Action(this.HandleArtifactKnowledgeInfoChanged);
	}

	private void OnEnable()
	{
		Main.instance.ArtifactKnowledgeInfoChangedAction += new Action(this.HandleArtifactKnowledgeInfoChanged);
	}

	public void PlayClickSound()
	{
		Main.instance.m_UISound.Play_ButtonRedClick();
	}

	public void Recruit()
	{
		if (this.m_charShipmentRec.GarrFollowerID == 0)
		{
			TroopSlot troopSlot = null;
			TroopSlot[] componentsInChildren = this.m_troopSlotsRootObject.GetComponentsInChildren<TroopSlot>(true);
			int num = 0;
			while (num < (int)componentsInChildren.Length)
			{
				TroopSlot troopSlot1 = componentsInChildren[num];
				if (!troopSlot1.IsEmpty())
				{
					num++;
				}
				else
				{
					troopSlot = troopSlot1;
					break;
				}
			}
			if (troopSlot == null)
			{
				return;
			}
			troopSlot.SetPendingCreate();
			this.UpdateRecruitButtonState();
		}
		MobilePlayerCreateShipment mobilePlayerCreateShipment = new MobilePlayerCreateShipment()
		{
			CharShipmentID = this.m_charShipmentRec.ID,
			NumShipments = 1
		};
		Login.instance.SendToMobileServer(mobilePlayerCreateShipment);
		Main.instance.m_UISound.Play_RecruitTroop();
	}

	public void SetCharShipment(MobileClientShipmentType shipmentType, bool isSealOfFateHack = false, CharShipmentRec sealOfFateHackCharShipmentRec = null)
	{
		this.m_akHintText.gameObject.SetActive(false);
		if (!isSealOfFateHack)
		{
			this.m_shipmentCost = shipmentType.CurrencyCost;
		}
		else
		{
			this.m_shipmentCost = 0;
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
		AbilityDisplay[] abilityDisplayArray = this.m_traitsAndAbilitiesRootObject.GetComponentsInChildren<AbilityDisplay>(true);
		for (int j = 0; j < (int)abilityDisplayArray.Length; j++)
		{
			UnityEngine.Object.DestroyImmediate(abilityDisplayArray[j].gameObject);
		}
		TroopSlot[] troopSlotArray = this.m_troopSlotsRootObject.GetComponentsInChildren<TroopSlot>(true);
		for (int k = 0; k < (int)troopSlotArray.Length; k++)
		{
			UnityEngine.Object.DestroyImmediate(troopSlotArray[k].gameObject);
		}
		CharShipmentRec charShipmentRec = (!isSealOfFateHack ? StaticDB.charShipmentDB.GetRecord(shipmentType.CharShipmentID) : sealOfFateHackCharShipmentRec);
		if (charShipmentRec == null)
		{
			Debug.LogError(string.Concat("Invalid Shipment ID: ", shipmentType.CharShipmentID));
			this.m_troopName.text = string.Concat("Invalid Shipment ID: ", shipmentType.CharShipmentID);
			return;
		}
		if (charShipmentRec.GarrFollowerID > 0)
		{
			this.SetCharShipmentTroop(shipmentType, charShipmentRec);
		}
		else if (charShipmentRec.DummyItemID > 0)
		{
			this.SetCharShipmentItem(shipmentType, (!isSealOfFateHack ? charShipmentRec : sealOfFateHackCharShipmentRec), isSealOfFateHack);
		}
	}

	private void SetCharShipmentItem(MobileClientShipmentType shipmentType, CharShipmentRec charShipmentRec, bool isSealOfFateHack = false)
	{
		this.m_rightStackLayoutElement.minHeight = 120f;
		this.m_isTroop = false;
		this.m_charShipmentRec = charShipmentRec;
		this.m_troopSpecificArea.SetActive(false);
		this.m_itemSpecificArea.SetActive(true);
		this.m_troopName.gameObject.SetActive(false);
		this.m_itemName.gameObject.SetActive(true);
		ItemRec record = StaticDB.itemDB.GetRecord(charShipmentRec.DummyItemID);
		if (record == null)
		{
			Debug.LogError(string.Concat("Invalid Item ID: ", charShipmentRec.DummyItemID));
			this.m_troopName.text = string.Concat("Invalid Item ID: ", charShipmentRec.DummyItemID);
			return;
		}
		this.m_itemDisplay.InitReward(MissionRewardDisplay.RewardType.item, charShipmentRec.DummyItemID, 1, 0, record.IconFileDataID);
		this.m_isArtifactResearch = (record.ID == 139390 ? true : record.ID == 146745);
		this.m_itemName.text = string.Concat(record.Display, (!this.m_isArtifactResearch ? string.Empty : this.GetCurrentArtifactPowerText()));
		Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, record.IconFileDataID);
		if (sprite != null)
		{
			this.m_troopSnapshotImage.sprite = sprite;
		}
		this.m_itemResourceCostText.gameObject.SetActive(!isSealOfFateHack);
		this.m_itemResourceIcon.gameObject.SetActive(!isSealOfFateHack);
		if (!isSealOfFateHack)
		{
			this.m_itemResourceCostText.text = string.Concat(string.Empty, shipmentType.CurrencyCost);
			Sprite sprite1 = GeneralHelpers.LoadCurrencyIcon(shipmentType.CurrencyTypeID);
			if (sprite1 != null)
			{
				this.m_itemResourceIcon.sprite = sprite1;
			}
		}
		this.UpdateAKStatus();
		this.UpdateItemSlots();
		this.UpdateRecruitButtonState();
	}

	private void SetCharShipmentTroop(MobileClientShipmentType shipmentType, CharShipmentRec charShipmentRec)
	{
		this.m_rightStackLayoutElement.minHeight = 170f;
		this.m_isTroop = true;
		this.m_charShipmentRec = charShipmentRec;
		this.m_troopSpecificArea.SetActive(true);
		this.m_itemSpecificArea.SetActive(false);
		this.m_troopName.gameObject.SetActive(true);
		this.m_itemName.gameObject.SetActive(false);
		GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord((int)charShipmentRec.GarrFollowerID);
		if (record == null)
		{
			Debug.LogError(string.Concat("Invalid Follower ID: ", charShipmentRec.GarrFollowerID));
			this.m_troopName.text = string.Concat("Invalid Follower ID: ", charShipmentRec.GarrFollowerID);
			return;
		}
		this.m_followerRec = record;
		int num = (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? record.AllianceCreatureID : record.HordeCreatureID);
		CreatureRec creatureRec = StaticDB.creatureDB.GetRecord(num);
		if (creatureRec == null)
		{
			Debug.LogError(string.Concat("Invalid Creature ID: ", num));
			this.m_troopName.text = string.Concat("Invalid Creature ID: ", num);
			return;
		}
		int d = creatureRec.ID;
		string str = string.Concat("Assets/BundleAssets/PortraitIcons/cid_", d.ToString("D8"), ".png");
		Sprite sprite = AssetBundleManager.portraitIcons.LoadAsset<Sprite>(str);
		if (sprite != null)
		{
			this.m_troopSnapshotImage.sprite = sprite;
		}
		for (int i = 0; i < record.Vitality; i++)
		{
			GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_troopHeartPrefab);
			gameObject1.transform.SetParent(this.m_troopHeartContainer.transform, false);
		}
		this.m_troopName.text = creatureRec.Name;
		StaticDB.garrFollowerXAbilityDB.EnumRecordsByParentID((int)charShipmentRec.GarrFollowerID, (GarrFollowerXAbilityRec xAbilityRec) => {
			if (xAbilityRec.FactionIndex == (int)GarrisonStatus.Faction())
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_abilityDisplayPrefab);
				gameObject.transform.SetParent(this.m_traitsAndAbilitiesRootObject.transform, false);
				gameObject.GetComponent<AbilityDisplay>().SetAbility(xAbilityRec.GarrAbilityID, true, true, null);
			}
			return true;
		});
		this.UpdateTroopSlots();
		this.m_troopResourceCostText.text = string.Concat(string.Empty, shipmentType.CurrencyCost);
		Sprite sprite1 = GeneralHelpers.LoadCurrencyIcon(shipmentType.CurrencyTypeID);
		if (sprite1 != null)
		{
			this.m_troopResourceIcon.sprite = sprite1;
		}
		this.UpdateAKStatus();
		this.UpdateRecruitButtonState();
	}

	private void SetTroopSlotForExistingFollower(TroopSlot[] troopSlots, JamGarrisonFollower follower)
	{
		if (follower.Durability <= 0)
		{
			return;
		}
		TroopSlot[] troopSlotArray = troopSlots;
		for (int i = 0; i < (int)troopSlotArray.Length; i++)
		{
			int ownedFollowerID = troopSlotArray[i].GetOwnedFollowerID();
			if (ownedFollowerID != 0 && ownedFollowerID == follower.GarrFollowerID)
			{
				return;
			}
		}
		TroopSlot[] troopSlotArray1 = troopSlots;
		for (int j = 0; j < (int)troopSlotArray1.Length; j++)
		{
			TroopSlot troopSlot = troopSlotArray1[j];
			if (troopSlot.IsCollected())
			{
				GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(follower.GarrFollowerID);
				int num = (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? record.AllianceIconFileDataID : record.HordeIconFileDataID);
				troopSlot.SetCharShipment(this.m_charShipmentRec.ID, (ulong)0, follower.GarrFollowerID, false, num);
				return;
			}
		}
		TroopSlot[] troopSlotArray2 = troopSlots;
		for (int k = 0; k < (int)troopSlotArray2.Length; k++)
		{
			TroopSlot troopSlot1 = troopSlotArray2[k];
			if (troopSlot1.IsPendingCreate())
			{
				GarrFollowerRec garrFollowerRec = StaticDB.garrFollowerDB.GetRecord(follower.GarrFollowerID);
				int num1 = (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? garrFollowerRec.AllianceIconFileDataID : garrFollowerRec.HordeIconFileDataID);
				troopSlot1.SetCharShipment(this.m_charShipmentRec.ID, (ulong)0, follower.GarrFollowerID, false, num1);
				return;
			}
		}
		TroopSlot[] troopSlotArray3 = troopSlots;
		for (int l = 0; l < (int)troopSlotArray3.Length; l++)
		{
			TroopSlot troopSlot2 = troopSlotArray3[l];
			if (troopSlot2.IsEmpty())
			{
				GarrFollowerRec record1 = StaticDB.garrFollowerDB.GetRecord(follower.GarrFollowerID);
				int num2 = (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? record1.AllianceIconFileDataID : record1.HordeIconFileDataID);
				troopSlot2.SetCharShipment(this.m_charShipmentRec.ID, (ulong)0, follower.GarrFollowerID, false, num2);
				return;
			}
		}
	}

	private void SetTroopSlotForPendingShipment(TroopSlot[] troopSlots, ulong shipmentDBID)
	{
		TroopSlot[] troopSlotArray = troopSlots;
		for (int i = 0; i < (int)troopSlotArray.Length; i++)
		{
			if (troopSlotArray[i].GetDBID() == shipmentDBID)
			{
				return;
			}
		}
		TroopSlot[] troopSlotArray1 = troopSlots;
		for (int j = 0; j < (int)troopSlotArray1.Length; j++)
		{
			TroopSlot troopSlot = troopSlotArray1[j];
			if (troopSlot.IsPendingCreate())
			{
				troopSlot.SetCharShipment(this.m_charShipmentRec.ID, shipmentDBID, 0, true, 0);
				return;
			}
		}
		TroopSlot[] troopSlotArray2 = troopSlots;
		for (int k = 0; k < (int)troopSlotArray2.Length; k++)
		{
			TroopSlot troopSlot1 = troopSlotArray2[k];
			if (troopSlot1.IsEmpty())
			{
				troopSlot1.SetCharShipment(this.m_charShipmentRec.ID, shipmentDBID, 0, true, 0);
				return;
			}
		}
	}

	private void Start()
	{
		if (Main.instance.IsNarrowScreen())
		{
			if (this.m_troopSpecificArea != null)
			{
				Main.instance.NudgeX(ref this.m_troopSpecificArea, 30f);
			}
			if (this.m_itemSpecificArea != null)
			{
				Main.instance.NudgeX(ref this.m_itemSpecificArea, 30f);
			}
			if (this.m_troopSlotsRootObject != null)
			{
				GridLayoutGroup component = this.m_troopSlotsRootObject.GetComponent<GridLayoutGroup>();
				if (component != null)
				{
					Vector2 vector2 = component.spacing;
					vector2.x = 10f;
					component.spacing = vector2;
				}
			}
		}
		Main.instance.ShipmentAddedAction += new Action<int, ulong>(this.HandleShipmentAdded);
		this.m_troopName.font = GeneralHelpers.LoadStandardFont();
		this.m_troopResourceCostText.font = GeneralHelpers.LoadStandardFont();
		this.m_itemResourceCostText.font = GeneralHelpers.LoadStandardFont();
		this.m_recruitButtonText.font = GeneralHelpers.LoadStandardFont();
		this.m_itemName.font = GeneralHelpers.LoadStandardFont();
		this.m_youReceivedLoot.font = GeneralHelpers.LoadStandardFont();
		if (!this.m_isArtifactResearch)
		{
			this.m_youReceivedLoot.text = StaticDB.GetString("YOU_RECEIVED_LOOT", "You received loot:");
		}
		else
		{
			this.m_youReceivedLoot.text = StaticDB.GetString("USABLE_ITEMS", "Usable Items: (PH)");
		}
		this.m_akHintText.font = GeneralHelpers.LoadStandardFont();
		this.m_artifactKnowledgeLevelIncreasedLabel.font = GeneralHelpers.LoadStandardFont();
		this.m_artifactKnowledgeLevelIncreasedLabel.gameObject.SetActive(false);
	}

	private void UpdateAKStatus()
	{
		if (!this.m_isArtifactResearch || ArtifactKnowledgeData.s_artifactKnowledgeInfo == null)
		{
			return;
		}
		this.m_akHintText.gameObject.SetActive(false);
		int num = 0;
		IEnumerator enumerator = PersistentShipmentData.shipmentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				if (((JamCharacterShipment)enumerator.Current).ShipmentRecID != this.m_charShipmentRec.ID)
				{
					continue;
				}
				num++;
				break;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			IDisposable disposable1 = disposable;
			if (disposable != null)
			{
				disposable1.Dispose();
			}
		}
		this.m_akResearchDisabled = true;
		if (ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel < 25)
		{
			this.m_akHintText.gameObject.SetActive(true);
			this.m_akHintText.text = GeneralHelpers.LimitZhLineLength(StaticDB.GetString("AK_BELOW_25_MSG", "Visit your Order Hall to increase your AK (PH)"), 14);
		}
		else if (ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel == 25)
		{
			this.m_akHintText.gameObject.SetActive(true);
			this.m_akHintText.text = GeneralHelpers.LimitZhLineLength(StaticDB.GetString("AK_AT_25_MSG", "Go see Khadgar (PH)"), 14);
		}
		else if (ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel + ArtifactKnowledgeData.s_artifactKnowledgeInfo.ActiveShipments + ArtifactKnowledgeData.s_artifactKnowledgeInfo.ItemsInBags + ArtifactKnowledgeData.s_artifactKnowledgeInfo.ItemsInBank + ArtifactKnowledgeData.s_artifactKnowledgeInfo.ItemsInMail < ArtifactKnowledgeData.s_artifactKnowledgeInfo.MaxLevel || num != 0)
		{
			this.m_akHintText.gameObject.SetActive(false);
			this.m_akResearchDisabled = false;
		}
		else
		{
			this.m_akHintText.gameObject.SetActive(true);
			this.m_akHintText.text = GeneralHelpers.LimitZhLineLength(StaticDB.GetString("AK_AT_MAX_MSG", "No more research available (PH)"), 14);
		}
		if (this.m_akResearchDisabled)
		{
			this.m_itemResourceCostText.gameObject.SetActive(false);
		}
	}

	private void UpdateItemSlots()
	{
		if (this.m_isArtifactResearch && this.m_akResearchDisabled)
		{
			TroopSlot[] componentsInChildren = this.m_troopSlotsRootObject.GetComponentsInChildren<TroopSlot>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
			}
			return;
		}
		bool flag = true;
		if (this.m_charShipmentRec != null && !PersistentShipmentData.CanPickupShipmentType(this.m_charShipmentRec.ID))
		{
			flag = false;
		}
		int num = 0;
		IEnumerator enumerator = PersistentShipmentData.shipmentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				if (((JamCharacterShipment)enumerator.Current).ShipmentRecID != this.m_charShipmentRec.ID)
				{
					continue;
				}
				num++;
				break;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			IDisposable disposable1 = disposable;
			if (disposable != null)
			{
				disposable1.Dispose();
			}
		}
		if ((num <= 0 || flag) && (!this.m_isArtifactResearch || ArtifactKnowledgeData.s_artifactKnowledgeInfo == null || ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel < ArtifactKnowledgeData.s_artifactKnowledgeInfo.MaxLevel))
		{
			this.m_troopSlotsCanvasGroup.alpha = 1f;
			this.m_troopSlotsCanvasGroup.interactable = true;
			this.m_troopSlotsCanvasGroup.blocksRaycasts = true;
		}
		else
		{
			this.m_troopSlotsCanvasGroup.alpha = 0f;
			this.m_troopSlotsCanvasGroup.interactable = false;
			this.m_troopSlotsCanvasGroup.blocksRaycasts = false;
		}
		int maxShipments = (int)this.m_charShipmentRec.MaxShipments;
		if (this.m_isArtifactResearch && ArtifactKnowledgeData.s_artifactKnowledgeInfo != null)
		{
			int maxLevel = ArtifactKnowledgeData.s_artifactKnowledgeInfo.MaxLevel - ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel;
			if (maxLevel > 0 && maxLevel < maxShipments)
			{
				maxShipments = maxLevel;
			}
		}
		TroopSlot[] troopSlotArray = this.m_troopSlotsRootObject.GetComponentsInChildren<TroopSlot>(true);
		if ((int)troopSlotArray.Length < maxShipments)
		{
			for (int j = (int)troopSlotArray.Length; j < maxShipments; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_troopSlotPrefab);
				gameObject.transform.SetParent(this.m_troopSlotsRootObject.transform, false);
				TroopSlot component = gameObject.GetComponent<TroopSlot>();
				component.SetCharShipment(this.m_charShipmentRec.ID, (ulong)0, 0, false, 0);
			}
		}
		if ((int)troopSlotArray.Length > maxShipments)
		{
			for (int k = maxShipments; k < (int)troopSlotArray.Length; k++)
			{
				UnityEngine.Object.DestroyImmediate(troopSlotArray[k].gameObject);
			}
		}
		troopSlotArray = this.m_troopSlotsRootObject.GetComponentsInChildren<TroopSlot>(true);
		TroopSlot[] troopSlotArray1 = troopSlotArray;
		for (int l = 0; l < (int)troopSlotArray1.Length; l++)
		{
			TroopSlot troopSlot = troopSlotArray1[l];
			if (troopSlot.GetDBID() != (long)0 && !PersistentShipmentData.shipmentDictionary.ContainsKey(troopSlot.GetDBID()))
			{
				troopSlot.SetCharShipment(this.m_charShipmentRec.ID, (ulong)0, 0, false, 0);
			}
		}
		IEnumerator enumerator1 = PersistentShipmentData.shipmentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator1.MoveNext())
			{
				JamCharacterShipment current = (JamCharacterShipment)enumerator1.Current;
				if (current.ShipmentRecID != this.m_charShipmentRec.ID)
				{
					continue;
				}
				this.SetTroopSlotForPendingShipment(troopSlotArray, current.ShipmentID);
			}
		}
		finally
		{
			IDisposable disposable2 = enumerator1 as IDisposable;
			IDisposable disposable3 = disposable2;
			if (disposable2 != null)
			{
				disposable3.Dispose();
			}
		}
	}

	private void UpdateRecruitButtonState()
	{
		bool flag = GarrisonStatus.Resources() >= this.m_shipmentCost;
		this.m_itemResourceCostText.color = (!flag ? Color.red : Color.white);
		bool flag1 = true;
		if (this.m_charShipmentRec != null && !PersistentShipmentData.CanOrderShipmentType(this.m_charShipmentRec.ID))
		{
			flag1 = false;
		}
		if (this.m_isArtifactResearch && ArtifactKnowledgeData.s_artifactKnowledgeInfo != null)
		{
			if (this.m_akResearchDisabled)
			{
				this.m_recruitTroopsButton.gameObject.SetActive(false);
			}
			if (!flag1 || ArtifactKnowledgeData.s_artifactKnowledgeInfo.CurrentLevel >= ArtifactKnowledgeData.s_artifactKnowledgeInfo.MaxLevel)
			{
				this.m_recruitButtonText.text = StaticDB.GetString("PLACE_ORDER", null);
				this.m_recruitButtonText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
				this.m_recruitTroopsButton.interactable = false;
				return;
			}
		}
		TroopSlot[] componentsInChildren = this.m_troopSlotsRootObject.GetComponentsInChildren<TroopSlot>(true);
		bool flag2 = false;
		TroopSlot[] troopSlotArray = componentsInChildren;
		int num = 0;
		while (num < (int)troopSlotArray.Length)
		{
			if (!troopSlotArray[num].IsEmpty())
			{
				num++;
			}
			else
			{
				flag2 = true;
				break;
			}
		}
		this.m_recruitButtonText.color = new Color(1f, 0.82f, 0f, 1f);
		if (!flag2)
		{
			this.m_recruitButtonText.text = StaticDB.GetString("SLOTS_FULL", null);
			this.m_recruitButtonText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		}
		else if (!flag)
		{
			this.m_recruitButtonText.text = StaticDB.GetString("CANT_AFFORD", "Can't Afford");
			this.m_recruitButtonText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		}
		else if (!this.m_isTroop)
		{
			this.m_recruitButtonText.text = StaticDB.GetString("PLACE_ORDER", null);
		}
		else
		{
			this.m_recruitButtonText.text = StaticDB.GetString("RECRUIT", null);
		}
		if (!flag2 || !flag || !flag1)
		{
			this.m_recruitTroopsButton.interactable = false;
			this.m_recruitButtonText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		}
		else
		{
			this.m_recruitTroopsButton.interactable = true;
		}
	}

	private void UpdateTroopSlots()
	{
		if (this.m_followerRec == null || this.m_charShipmentRec == null)
		{
			return;
		}
		int maxTroops = this.GetMaxTroops((GarrisonStatus.Faction() != PVP_FACTION.HORDE ? (int)this.m_followerRec.AllianceGarrClassSpecID : (int)this.m_followerRec.HordeGarrClassSpecID));
		TroopSlot[] componentsInChildren = this.m_troopSlotsRootObject.GetComponentsInChildren<TroopSlot>(true);
		if ((int)componentsInChildren.Length < maxTroops)
		{
			for (int i = (int)componentsInChildren.Length; i < maxTroops; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_troopSlotPrefab);
				gameObject.transform.SetParent(this.m_troopSlotsRootObject.transform, false);
				TroopSlot component = gameObject.GetComponent<TroopSlot>();
				component.SetCharShipment(this.m_charShipmentRec.ID, (ulong)0, 0, false, 0);
			}
		}
		if ((int)componentsInChildren.Length > maxTroops)
		{
			for (int j = maxTroops; j < (int)componentsInChildren.Length; j++)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[j].gameObject);
			}
		}
		componentsInChildren = this.m_troopSlotsRootObject.GetComponentsInChildren<TroopSlot>(true);
		TroopSlot[] troopSlotArray = componentsInChildren;
		for (int k = 0; k < (int)troopSlotArray.Length; k++)
		{
			TroopSlot troopSlot = troopSlotArray[k];
			int ownedFollowerID = troopSlot.GetOwnedFollowerID();
			if (ownedFollowerID != 0 && (!PersistentFollowerData.followerDictionary.ContainsKey(ownedFollowerID) || PersistentFollowerData.followerDictionary[ownedFollowerID].Durability == 0))
			{
				troopSlot.SetCharShipment(this.m_charShipmentRec.ID, (ulong)0, 0, false, 0);
			}
		}
		uint num = (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? this.m_followerRec.AllianceGarrClassSpecID : this.m_followerRec.HordeGarrClassSpecID);
		foreach (JamGarrisonFollower value in PersistentFollowerData.followerDictionary.Values)
		{
			GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(value.GarrFollowerID);
			if ((GarrisonStatus.Faction() != PVP_FACTION.HORDE ? record.AllianceGarrClassSpecID : record.HordeGarrClassSpecID) != num || value.Durability <= 0)
			{
				continue;
			}
			this.SetTroopSlotForExistingFollower(componentsInChildren, value);
		}
		CharShipmentRec charShipmentRec = StaticDB.charShipmentDB.GetRecord(this.m_charShipmentRec.ID);
		IEnumerator enumerator = PersistentShipmentData.shipmentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				JamCharacterShipment current = (JamCharacterShipment)enumerator.Current;
				if (current.ShipmentRecID != this.m_charShipmentRec.ID)
				{
					if (StaticDB.charShipmentDB.GetRecord(current.ShipmentRecID).ContainerID != charShipmentRec.ContainerID)
					{
						continue;
					}
					this.SetTroopSlotForPendingShipment(componentsInChildren, current.ShipmentID);
				}
				else
				{
					this.SetTroopSlotForPendingShipment(componentsInChildren, current.ShipmentID);
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			IDisposable disposable1 = disposable;
			if (disposable != null)
			{
				disposable1.Dispose();
			}
		}
	}
}