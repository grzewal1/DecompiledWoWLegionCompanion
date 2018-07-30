using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class DeactivationConfirmationDialog : BaseDialog
	{
		public Text m_reactivationCostText;

		private FollowerDetailView m_followerDetailView;

		public DeactivationConfirmationDialog()
		{
		}

		public void ConfirmDeactivate()
		{
			this.m_followerDetailView.DeactivateFollower();
			base.gameObject.SetActive(false);
		}

		public void Show(FollowerDetailView followerDetailView)
		{
			base.gameObject.SetActive(true);
			this.m_followerDetailView = followerDetailView;
			this.m_reactivationCostText.text = GarrisonStatus.GetFollowerActivationGoldCost().ToString();
		}

		private void Start()
		{
		}
	}
}