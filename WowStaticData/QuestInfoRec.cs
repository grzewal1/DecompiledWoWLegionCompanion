using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class QuestInfoRec
	{
		public int ID
		{
			get;
			private set;
		}

		public string InfoName
		{
			get;
			private set;
		}

		public int Modifiers
		{
			get;
			private set;
		}

		public int Profession
		{
			get;
			private set;
		}

		public int Type
		{
			get;
			private set;
		}

		public QuestInfoRec()
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
					this.InfoName = valueText;
					break;
				}
				case 2:
				{
					this.Type = Convert.ToInt32(valueText);
					break;
				}
				case 3:
				{
					this.Modifiers = Convert.ToInt32(valueText);
					break;
				}
				case 4:
				{
					this.Profession = Convert.ToInt32(valueText);
					break;
				}
			}
		}
	}
}