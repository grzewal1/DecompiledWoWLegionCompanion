using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class InviteLinkDialog : MonoBehaviour
	{
		public InputField m_inputField;

		public LocalizedText m_errorText;

		public Text m_communityText;

		public Button m_nextButton;

		public GameObject m_inputParentObj;

		public GameObject m_confirmationParentObj;

		public RectTransform m_windowFrame;

		public float m_linkInputDialogHeight;

		public float m_confirmationDialogHeight;

		private string m_validTicketKey;

		private ClubInfo m_validClubInfo;

		private const string COMMUNITIES_INVALID_LINK = "COMMUNITIES_INVALID_LINK";

		private const string COMMUNITIES_WRONG_COMMUNITY = "COMMUNITIES_WRONG_COMMUNITY";

		public InviteLinkDialog()
		{
		}

		private void Awake()
		{
			Club.OnClubAdded += new Club.ClubAddedHandler(this.OnClubAdded);
			Club.OnClubTicketReceived += new Club.ClubTicketReceivedHandler(this.OnTicketReceived);
			this.m_inputField.onEndEdit.AddListener((string argument0) => this.ValidateLink());
			this.ResetObjectVisibility();
		}

		public void ConfirmLink()
		{
			Club.RedeemTicket(this.m_validTicketKey);
		}

		private void HideLinkConfirmPanel()
		{
			this.m_inputParentObj.SetActive(true);
			this.m_confirmationParentObj.SetActive(false);
			RectTransform mWindowFrame = this.m_windowFrame;
			Vector2 vector2 = this.m_windowFrame.sizeDelta;
			mWindowFrame.sizeDelta = new Vector2(vector2.x, this.m_linkInputDialogHeight);
		}

		public void OnBackClick()
		{
			this.HideLinkConfirmPanel();
		}

		private void OnClubAdded(Club.ClubAddedEvent clubAddedEvent)
		{
			base.GetComponent<BaseDialog>().CloseDialog();
		}

		private void OnDestroy()
		{
			Club.OnClubAdded -= new Club.ClubAddedHandler(this.OnClubAdded);
			Club.OnClubTicketReceived -= new Club.ClubTicketReceivedHandler(this.OnTicketReceived);
		}

		public void OnNextClick()
		{
			this.ShowLinkConfirmPanel();
		}

		private void OnTicketReceived(Club.ClubTicketReceivedEvent ticketEvent)
		{
			if (ticketEvent.Error != ClubErrorType.ERROR_COMMUNITIES_NONE)
			{
				this.ShowErrorText("COMMUNITIES_INVALID_LINK");
			}
			else if (ticketEvent.Info.HasValue)
			{
				ClubInfo value = ticketEvent.Info.Value;
				if (value.clubType != ClubType.Character)
				{
					this.ShowErrorText("COMMUNITIES_WRONG_COMMUNITY");
					return;
				}
				this.m_nextButton.interactable = true;
				this.m_communityText.text = value.name.ToUpper();
				this.m_validTicketKey = ticketEvent.Ticket;
				this.m_validClubInfo = value;
			}
		}

		public void PasteCode()
		{
		}

		private void ResetObjectVisibility()
		{
			this.m_errorText.gameObject.SetActive(false);
			this.m_nextButton.interactable = false;
		}

		private void ShowErrorText(string key)
		{
			this.m_errorText.gameObject.SetActive(true);
			this.m_errorText.SetNewStringKey(key);
		}

		private void ShowLinkConfirmPanel()
		{
			this.m_inputParentObj.SetActive(false);
			this.m_confirmationParentObj.SetActive(true);
			RectTransform mWindowFrame = this.m_windowFrame;
			Vector2 vector2 = this.m_windowFrame.sizeDelta;
			mWindowFrame.sizeDelta = new Vector2(vector2.x, this.m_confirmationDialogHeight);
		}

		private void ValidateLink()
		{
			if (this.m_validTicketKey != this.m_inputField.text)
			{
				if (!string.IsNullOrEmpty(this.m_inputField.text))
				{
					this.m_errorText.gameObject.SetActive(false);
					this.m_nextButton.interactable = false;
					this.m_validTicketKey = null;
					Club.RequestTicket(this.m_inputField.text);
					return;
				}
				this.ResetObjectVisibility();
			}
		}
	}
}