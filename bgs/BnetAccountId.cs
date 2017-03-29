using bgs.types;
using System;

namespace bgs
{
	public class BnetAccountId : BnetEntityId
	{
		public BnetAccountId()
		{
		}

		public new BnetAccountId Clone()
		{
			return (BnetAccountId)base.Clone();
		}

		public static BnetAccountId CreateFromBnetEntityId(BnetEntityId src)
		{
			BnetAccountId bnetAccountId = new BnetAccountId();
			bnetAccountId.CopyFrom(src);
			return bnetAccountId;
		}

		public static new BnetAccountId CreateFromEntityId(EntityId src)
		{
			BnetAccountId bnetAccountId = new BnetAccountId();
			bnetAccountId.CopyFrom(src);
			return bnetAccountId;
		}
	}
}