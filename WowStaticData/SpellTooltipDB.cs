using System;
using System.Collections;
using UnityEngine;

namespace WowStaticData
{
	public class SpellTooltipDB
	{
		private Hashtable m_records;

		public SpellTooltipDB()
		{
		}

		public void EnumRecords(Predicate<SpellTooltipRec> callback)
		{
			IEnumerator enumerator = this.m_records.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (callback((SpellTooltipRec)enumerator.Current))
					{
						continue;
					}
					break;
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
		}

		public SpellTooltipRec GetRecord(int id)
		{
			return (SpellTooltipRec)this.m_records[id];
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/SpellTooltip_", locale, ".txt" });
			if (this.m_records != null)
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
			this.m_records = new Hashtable();
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
				SpellTooltipRec spellTooltipRec = new SpellTooltipRec();
				spellTooltipRec.Deserialize(str2);
				this.m_records.Add(spellTooltipRec.ID, spellTooltipRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}