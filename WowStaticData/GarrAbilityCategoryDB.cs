using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class GarrAbilityCategoryDB
	{
		private Dictionary<int, GarrAbilityCategoryRec> m_records = new Dictionary<int, GarrAbilityCategoryRec>();

		public GarrAbilityCategoryDB()
		{
		}

		public GarrAbilityCategoryRec GetRecord(int id)
		{
			GarrAbilityCategoryRec item;
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

		public GarrAbilityCategoryRec GetRecordFirstOrDefault(Func<GarrAbilityCategoryRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrAbilityCategoryRec>(matcher);
		}

		public IEnumerable<GarrAbilityCategoryRec> GetRecordsWhere(Func<GarrAbilityCategoryRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrAbilityCategoryRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/GarrAbilityCategory_", locale, ".txt" });
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
				GarrAbilityCategoryRec garrAbilityCategoryRec = new GarrAbilityCategoryRec();
				garrAbilityCategoryRec.Deserialize(str2);
				this.m_records.Add(garrAbilityCategoryRec.ID, garrAbilityCategoryRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}