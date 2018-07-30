using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class UiTextureAtlasDB
	{
		private Dictionary<int, UiTextureAtlasRec> m_records = new Dictionary<int, UiTextureAtlasRec>();

		public UiTextureAtlasDB()
		{
		}

		public UiTextureAtlasRec GetRecord(int id)
		{
			UiTextureAtlasRec item;
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

		public UiTextureAtlasRec GetRecordFirstOrDefault(Func<UiTextureAtlasRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<UiTextureAtlasRec>(matcher);
		}

		public IEnumerable<UiTextureAtlasRec> GetRecordsWhere(Func<UiTextureAtlasRec, bool> matcher)
		{
			return this.m_records.Values.Where<UiTextureAtlasRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/UiTextureAtlas.txt");
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
				UiTextureAtlasRec uiTextureAtlasRec = new UiTextureAtlasRec();
				uiTextureAtlasRec.Deserialize(str2);
				this.m_records.Add(uiTextureAtlasRec.ID, uiTextureAtlasRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}