using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class GarrFollowerQualityRec
	{
		public int ClassSpecID
		{
			get;
			private set;
		}

		public uint GarrFollowerTypeID
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public uint Quality
		{
			get;
			private set;
		}

		public uint ShipmentXP
		{
			get;
			private set;
		}

		public uint XpToNextQuality
		{
			get;
			private set;
		}

		public GarrFollowerQualityRec()
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
					this.Quality = Convert.ToUInt32(valueText);
					break;
				}
				case 2:
				{
					this.XpToNextQuality = Convert.ToUInt32(valueText);
					break;
				}
				case 3:
				{
					this.ShipmentXP = Convert.ToUInt32(valueText);
					break;
				}
				case 4:
				{
					this.GarrFollowerTypeID = Convert.ToUInt32(valueText);
					break;
				}
				case 5:
				{
					this.ClassSpecID = Convert.ToInt32(valueText);
					break;
				}
			}
		}
	}
}