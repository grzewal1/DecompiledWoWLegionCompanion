using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4841, Name="MobileClientTestResult", Version=39869590)]
	public class MobileClientTestResult
	{
		[DataMember(Name="result")]
		[FlexJamMember(Name="result", Type=FlexJamType.String)]
		public string Result
		{
			get;
			set;
		}

		public MobileClientTestResult()
		{
		}
	}
}