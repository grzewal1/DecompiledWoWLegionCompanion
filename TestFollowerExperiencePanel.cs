using System;
using UnityEngine;
using WowJamMessages;

public class TestFollowerExperiencePanel : MonoBehaviour
{
	public FollowerExperienceDisplay[] m_testXPDisplay;

	public TestFollowerExperiencePanel()
	{
	}

	public void Test()
	{
		JamGarrisonFollower jamGarrisonFollower = new JamGarrisonFollower();
		JamGarrisonFollower jamGarrisonFollower1 = new JamGarrisonFollower();
		jamGarrisonFollower.Quality = 1;
		jamGarrisonFollower.FollowerLevel = 108;
		jamGarrisonFollower.Xp = 2400;
		jamGarrisonFollower.GarrFollowerID = 616;
		jamGarrisonFollower1.Quality = 1;
		jamGarrisonFollower1.FollowerLevel = 109;
		jamGarrisonFollower1.Xp = 124;
		jamGarrisonFollower1.GarrFollowerID = 616;
		this.m_testXPDisplay[0].SetFollower(jamGarrisonFollower, jamGarrisonFollower1, 0f);
		JamGarrisonFollower jamGarrisonFollower2 = new JamGarrisonFollower();
		JamGarrisonFollower jamGarrisonFollower3 = new JamGarrisonFollower();
		jamGarrisonFollower2.Quality = 2;
		jamGarrisonFollower2.FollowerLevel = 109;
		jamGarrisonFollower2.Xp = 2650;
		jamGarrisonFollower2.GarrFollowerID = 621;
		jamGarrisonFollower3.Quality = 2;
		jamGarrisonFollower3.FollowerLevel = 110;
		jamGarrisonFollower3.Xp = 777;
		jamGarrisonFollower3.GarrFollowerID = 621;
		this.m_testXPDisplay[1].SetFollower(jamGarrisonFollower2, jamGarrisonFollower3, 0.5f);
		JamGarrisonFollower jamGarrisonFollower4 = new JamGarrisonFollower();
		JamGarrisonFollower jamGarrisonFollower5 = new JamGarrisonFollower();
		jamGarrisonFollower4.Quality = 3;
		jamGarrisonFollower4.FollowerLevel = 110;
		jamGarrisonFollower4.Xp = 57000;
		jamGarrisonFollower4.GarrFollowerID = 617;
		jamGarrisonFollower5.Quality = 4;
		jamGarrisonFollower5.FollowerLevel = 110;
		jamGarrisonFollower5.Xp = 0;
		jamGarrisonFollower5.GarrFollowerID = 617;
		this.m_testXPDisplay[2].SetFollower(jamGarrisonFollower4, jamGarrisonFollower5, 1f);
		JamGarrisonFollower jamGarrisonFollower6 = new JamGarrisonFollower()
		{
			Quality = 3,
			FollowerLevel = 110,
			Xp = 57000,
			GarrFollowerID = 729,
			Flags = 8,
			Durability = 0
		};
		this.m_testXPDisplay[3].SetFollower(jamGarrisonFollower6, null, 1.5f);
	}
}