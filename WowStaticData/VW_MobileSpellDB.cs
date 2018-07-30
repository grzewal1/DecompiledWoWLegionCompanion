using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class VW_MobileSpellDB
	{
		private Dictionary<int, VW_MobileSpellRec> m_records = new Dictionary<int, VW_MobileSpellRec>();

		public VW_MobileSpellDB()
		{
		}

		public VW_MobileSpellRec GetRecord(int id)
		{
			VW_MobileSpellRec item;
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

		public VW_MobileSpellRec GetRecordFirstOrDefault(Func<VW_MobileSpellRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<VW_MobileSpellRec>(matcher);
		}

		public IEnumerable<VW_MobileSpellRec> GetRecordsWhere(Func<VW_MobileSpellRec, bool> matcher)
		{
			return this.m_records.Values.Where<VW_MobileSpellRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/VW_MobileSpell_", locale, ".txt" });
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
				VW_MobileSpellRec vWMobileSpellRec = new VW_MobileSpellRec();
				vWMobileSpellRec.Deserialize(str2);
				this.m_records.Add(vWMobileSpellRec.ID, vWMobileSpellRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}