using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class ZoneButton : MonoBehaviour
	{
		public string m_zoneNameTag;

		public AdventureMapPanel.eZone m_zoneID;

		private string m_zoneName;

		public ZoneButton()
		{
		}

		public string GetZoneName()
		{
			return this.m_zoneName;
		}

		public void OnTap()
		{
			AdventureMapPanel.instance.SetSelectedIconContainer(null);
			if (!AdventureMapPanel.instance.m_testEnableTapToZoomOut || !Mathf.Approximately(AdventureMapPanel.instance.m_pinchZoomContentManager.m_zoomFactor, AdventureMapPanel.instance.m_mainMapInfo.m_maxZoomFactor))
			{
				AdventureMapPanel.instance.CenterAndZoom(base.transform.position, this, true);
			}
			else if (AdventureMapPanel.instance.GetCurrentMapMission() > 0 || AdventureMapPanel.instance.GetCurrentWorldQuest() > 0)
			{
				AdventureMapPanel.instance.SelectMissionFromMap(0);
				AdventureMapPanel.instance.SelectWorldQuest(0);
			}
		}

		private void Start()
		{
			this.m_zoneName = StaticDB.GetString(this.m_zoneNameTag, null);
		}

		private void Update()
		{
		}
	}
}