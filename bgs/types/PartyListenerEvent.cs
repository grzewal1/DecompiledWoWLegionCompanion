using bgs;
using System;

namespace bgs.types
{
	public struct PartyListenerEvent
	{
		public PartyListenerEventType Type;

		public bgs.PartyId PartyId;

		public BnetGameAccountId SubjectMemberId;

		public BnetGameAccountId TargetMemberId;

		public uint UintData;

		public ulong UlongData;

		public string StringData;

		public string StringData2;

		public byte[] BlobData;

		public PartyError ToPartyError()
		{
			PartyError partyError = new PartyError()
			{
				IsOperationCallback = this.Type == PartyListenerEventType.OPERATION_CALLBACK,
				DebugContext = this.StringData,
				ErrorCode = this.UintData,
				Feature = (BnetFeature)((int)(this.UlongData >> 32)),
				FeatureEvent = (BnetFeatureEvent)((uint)(this.UlongData & (ulong)-1)),
				PartyId = this.PartyId,
				szPartyType = this.StringData2,
				StringData = this.StringData
			};
			return partyError;
		}

		public enum AttributeChangeEvent_AttrType
		{
			ATTR_TYPE_NULL,
			ATTR_TYPE_LONG,
			ATTR_TYPE_STRING,
			ATTR_TYPE_BLOB
		}
	}
}