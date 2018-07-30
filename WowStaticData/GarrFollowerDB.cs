using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class GarrFollowerDB
	{
		private Dictionary<int, GarrFollowerRec> m_records = new Dictionary<int, GarrFollowerRec>();

		public GarrFollowerDB()
		{
		}

		public GarrFollowerRec GetRecord(int id)
		{
			GarrFollowerRec item;
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

		public GarrFollowerRec GetRecordFirstOrDefault(Func<GarrFollowerRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrFollowerRec>(matcher);
		}

		public IEnumerable<GarrFollowerRec> GetRecordsWhere(Func<GarrFollowerRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrFollowerRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/GarrFollower_", locale, ".txt" });
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
				GarrFollowerRec garrFollowerRec = new GarrFollowerRec();
				garrFollowerRec.Deserialize(str2);
				this.m_records.Add(garrFollowerRec.ID, garrFollowerRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}