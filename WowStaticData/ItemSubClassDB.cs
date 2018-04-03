using System;
using System.Collections;
using UnityEngine;

namespace WowStaticData
{
	public class ItemSubClassDB
	{
		private Hashtable m_records;

		public ItemSubClassDB()
		{
		}

		public void EnumRecords(Predicate<ItemSubClassRec> callback)
		{
			IEnumerator enumerator = this.m_records.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (callback((ItemSubClassRec)enumerator.Current))
					{
						continue;
					}
					break;
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				IDisposable disposable1 = disposable;
				if (disposable != null)
				{
					disposable1.Dispose();
				}
			}
		}

		public void EnumRecordsByParentID(int parentID, Predicate<ItemSubClassRec> callback)
		{
			IEnumerator enumerator = this.m_records.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ItemSubClassRec current = (ItemSubClassRec)enumerator.Current;
					if (current.ClassID != parentID || callback(current))
					{
						continue;
					}
					break;
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				IDisposable disposable1 = disposable;
				if (disposable != null)
				{
					disposable1.Dispose();
				}
			}
		}

		public ItemSubClassRec GetRecord(int id)
		{
			return (ItemSubClassRec)this.m_records[id];
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/ItemSubClass_", locale, ".txt" });
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
				ItemSubClassRec itemSubClassRec = new ItemSubClassRec();
				itemSubClassRec.Deserialize(str2);
				this.m_records.Add(itemSubClassRec.ID, itemSubClassRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}