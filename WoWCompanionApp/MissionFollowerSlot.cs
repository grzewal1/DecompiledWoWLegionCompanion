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
	public class MissionFollowerSlot : MonoBehaviour
	{
		public Image m_portraitFrameImage;

		public Image m_portraitImage;

		public Image m_qualityColorImage;

		public Image m_portraitRingImage;

		public Image m_levelBorderImage;

		public Image m_levelBorderImage_TitleQuality;

		public Text m_levelText;

		public GameObject m_abilityAreaRootObject;

		public MissionDetailView m_missionDetailView;

		public GameObject m_missionMechanicCounterPrefab;

		public bool m_disableButtonWhenFollowerAssigned;

		private GarrFollowerRec m_garrFollowerRec;

		private WrapperGarrisonFollower m_follower;

		public GameObject m_heartPanel;

		public GameObject m_heartArea;

		public GameObject m_enemyPortraitsGroup;

		public GameObject m_fullHeartPrefab;

		public GameObject m_emptyHeartPrefab;

		public GameObject m_outlineHeartPrefab;

		private bool isOccupied;

		private int currentGarrFollowerID;

		public MissionFollowerSlot()
		{
		}

		private void Awake()
		{
			this.ClearFollower();
		}

		public void ClearFollower()
		{
			if (this.currentGarrFollowerID != 0)
			{
				GameObject gameObject = base.transform.parent.parent.parent.parent.gameObject;
				gameObject.BroadcastMessage("RemoveFromParty", this.currentGarrFollowerID, SendMessageOptions.DontRequireReceiver);
			}
			this.SetFollower(0);
			if (this.m_disableButtonWhenFollowerAssigned)
			{
				this.m_portraitFrameImage.GetComponent<Image>().enabled = true;
			}
		}

		public int GetCurrentGarrFollowerID()
		{
			return this.currentGarrFollowerID;
		}

		public void InitHeartPanel()
		{
			if (this.m_heartPanel == null || this.m_garrFollowerRec == null)
			{
				return;
			}
			Image[] componentsInChildren = this.m_heartArea.GetComponentsInChildren<Image>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				Image image = componentsInChildren[i];
				if (image != null && image.gameObject != this.m_heartArea)
				{
					UnityEngine.Object.Destroy(image.gameObject);
				}
			}
			int durability = 1;
			if (GeneralHelpers.MissionHasUncounteredDeadly(this.m_enemyPortraitsGroup))
			{
				durability = this.m_follower.Durability;
			}
			for (int j = 0; j < this.m_follower.Durability - durability; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_fullHeartPrefab);
				gameObject.transform.SetParent(this.m_heartArea.transform, false);
			}
			for (int k = 0; k < durability; k++)
			{
				GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_outlineHeartPrefab);
				gameObject1.transform.SetParent(this.m_heartArea.transform, false);
			}
			for (int l = 0; l < this.m_garrFollowerRec.Vitality - this.m_follower.Durability; l++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.m_emptyHeartPrefab);
				gameObject2.transform.SetParent(this.m_heartArea.transform, false);
			}
		}

		public bool IsOccupied()
		{
			return this.isOccupied;
		}

		private void OnDestroy()
		{
			if (this.m_missionDetailView != null)
			{
				this.m_missionDetailView.FollowerSlotsChangedAction -= new Action(this.InitHeartPanel);
			}
		}

		public void OnTapCombatAllySlot()
		{
			if (this.m_missionDetailView != null && this.m_missionDetailView.GetCombatAllyMissionState() == CombatAllyMissionState.available)
			{
				this.ClearFollower();
			}
		}

		public void OnTapFollowerSlot()
		{
			if (this.m_missionDetailView != null)
			{
				this.ClearFollower();
				this.PlayUnslotSound();
			}
		}

		public void PlayUnslotSound()
		{
			Main.instance.m_UISound.Play_DefaultNavClick();
		}

		public void SetFollower(int garrFollowerID)
		{
			this.m_garrFollowerRec = null;
			if (this.m_abilityAreaRootObject != null)
			{
				RectTransform[] componentsInChildren = this.m_abilityAreaRootObject.GetComponentsInChildren<RectTransform>(true);
				if (componentsInChildren != null)
				{
					for (int i = 0; i < (int)componentsInChildren.Length; i++)
					{
						if (componentsInChildren[i] != null && componentsInChildren[i] != this.m_abilityAreaRootObject.transform)
						{
							componentsInChildren[i].gameObject.transform.SetParent(null);
							UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
						}
					}
				}
			}
			if (garrFollowerID == 0)
			{
				if (this.m_portraitRingImage != null)
				{
					this.m_portraitRingImage.gameObject.SetActive(false);
				}
				if (this.m_heartPanel != null)
				{
					this.m_heartPanel.SetActive(false);
				}
				this.m_levelBorderImage.gameObject.SetActive(false);
				this.m_levelBorderImage_TitleQuality.gameObject.SetActive(false);
				this.m_portraitImage.gameObject.SetActive(false);
				this.m_qualityColorImage.gameObject.SetActive(false);
				this.m_levelText.gameObject.SetActive(false);
				this.isOccupied = false;
				this.m_portraitFrameImage.enabled = true;
				if (this.currentGarrFollowerID != garrFollowerID)
				{
					AdventureMapPanel.instance.MissionFollowerSlotChanged(this.currentGarrFollowerID, false);
				}
				bool flag = this.currentGarrFollowerID != 0;
				this.currentGarrFollowerID = 0;
				if (flag && this.m_missionDetailView != null)
				{
					this.m_missionDetailView.UpdateMissionStatus();
				}
				if (this.m_disableButtonWhenFollowerAssigned)
				{
					this.m_portraitFrameImage.GetComponent<Image>().enabled = true;
				}
				if (this.m_missionDetailView != null)
				{
					this.m_missionDetailView.NotifyFollowerSlotsChanged();
				}
				return;
			}
			this.m_portraitRingImage.gameObject.SetActive(true);
			this.m_levelBorderImage.gameObject.SetActive(true);
			GarrFollowerRec garrFollowerRec = StaticDB.garrFollowerDB.GetRecord(garrFollowerID);
			if (garrFollowerRec == null)
			{
				return;
			}
			if (garrFollowerRec.GarrFollowerTypeID != (uint)GarrisonStatus.GarrisonFollowerType)
			{
				return;
			}
			MissionMechanic[] missionMechanicArray = base.gameObject.transform.parent.parent.parent.gameObject.GetComponentsInChildren<MissionMechanic>(true);
			if (missionMechanicArray == null)
			{
				return;
			}
			WrapperGarrisonFollower item = PersistentFollowerData.followerDictionary[garrFollowerID];
			float single = 0f;
			if (this.m_missionDetailView != null)
			{
				single = MissionDetailView.ComputeFollowerBias(item, item.FollowerLevel, (item.ItemLevelWeapon + item.ItemLevelArmor) / 2, this.m_missionDetailView.GetCurrentMissionID());
			}
			if (single == -1f)
			{
				this.m_levelText.color = Color.red;
			}
			else if (single >= 0f)
			{
				this.m_levelText.color = Color.white;
			}
			else
			{
				this.m_levelText.color = new Color(0.9333f, 0.4392f, 0.2117f);
			}
			if (this.m_abilityAreaRootObject != null && single > -1f)
			{
				for (int j = 0; j < item.AbilityIDs.Count; j++)
				{
					GarrAbilityRec garrAbilityRec = StaticDB.garrAbilityDB.GetRecord(item.AbilityIDs[j]);
					if ((garrAbilityRec.Flags & 1) == 0)
					{
						GarrAbilityEffectRec garrAbilityEffectRec1 = StaticDB.garrAbilityEffectDB.GetRecordsByParentID(garrAbilityRec.ID).FirstOrDefault<GarrAbilityEffectRec>((GarrAbilityEffectRec garrAbilityEffectRec) => {
							if (garrAbilityEffectRec.GarrMechanicTypeID == 0 || garrAbilityEffectRec.AbilityAction != 0)
							{
								return false;
							}
							GarrMechanicTypeRec record = StaticDB.garrMechanicTypeDB.GetRecord((int)garrAbilityEffectRec.GarrMechanicTypeID);
							if (record == null)
							{
								return false;
							}
							return missionMechanicArray.Any<MissionMechanic>((MissionMechanic mechanic) => mechanic.m_missionMechanicTypeID == record.ID);
						});
						if (garrAbilityEffectRec1 != null)
						{
							GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_missionMechanicCounterPrefab, this.m_abilityAreaRootObject.transform, false);
							MissionMechanicTypeCounter component = gameObject.GetComponent<MissionMechanicTypeCounter>();
							component.usedIcon.gameObject.SetActive(false);
							Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, garrAbilityRec.IconFileDataID);
							if (sprite != null)
							{
								component.missionMechanicIcon.sprite = sprite;
							}
							component.countersMissionMechanicTypeID = (int)garrAbilityEffectRec1.GarrMechanicTypeID;
						}
					}
				}
			}
			this.m_levelText.gameObject.SetActive(true);
			this.m_levelText.text = item.FollowerLevel.ToString();
			this.m_portraitImage.gameObject.SetActive(true);
			Sprite sprite1 = GeneralHelpers.LoadIconAsset(AssetBundleType.PortraitIcons, (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? garrFollowerRec.AllianceIconFileDataID : garrFollowerRec.HordeIconFileDataID));
			if (sprite1 != null)
			{
				this.m_portraitImage.sprite = sprite1;
			}
			if (item.Quality != 6)
			{
				this.m_levelBorderImage_TitleQuality.gameObject.SetActive(false);
				this.m_qualityColorImage.gameObject.SetActive(true);
				this.m_levelBorderImage.gameObject.SetActive(true);
				Color qualityColor = GeneralHelpers.GetQualityColor(item.Quality);
				this.m_qualityColorImage.color = qualityColor;
			}
			else
			{
				this.m_levelBorderImage_TitleQuality.gameObject.SetActive(true);
				this.m_qualityColorImage.gameObject.SetActive(false);
				this.m_levelBorderImage.gameObject.SetActive(false);
			}
			this.isOccupied = true;
			bool flags = (item.Flags & 8) != 0;
			this.m_qualityColorImage.gameObject.SetActive(!flags);
			this.m_levelBorderImage.gameObject.SetActive(!flags);
			if (this.m_heartPanel != null)
			{
				this.m_heartPanel.SetActive(flags);
				if (flags)
				{
					this.m_garrFollowerRec = garrFollowerRec;
					this.m_follower = item;
				}
			}
			this.m_portraitFrameImage.enabled = !flags;
			this.currentGarrFollowerID = garrFollowerID;
			if (this.m_disableButtonWhenFollowerAssigned)
			{
				this.m_portraitFrameImage.GetComponent<Image>().enabled = false;
			}
			if (this.m_missionDetailView != null)
			{
				this.m_missionDetailView.UpdateMissionStatus();
				this.m_missionDetailView.NotifyFollowerSlotsChanged();
			}
		}

		private void Start()
		{
			if (this.m_missionDetailView != null)
			{
				this.m_missionDetailView.FollowerSlotsChangedAction += new Action(this.InitHeartPanel);
			}
		}

		private void Update()
		{
		}
	}
}