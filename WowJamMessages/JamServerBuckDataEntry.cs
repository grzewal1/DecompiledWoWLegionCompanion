using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamServerBuckDataEntry", Version=28333852)]
	public class JamServerBuckDataEntry
	{
		[DataMember(Name="accum")]
		[FlexJamMember(Name="accum", Type=FlexJamType.UInt64)]
		public ulong Accum
		{
			get;
			set;
		}

		[DataMember(Name="arg")]
		[FlexJamMember(Name="arg", Type=FlexJamType.UInt64)]
		public ulong Arg
		{
			get;
			set;
		}

		[DataMember(Name="argname")]
		[FlexJamMember(Name="argname", Type=FlexJamType.String)]
		public string Argname
		{
			get;
			set;
		}

		[DataMember(Name="count")]
		[FlexJamMember(Name="count", Type=FlexJamType.UInt64)]
		public ulong Count
		{
			get;
			set;
		}

		[DataMember(Name="maximum")]
		[FlexJamMember(Name="maximum", Type=FlexJamType.UInt64)]
		public ulong Maximum
		{
			get;
			set;
		}

		[DataMember(Name="minimum")]
		[FlexJamMember(Name="minimum", Type=FlexJamType.UInt64)]
		public ulong Minimum
		{
			get;
			set;
		}

		[DataMember(Name="sqaccum")]
		[FlexJamMember(Name="sqaccum", Type=FlexJamType.UInt64)]
		public ulong Sqaccum
		{
			get;
			set;
		}

		public JamServerBuckDataEntry()
		{
			this.Arg = (ulong)0;
			this.Argname = string.Empty;
			this.Count = (ulong)0;
			this.Accum = (ulong)0;
			this.Sqaccum = (ulong)0;
			this.Maximum = (ulong)0;
			this.Minimum = (ulong)2000000000;
		}
	}
}