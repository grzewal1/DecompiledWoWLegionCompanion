using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class ItemSubClassDB
	{
		private Dictionary<int, ItemSubClassRec> m_records = new Dictionary<int, ItemSubClassRec>();

		public ItemSubClassDB()
		{
		}

		public ItemSubClassRec GetRecord(int id)
		{
			ItemSubClassRec item;
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

		public ItemSubClassRec GetRecordFirstOrDefault(Func<ItemSubClassRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<ItemSubClassRec>(matcher);
		}

		public IEnumerable<ItemSubClassRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where rec.ClassID == parentID
				select rec;
		}

		public IEnumerable<ItemSubClassRec> GetRecordsWhere(Func<ItemSubClassRec, bool> matcher)
		{
			return this.m_records.Values.Where<ItemSubClassRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/ItemSubClass_", locale, ".txt" });
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
				ItemSubClassRec itemSubClassRec = new ItemSubClassRec();
				itemSubClassRec.Deserialize(str2);
				this.m_records.Add(itemSubClassRec.ID, itemSubClassRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}