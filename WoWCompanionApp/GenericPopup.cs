using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class GenericPopup : BaseDialog
	{
		public Text m_headerText;

		public Text m_descriptionText;

		public Text m_fullText;

		public static Action DisabledAction;

		public GenericPopup()
		{
		}

		private void OnDisable()
		{
			if (GenericPopup.DisabledAction != null)
			{
				GenericPopup.DisabledAction();
			}
		}

		public void SetFullText(string fullText)
		{
			this.m_headerText.gameObject.SetActive(false);
			this.m_descriptionText.gameObject.SetActive(false);
			this.m_fullText.gameObject.SetActive(true);
			this.m_fullText.text = fullText;
		}

		public void SetText(string headerText, string descriptionText)
		{
			this.m_headerText.gameObject.SetActive(true);
			this.m_descriptionText.gameObject.SetActive(true);
			this.m_fullText.gameObject.SetActive(false);
			this.m_headerText.text = headerText;
			this.m_descriptionText.text = descriptionText;
		}

		private void Start()
		{
		}
	}
}