using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using WoWCompanionApp;
using WowStatConstants;

namespace WowStaticData
{
	public class CurrencyContainerDB
	{
		private Dictionary<int, CurrencyContainerRec> m_records = new Dictionary<int, CurrencyContainerRec>();

		public CurrencyContainerDB()
		{
		}

		public static CurrencyContainerRec CheckAndGetValidCurrencyContainer(int currencyType, int quantity)
		{
			CurrencyContainerRec currencyContainerRec;
			using (IEnumerator<CurrencyContainerRec> enumerator = StaticDB.currencyContainerDB.GetRecordsByParentID(currencyType).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CurrencyContainerRec current = enumerator.Current;
					if (!CurrencyContainerDB.IsCurrencyContainerValid(quantity, current.MinAmount, current.MaxAmount))
					{
						continue;
					}
					currencyContainerRec = current;
					return currencyContainerRec;
				}
				return null;
			}
			return currencyContainerRec;
		}

		public CurrencyContainerRec GetRecord(int id)
		{
			CurrencyContainerRec item;
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

		public CurrencyContainerRec GetRecordFirstOrDefault(Func<CurrencyContainerRec, bool> matcher)
		{
			return this.m_records.Values.FirstOrDefault<CurrencyContainerRec>(matcher);
		}

		public IEnumerable<CurrencyContainerRec> GetRecordsByParentID(int parentID)
		{
			return 
				from rec in this.m_records.Values
				where rec.CurrencyTypeId == parentID
				select rec;
		}

		public IEnumerable<CurrencyContainerRec> GetRecordsWhere(Func<CurrencyContainerRec, bool> matcher)
		{
			return this.m_records.Values.Where<CurrencyContainerRec>(matcher);
		}

		public static bool IsCurrencyContainerValid(int quantity, int minAmount, int maxAmount)
		{
			bool flag;
			if (minAmount > quantity)
			{
				flag = false;
			}
			else
			{
				flag = (maxAmount == 0 ? true : maxAmount >= quantity);
			}
			return flag;
		}

		public bool Load(string path, AssetBundle nonLocalizedBundle, AssetBundle localizedBundle, string locale)
		{
			string str = string.Concat(new string[] { path, locale, "/CurrencyContainer_", locale, ".txt" });
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
				CurrencyContainerRec currencyContainerRec = new CurrencyContainerRec();
				currencyContainerRec.Deserialize(str2);
				this.m_records.Add(currencyContainerRec.ID, currencyContainerRec);
				num1 = num + 1;
			}
			while (num > 0);
			return true;
		}

		public static Sprite LoadCurrencyContainerIcon(int currencyType, int quantity)
		{
			CurrencyContainerRec currencyContainerRec = CurrencyContainerDB.CheckAndGetValidCurrencyContainer(currencyType, quantity);
			if (currencyContainerRec == null)
			{
				return GeneralHelpers.LoadCurrencyIcon(currencyType);
			}
			return GeneralHelpers.LoadIconAsset(AssetBundleType.Icons, currencyContainerRec.ContainerIconID);
		}
	}
}