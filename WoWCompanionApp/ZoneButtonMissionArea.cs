using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class ZoneButtonMissionArea : MonoBehaviour
	{
		private static PinchZoomContentManager m_pinchZoomManager;

		public ZoneButtonMissionArea()
		{
		}

		private void OnDisable()
		{
			ZoneButtonMissionArea.m_pinchZoomManager.ZoomFactorChanged -= new Action<bool>(this.OnZoomChanged);
		}

		private void OnEnable()
		{
			if (ZoneButtonMissionArea.m_pinchZoomManager == null)
			{
				ZoneButtonMissionArea.m_pinchZoomManager = base.gameObject.GetComponentInParent<PinchZoomContentManager>();
			}
			ZoneButtonMissionArea.m_pinchZoomManager.ZoomFactorChanged += new Action<bool>(this.OnZoomChanged);
		}

		private void OnZoomChanged(bool force)
		{
			MapInfo componentInParent = base.gameObject.GetComponentInParent<MapInfo>();
			CanvasGroup component = base.gameObject.GetComponent<CanvasGroup>();
			component.alpha = (ZoneButtonMissionArea.m_pinchZoomManager.m_zoomFactor - 1f) / (componentInParent.m_maxZoomFactor - 1f);
			bool flag = (component.alpha <= 0.99f ? false : true);
			if (flag != component.blocksRaycasts || force)
			{
				CanvasGroup[] componentsInChildren = base.gameObject.GetComponentsInChildren<CanvasGroup>(true);
				for (int i = 0; i < (int)componentsInChildren.Length; i++)
				{
					componentsInChildren[i].blocksRaycasts = flag;
				}
			}
		}
	}
}