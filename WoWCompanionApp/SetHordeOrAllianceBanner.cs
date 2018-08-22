using System;
using UnityEngine;
using WowStatConstants;

namespace WoWCompanionApp
{
	public class SetHordeOrAllianceBanner : MonoBehaviour
	{
		public GameObject m_allianceBG;

		public GameObject m_hordeBG;

		public SetHordeOrAllianceBanner()
		{
		}

		private void Start()
		{
			bool flag = (GarrisonStatus.Faction() != PVP_FACTION.HORDE ? false : true);
			this.m_hordeBG.gameObject.SetActive(flag);
			this.m_allianceBG.gameObject.SetActive(!flag);
		}
	}
}