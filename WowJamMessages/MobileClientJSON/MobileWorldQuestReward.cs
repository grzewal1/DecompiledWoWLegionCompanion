using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamStruct(Name="MobileWorldQuestReward", Version=39869590)]
	public class MobileWorldQuestReward
	{
		[DataMember(Name="fileDataID")]
		[FlexJamMember(Name="fileDataID", Type=FlexJamType.Int32)]
		public int FileDataID
		{
			get;
			set;
		}

		[DataMember(Name="itemContext")]
		[FlexJamMember(Name="itemContext", Type=FlexJamType.Int32)]
		public int ItemContext
		{
			get;
			set;
		}

		[DataMember(Name="quantity")]
		[FlexJamMember(Name="quantity", Type=FlexJamType.Int32)]
		public int Quantity
		{
			get;
			set;
		}

		[DataMember(Name="recordID")]
		[FlexJamMember(Name="recordID", Type=FlexJamType.Int32)]
		public int RecordID
		{
			get;
			set;
		}

		public MobileWorldQuestReward()
		{
			this.FileDataID = 0;
			this.ItemContext = 0;
		}
	}
}