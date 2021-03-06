using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class GarrTalentRec
	{
		public string Description
		{
			get;
			private set;
		}

		public int Flags
		{
			get;
			private set;
		}

		public uint GarrAbilityID
		{
			get;
			private set;
		}

		public uint GarrTalentTreeID
		{
			get;
			private set;
		}

		public int IconFileDataID
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public uint PerkSpellID
		{
			get;
			private set;
		}

		public int ResearchCost
		{
			get;
			private set;
		}

		public uint ResearchCostCurrencyTypesID
		{
			get;
			private set;
		}

		public int ResearchDurationSecs
		{
			get;
			private set;
		}

		public int ResearchGoldCost
		{
			get;
			private set;
		}

		public int RespecCost
		{
			get;
			private set;
		}

		public uint RespecCostCurrencyTypesID
		{
			get;
			private set;
		}

		public int RespecDurationSecs
		{
			get;
			private set;
		}

		public int RespecGoldCost
		{
			get;
			private set;
		}

		public int Tier
		{
			get;
			private set;
		}

		public int UiOrder
		{
			get;
			private set;
		}

		public GarrTalentRec()
		{
		}

		public void Deserialize(string valueLine)
		{
			int num = 0;
			int num1 = 0;
			int num2 = 0;
			do
			{
				num = valueLine.IndexOf('\t', num1);
				if (num >= 0)
				{
					string str = valueLine.Substring(num1, num - num1).Trim();
					this.DeserializeIndex(num2, str);
					num2++;
				}
				num1 = num + 1;
			}
			while (num > 0);
		}

		private void DeserializeIndex(int index, string valueText)
		{
			switch (index)
			{
				case 0:
				{
					this.ID = Convert.ToInt32(valueText);
					break;
				}
				case 1:
				{
					this.GarrTalentTreeID = Convert.ToUInt32(valueText);
					break;
				}
				case 2:
				{
					this.Tier = Convert.ToInt32(valueText);
					break;
				}
				case 3:
				{
					this.UiOrder = Convert.ToInt32(valueText);
					break;
				}
				case 4:
				{
					this.IconFileDataID = Convert.ToInt32(valueText);
					break;
				}
				case 5:
				{
					this.Name = valueText;
					break;
				}
				case 6:
				{
					this.Description = valueText;
					break;
				}
				case 7:
				{
					this.GarrAbilityID = Convert.ToUInt32(valueText);
					break;
				}
				case 8:
				{
					this.ResearchCost = Convert.ToInt32(valueText);
					break;
				}
				case 9:
				{
					this.ResearchCostCurrencyTypesID = Convert.ToUInt32(valueText);
					break;
				}
				case 10:
				{
					this.ResearchDurationSecs = Convert.ToInt32(valueText);
					break;
				}
				case 11:
				{
					this.ResearchGoldCost = Convert.ToInt32(valueText);
					break;
				}
				case 12:
				{
					this.Flags = Convert.ToInt32(valueText);
					break;
				}
				case 13:
				{
					this.PerkSpellID = Convert.ToUInt32(valueText);
					break;
				}
				case 14:
				{
					this.RespecCost = Convert.ToInt32(valueText);
					break;
				}
				case 15:
				{
					this.RespecCostCurrencyTypesID = Convert.ToUInt32(valueText);
					break;
				}
				case 16:
				{
					this.RespecDurationSecs = Convert.ToInt32(valueText);
					break;
				}
				case 17:
				{
					this.RespecGoldCost = Convert.ToInt32(valueText);
					break;
				}
			}
		}
	}
}