using System;
using UnityEngine;
using UnityEngine.UI;
using WowStatConstants;
using WowStaticData;

namespace WoWCompanionApp
{
	public class MissionTypeDialog : MonoBehaviour
	{
		public Image m_icon;

		public Text m_missionTypeName;

		public Text m_missionTypeDescription;

		private const string BROKEN_COLOR_STRING = "FFFFD200";

		private const string FIXED_COLOR_STRING = "FFD200FF";

		public MissionTypeDialog()
		{
		}

		public void InitializeMissionDialog(int missionId)
		{
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(missionId);
			if (record != null)
			{
				GarrMechanicRec garrMechanicRec = StaticDB.garrMechanicDB.GetRecord(record.EnvGarrMechanicID);
				if (garrMechanicRec != null)
				{
					GarrAbilityRec garrAbilityRec = StaticDB.garrAbilityDB.GetRecord(garrMechanicRec.GarrAbilityID);
					if (garrAbilityRec != null)
					{
						this.m_icon.sprite = GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, garrAbilityRec.IconFileDataID);
						this.m_missionTypeName.text = garrAbilityRec.Name;
						this.m_missionTypeDescription.text = WowTextParser.parser.Parse(garrAbilityRec.Description, 0).Replace("FFFFD200", "FFD200FF");
					}
				}
			}
		}
	}
}