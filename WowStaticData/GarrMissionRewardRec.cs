using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class GarrMissionRewardRec
	{
		public uint CurrencyQuantity
		{
			get;
			private set;
		}

		public uint CurrencyType
		{
			get;
			private set;
		}

		public uint FollowerXP
		{
			get;
			private set;
		}

		public uint GarrMissionID
		{
			get;
			private set;
		}

		public uint GarrMssnBonusAbilityID
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public int ItemID
		{
			get;
			private set;
		}

		public uint ItemQuantity
		{
			get;
			private set;
		}

		public uint TreasureChance
		{
			get;
			private set;
		}

		public uint TreasureQuality
		{
			get;
			private set;
		}

		public uint TreasureXP
		{
			get;
			private set;
		}

		public GarrMissionRewardRec()
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
					this.GarrMissionID = Convert.ToUInt32(valueText);
					break;
				}
				case 2:
				{
					this.FollowerXP = Convert.ToUInt32(valueText);
					break;
				}
				case 3:
				{
					this.ItemID = Convert.ToInt32(valueText);
					break;
				}
				case 4:
				{
					this.ItemQuantity = Convert.ToUInt32(valueText);
					break;
				}
				case 5:
				{
					this.CurrencyType = Convert.ToUInt32(valueText);
					break;
				}
				case 6:
				{
					this.CurrencyQuantity = Convert.ToUInt32(valueText);
					break;
				}
				case 7:
				{
					this.TreasureChance = Convert.ToUInt32(valueText);
					break;
				}
				case 8:
				{
					this.TreasureXP = Convert.ToUInt32(valueText);
					break;
				}
				case 9:
				{
					this.TreasureQuality = Convert.ToUInt32(valueText);
					break;
				}
				case 10:
				{
					this.GarrMssnBonusAbilityID = Convert.ToUInt32(valueText);
					break;
				}
			}
		}
	}
}