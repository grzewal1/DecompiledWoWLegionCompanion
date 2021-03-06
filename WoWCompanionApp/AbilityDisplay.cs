using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class AbilityDisplay : MonoBehaviour
	{
		public Image m_abilityIcon;

		public Text m_abilityNameText;

		public Button m_mainButton;

		public Text m_iconErrorText;

		public GameObject m_counteredMechanicGroup;

		public Text m_counteredMechanicName;

		public Image m_counteredMechanicIcon;

		public Image m_canCounterMechanicIcon;

		public Image m_canCounterMechanicButBusyIcon;

		public Shader m_grayscaleShader;

		private bool m_isCountered;

		public GameObject m_redFailX;

		public FollowerDetailView m_followerDetailView;

		private int m_garrAbilityID;

		private int m_counteredGarrMechanicTypeID;

		private FollowerCanCounterMechanic m_canCounterStatus;

		public AbilityDisplay()
		{
		}

		public int GetAbilityID()
		{
			return this.m_garrAbilityID;
		}

		public FollowerCanCounterMechanic GetCanCounterStatus()
		{
			return this.m_canCounterStatus;
		}

		public void SetAbility(int garrAbilityID, bool hideCounterInfo = false, bool hideName = false, FollowerDetailView followerDetailView = null)
		{
			this.m_followerDetailView = followerDetailView;
			if (this.m_iconErrorText != null)
			{
				this.m_iconErrorText.gameObject.SetActive(false);
			}
			this.m_garrAbilityID = garrAbilityID;
			GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(this.m_garrAbilityID);
			if (record == null)
			{
				Debug.LogWarning(string.Concat("Invalid garrAbilityID ", this.m_garrAbilityID));
				return;
			}
			this.m_abilityNameText.text = record.Name;
			if (record.IconFileDataID <= 0)
			{
				this.m_abilityIcon.enabled = false;
			}
			else
			{
				Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, record.IconFileDataID);
				if (sprite != null)
				{
					this.m_abilityIcon.sprite = sprite;
					if (this.m_grayscaleShader != null)
					{
						Material material = new Material(this.m_grayscaleShader);
						this.m_abilityIcon.material = material;
					}
				}
				else if (this.m_iconErrorText != null)
				{
					this.m_iconErrorText.gameObject.SetActive(true);
					this.m_iconErrorText.text = string.Concat(string.Empty, record.IconFileDataID);
				}
				this.m_abilityIcon.enabled = true;
			}
			this.m_garrAbilityID = record.ID;
			GarrAbilityCategoryRec garrAbilityCategoryRec = StaticDB.garrAbilityCategoryDB.GetRecord((int)record.GarrAbilityCategoryID);
			if (garrAbilityCategoryRec != null)
			{
				this.m_counteredMechanicName.text = garrAbilityCategoryRec.Name;
			}
			if (this.m_counteredMechanicGroup != null)
			{
				if (!hideCounterInfo)
				{
					this.m_counteredGarrMechanicTypeID = 0;
					GarrAbilityEffectRec garrAbilityEffectRec1 = StaticDB.garrAbilityEffectDB.GetRecordsByParentID(record.ID).FirstOrDefault<GarrAbilityEffectRec>((GarrAbilityEffectRec garrAbilityEffectRec) => {
						if (garrAbilityEffectRec.GarrMechanicTypeID == 0)
						{
							return false;
						}
						if (garrAbilityEffectRec.AbilityAction != 0)
						{
							return false;
						}
						return StaticDB.garrMechanicTypeDB.GetRecord((int)garrAbilityEffectRec.GarrMechanicTypeID) != null;
					});
					if (garrAbilityEffectRec1 != null)
					{
						GarrMechanicTypeRec garrMechanicTypeRec = StaticDB.garrMechanicTypeDB.GetRecord((int)garrAbilityEffectRec1.GarrMechanicTypeID);
						Sprite sprite1 = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, garrMechanicTypeRec.IconFileDataID);
						if (sprite1 == null)
						{
							this.m_counteredMechanicName.text = string.Concat("ERR ", garrMechanicTypeRec.IconFileDataID);
						}
						else
						{
							this.m_counteredMechanicIcon.sprite = sprite1;
						}
						this.m_counteredGarrMechanicTypeID = garrMechanicTypeRec.ID;
					}
				}
				else
				{
					this.m_counteredMechanicGroup.SetActive(false);
				}
			}
			this.SetCountered(false, true);
			if (this.m_counteredMechanicGroup != null)
			{
				this.m_counteredMechanicGroup.SetActive(this.m_counteredGarrMechanicTypeID > 0);
			}
			this.m_abilityNameText.gameObject.SetActive(!hideName);
		}

		public void SetCanCounterStatus(FollowerCanCounterMechanic canCounterStatus)
		{
			this.m_canCounterStatus = canCounterStatus;
			if (this.m_canCounterMechanicIcon == null || this.m_canCounterMechanicButBusyIcon == null)
			{
				return;
			}
			this.m_canCounterMechanicIcon.gameObject.SetActive(canCounterStatus == FollowerCanCounterMechanic.yesAndAvailable);
			this.m_canCounterMechanicButBusyIcon.gameObject.SetActive(canCounterStatus == FollowerCanCounterMechanic.yesButBusy);
		}

		public void SetCountered(bool isCountered, bool playCounteredEffect = true)
		{
			if (isCountered && this.m_isCountered)
			{
				return;
			}
			this.m_isCountered = isCountered;
			if (this.m_abilityIcon.material != null)
			{
				this.m_abilityIcon.material.SetFloat("_GrayscaleAmount", (!this.m_isCountered ? 0f : 1f));
			}
			if (this.m_isCountered && playCounteredEffect)
			{
				UiAnimMgr.instance.PlayAnim("RedFailX", base.transform, Vector3.zero, 0.8f, 0f);
			}
			if (this.m_redFailX != null)
			{
				this.m_redFailX.SetActive(this.m_isCountered);
			}
		}

		public void ShowEquipmentDialog()
		{
			if (AllPopups.instance.GetCurrentFollowerDetailView() != this.m_followerDetailView)
			{
				return;
			}
			AllPopups.instance.ShowEquipmentDialog(this.m_garrAbilityID, this.m_followerDetailView, true);
		}

		public void ShowEquipmentPopup()
		{
			this.ShowTooltip();
		}

		public void ShowTooltip()
		{
			Main.instance.allPopups.ShowAbilityInfoPopup(this.m_garrAbilityID);
		}
	}
}