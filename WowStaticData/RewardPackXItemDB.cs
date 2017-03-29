using System;
using System.Collections;
using UnityEngine;

namespace WowStaticData
{
	public class RewardPackXItemDB
	{
		private Hashtable m_records;

		public RewardPackXItemDB()
		{
		}

		public void EnumRecords(Predicate<RewardPackXItemRec> callback)
		{
			IEnumerator enumerator = this.m_records.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (callback((RewardPackXItemRec)enumerator.Current))
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

		public void EnumRecordsByParentID(int parentID, Predicate<RewardPackXItemRec> callback)
		{
			IEnumerator enumerator = this.m_records.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RewardPackXItemRec current = (RewardPackXItemRec)enumerator.Current;
					if (current.RewardPackID != parentID || callback(current))
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

		public RewardPackXItemRec GetRecord(int id)
		{
			return (RewardPackXItemRec)this.m_records[id];
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/RewardPackXItem.txt");
			if (this.m_records != null)
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
				RewardPackXItemRec rewardPackXItemRec = new RewardPackXItemRec();
				rewardPackXItemRec.Deserialize(str2);
				this.m_records.Add(rewardPackXItemRec.ID, rewardPackXItemRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}