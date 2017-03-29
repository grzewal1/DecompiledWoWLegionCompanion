using System;
using System.Collections;
using UnityEngine;

namespace WowStaticData
{
	public class GarrFollowerTypeDB
	{
		private Hashtable m_records;

		public GarrFollowerTypeDB()
		{
		}

		public void EnumRecords(Predicate<GarrFollowerTypeRec> callback)
		{
			IEnumerator enumerator = this.m_records.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (callback((GarrFollowerTypeRec)enumerator.Current))
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

		public GarrFollowerTypeRec GetRecord(int id)
		{
			return (GarrFollowerTypeRec)this.m_records[id];
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/GarrFollowerType.txt");
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
				GarrFollowerTypeRec garrFollowerTypeRec = new GarrFollowerTypeRec();
				garrFollowerTypeRec.Deserialize(str2);
				this.m_records.Add(garrFollowerTypeRec.ID, garrFollowerTypeRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}