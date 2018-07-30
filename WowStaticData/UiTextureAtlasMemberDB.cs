using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class UiTextureAtlasMemberDB
	{
		private Dictionary<int, UiTextureAtlasMemberRec> m_records = new Dictionary<int, UiTextureAtlasMemberRec>();

		public UiTextureAtlasMemberDB()
		{
		}

		public UiTextureAtlasMemberRec GetRecord(int id)
		{
			UiTextureAtlasMemberRec item;
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

		public UiTextureAtlasMemberRec GetRecordFirstOrDefault(Func<UiTextureAtlasMemberRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<UiTextureAtlasMemberRec>(matcher);
		}

		public IEnumerable<UiTextureAtlasMemberRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where (ulong)rec.UiTextureAtlasID == (long)parentID
				select rec;
		}

		public IEnumerable<UiTextureAtlasMemberRec> GetRecordsWhere(Func<UiTextureAtlasMemberRec, bool> matcher)
		{
			return this.m_records.Values.Where<UiTextureAtlasMemberRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/UiTextureAtlasMember.txt");
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
				UiTextureAtlasMemberRec uiTextureAtlasMemberRec = new UiTextureAtlasMemberRec();
				uiTextureAtlasMemberRec.Deserialize(str2);
				this.m_records.Add(uiTextureAtlasMemberRec.ID, uiTextureAtlasMemberRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}