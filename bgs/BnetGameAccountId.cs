using bgs.types;
using bnet.protocol;
using System;

namespace bgs
{
	public class BnetGameAccountId : BnetEntityId
	{
		public BnetGameAccountId()
		{
		}

		public new BnetGameAccountId Clone()
		{
			return (BnetGameAccountId)base.Clone();
		}

		public static new BnetGameAccountId CreateFromEntityId(bgs.types.EntityId src)
		{
			BnetGameAccountId bnetGameAccountId = new BnetGameAccountId();
			bnetGameAccountId.CopyFrom(src);
			return bnetGameAccountId;
		}

		public static new BnetGameAccountId CreateFromProtocol(bnet.protocol.EntityId src)
		{
			BnetGameAccountId bnetGameAccountId = new BnetGameAccountId();
			bnetGameAccountId.SetLo(src.Low);
			bnetGameAccountId.SetHi(src.High);
			return bnetGameAccountId;
		}
	}
}