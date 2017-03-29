using System;
using UnityEngine;
using UnityEngine.UI;
using WowJamMessages.MobileClientJSON;

public class EmissaryPopup : MonoBehaviour
{
	public Text m_descriptionText;

	public EmissaryPopup()
	{
	}

	public void FactionUpdate(MobileClientEmissaryFactionUpdate msg)
	{
		this.m_descriptionText.text = string.Empty;
		MobileEmissaryFaction[] faction = msg.Faction;
		for (int i = 0; i < (int)faction.Length; i++)
		{
			MobileEmissaryFaction mobileEmissaryFaction = faction[i];
			Text mDescriptionText = this.m_descriptionText;
			string str = mDescriptionText.text;
			mDescriptionText.text = string.Concat(new object[] { str, "FactionID:\t", mobileEmissaryFaction.FactionID, "\t Standing:\t", mobileEmissaryFaction.FactionAmount, "\n" });
		}
	}
}