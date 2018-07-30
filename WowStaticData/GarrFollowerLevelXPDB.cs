using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class GarrFollowerLevelXPDB
	{
		private Dictionary<int, GarrFollowerLevelXPRec> m_records = new Dictionary<int, GarrFollowerLevelXPRec>();

		public GarrFollowerLevelXPDB()
		{
		}

		public GarrFollowerLevelXPRec GetRecord(int id)
		{
			GarrFollowerLevelXPRec item;
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

		public GarrFollowerLevelXPRec GetRecordFirstOrDefault(Func<GarrFollowerLevelXPRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrFollowerLevelXPRec>(matcher);
		}

		public IEnumerable<GarrFollowerLevelXPRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where (ulong)rec.FollowerLevel == (long)parentID
				select rec;
		}

		public IEnumerable<GarrFollowerLevelXPRec> GetRecordsWhere(Func<GarrFollowerLevelXPRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrFollowerLevelXPRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/GarrFollowerLevelXP.txt");
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
				GarrFollowerLevelXPRec garrFollowerLevelXPRec = new GarrFollowerLevelXPRec();
				garrFollowerLevelXPRec.Deserialize(str2);
				this.m_records.Add(garrFollowerLevelXPRec.ID, garrFollowerLevelXPRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}