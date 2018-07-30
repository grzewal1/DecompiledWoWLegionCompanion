using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class SpellDurationDB
	{
		private Dictionary<int, SpellDurationRec> m_records = new Dictionary<int, SpellDurationRec>();

		public SpellDurationDB()
		{
		}

		public SpellDurationRec GetRecord(int id)
		{
			SpellDurationRec item;
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

		public SpellDurationRec GetRecordFirstOrDefault(Func<SpellDurationRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<SpellDurationRec>(matcher);
		}

		public IEnumerable<SpellDurationRec> GetRecordsWhere(Func<SpellDurationRec, bool> matcher)
		{
			return this.m_records.Values.Where<SpellDurationRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/SpellDuration.txt");
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
				SpellDurationRec spellDurationRec = new SpellDurationRec();
				spellDurationRec.Deserialize(str2);
				this.m_records.Add(spellDurationRec.ID, spellDurationRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}