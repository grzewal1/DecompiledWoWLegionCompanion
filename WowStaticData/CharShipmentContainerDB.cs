using System;
using System.Collections;
using UnityEngine;

namespace WowStaticData
{
	public class CharShipmentContainerDB
	{
		private Hashtable m_records;

		public CharShipmentContainerDB()
		{
		}

		public void EnumRecords(Predicate<CharShipmentContainerRec> callback)
		{
			IEnumerator enumerator = this.m_records.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (callback((CharShipmentContainerRec)enumerator.Current))
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

		public CharShipmentContainerRec GetRecord(int id)
		{
			return (CharShipmentContainerRec)this.m_records[id];
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/CharShipmentContainer_", locale, ".txt" });
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
				CharShipmentContainerRec charShipmentContainerRec = new CharShipmentContainerRec();
				charShipmentContainerRec.Deserialize(str2);
				this.m_records.Add(charShipmentContainerRec.ID, charShipmentContainerRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}