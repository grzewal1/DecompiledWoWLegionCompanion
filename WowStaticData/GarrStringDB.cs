using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class GarrStringDB
	{
		private Dictionary<int, GarrStringRec> m_records = new Dictionary<int, GarrStringRec>();

		public GarrStringDB()
		{
		}

		public GarrStringRec GetRecord(int id)
		{
			GarrStringRec item;
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

		public GarrStringRec GetRecordFirstOrDefault(Func<GarrStringRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrStringRec>(matcher);
		}

		public IEnumerable<GarrStringRec> GetRecordsWhere(Func<GarrStringRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrStringRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/GarrString_", locale, ".txt" });
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
				GarrStringRec garrStringRec = new GarrStringRec();
				garrStringRec.Deserialize(str2);
				this.m_records.Add(garrStringRec.ID, garrStringRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}