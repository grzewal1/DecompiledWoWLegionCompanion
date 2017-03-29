using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4798, Name="MobilePlayerCanResearchGarrisonTalent", Version=38820897)]
	public class MobilePlayerCanResearchGarrisonTalent
	{
		[DataMember(Name="garrTalentID")]
		[FlexJamMember(Name="garrTalentID", Type=FlexJamType.Int32)]
		public int GarrTalentID
		{
			get;
			set;
		}

		public MobilePlayerCanResearchGarrisonTalent()
		{
		}
	}
}