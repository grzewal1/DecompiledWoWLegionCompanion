using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	[RequireComponent(typeof(Dropdown))]
	public class LanguageDropdown : MonoBehaviour
	{
		private Dropdown m_dropdown;

		private List<LanguageDropdown.LanguageEntry> m_languageEntries;

		private int m_value;

		public LanguageDropdown()
		{
			LanguageDropdown.LanguageEntry[] languageEntryArray = new LanguageDropdown.LanguageEntry[11];
			LanguageDropdown.LanguageEntry languageEntry = new LanguageDropdown.LanguageEntry()
			{
				localeCode = "enUS",
				displayName = "English"
			};
			languageEntryArray[0] = languageEntry;
			LanguageDropdown.LanguageEntry languageEntry1 = new LanguageDropdown.LanguageEntry()
			{
				localeCode = "deDE",
				displayName = "Deutsch"
			};
			languageEntryArray[1] = languageEntry1;
			LanguageDropdown.LanguageEntry languageEntry2 = new LanguageDropdown.LanguageEntry()
			{
				localeCode = "esES",
				displayName = "Español (EU)"
			};
			languageEntryArray[2] = languageEntry2;
			LanguageDropdown.LanguageEntry languageEntry3 = new LanguageDropdown.LanguageEntry()
			{
				localeCode = "esMX",
				displayName = "Español (AL)"
			};
			languageEntryArray[3] = languageEntry3;
			LanguageDropdown.LanguageEntry languageEntry4 = new LanguageDropdown.LanguageEntry()
			{
				localeCode = "frFR",
				displayName = "Français"
			};
			languageEntryArray[4] = languageEntry4;
			LanguageDropdown.LanguageEntry languageEntry5 = new LanguageDropdown.LanguageEntry()
			{
				localeCode = "itIT",
				displayName = "Italiano"
			};
			languageEntryArray[5] = languageEntry5;
			LanguageDropdown.LanguageEntry languageEntry6 = new LanguageDropdown.LanguageEntry()
			{
				localeCode = "koKR",
				displayName = "한국어"
			};
			languageEntryArray[6] = languageEntry6;
			LanguageDropdown.LanguageEntry languageEntry7 = new LanguageDropdown.LanguageEntry()
			{
				localeCode = "ptBR",
				displayName = "Português"
			};
			languageEntryArray[7] = languageEntry7;
			LanguageDropdown.LanguageEntry languageEntry8 = new LanguageDropdown.LanguageEntry()
			{
				localeCode = "ruRU",
				displayName = "Русский"
			};
			languageEntryArray[8] = languageEntry8;
			LanguageDropdown.LanguageEntry languageEntry9 = new LanguageDropdown.LanguageEntry()
			{
				localeCode = "zhCN",
				displayName = "简体中文"
			};
			languageEntryArray[9] = languageEntry9;
			LanguageDropdown.LanguageEntry languageEntry10 = new LanguageDropdown.LanguageEntry()
			{
				localeCode = "zhTW",
				displayName = "繁體中文"
			};
			languageEntryArray[10] = languageEntry10;
			this.m_languageEntries = new List<LanguageDropdown.LanguageEntry>(languageEntryArray);
			base();
		}

		public void OnValueChanged(int value)
		{
			if (this.m_value == value)
			{
				return;
			}
			Singleton<DialogFactory>.Instance.CreateOKCancelDialog("RESTART_REQUIRED", "ARE_YOU_SURE", () => {
				SecurePlayerPrefs.SetString(MobileDeviceLocale.LocaleKey, this.m_languageEntries[value].localeCode, Main.uniqueIdentifier);
				Application.Quit();
			}, () => this.m_dropdown.@value = this.m_value);
		}

		private void Start()
		{
			if (MobileDeviceLocale.IsChineseDevice())
			{
				UnityEngine.Object.Destroy(base.transform.parent.gameObject);
				return;
			}
			this.m_dropdown = base.GetComponent<Dropdown>();
			this.m_dropdown.options = (
				from entry in this.m_languageEntries
				select new Dropdown.OptionData(entry.displayName)).ToList<Dropdown.OptionData>();
			int num = this.m_languageEntries.FindIndex((LanguageDropdown.LanguageEntry entry) => entry.localeCode == MobileDeviceLocale.GetBestGuessForLocale());
			this.m_dropdown.@value = (num == -1 ? 0 : num);
			this.m_dropdown.RefreshShownValue();
			this.m_dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnValueChanged));
			this.m_value = this.m_dropdown.@value;
		}

		private struct LanguageEntry
		{
			public string localeCode;

			public string displayName;
		}
	}
}