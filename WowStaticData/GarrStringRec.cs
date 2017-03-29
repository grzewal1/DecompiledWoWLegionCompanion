using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class GarrStringRec
	{
		public int ID
		{
			get;
			private set;
		}

		public string Text
		{
			get;
			private set;
		}

		public GarrStringRec()
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
			int num = index;
			if (num == 0)
			{
				this.ID = Convert.ToInt32(valueText);
			}
			else if (num == 1)
			{
				this.Text = valueText;
			}
		}
	}
}