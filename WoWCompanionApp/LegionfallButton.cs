using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class LegionfallButton : MonoBehaviour
	{
		public PinchZoomContentManager m_pinchZoomManager;

		private bool m_isVisible;

		public LegionfallButton()
		{
		}

		private void HandleContributionInfoChanged()
		{
			this.SetVisibility(LegionfallData.HasAccess());
		}

		private void OnDisable()
		{
			this.m_pinchZoomManager.ZoomFactorChanged -= new Action<bool>(this.OnZoomChanged);
			Main.instance.ContributionInfoChangedAction -= new Action(this.HandleContributionInfoChanged);
		}

		private void OnEnable()
		{
			this.SetVisibility(LegionfallData.HasAccess());
			this.m_pinchZoomManager.ZoomFactorChanged += new Action<bool>(this.OnZoomChanged);
			Main.instance.ContributionInfoChangedAction += new Action(this.HandleContributionInfoChanged);
		}

		private void OnZoomChanged(bool force)
		{
			if (!this.m_isVisible)
			{
				return;
			}
			CanvasGroup component = base.gameObject.GetComponent<CanvasGroup>();
			MapInfo componentInParent = base.gameObject.GetComponentInParent<MapInfo>();
			component.alpha = (componentInParent.m_maxZoomFactor - this.m_pinchZoomManager.m_zoomFactor) / (componentInParent.m_maxZoomFactor - 1f);
			if (component.alpha >= 0.99f)
			{
				component.interactable = true;
				component.blocksRaycasts = true;
			}
			else
			{
				component.interactable = false;
				component.blocksRaycasts = false;
			}
		}

		private void SetVisibility(bool isVisible)
		{
			this.m_isVisible = isVisible;
			CanvasGroup component = base.gameObject.GetComponent<CanvasGroup>();
			component.alpha = (!this.m_isVisible ? 0f : 1f);
			component.interactable = this.m_isVisible;
			component.blocksRaycasts = this.m_isVisible;
		}
	}
}