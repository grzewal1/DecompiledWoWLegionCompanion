using System;
using UnityEngine;
using UnityEngine.UI;
using WowStaticData;

public class EncounterPopup : MonoBehaviour
{
	public MissionEncounter m_missionEncounter;

	public Text m_encounterTypeLabel;

	public AbilityInfoPopup m_mechanicEffect;

	public Text m_counteredByLabel;

	public AbilityInfoPopup m_mechanicCounterAbility;

	private void Awake()
	{
		this.m_encounterTypeLabel.set_font(GeneralHelpers.LoadStandardFont());
		this.m_counteredByLabel.set_font(GeneralHelpers.LoadStandardFont());
		this.m_counteredByLabel.set_text(StaticDB.GetString("COUNTERED_BY", "Countered By:"));
	}

	public void SetEncounter(int garrEncounterID, int garrMechanicID)
	{
		this.m_missionEncounter.SetEncounter(garrEncounterID, garrMechanicID);
		GarrMechanicRec record = StaticDB.garrMechanicDB.GetRecord(garrMechanicID);
		if (record == null || record.GarrAbilityID == 0)
		{
			base.get_gameObject().SetActive(false);
			return;
		}
		this.m_mechanicEffect.SetAbility(record.GarrAbilityID);
		int abilityToCounterMechanicType = MissionMechanic.GetAbilityToCounterMechanicType((int)record.GarrMechanicTypeID);
		this.m_mechanicCounterAbility.SetAbility(abilityToCounterMechanicType);
		GarrMechanicTypeRec record2 = StaticDB.garrMechanicTypeDB.GetRecord((int)record.GarrMechanicTypeID);
		if (record2 == null)
		{
			base.get_gameObject().SetActive(false);
			return;
		}
		this.m_encounterTypeLabel.set_text(record2.Name);
	}
}
