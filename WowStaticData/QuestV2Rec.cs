using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class QuestV2Rec
	{
		public int AreaID
		{
			get;
			private set;
		}

		public string Description
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public string LogDescription
		{
			get;
			private set;
		}

		public int QuestInfoID
		{
			get;
			private set;
		}

		public int QuestSortID
		{
			get;
			private set;
		}

		public string QuestTitle
		{
			get;
			private set;
		}

		public string RewardText
		{
			get;
			private set;
		}

		public QuestV2Rec()
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
					this.QuestTitle = valueText;
					break;
				}
				case 2:
				{
					this.Description = valueText;
					break;
				}
				case 3:
				{
					this.LogDescription = valueText;
					break;
				}
				case 4:
				{
					this.RewardText = valueText;
					break;
				}
				case 5:
				{
					this.AreaID = Convert.ToInt32(valueText);
					break;
				}
				case 6:
				{
					this.QuestInfoID = Convert.ToInt32(valueText);
					break;
				}
				case 7:
				{
					this.QuestSortID = Convert.ToInt32(valueText);
					break;
				}
			}
		}
	}
}