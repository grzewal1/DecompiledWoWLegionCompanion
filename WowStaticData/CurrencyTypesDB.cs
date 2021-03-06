using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class CurrencyTypesDB
	{
		private Dictionary<int, CurrencyTypesRec> m_records = new Dictionary<int, CurrencyTypesRec>();

		public CurrencyTypesDB()
		{
		}

		public CurrencyTypesRec GetRecord(int id)
		{
			CurrencyTypesRec item;
			if (!this.m_records.ContainsKey(id))
			{
				item = null;
			}
			else
			{
				item = this.m_records[id];
			}
			return item;
		}

		public CurrencyTypesRec GetRecordFirstOrDefault(Func<CurrencyTypesRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<CurrencyTypesRec>(matcher);
		}

		public IEnumerable<CurrencyTypesRec> GetRecordsWhere(Func<CurrencyTypesRec, bool> matcher)
		{
			return this.m_records.Values.Where<CurrencyTypesRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/CurrencyTypes_", locale, ".txt" });
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
				CurrencyTypesRec currencyTypesRec = new CurrencyTypesRec();
				currencyTypesRec.Deserialize(str2);
				this.m_records.Add(currencyTypesRec.ID, currencyTypesRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}