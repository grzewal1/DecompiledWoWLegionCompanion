using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class GarrMissionDB
	{
		private Dictionary<int, GarrMissionRec> m_records = new Dictionary<int, GarrMissionRec>();

		public GarrMissionDB()
		{
		}

		public GarrMissionRec GetRecord(int id)
		{
			GarrMissionRec item;
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

		public GarrMissionRec GetRecordFirstOrDefault(Func<GarrMissionRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrMissionRec>(matcher);
		}

		public IEnumerable<GarrMissionRec> GetRecordsWhere(Func<GarrMissionRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrMissionRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/GarrMission_", locale, ".txt" });
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
				GarrMissionRec garrMissionRec = new GarrMissionRec();
				garrMissionRec.Deserialize(str2);
				this.m_records.Add(garrMissionRec.ID, garrMissionRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}