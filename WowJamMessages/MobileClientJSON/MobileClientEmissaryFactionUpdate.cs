using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4859, Name="MobileClientEmissaryFactionUpdate", Version=39869590)]
	public class MobileClientEmissaryFactionUpdate
	{
		[DataMember(Name="faction")]
		[FlexJamMember(ArrayDimensions=1, Name="faction", Type=FlexJamType.Struct)]
		public MobileEmissaryFaction[] Faction
		{
			get;
			set;
		}

		public MobileClientEmissaryFactionUpdate()
		{
		}
	}
}