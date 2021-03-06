using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class ResourcesDisplay : MonoBehaviour
	{
		public Image m_currencyIcon;

		public Text m_currencyAmountText;

		public Text m_researchText;

		public Text m_costText;

		public ResourcesDisplay()
		{
		}

		private void OnDisable()
		{
			Main.instance.GarrisonDataResetFinishedAction -= new Action(this.UpdateCurrencyDisplayAmount);
		}

		private void OnEnable()
		{
			this.UpdateCurrencyDisplayAmount();
			Main.instance.GarrisonDataResetFinishedAction += new Action(this.UpdateCurrencyDisplayAmount);
		}

		private void Start()
		{
			this.UpdateCurrencyDisplayAmount();
			Sprite sprite = GeneralHelpers.LoadCurrencyIcon(1560);
			if (sprite != null)
			{
				this.m_currencyIcon.sprite = sprite;
			}
			if (this.m_researchText != null)
			{
				this.m_researchText.text = StaticDB.GetString("RESEARCH_TIME", null);
			}
			if (this.m_costText != null)
			{
				this.m_costText.text = StaticDB.GetString("COST", null);
			}
		}

		private void UpdateCurrencyDisplayAmount()
		{
			Text mCurrencyAmountText = this.m_currencyAmountText;
			int num = GarrisonStatus.WarResources();
			mCurrencyAmountText.text = num.ToString("N0", MobileDeviceLocale.GetCultureInfoLocale());
		}
	}
}