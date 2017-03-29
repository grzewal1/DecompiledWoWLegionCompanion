using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class CharShipmentRec
	{
		public uint ContainerID
		{
			get;
			private set;
		}

		public int DummyItemID
		{
			get;
			private set;
		}

		public uint Duration
		{
			get;
			private set;
		}

		public uint Flags
		{
			get;
			private set;
		}

		public uint GarrFollowerID
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public uint MaxShipments
		{
			get;
			private set;
		}

		public CharShipmentRec()
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
					this.ContainerID = Convert.ToUInt32(valueText);
					break;
				}
				case 2:
				{
					this.GarrFollowerID = Convert.ToUInt32(valueText);
					break;
				}
				case 3:
				{
					this.MaxShipments = Convert.ToUInt32(valueText);
					break;
				}
				case 4:
				{
					this.Duration = Convert.ToUInt32(valueText);
					break;
				}
				case 5:
				{
					this.DummyItemID = Convert.ToInt32(valueText);
					break;
				}
				case 6:
				{
					this.Flags = Convert.ToUInt32(valueText);
					break;
				}
			}
		}
	}
}