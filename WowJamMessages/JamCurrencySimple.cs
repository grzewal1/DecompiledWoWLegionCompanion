using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamCurrencySimple", Version=28333852)]
	public class JamCurrencySimple
	{
		[DataMember(Name="quantity")]
		[FlexJamMember(Name="quantity", Type=FlexJamType.Int32)]
		public int Quantity
		{
			get;
			set;
		}

		[DataMember(Name="type")]
		[FlexJamMember(Name="type", Type=FlexJamType.Int32)]
		public int Type
		{
			get;
			set;
		}

		public JamCurrencySimple()
		{
		}
	}
}