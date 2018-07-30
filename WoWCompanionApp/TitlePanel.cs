using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class TitlePanel : MonoBehaviour
	{
		public GameObject m_legionLogo;

		public GameObject m_legionLogo_CN;

		public GameObject m_legionLogo_TW;

		public GameObject m_legionLogoGlowRoot;

		public Text m_buildText;

		public Text m_briefBuildText;

		public Text m_loginButtonText;

		public Text m_legalText;

		public Dropdown m_portalDropdown;

		public Button m_resumeButton;

		private bool m_showDialog = true;

		private bool m_showPTR = true;

		private string[] m_regionOptions;

		public int CancelIndex
		{
			get;
			set;
		}

		public TitlePanel()
		{
		}

		public void CancelRegionIndex()
		{
			Debug.Log(string.Concat("Canceled index ", this.CancelIndex));
			this.m_showDialog = false;
			this.m_portalDropdown.@value = this.CancelIndex;
			this.m_showDialog = true;
		}

		private string GetDropdownPortalText()
		{
			string lower;
			if (!Singleton<Login>.instance.IsDevRegionList())
			{
				switch (this.m_portalDropdown.@value)
				{
					case 0:
					{
						lower = "us";
						break;
					}
					case 1:
					{
						lower = "eu";
						break;
					}
					case 2:
					{
						lower = "kr";
						break;
					}
					case 3:
					{
						lower = "cn";
						break;
					}
					case 4:
					{
						lower = "test";
						break;
					}
					default:
					{
						goto case 0;
					}
				}
			}
			else
			{
				lower = this.m_portalDropdown.options.ToArray()[this.m_portalDropdown.@value].text.ToLower();
				if (lower.ToLower() == "ptr")
				{
					lower = "test";
				}
			}
			return lower;
		}

		private void OnEnable()
		{
			this.m_legionLogo.SetActive(false);
			this.m_legionLogo_CN.SetActive(false);
			this.m_legionLogo_TW.SetActive(false);
			string locale = Main.instance.GetLocale();
			if (locale == "zhCN")
			{
				this.m_legionLogo_CN.SetActive(true);
			}
			else if (locale != "zhTW")
			{
				this.m_legionLogo.SetActive(true);
			}
			else
			{
				this.m_legionLogo_TW.SetActive(true);
			}
			if (!Singleton<Login>.instance.IsDevRegionList())
			{
				int num = 0;
				string bnPortal = Singleton<Login>.instance.GetBnPortal();
				if (bnPortal != null)
				{
					if (bnPortal == "us")
					{
						num = 0;
					}
					else if (bnPortal == "eu")
					{
						num = 1;
					}
					else if (bnPortal == "kr")
					{
						num = 2;
					}
					else if (bnPortal == "cn")
					{
						num = 3;
					}
					else if (bnPortal == "beta" || bnPortal == "test")
					{
						num = 4;
					}
				}
				this.m_showDialog = false;
				this.m_portalDropdown.@value = num;
				this.m_showDialog = true;
			}
			else
			{
				int num1 = 0;
				while (num1 < this.m_portalDropdown.options.Count)
				{
					if (this.m_portalDropdown.options.ToArray()[num1].text.ToLower() != Singleton<Login>.instance.GetBnPortal())
					{
						num1++;
					}
					else
					{
						this.m_showDialog = false;
						this.m_portalDropdown.@value = num1;
						this.m_showDialog = true;
						break;
					}
				}
			}
			this.CancelIndex = this.m_portalDropdown.@value;
		}

		public void PortalDropdownChanged(int index)
		{
			Debug.Log(string.Concat(new object[] { "New index ", index, ", cancelIndex ", this.CancelIndex }));
			if (this.m_showDialog)
			{
				Singleton<Login>.Instance.LoginUI.ShowRegionConfirmationPopup(index);
			}
		}

		public void SetRegionIndex()
		{
			Debug.Log(string.Concat("Set index ", this.m_portalDropdown.@value));
			this.CancelIndex = this.m_portalDropdown.@value;
			string dropdownPortalText = this.GetDropdownPortalText();
			bool bnPortal = Singleton<Login>.instance.GetBnPortal() != dropdownPortalText;
			Singleton<Login>.instance.SetPortal(dropdownPortalText);
			if (bnPortal)
			{
				Singleton<Login>.instance.ClearAllCachedTokens();
				Debug.Log("Quitting");
				Application.Quit();
			}
		}

		private void Start()
		{
			DateTime utcNow = DateTime.UtcNow;
			Debug.Log(string.Concat(new object[] { "Date: ", utcNow.Month, "/", utcNow.Day, "/", utcNow.Year }));
			DateTime dateTime = new DateTime(2018, 7, 17, 7, 0, 0, DateTimeKind.Utc);
			if (utcNow > dateTime)
			{
				this.m_showPTR = false;
			}
			if (Singleton<Login>.instance.IsDevRegionList())
			{
				this.m_regionOptions = new string[] { "WoW-Dev", "PTR", "ST-US", "ST-EU", "ST-KR", "ST-CN", "US", "EU", "CN", "KR", "ST21" };
			}
			else if (!this.m_showPTR)
			{
				this.m_regionOptions = new string[4];
			}
			else
			{
				this.m_regionOptions = new string[5];
			}
			this.m_briefBuildText.text = Application.version;
			this.m_buildText.text = string.Empty;
			this.UpdateResumeButtonVisiblity();
			List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
			if (!Singleton<Login>.instance.IsDevRegionList())
			{
				this.m_regionOptions[0] = StaticDB.GetString("AMERICAS_AND_OCEANIC", "Americas and Oceanic");
				this.m_regionOptions[1] = StaticDB.GetString("EUROPE", "Europe");
				this.m_regionOptions[2] = StaticDB.GetString("KOREA_AND_TAIWAN", "Korea and Taiwan");
				this.m_regionOptions[3] = StaticDB.GetString("CHINA", "China");
				if (this.m_showPTR)
				{
					this.m_regionOptions[4] = "PTR";
				}
			}
			for (int i = 0; i < (int)this.m_regionOptions.Length; i++)
			{
				Dropdown.OptionData optionDatum = new Dropdown.OptionData()
				{
					text = this.m_regionOptions[i]
				};
				optionDatas.Add(optionDatum);
			}
			this.m_portalDropdown.ClearOptions();
			this.m_portalDropdown.AddOptions(optionDatas);
		}

		private void Update()
		{
		}

		public void UpdateResumeButtonVisiblity()
		{
			bool flag = Singleton<Login>.instance.HaveCachedWebToken();
			this.m_resumeButton.gameObject.SetActive(flag);
			if (!flag)
			{
				this.m_loginButtonText.font = GeneralHelpers.LoadStandardFont();
				this.m_loginButtonText.text = StaticDB.GetString("LOGIN", null);
			}
			else
			{
				Text componentInChildren = this.m_resumeButton.GetComponentInChildren<Text>();
				if (componentInChildren != null)
				{
					componentInChildren.font = GeneralHelpers.LoadStandardFont();
					componentInChildren.text = StaticDB.GetString("LOGIN", null);
					this.m_loginButtonText.font = GeneralHelpers.LoadStandardFont();
					this.m_loginButtonText.text = StaticDB.GetString("ACCOUNT_SELECTION", null);
				}
			}
		}
	}
}