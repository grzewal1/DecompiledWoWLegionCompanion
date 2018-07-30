using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class PinchZoomContentManager : MonoBehaviour
	{
		public float m_zoomSpeed;

		public ScrollRect m_scrollRect;

		public RectTransform m_mapViewRT;

		public RectTransform m_mapViewContentsRT;

		public float m_zoomFactor = 1f;

		public bool m_isPinching;

		public Vector2 m_pinchPivot;

		public float m_zoomInThreshold;

		public float m_zoomOutThreshold;

		public Action<bool> ZoomFactorChanged;

		public PinchZoomContentManager()
		{
		}

		public void ForceZoomFactorChanged()
		{
			if (this.ZoomFactorChanged != null)
			{
				this.ZoomFactorChanged(true);
			}
		}

		public bool IsZoomedIn()
		{
			MapInfo componentInChildren = base.GetComponentInChildren<MapInfo>();
			if (componentInChildren == null)
			{
				return false;
			}
			if (Mathf.Approximately(this.m_zoomFactor, componentInChildren.m_maxZoomFactor))
			{
				return true;
			}
			return false;
		}

		public void SetZoom(float newZoomFactor, Vector2 zoomCenter, bool bypassLegalPositionEnforcement = false)
		{
			Vector2 vector2 = new Vector2();
			vector2.x = zoomCenter.x - base.transform.position.x;
			vector2.y = zoomCenter.y - base.transform.position.y;
			Vector2 vector21 = vector2 * (newZoomFactor / this.m_zoomFactor);
			Vector2 vector22 = vector2 - vector21;
			base.transform.Translate(vector22.x, vector22.y, 0f);
			this.SetZoom(newZoomFactor, bypassLegalPositionEnforcement);
		}

		public void SetZoom(float newZoomFactor, bool bypassLegalPositionEnforcement = false)
		{
			Vector2 vector2 = new Vector2();
			Vector2 vector21 = new Vector2();
			MapInfo componentInChildren = base.GetComponentInChildren<MapInfo>();
			componentInChildren.m_scaleRoot.transform.localScale = (Vector3.one * newZoomFactor) * componentInChildren.GetViewRelativeScale();
			RectTransform component = componentInChildren.gameObject.GetComponent<RectTransform>();
			this.m_zoomFactor = newZoomFactor;
			float mZoomFactor = this.m_zoomFactor * componentInChildren.GetFillViewSize().x;
			float single = this.m_zoomFactor * componentInChildren.GetFillViewSize().y;
			component.sizeDelta = new Vector2(mZoomFactor, single);
			this.m_mapViewContentsRT.sizeDelta = component.sizeDelta;
			if (!bypassLegalPositionEnforcement)
			{
				Vector3[] vector3Array = new Vector3[4];
				this.m_mapViewRT.GetWorldCorners(vector3Array);
				float single1 = vector3Array[2].x - vector3Array[0].x;
				float single2 = vector3Array[2].y - vector3Array[0].y;
				vector2.x = vector3Array[0].x + single1 * 0.5f;
				vector2.y = vector3Array[0].y + single2 * 0.5f;
				Vector3[] vector3Array1 = new Vector3[4];
				this.m_mapViewContentsRT.GetWorldCorners(vector3Array1);
				float single3 = vector3Array1[2].x - vector3Array1[0].x;
				float single4 = vector3Array1[2].y - vector3Array1[0].y;
				vector21.x = vector3Array1[0].x + single3 * 0.5f;
				vector21.y = vector3Array1[0].y + single4 * 0.5f;
				float single5 = Mathf.Abs(single3 / 2f - single1 / 2f);
				float single6 = Mathf.Abs(single4 / 2f - single2 / 2f);
				Vector3 mMapViewContentsRT = this.m_mapViewContentsRT.position;
				mMapViewContentsRT.x = Mathf.Clamp(mMapViewContentsRT.x, vector2.x - single5, vector2.x + single5);
				mMapViewContentsRT.y = Mathf.Clamp(mMapViewContentsRT.y, vector2.y - single6, vector2.y + single6);
				this.m_mapViewContentsRT.position = mMapViewContentsRT;
			}
			if (this.ZoomFactorChanged != null)
			{
				this.ZoomFactorChanged(false);
			}
		}

		private void Update()
		{
			bool component = this.m_mapViewContentsRT.GetComponent<iTween>() != null;
			if (Input.touchCount != 2)
			{
				this.m_isPinching = false;
				this.m_scrollRect.enabled = true;
			}
			else
			{
				MapInfo componentInChildren = base.GetComponentInChildren<MapInfo>();
				if (componentInChildren == null)
				{
					return;
				}
				Touch touch = Input.GetTouch(0);
				Touch touch1 = Input.GetTouch(1);
				if (!this.m_isPinching)
				{
					this.m_isPinching = true;
					this.m_scrollRect.enabled = false;
				}
				Vector2 vector2 = touch.position - touch.deltaPosition;
				Vector2 vector21 = touch1.position - touch1.deltaPosition;
				float single = (vector2 - vector21).magnitude;
				float single1 = (touch.position - touch1.position).magnitude;
				float mZoomSpeed = (single - single1) * -this.m_zoomSpeed;
				Vector2 vector22 = (touch.position + touch1.position) / 2f;
				Ray ray = Camera.main.ScreenPointToRay(vector22);
				if (AdventureMapPanel.instance.m_testEnableAutoZoomInOut)
				{
					if (!component)
					{
						if (mZoomSpeed > this.m_zoomInThreshold)
						{
							AdventureMapPanel.instance.CenterAndZoom(ray.origin, null, true);
							return;
						}
						if (mZoomSpeed < -this.m_zoomOutThreshold)
						{
							AdventureMapPanel.instance.CenterAndZoom(ray.origin, null, false);
							return;
						}
					}
					return;
				}
				float mZoomFactor = this.m_zoomFactor + mZoomSpeed;
				mZoomFactor = Mathf.Clamp(mZoomFactor, componentInChildren.m_minZoomFactor, componentInChildren.m_maxZoomFactor);
				this.SetZoom(mZoomFactor, ray.origin, false);
			}
		}
	}
}