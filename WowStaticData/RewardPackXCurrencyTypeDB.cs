using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class RewardPackXCurrencyTypeDB
	{
		private Dictionary<int, RewardPackXCurrencyTypeRec> m_records = new Dictionary<int, RewardPackXCurrencyTypeRec>();

		public RewardPackXCurrencyTypeDB()
		{
		}

		public RewardPackXCurrencyTypeRec GetRecord(int id)
		{
			RewardPackXCurrencyTypeRec item;
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

		public RewardPackXCurrencyTypeRec GetRecordFirstOrDefault(Func<RewardPackXCurrencyTypeRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<RewardPackXCurrencyTypeRec>(matcher);
		}

		public IEnumerable<RewardPackXCurrencyTypeRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where rec.RewardPackID == parentID
				select rec;
		}

		public IEnumerable<RewardPackXCurrencyTypeRec> GetRecordsWhere(Func<RewardPackXCurrencyTypeRec, bool> matcher)
		{
			return this.m_records.Values.Where<RewardPackXCurrencyTypeRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/RewardPackXCurrencyType.txt");
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
				RewardPackXCurrencyTypeRec rewardPackXCurrencyTypeRec = new RewardPackXCurrencyTypeRec();
				rewardPackXCurrencyTypeRec.Deserialize(str2);
				this.m_records.Add(rewardPackXCurrencyTypeRec.ID, rewardPackXCurrencyTypeRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}