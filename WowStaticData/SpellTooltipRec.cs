using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class SpellTooltipRec
	{
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

		public SpellTooltipRec()
		{
		}

		public void Deserialize(string valueLine)
		{
			int num = 0;
			num = valueLine.IndexOf('\t', 0);
			string str = valueLine.Substring(0, num).Trim();
			this.ID = Convert.ToInt32(str);
			this.Description = valueLine.Substring(num + 1).Trim();
		}
	}
}