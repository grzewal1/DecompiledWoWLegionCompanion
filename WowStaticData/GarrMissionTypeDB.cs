using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class GarrMissionTypeDB
	{
		private Dictionary<int, GarrMissionTypeRec> m_records = new Dictionary<int, GarrMissionTypeRec>();

		public GarrMissionTypeDB()
		{
		}

		public GarrMissionTypeRec GetRecord(int id)
		{
			GarrMissionTypeRec item;
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

		public GarrMissionTypeRec GetRecordFirstOrDefault(Func<GarrMissionTypeRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrMissionTypeRec>(matcher);
		}

		public IEnumerable<GarrMissionTypeRec> GetRecordsWhere(Func<GarrMissionTypeRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrMissionTypeRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/GarrMissionType_", locale, ".txt" });
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
				GarrMissionTypeRec garrMissionTypeRec = new GarrMissionTypeRec();
				garrMissionTypeRec.Deserialize(str2);
				this.m_records.Add(garrMissionTypeRec.ID, garrMissionTypeRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}