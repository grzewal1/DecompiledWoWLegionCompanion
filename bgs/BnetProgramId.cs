using System;

namespace bgs
{
	[Serializable]
	public class BnetProgramId : FourCC
	{
		public readonly static BnetProgramId HEARTHSTONE;

		public readonly static BnetProgramId WOW;

		public readonly static BnetProgramId DIABLO3;

		public readonly static BnetProgramId STARCRAFT2;

		public readonly static BnetProgramId BNET;

		public readonly static BnetProgramId PHOENIX;

		public readonly static BnetProgramId PHOENIX_OLD;

		public readonly static BnetProgramId HEROES;

		public readonly static BnetProgramId OVERWATCH;

		private readonly static Map<BnetProgramId, string> s_textureNameMap;

		private readonly static Map<BnetProgramId, string> s_nameStringTagMap;

		static BnetProgramId()
		{
			BnetProgramId.HEARTHSTONE = new BnetProgramId("WTCG");
			BnetProgramId.WOW = new BnetProgramId("WoW");
			BnetProgramId.DIABLO3 = new BnetProgramId("D3");
			BnetProgramId.STARCRAFT2 = new BnetProgramId("S2");
			BnetProgramId.BNET = new BnetProgramId("BN");
			BnetProgramId.PHOENIX = new BnetProgramId("App");
			BnetProgramId.PHOENIX_OLD = new BnetProgramId("CLNT");
			BnetProgramId.HEROES = new BnetProgramId("Hero");
			BnetProgramId.OVERWATCH = new BnetProgramId("Pro");
			Map<BnetProgramId, string> bnetProgramIds = new Map<BnetProgramId, string>()
			{
				{ BnetProgramId.HEARTHSTONE, "HS" },
				{ BnetProgramId.WOW, "WOW" },
				{ BnetProgramId.DIABLO3, "D3" },
				{ BnetProgramId.STARCRAFT2, "SC2" },
				{ BnetProgramId.PHOENIX, "BN" },
				{ BnetProgramId.PHOENIX_OLD, "BN" },
				{ BnetProgramId.HEROES, "Heroes" },
				{ BnetProgramId.OVERWATCH, "Overwatch" }
			};
			BnetProgramId.s_textureNameMap = bnetProgramIds;
			bnetProgramIds = new Map<BnetProgramId, string>()
			{
				{ BnetProgramId.HEARTHSTONE, "GLOBAL_PROGRAMNAME_HEARTHSTONE" },
				{ BnetProgramId.WOW, "GLOBAL_PROGRAMNAME_WOW" },
				{ BnetProgramId.DIABLO3, "GLOBAL_PROGRAMNAME_DIABLO3" },
				{ BnetProgramId.STARCRAFT2, "GLOBAL_PROGRAMNAME_STARCRAFT2" },
				{ BnetProgramId.PHOENIX, "GLOBAL_PROGRAMNAME_PHOENIX" },
				{ BnetProgramId.PHOENIX_OLD, "GLOBAL_PROGRAMNAME_PHOENIX" },
				{ BnetProgramId.HEROES, "GLOBAL_PROGRAMNAME_HEROES" },
				{ BnetProgramId.OVERWATCH, "GLOBAL_PROGRAMNAME_OVERWATCH" }
			};
			BnetProgramId.s_nameStringTagMap = bnetProgramIds;
		}

		public BnetProgramId()
		{
		}

		public BnetProgramId(uint val) : base(val)
		{
		}

		public BnetProgramId(string stringVal) : base(stringVal)
		{
		}

		public new BnetProgramId Clone()
		{
			return (BnetProgramId)base.MemberwiseClone();
		}

		public static string GetNameTag(BnetProgramId programId)
		{
			if (programId == null)
			{
				return null;
			}
			string str = null;
			BnetProgramId.s_nameStringTagMap.TryGetValue(programId, out str);
			return str;
		}

		public static string GetTextureName(BnetProgramId programId)
		{
			if (programId == null)
			{
				return null;
			}
			string str = null;
			BnetProgramId.s_textureNameMap.TryGetValue(programId, out str);
			return str;
		}

		public bool IsGame()
		{
			return (this == BnetProgramId.PHOENIX ? false : this != BnetProgramId.PHOENIX_OLD);
		}

		public bool IsPhoenix()
		{
			return (this == BnetProgramId.PHOENIX ? true : this == BnetProgramId.PHOENIX_OLD);
		}
	}
}