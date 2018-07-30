using System;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class SpellDisplay : MonoBehaviour
	{
		public Image m_spellIcon;

		public Text m_iconError;

		public Text m_spellName;

		public Image m_padlockIcon;

		public Shader m_grayscaleShader;

		private int m_spellID;

		public SpellDisplay()
		{
		}

		public void SetLocked(bool locked)
		{
			if (this.m_padlockIcon != null)
			{
				this.m_padlockIcon.gameObject.SetActive(locked);
				this.m_spellIcon.material.SetFloat("_GrayscaleAmount", (!locked ? 0f : 1f));
			}
		}

		public void SetSpell(int spellID)
		{
			this.m_spellID = spellID;
			VW_MobileSpellRec record = StaticDB.vw_mobileSpellDB.GetRecord(this.m_spellID);
			if (record == null)
			{
				this.m_spellName.text = string.Concat("Err Spell ID ", this.m_spellID);
				Debug.LogWarning(string.Concat("Invalid spellID ", this.m_spellID));
				return;
			}
			Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, record.SpellIconFileDataID);
			if (sprite == null)
			{
				Debug.LogWarning(string.Concat("Invalid or missing icon: ", record.SpellIconFileDataID));
				this.m_iconError.gameObject.SetActive(true);
				this.m_iconError.text = string.Concat("Missing Icon ", record.SpellIconFileDataID);
			}
			else
			{
				this.m_spellIcon.sprite = sprite;
				this.m_iconError.gameObject.SetActive(false);
				if (this.m_grayscaleShader != null)
				{
					Material material = new Material(this.m_grayscaleShader);
					this.m_spellIcon.material = material;
				}
			}
			this.m_spellName.text = record.Name;
		}

		public void ShowTooltip()
		{
			Main.instance.allPopups.ShowSpellInfoPopup(this.m_spellID);
		}
	}
}