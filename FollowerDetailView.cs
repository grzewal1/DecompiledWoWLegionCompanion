using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages;
using WowJamMessages.MobilePlayerJSON;
using WowStatConstants;
using WowStaticData;

public class FollowerDetailView : MonoBehaviour
{
	[Header("Spec and Abilities")]
	public GameObject traitsAndAbilitiesRootObject;

	public GameObject specializationHeaderPrefab;

	public GameObject abilitiesHeaderPrefab;

	public GameObject zoneSupportAbilityHeaderPrefab;

	public GameObject abilityDisplayPrefab;

	public GameObject m_spellDisplayPrefab;

	public GameObject m_equipmentSlotPrefab;

	[Header("Follower Snapshot")]
	public Image followerSnapshot;

	[Header("Equipment Slots")]
	public GameObject m_equipmentSlotsRootObject;

	[Header("Misc")]
	public FollowerListView m_followerListView;

	public FollowerInventoryListView m_followerInventoryListView;

	[Header("More Text")]
	public Text m_equipmentSlotsText;

	[Header("Champion Only")]
	public GameObject m_activateChampionButton;

	public GameObject m_deactivateChampionButton;

	[Header("Troops Only")]
	public Text m_troopDescriptionPrefab;

	private int m_garrFollowerID;

	public FollowerDetailView()
	{
	}

	public void ActivateFollower()
	{
		Main.instance.m_UISound.Play_ActivateChampion();
		Debug.Log(string.Concat("Attempting to Activate follower ", this.m_garrFollowerID));
		MobilePlayerChangeFollowerActive mobilePlayerChangeFollowerActive = new MobilePlayerChangeFollowerActive()
		{
			SetInactive = false,
			GarrFollowerID = this.m_garrFollowerID
		};
		Login.instance.SendToMobileServer(mobilePlayerChangeFollowerActive);
	}

	public void DeactivateFollower()
	{
		Main.instance.m_UISound.Play_DeactivateChampion();
		Debug.Log(string.Concat("Attempting to Deactivate follower ", this.m_garrFollowerID));
		MobilePlayerChangeFollowerActive mobilePlayerChangeFollowerActive = new MobilePlayerChangeFollowerActive()
		{
			SetInactive = true,
			GarrFollowerID = this.m_garrFollowerID
		};
		Login.instance.SendToMobileServer(mobilePlayerChangeFollowerActive);
	}

	public int GetCurrentFollower()
	{
		return this.m_garrFollowerID;
	}

	public void HandleFollowerDataChanged()
	{
		this.SetFollower(this.m_garrFollowerID);
	}

	private void HandleFollowerToInspectChanged(int garrFollowerID)
	{
		this.SetFollower(garrFollowerID);
	}

	private void HandleUseEquipmentResult(JamGarrisonFollower oldFollower, JamGarrisonFollower newFollower)
	{
		if (this.m_garrFollowerID != newFollower.GarrFollowerID)
		{
			return;
		}
		int[] abilityID = newFollower.AbilityID;
		for (int i = 0; i < (int)abilityID.Length; i++)
		{
			int num = abilityID[i];
			if ((StaticDB.garrAbilityDB.GetRecord(num).Flags & 1) != 0)
			{
				bool flag = true;
				int[] numArray = oldFollower.AbilityID;
				int num1 = 0;
				while (num1 < (int)numArray.Length)
				{
					if (numArray[num1] != num)
					{
						num1++;
					}
					else
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					AbilityDisplay[] componentsInChildren = this.m_equipmentSlotsRootObject.GetComponentsInChildren<AbilityDisplay>(true);
					for (int j = 0; j < (int)componentsInChildren.Length; j++)
					{
						AbilityDisplay abilityDisplay = componentsInChildren[j];
						bool flag1 = true;
						int[] abilityID1 = newFollower.AbilityID;
						int num2 = 0;
						while (num2 < (int)abilityID1.Length)
						{
							int num3 = abilityID1[num2];
							if (abilityDisplay.GetAbilityID() != num3)
							{
								num2++;
							}
							else
							{
								flag1 = false;
								break;
							}
						}
						if (flag1)
						{
							Debug.Log(string.Concat(new object[] { "New ability is ", num, " replacing ability ID ", abilityDisplay.GetAbilityID() }));
							abilityDisplay.SetAbility(num, true, true, this);
							Main.instance.m_UISound.Play_UpgradeEquipment();
							UiAnimMgr.instance.PlayAnim("FlameGlowPulse", abilityDisplay.transform, Vector3.zero, 2f, 0f);
						}
					}
				}
			}
		}
	}

