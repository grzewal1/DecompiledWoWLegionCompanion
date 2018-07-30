using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class FactionDB
	{
		private Dictionary<int, FactionRec> m_records = new Dictionary<int, FactionRec>();

		public FactionDB()
		{
		}

		public FactionRec GetRecord(int id)
		{
			FactionRec item;
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

		public FactionRec GetRecordFirstOrDefault(Func<FactionRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<FactionRec>(matcher);
		}

		public IEnumerable<FactionRec> GetRecordsWhere(Func<FactionRec, bool> matcher)
		{
			return this.m_records.Values.Where<FactionRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/Faction_", locale, ".txt" });
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
				FactionRec factionRec = new FactionRec();
				factionRec.Deserialize(str2);
				this.m_records.Add(factionRec.ID, factionRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}