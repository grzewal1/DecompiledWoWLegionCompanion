using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="JamBattlenetRequestHeader", Version=28333852)]
	public class JamBattlenetRequestHeader
	{
		[DataMember(Name="methodType")]
		[FlexJamMember(Name="methodType", Type=FlexJamType.UInt64)]
		public ulong MethodType
		{
			get;
			set;
		}

		[DataMember(Name="objectID")]
		[FlexJamMember(Name="objectID", Type=FlexJamType.UInt64)]
		public ulong ObjectID
		{
			get;
			set;
		}

		[DataMember(Name="token")]
		[FlexJamMember(Name="token", Type=FlexJamType.UInt32)]
		public uint Token
		{
			get;
			set;
		}

		public JamBattlenetRequestHeader()
		{
			this.ObjectID = (ulong)0;
		}
	}
}