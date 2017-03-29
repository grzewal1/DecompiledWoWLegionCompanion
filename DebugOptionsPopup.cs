using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WowStatConstants;

public class DebugOptionsPopup : MonoBehaviour
{
	public Toggle m_enableDetailedZoneMaps;

	public Toggle m_enableAutoZoomInOut;

	public Toggle m_enableTapToZoomOut;

	public Toggle m_enableCheatCompleteMissionButton;

	public GameObject m_cheatCompleteButton;

	public Dropdown m_localeDropdown;

	public GameObject m_testEffectArea;

	public DebugOptionsPopup()
	{
	}

	private void OnDisable()
	{
		Main.instance.m_backButtonManager.PopBackAction();
	}

	private void OnEnable()
	{
		this.m_enableDetailedZoneMaps.isOn = AdventureMapPanel.instance.m_testEnableDetailedZoneMaps;
		this.m_enableAutoZoomInOut.isOn = AdventureMapPanel.instance.m_testEnableAutoZoomInOut;
		this.m_enableTapToZoomOut.isOn = AdventureMapPanel.instance.m_testEnableTapToZoomOut;
		this.m_enableCheatCompleteMissionButton.isOn = this.m_cheatCompleteButton.activeSelf;
		int num = 0;
		while (num < this.m_localeDropdown.options.Count)
		{
			if (this.m_localeDropdown.options.ToArray()[num].text != Main.instance.GetLocale())
			{
				num++;
			}
			else
			{
				this.m_localeDropdown.@value = num;
				break;
			}
		}
		Main.instance.m_backButtonManager.PushBackAction(BackAction.hideAllPopups, null);
	}

	private void OnValueChanged_EnableAutoZoomInOut(bool isOn)
	{
		AdventureMapPanel.instance.m_testEnableAutoZoomInOut = isOn;
	}

	private void OnValueChanged_EnableCheatCompleteButton(bool isOn)
	{
		this.m_cheatCompleteButton.SetActive(isOn);
	}

	private void OnValueChanged_EnableDetailedZoneMaps(bool isOn)
	{
		AdventureMapPanel.instance.m_testEnableDetailedZoneMaps = isOn;
	}

	private void OnValueChanged_EnableTapToZoomOut(bool isOn)
	{
		AdventureMapPanel.instance.m_testEnableTapToZoomOut = isOn;
	}

	public void OnValueChanged_LocaleDropdown(int index)
	{
		string array = this.m_localeDropdown.options.ToArray()[this.m_localeDropdown.@value].text;
		Debug.Log(string.Concat("Locale option is now ", array));
		SecurePlayerPrefs.SetString("locale", array, Main.uniqueIdentifier);
		PlayerPrefs.Save();
	}

	private void Start()
	{
		this.m_enableDetailedZoneMaps.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged_EnableDetailedZoneMaps));
		this.m_enableAutoZoomInOut.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged_EnableAutoZoomInOut));
		this.m_enableTapToZoomOut.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged_EnableTapToZoomOut));
		this.m_enableCheatCompleteMissionButton.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged_EnableCheatCompleteButton));
	}

	public void TestUIEffect()
	{
		UiAnimMgr.instance.PlayAnim("ItemReadyToUseGlowLoop", this.m_testEffectArea.transform, Vector3.zero, 2f, 0f);
	}
}