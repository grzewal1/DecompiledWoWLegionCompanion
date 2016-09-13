using System;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

public class MissionEncounter : MonoBehaviour
{
	public Text nameText;

	public Text missingIconText;

	public Image portraitImage;

	public GameObject m_mechanicRoot;

	public GameObject m_mechanicEffectRoot;

	public GameObject m_missionMechanicPrefab;

	public GameObject m_mechanicEffectDisplayPrefab;

	private int m_garrEncounterID;

	private int m_garrMechanicID;

	public int GetEncounterID()
	{
		return this.m_garrEncounterID;
	}

	public int GetMechanicID()
	{
		return this.m_garrMechanicID;
	}

	public void SetEncounter(int garrEncounterID, int garrMechanicID)
	{
		this.m_garrEncounterID = garrEncounterID;
		this.m_garrMechanicID = garrMechanicID;
		GarrEncounterRec record = StaticDB.garrEncounterDB.GetRecord(garrEncounterID);
		if (record == null)
		{
			this.nameText.set_text(string.Empty + garrEncounterID);
			return;
		}
		this.nameText.set_text(record.Name);
		Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.PortraitIcons, record.PortraitFileDataID);
		if (sprite != null)
		{
			this.portraitImage.set_sprite(sprite);
		}
		else
		{
			this.missingIconText.get_gameObject().SetActive(true);
			this.missingIconText.set_text(string.Empty + record.PortraitFileDataID);
		}
		if (this.m_missionMechanicPrefab != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.m_missionMechanicPrefab);
			gameObject.get_transform().SetParent(this.m_mechanicRoot.get_transform(), false);
			MissionMechanic component = gameObject.GetComponent<MissionMechanic>();
			component.SetMechanicTypeWithMechanicID(garrMechanicID, false);
		}
		if (this.m_mechanicEffectDisplayPrefab != null)
		{
			GarrMechanicRec record2 = StaticDB.garrMechanicDB.GetRecord(garrMechanicID);
			if (record2 == null)
			{
				this.m_mechanicRoot.SetActive(false);
			}
			if (record2 != null && record2.GarrAbilityID != 0)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(this.m_mechanicEffectDisplayPrefab);
				gameObject2.get_transform().SetParent(this.m_mechanicEffectRoot.get_transform(), false);
				AbilityDisplay component2 = gameObject2.GetComponent<AbilityDisplay>();
				component2.SetAbility(record2.GarrAbilityID, false, false, null);
			}
		}
	}

	public void ShowEncounterPopup()
	{
		AllPopups.instance.ShowEncounterPopup(this.m_garrEncounterID, this.m_garrMechanicID);
	}
}
