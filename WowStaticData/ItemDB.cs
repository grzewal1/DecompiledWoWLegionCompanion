using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class ItemDB
	{
		private Dictionary<int, ItemRec> m_records = new Dictionary<int, ItemRec>();

		public ItemDB()
		{
		}

		public ItemRec GetRecord(int id)
		{
			ItemRec item;
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

		public ItemRec GetRecordFirstOrDefault(Func<ItemRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<ItemRec>(matcher);
		}

		public IEnumerable<ItemRec> GetRecordsWhere(Func<ItemRec, bool> matcher)
		{
			return this.m_records.Values.Where<ItemRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/Item_", locale, ".txt" });
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
				ItemRec itemRec = new ItemRec();
				itemRec.Deserialize(str2);
				this.m_records.Add(itemRec.ID, itemRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}