using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class GarrFollItemSetMemberDB
	{
		private Dictionary<int, GarrFollItemSetMemberRec> m_records = new Dictionary<int, GarrFollItemSetMemberRec>();

		public GarrFollItemSetMemberDB()
		{
		}

		public GarrFollItemSetMemberRec GetRecord(int id)
		{
			GarrFollItemSetMemberRec item;
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

		public GarrFollItemSetMemberRec GetRecordFirstOrDefault(Func<GarrFollItemSetMemberRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<GarrFollItemSetMemberRec>(matcher);
		}

		public IEnumerable<GarrFollItemSetMemberRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where (ulong)rec.GarrFollItemSetID == (long)parentID
				select rec;
		}

		public IEnumerable<GarrFollItemSetMemberRec> GetRecordsWhere(Func<GarrFollItemSetMemberRec, bool> matcher)
		{
			return this.m_records.Values.Where<GarrFollItemSetMemberRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/GarrFollItemSetMember.txt");
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
				GarrFollItemSetMemberRec garrFollItemSetMemberRec = new GarrFollItemSetMemberRec();
				garrFollItemSetMemberRec.Deserialize(str2);
				this.m_records.Add(garrFollItemSetMemberRec.ID, garrFollItemSetMemberRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}