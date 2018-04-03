using System;
using System.Collections;
using UnityEngine;

namespace WowStaticData
{
	public class WorldMapAreaDB
	{
		private Hashtable m_records;

		public WorldMapAreaDB()
		{
		}

		public void EnumRecords(Predicate<WorldMapAreaRec> callback)
		{
			IEnumerator enumerator = this.m_records.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (callback((WorldMapAreaRec)enumerator.Current))
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

		public WorldMapAreaRec GetRecord(int id)
		{
			return (WorldMapAreaRec)this.m_records[id];
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/WorldMapArea.txt");
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
				WorldMapAreaRec worldMapAreaRec = new WorldMapAreaRec();
				worldMapAreaRec.Deserialize(str2);
				this.m_records.Add(worldMapAreaRec.ID, worldMapAreaRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}