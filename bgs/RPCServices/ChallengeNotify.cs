using bgs;
using System;
using System.Runtime.CompilerServices;

namespace bgs.RPCServices
{
	public class ChallengeNotify : ServiceDescriptor
	{
		public const uint CHALLENGE_USER = 1;

		public const uint CHALLENGE_RESULT = 2;

		public const uint CHALLENGE_EXTERNAL_REQUEST = 3;

		public const uint CHALLENGE_EXTERNAL_REQUEST_RESULT = 4;

		public ChallengeNotify() : base("bnet.protocol.challenge.ChallengeNotify")
		{
			unsafe
			{
				this.Methods = new MethodDescriptor[] { null, new MethodDescriptor("bnet.protocol.challenge.ChallengeNotify.ChallengeUser", 1, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ChallengeUserRequest>)), new MethodDescriptor("bnet.protocol.challenge.ChallengeNotify.ChallengeResult", 2, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ChallengeResultRequest>)), new MethodDescriptor("bnet.protocol.challenge.ChallengeNotify.OnExternalChallenge", 3, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ChallengeExternalRequest>)), new MethodDescriptor("bnet.protocol.challenge.ChallengeNotify.OnExternalChallengeResult", 4, new MethodDescriptor.ParseMethod(ProtobufUtil.ParseFromGeneric<ChallengeExternalResult>)) };
			}
		}
	}
}