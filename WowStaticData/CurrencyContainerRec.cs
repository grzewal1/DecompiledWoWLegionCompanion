using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class CurrencyContainerRec
	{
		public string ContainerDescription
		{
			get;
			private set;
		}

		public int ContainerIconID
		{
			get;
			private set;
		}

		public string ContainerName
		{
			get;
			private set;
		}

		public int ContainerQuality
		{
			get;
			private set;
		}

		public int CurrencyTypeId
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public int MaxAmount
		{
			get;
			private set;
		}

		public int MinAmount
		{
			get;
			private set;
		}

		public CurrencyContainerRec()
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
					this.CurrencyTypeId = Convert.ToInt32(valueText);
					break;
				}
				case 2:
				{
					this.MinAmount = Convert.ToInt32(valueText);
					break;
				}
				case 3:
				{
					this.MaxAmount = Convert.ToInt32(valueText);
					break;
				}
				case 4:
				{
					this.ContainerIconID = Convert.ToInt32(valueText);
					break;
				}
				case 5:
				{
					this.ContainerName = valueText;
					break;
				}
				case 6:
				{
					this.ContainerDescription = valueText;
					break;
				}
				case 7:
				{
					this.ContainerQuality = Convert.ToInt32(valueText);
					break;
				}
			}
		}
	}
}