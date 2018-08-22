using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class CompanionNavButton : MonoBehaviour
	{
		public GameObject m_selectedImage;

		public GameObject m_notSelectedImage;

		public GameObject m_notificationImage;

		public GameObject m_comingSoonEffectPrefab;

		public GameObject m_comingSoonPivot;

		private GameObject m_currentComingSoonEffect;

		private bool m_isSelected;

		public CompanionNavButton()
		{
		}

		private void HandleCompanionNavButtonSelected(CompanionNavButton navButton)
		{
			this.m_isSelected = navButton == this;
			this.UpdateSelectedState();
		}

		protected void InitializeButtonState(bool state)
		{
			this.m_isSelected = false;
			Main.instance.CompanionNavButtonSelectionAction += new Action<CompanionNavButton>(this.HandleCompanionNavButtonSelected);
			this.UpdateSelectedState();
			this.UpdateNotificationState();
		}

		public void PlayComingSoonEffect()
		{
			if (this.m_currentComingSoonEffect == null && this.m_comingSoonEffectPrefab != null && this.m_comingSoonPivot != null)
			{
				this.m_currentComingSoonEffect = UnityEngine.Object.Instantiate<GameObject>(this.m_comingSoonEffectPrefab);
				this.m_currentComingSoonEffect.transform.SetParent(this.m_comingSoonPivot.transform, false);
				this.m_currentComingSoonEffect.transform.localPosition = Vector3.zero;
			}
		}

		public void SelectMe()
		{
			Main.instance.SelectCompanionNavButton(this);
		}

		private void Start()
		{
			this.InitializeButtonState(false);
		}

		protected virtual void UpdateNotificationState()
		{
		}

		private void UpdateSelectedState()
		{
			if (this.m_selectedImage && this.m_notSelectedImage)
			{
				this.m_selectedImage.SetActive(this.m_isSelected);
				this.m_notSelectedImage.SetActive(!this.m_isSelected);
			}
		}
	}
}