using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class GarrFollowerQualityDB
	{
		private Dictionary<int, GarrFollowerQualityRec> m_records = new Dictionary<int, GarrFollowerQualityRec>();

		public GarrFollowerQualityDB()
		{
		}

		public GarrFollowerQualityRec GetRecord(int id)
		{
			GarrFollowerQualityRec item;
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

		public GarrFollowerQualityRec GetRecordFirstOrDefault(Func<GarrFollowerQualityRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrFollowerQualityRec>(matcher);
		}

		public IEnumerable<GarrFollowerQualityRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where (ulong)rec.Quality == (long)parentID
				select rec;
		}

		public IEnumerable<GarrFollowerQualityRec> GetRecordsWhere(Func<GarrFollowerQualityRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrFollowerQualityRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/GarrFollowerQuality.txt");
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
				GarrFollowerQualityRec garrFollowerQualityRec = new GarrFollowerQualityRec();
				garrFollowerQualityRec.Deserialize(str2);
				this.m_records.Add(garrFollowerQualityRec.ID, garrFollowerQualityRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}