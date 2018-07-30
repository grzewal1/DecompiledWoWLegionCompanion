using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class GarrEncounterSetXEncounterDB
	{
		private Dictionary<int, GarrEncounterSetXEncounterRec> m_records = new Dictionary<int, GarrEncounterSetXEncounterRec>();

		public GarrEncounterSetXEncounterDB()
		{
		}

		public GarrEncounterSetXEncounterRec GetRecord(int id)
		{
			GarrEncounterSetXEncounterRec item;
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

		public GarrEncounterSetXEncounterRec GetRecordFirstOrDefault(Func<GarrEncounterSetXEncounterRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrEncounterSetXEncounterRec>(matcher);
		}

		public IEnumerable<GarrEncounterSetXEncounterRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where rec.GarrEncounterSetID == parentID
				select rec;
		}

		public IEnumerable<GarrEncounterSetXEncounterRec> GetRecordsWhere(Func<GarrEncounterSetXEncounterRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrEncounterSetXEncounterRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/GarrEncounterSetXEncounter.txt");
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
				GarrEncounterSetXEncounterRec garrEncounterSetXEncounterRec = new GarrEncounterSetXEncounterRec();
				garrEncounterSetXEncounterRec.Deserialize(str2);
				this.m_records.Add(garrEncounterSetXEncounterRec.ID, garrEncounterSetXEncounterRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}