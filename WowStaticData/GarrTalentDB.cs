using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class GarrTalentDB
	{
		private Dictionary<int, GarrTalentRec> m_records = new Dictionary<int, GarrTalentRec>();

		public GarrTalentDB()
		{
		}

		public GarrTalentRec GetRecord(int id)
		{
			GarrTalentRec item;
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

		public GarrTalentRec GetRecordFirstOrDefault(Func<GarrTalentRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrTalentRec>(matcher);
		}

		public IEnumerable<GarrTalentRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where (ulong)rec.GarrTalentTreeID == (long)parentID
				select rec;
		}

		public IEnumerable<GarrTalentRec> GetRecordsWhere(Func<GarrTalentRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrTalentRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/GarrTalent_", locale, ".txt" });
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
				GarrTalentRec garrTalentRec = new GarrTalentRec();
				garrTalentRec.Deserialize(str2);
				this.m_records.Add(garrTalentRec.ID, garrTalentRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}