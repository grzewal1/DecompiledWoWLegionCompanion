using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class CreatureDB
	{
		private Dictionary<int, CreatureRec> m_records = new Dictionary<int, CreatureRec>();

		public CreatureDB()
		{
		}

		public CreatureRec GetRecord(int id)
		{
			CreatureRec item;
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

		public CreatureRec GetRecordFirstOrDefault(Func<CreatureRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<CreatureRec>(matcher);
		}

		public IEnumerable<CreatureRec> GetRecordsWhere(Func<CreatureRec, bool> matcher)
		{
			return this.m_records.Values.Where<CreatureRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/Creature_", locale, ".txt" });
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
				CreatureRec creatureRec = new CreatureRec();
				creatureRec.Deserialize(str2);
				this.m_records.Add(creatureRec.ID, creatureRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}