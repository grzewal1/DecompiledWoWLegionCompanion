using System;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
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

		public MissionEncounter()
		{
		}

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
				this.nameText.text = string.Concat(string.Empty, garrEncounterID);
				return;
			}
			this.nameText.text = record.Name;
			Sprite sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.PortraitIcons, record.PortraitFileDataID);
			if (sprite == null)
			{
				this.missingIconText.gameObject.SetActive(true);
				this.missingIconText.text = string.Concat(string.Empty, record.PortraitFileDataID);
			}
			else
			{
				this.portraitImage.sprite = sprite;
			}
			if (this.m_missionMechanicPrefab != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_missionMechanicPrefab);
				gameObject.transform.SetParent(this.m_mechanicRoot.transform, false);
				gameObject.GetComponent<MissionMechanic>().SetMechanicTypeWithMechanicID(garrMechanicID, false);
			}
			if (this.m_mechanicEffectDisplayPrefab != null)
			{
				GarrMechanicRec garrMechanicRec = StaticDB.garrMechanicDB.GetRecord(garrMechanicID);
				if (garrMechanicRec == null)
				{
					this.m_mechanicRoot.SetActive(false);
				}
				if (garrMechanicRec != null && garrMechanicRec.GarrAbilityID != 0)
				{
					GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_mechanicEffectDisplayPrefab);
					gameObject1.transform.SetParent(this.m_mechanicEffectRoot.transform, false);
					AbilityDisplay component = gameObject1.GetComponent<AbilityDisplay>();
					component.SetAbility(garrMechanicRec.GarrAbilityID, false, false, null);
				}
			}
		}

		public void ShowEncounterPopup()
		{
			AllPopups.instance.ShowEncounterPopup(this.m_garrEncounterID, this.m_garrMechanicID);
		}
	}
}