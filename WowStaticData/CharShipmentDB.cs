using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WowStaticData
{
	public class CharShipmentDB
	{
		private Dictionary<int, CharShipmentRec> m_records = new Dictionary<int, CharShipmentRec>();

		public CharShipmentDB()
		{
		}

		public CharShipmentRec GetRecord(int id)
		{
			CharShipmentRec item;
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

		public CharShipmentRec GetRecordFirstOrDefault(Func<CharShipmentRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<CharShipmentRec>(matcher);
		}

		public IEnumerable<CharShipmentRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where (ulong)rec.ContainerID == (long)parentID
				select rec;
		}

		public IEnumerable<CharShipmentRec> GetRecordsWhere(Func<CharShipmentRec, bool> matcher)
		{
			return this.m_records.Values.Where<CharShipmentRec>(matcher);
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(path, "NonLocalized/CharShipment.txt");
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
				CharShipmentRec charShipmentRec = new CharShipmentRec();
				charShipmentRec.Deserialize(str2);
				this.m_records.Add(charShipmentRec.ID, charShipmentRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}
	}
}