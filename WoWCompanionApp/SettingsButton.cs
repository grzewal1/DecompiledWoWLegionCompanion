using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class SettingsButton : MonoBehaviour
	{
		public SettingsButton()
		{
		}

		public void OnClick()
		{
			Singleton<DialogFactory>.Instance.CreateAppSettingsDialog();
		}

		private void Start()
		{
		}
	}
}