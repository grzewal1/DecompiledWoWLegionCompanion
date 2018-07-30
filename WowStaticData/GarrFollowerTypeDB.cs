using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class GarrFollowerTypeDB
	{
		private Dictionary<int, GarrFollowerTypeRec> m_records = new Dictionary<int, GarrFollowerTypeRec>();

		public GarrFollowerTypeDB()
		{
		}

		public GarrFollowerTypeRec GetRecord(int id)
		{
			GarrFollowerTypeRec item;
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

		public GarrFollowerTypeRec GetRecordFirstOrDefault(Func<GarrFollowerTypeRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrFollowerTypeRec>(matcher);
		}

		public IEnumerable<GarrFollowerTypeRec> GetRecordsWhere(Func<GarrFollowerTypeRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrFollowerTypeRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/GarrFollowerType.txt");
			if (this.m_records.Count > 0)
			{
				Debug.Log(string.Concat("Already loaded static db ", str));
				return false;
			}
			TextAsset textAsset = nonLocalizedBundle.LoadAsset<TextAsset>(str);
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
				GarrFollowerTypeRec garrFollowerTypeRec = new GarrFollowerTypeRec();
				garrFollowerTypeRec.Deserialize(str2);
				this.m_records.Add(garrFollowerTypeRec.ID, garrFollowerTypeRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}