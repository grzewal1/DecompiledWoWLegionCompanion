using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class ItemSubClassRec
	{
		public int ClassID
		{
			get;
			private set;
		}

		public int DisplayFlags
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		public int Flags
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public int SubClassID
		{
			get;
			private set;
		}

		public ItemSubClassRec()
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
					this.ClassID = Convert.ToInt32(valueText);
					break;
				}
				case 2:
				{
					this.SubClassID = Convert.ToInt32(valueText);
					break;
				}
				case 3:
				{
					this.Flags = Convert.ToInt32(valueText);
					break;
				}
				case 4:
				{
					this.DisplayFlags = Convert.ToInt32(valueText);
					break;
				}
				case 5:
				{
					this.DisplayName = valueText;
					break;
				}
			}
		}
	}
}