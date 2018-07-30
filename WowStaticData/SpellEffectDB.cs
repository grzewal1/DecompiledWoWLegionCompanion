using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class SpellEffectDB
	{
		private Dictionary<int, SpellEffectRec> m_records = new Dictionary<int, SpellEffectRec>();

		public SpellEffectDB()
		{
		}

		public SpellEffectRec GetRecord(int id)
		{
			SpellEffectRec item;
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

		public SpellEffectRec GetRecordFirstOrDefault(Func<SpellEffectRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<SpellEffectRec>(matcher);
		}

		public IEnumerable<SpellEffectRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where rec.SpellID == parentID
				select rec;
		}

		public IEnumerable<SpellEffectRec> GetRecordsWhere(Func<SpellEffectRec, bool> matcher)
		{
			return this.m_records.Values.Where<SpellEffectRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/SpellEffect.txt");
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
				SpellEffectRec spellEffectRec = new SpellEffectRec();
				spellEffectRec.Deserialize(str2);
				this.m_records.Add(spellEffectRec.ID, spellEffectRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}