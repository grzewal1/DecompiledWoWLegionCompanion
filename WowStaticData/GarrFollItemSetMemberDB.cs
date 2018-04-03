using System;
using System.Collections;
using UnityEngine;

namespace WowStaticData
{
	public class GarrFollItemSetMemberDB
	{
		private Hashtable m_records;

		public GarrFollItemSetMemberDB()
		{
		}

		public void EnumRecords(Predicate<GarrFollItemSetMemberRec> callback)
		{
			IEnumerator enumerator = this.m_records.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (callback((GarrFollItemSetMemberRec)enumerator.Current))
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

		public void EnumRecordsByParentID(int parentID, Predicate<GarrFollItemSetMemberRec> callback)
		{
			IEnumerator enumerator = this.m_records.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					GarrFollItemSetMemberRec current = (GarrFollItemSetMemberRec)enumerator.Current;
					if ((ulong)current.GarrFollItemSetID != (long)parentID || callback(current))
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

		public GarrFollItemSetMemberRec GetRecord(int id)
		{
			return (GarrFollItemSetMemberRec)this.m_records[id];
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/GarrFollItemSetMember.txt");
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
				GarrFollItemSetMemberRec garrFollItemSetMemberRec = new GarrFollItemSetMemberRec();
				garrFollItemSetMemberRec.Deserialize(str2);
				this.m_records.Add(garrFollItemSetMemberRec.ID, garrFollItemSetMemberRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}