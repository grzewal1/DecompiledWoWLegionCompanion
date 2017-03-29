using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class GarrClassSpecRec
	{
		public string ClassSpec
		{
			get;
			private set;
		}

		public string ClassSpec_Female
		{
			get;
			private set;
		}

		public string ClassSpec_Male
		{
			get;
			private set;
		}

		public int Flags
		{
			get;
			private set;
		}

		public uint FollowerClassLimit
		{
			get;
			private set;
		}

		public uint GarrFollItemSetID
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public uint UiTextureAtlasMemberID
		{
			get;
			private set;
		}

		public GarrClassSpecRec()
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
					this.ClassSpec = valueText;
					break;
				}
				case 2:
				{
					this.ClassSpec_Male = valueText;
					break;
				}
				case 3:
				{
					this.ClassSpec_Female = valueText;
					break;
				}
				case 4:
				{
					this.UiTextureAtlasMemberID = Convert.ToUInt32(valueText);
					break;
				}
				case 5:
				{
					this.GarrFollItemSetID = Convert.ToUInt32(valueText);
					break;
				}
				case 6:
				{
					this.Flags = Convert.ToInt32(valueText);
					break;
				}
				case 7:
				{
					this.FollowerClassLimit = Convert.ToUInt32(valueText);
					break;
				}
			}
		}
	}
}