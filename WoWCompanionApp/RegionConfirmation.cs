using System;
using UnityEngine;
using WowStatConstants;

namespace WoWCompanionApp
{
	public class RegionConfirmation : MonoBehaviour
	{
		public RegionConfirmation()
		{
		}

		public void OnClickCancel()
		{
			Singleton<Login>.instance.CancelRegionIndex();
		}

		public void OnClickOkay()
		{
			Singleton<Login>.instance.SetRegionIndex();
		}

		private void OnDisable()
		{
			Main.instance.m_canvasBlurManager.RemoveBlurRef_MainCanvas();
			Main.instance.m_backButtonManager.PopBackAction();
		}

		private void OnEnable()
		{
			Main.instance.m_canvasBlurManager.AddBlurRef_MainCanvas();
			Main.instance.m_backButtonManager.PushBackAction(BackActionType.hideAllPopups, null);
		}
	}
}