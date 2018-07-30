using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class GarrMissionXEncounterDB
	{
		private Dictionary<int, GarrMissionXEncounterRec> m_records = new Dictionary<int, GarrMissionXEncounterRec>();

		public GarrMissionXEncounterDB()
		{
		}

		public GarrMissionXEncounterRec GetRecord(int id)
		{
			GarrMissionXEncounterRec item;
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

		public GarrMissionXEncounterRec GetRecordFirstOrDefault(Func<GarrMissionXEncounterRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrMissionXEncounterRec>(matcher);
		}

		public IEnumerable<GarrMissionXEncounterRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where (ulong)rec.GarrMissionID == (long)parentID
				select rec;
		}

		public IEnumerable<GarrMissionXEncounterRec> GetRecordsWhere(Func<GarrMissionXEncounterRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrMissionXEncounterRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/GarrMissionXEncounter.txt");
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
				GarrMissionXEncounterRec garrMissionXEncounterRec = new GarrMissionXEncounterRec();
				garrMissionXEncounterRec.Deserialize(str2);
				this.m_records.Add(garrMissionXEncounterRec.ID, garrMissionXEncounterRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}