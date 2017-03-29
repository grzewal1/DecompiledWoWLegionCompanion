using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobilePlayerJSON
{
	[DataContract]
	[FlexJamMessage(Id=4799, Name="MobilePlayerResearchGarrisonTalent", Version=38820897)]
	public class MobilePlayerResearchGarrisonTalent
	{
		[DataMember(Name="garrTalentID")]
		[FlexJamMember(Name="garrTalentID", Type=FlexJamType.Int32)]
		public int GarrTalentID
		{
			get;
			set;
		}

		public MobilePlayerResearchGarrisonTalent()
		{
		}
	}
}