using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class ZoneLabel : MonoBehaviour
	{
		public float m_minZoom;

		public float m_maxZoom;

		private MapInfo m_mainMapInfo;

		private CanvasGroup m_canvasGroup;

		public ZoneLabel()
		{
		}

		private void Start()
		{
			this.m_canvasGroup = base.GetComponent<CanvasGroup>();
			if (this.m_canvasGroup == null)
			{
				this.m_canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
				this.m_canvasGroup.blocksRaycasts = false;
				this.m_canvasGroup.interactable = false;
			}
			this.m_canvasGroup.alpha = 0f;
			this.m_mainMapInfo = AdventureMapPanel.instance.m_mainMapInfo;
		}

		private void Update()
		{
			float mZoomFactor = (AdventureMapPanel.instance.m_pinchZoomContentManager.m_zoomFactor - this.m_mainMapInfo.m_minZoomFactor) / (this.m_mainMapInfo.m_maxZoomFactor - this.m_mainMapInfo.m_minZoomFactor);
			if (mZoomFactor <= this.m_minZoom)
			{
				this.m_canvasGroup.alpha = 0f;
			}
			else if (mZoomFactor < this.m_maxZoom)
			{
				this.m_canvasGroup.alpha = (mZoomFactor - this.m_minZoom) / (this.m_maxZoom - this.m_minZoom);
			}
			else
			{
				this.m_canvasGroup.alpha = 1f;
			}
		}
	}
}