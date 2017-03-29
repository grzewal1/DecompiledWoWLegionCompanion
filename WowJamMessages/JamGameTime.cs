using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamGameTime", Version=28333852)]
	public class JamGameTime
	{
		[DataMember(Name="billingType")]
		[FlexJamMember(Name="billingType", Type=FlexJamType.Int32)]
		public int BillingType
		{
			get;
			set;
		}

		[DataMember(Name="isCAISEnabled")]
		[FlexJamMember(Name="isCAISEnabled", Type=FlexJamType.Bool)]
		public bool IsCAISEnabled
		{
			get;
			set;
		}

		[DataMember(Name="isInIGR")]
		[FlexJamMember(Name="isInIGR", Type=FlexJamType.Bool)]
		public bool IsInIGR
		{
			get;
			set;
		}

		[DataMember(Name="isPaidForByIGR")]
		[FlexJamMember(Name="isPaidForByIGR", Type=FlexJamType.Bool)]
		public bool IsPaidForByIGR
		{
			get;
			set;
		}

		[DataMember(Name="minutesRemaining")]
		[FlexJamMember(Name="minutesRemaining", Type=FlexJamType.UInt32)]
		public uint MinutesRemaining
		{
			get;
			set;
		}

		public JamGameTime()
		{
			this.BillingType = 0;
			this.MinutesRemaining = 0;
			this.IsInIGR = false;
			this.IsPaidForByIGR = false;
			this.IsCAISEnabled = false;
		}
	}
}