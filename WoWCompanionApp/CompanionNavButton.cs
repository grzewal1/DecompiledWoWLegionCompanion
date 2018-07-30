using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class CompanionNavButton : MonoBehaviour
	{
		public GameObject m_selectedImage;

		public GameObject m_notSelectedImage;

		public GameObject m_notificationImage;

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