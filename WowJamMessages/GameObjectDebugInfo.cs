using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="GameObjectDebugInfo", Version=28333852)]
	public class GameObjectDebugInfo
	{
		[DataMember(Name="debugName")]
		[FlexJamMember(Name="debugName", Type=FlexJamType.String)]
		public string DebugName
		{
			get;
			set;
		}

		[DataMember(Name="flags")]
		[FlexJamMember(Name="flags", Type=FlexJamType.UInt32)]
		public uint Flags
		{
			get;
			set;
		}

		[DataMember(Name="gameObjectType")]
		[FlexJamMember(Name="gameObjectType", Type=FlexJamType.Int32)]
		public int GameObjectType
		{
			get;
			set;
		}

		[DataMember(Name="health")]
		[FlexJamMember(Name="health", Type=FlexJamType.Float)]
		public float Health
		{
			get;
			set;
		}

		[DataMember(Name="state")]
		[FlexJamMember(Name="state", Type=FlexJamType.Int32)]
		public int State
		{
			get;
			set;
		}

		public GameObjectDebugInfo()
		{
		}
	}
}