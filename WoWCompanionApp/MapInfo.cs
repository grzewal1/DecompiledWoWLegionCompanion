using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class MapInfo : MonoBehaviour
	{
		public float m_minZoomFactor;

		public float m_maxZoomFactor;

		public GameObject m_scaleRoot;

		public Image m_mapImage;

		public float m_mapW;

		public float m_mapH;

		private bool m_initialized;

		private Vector2 m_fillViewSize;

		private float m_viewRelativeScale;

		public MapInfo()
		{
		}

		private void Awake()
		{
			this.Init();
		}

		public void CalculateFillScale()
		{
			float mMapW = this.m_mapW;
			float mMapH = this.m_mapH;
			float single = mMapW / mMapH;
			Rect mMapViewRT = AdventureMapPanel.instance.m_mapViewRT.rect;
			float mMapViewRT1 = mMapViewRT.width / AdventureMapPanel.instance.m_mapViewRT.rect.height;
			this.m_viewRelativeScale = 1f;
			if (single >= mMapViewRT1)
			{
				Rect rect = AdventureMapPanel.instance.m_mapViewRT.rect;
				this.m_viewRelativeScale = rect.height / mMapH;
			}
			else
			{
				Rect rect1 = AdventureMapPanel.instance.m_mapViewRT.rect;
				this.m_viewRelativeScale = rect1.width / mMapW;
			}
			this.m_fillViewSize.x = mMapW * this.m_viewRelativeScale;
			this.m_fillViewSize.y = mMapH * this.m_viewRelativeScale;
		}

		public Vector2 GetFillViewSize()
		{
			if (!this.m_initialized)
			{
				this.Init();
				AdventureMapPanel.instance.m_pinchZoomContentManager.SetZoom(1f, false);
			}
			return this.m_fillViewSize;
		}

		public float GetViewRelativeScale()
		{
			return this.m_viewRelativeScale;
		}

		private void Init()
		{
			if (!this.m_initialized)
			{
				this.CalculateFillScale();
				this.m_initialized = true;
			}
		}

		public void SetMaxZoom(float val)
		{
			this.m_maxZoomFactor = val;
		}

		private void Update()
		{
			if (this.m_initialized)
			{
				this.CalculateFillScale();
				AdventureMapPanel.instance.m_pinchZoomContentManager.SetZoom(AdventureMapPanel.instance.m_pinchZoomContentManager.m_zoomFactor, false);
			}
		}
	}
}