using JamLib;
using System;
using System.Runtime.Serialization;

namespace WowJamMessages.MobileClientJSON
{
	[FlexJamMessage(Id = 4872, Name = "MobileClientAuthChallenge", Version = 33577221u), DataContract]
	public class MobileClientAuthChallenge
	{
		[FlexJamMember(ArrayDimensions = 1, Name = "serverChallenge", Type = FlexJamType.UInt8), DataMember(Name = "serverChallenge")]
		public byte[] ServerChallenge
		{
			get;
			set;
		}

		public MobileClientAuthChallenge()
		{
			this.ServerChallenge = new byte[16];
		}
	}
}
