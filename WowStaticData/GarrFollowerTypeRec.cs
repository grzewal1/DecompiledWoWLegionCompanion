using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class GarrFollowerTypeRec
	{
		public uint Flags
		{
			get;
			private set;
		}

		public uint GarrTypeID
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public uint ItemLevelRangeBias
		{
			get;
			private set;
		}

		public uint LevelRangeBias
		{
			get;
			private set;
		}

		public uint MaxFollowerBuildingType
		{
			get;
			private set;
		}

		public uint MaxFollowers
		{
			get;
			private set;
		}

		public uint MaxItemLevel
		{
			get;
			private set;
		}

		public GarrFollowerTypeRec()
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
					this.MaxFollowers = Convert.ToUInt32(valueText);
					break;
				}
				case 2:
				{
					this.MaxFollowerBuildingType = Convert.ToUInt32(valueText);
					break;
				}
				case 3:
				{
					this.MaxItemLevel = Convert.ToUInt32(valueText);
					break;
				}
				case 4:
				{
					this.GarrTypeID = Convert.ToUInt32(valueText);
					break;
				}
				case 5:
				{
					this.LevelRangeBias = Convert.ToUInt32(valueText);
					break;
				}
				case 6:
				{
					this.ItemLevelRangeBias = Convert.ToUInt32(valueText);
					break;
				}
				case 7:
				{
					this.Flags = Convert.ToUInt32(valueText);
					break;
				}
			}
		}
	}
}