using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class GarrMissionTypeRec
	{
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

		public uint UiTextureAtlasMemberID
		{
			get;
			private set;
		}

		public uint UiTextureKitID
		{
			get;
			private set;
		}

		public GarrMissionTypeRec()
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
					this.Name = valueText;
					break;
				}
				case 2:
				{
					this.UiTextureAtlasMemberID = Convert.ToUInt32(valueText);
					break;
				}
				case 3:
				{
					this.UiTextureKitID = Convert.ToUInt32(valueText);
					break;
				}
			}
		}
	}
}