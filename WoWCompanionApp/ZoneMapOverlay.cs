using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class ZoneMapOverlay : MonoBehaviour
	{
		public float m_minZoomFade;

		public float m_maxZoomFade;

		private MapInfo m_mainMapInfo;

		private CanvasGroup m_canvasGroup;

		private float m_targetAlpha;

		public float m_fadeInSpeed;

		public float m_fadeOutSpeed;

		public ZoneMapOverlay()
		{
		}

		private void Awake()
		{
			this.m_mainMapInfo = base.GetComponentInParent<MapInfo>();
			this.m_canvasGroup = base.GetComponent<CanvasGroup>();
		}

		private void SetTargetAlphaForZoomFactor(float zoomFactor)
		{
			if (!AdventureMapPanel.instance.m_testEnableDetailedZoneMaps)
			{
				this.m_targetAlpha = 0f;
				return;
			}
			float single = (zoomFactor - this.m_mainMapInfo.m_minZoomFactor) / (this.m_mainMapInfo.m_maxZoomFactor - this.m_mainMapInfo.m_minZoomFactor);
			if (single < this.m_minZoomFade)
			{
				this.m_targetAlpha = 0f;
			}
			else if (single <= this.m_maxZoomFade)
			{
				this.m_targetAlpha = (single - this.m_minZoomFade) / (this.m_maxZoomFade - this.m_minZoomFade);
			}
			else
			{
				this.m_targetAlpha = 1f;
			}
		}

		private void Update()
		{
			this.SetTargetAlphaForZoomFactor(AdventureMapPanel.instance.m_pinchZoomContentManager.m_zoomFactor);
			this.UpdateFade();
		}

		private void UpdateFade()
		{
			if (this.m_canvasGroup.alpha == this.m_targetAlpha)
			{
				return;
			}
			float mCanvasGroup = this.m_canvasGroup.alpha;
			mCanvasGroup = (mCanvasGroup >= this.m_targetAlpha ? mCanvasGroup - this.m_fadeOutSpeed * Time.deltaTime : mCanvasGroup + this.m_fadeInSpeed * Time.deltaTime);
			this.m_canvasGroup.alpha = Mathf.Clamp(mCanvasGroup, 0f, 1f);
		}
	}
}