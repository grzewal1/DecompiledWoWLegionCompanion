using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class EmissaryPopup : MonoBehaviour
	{
		public Text m_descriptionText;

		public EmissaryPopup()
		{
		}

		public void FactionUpdate(IEnumerable<WrapperEmissaryFaction> factions)
		{
			this.m_descriptionText.text = string.Empty;
			foreach (WrapperEmissaryFaction faction in factions)
			{
				Text mDescriptionText = this.m_descriptionText;
				string str = mDescriptionText.text;
				mDescriptionText.text = string.Concat(new object[] { str, "FactionID:\t", faction.FactionID, "\t Standing:\t", faction.FactionAmount, "\n" });
			}
		}
	}
}