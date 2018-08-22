using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class AppSettingsDialog : MonoBehaviour
	{
		public Toggle m_enableSFX;

		public Toggle m_enableNotifications;

		public Dictionary<TiledRandomTexture, float> HeightDictionary = new Dictionary<TiledRandomTexture, float>();

		public RectTransform m_headerBar;

		public AppSettingsDialog()
		{
		}

		private void AdjustForNotch()
		{
		}

		public void DestroyDialog()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void OnDisable()
		{
			Main.instance.m_backButtonManager.PopBackAction();
		}

		private void OnEnable()
		{
			Main.instance.m_UISound.Play_ShowGenericTooltip();
			Main.instance.m_backButtonManager.PushBackAction(() => this.DestroyDialog());
		}

		public void OnValueChanged_EnableNotifications(bool isOn)
		{
			Main.instance.m_UISound.Play_ButtonBlackClick();
			Main.instance.m_enableNotifications = isOn;
			SecurePlayerPrefs.SetString("EnableNotifications", isOn.ToString().ToLower(), Main.uniqueIdentifier);
		}

		public void OnValueChanged_EnableSFX(bool isOn)
		{
			Main.instance.m_UISound.EnableSFX(isOn);
			Main.instance.m_UISound.Play_ButtonBlackClick();
			SecurePlayerPrefs.SetString("EnableSFX", isOn.ToString().ToLower(), Main.uniqueIdentifier);
		}

		private void Start()
		{
			this.SyncWithOptions();
			this.AdjustForNotch();
		}

		public void SyncWithOptions()
		{
			this.m_enableSFX.isOn = Main.instance.m_UISound.IsSFXEnabled();
			this.m_enableNotifications.isOn = Main.instance.m_enableNotifications;
		}

		private void Update()
		{
			TiledRandomTexture[] componentsInChildren = base.GetComponentsInChildren<TiledRandomTexture>();
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				(componentsInChildren[i].transform as RectTransform).rect.height = 16f;
			}
		}
	}
}