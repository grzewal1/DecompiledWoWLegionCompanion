using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages
{
	[DataContract]
	[FlexJamStruct(Name="ObjectDebugInfo", Version=28333852)]
	public class ObjectDebugInfo
	{
		[DataMember(Name="attributeDescriptions")]
		[FlexJamMember(ArrayDimensions=1, Name="attributeDescriptions", Type=FlexJamType.Struct)]
		public DebugAttributeDescription[] AttributeDescriptions
		{
			get;
			set;
		}

		[DataMember(Name="attributes")]
		[FlexJamMember(ArrayDimensions=1, Name="attributes", Type=FlexJamType.Struct)]
		public DebugAttribute[] Attributes
		{
			get;
			set;
		}

		[DataMember(Name="facing")]
		[FlexJamMember(Name="facing", Type=FlexJamType.Float)]
		public float Facing
		{
			get;
			set;
		}

		[DataMember(Name="gameObjectDebugInfo")]
		[FlexJamMember(Optional=true, Name="gameObjectDebugInfo", Type=FlexJamType.Struct)]
		public WowJamMessages.GameObjectDebugInfo[] GameObjectDebugInfo
		{
			get;
			set;
		}

		[DataMember(Name="guid")]
		[FlexJamMember(Name="guid", Type=FlexJamType.WowGuid)]
		public string Guid
		{
			get;
			set;
		}

		[DataMember(Name="ID")]
		[FlexJamMember(Name="ID", Type=FlexJamType.Int32)]
		public int ID
		{
			get;
			set;
		}

		[DataMember(Name="initialized")]
		[FlexJamMember(Name="initialized", Type=FlexJamType.Bool)]
		public bool Initialized
		{
			get;
			set;
		}

		[DataMember(Name="mapID")]
		[FlexJamMember(Name="mapID", Type=FlexJamType.Int32)]
		public int MapID
		{
			get;
			set;
		}

		[DataMember(Name="name")]
		[FlexJamMember(Name="name", Type=FlexJamType.String)]
		public string Name
		{
			get;
			set;
		}

		[DataMember(Name="phaseInfo")]
		[FlexJamMember(ArrayDimensions=1, Name="phaseInfo", Type=FlexJamType.Struct)]
		public ObjectPhaseDebugInfo[] PhaseInfo
		{
			get;
			set;
		}

		[DataMember(Name="playerDebugInfo")]
		[FlexJamMember(Optional=true, Name="playerDebugInfo", Type=FlexJamType.Struct)]
		public WowJamMessages.PlayerDebugInfo[] PlayerDebugInfo
		{
			get;
			set;
		}

		[DataMember(Name="position")]
		[FlexJamMember(Name="position", Type=FlexJamType.Struct)]
		public Vector3 Position
		{
			get;
			set;
		}

		[DataMember(Name="rawFacing")]
		[FlexJamMember(Name="rawFacing", Type=FlexJamType.Float)]
		public float RawFacing
		{
			get;
			set;
		}

		[DataMember(Name="rawPosition")]
		[FlexJamMember(Name="rawPosition", Type=FlexJamType.Struct)]
		public Vector3 RawPosition
		{
			get;
			set;
		}

		[DataMember(Name="scriptTableValueDebugInfo")]
		[FlexJamMember(ArrayDimensions=1, Name="scriptTableValueDebugInfo", Type=FlexJamType.Struct)]
		public WowJamMessages.ScriptTableValueDebugInfo[] ScriptTableValueDebugInfo
		{
			get;
			set;
		}

		[DataMember(Name="typeID")]
		[FlexJamMember(Name="typeID", Type=FlexJamType.Int32)]
		public int TypeID
		{
			get;
			set;
		}

		[DataMember(Name="unitDebugInfo")]
		[FlexJamMember(Optional=true, Name="unitDebugInfo", Type=FlexJamType.Struct)]
		public WowJamMessages.UnitDebugInfo[] UnitDebugInfo
		{
			get;
			set;
		}

		[DataMember(Name="updateTime")]
		[FlexJamMember(Name="updateTime", Type=FlexJamType.Int32)]
		public int UpdateTime
		{
			get;
			set;
		}

		public ObjectDebugInfo()
		{
			this.Initialized = false;
			this.MapID = 0;
		}
	}
}