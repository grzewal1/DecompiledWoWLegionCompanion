using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileEmissaryFaction", Version=39869590)]
	public class MobileEmissaryFaction
	{
		[DataMember(Name="factionAmount")]
		[FlexJamMember(Name="factionAmount", Type=FlexJamType.Int32)]
		public int FactionAmount
		{
			get;
			set;
		}

		[DataMember(Name="factionID")]
		[FlexJamMember(Name="factionID", Type=FlexJamType.UInt16)]
		public ushort FactionID
		{
			get;
			set;
		}

		public MobileEmissaryFaction()
		{
		}
	}
}