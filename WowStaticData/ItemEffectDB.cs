using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class ItemEffectDB
	{
		private Dictionary<int, ItemEffectRec> m_records = new Dictionary<int, ItemEffectRec>();

		public ItemEffectDB()
		{
		}

		public ItemEffectRec GetRecord(int id)
		{
			ItemEffectRec item;
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

		public ItemEffectRec GetRecordFirstOrDefault(Func<ItemEffectRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<ItemEffectRec>(matcher);
		}

		public IEnumerable<ItemEffectRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where rec.ParentItemID == parentID
				select rec;
		}

		public IEnumerable<ItemEffectRec> GetRecordsWhere(Func<ItemEffectRec, bool> matcher)
		{
			return this.m_records.Values.Where<ItemEffectRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/ItemEffect.txt");
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
				ItemEffectRec itemEffectRec = new ItemEffectRec();
				itemEffectRec.Deserialize(str2);
				this.m_records.Add(itemEffectRec.ID, itemEffectRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}