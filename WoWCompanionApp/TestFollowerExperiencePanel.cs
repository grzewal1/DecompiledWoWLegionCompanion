using System;
using UnityEngine;

namespace WoWCompanionApp
{
	public class TestFollowerExperiencePanel : MonoBehaviour
	{
		public FollowerExperienceDisplay[] m_testXPDisplay;

		public TestFollowerExperiencePanel()
		{
		}

		public void Test()
		{
			WrapperGarrisonFollower wrapperGarrisonFollower = new WrapperGarrisonFollower();
			WrapperGarrisonFollower wrapperGarrisonFollower1 = new WrapperGarrisonFollower();
			wrapperGarrisonFollower.Quality = 1;
			wrapperGarrisonFollower.FollowerLevel = 108;
			wrapperGarrisonFollower.Xp = 2400;
			wrapperGarrisonFollower.GarrFollowerID = 616;
			wrapperGarrisonFollower1.Quality = 1;
			wrapperGarrisonFollower1.FollowerLevel = 109;
			wrapperGarrisonFollower1.Xp = 124;
			wrapperGarrisonFollower1.GarrFollowerID = 616;
			this.m_testXPDisplay[0].SetFollower(wrapperGarrisonFollower, wrapperGarrisonFollower1, 0f);
			WrapperGarrisonFollower wrapperGarrisonFollower2 = new WrapperGarrisonFollower();
			WrapperGarrisonFollower wrapperGarrisonFollower3 = new WrapperGarrisonFollower();
			wrapperGarrisonFollower2.Quality = 2;
			wrapperGarrisonFollower2.FollowerLevel = 109;
			wrapperGarrisonFollower2.Xp = 2650;
			wrapperGarrisonFollower2.GarrFollowerID = 621;
			wrapperGarrisonFollower3.Quality = 2;
			wrapperGarrisonFollower3.FollowerLevel = 110;
			wrapperGarrisonFollower3.Xp = 777;
			wrapperGarrisonFollower3.GarrFollowerID = 621;
			this.m_testXPDisplay[1].SetFollower(wrapperGarrisonFollower2, wrapperGarrisonFollower3, 0.5f);
			WrapperGarrisonFollower wrapperGarrisonFollower4 = new WrapperGarrisonFollower();
			WrapperGarrisonFollower wrapperGarrisonFollower5 = new WrapperGarrisonFollower();
			wrapperGarrisonFollower4.Quality = 3;
			wrapperGarrisonFollower4.FollowerLevel = 110;
			wrapperGarrisonFollower4.Xp = 57000;
			wrapperGarrisonFollower4.GarrFollowerID = 617;
			wrapperGarrisonFollower5.Quality = 4;
			wrapperGarrisonFollower5.FollowerLevel = 110;
			wrapperGarrisonFollower5.Xp = 0;
			wrapperGarrisonFollower5.GarrFollowerID = 617;
			this.m_testXPDisplay[2].SetFollower(wrapperGarrisonFollower4, wrapperGarrisonFollower5, 1f);
			WrapperGarrisonFollower wrapperGarrisonFollower6 = new WrapperGarrisonFollower();
			WrapperGarrisonFollower wrapperGarrisonFollower7 = new WrapperGarrisonFollower();
			wrapperGarrisonFollower7 = new WrapperGarrisonFollower()
			{
				GarrFollowerID = wrapperGarrisonFollower6.GarrFollowerID,
				Quality = wrapperGarrisonFollower6.Quality,
				Durability = 0
			};
			wrapperGarrisonFollower6.Quality = 3;
			wrapperGarrisonFollower6.FollowerLevel = 110;
			wrapperGarrisonFollower6.Xp = 57000;
			wrapperGarrisonFollower6.GarrFollowerID = 729;
			wrapperGarrisonFollower6.Flags = 8;
			wrapperGarrisonFollower6.Durability = 0;
			this.m_testXPDisplay[3].SetFollower(wrapperGarrisonFollower6, wrapperGarrisonFollower7, 1.5f);
		}
	}
}