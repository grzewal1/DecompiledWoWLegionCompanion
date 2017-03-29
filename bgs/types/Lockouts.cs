using System;

namespace bgs.types
{
	public struct Lockouts
	{
		public bool loaded;

		public bool loading;

		public bool readingPCI;

		public bool readingGTRI;

		public bool readingCAISI;

		public bool readingGSI;

		public bool parentalControls;

		public bool parentalTimedAccount;

		public int parentalMinutesRemaining;

		public IntPtr day1;

		public IntPtr day2;

		public IntPtr day3;

		public IntPtr day4;

		public IntPtr day5;

		public IntPtr day6;

		public IntPtr day7;

		public bool timedAccount;

		public int minutesRemaining;

		public ulong sessionStartTime;

		public bool CAISactive;

		public int CAISplayed;

		public int CAISrested;
	}
}