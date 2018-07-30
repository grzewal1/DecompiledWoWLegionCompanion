using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class EquipmentPopup : MonoBehaviour
	{
		[Header("New Equipment")]
		public Image m_equipmentIcon;

		public Text m_equipmentName;

		public Text m_equipmentQuantity;

		public Text m_equipmentDescription;

		[Header("Equipment Slots")]
		public GameObject m_followerEquipmentReplacementSlotArea;

		public GameObject m_followerEquipmentReplacementSlotPrefab;

		public Text m_tapASlotSlotMessage;

		public Text m_noEquipmentSlotsMessage;

		[Header("Error reporting")]
		public Text m_iconErrorText;

		private int m_garrFollowerID;

		private WrapperFollowerEquipment m_item;

		public EquipmentPopup()
		{
		}

		private void OnDisable()
		{
			Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
		}

		public void OnEnable()
		{
			Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
		}

		public void SetEquipment(WrapperFollowerEquipment item, int garrFollowerID)
		{
			this.m_garrFollowerID = garrFollowerID;
			this.m_item = item;
			ItemRec record = StaticDB.itemDB.GetRecord(item.ItemID);
			this.m_equipmentName.text = record.Display;
			GarrAbilityRec garrAbilityRec = StaticDB.garrAbilityDB.GetRecord(item.GarrAbilityID);
			if (garrAbilityRec == null)
			{
				SpellTooltipRec spellTooltipRec = StaticDB.spellTooltipDB.GetRecord(item.SpellID);
				if (spellTooltipRec == null)
				{
					this.m_equipmentDescription.text = string.Concat(new object[] { "ERROR. Ability ID:", item.GarrAbilityID, " Spell ID: ", item.SpellID, " Item ID:", item.ItemID });
				}
				else
				{
					this.m_equipmentDescription.text = spellTooltipRec.Description;
				}
			}
			else
			{
				this.m_equipmentDescription.text = garrAbilityRec.Description;
			}
			this.m_equipmentDescription.text = WowTextParser.parser.Parse(this.m_equipmentDescription.text, 0);
			if (this.m_iconErrorText != null)
			{
				this.m_iconErrorText.gameObject.SetActive(false);
			}
			Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, record.IconFileDataID);
			if (sprite != null)
			{
				this.m_equipmentIcon.sprite = sprite;
			}
			else if (this.m_iconErrorText != null)
			{
				this.m_iconErrorText.gameObject.SetActive(true);
				this.m_iconErrorText.text = string.Concat(string.Empty, record.IconFileDataID);
			}
			this.m_equipmentQuantity.text = (item.Quantity <= 1 ? string.Empty : string.Concat(string.Empty, item.Quantity));
			FollowerEquipmentReplacementSlot[] componentsInChildren = this.m_followerEquipmentReplacementSlotArea.GetComponentsInChildren<FollowerEquipmentReplacementSlot>(true);
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
			WrapperGarrisonFollower wrapperGarrisonFollower = PersistentFollowerData.followerDictionary[garrFollowerID];
			for (int j = 0; j < wrapperGarrisonFollower.AbilityIDs.Count; j++)
			{
				if ((StaticDB.garrAbilityDB.GetRecord(wrapperGarrisonFollower.AbilityIDs[j]).Flags & 1) != 0)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_followerEquipmentReplacementSlotPrefab);
					gameObject.transform.SetParent(this.m_followerEquipmentReplacementSlotArea.transform, false);
					gameObject.GetComponent<FollowerEquipmentReplacementSlot>().SetAbility(wrapperGarrisonFollower.AbilityIDs[j]);
				}
			}
			FollowerEquipmentReplacementSlot[] followerEquipmentReplacementSlotArray = this.m_followerEquipmentReplacementSlotArea.GetComponentsInChildren<FollowerEquipmentReplacementSlot>(true);
			bool flag = (followerEquipmentReplacementSlotArray == null ? false : (int)followerEquipmentReplacementSlotArray.Length > 0);
			this.m_noEquipmentSlotsMessage.gameObject.SetActive(!flag);
			this.m_tapASlotSlotMessage.gameObject.SetActive(flag);
		}

		public void UseEquipment(int replaceThisAbilityID)
		{
			Debug.Log(string.Concat(new object[] { "Attempting to equip item ", this.m_item.ItemID, " replacing ability ", replaceThisAbilityID }));
			Main.instance.UseEquipment(this.m_garrFollowerID, this.m_item.ItemID, replaceThisAbilityID);
			base.gameObject.SetActive(false);
		}
	}
}