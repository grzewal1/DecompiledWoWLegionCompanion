using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class AzeriteEmpoweredItemDB
	{
		private Dictionary<int, AzeriteEmpoweredItemRec> m_records = new Dictionary<int, AzeriteEmpoweredItemRec>();

		public AzeriteEmpoweredItemDB()
		{
		}

		public AzeriteEmpoweredItemRec GetRecord(int id)
		{
			AzeriteEmpoweredItemRec item;
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

		public AzeriteEmpoweredItemRec GetRecordFirstOrDefault(Func<AzeriteEmpoweredItemRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<AzeriteEmpoweredItemRec>(matcher);
		}

		public IEnumerable<AzeriteEmpoweredItemRec> GetRecordsWhere(Func<AzeriteEmpoweredItemRec, bool> matcher)
		{
			return this.m_records.Values.Where<AzeriteEmpoweredItemRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/AzeriteEmpoweredItem.txt");
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
				AzeriteEmpoweredItemRec azeriteEmpoweredItemRec = new AzeriteEmpoweredItemRec();
				azeriteEmpoweredItemRec.Deserialize(str2);
				this.m_records.Add(azeriteEmpoweredItemRec.ID, azeriteEmpoweredItemRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}