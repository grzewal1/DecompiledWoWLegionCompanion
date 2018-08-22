using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WoWCompanionApp
{
	public class MobileDeviceLocale
	{
		public readonly static string LocaleKey;

		private static Dictionary<string, string> s_languageCodeToLocale;

		private static Dictionary<string, CultureInfo> s_localeToCultureInfo;

		private static Dictionary<string, int> s_countryCodeToRegionId;

		public static IEnumerable<string> SupportedLocales
		{
			get
			{
				return MobileDeviceLocale.s_languageCodeToLocale.Values;
			}
		}

		static MobileDeviceLocale()
		{
			MobileDeviceLocale.LocaleKey = "WoWCompanion:Locale";
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "en", "enUS" },
				{ "fr", "frFR" },
				{ "de", "deDE" },
				{ "ko", "koKR" },
				{ "ru", "ruRU" },
				{ "it", "itIT" },
				{ "pt", "ptBR" },
				{ "es", "esES" },
				{ "en-AU", "enUS" },
				{ "en-GB", "enUS" },
				{ "fr-CA", "frFR" },
				{ "es-MX", "esMX" },
				{ "zh-Hans", "zhCN" },
				{ "zh-Hant", "zhTW" },
				{ "pt-PT", "ptBR" }
			};
			MobileDeviceLocale.s_languageCodeToLocale = strs;
			MobileDeviceLocale.s_localeToCultureInfo = new Dictionary<string, CultureInfo>();
			Dictionary<string, int> strs1 = new Dictionary<string, int>()
			{
				{ "AD", 2 },
				{ "AE", 2 },
				{ "AG", 1 },
				{ "AL", 2 },
				{ "AM", 2 },
				{ "AO", 2 },
				{ "AR", 1 },
				{ "AT", 2 },
				{ "AU", 1 },
				{ "AZ", 2 },
				{ "BA", 2 },
				{ "BB", 1 },
				{ "BD", 1 },
				{ "BE", 2 },
				{ "BF", 2 },
				{ "BG", 2 },
				{ "BH", 2 },
				{ "BI", 2 },
				{ "BJ", 2 },
				{ "BM", 2 },
				{ "BN", 1 },
				{ "BO", 1 },
				{ "BR", 1 },
				{ "BS", 1 },
				{ "BT", 1 },
				{ "BW", 2 },
				{ "BY", 2 },
				{ "BZ", 1 },
				{ "CA", 1 },
				{ "CD", 2 },
				{ "CF", 2 },
				{ "CG", 2 },
				{ "CH", 2 },
				{ "CI", 2 },
				{ "CL", 1 },
				{ "CM", 2 },
				{ "CN", 3 },
				{ "CO", 1 },
				{ "CR", 1 },
				{ "CU", 1 },
				{ "CV", 2 },
				{ "CY", 2 },
				{ "CZ", 2 },
				{ "DE", 2 },
				{ "DJ", 2 },
				{ "DK", 2 },
				{ "DM", 1 },
				{ "DO", 1 },
				{ "DZ", 2 },
				{ "EC", 1 },
				{ "EE", 2 },
				{ "EG", 2 },
				{ "ER", 2 },
				{ "ES", 2 },
				{ "ET", 2 },
				{ "FI", 2 },
				{ "FJ", 1 },
				{ "FK", 2 },
				{ "FO", 2 },
				{ "FR", 2 },
				{ "GA", 2 },
				{ "GB", 2 },
				{ "GD", 1 },
				{ "GE", 2 },
				{ "GL", 2 },
				{ "GM", 2 },
				{ "GN", 2 },
				{ "GQ", 2 },
				{ "GR", 2 },
				{ "GS", 2 },
				{ "GT", 1 },
				{ "GW", 2 },
				{ "GY", 1 },
				{ "HK", 3 },
				{ "HN", 1 },
				{ "HR", 2 },
				{ "HT", 1 },
				{ "HU", 2 },
				{ "ID", 1 },
				{ "IE", 2 },
				{ "IL", 2 },
				{ "IM", 2 },
				{ "IN", 1 },
				{ "IQ", 2 },
				{ "IR", 2 },
				{ "IS", 2 },
				{ "IT", 2 },
				{ "JM", 1 },
				{ "JO", 2 },
				{ "JP", 3 },
				{ "KE", 2 },
				{ "KG", 2 },
				{ "KH", 2 },
				{ "KI", 1 },
				{ "KM", 2 },
				{ "KP", 1 },
				{ "KR", 3 },
				{ "KW", 2 },
				{ "KY", 2 },
				{ "KZ", 2 },
				{ "LA", 1 },
				{ "LB", 2 },
				{ "LC", 1 },
				{ "LI", 2 },
				{ "LK", 1 },
				{ "LR", 2 },
				{ "LS", 2 },
				{ "LT", 2 },
				{ "LU", 2 },
				{ "LV", 2 },
				{ "LY", 2 },
				{ "MA", 2 },
				{ "MC", 2 },
				{ "MD", 2 },
				{ "ME", 2 },
				{ "MG", 2 },
				{ "MK", 2 },
				{ "ML", 2 },
				{ "MM", 1 },
				{ "MN", 2 },
				{ "MO", 3 },
				{ "MR", 2 },
				{ "MT", 2 },
				{ "MU", 2 },
				{ "MV", 2 },
				{ "MW", 2 },
				{ "MX", 1 },
				{ "MY", 1 },
				{ "MZ", 2 },
				{ "NA", 2 },
				{ "NC", 2 },
				{ "NE", 2 },
				{ "NG", 2 },
				{ "NI", 1 },
				{ "NL", 2 },
				{ "NO", 2 },
				{ "NP", 1 },
				{ "NR", 1 },
				{ "NZ", 1 },
				{ "OM", 2 },
				{ "PA", 1 },
				{ "PE", 1 },
				{ "PF", 1 },
				{ "PG", 1 },
				{ "PH", 1 },
				{ "PK", 2 },
				{ "PL", 2 },
				{ "PT", 2 },
				{ "PY", 1 },
				{ "QA", 2 },
				{ "RO", 2 },
				{ "RS", 2 },
				{ "RU", 2 },
				{ "RW", 2 },
				{ "SA", 2 },
				{ "SB", 1 },
				{ "SC", 2 },
				{ "SD", 2 },
				{ "SE", 2 },
				{ "SG", 1 },
				{ "SH", 2 },
				{ "SI", 2 },
				{ "SK", 2 },
				{ "SL", 2 },
				{ "SN", 2 },
				{ "SO", 2 },
				{ "SR", 2 },
				{ "ST", 2 },
				{ "SV", 1 },
				{ "SY", 2 },
				{ "SZ", 2 },
				{ "TD", 2 },
				{ "TG", 2 },
				{ "TH", 1 },
				{ "TJ", 2 },
				{ "TL", 1 },
				{ "TM", 2 },
				{ "TN", 2 },
				{ "TO", 1 },
				{ "TR", 2 },
				{ "TT", 1 },
				{ "TV", 1 },
				{ "TW", 3 },
				{ "TZ", 2 },
				{ "UA", 2 },
				{ "UG", 2 },
				{ "US", 1 },
				{ "UY", 1 },
				{ "UZ", 2 },
				{ "VA", 2 },
				{ "VC", 1 },
				{ "VE", 1 },
				{ "VN", 1 },
				{ "VU", 1 },
				{ "WS", 1 },
				{ "YE", 2 },
				{ "YU", 2 },
				{ "ZA", 2 },
				{ "ZM", 2 },
				{ "ZW", 2 }
			};
			MobileDeviceLocale.s_countryCodeToRegionId = strs1;
			List<string> strs2 = new List<string>()
			{
				"en-US",
				"fr-FR",
				"de-DE",
				"ko-KR",
				"ru-RU",
				"it-IT",
				"pt-BR",
				"es-ES",
				"es-MX",
				"zh-CN",
				"zh-TW"
			};
			List<string> strs3 = strs2;
			Dictionary<string, CultureInfo> dictionary = CultureInfo.GetCultures(CultureTypes.AllCultures).ToDictionary<CultureInfo, string, CultureInfo>((CultureInfo culture) => culture.Name, (CultureInfo culture) => culture);
			foreach (string str in strs3)
			{
				if (!dictionary.ContainsKey(str))
				{
					continue;
				}
				MobileDeviceLocale.s_localeToCultureInfo.Add(str.Replace("-", string.Empty), dictionary[str]);
			}
		}

		public MobileDeviceLocale()
		{
		}

		public static string GetBestGuessForLocale()
		{
			string str = SecurePlayerPrefs.GetString(MobileDeviceLocale.LocaleKey, Main.uniqueIdentifier);
			if (!string.IsNullOrEmpty(str))
			{
				return str;
			}
			string str1 = "enUS";
			bool flag = false;
			string languageCode = MobileDeviceLocale.GetLanguageCode();
			try
			{
				flag = MobileDeviceLocale.s_languageCodeToLocale.TryGetValue(languageCode, out str1);
			}
			catch (Exception exception)
			{
			}
			if (!flag)
			{
				languageCode = languageCode.Substring(0, 2);
				try
				{
					flag = MobileDeviceLocale.s_languageCodeToLocale.TryGetValue(languageCode, out str1);
				}
				catch (Exception exception1)
				{
				}
			}
			if (!flag || languageCode == "es")
			{
				int num = 1;
				string countryCode = MobileDeviceLocale.GetCountryCode();
				try
				{
					MobileDeviceLocale.s_countryCodeToRegionId.TryGetValue(countryCode, out num);
				}
				catch (Exception exception2)
				{
				}
				if (languageCode != null)
				{
					if (languageCode == "es")
					{
						str1 = (num != 1 ? "esES" : "esMX");
						return str1;
					}
					else
					{
						if (languageCode != "zh")
						{
							goto Label2;
						}
						str1 = (countryCode != "CN" ? "zhTW" : "zhCN");
						return str1;
					}
				}
			Label2:
				str1 = "enUS";
			}
			return str1;
		}

		public static string GetCountryCode()
		{
			return MobileDeviceLocale.GetLocaleCountryCode();
		}

		public static CultureInfo GetCultureInfoLocale()
		{
			string bestGuessForLocale = MobileDeviceLocale.GetBestGuessForLocale();
			if (!MobileDeviceLocale.s_localeToCultureInfo.ContainsKey(bestGuessForLocale))
			{
				return CultureInfo.CurrentCulture;
			}
			return MobileDeviceLocale.s_localeToCultureInfo[bestGuessForLocale];
		}

		public static string GetLanguageCode()
		{
			return MobileDeviceLocale.GetLocaleLanguageCode();
		}

		private static string GetLocaleCountryCode()
		{
			string str;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build$VERSION"))
			{
				if (androidJavaClass == null)
				{
					str = "US";
				}
				else
				{
					int @static = androidJavaClass.GetStatic<int>("SDK_INT");
					AndroidJavaObject androidJavaObject = null;
					if (@static < 24)
					{
						using (AndroidJavaClass androidJavaClass1 = new AndroidJavaClass("java.util.Locale"))
						{
							if (androidJavaClass1 != null)
							{
								androidJavaObject = androidJavaClass1.CallStatic<AndroidJavaObject>("getDefault", new object[0]);
							}
						}
					}
					else
					{
						using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.os.LocaleList"))
						{
							if (androidJavaClass2 != null)
							{
								androidJavaObject = androidJavaClass2.CallStatic<AndroidJavaObject>("getDefault", new object[0]).Call<AndroidJavaObject>("get", new object[] { 0 });
							}
						}
					}
					str = (androidJavaObject == null ? "US" : androidJavaObject.Call<string>("getCountry", new object[0]));
				}
			}
			return str;
		}

		private static string GetLocaleLanguageCode()
		{
			string str;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build$VERSION"))
			{
				if (androidJavaClass == null)
				{
					str = "enUS";
				}
				else
				{
					int @static = androidJavaClass.GetStatic<int>("SDK_INT");
					AndroidJavaObject androidJavaObject = null;
					if (@static < 24)
					{
						using (AndroidJavaClass androidJavaClass1 = new AndroidJavaClass("java.util.Locale"))
						{
							if (androidJavaClass1 != null)
							{
								androidJavaObject = androidJavaClass1.CallStatic<AndroidJavaObject>("getDefault", new object[0]);
							}
						}
					}
					else
					{
						using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.os.LocaleList"))
						{
							if (androidJavaClass2 != null)
							{
								androidJavaObject = androidJavaClass2.CallStatic<AndroidJavaObject>("getDefault", new object[0]).Call<AndroidJavaObject>("get", new object[] { 0 });
							}
						}
					}
					str = (androidJavaObject == null ? "enUS" : string.Concat(androidJavaObject.Call<string>("getLanguage", new object[0]), androidJavaObject.Call<string>("getCountry", new object[0])));
				}
			}
			return str;
		}

		public static bool IsChineseDevice()
		{
			return MobileDeviceLocale.GetCountryCode().Equals("CN", StringComparison.OrdinalIgnoreCase);
		}
	}
}