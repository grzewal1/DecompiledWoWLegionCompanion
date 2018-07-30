using System;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class FollowerEquipmentReplacementSlot : MonoBehaviour
	{
		public Image m_abilityIcon;

		public Text m_abilityNameText;

		public Text m_iconErrorText;

		private int m_garrAbilityID;

		public FollowerEquipmentReplacementSlot()
		{
		}

		public void OnTap()
		{
		}

		public void SetAbility(int garrAbilityID)
		{
			this.m_garrAbilityID = garrAbilityID;
			if (this.m_iconErrorText != null)
			{
				this.m_iconErrorText.gameObject.SetActive(false);
			}
			GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(this.m_garrAbilityID);
			if (record == null)
			{
				Debug.LogWarning(string.Concat("Invalid garrAbilityID ", this.m_garrAbilityID));
				return;
			}
			this.m_abilityNameText.text = record.Name;
			if (record.IconFileDataID != 0)
			{
				Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, record.IconFileDataID);
				if (sprite != null)
				{
					this.m_abilityIcon.sprite = sprite;
				}
				else if (this.m_iconErrorText != null)
				{
					this.m_iconErrorText.gameObject.SetActive(true);
					this.m_iconErrorText.text = string.Concat(string.Empty, record.IconFileDataID);
				}
			}
		}
	}
}