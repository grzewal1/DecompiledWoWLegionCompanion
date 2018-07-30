using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WowStaticData
{
	public class QuestV2DB
	{
		private Dictionary<int, QuestV2Rec> m_records = new Dictionary<int, QuestV2Rec>();

		public QuestV2DB()
		{
		}

		public QuestV2Rec GetRecord(int id)
		{
			QuestV2Rec item;
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

		public QuestV2Rec GetRecordFirstOrDefault(Func<QuestV2Rec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<QuestV2Rec>(matcher);
		}

		public IEnumerable<QuestV2Rec> GetRecordsWhere(Func<QuestV2Rec, bool> matcher)
		{
			return this.m_records.Values.Where<QuestV2Rec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/QuestV2_", locale, ".txt" });
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
				QuestV2Rec questV2Rec = new QuestV2Rec();
				questV2Rec.Deserialize(str2);
				this.m_records.Add(questV2Rec.ID, questV2Rec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}