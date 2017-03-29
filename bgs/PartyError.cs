using System;

namespace bgs
{
	public struct PartyError
	{
		public bool IsOperationCallback;

		public string DebugContext;

		public Error ErrorCode;

		public BnetFeature Feature;

		public BnetFeatureEvent FeatureEvent;

		public bgs.PartyId PartyId;

		public string szPartyType;

		public string StringData;

		public bgs.PartyType PartyType
		{
			get
			{
				return BnetParty.GetPartyTypeFromString(this.szPartyType);
			}
		}
	}
}