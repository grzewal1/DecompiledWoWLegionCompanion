using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class ZoneMissionOverview : MonoBehaviour
	{
		public ZoneMissionStat statDisplay_AvailableMissions;

		public ZoneMissionStat statDisplay_InProgressMissions;

		public ZoneMissionStat statDisplay_CompleteMissions;

		public ZoneMissionStat statDisplay_WorldQuests;

		public GameObject m_zoneNameArea;

		public GameObject m_statsArea;

		public GameObject m_bountyButtonRoot;

		public GameObject m_anonymousBountyButtonRoot;

		public int[] m_areaID;

		public string zoneNameTag;

		public Text zoneNameText;

		public int m_invasionPOIID;

		public GameObject m_invasionZoneNameArea;

		public Text m_invasionZoneNameText;

		private static PinchZoomContentManager m_pinchZoomManager;

		public ZoneMissionOverview()
		{
		}

		private void OnDisable()
		{
			ZoneMissionOverview.m_pinchZoomManager.ZoomFactorChanged -= new Action<bool>(this.OnZoomChanged);
		}

		private void OnEnable()
		{
			if (ZoneMissionOverview.m_pinchZoomManager == null)
			{
				ZoneMissionOverview.m_pinchZoomManager = base.gameObject.GetComponentInParent<PinchZoomContentManager>();
			}
			ZoneMissionOverview.m_pinchZoomManager.ZoomFactorChanged += new Action<bool>(this.OnZoomChanged);
		}

		private void OnZoomChanged(bool force)
		{
			CanvasGroup component = base.gameObject.GetComponent<CanvasGroup>();
			MapInfo componentInParent = base.gameObject.GetComponentInParent<MapInfo>();
			component.alpha = (componentInParent.m_maxZoomFactor - ZoneMissionOverview.m_pinchZoomManager.m_zoomFactor) / (componentInParent.m_maxZoomFactor - 1f);
			if (component.alpha >= 0.99f)
			{
				component.interactable = true;
			}
			else
			{
				component.interactable = false;
			}
		}

		private void Start()
		{
			if (this.zoneNameTag.Length <= 0)
			{
				this.m_invasionZoneNameArea.SetActive(false);
				this.m_zoneNameArea.SetActive(false);
				this.m_statsArea.SetActive(false);
			}
			else
			{
				this.m_zoneNameArea.SetActive(this.zoneNameTag.Length > 0);
				this.zoneNameText.text = StaticDB.GetString(this.zoneNameTag, null);
				this.m_invasionZoneNameText.text = StaticDB.GetString(this.zoneNameTag, null);
				this.m_invasionZoneNameArea.SetActive(false);
				this.m_zoneNameArea.SetActive(this.zoneNameTag.Length > 0);
			}
		}

		private void Update()
		{
			if (this.m_invasionZoneNameArea.activeSelf)
			{
				TimeSpan currentInvasionExpirationTime = LegionfallData.GetCurrentInvasionExpirationTime() - GarrisonStatus.CurrentTime();
				currentInvasionExpirationTime = (currentInvasionExpirationTime.TotalSeconds <= 0 ? TimeSpan.Zero : currentInvasionExpirationTime);
				if (currentInvasionExpirationTime.TotalSeconds <= 0)
				{
					this.m_invasionZoneNameArea.SetActive(false);
					this.m_zoneNameArea.SetActive(!string.IsNullOrEmpty(this.zoneNameTag));
				}
			}
		}
	}
}