	private void InitEquipmentSlots(JamGarrisonFollower follower)
	{
		AbilityDisplay[] componentsInChildren = this.m_equipmentSlotsRootObject.GetComponentsInChildren<AbilityDisplay>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
		bool flag = false;
		bool flag1 = true;
		for (int j = 0; j < (int)follower.AbilityID.Length; j++)
		{
			if ((StaticDB.garrAbilityDB.GetRecord(follower.AbilityID[j]).Flags & 1) != 0)
			{
				flag = true;
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_equipmentSlotPrefab);
				gameObject.transform.SetParent(this.m_equipmentSlotsRootObject.transform, false);
				AbilityDisplay component = gameObject.GetComponent<AbilityDisplay>();
				component.SetAbility(follower.AbilityID[j], true, true, this);
			}
		}
		bool flags = (follower.Flags & 8) != 0;
		GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(follower.GarrFollowerID);
		if (flags || follower.FollowerLevel < MissionDetailView.GarrisonFollower_GetMaxFollowerLevel((int)record.GarrFollowerTypeID))
		{
			flag1 = false;
		}
		this.m_equipmentSlotsText.gameObject.SetActive((flag ? true : flag1));
	}

	private void OnDisable()
	{
		Main.instance.UseEquipmentResultAction -= new Action<JamGarrisonFollower, JamGarrisonFollower>(this.HandleUseEquipmentResult);
		AdventureMapPanel.instance.FollowerToInspectChangedAction -= new Action<int>(this.HandleFollowerToInspectChanged);
	}

	private void OnEnable()
	{
		Main.instance.UseEquipmentResultAction += new Action<JamGarrisonFollower, JamGarrisonFollower>(this.HandleUseEquipmentResult);
		AdventureMapPanel.instance.FollowerToInspectChangedAction += new Action<int>(this.HandleFollowerToInspectChanged);
	}

	public void SetFollower(int followerID)
	{
		this.m_garrFollowerID = followerID;
		if (followerID == 0)
		{
			RectTransform[] componentsInChildren = this.traitsAndAbilitiesRootObject.GetComponentsInChildren<RectTransform>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] != null && componentsInChildren[i] != this.traitsAndAbilitiesRootObject.transform)
				{
					UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
				}
			}
			AbilityDisplay[] abilityDisplayArray = this.m_equipmentSlotsRootObject.GetComponentsInChildren<AbilityDisplay>(true);
			for (int j = 0; j < (int)abilityDisplayArray.Length; j++)
			{
				UnityEngine.Object.DestroyImmediate(abilityDisplayArray[j].gameObject);
			}
		}
		if (!PersistentFollowerData.followerDictionary.ContainsKey(followerID))
		{
			return;
		}
		GarrFollowerRec record = StaticDB.garrFollowerDB.GetRecord(followerID);
		if (record == null)
		{
			return;
		}
		CreatureRec creatureRec = StaticDB.creatureDB.GetRecord((GarrisonStatus.Faction() != PVP_FACTION.HORDE ? record.AllianceCreatureID : record.HordeCreatureID));
		if (creatureRec == null)
		{
			return;
		}
		JamGarrisonFollower item = PersistentFollowerData.followerDictionary[followerID];
		int d = creatureRec.ID;
		string str = string.Concat("Assets/BundleAssets/PortraitIcons/cid_", d.ToString("D8"), ".png");
		Sprite sprite = AssetBundleManager.portraitIcons.LoadAsset<Sprite>(str);
		if (sprite != null)
		{
			this.followerSnapshot.sprite = sprite;
		}
		RectTransform[] rectTransformArray = this.traitsAndAbilitiesRootObject.GetComponentsInChildren<RectTransform>(true);
		for (int k = 0; k < (int)rectTransformArray.Length; k++)
		{
			if (rectTransformArray[k] != null && rectTransformArray[k] != this.traitsAndAbilitiesRootObject.transform)
			{
				UnityEngine.Object.DestroyImmediate(rectTransformArray[k].gameObject);
			}
		}
		bool flag = false;
		for (int l = 0; l < (int)item.AbilityID.Length; l++)
		{
			GarrAbilityRec garrAbilityRec = StaticDB.garrAbilityDB.GetRecord(item.AbilityID[l]);
			if ((garrAbilityRec.Flags & 512) != 0)
			{
				if (!flag)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.specializationHeaderPrefab);
					gameObject.transform.SetParent(this.traitsAndAbilitiesRootObject.transform, false);
					flag = true;
					Text component = gameObject.GetComponent<Text>();
					if (component != null)
					{
						component.text = StaticDB.GetString("SPECIALIZATION", null);
					}
				}
				GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.abilityDisplayPrefab);
				gameObject1.transform.SetParent(this.traitsAndAbilitiesRootObject.transform, false);
				AbilityDisplay abilityDisplay = gameObject1.GetComponent<AbilityDisplay>();
				abilityDisplay.SetAbility(garrAbilityRec.ID, false, false, null);
			}
		}
		bool flag1 = false;
		for (int m = 0; m < (int)item.AbilityID.Length; m++)
		{
			GarrAbilityRec record1 = StaticDB.garrAbilityDB.GetRecord(item.AbilityID[m]);
			if ((record1.Flags & 1) == 0)
			{
				if ((record1.Flags & 512) == 0)
				{
					if (!flag1)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.abilitiesHeaderPrefab);
						gameObject2.transform.SetParent(this.traitsAndAbilitiesRootObject.transform, false);
						flag1 = true;
						Text text = gameObject2.GetComponent<Text>();
						if (text != null)
						{
							text.text = StaticDB.GetString("ABILITIES", null);
						}
					}
					GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.abilityDisplayPrefab);
					gameObject3.transform.SetParent(this.traitsAndAbilitiesRootObject.transform, false);
					AbilityDisplay component1 = gameObject3.GetComponent<AbilityDisplay>();
					component1.SetAbility(item.AbilityID[m], false, false, null);
				}
			}
		}
		if (item.ZoneSupportSpellID > 0)
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(this.zoneSupportAbilityHeaderPrefab);
			gameObject4.transform.SetParent(this.traitsAndAbilitiesRootObject.transform, false);
			GameObject gameObject5 = UnityEngine.Object.Instantiate<GameObject>(this.m_spellDisplayPrefab);
			gameObject5.transform.SetParent(this.traitsAndAbilitiesRootObject.transform, false);
			gameObject5.GetComponent<SpellDisplay>().SetSpell(item.ZoneSupportSpellID);
			Text componentInChildren = gameObject4.GetComponentInChildren<Text>();
			if (componentInChildren != null)
			{
				componentInChildren.text = StaticDB.GetString("COMBAT_ALLY", null);
			}
		}
		if ((item.Flags & 8) != 0)
		{
			GarrStringRec garrStringRec = StaticDB.garrStringDB.GetRecord((GarrisonStatus.Faction() != PVP_FACTION.ALLIANCE ? record.HordeFlavorGarrStringID : record.AllianceFlavorGarrStringID));
			if (garrStringRec != null)
			{
				Text text1 = UnityEngine.Object.Instantiate<Text>(this.m_troopDescriptionPrefab);
				text1.transform.SetParent(this.traitsAndAbilitiesRootObject.transform, false);
				text1.text = garrStringRec.Text;
			}
		}
		this.InitEquipmentSlots(item);
		this.UpdateChampionButtons(item);
	}

	public void ShowChampionActivationConfirmationDialog()
	{
		AllPopups.instance.ShowChampionActivationConfirmationDialog(this);
	}

	public void ShowChampionDeactivationConfirmationDialog()
	{
		AllPopups.instance.ShowChampionDeactivationConfirmationDialog(this);
	}

	private void Start()
	{
		this.m_equipmentSlotsText.font = GeneralHelpers.LoadStandardFont();
		this.m_equipmentSlotsText.text = StaticDB.GetString("EQUIPMENT_AND_ARMAMENTS", "Equipment and Armaments PH");
		Text componentInChildren = this.m_activateChampionButton.GetComponentInChildren<Text>();
		if (componentInChildren != null)
		{
			componentInChildren.text = StaticDB.GetString("ACTIVATE", null);
		}
		componentInChildren = this.m_deactivateChampionButton.GetComponentInChildren<Text>();
		if (componentInChildren != null)
		{
			componentInChildren.text = StaticDB.GetString("DEACTIVATE", null);
		}
	}

	private void UpdateChampionButtons(JamGarrisonFollower follower)
	{
		if (this.m_activateChampionButton == null || this.m_deactivateChampionButton == null)
		{
			return;
		}
		if ((follower.Flags & 8) == 0)
		{
			bool flags = (follower.Flags & 4) != 0;
			bool remainingFollowerActivations = GarrisonStatus.GetRemainingFollowerActivations() > 0;
			this.m_activateChampionButton.SetActive((!flags ? false : remainingFollowerActivations));
			int numActiveChampions = GeneralHelpers.GetNumActiveChampions();
			int maxActiveFollowers = GarrisonStatus.GetMaxActiveFollowers();
			bool currentMissionID = follower.CurrentMissionID != 0;
			this.m_deactivateChampionButton.SetActive((flags || currentMissionID ? false : numActiveChampions > maxActiveFollowers));
		}
		else
		{
			this.m_activateChampionButton.SetActive(false);
			this.m_deactivateChampionButton.SetActive(false);
		}
	}
}