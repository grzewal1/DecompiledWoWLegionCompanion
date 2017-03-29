using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4871, Name="MobileClientResearchGarrisonTalentResult", Version=39869590)]
	public class MobileClientResearchGarrisonTalentResult
	{
		[DataMember(Name="garrTalentID")]
		[FlexJamMember(Name="garrTalentID", Type=FlexJamType.Int32)]
		public int GarrTalentID
		{
			get;
			set;
		}

		[DataMember(Name="result")]
		[FlexJamMember(Name="result", Type=FlexJamType.Int32)]
		public int Result
		{
			get;
			set;
		}

		public MobileClientResearchGarrisonTalentResult()
		{
		}
	}
}