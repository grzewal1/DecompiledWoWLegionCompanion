using System;
using UnityEngine;
using UnityEngine.UI;
using WowStaticData;

namespace WoWCompanionApp
{
	public class EncounterPopup : MonoBehaviour
	{
		public MissionEncounter m_missionEncounter;

		public Text m_encounterTypeLabel;

		public AbilityInfoPopup m_mechanicEffect;

		public Text m_counteredByLabel;

		public AbilityInfoPopup m_mechanicCounterAbility;

		public EncounterPopup()
		{
		}

		private void Awake()
		{
			this.m_encounterTypeLabel.font = GeneralHelpers.LoadStandardFont();
			this.m_counteredByLabel.font = GeneralHelpers.LoadStandardFont();
			this.m_counteredByLabel.text = StaticDB.GetString("COUNTERED_BY", "Countered By:");
		}

		public void SetEncounter(int garrEncounterID, int garrMechanicID)
		{
			this.m_missionEncounter.SetEncounter(garrEncounterID, garrMechanicID);
			GarrMechanicRec record = StaticDB.garrMechanicDB.GetRecord(garrMechanicID);
			if (record == null || record.GarrAbilityID == 0)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.m_mechanicEffect.SetAbility(record.GarrAbilityID);
			int abilityToCounterMechanicType = MissionMechanic.GetAbilityToCounterMechanicType((int)record.GarrMechanicTypeID);
			this.m_mechanicCounterAbility.SetAbility(abilityToCounterMechanicType);
			GarrMechanicTypeRec garrMechanicTypeRec = StaticDB.garrMechanicTypeDB.GetRecord((int)record.GarrMechanicTypeID);
			if (garrMechanicTypeRec == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.m_encounterTypeLabel.text = garrMechanicTypeRec.Name;
		}
	}
}