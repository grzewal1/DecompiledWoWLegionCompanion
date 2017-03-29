using System;
using System.Runtime.CompilerServices;

namespace WowStaticData
{
	public class GarrAbilityEffectRec
	{
		public uint AbilityAction
		{
			get;
			private set;
		}

		public uint AbilityTargetType
		{
			get;
			private set;
		}

		public uint ActionHours
		{
			get;
			private set;
		}

		public uint ActionRace
		{
			get;
			private set;
		}

		public uint ActionRecordID
		{
			get;
			private set;
		}

		public float ActionValueFlat
		{
			get;
			private set;
		}

		public float CombatWeightBase
		{
			get;
			private set;
		}

		public float CombatWeightMax
		{
			get;
			private set;
		}

		public uint Flags
		{
			get;
			private set;
		}

		public uint GarrAbilityID
		{
			get;
			private set;
		}

		public uint GarrMechanicTypeID
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public GarrAbilityEffectRec()
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
					this.AbilityAction = Convert.ToUInt32(valueText);
					break;
				}
				case 2:
				{
					this.GarrAbilityID = Convert.ToUInt32(valueText);
					break;
				}
				case 3:
				{
					this.AbilityTargetType = Convert.ToUInt32(valueText);
					break;
				}
				case 4:
				{
					this.GarrMechanicTypeID = Convert.ToUInt32(valueText);
					break;
				}
				case 5:
				{
					this.Flags = Convert.ToUInt32(valueText);
					break;
				}
				case 6:
				{
					this.CombatWeightBase = (float)Convert.ToDouble(valueText);
					break;
				}
				case 7:
				{
					this.CombatWeightMax = (float)Convert.ToDouble(valueText);
					break;
				}
				case 8:
				{
					this.ActionValueFlat = (float)Convert.ToDouble(valueText);
					break;
				}
				case 9:
				{
					this.ActionRace = Convert.ToUInt32(valueText);
					break;
				}
				case 10:
				{
					this.ActionHours = Convert.ToUInt32(valueText);
					break;
				}
				case 11:
				{
					this.ActionRecordID = Convert.ToUInt32(valueText);
					break;
				}
			}
		}
	}
}