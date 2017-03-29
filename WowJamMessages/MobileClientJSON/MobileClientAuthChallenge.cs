using JamLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[DataContract]
	[FlexJamMessage(Id=4883, Name="MobileClientAuthChallenge", Version=39869590)]
	public class MobileClientAuthChallenge
	{
		[DataMember(Name="serverChallenge")]
		[FlexJamMember(ArrayDimensions=1, Name="serverChallenge", Type=FlexJamType.UInt8)]
		public byte[] ServerChallenge
		{
			get;
			set;
		}

		public MobileClientAuthChallenge()
		{
			unsafe
			{
				this.ServerChallenge = new byte[16];
			}
		}
	}
}