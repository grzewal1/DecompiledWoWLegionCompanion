using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class MobileStringsDB
	{
		private Dictionary<string, MobileStringsRec> m_records = new Dictionary<string, MobileStringsRec>();

		public MobileStringsDB()
		{
		}

		public MobileStringsRec GetRecord(string baseTag)
		{
			MobileStringsRec item;
			if (!this.m_records.ContainsKey(baseTag))
			{
				item = null;
			}
			else
			{
				item = this.m_records[baseTag];
			}
			return item;
		}

		public MobileStringsRec GetRecordFirstOrDefault(Func<MobileStringsRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<MobileStringsRec>(matcher);
		}

		public IEnumerable<MobileStringsRec> GetRecordsWhere(Func<MobileStringsRec, bool> matcher)
		{
			return this.m_records.Values.Where<MobileStringsRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/MobileStrings_", locale, ".txt" });
			if (this.m_records.Count > 0)
			{
				Debug.Log(string.Concat("Already loaded static db ", str));
				return false;
			}
			TextAsset textAsset = localizedBundle.LoadAsset<TextAsset>(str);
			if (textAsset == null)
			{
				Debug.Log(string.Concat("Unable to load static db ", str));
				return false;
			}
			string str1 = textAsset.ToString();
			int num = 0;
			int num1 = 0;
			do
			{
				num = str1.IndexOf('\n', num1);
				if (num < 0)
				{
					continue;
				}
				string str2 = str1.Substring(num1, num - num1 + 1).Trim();
				MobileStringsRec mobileStringsRec = new MobileStringsRec();
				mobileStringsRec.Deserialize(str2);
				this.m_records.Add(mobileStringsRec.BaseTag, mobileStringsRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}