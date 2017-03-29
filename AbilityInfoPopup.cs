using System;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

public class AbilityInfoPopup : MonoBehaviour
{
	public Image m_abilityIcon;

	public Text m_abilityNameText;

	public Text m_abilityDescription;

	public AbilityInfoPopup()
	{
	}

	private void OnDisable()
	{
		Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
		Main.instance.m_canvasBlurManager.RemoveBlurRef_Level2Canvas();
		Main.instance.m_backButtonManager.PopBackAction();
	}

	public void OnEnable()
	{
		Main.instance.m_UISound.Play_ShowGenericTooltip();
		Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
		Main.instance.m_canvasBlurManager.AddBlurRef_Level2Canvas();
		Main.instance.m_backButtonManager.PushBackAction(BackAction.hideAllPopups, null);
	}

	public void SetAbility(int garrAbilityID)
	{
		GarrAbilityRec record = StaticDB.garrAbilityDB.GetRecord(garrAbilityID);
		if (record == null)
		{
			Debug.LogWarning(string.Concat("Invalid garrAbilityID ", garrAbilityID));
			return;
		}
		this.m_abilityNameText.text = record.Name;
		this.m_abilityDescription.text = WowTextParser.parser.Parse(record.Description, 0);
		this.m_abilityDescription.supportRichText = WowTextParser.parser.IsRichText();
		Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, record.IconFileDataID);
		if (sprite != null)
		{
			this.m_abilityIcon.sprite = sprite;
		}
	}

	public void SetSpell(int spellID)
	{
		VW_MobileSpellRec record = StaticDB.vw_mobileSpellDB.GetRecord(spellID);
		if (record == null)
		{
			this.m_abilityNameText.text = string.Concat("Err Spell ID ", spellID);
			Debug.LogWarning(string.Concat("Invalid spellID ", spellID));
			return;
		}
		Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, record.SpellIconFileDataID);
		if (sprite == null)
		{
			Debug.LogWarning(string.Concat("Invalid or missing icon: ", record.SpellIconFileDataID));
		}
		else
		{
			this.m_abilityIcon.sprite = sprite;
		}
		this.m_abilityNameText.text = record.Name;
		SpellTooltipRec spellTooltipRec = StaticDB.spellTooltipDB.GetRecord(spellID);
		if (spellTooltipRec != null)
		{
			this.m_abilityDescription.text = WowTextParser.parser.Parse(spellTooltipRec.Description, 0);
			return;
		}
		this.m_abilityNameText.text = string.Concat("Err Tooltip ID ", spellID);
		Debug.LogWarning(string.Concat("Invalid Tooltip ", spellID));
	}
}