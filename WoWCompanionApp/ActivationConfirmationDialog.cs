using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class ActivationConfirmationDialog : BaseDialog
	{
		public Text m_okButtonLabel;

		public Button m_okButton;

		public Text m_activationsRemainingText;

		public Text m_activationCostText;

		private FollowerDetailView m_followerDetailView;

		public ActivationConfirmationDialog()
		{
		}

		public void ConfirmActivate()
		{
			this.m_followerDetailView.ActivateFollower();
			base.gameObject.SetActive(false);
		}

		public void Show(FollowerDetailView followerDetailView)
		{
			base.gameObject.SetActive(true);
			this.m_followerDetailView = followerDetailView;
			if (GarrisonStatus.Gold() >= 250)
			{
				this.m_okButton.interactable = true;
			}
			else
			{
				this.m_okButtonLabel.text = StaticDB.GetString("CANT_AFFORD", null);
				this.m_okButton.interactable = false;
			}
			this.m_activationsRemainingText.text = GarrisonStatus.GetRemainingFollowerActivations().ToString();
			this.m_activationCostText.text = GarrisonStatus.GetFollowerActivationGoldCost().ToString();
		}

		public void Start()
		{
		}
	}
}